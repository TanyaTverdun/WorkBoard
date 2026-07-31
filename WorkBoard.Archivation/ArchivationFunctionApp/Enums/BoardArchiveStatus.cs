namespace ArchivationFunctionApp.Enums;

public enum BoardArchiveStatus : byte
{
    Active = 0,
    Pending = 1,
    Queued = 2,
    Archived = 3,
    Migrating = 4,
    Failed = 5,
    RestorePending = 6
}
