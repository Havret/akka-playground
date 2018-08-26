using InventoryManagement.Contact.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagement.Domain.Book
{
    public class BookAggregate : ICloneable
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public IReadOnlyList<string> Tags { get; private set; }
        public decimal Cost { get; private set; }
        public int InventoryAmount { get; private set; }

        public void Apply(BookCreated bookCreated)
        {
            Id = bookCreated.Id;
            Title = bookCreated.Title;
            Author = bookCreated.Author;
            Tags = bookCreated.Tags;
            Cost = bookCreated.Cost;
            InventoryAmount = bookCreated.InventoryAmount;
        }

        public void Apply(TagAdded bookCreated)
        {
            Tags = Tags.Concat(new[] { bookCreated.Tag }).ToList();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}