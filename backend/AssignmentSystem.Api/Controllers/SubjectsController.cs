using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Features.Academic;
using AssignmentSystem.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

[Authorize]
public class SubjectsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SubjectDto>>>> GetAll([FromQuery] Guid? classCourseId)
    {
        var subjects = await Mediator.Send(new GetSubjectsQuery(classCourseId));
        return OkResponse(subjects);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> Create([FromBody] CreateSubjectDto request)
    {
        var result = await Mediator.Send(new CreateSubjectCommand(request));
        return CreatedResponse(result, "Subject created successfully");
    }
}
