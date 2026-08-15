using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record PublishAssignmentCommand(Guid Id) : IRequest<AssignmentDto>;
