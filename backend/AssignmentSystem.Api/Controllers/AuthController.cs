using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Features.Auth;
using AssignmentSystem.Application.Features.Users;
using AssignmentSystem.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto request)
    {
        var result = await Mediator.Send(new LoginCommand(request));
        return OkResponse(result, "Login successful");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ApiResponse<UserDto>(false, 401, "Invalid token claims"));
        }

        var user = await Mediator.Send(new GetUserByIdQuery(userId));
        return OkResponse(user);
    }
}
