ALTER TABLE [Users]
ADD 
    [SubscriptionTier]     TINYINT       NOT NULL DEFAULT 0,
    [StripeSubscriptionId] NVARCHAR(100) NULL;