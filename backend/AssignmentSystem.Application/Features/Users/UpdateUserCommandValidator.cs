using FluentValidation;

namespace AssignmentSystem.Application.Features.Users;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.User.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.User.Role).IsInEnum();
    }
}
