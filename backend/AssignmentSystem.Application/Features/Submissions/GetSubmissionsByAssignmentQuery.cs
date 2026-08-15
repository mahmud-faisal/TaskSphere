using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Submissions;

public record GetSubmissionsByAssignmentQuery(Guid AssignmentId) : IRequest<IReadOnlyList<SubmissionDto>>;
