CREATE TABLE [Labels] (
    [LabelId]   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardId]   UNIQUEIDENTIFIER NOT NULL,
    [Name]      NVARCHAR(50)     NOT NULL,
    [Color]     VARCHAR(9)       NULL DEFAULT NULL,
    [CreatedAt] DATETIME2        NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2        NULL DEFAULT NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL DEFAULT NULL,

    CONSTRAINT [UQ_Labels_BoardId_Name] 
        UNIQUE ([BoardId], [Name]),

    CONSTRAINT [FK_Labels_Boards] 
        FOREIGN KEY ([BoardId]) 
        REFERENCES [Boards]([BoardId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Labels_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId]),

    CONSTRAINT [FK_Labels_UpdatedBy_Users] 
        FOREIGN KEY ([UpdatedBy]) 
        REFERENCES [Users]([UserId])
);