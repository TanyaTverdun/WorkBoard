using FluentValidation;

namespace WorkBoard.Application.Features.Workspace.Commands.UpdateWorkspaceMemberRole;

public class UpdateWorkspaceMemberRoleCommandValidator
    : AbstractValidator<UpdateWorkspaceMemberRoleCommand>
{
    public UpdateWorkspaceMemberRoleCommandValidator()
    {
        RuleFor(v => v.NewRole)
            .IsInEnum()
            .WithMessage(
                "The specified role is not valid. " +
                "It must be an existing workspace role.");
    }
}
