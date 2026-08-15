using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<Assignment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Assignment>> GetForClassCourseAsync(Guid classCourseId, bool includeDrafts = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Assignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
}
