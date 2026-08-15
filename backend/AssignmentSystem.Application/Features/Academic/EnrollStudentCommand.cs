using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record EnrollStudentCommand(EnrollStudentDto Enrollment) : IRequest<StudentEnrollmentDto>;
