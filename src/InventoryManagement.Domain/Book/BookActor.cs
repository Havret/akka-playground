using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Events;
using InventoryManagement.Contact.Query;

namespace InventoryManagement.Domain.Book
{
    public class BookActor : ReceivePersistentActor
    {
        private readonly BookAggregate _aggregate = new BookAggregate();
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        public override string PersistenceId { get; }

        public BookActor()
        {
            PersistenceId = Context.Self.Path.Name;

            Command<CreateBook>(createBook =>
            {
                var bookCreated = new BookCreated
                (
                    id: createBook.Id,
                    title: createBook.Title,
                    author: createBook.Author,
                    tags: createBook.Tags,
                    cost: createBook.Cost,
                    inventoryAmount: createBook.InventoryAmount
                );

                Persist(bookCreated, _ =>
                {
                    _logger.Info($"Book created: {bookCreated}");
                    _aggregate.Apply(bookCreated);
                });
            });

            Command<GetBook>(getBook =>
            {
                var bookDto = new BookDto
                (
                    id: _aggregate.Id,
                    title: _aggregate.Title,
                    author: _aggregate.Author,
                    tags: _aggregate.Tags,
                    cost: _aggregate.Cost
                );

                Sender.Tell(bookDto);
            });

            Recover<BookCreated>(bookCreated => _aggregate.Apply(bookCreated));
        }
    }
}