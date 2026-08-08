namespace AssignmentSystem.Domain.Entities;

public class StudentEnrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public User? Student { get; set; }
    public Guid ClassCourseId { get; set; }
    public ClassCourse? ClassCourse { get; set; }
}
