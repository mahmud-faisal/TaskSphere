namespace AssignmentSystem.Application.DTOs;

public class StudentEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public Guid ClassCourseId { get; set; }
    public string? ClassCourseName { get; set; }
}
