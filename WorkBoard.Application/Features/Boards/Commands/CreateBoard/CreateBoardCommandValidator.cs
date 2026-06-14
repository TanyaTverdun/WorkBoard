using FluentValidation;

namespace WorkBoard.Application.Features.Boards.Commands.CreateBoard;

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
                .WithMessage("Board name is required.")
            .MaximumLength(50)
                .WithMessage("Board name must not exceed 50 characters.");
    }
}
