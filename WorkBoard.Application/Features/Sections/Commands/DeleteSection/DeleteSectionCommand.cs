using MediatR;

namespace WorkBoard.Application.Features.Sections.Commands.DeleteSection;

public record DeleteSectionCommand(
    Guid BoardId, 
    Guid SectionId) : IRequest<Unit>;
