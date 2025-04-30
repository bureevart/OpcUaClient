namespace OpcUaClient.Domain.Exceptions;

public class CantReadValueException : Exception
{
    public CantReadValueException(string? message) : base(message)
    {
    }

    public CantReadValueException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}