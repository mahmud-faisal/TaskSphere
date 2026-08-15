using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface IClassCourseRepository : IRepository<ClassCourse>
{
    Task<ClassCourse?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
