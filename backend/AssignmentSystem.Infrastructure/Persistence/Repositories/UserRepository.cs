using AssignmentSystem.Application.Common.Interfaces;
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
}
