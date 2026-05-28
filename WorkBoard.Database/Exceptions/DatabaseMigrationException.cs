namespace WorkBoard.Database.Exceptions;

public class DatabaseMigrationException : Exception
{
    public DatabaseMigrationException(
        string message, 
        Exception? innerExeption)
        : base(
            message, 
            innerExeption)
    {
    }
}
