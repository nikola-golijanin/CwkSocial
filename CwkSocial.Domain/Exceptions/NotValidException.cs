namespace CwkSocial.Domain.Exceptions;

public class NotValidException : Exception
{
    public List<string> ValidationErrors { get; }

    internal NotValidException()
    {
        ValidationErrors = new();
    }

    internal NotValidException(string message) : base(message)
    {
        ValidationErrors = new();
    }

    internal NotValidException(string message, Exception inner) : base(message, inner)
    {
        ValidationErrors = new();
    }
}