namespace WorkBoard.Application.Common.Interfaces;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
