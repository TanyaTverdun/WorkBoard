namespace WorkBoard.Application.Common.Interfaces;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
}
