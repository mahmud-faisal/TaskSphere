using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public interface IClassCourseRepository : IRepository<ClassCourse>
{
    Task<ClassCourse?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ISubjectRepository : IRepository<Subject>
{
    Task<IReadOnlyList<Subject>> GetByClassCourseIdAsync(Guid classCourseId, CancellationToken cancellationToken = default);
}

public interface ITeacherSubjectAssignmentRepository : IRepository<TeacherSubjectAssignment>
{
    Task<bool> IsTeacherAssignedAsync(Guid teacherId, Guid subjectId, Guid classCourseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeacherSubjectAssignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
}

public interface IStudentEnrollmentRepository : IRepository<StudentEnrollment>
{
    Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classCourseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
}

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<Assignment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Assignment>> GetForClassCourseAsync(Guid classCourseId, bool includeDrafts = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Assignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
}

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<Submission?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
}
