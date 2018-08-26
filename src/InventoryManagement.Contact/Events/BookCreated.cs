using Infrastructure;
using System;
using System.Collections.Generic;

namespace InventoryManagement.Contact.Events
{
    public class BookCreated : IDomainEvent
    {
        public BookCreated(Guid id, string title, string author, IReadOnlyList<string> tags, decimal cost, int inventoryAmount)
        {
            Id = id;
            Title = title;
            Author = author;
            Tags = tags;
            Cost = cost;
            InventoryAmount = inventoryAmount;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Author { get; }
        public IReadOnlyList<string> Tags { get; }
        public decimal Cost { get; }
        public int InventoryAmount { get; }

        public override string ToString() =>
            $"[{nameof(BookCreated)}] {nameof(Id)}: {Id}, {nameof(Title)}: {Title}, {nameof(Author)}: {Author}, {nameof(Tags)}: {Tags.ToString()}, {nameof(Cost)}: {Cost}, {nameof(InventoryAmount)}: {InventoryAmount}";
    }
}