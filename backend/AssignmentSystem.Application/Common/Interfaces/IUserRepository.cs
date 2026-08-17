using AssignmentSystem.Application.Features.Users;
using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>?> GetPaginatedUsersAsync(GetUsersWithPaginationQuery request , CancellationToken cancellationToken = default);
}
