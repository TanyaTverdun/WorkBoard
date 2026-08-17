DECLARE @ConstraintName NVARCHAR(200);

SELECT 
    @ConstraintName = d.name
FROM 
    sys.default_constraints d
INNER JOIN sys.columns c
    ON d.parent_object_id = c.object_id AND 
    d.parent_column_id = c.column_id
WHERE 
    d.parent_object_id = OBJECT_ID('Workspaces') AND 
    c.name = 'SubscriptionTier';

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [Workspaces] DROP CONSTRAINT ' + @ConstraintName);
END

ALTER TABLE [Workspaces] DROP COLUMN [SubscriptionTier];