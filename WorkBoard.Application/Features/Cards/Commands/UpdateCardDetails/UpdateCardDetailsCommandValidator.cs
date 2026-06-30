using FluentValidation;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDetails;

public class UpdateCardDetailsCommandValidator : AbstractValidator<UpdateCardDetailsCommand>
{
    public UpdateCardDetailsCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty if provided.")
            .MaximumLength(100)
            .WithMessage("Title must not exceed 100 characters.")
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .WithMessage("Description is too long.")
            .When(x => x.IsDescriptionUpdated && x.Description != null);
    }
}
