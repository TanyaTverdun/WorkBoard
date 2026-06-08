CREATE TABLE [Workspaces] (
    [WorkspaceId]       UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY,
    [Name]              NVARCHAR(50)        NOT NULL,
    [SubscriptionTier]  TINYINT             NOT NULL DEFAULT 0,
    [CreatedAt]         DATETIME2           NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER    NOT NULL,
    [UpdatedAt]         DATETIME2           NULL DEFAULT NULL,
    [UpdatedBy]         UNIQUEIDENTIFIER    NULL DEFAULT NULL,
    
    CONSTRAINT [FK_Workspaces_CreatedBy_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Workspaces_UpdatedBy_Users] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([UserId])
);