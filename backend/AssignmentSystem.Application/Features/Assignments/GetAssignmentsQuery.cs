using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record GetAssignmentsQuery(Guid? ClassCourseId = null, Guid? SubjectId = null) : IRequest<IReadOnlyList<AssignmentDto>>;
