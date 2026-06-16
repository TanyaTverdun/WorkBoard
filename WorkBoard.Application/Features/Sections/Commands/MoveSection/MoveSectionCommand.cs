using MediatR;

namespace WorkBoard.Application.Features.Sections.Commands.MoveSection;

public record MoveSectionCommand(
    Guid BoardId, 
    Guid SectionId, 
    double NewPosition) : IRequest<Unit>;
