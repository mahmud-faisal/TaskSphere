using AssignmentSystem.Domain.Enums;

namespace AssignmentSystem.Application.DTOs;

public class GradeSubmissionDto
{
    public int MarksObtained { get; set; }
    public string? Feedback { get; set; }
    public SubmissionStatus? Status { get; set; }
}
