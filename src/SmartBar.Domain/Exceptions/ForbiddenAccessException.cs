namespace SmartBar.Domain.Exceptions;

public class ForbiddenAccessException : DomainException
{
    public ForbiddenAccessException() : base("Access to this resource is forbidden.") { }

    public ForbiddenAccessException(string message) : base(message) { }
}
