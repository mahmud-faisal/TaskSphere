using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Submissions;

public record CreateSubmissionCommand(CreateSubmissionDto Submission) : IRequest<SubmissionDto>;
