namespace Domain.Entities;

public class TeacherSubjectAssignment
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public User? Teacher { get; set; }
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public Guid ClassCourseId { get; set; }
    public ClassCourse? ClassCourse { get; set; }
}
