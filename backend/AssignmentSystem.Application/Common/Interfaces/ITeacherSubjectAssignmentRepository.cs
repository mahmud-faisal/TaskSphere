using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface ITeacherSubjectAssignmentRepository : IRepository<TeacherSubjectAssignment>
{
    Task<bool> IsTeacherAssignedAsync(Guid teacherId, Guid subjectId, Guid classCourseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeacherSubjectAssignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
}
