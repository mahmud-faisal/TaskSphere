using FluentValidation;

namespace AssignmentSystem.Application.Features.Users;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.User.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.User.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.User.Role).IsInEnum();
    }
}
