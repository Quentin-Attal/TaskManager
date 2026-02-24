using Domain.Entities;

namespace Domain.Specification.User
{
    public class UserByEmailSpecification : Specification<AppUser>
    {
        public UserByEmailSpecification(string email)
            : base(u => u.Email == email)
        {
        }
    }
}