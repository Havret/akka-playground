using Akka;
using Akka.Actor;
using Akka.Event;
using FluentValidation;
using Infrastructure;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using InventoryManagement.Contact.Query;
using System;

namespace InventoryManagement.Domain.Book.Validation
{
    public class BookValidationProxyActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly IActorRef _bookTitleGuard;
        private readonly IActorRef _bookActor;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public BookValidationProxyActor(IActorRef bookTitleGuard)
        {
            _bookTitleGuard = bookTitleGuard;
            _bookActor = Context.ActorOf(Props.Create<BookActor>(), Context.Self.Path.Name);
            Become(Idle);
        }

        public IStash Stash { get; set; }

        private void Idle()
        {
            Receive<CreateBook>(createBook =>
            {
                _bookTitleGuard.Tell(new BookNameGuardActor.CheckBookTitleAvailability(createBook.Title));
                _bookActor.Tell(GetBook.Instance);

                Become(Validate(createBook, new CreateBookValidator(), FillValidator));

                Case FillValidator(Case match, CreateBookValidator validator) => match
                    .With<BookNameGuardActor.BookTitleAvailability>(x => validator.BookTitleAvailable = x.IsAvailable)
                    .With<BookDto>(x => validator.Book = x);
            });

            Receive<AddTag>(addTag =>
            {
                _bookActor.Tell(GetBook.Instance);

                Become(Validate(addTag, new AddTagValidator(), FillValidator));

                Case FillValidator(Case match, AddTagValidator validator) => match
                    .With<BookDto>(x => validator.Book = x);
            });

            Receive<RemoveTag>(removeTag =>
            {
                _bookActor.Tell(GetBook.Instance);

                Become(Validate(removeTag, new RemoveTagValidator(), FillValidator));

                Case FillValidator(Case match, RemoveTagValidator validator) => match
                    .With<BookDto>(x => validator.Book = x);
            });

            ReceiveAny(Forward);
        }

        private UntypedReceive Validate<TCommand, TValidator>(TCommand command, TValidator validator,
            Func<Case, TValidator, Case> fillValidator)
            where TValidator : IValidator<TCommand>, IDeferredValidator
            where TCommand : ICommand => message =>
        {
            fillValidator(message.Match(), validator)
                .With<ICommand>(_ => Stash.Stash())
                .Default(Forward);

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
        };

        private void Forward(object message) => _bookActor.Forward(message);

    }
}