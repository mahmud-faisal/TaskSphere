using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Users;

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
