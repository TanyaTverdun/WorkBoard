using MediatR;

namespace WorkBoard.Application.Features.Sections.Commands.CreateSection;

public record CreateSectionCommand(
    Guid BoardId, 
    string Name) : IRequest<Guid>;
