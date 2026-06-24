using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IWorkspaceRepository WorkspaceRepository { get; }
    IWorkspaceMemberRepository WorkspaceMemberRepository { get; }
    IBoardRepository BoardRepository { get; }
    IBoardMemberRepository BoardMemberRepository { get; }
    ISectionRepository SectionRepository { get; }
    ICardRepository CardRepository { get; }

    void Commit();
    void Rollback();
}