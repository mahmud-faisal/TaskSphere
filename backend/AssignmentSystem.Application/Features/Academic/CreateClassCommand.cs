using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record CreateClassCommand(CreateClassCourseDto ClassCourse) : IRequest<ClassCourseDto>;
