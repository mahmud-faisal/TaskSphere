namespace AssignmentSystem.Application.DTOs;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClassCourseId { get; set; }
    public string? ClassCourseName { get; set; }
}
