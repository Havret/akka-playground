using System;
using Akka.Streams.Util;
using FluentValidation;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;

namespace InventoryManagement.Domain.Book.Validation
{
    public class CreateBookValidator : AbstractValidator<CreateBook>, IDeferredValidator
    {
        public CreateBookValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(_ => Book.Value.Id == Guid.Empty).WithMessage("Book already exists.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Must(_ => BookTitleAvailable.Value).WithMessage("Title already exists.");

            RuleFor(x => x.Author).NotEmpty();
            RuleFor(x => x.InventoryAmount).Must(x => x >= 0);
            RuleFor(x => x.Cost).Must(x => x > 0);
        }

        public bool IsReady => BookTitleAvailable.HasValue && Book.HasValue;
        
        public Option<bool> BookTitleAvailable { private get; set; }
        public Option<BookDto> Book { private get; set; }
    }
}