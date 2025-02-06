using Forum.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Forum.Domain.Entities.UserEntities.UserProfileEntities
{
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Profil Bilgileri
        public string? ProfileImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Bio { get; set; }
        public string? Content { get; set; }
        public string? CustomStatus { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }

        // Sosyal Bağlantılar ve Hobiler
        public List<UserSocialMedia>? SocialMediaLinks { get; set; }
        public List<UserHobby>? Hobbies { get; set; }

        // Aktivite Bilgileri
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActiveAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int PostCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public int VisitCount { get; set; } = 0;

        // Gizlilik ve Ayarlar
        public bool IsAnonymous { get; set; }
        public bool IsEmailNotifications { get; set; }
        public bool ThemePreference { get; set; } = false;
        public bool IsPrivateProfile { get; set; } = false;
        public ProfileVisibility ProfileVisibility { get; set; } = ProfileVisibility.Public;
        public bool IsEmailVisible { get; set; } = true;
        public bool IsPhoneNumberVisible { get; set; } = false;

    }
}
