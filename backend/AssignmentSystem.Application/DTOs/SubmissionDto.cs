using AssignmentSystem.Domain.Enums;

namespace AssignmentSystem.Application.DTOs;

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
