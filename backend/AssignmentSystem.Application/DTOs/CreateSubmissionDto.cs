namespace AssignmentSystem.Application.DTOs;

public class CreateSubmissionDto
{
    public Guid AssignmentId { get; set; }
    public string? AnswerText { get; set; }
    public string? FileUrl { get; set; }
}
