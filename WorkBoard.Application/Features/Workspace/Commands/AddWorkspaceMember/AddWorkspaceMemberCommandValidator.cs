using FluentValidation;

namespace WorkBoard.Application.Features.Workspace.Commands.AddWorkspaceMember;

public class AddWorkspaceMemberCommandValidator
    : AbstractValidator<AddWorkspaceMemberCommand>
{
    public AddWorkspaceMemberCommandValidator()
    {
        RuleFor(v => v.Role)
            .IsInEnum()
            .WithMessage("The specified role is not valid.");
    }
}
