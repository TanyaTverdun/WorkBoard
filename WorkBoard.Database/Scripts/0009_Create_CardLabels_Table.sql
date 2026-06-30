CREATE TABLE [CardLabels] (
    [CardId]  UNIQUEIDENTIFIER NOT NULL,
    [LabelId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_CardLabels] 
        PRIMARY KEY CLUSTERED ([CardId], [LabelId]),

    CONSTRAINT [FK_CardLabels_Cards] 
        FOREIGN KEY ([CardId]) 
        REFERENCES [Cards]([CardId])
        ON DELETE CASCADE,

    CONSTRAINT [FK_CardLabels_Labels] 
        FOREIGN KEY ([LabelId]) 
        REFERENCES [Labels]([LabelId]) 
        ON DELETE NO ACTION
);