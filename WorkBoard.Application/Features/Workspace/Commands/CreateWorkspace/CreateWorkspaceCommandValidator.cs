using FluentValidation;

namespace WorkBoard.Application.Features.Workspace.Commands.CreateWorkspace;

public class CreateWorkspaceCommandValidator : 
    AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
                .WithMessage("Workspace name is required.")
            .MaximumLength(50)
                .WithMessage("Workspace name must not exceed 50 characters.");
    }
}
