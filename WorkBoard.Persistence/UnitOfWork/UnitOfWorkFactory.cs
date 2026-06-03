using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Persistence.UnitOfWork;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UnitOfWorkFactory(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IUnitOfWork Create()
    {
        return new UnitOfWork(_connectionFactory);
    }
}
