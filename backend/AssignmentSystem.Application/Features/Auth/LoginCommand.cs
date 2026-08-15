using AssignmentSystem.Application.DTOs;
using MediatR;

namespace AssignmentSystem.Application.Features.Auth;

public record LoginCommand(LoginDto Login) : IRequest<AuthResponseDto>;
