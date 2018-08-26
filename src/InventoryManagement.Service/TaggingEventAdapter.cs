using Akka.Persistence.Journal;
using InventoryManagement.Contact.Events;

namespace InventoryManagement.Service
{
    public class TaggingEventAdapter : IWriteEventAdapter
    {
        public string Manifest(object evt)
        {
            return string.Empty;
        }

        public object ToJournal(object evt)
        {
            switch (evt)
            {
                case BookCreated _:
                    return new Tagged(evt, new[] { nameof(BookCreated), "book" });
                case TagAdded _:
                case TagRemoved _:
                    return new Tagged(evt, new[] { "book" });
                default:
                    return evt;
            }
        }
    }
}