using System.Data;

namespace WorkBoard.Application.Common.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection GetOrCreateConnection();
}
