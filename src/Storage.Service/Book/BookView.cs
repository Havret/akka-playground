using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Query;
using MongoDB.Driver;

namespace Storage.Service.Book
{
    public class BookView : ReceiveActor
    {
        public BookView()
        {
            var storageContext = new StorageContext();

            Receive<GetBooks>(getBooks =>
            {
                storageContext.Books.FindAsync(FilterDefinition<BookReadModel>.Empty).PipeTo(recipient: Self, sender: Sender);
            });

            Receive<IAsyncCursor<BookReadModel>>(cursor =>
            {
                BookDto[] bookDtos = cursor.ToList().Select(x => new BookDto(
                    id: x.Id,
                    title: x.Title,
                    author: x.Author,
                    cost: x.Cost,
                    tags: x.Tags
                )).ToArray();

                Sender.Tell(bookDtos);
            });
        }
    }
}