namespace OpcUaClient.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public EntityNotFoundException(Type t, Guid id) : base($"Entity of type {t.Name} with id {id} not found")
    { 
    }

}