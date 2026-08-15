using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record GetAssignmentByIdQuery(Guid Id) : IRequest<AssignmentDto>;
