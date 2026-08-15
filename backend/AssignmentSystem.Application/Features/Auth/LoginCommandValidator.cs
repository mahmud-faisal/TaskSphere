using FluentValidation;

namespace AssignmentSystem.Application.Features.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Login.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
