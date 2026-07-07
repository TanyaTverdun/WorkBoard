namespace WorkBoard.Application.Common.Exceptions;

public class DuplicateTitleException : Exception
{
    public DuplicateTitleException()
        : base("An item with this title already exists.")
    {
    }

    public DuplicateTitleException(string message)
        : base(message)
    {
    }
}