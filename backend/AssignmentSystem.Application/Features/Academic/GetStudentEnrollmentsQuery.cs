using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record GetStudentEnrollmentsQuery(Guid? StudentId = null, Guid? ClassCourseId = null) : IRequest<IReadOnlyList<StudentEnrollmentDto>>;
