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
                    return new Tagged(bookCreated, new[] { nameof(BookCreated) });
                default:
                    return evt;
            }
        }
    }
}