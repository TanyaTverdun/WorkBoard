namespace WorkBoard.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    void Commit();
    void Rollback();
}
