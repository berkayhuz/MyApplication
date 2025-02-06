using Forum.Domain.Entities;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Persistence
{
    public class ProfileDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserHobby> UserHobbies { get; set; }
        public DbSet<UserSocialMedia> UserSocialMedias { get; set; }
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                .ToTable("UserProfiles");

            modelBuilder.Entity<UserProfile>()
                .HasMany(up => up.SocialMediaLinks)
                .WithOne(sm => sm.Profile)
                .HasForeignKey(sm => sm.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProfile>()
                .HasMany(up => up.Hobbies)
                .WithOne(h => h.Profile)
                .HasForeignKey(h => h.ProfileId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<UserHobby>()
                .ToTable("UserHobbies");

            modelBuilder.Entity<UserSocialMedia>()
                .ToTable("UserSocialMedias");
        }
    }
}
