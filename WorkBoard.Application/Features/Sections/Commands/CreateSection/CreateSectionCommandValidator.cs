using FluentValidation;

namespace WorkBoard.Application.Features.Sections.Commands.CreateSection;

public class CreateSectionCommandValidator 
    : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Section name is required.")
            .MaximumLength(50)
            .WithMessage("Section name must not exceed 50 characters.");
    }
}
