using Domain.Entities;

namespace Domain.Specification.User
{
    public class UserByEmailSpecification(string email) : Specification<AppUser>(u => u.Email == email)
    {
    }
}