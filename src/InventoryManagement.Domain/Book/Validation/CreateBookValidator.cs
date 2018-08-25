using System;
using FluentValidation;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;

namespace InventoryManagement.Domain.Book.Validation
{
    public class CreateBookValidator : AbstractValidator<CreateBook>
    {
        public CreateBookValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(_ => Book.Id == Guid.Empty).WithMessage("Book already exists");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Must(_ => BookTitleAvailable).WithMessage("Title already exists.");

            RuleFor(x => x.Author).NotEmpty();
            RuleFor(x => x.InventoryAmount).Must(x => x >= 0);
            RuleFor(x => x.Cost).Must(x => x > 0);
        }
        
        public bool BookTitleAvailable { private get; set; }
        public BookDto Book { private get; set; }
    }
}