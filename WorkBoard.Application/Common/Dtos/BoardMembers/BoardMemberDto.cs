using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.BoardMembers;

public record BoardMemberDto(
    Guid UserId,
    string FullName,
    string Initials,
    string Email,
    string? AvatarUrl,
    BoardRole UserRole);
