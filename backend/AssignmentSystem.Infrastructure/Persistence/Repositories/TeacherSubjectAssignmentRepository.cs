using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence.Repositories;

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
