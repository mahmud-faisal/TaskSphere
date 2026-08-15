using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence.Repositories;

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
