CREATE TABLE [Coments] (
    [ComentId]  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CardId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [Text]      NVARCHAR(MAX)    NOT NULL,
    [CreatedAt] DATETIME2        NOT NULL,

    CONSTRAINT [FK_Coments_Cards] 
        FOREIGN KEY ([CardId])
        REFERENCES [Cards]([CardId]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_Coments_Users] 
        FOREIGN KEY ([UserId])
        REFERENCES [Users]([UserId])
        ON DELETE NO ACTION
);