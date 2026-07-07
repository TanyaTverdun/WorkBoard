CREATE TABLE [Checklist_items] (
    [ChecklistItemId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [ChecklistId]     UNIQUEIDENTIFIER NOT NULL,
    [Title]           NVARCHAR(200)    NOT NULL,
    [IsDone]          BIT              NOT NULL DEFAULT 0,
    [CreatedAt]       DATETIME2        NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt]       DATETIME2        NULL DEFAULT NULL,
    [UpdatedBy]       UNIQUEIDENTIFIER NULL DEFAULT NULL,

    CONSTRAINT [UQ_Checklist_items_ChecklistId_Title] 
        UNIQUE ([ChecklistId], [Title]),

    CONSTRAINT [FK_Checklist_items_Checklists] 
        FOREIGN KEY ([ChecklistId])
        REFERENCES [Checklists]([ChecklistId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Checklist_items_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_Checklist_items_UpdatedBy_Users] 
        FOREIGN KEY ([UpdatedBy]) 
        REFERENCES [Users]([UserId])
);