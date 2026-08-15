namespace AssignmentSystem.Application.DTOs;

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public Guid ClassCourseId { get; set; }
}
