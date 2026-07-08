using MediatR;
using WorkBoard.Application.Common.Dtos.Comments;

namespace WorkBoard.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommand : IRequest<CommentDto>
{
    public Guid CardId { get; set; }
    public required string Text { get; set; }
}
