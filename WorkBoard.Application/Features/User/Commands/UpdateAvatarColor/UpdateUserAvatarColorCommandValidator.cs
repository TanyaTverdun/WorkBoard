using FluentValidation;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarColor;

public class UpdateUserAvatarColorCommandValidator 
    : AbstractValidator<UpdateUserAvatarColorCommand>
{
    public UpdateUserAvatarColorCommandValidator()
    {
        RuleFor(v => v.AvatarColor)
            .NotEmpty()
            .WithMessage("Avatar color is required.")
            .MaximumLength(9)
            .WithMessage("Color must not exceed 9 characters.")
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$")
            .WithMessage("Color must be a valid HEX format (e.g., #172B4D or #172B4D80).");
    }
}
