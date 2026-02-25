using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AppUser
    {
        private AppUser() { }

        public AppUser(Guid id, string email, DateTime createdAtUtc)
        {
            if (id == Guid.Empty) throw new DomainException("User id cannot be empty.");
            SetEmail(email);

            Id = id;
            CreatedAtUtc = createdAtUtc;
        }

        public Guid Id { get; private set; }
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public DateTime CreatedAtUtc { get; private set; }

        public ICollection<TaskItem> Tasks { get; private set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

        public static AppUser Create(string email, DateTime nowUtc)
            => new(Guid.NewGuid(), email, nowUtc);

        public void SetEmail(string email)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            if (email.Length is < 3 or > 256)
                throw new DomainException("Email length is invalid.");

            Email = email;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash cannot be empty.");

            PasswordHash = passwordHash;
        }
    }
}
