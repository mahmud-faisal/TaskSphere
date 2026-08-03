using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using ValidationException = Domain.Exceptions.ValidationException;
using FluentValidation;
using MediatR;

namespace Application.Features.Users;

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

public record CreateUserCommand(CreateUserDto User) : IRequest<UserDto>;

public record UpdateUserCommand(Guid Id, UpdateUserDto User) : IRequest<UserDto>;

public record DeactivateUserCommand(Guid Id) : IRequest<bool>;

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

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.User.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.User.Role).IsInEnum();
    }
}

public class UserQueryAndCommandHandler :
    IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>,
    IRequestHandler<GetUserByIdQuery, UserDto>,
    IRequestHandler<CreateUserCommand, UserDto>,
    IRequestHandler<UpdateUserCommand, UserDto>,
    IRequestHandler<DeactivateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public UserQueryAndCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(u => MapToDto(u)).ToList();
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }
        return MapToDto(user);
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.User.Email, cancellationToken);
        if (existing != null)
        {
            throw new ValidationException($"A user with email '{request.User.Email}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.User.Name,
            Email = request.User.Email,
            PasswordHash = _passwordHasher.HashPassword(request.User.Password),
            Role = request.User.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        var existingWithEmail = await _userRepository.GetByEmailAsync(request.User.Email, cancellationToken);
        if (existingWithEmail != null && existingWithEmail.Id != request.Id)
        {
            throw new ValidationException($"Email '{request.User.Email}' is already in use by another user.");
        }

        user.Name = request.User.Name;
        user.Email = request.User.Email;
        user.Role = request.User.Role;
        user.IsActive = request.User.IsActive;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        user.IsActive = false;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UserDto MapToDto(User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Role = u.Role,
        CreatedAt = u.CreatedAt,
        IsActive = u.IsActive
    };
}
