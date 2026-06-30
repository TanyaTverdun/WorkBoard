using FluentValidation;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDescription;

public class UpdateCardDescriptionCommandValidator 
    : AbstractValidator<UpdateCardDescriptionCommand>
{
    public UpdateCardDescriptionCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .WithMessage("Card description must not exceed 4000 characters.");
    }
}
