using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class CardRepository : GenericRepository<Card, Guid>, ICardRepository
{
    public CardRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal CardRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }
}
