using FluentValidation;

namespace WorkBoard.Application.Features.Boards.Commands.UpdateMemberRole;

public class UpdateMemberRoleCommandValidator 
    : AbstractValidator<UpdateMemberRoleCommand>
{
    public UpdateMemberRoleCommandValidator()
    {
        RuleFor(v => v.NewRole)
            .IsInEnum()
            .WithMessage("The specified role is not valid. " +
                "It must be an existing board role.");
    }
}
