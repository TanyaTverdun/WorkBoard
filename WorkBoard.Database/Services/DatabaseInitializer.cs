using System.Reflection;
using DbUp;

namespace WorkBoard.Database.Services;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new DataMisalignedException(
                "Database migration faild during DbUp execution", 
                result.Error);
        }
    }
}
