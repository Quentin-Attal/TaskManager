using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> Tasks { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
