using Application.Repositories;
using Domain.Entities;
using Domain.Specification.User;

namespace Infrastructure.Repositories
{
    public class UserRepository: Repository<AppUser>, IUserRepository
    {
        public UserRepository(ICRUDRepository<AppUser> repository) : base(repository)
        {
        }
        
        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _crud.SingleOrDefaultAsync(new UserByEmailSpecification(email), ct);
        }
    }
}
