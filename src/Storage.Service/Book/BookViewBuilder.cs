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
                var eventsByTag = queries.EventsByTag(nameof(BookCreated), new Sequence(option.Value));
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

                    _storageContext.Books.UpdateOneAsync(x => x.Id == bookCreated.Id, updateDefinition, updateOptions).PipeTo(Sender,
                        Self, () => envelope.Offset, exception => new Status.Failure(exception));
                }));
        }

        protected override void PreStart()
        {
            _resumableProjection.FetchLatestOffset("book-view").PipeTo(Self);
        }
    }
}