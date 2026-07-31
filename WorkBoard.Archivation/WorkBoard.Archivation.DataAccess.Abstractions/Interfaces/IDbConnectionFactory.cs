using System.Data;

namespace WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
