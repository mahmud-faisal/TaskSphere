using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Submissions;

public record UpdateSubmissionCommand(Guid Id, UpdateSubmissionDto Submission) : IRequest<SubmissionDto>;
