using FluentValidation;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardTitle;

public class UpdateCardTitleCommandValidator 
    : AbstractValidator<UpdateCardTitleCommand>
{
    public UpdateCardTitleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Card title is required.")
            .MaximumLength(100)
            .WithMessage("Card title must not exceed 100 characters.");
    }
}
