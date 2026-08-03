using Application.DTOs;
using Application.Features.Users;
using BdjobsMIS.Aggregator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> GetAll()
    {
        var users = await Mediator.Send(new GetUsersQuery());
        return OkResponse(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var user = await Mediator.Send(new GetUserByIdQuery(id));
        return OkResponse(user);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto request)
    {
        var user = await Mediator.Send(new CreateUserCommand(request));
        return CreatedResponse(user, "User created successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserDto request)
    {
        var user = await Mediator.Send(new UpdateUserCommand(id, request));
        return OkResponse(user, "User updated successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(Guid id)
    {
        await Mediator.Send(new DeactivateUserCommand(id));
        return SuccessResponse("User deactivated successfully");
    }
}
