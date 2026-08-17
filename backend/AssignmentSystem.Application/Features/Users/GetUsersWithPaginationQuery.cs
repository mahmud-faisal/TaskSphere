using AssignmentSystem.Application.DTOs;
using MediatR;


namespace AssignmentSystem.Application.Features.Users;

    public record GetUsersWithPaginationQuery(UsersWithPaginationRequestDto request) : IRequest<IReadOnlyList<UserDto>>;


