using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.BoardMembers;

public record UpdateRoleRequest(
    BoardRole NewRole);
