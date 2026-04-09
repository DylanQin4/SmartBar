namespace SmartBar.Domain.Exceptions;

public class ConflictException : DomainException
{
    public ConflictException(string entityName, object key)
        : base($"A conflict occurred for {entityName} with key '{key}'.") { }

    public ConflictException(string message) : base(message) { }
}
