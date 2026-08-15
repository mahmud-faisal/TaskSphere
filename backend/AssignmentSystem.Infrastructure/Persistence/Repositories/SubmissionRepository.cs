using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence.Repositories;

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
