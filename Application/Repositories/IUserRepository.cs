using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository: IRepository<AppUser>
    {
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
