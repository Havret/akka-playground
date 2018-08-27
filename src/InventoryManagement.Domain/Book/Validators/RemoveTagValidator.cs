using System;
using System.Linq;
using Akka.Streams.Util;
using FluentValidation;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;

namespace InventoryManagement.Domain.Book.Validators
{
    public class RemoveTagValidator : AbstractValidator<RemoveTag>, IDeferredValidator
    {
        public RemoveTagValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(x => Book.Value.Id != Guid.Empty).WithMessage(x => $"Book {x.Id} does not exist.");

            RuleFor(x => x.Tag)
                .NotEmpty()
                .Must(x => Book.Value.Tags.Contains(x, StringComparer.CurrentCultureIgnoreCase)).WithMessage(x => $"Cannot remove {x.Tag} from book {x.Id}. Tag not present.");
        }

        public bool IsReady => Book.HasValue;
        
        public Option<BookDto> Book { private get; set; }
    }
}