using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.Email).IsUnique();
                b.Property(x => x.Email).IsRequired().HasMaxLength(256);
                b.Property(x => x.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<TaskItem>(b =>
            {
                b.HasKey(t => t.Id);

                b.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                b.HasOne(t => t.AppUser)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.TokenHash).IsUnique();
                b.HasOne(x => x.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.Property(x => x.TokenHash).IsRequired();
            });
        }
    }

}
