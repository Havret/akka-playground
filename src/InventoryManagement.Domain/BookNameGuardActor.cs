using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using InventoryManagement.Contact.Events;
using System;
using System.Collections.Generic;

namespace InventoryManagement.Domain
{
    public class BookNameGuardActor : ReceiveActor
    {
        private readonly HashSet<string> _bookNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        public BookNameGuardActor()
        {
            var queries = PersistenceQuery.Get(Context.System).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
            var materializer = ActorMaterializer.Create(Context.System);
            var eventsByTag = queries.EventsByTag(nameof(BookCreated));

            eventsByTag.RunForeach(envelope =>
            {
                if (envelope.Event is BookCreated bookCreated)
                    _bookNames.Add(bookCreated.Title);
            }, materializer);

            Receive<CheckBookTitleAvailability>(query => Sender.Tell(new BookTitleAvailability(query.Title, !_bookNames.Contains(query.Title))));
        }

        internal class CheckBookTitleAvailability
        {
            public CheckBookTitleAvailability(string title)
            {
                Title = title;
            }

            public string Title { get; }
        }

        internal class BookTitleAvailability
        {
            public BookTitleAvailability(string title, bool isAvailable)
            {
                Title = title;
                IsAvailable = isAvailable;
            }

            public string Title { get; }
            public bool IsAvailable { get; }
        }
    }
}