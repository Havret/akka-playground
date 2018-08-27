using Akka;
using Akka.Actor;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Query;
using InventoryManagement.Domain.Book.Validators;

namespace InventoryManagement.Domain.Book
{
    public class BookValidationActor : ValidationActor
    {
        public BookValidationActor(IActorRef bookTitleGuard)
        {
            InnerActor = Context.ActorOf(Props.Create<BookActor>(), Context.Self.Path.Name);

            Receive<CreateBook>(createBook =>
            {
                bookTitleGuard.Tell(new BookNameGuardActor.CheckBookTitleAvailability(createBook.Title));
                InnerActor.Tell(GetBook.Instance);

                Validate(createBook, new CreateBookValidator(), FillValidator);

                Case FillValidator(Case match, CreateBookValidator validator) => match
                    .With<BookNameGuardActor.BookTitleAvailability>(x => validator.BookTitleAvailable = x.IsAvailable)
                    .With<BookDto>(x => validator.Book = x);
            });

            Receive<AddTag>(addTag =>
            {
                InnerActor.Tell(GetBook.Instance);

                Validate(addTag, new AddTagValidator(), FillValidator);

                Case FillValidator(Case match, AddTagValidator validator) => match
                    .With<BookDto>(x => validator.Book = x);
            });

            Receive<RemoveTag>(removeTag =>
            {
                InnerActor.Tell(GetBook.Instance);

                Validate(removeTag, new RemoveTagValidator(), FillValidator);

                Case FillValidator(Case match, RemoveTagValidator validator) => match
                    .With<BookDto>(x => validator.Book = x);
            });

            ReceiveAny(Forward);
        }

        protected override IActorRef InnerActor { get; }
    }
}