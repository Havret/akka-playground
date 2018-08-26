using Akka.Streams.Util;
using FluentValidation;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using System;
using System.Linq;

namespace InventoryManagement.Domain.Book.Validation
{
    public class AddTagValidator : AbstractValidator<AddTag>
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