using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record CreateAssignmentCommand(CreateAssignmentDto Assignment) : IRequest<AssignmentDto>;
