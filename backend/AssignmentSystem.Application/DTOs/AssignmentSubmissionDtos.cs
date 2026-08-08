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

public class UpdateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public bool AllowResubmission { get; set; }
}

public class SubmissionDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string? AssignmentTitle { get; set; }
    public int MaxMarks { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string? AnswerText { get; set; }
    public string? FileUrl { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public SubmissionStatus Status { get; set; }
    public int? MarksObtained { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public Guid? GradedByTeacherId { get; set; }
    public string? GradedByTeacherName { get; set; }
}

public class CreateSubmissionDto
{
    public Guid AssignmentId { get; set; }
    public string? AnswerText { get; set; }
    public string? FileUrl { get; set; }
}

public class UpdateSubmissionDto
{
    public string? AnswerText { get; set; }
    public string? FileUrl { get; set; }
}

public class GradeSubmissionDto
{
    public int MarksObtained { get; set; }
    public string? Feedback { get; set; }
    public SubmissionStatus? Status { get; set; } // Graded, ResubmissionRequired, etc.
}
