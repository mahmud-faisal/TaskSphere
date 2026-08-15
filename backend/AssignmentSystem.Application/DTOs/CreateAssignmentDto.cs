namespace AssignmentSystem.Application.DTOs;

public class CreateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassCourseId { get; set; }
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public bool PublishImmediately { get; set; } = false;
    public bool AllowResubmission { get; set; } = false;
}
