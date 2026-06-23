using FluentValidation;

namespace WorkBoard.Application.Features.Boards.Commands.AddBoardMember;

public class AddBoardMemberCommandValidator 
    : AbstractValidator<AddBoardMemberCommand>
{
    public AddBoardMemberCommandValidator()
    {
        RuleFor(v => v.Role)
            .IsInEnum()
            .WithMessage("Invalid role specified.");
    }
}
