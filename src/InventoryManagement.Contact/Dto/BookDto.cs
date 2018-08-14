using System;
using System.Collections.Generic;

namespace InventoryManagement.Contact.Dto
{
    public class BookDto
    {
        public BookDto(Guid id, string title, string author, IReadOnlyList<string> tags, decimal cost)
        {
            Id = id;
            Title = title;
            Author = author;
            Tags = tags;
            Cost = cost;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Author { get; }
        public IReadOnlyList<string> Tags { get; }
        public decimal Cost { get; }
    }
}