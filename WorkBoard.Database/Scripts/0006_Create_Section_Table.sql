CREATE TABLE [Sections] (
    [SectionId]   UNIQUEIDENTIFIER   NOT NULL PRIMARY KEY,
    [BoardId]     UNIQUEIDENTIFIER   NOT NULL,
    [Name]        NVARCHAR(50)       NOT NULL,
    [Position]    FLOAT              NOT NULL,
    [CreatedAt]   DATETIME2          NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER   NOT NULL,
    [UpdatedAt]   DATETIME2          NULL DEFAULT NULL,
    [UpdatedBy]   UNIQUEIDENTIFIER   NULL DEFAULT NULL,

    CONSTRAINT [FK_Sections_Boards] 
        FOREIGN KEY ([BoardId]) 
        REFERENCES [Boards]([BoardId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Sections_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_Sections_UpdatedBy_Users] 
        FOREIGN KEY ([UpdatedBy]) 
        REFERENCES [Users]([UserId])
);