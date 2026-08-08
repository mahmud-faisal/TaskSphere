namespace AssignmentSystem.Application.DTOs;

public class ClassCourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateClassCourseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClassCourseId { get; set; }
    public string? ClassCourseName { get; set; }
}

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public Guid ClassCourseId { get; set; }
}

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

public class AssignTeacherDto
{
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassCourseId { get; set; }
}

public class StudentEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public Guid ClassCourseId { get; set; }
    public string? ClassCourseName { get; set; }
}

public class EnrollStudentDto
{
    public Guid StudentId { get; set; }
    public Guid ClassCourseId { get; set; }
}
