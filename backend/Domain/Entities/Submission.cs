using Domain.Enums;

namespace Domain.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
    public Guid StudentId { get; set; }
    public User? Student { get; set; }
    public string? AnswerText { get; set; }
    public string? FileUrl { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public int? MarksObtained { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public Guid? GradedByTeacherId { get; set; }
    public User? GradedByTeacher { get; set; }
}
