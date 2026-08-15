using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record DeleteAssignmentCommand(Guid Id) : IRequest<bool>;
