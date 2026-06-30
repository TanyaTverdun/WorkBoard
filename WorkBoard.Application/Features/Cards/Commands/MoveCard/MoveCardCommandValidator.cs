using FluentValidation;

namespace WorkBoard.Application.Features.Cards.Commands.MoveCard;

public class MoveCardCommandValidator : AbstractValidator<MoveCardCommand>
{
    public MoveCardCommandValidator()
    {
        RuleFor(x => x.NewPosition)
            .GreaterThanOrEqualTo(0);
    }
}
