using System.Data;

namespace ArchivationFunctionApp.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
