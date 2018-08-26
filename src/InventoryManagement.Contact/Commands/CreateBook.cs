using Infrastructure;
using System;
using System.Collections.Generic;

namespace InventoryManagement.Contact.Commands
{
    public class CreateBook : ICommand
    {
        public CreateBook(string title, string author, IReadOnlyList<string> tags, decimal cost, int inventoryAmount)
        {
            Title = title;
            Author = author;
            Tags = tags;
            Cost = cost;
            InventoryAmount = inventoryAmount;
        }

        public Guid Id { get; set; }
        public string Title { get; }
        public string Author { get; }
        public IReadOnlyList<string> Tags { get; }
        public decimal Cost { get; }
        public int InventoryAmount { get; }

        public CreateBook WithId(Guid id)
        {
            var clone = (CreateBook)MemberwiseClone();
            clone.Id = id;
            return clone;
        }
    }
}