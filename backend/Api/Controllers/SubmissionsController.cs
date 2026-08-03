using Application.DTOs;
using Application.Features.Submissions;
using BdjobsMIS.Aggregator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
public class SubmissionsController : ApiControllerBase
{
    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SubmissionDto>>>> GetMySubmissions()
    {
        var submissions = await Mediator.Send(new GetMySubmissionsQuery());
        return OkResponse(submissions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> GetById(Guid id)
    {
        var submission = await Mediator.Send(new GetSubmissionByIdQuery(id));
        return OkResponse(submission);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> Update(Guid id, [FromBody] UpdateSubmissionDto request)
    {
        var submission = await Mediator.Send(new UpdateSubmissionCommand(id, request));
        return OkResponse(submission, "Submission updated successfully");
    }

    [HttpPut("{id:guid}/grade")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> Grade(Guid id, [FromBody] GradeSubmissionDto request)
    {
        var submission = await Mediator.Send(new GradeSubmissionCommand(id, request));
        return OkResponse(submission, "Submission graded successfully");
    }
}
