using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Dtos.BoardMembers;

public record BoardMemberWithUserDto(
    BoardMember Member,
    User User);
