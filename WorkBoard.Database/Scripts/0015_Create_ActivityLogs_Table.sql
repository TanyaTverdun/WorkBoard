CREATE TABLE [ActivityLogs] (
    [ActivityLogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CardId]        UNIQUEIDENTIFIER NOT NULL,
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [Text]          NVARCHAR(250)    NOT NULL,
    [CreatedAt]     DATETIME2        NOT NULL,

    CONSTRAINT [FK_Activity_logs_Cards] 
        FOREIGN KEY ([CardId])
        REFERENCES [Cards]([CardId])
        ON DELETE CASCADE,

    CONSTRAINT [FK_Activity_logs_Users] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users]([UserId])
);