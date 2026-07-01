using FluentValidation;

namespace WorkBoard.Application.Features.Labels.Commands.UpdateLabel;

public class UpdateLabelCommandValidator : AbstractValidator<UpdateLabelCommand>
{
    public UpdateLabelCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.Color)
            .MaximumLength(9)
            .WithMessage("Color must not exceed 9 characters.")
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$")
            .When(v => !string.IsNullOrEmpty(v.Color))
            .WithMessage("Color must be a valid HEX format (#172B4D or #172B4D80).");
    }
}
