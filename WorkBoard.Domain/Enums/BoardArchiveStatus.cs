namespace WorkBoard.Domain.Enums;

public enum BoardArchiveStatus : byte
{
    Active = 0,
    Pending = 1,
    Queued = 2,
    Uploading = 3,
    Migrating = 4,
    Archived = 5,
    Failed = 6,
    RestorePending = 7
}
