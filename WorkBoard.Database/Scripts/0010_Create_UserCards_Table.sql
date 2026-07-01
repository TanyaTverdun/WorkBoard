CREATE TABLE [UserCards] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [CardId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_UserCards] 
        PRIMARY KEY CLUSTERED ([UserId], [CardId]),

    CONSTRAINT [FK_UserCards_Users] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users]([UserId])
        ON DELETE NO ACTION,

    CONSTRAINT [FK_UserCards_Cards] 
        FOREIGN KEY ([CardId]) 
        REFERENCES [Cards]([CardId])
        ON DELETE CASCADE
);