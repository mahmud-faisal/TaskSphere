using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record GetSubjectsQuery(Guid? ClassCourseId = null) : IRequest<IReadOnlyList<SubjectDto>>;
