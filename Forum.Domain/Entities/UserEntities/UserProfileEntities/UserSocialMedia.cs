using System.ComponentModel.DataAnnotations;

namespace Forum.Domain.Entities.UserEntities.UserProfileEntities
{
    public class UserSocialMedia
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProfileId { get; set; }
        public virtual UserProfile? Profile { get; set; }
    }
}