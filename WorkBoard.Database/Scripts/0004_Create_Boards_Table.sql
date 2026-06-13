CREATE TABLE [Boards] (
    [BoardId]         UNIQUEIDENTIFIER   NOT NULL PRIMARY KEY,
    [WorkspaceId]     UNIQUEIDENTIFIER   NOT NULL,
    [Name]            NVARCHAR(50)       NOT NULL,
    [IsArchived]      BIT                NOT NULL DEFAULT 0,
    [ArchiveStatus]   TINYINT            NOT NULL DEFAULT 0,
    [CreatedAt]       DATETIME2          NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER   NOT NULL,
    [UpdatedAt]       DATETIME2          NULL DEFAULT NULL,
    [UpdatedBy]       UNIQUEIDENTIFIER   NULL DEFAULT NULL,

    CONSTRAINT [FK_Boards_Workspaces] 
        FOREIGN KEY ([WorkspaceId]) 
        REFERENCES [Workspaces]([WorkspaceId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Boards_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_Boards_UpdatedBy_Users] 
        FOREIGN KEY ([UpdatedBy]) 
        REFERENCES [Users]([UserId])
);