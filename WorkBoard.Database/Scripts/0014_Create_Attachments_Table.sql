CREATE TABLE [Attachments] (
    [AttachmentId]  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CardId]        UNIQUEIDENTIFIER NOT NULL,
    [FileUrl]       NVARCHAR(2048)   NOT NULL,
    [FileName]      NVARCHAR(100)    NOT NULL,
    [FileSizeBytes] BIGINT           NOT NULL,
    [CreatedAt]     DATETIME2        NOT NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [FK_Attachments_Cards] 
        FOREIGN KEY ([CardId])
        REFERENCES [Cards]([CardId])
        ON DELETE CASCADE,

    CONSTRAINT [FK_Attachments_CreatedBy_Users] 
        FOREIGN KEY ([CreatedBy]) 
        REFERENCES [Users]([UserId])
);