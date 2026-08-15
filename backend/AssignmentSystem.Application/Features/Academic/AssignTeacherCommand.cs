using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record AssignTeacherCommand(AssignTeacherDto Assignment) : IRequest<TeacherSubjectAssignmentDto>;
