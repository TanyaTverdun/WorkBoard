CREATE TABLE [WorkspaceMembers] (
    [UserId]            UNIQUEIDENTIFIER    NOT NULL,
    [WorkspaceId]       UNIQUEIDENTIFIER    NOT NULL,
    [UserRole]          TINYINT             NOT NULL DEFAULT 1,

    CONSTRAINT [PK_WorkspaceMembers] 
        PRIMARY KEY CLUSTERED ([UserId], [WorkspaceId]),
    
    CONSTRAINT [FK_WorkspaceMembers_Users] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_WorkspaceMembers_Workspaces] 
        FOREIGN KEY ([WorkspaceId]) 
        REFERENCES [Workspaces]([WorkspaceId]) 
        ON DELETE CASCADE
);