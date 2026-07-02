using FluentValidation;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklist;

public class UpdateChecklistCommandValidator 
    : AbstractValidator<UpdateChecklistCommand>
{
    public UpdateChecklistCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Checklist name is required.")
            .MaximumLength(50)
            .WithMessage("Checklist name must not exceed 50 characters.");
    }
}
