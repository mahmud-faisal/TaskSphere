using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<IReadOnlyList<Subject>> GetByClassCourseIdAsync(Guid classCourseId, CancellationToken cancellationToken = default);
}
