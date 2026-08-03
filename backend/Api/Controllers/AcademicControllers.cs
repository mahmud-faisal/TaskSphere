using Application.DTOs;
using Application.Features.Academic;
using BdjobsMIS.Aggregator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
