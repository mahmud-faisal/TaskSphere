namespace AssignmentSystem.Domain.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Authentication credentials are invalid or missing.") 
        : base(message) { }
}
