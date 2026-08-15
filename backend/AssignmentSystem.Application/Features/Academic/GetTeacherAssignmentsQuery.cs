using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record GetTeacherAssignmentsQuery(Guid? TeacherId = null) : IRequest<IReadOnlyList<TeacherSubjectAssignmentDto>>;
