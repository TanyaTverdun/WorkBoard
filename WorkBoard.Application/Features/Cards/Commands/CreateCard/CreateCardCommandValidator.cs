using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkBoard.Application.Features.Cards.Commands.CreateCard
{
    public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
    {
        public CreateCardCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Card title is required.")
                .MaximumLength(100)
                .WithMessage("Card title must not exceed 100 characters.");

            RuleFor(x => x.Position)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Position must be greater than or equal to 0.");
        }
    }
}
