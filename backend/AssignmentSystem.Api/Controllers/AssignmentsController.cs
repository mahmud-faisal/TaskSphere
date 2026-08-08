using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Features.Assignments;
using AssignmentSystem.Application.Features.Submissions;
using AssignmentSystem.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.Api.Controllers;

[Authorize]
public class AssignmentsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssignmentDto>>>> GetAll([FromQuery] Guid? classCourseId, [FromQuery] Guid? subjectId)
    {
        var assignments = await Mediator.Send(new GetAssignmentsQuery(classCourseId, subjectId));
        return OkResponse(assignments);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> GetById(Guid id)
    {
        var assignment = await Mediator.Send(new GetAssignmentByIdQuery(id));
        return OkResponse(assignment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> Create([FromBody] CreateAssignmentDto request)
    {
        var assignment = await Mediator.Send(new CreateAssignmentCommand(request));
        return CreatedResponse(assignment, "Assignment created successfully");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> Update(Guid id, [FromBody] UpdateAssignmentDto request)
    {
        var assignment = await Mediator.Send(new UpdateAssignmentCommand(id, request));
        return OkResponse(assignment, "Assignment updated successfully");
    }

    [HttpPut("{id:guid}/publish")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> Publish(Guid id)
    {
        var assignment = await Mediator.Send(new PublishAssignmentCommand(id));
        return OkResponse(assignment, "Assignment published successfully");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        await Mediator.Send(new DeleteAssignmentCommand(id));
        return SuccessResponse("Assignment deleted successfully");
    }

    [HttpPost("{id:guid}/submissions")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> SubmitAnswer(Guid id, [FromBody] CreateSubmissionDto request)
    {
        // Enforce assignmentId match
        request.AssignmentId = id;
        var submission = await Mediator.Send(new CreateSubmissionCommand(request));
        return CreatedResponse(submission, "Submission submitted successfully");
    }

    [HttpGet("{id:guid}/submissions")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SubmissionDto>>>> GetSubmissionsForAssignment(Guid id)
    {
        var submissions = await Mediator.Send(new GetSubmissionsByAssignmentQuery(id));
        return OkResponse(submissions);
    }
}
