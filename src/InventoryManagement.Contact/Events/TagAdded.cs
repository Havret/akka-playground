﻿using System;
using Infrastructure;

namespace InventoryManagement.Contact.Events
{
    public class TagAdded : IDomainEvent
    {
        public TagAdded(Guid id, string tag)
        {
            Id = id;
            Tag = tag;
        }

        public Guid Id { get; }
        public string Tag { get; }

        public override string ToString()
        {
            return $"[{nameof(TagAdded)}] {nameof(Id)}: {Id}, {nameof(Tag)}: {Tag}";
        }
    }
}