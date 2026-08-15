using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Submissions;

public record GetSubmissionByIdQuery(Guid Id) : IRequest<SubmissionDto>;
