using System;
using System.Linq;
using Akka.Streams.Util;
using FluentValidation;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;

namespace InventoryManagement.Domain.Book.Validators
{
    public class AddTagValidator : AbstractValidator<AddTag>, IDeferredValidator
    {
        public AddTagValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(x => Book.Value.Id != Guid.Empty).WithMessage(x => $"Book {x.Id} does not exist.");

            RuleFor(x => x.Tag)
                .NotEmpty()
                .Must(x => !Book.Value.Tags.Contains(x, StringComparer.CurrentCultureIgnoreCase)).WithMessage(x => $"Book {x.Id} already has tag {x.Tag}.");
        }

        public bool IsReady => Book.HasValue;
        public Option<BookDto> Book { private get; set; }
    }
}