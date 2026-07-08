CREATE TABLE [Attachments] (
    [AttachmentId]  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CardId]        UNIQUEIDENTIFIER NOT NULL,
    [FileUrl]       NVARCHAR(2048)   NOT NULL,
    [FileName]      NVARCHAR(100)    NOT NULL,
    [FileSizeBytes] BIGINT           NOT NULL,

    CONSTRAINT [FK_Attachments_Cards] 
        FOREIGN KEY ([CardId])
        REFERENCES [Cards]([CardId])
        ON DELETE CASCADE
);