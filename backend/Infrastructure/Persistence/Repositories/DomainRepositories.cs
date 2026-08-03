using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }
}

public class ClassCourseRepository : Repository<ClassCourse>, IClassCourseRepository
{
    public ClassCourseRepository(AppDbContext context) : base(context) { }

    public async Task<ClassCourse?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.ClassCourses
            .Include(c => c.Subjects)
            .Include(c => c.StudentEnrollments)
            .Include(c => c.TeacherAssignments)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Subject>> GetByClassCourseIdAsync(Guid classCourseId, CancellationToken cancellationToken = default)
    {
        return await Context.Subjects
            .Where(s => s.ClassCourseId == classCourseId)
            .ToListAsync(cancellationToken);
    }
}

public class TeacherSubjectAssignmentRepository : Repository<TeacherSubjectAssignment>, ITeacherSubjectAssignmentRepository
{
    public TeacherSubjectAssignmentRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsTeacherAssignedAsync(Guid teacherId, Guid subjectId, Guid classCourseId, CancellationToken cancellationToken = default)
    {
        return await Context.TeacherSubjectAssignments
            .AnyAsync(t => t.TeacherId == teacherId && t.SubjectId == subjectId && t.ClassCourseId == classCourseId, cancellationToken);
    }

    public async Task<IReadOnlyList<TeacherSubjectAssignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        return await Context.TeacherSubjectAssignments
            .Include(t => t.Subject)
            .Include(t => t.ClassCourse)
            .Where(t => t.TeacherId == teacherId)
            .ToListAsync(cancellationToken);
    }
}

public class StudentEnrollmentRepository : Repository<StudentEnrollment>, IStudentEnrollmentRepository
{
    public StudentEnrollmentRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classCourseId, CancellationToken cancellationToken = default)
    {
        return await Context.StudentEnrollments
            .AnyAsync(e => e.StudentId == studentId && e.ClassCourseId == classCourseId, cancellationToken);
    }

    public async Task<IReadOnlyList<StudentEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await Context.StudentEnrollments
            .Include(e => e.ClassCourse)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }
}

public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(AppDbContext context) : base(context) { }

    public async Task<Assignment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Assignments
            .Include(a => a.Subject)
            .Include(a => a.ClassCourse)
            .Include(a => a.Teacher)
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetForClassCourseAsync(Guid classCourseId, bool includeDrafts = false, CancellationToken cancellationToken = default)
    {
        var query = Context.Assignments
            .Include(a => a.Subject)
            .Include(a => a.Teacher)
            .Where(a => a.ClassCourseId == classCourseId);

        if (!includeDrafts)
        {
            query = query.Where(a => a.Status == Domain.Enums.AssignmentStatus.Published);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        return await Context.Assignments
            .Include(a => a.Subject)
            .Include(a => a.ClassCourse)
            .Include(a => a.Submissions)
            .Where(a => a.TeacherId == teacherId)
            .ToListAsync(cancellationToken);
    }
}

public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(AppDbContext context) : base(context) { }

    public async Task<Submission?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .Include(s => s.GradedByTeacher)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return await Context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId, cancellationToken);
    }

    public async Task<IReadOnlyList<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await Context.Submissions
            .Include(s => s.Student)
            .Include(s => s.GradedByTeacher)
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await Context.Submissions
            .Include(s => s.Assignment)
                .ThenInclude(a => a!.Subject)
            .Include(s => s.GradedByTeacher)
            .Where(s => s.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }
}
