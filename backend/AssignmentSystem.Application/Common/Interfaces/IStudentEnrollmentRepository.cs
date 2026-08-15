using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface IStudentEnrollmentRepository : IRepository<StudentEnrollment>
{
    Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classCourseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
}
