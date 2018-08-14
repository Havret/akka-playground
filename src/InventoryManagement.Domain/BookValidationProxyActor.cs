using Akka;
using Akka.Actor;
using Akka.Event;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Query;

namespace InventoryManagement.Domain
{
    public class BookValidationProxyActor : ReceiveActor
    {
        private readonly IActorRef _bookNameGuard;
        private readonly IActorRef _bookActor;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public BookValidationProxyActor(IActorRef bookNameGuard)
        {
            _bookNameGuard = bookNameGuard;
            _bookActor = Context.ActorOf(Props.Create<BookActor>(), Context.Self.Path.Name);
            Become(Idle);
        }

        private void Idle()
        {
            Receive<CreateBook>(createBook =>
            {
                _bookNameGuard.Tell(new BookNameGuardActor.CheckBookTitleAvailability(createBook.Title));
                _bookActor.Tell(GetBook.Instance);
                Become(ValidateCreateBook(createBook));
            });

            ReceiveAny(Forward);
        }

        private UntypedReceive ValidateCreateBook(CreateBook command,
            BookNameGuardActor.BookTitleAvailability bookTitleAvailability = null, BookDto book = null) => message =>
        {
            message.Match()
                .With<BookNameGuardActor.BookTitleAvailability>(x => bookTitleAvailability = x)
                .With<BookDto>(x => book = x)
                .Default(Forward);

            if (bookTitleAvailability == null || book == null)
                return;

            var result = new CreateBookValidator
            {
                BookTitleAvailable = bookTitleAvailability.IsAvailable,
                Book = book
            }
                .Validate(command);
            if (result.IsValid)
                Forward(command);
            else
                _logger.Info(result.ToString());

            Become(Idle);
        };

        private void Forward(object message)
        {
            _bookActor.Forward(message);
        }
    }
}