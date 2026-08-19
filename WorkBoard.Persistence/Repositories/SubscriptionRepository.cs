using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Subscriptions;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Persistence.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    internal SubscriptionRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<EnforceFreePlanLimitsResult> EnforceFreePlanLimitsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                WorkspaceId, 
                ROW_NUMBER() OVER(
                    ORDER BY 
                        CreatedAt ASC
                ) AS rnk
            INTO 
                #RankedWorkspaces
            FROM 
                Workspaces 
            WHERE 
                CreatedBy = @UserId;

            SELECT 
                b.BoardId, 
                b.WorkspaceId, 
                ROW_NUMBER() OVER(
                    PARTITION BY 
                        b.WorkspaceId 
                    ORDER BY 
                        b.CreatedAt ASC
                ) AS rnk
            INTO 
                #RankedBoards
            FROM 
                Boards b
            INNER JOIN 
                Workspaces w 
                ON b.WorkspaceId = w.WorkspaceId
            WHERE 
                w.CreatedBy = @UserId;

            SELECT 
                BoardId 
            INTO 
                #DoomedBoards
            FROM 
                #RankedBoards
            WHERE 
                WorkspaceId IN (
                    SELECT 
                        WorkspaceId 
                    FROM 
                        #RankedWorkspaces WHERE rnk > 1
                )
                OR (
                WorkspaceId IN (
                    SELECT 
                        WorkspaceId 
                    FROM 
                        #RankedWorkspaces 
                    WHERE 
                        rnk = 1
                ) 
                AND 
                    rnk > 5
            );

            SELECT 
                BoardId 
            INTO 
                #SurvivingBoards
            FROM 
                #RankedBoards
            WHERE 
                WorkspaceId IN (
                    SELECT 
                        WorkspaceId 
                    FROM 
                        #RankedWorkspaces 
                    WHERE 
                        rnk = 1
                )
                AND 
                    rnk <= 5;

            WITH RankedSections AS (
                SELECT 
                    s.SectionId, 
                    s.BoardId, 
                    ROW_NUMBER() OVER(PARTITION BY s.BoardId ORDER BY s.CreatedAt ASC) AS rnk
                FROM 
                    Sections s
                INNER JOIN 
                    #SurvivingBoards sb 
                    ON s.BoardId = sb.BoardId
            )
            SELECT 
                BoardId, 
                SectionId 
            INTO 
                #DoomedSections
            FROM 
                RankedSections
            WHERE 
                rnk > 10;

            SELECT 
                BoardId 
            FROM 
                #DoomedBoards;
            SELECT 
                BoardId, 
                SectionId 
            FROM 
                #DoomedSections;

            DELETE FROM 
                Sections 
            WHERE 
                SectionId IN (
                    SELECT 
                        SectionId 
                    FROM 
                        #DoomedSections
                );

            DELETE FROM 
                Boards 
            WHERE 
                BoardId IN (
                    SELECT 
                        BoardId 
                    FROM 
                        #DoomedBoards
                );

            DELETE FROM 
                Workspaces 
            WHERE 
                WorkspaceId IN (
                    SELECT 
                        WorkspaceId 
                    FROM 
                        #RankedWorkspaces 
                    WHERE 
                        rnk > 1
                );

            DROP TABLE #RankedWorkspaces;
            DROP TABLE #RankedBoards;
            DROP TABLE #DoomedBoards;
            DROP TABLE #SurvivingBoards;
            DROP TABLE #DoomedSections;
        ";

        using var multi = await _connection.QueryMultipleAsync(
            sql,
            new 
            { 
                UserId = userId 
            },
            transaction: _transaction);

        var deletedBoardIds = (await multi.ReadAsync<Guid>()).ToList();
        var deletedSections = (await multi.ReadAsync<DeletedSectionInfo>()).ToList();

        return new EnforceFreePlanLimitsResult(deletedBoardIds, deletedSections);
    }
}
