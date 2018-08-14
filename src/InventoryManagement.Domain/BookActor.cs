using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Events;
using InventoryManagement.Contact.Query;
using System;

namespace InventoryManagement.Domain
{
    public class BookActor : ReceivePersistentActor
    {
        private readonly Book _state = new Book();
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        public override string PersistenceId { get; }

        public BookActor()
        {
            PersistenceId = Context.Self.Path.Name;

            Command<CreateBook>(createBook =>
            {
                var bookCreated = new BookCreated
                (
                    id: Guid.Parse(PersistenceId),
                    title: createBook.Title,
                    author: createBook.Author,
                    tags: createBook.Tags,
                    cost: createBook.Cost,
                    inventoryAmount: createBook.InventoryAmount
                );

                Persist(bookCreated, _ =>
                {
                    _logger.Info($"Book created: {bookCreated}");
                    _state.Apply(bookCreated);
                });
            });

            Command<GetBook>(getBook =>
            {
                var bookDto = new BookDto
                (
                    id: _state.Id,
                    title: _state.Title,
                    author: _state.Author,
                    tags: _state.Tags,
                    cost: _state.Cost
                );

                Sender.Tell(bookDto);
            });

            Recover<BookCreated>(bookCreated => _state.Apply(bookCreated));
        }
    }
}