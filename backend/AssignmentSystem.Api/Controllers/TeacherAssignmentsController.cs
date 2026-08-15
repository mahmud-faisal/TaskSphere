using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Features.Academic;
using AssignmentSystem.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

[Authorize]
public class TeacherAssignmentsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TeacherSubjectAssignmentDto>>>> GetAll([FromQuery] Guid? teacherId)
    {
        var assignments = await Mediator.Send(new GetTeacherAssignmentsQuery(teacherId));
        return OkResponse(assignments);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TeacherSubjectAssignmentDto>>> Assign([FromBody] AssignTeacherDto request)
    {
        var result = await Mediator.Send(new AssignTeacherCommand(request));
        return CreatedResponse(result, "Teacher assigned successfully");
    }
}
