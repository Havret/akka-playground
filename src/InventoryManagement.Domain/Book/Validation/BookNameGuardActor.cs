using System;
using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using InventoryManagement.Contact.Events;

namespace InventoryManagement.Domain.Book.Validation
{
    public class BookNameGuardActor : ReceiveActor
    {
        private readonly HashSet<string> _bookNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        private readonly HashSet<string> _nameReservations = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        public BookNameGuardActor()
        {
            var queries = PersistenceQuery.Get(Context.System).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
            var materializer = ActorMaterializer.Create(Context.System);
            var eventsByTag = queries.EventsByTag(nameof(BookCreated));

            eventsByTag.RunForeach(envelope => envelope.Event.Match()
                .With<BookCreated>(Self.Tell), materializer);

            Receive<BookCreated>(bookCreated =>
            {
                _bookNames.Add(bookCreated.Title);
                _nameReservations.Remove(bookCreated.Title);
            });

            Receive<CheckBookTitleAvailability>(query =>
            {
                var reserved = _nameReservations.Contains(query.Title);
                if (reserved)
                {
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Self, query, Sender);
                    return;
                }

                var isAvailable = !_bookNames.Contains(query.Title);
                if (isAvailable)
                {
                    _nameReservations.Add(query.Title);
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Self, new ClearReservation(query.Title), Self);
                }

                Sender.Tell(new BookTitleAvailability(query.Title, isAvailable));
            });

            Receive<ClearReservation>(clearReservation => _nameReservations.Remove(clearReservation.Title));
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

        internal class ClearReservation
        {
            public ClearReservation(string title)
            {
                Title = title;
            }

            public string Title { get; }
        }
    }
}