namespace WorkBoard.Archivation.Domain.Constants;

public static class ServiceBusConstants
{
    public const string ArchivationQueue = "archivation-queue";
    public const string RestoreQueue = "restore-queue";
    public const string ConnectionStringKey = "ServiceBus:ConnectionString";
}
