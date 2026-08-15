using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Features.Academic;
using AssignmentSystem.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

[Authorize]
public class StudentEnrollmentsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudentEnrollmentDto>>>> GetAll([FromQuery] Guid? studentId, [FromQuery] Guid? classCourseId)
    {
        var enrollments = await Mediator.Send(new GetStudentEnrollmentsQuery(studentId, classCourseId));
        return OkResponse(enrollments);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StudentEnrollmentDto>>> Enroll([FromBody] EnrollStudentDto request)
    {
        var result = await Mediator.Send(new EnrollStudentCommand(request));
        return CreatedResponse(result, "Student enrolled successfully");
    }
}
