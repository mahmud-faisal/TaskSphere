using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Features.Academic;
using AssignmentSystem.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

[Authorize]
public class ClassesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ClassCourseDto>>>> GetAll()
    {
        var classes = await Mediator.Send(new GetClassesQuery());
        return OkResponse(classes);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ClassCourseDto>>> Create([FromBody] CreateClassCourseDto request)
    {
        var result = await Mediator.Send(new CreateClassCommand(request));
        return CreatedResponse(result, "Class created successfully");
    }
}
