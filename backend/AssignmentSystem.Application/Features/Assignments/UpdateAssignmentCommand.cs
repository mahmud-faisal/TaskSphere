using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record UpdateAssignmentCommand(Guid Id, UpdateAssignmentDto Assignment) : IRequest<AssignmentDto>;
