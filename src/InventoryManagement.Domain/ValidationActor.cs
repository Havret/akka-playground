using System;
using Akka;
using Akka.Actor;
using Akka.Event;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure;

namespace InventoryManagement.Domain
{
    public abstract class ValidationActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public IStash Stash { get; set; }
        protected abstract IActorRef InnerActor { get; }

        protected void Validate<TCommand, TValidator>(TCommand command, TValidator validator,
            Func<Case, TValidator, Case> fillValidator)
            where TValidator : IValidator<TCommand>, IDeferredValidator
            where TCommand : ICommand
        {
            BecomeStacked(ValidateImpl(command, validator, fillValidator));
        }

        private UntypedReceive ValidateImpl<TCommand, TValidator>(TCommand command, TValidator validator,
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
                    OnValidationFailure(command, result);

                Stash.UnstashAll();
                UnbecomeStacked();
            }
        };

        protected virtual void OnValidationFailure(object command, ValidationResult result)
        {
            _logger.Info(result.ToString());
        }

        protected void Forward(object message) => InnerActor.Forward(message);

    }
}