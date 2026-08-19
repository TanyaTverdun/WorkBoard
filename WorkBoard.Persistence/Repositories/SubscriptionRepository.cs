using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Subscriptions;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Persistence.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    private const string EnforceFreePlanLimits = "sp_EnforceFreePlanLimits";
    private const string UserId = "@UserId";

    internal SubscriptionRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<EnforceFreePlanLimitsResult> EnforceFreePlanLimitsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add(
            UserId, 
            userId, 
            DbType.Guid);

        var command = new CommandDefinition(
            commandText: EnforceFreePlanLimits,
            parameters: parameters,
            transaction: _transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);

        var deletedBoardIds = (await multi.ReadAsync<Guid>()).ToList();
        var deletedSections = (await multi.ReadAsync<DeletedSectionInfo>()).ToList();

        return new EnforceFreePlanLimitsResult(
            deletedBoardIds, 
            deletedSections);
    }
}
