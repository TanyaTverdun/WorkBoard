using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemDeletedDto(
    Guid CardId,
    Guid ChecklistId,
    ChecklistItemDto Item);
