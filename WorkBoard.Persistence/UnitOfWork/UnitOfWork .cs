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
    private IBoardRepository? _boardRepository;
    private IBoardMemberRepository? _boardMemberRepository;
    private ISectionRepository? _sectionRepository;
    private ICardRepository? _cardRepository;
    private ILabelRepository? _labelRepository;
    private ICardLabelRepository? _cardLabelRepository;

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

    public IBoardRepository BoardRepository =>
        _boardRepository ??= new BoardRepository(
            _sqlConnection, 
            _transaction);

    public IBoardMemberRepository BoardMemberRepository =>
        _boardMemberRepository ??= new BoardMemberRepository(
            _sqlConnection, 
            _transaction);

    public ISectionRepository SectionRepository =>
        _sectionRepository ??= new SectionRepository(
            _sqlConnection,
            _transaction);

    public ICardRepository CardRepository =>
        _cardRepository ??= new CardRepository(
            _sqlConnection,
            _transaction);

    public ILabelRepository labelRepository =>
        _labelRepository ??= new LabelRepository(
            _sqlConnection,
            _transaction);

    public ICardLabelRepository CardLabelRepository =>
        _cardLabelRepository ??= new CardLabelRepository(
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