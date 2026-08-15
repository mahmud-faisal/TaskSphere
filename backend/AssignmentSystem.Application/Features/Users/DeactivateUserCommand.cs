using MediatR;

namespace AssignmentSystem.Application.Features.Users;

public record DeactivateUserCommand(Guid Id) : IRequest<bool>;
