using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Submissions;

public record GradeSubmissionCommand(Guid Id, GradeSubmissionDto Grade) : IRequest<SubmissionDto>;
