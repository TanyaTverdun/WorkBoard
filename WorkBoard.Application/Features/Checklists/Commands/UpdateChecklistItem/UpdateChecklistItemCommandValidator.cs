using FluentValidation;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandValidator 
    : AbstractValidator<UpdateChecklistItemCommand>
{
    public UpdateChecklistItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Item title is required.")
            .MaximumLength(200)
            .WithMessage("Item title must not exceed 200 characters.");
    }
}
