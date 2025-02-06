using Microsoft.AspNetCore.Identity;

namespace Forum.Domain.Entities.User
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? CurrentJti { get; set; }
        public string? PreviousJti { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
