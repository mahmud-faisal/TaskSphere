namespace AssignmentSystem.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string name, object key) 
        : base($"Entity '{name}' with key '{key}' was not found.") { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You are not authorized to perform this operation.") 
        : base(message) { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Authentication credentials are invalid or missing.") 
        : base(message) { }
}

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}

public class DeadlinePassedException : DomainException
{
    public DeadlinePassedException(string message = "The assignment deadline has passed and late submissions are not allowed.") 
        : base(message) { }
}
