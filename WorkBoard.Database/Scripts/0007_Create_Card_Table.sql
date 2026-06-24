CREATE TABLE [Cards] (
    [CardId]      UNIQUEIDENTIFIER   NOT NULL PRIMARY KEY,
    [SectionId]   UNIQUEIDENTIFIER   NOT NULL,
    [Title]       NVARCHAR(100)      NOT NULL,
    [Description] NVARCHAR(MAX)      NULL DEFAULT NULL,
    [DueDate]     DATETIME2          NULL DEFAULT NULL,
    [Position]    FLOAT              NOT NULL,
    [CreatedAt]   DATETIME2          NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER   NOT NULL,
    [UpdatedAt]   DATETIME2          NULL DEFAULT NULL,
    [UpdatedBy]   UNIQUEIDENTIFIER   NULL DEFAULT NULL,

    CONSTRAINT [FK_Cards_Sections] 
        FOREIGN KEY ([SectionId]) 
        REFERENCES [Sections]([SectionId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Cards_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_Cards_UpdatedBy_Users] 
        FOREIGN KEY ([UpdatedBy]) 
        REFERENCES [Users]([UserId])
);