using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Application.Features.Users;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }
    public async Task<List<User>?> GetPaginatedUsersAsync(GetUsersWithPaginationQuery request, CancellationToken cancellationToken = default)
    {
        int Skiped = (request.request.PageNo - 1) * request.request.PageSize;
        return await Context.Users
            .OrderBy(u=>u.CreatedAt)
            .Skip(Skiped)
            .Take(request.request.PageSize)
            .ToListAsync();

    }
}
