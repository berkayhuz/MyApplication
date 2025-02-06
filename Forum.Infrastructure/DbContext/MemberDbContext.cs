using Forum.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Forum.Infrastructure.Persistence
{
    public class MemberDbContext : IdentityDbContext<User>
    {
        public new DbSet<User> Users { get; set; }
        public MemberDbContext(DbContextOptions<MemberDbContext> options)
            : base(options)
        {
        }
    }
}
