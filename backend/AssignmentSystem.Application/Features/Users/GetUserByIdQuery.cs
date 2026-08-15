using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
