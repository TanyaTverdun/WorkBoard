using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.BoardMembers;

public record AddMemberRequest(
    Guid UserId, 
    BoardRole Role);
