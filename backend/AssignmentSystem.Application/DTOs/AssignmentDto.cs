using AssignmentSystem.Domain.Enums;

namespace AssignmentSystem.Application.DTOs;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public Guid ClassCourseId { get; set; }
    public string? ClassCourseName { get; set; }
    public Guid TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public AssignmentStatus Status { get; set; }
    public bool AllowResubmission { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int SubmissionsCount { get; set; }
}
