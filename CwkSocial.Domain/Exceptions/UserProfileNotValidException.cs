namespace CwkSocial.Domain.Exceptions;

public class UserProfileNotValidException : Exception
{
    internal UserProfileNotValidException()
    {
        ValidationErrors = new();
    }

    internal UserProfileNotValidException(string message) : base(message)
    {
        ValidationErrors = new();
    }

    public List<string> ValidationErrors { get; }
}