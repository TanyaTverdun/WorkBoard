using System.Data;

namespace WorkBoard.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a factory for creating and managing database connections
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Gets an existing open database connection or creates a new one
        /// </summary>
        /// <returns>An open <see cref="IDbConnection"/> instance.</returns>
        IDbConnection GetOrCreateConnection();
    }
}
