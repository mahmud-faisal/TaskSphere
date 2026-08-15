using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Users;

public record CreateUserCommand(CreateUserDto User) : IRequest<UserDto>;
