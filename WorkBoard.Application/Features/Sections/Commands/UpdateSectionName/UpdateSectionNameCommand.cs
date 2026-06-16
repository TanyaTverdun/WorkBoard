using MediatR;

namespace WorkBoard.Application.Features.Sections.Commands.UpdateSectionName;

public record UpdateSectionNameCommand(
    Guid BoardId, 
    Guid SectionId, 
    string Name) : IRequest<Unit>;
