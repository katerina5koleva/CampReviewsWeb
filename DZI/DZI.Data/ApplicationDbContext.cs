using CampRating.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Camp> Camps { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string adminUserId = "a820ccf9-54ac-4047-b4b5-48dab0dc962b";
            const string adminRoleId = "a23a7ee8-beb5-4238-ad8a-88d54b3c3d29";
            const string userRoleId = "a23a7ee8-beb5-4238-ad8a-88d54b3c3d28";
            const string adminConcurrencyStamp = "12345678-1234-1234-1234-123456789012";
            const string userConcurrencyStamp = "12345678-1234-1234-1234-123456789013";
            const string adminPasswordHash = "AQAAAAIAAYagAAAAEJmsXmTvQxGXj8Yzq1uXW5JZ6+7V9kKj1pZ2h3Y4vR4X5nB6r7s8W3Y2w1oA1xg=="; // Hash for "Abc123!"

            // Seed Admin role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                Id = adminRoleId,
                ConcurrencyStamp = adminConcurrencyStamp
            });

            // Seed User role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "BasicUser",
                NormalizedName = "BASICUSER",
                Id = userRoleId,
                ConcurrencyStamp = userConcurrencyStamp
            });

            // Seed Admin user
            builder.Entity<User>().HasData(new User
            {
                Id = adminUserId,
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                PasswordHash = adminPasswordHash,
                SecurityStamp = string.Empty,
                ConcurrencyStamp = adminConcurrencyStamp,
                FirstName = "Admin",
                LastName = "User"
            });

            // Assign Admin user to Admin role
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
        }
    }
}
