namespace WorkBoard.Application.Common.Dtos.Subscriptions;

public record EnforceFreePlanLimitsResult(
    List<Guid> DeletedBoardIds,
    List<DeletedSectionInfo> DeletedSections
);
