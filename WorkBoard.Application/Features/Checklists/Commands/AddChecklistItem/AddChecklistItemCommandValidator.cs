using FluentValidation;

namespace WorkBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public class AddChecklistItemCommandValidator 
    : AbstractValidator<AddChecklistItemCommand>
{
    public AddChecklistItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Item title is required.")
            .MaximumLength(200)
            .WithMessage("Item title must not exceed 200 characters.");
    }
}
