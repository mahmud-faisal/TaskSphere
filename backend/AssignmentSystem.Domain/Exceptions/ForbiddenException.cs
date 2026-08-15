namespace AssignmentSystem.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You are not authorized to perform this operation.") 
        : base(message) { }
}
