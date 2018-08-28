using Akka.Actor;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Query;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Service.Book
{
    public class BookView : ReceiveActor
    {
        private readonly StorageContext _storageContext;

        public BookView()
        {
            _storageContext = new StorageContext();

            Receive<GetBooks>(query =>
            {
                var filterDefinition = FilterDefinition<BookReadModel>.Empty;
                if (!string.IsNullOrEmpty(query.Tag))
                    filterDefinition &= new ExpressionFilterDefinition<BookReadModel>(x => x.Tags.Contains(query.Tag));

                FindBooks(filterDefinition).PipeTo(recipient: Sender, sender: Self);
            });
        }

        private async Task<IEnumerable<BookDto>> FindBooks(FilterDefinition<BookReadModel> filterDefinition)
        {
            var result = await _storageContext.Books.FindAsync(filterDefinition);
            var books = await result.ToListAsync();
            return books.Select(x => new BookDto
            (
                id: x.Id,
                title: x.Title,
                author: x.Author,
                cost: x.Cost,
                tags: x.Tags
            )).ToArray();
        }
    }
}