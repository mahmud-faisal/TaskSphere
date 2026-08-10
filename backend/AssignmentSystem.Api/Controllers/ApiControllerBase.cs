using AssignmentSystem.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string message = "Success")
    {
        return Ok(new ApiResponse<T>(true, 200, message, data));
    }

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(T data, string message = "Created successfully")
    {
        return StatusCode(201, new ApiResponse<T>(true, 201, message, data));
    }

    protected ActionResult<ApiResponse<object>> SuccessResponse(string message = "Operation completed successfully")
    {
        return Ok(new ApiResponse<object>(true, 200, message, null));
    }
}
