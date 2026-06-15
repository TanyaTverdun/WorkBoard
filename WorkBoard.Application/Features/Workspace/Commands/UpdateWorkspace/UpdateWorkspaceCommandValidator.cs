using FluentValidation;

namespace WorkBoard.Application.Features.Workspace.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommandValidator 
    : AbstractValidator<UpdateWorkspaceCommand>
{
    public UpdateWorkspaceCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
                .WithMessage("Workspace name is required.")
            .MaximumLength(50)
                .WithMessage("Workspace name must not exceed 50 characters.");
    }
}
