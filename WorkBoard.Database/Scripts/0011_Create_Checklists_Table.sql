CREATE TABLE [Checklists] (
    [ChecklistId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CardId]      UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR(50)     NOT NULL,
    [CreatedAt]   DATETIME2        NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt]   DATETIME2        NULL DEFAULT NULL,
    [UpdatedBy]   UNIQUEIDENTIFIER NULL DEFAULT NULL,

    CONSTRAINT [FK_Checklists_Cards] 
        FOREIGN KEY ([CardId]) 
        REFERENCES [Cards]([CardId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Checklists_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_Checklists_UpdatedBy_Users] 
        FOREIGN KEY ([UpdatedBy]) 
        REFERENCES [Users]([UserId])
);