namespace AssignmentSystem.Domain.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClassCourseId { get; set; }
    public ClassCourse? ClassCourse { get; set; }
}
