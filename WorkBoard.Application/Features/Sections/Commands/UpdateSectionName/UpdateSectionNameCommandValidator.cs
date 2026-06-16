using FluentValidation;

namespace WorkBoard.Application.Features.Sections.Commands.UpdateSectionName;

public class UpdateSectionNameCommandValidator 
    : AbstractValidator<UpdateSectionNameCommand>
{
    public UpdateSectionNameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Section name cannot be empty.")
            .MaximumLength(50)
            .WithMessage("Section name must not exceed 50 characters.");
    }
}
