CREATE OR ALTER PROCEDURE sp_EnforceFreePlanLimits
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        WorkspaceId, 
        ROW_NUMBER() OVER (
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
        ROW_NUMBER() OVER (
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
                #RankedWorkspaces 
            WHERE 
                rnk > 1
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
        AND rnk <= 5;

    WITH RankedSections AS (
        SELECT 
            s.SectionId, 
            s.BoardId, 
            ROW_NUMBER() OVER (
                PARTITION BY 
                    s.BoardId 
                ORDER BY 
                    s.CreatedAt ASC
            ) AS rnk
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
END;