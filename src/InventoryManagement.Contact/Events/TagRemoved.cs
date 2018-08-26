using System;
using Infrastructure;

namespace InventoryManagement.Contact.Events
{
    public class TagRemoved : IDomainEvent
    {
        public TagRemoved(Guid id, string tag)
        {
            Id = id;
            Tag = tag;
        }

        public Guid Id { get; }
        public string Tag { get; }

        public override string ToString()
        {
            return $"[{nameof(TagRemoved)}] {nameof(Id)}: {Id}, {nameof(Tag)}: {Tag}";
        }
    }
}