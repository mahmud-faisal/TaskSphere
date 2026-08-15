namespace AssignmentSystem.Application.DTOs;

public class UpdateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public bool AllowResubmission { get; set; }
}
