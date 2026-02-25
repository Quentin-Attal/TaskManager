using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository : IBaseRepository<AppUser>
    {
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
