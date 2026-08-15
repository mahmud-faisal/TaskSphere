using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Users;

public record UpdateUserCommand(Guid Id, UpdateUserDto User) : IRequest<UserDto>;
