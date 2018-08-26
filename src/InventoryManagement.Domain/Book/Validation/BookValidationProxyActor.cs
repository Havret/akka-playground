using Akka;
using Akka.Actor;
using Akka.Event;
using FluentValidation;
using Infrastructure;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Query;

namespace InventoryManagement.Domain.Book.Validation
{
    public class BookValidationProxyActor : ReceiveActor, IWithUnboundedStash
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

        public IStash Stash { get; set; }

        private void Idle()
        {
            Receive<CreateBook>(createBook =>
            {
                _bookNameGuard.Tell(new BookNameGuardActor.CheckBookTitleAvailability(createBook.Title));
                _bookActor.Tell(GetBook.Instance);
                Become(ValidateCreateBook(createBook, new CreateBookValidator()));
            });

            Receive<AddTag>(addTag =>
            {
                _bookActor.Tell(GetBook.Instance);
                Become(ValidateAddTag(addTag, new AddTagValidator()));
            });

            Receive<RemoveTag>(removeTag =>
            {
                _bookActor.Tell(GetBook.Instance);
                Become(ValidateRemoveTag(removeTag, new RemoveTagValidator()));
            });

            ReceiveAny(Forward);
        }

        private UntypedReceive ValidateCreateBook(CreateBook command, CreateBookValidator validator) => message =>
         {
             message.Match()
                 .With<BookNameGuardActor.BookTitleAvailability>(x => validator.BookTitleAvailable = x.IsAvailable)
                 .With<BookDto>(x => validator.Book = x)
                 .With<ICommand>(_ => Stash.Stash())
                 .Default(Forward);

             Validate(command, validator);
         };

        private UntypedReceive ValidateAddTag(AddTag command, AddTagValidator validator) => message =>
        {
            message.Match()
                .With<BookDto>(x => validator.Book = x)
                .With<ICommand>(_ => Stash.Stash())
                .Default(Forward);

            Validate(command, validator);
        };

        private UntypedReceive ValidateRemoveTag(RemoveTag command, RemoveTagValidator validator) => message =>
        {
            message.Match()
                .With<BookDto>(x => validator.Book = x)
                .With<ICommand>(_ => Stash.Stash())
                .Default(Forward);

            Validate(command, validator);
        };

        private void Validate<TCommand, TValidator>(TCommand command, TValidator validator) where TValidator : IValidator<TCommand>, IDeferredValidator where TCommand: ICommand
        {
            if (validator.IsReady)
            {
                var result = validator.Validate(command);
                if (result.IsValid)
                    Forward(command);
                else
                    _logger.Info(result.ToString());

                Stash.UnstashAll();
                Become(Idle);
            }
        }

        private void Forward(object message) => _bookActor.Forward(message);
        
    }
}