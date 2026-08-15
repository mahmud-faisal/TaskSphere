using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Academic;

public record CreateSubjectCommand(CreateSubjectDto Subject) : IRequest<SubjectDto>;
