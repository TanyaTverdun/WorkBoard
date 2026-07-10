using MediatR;
using WorkBoard.Application.Common.Dtos.ActivityLogs;

namespace WorkBoard.Application.Features.ActivityLogs.Queries.GetActivityLogsByCard;

public record GetActivityLogsByCardQuery(Guid CardId) 
    : IRequest<IReadOnlyList<ActivityLogDto>>;
