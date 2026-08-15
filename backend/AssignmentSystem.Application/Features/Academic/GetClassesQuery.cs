using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record GetClassesQuery : IRequest<IReadOnlyList<ClassCourseDto>>;
