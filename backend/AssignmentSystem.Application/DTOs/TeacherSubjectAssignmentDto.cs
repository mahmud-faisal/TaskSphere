namespace AssignmentSystem.Application.DTOs;

public class TeacherSubjectAssignmentDto
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public Guid SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public Guid ClassCourseId { get; set; }
    public string? ClassCourseName { get; set; }
}
