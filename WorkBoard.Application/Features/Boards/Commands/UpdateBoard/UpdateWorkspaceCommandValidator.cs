using FluentValidation;

namespace WorkBoard.Application.Features.Boards.Commands.UpdateBoard;

public class UpdateWorkspaceCommandValidator 
    : AbstractValidator<UpdateBoardCommand>
{
    public UpdateWorkspaceCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
                .WithMessage("Board name is required.")
            .MaximumLength(50)
                .WithMessage("Board name must not exceed 50 characters.");
    }
}
