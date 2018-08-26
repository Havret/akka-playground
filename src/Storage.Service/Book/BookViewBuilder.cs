using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.Util;
using InventoryManagement.Contact.Events;
using MongoDB.Driver;
using Storage.Service.ResumableProjection;
using System;
using System.Linq.Expressions;

namespace Storage.Service.Book
{
    public class BookViewBuilder : ReceiveActor
    {
        private readonly IResumableProjection _resumableProjection;
        private readonly StorageContext _storageContext;

        public BookViewBuilder(IResumableProjection resumableProjection)
        {
            _resumableProjection = resumableProjection;
            _storageContext = new StorageContext();

            var self = Self;

            Receive<Option<long>>(option =>
            {
                SqlReadJournal queries = PersistenceQuery.Get(Context.System)
                    .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
                var materializer = ActorMaterializer.Create(Context.System);
                var eventsByTag = queries.EventsByTag("book", new Sequence(option.Value));
                eventsByTag
                    .SelectAsync(1, x => self.Ask(x))
                    .Select(x => (Sequence)x)
                    .SelectAsync(1, x => _resumableProjection.StoreLatestOffset("book-view", x.Value))
                    .RunWith(Sink.Ignore<bool>(), materializer);
            });

            Receive<EventEnvelope>(envelope => envelope.Event.Match()
                .With<BookCreated>(bookCreated =>
                {
                    var updateDefinition = new UpdateDefinitionBuilder<BookReadModel>()
                        .SetOnInsert(model => model.Title, bookCreated.Title)
                        .SetOnInsert(model => model.Author, bookCreated.Author)
                        .SetOnInsert(model => model.Tags, bookCreated.Tags)
                        .SetOnInsert(model => model.Cost, bookCreated.Cost)
                        .SetOnInsert(model => model.InventoryAmount, bookCreated.InventoryAmount)
                        .SetOnInsert(model => model.SequenceNr, envelope.SequenceNr);

                    var updateOptions = new UpdateOptions { IsUpsert = true };

                    _storageContext.Books.UpdateOneAsync(x => x.Id == bookCreated.Id, updateDefinition, updateOptions)
                        .PipeTo(Sender,
                            Self, () => envelope.Offset, exception => new Status.Failure(exception));
                })
                .With<TagAdded>(tagAdded =>
                {
                    var updateDefinition = new UpdateDefinitionBuilder<BookReadModel>()
                        .Push(model => model.Tags, tagAdded.Tag)
                        .Set(model => model.SequenceNr, envelope.SequenceNr);

                    _storageContext.Books
                        .FindOneAndUpdateAsync(ShouldBeApplied(tagAdded.Id, envelope.SequenceNr), updateDefinition)
                        .PipeTo(Sender, Self, () => envelope.Offset, exception => new Status.Failure(exception));
                })
            );
        }

        private static Expression<Func<BookReadModel, bool>> ShouldBeApplied(Guid id, long sequenceNr) =>
            x => x.Id == id && x.SequenceNr == sequenceNr - 1;

        protected override void PreStart()
        {
            _resumableProjection.FetchLatestOffset("book-view").PipeTo(Self);
        }
    }
}