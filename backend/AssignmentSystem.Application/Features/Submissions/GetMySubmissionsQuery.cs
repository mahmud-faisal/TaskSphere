using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Submissions;

public record GetMySubmissionsQuery : IRequest<IReadOnlyList<SubmissionDto>>;
