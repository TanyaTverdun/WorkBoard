using MediatR;
using WorkBoard.Application.Common.Dtos.Comments;

namespace WorkBoard.Application.Features.Comments.GetCommentsByCard;

public record GetCommentsByCardQuery(Guid CardId)
    : IRequest<IReadOnlyList<CommentDto>>;
