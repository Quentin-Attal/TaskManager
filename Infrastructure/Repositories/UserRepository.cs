using Application.Repositories;
using Domain.Entities;
using Domain.Specification.User;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Repositories.EFRepository;

namespace Infrastructure.Repositories
{
    public class UserRepository(IEFRepository<AppUser> repository) : BaseRepository<AppUser>(repository), IUserRepository
    {
        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _crud.SingleOrDefaultAsync(new UserByEmailSpecification(email), QueryOptions.ReadOnly, ct);
        }
    }
}
