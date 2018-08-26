using System;
using System.Collections.Generic;

namespace Storage.Service.Book
{
    public class BookReadModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public IReadOnlyList<string> Tags { get; set; }
        public decimal Cost { get; set; }
        public int InventoryAmount { get; set; }
        public long SequenceNr { get; set; }
    }
}