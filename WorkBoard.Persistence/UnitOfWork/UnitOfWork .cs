using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Persistence.Repositories;

namespace WorkBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _sqlConnection;
    private readonly IDbTransaction _transaction;

    private IUserRepository? _userRepository;
    private IWorkspaceRepository? _workspaceRepository;
    private IWorkspaceMemberRepository? _workspaceMemberRepository;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _sqlConnection = connectionFactory.Create();

        if (_sqlConnection.State == ConnectionState.Closed)
        {
            _sqlConnection.Open();
        }

        _transaction = _sqlConnection.BeginTransaction();
    }

    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(
            _sqlConnection, 
            _transaction);

    public IWorkspaceRepository WorkspaceRepository =>
        _workspaceRepository ??= new WorkspaceRepository(
            _sqlConnection, 
            _transaction);

    public IWorkspaceMemberRepository WorkspaceMemberRepository =>
        _workspaceMemberRepository ??= new WorkspaceMemberRepository(
            _sqlConnection, 
            _transaction);

    public void Commit()
    {
        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _sqlConnection?.Dispose();
    }
}