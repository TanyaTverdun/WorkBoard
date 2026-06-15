CREATE TABLE [BoardMembers] (
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [BoardId]   UNIQUEIDENTIFIER NOT NULL,
    [UserRole]  TINYINT          NOT NULL DEFAULT 1,

    CONSTRAINT [PK_BoardMembers] 
        PRIMARY KEY CLUSTERED ([UserId], [BoardId]),

    CONSTRAINT [FK_BoardMembers_Users] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users]([UserId])
        ON DELETE CASCADE,

    CONSTRAINT [FK_BoardMembers_Boards] 
        FOREIGN KEY ([BoardId]) 
        REFERENCES [Boards]([BoardId]) 
        ON DELETE CASCADE
);