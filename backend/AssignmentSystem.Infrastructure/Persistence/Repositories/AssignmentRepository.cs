using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence.Repositories;

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
            query = query.Where(a => a.Status == AssignmentSystem.Domain.Enums.AssignmentStatus.Published);
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
