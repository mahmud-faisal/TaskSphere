using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace AssignmentSystem.Application.Features.Auth;

public record LoginCommand(LoginDto Login) : IRequest<AuthResponseDto>;

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

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Login.Email, cancellationToken);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        bool isPasswordValid = _passwordHasher.VerifyPassword(request.Login.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        string token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            }
        };
    }
}
