using FluentValidation;

namespace WorkBoard.Application.Features.Checklists.Commands.CreateChecklist;

public class CreateChecklistCommandValidator : AbstractValidator<CreateChecklistCommand>
{
    public CreateChecklistCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Checklist name is required.")
            .MaximumLength(50)
            .WithMessage("Checklist name must not exceed 50 characters.");
    }
}
