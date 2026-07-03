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
    ILabelRepository LabelRepository { get; }
    ICardLabelRepository CardLabelRepository { get; }
    IUserCardRepository UserCardRepository { get; }
    IChecklistRepository ChecklistRepository { get; }
    IChecklistItemRepository ChecklistItemRepository { get; }

    void Commit();
    void Rollback();
}