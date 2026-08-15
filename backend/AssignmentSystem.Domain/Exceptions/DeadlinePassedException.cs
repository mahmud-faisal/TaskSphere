namespace AssignmentSystem.Domain.Exceptions;

public class DeadlinePassedException : DomainException
{
    public DeadlinePassedException(string message = "The assignment deadline has passed and late submissions are not allowed.") 
        : base(message) { }
}
