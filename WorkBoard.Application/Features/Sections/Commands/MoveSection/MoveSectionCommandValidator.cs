using FluentValidation;

namespace WorkBoard.Application.Features.Sections.Commands.MoveSection;

public class MoveSectionCommandValidator : AbstractValidator<MoveSectionCommand>
{
    public MoveSectionCommandValidator()
    {
        RuleFor(x => x.NewPosition)
            .GreaterThan(0)
            .WithMessage("Position must be a positive number.");
    }
}
