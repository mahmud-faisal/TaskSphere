using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence.Repositories;

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
