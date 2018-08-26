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
                case BookCreated bookCreated:
                    return new Tagged(bookCreated, new[] { nameof(BookCreated), "book" });
                case TagAdded tagAdded:
                    return new Tagged(tagAdded, new[] { "book" });
                default:
                    return evt;
            }
        }
    }
}