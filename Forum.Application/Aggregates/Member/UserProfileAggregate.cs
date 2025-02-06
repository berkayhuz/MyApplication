using Forum.Application.Common.DTOs;
using Forum.Domain.Entities;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Forum.Domain.Enums;
using Forum.Shared.EventBus;

namespace Forum.Application.Aggregates
{
    public class UserProfileAggregate
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Profil Bilgileri
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
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
        public List<UserSocialMedia>? SocialMediaLinks { get; set; } = new List<UserSocialMedia>();
        public List<UserHobby>? Hobbies { get; set; } = new List<UserHobby>();

        // Aktivite Bilgileri
        public DateTime CreatedAt { get; set; }
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

        private List<IEvent> _domainEvents = new List<IEvent>();
        public IReadOnlyList<IEvent> DomainEvents => _domainEvents.AsReadOnly();

        public UserProfileAggregate(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetSocialMediaLinks(UserSocialMedia[] socialMediaLinks)
        {
            SocialMediaLinks = new List<UserSocialMedia>(socialMediaLinks);
        }

        public void SetHobbies(UserHobby[] hobbies)
        {
            Hobbies = new List<UserHobby>(hobbies);
        }

        public UserProfileAggregate() { }

        private void AddDomainEvent(IEvent eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void UpdateProfile(ProfileUpdateDTO profileUpdateDTO)
        {
            ProfileImageUrl = profileUpdateDTO.ProfileImageUrl;
            CoverImageUrl = profileUpdateDTO.CoverImageUrl;
            Bio = profileUpdateDTO.Bio;
            Content = profileUpdateDTO.Content;
            CustomStatus = profileUpdateDTO.CustomStatus;
            Gender = profileUpdateDTO.Gender;
            BirthDate = profileUpdateDTO.BirthDate;
            Location = profileUpdateDTO.Location;
            PhoneNumber = profileUpdateDTO.PhoneNumber;
            Website = profileUpdateDTO.Website;
        }

        public void UpdatePrivacySettings(bool isAnonymous, bool isEmailNotifications, bool themePreference, bool isPrivateProfile,
            ProfileVisibility profileVisibility, bool isEmailVisible, bool isPhoneNumberVisible)
        {
            IsAnonymous = isAnonymous;
            IsEmailNotifications = isEmailNotifications;
            ThemePreference = themePreference;
            IsPrivateProfile = isPrivateProfile;
            ProfileVisibility = profileVisibility;
            IsEmailVisible = isEmailVisible;
            IsPhoneNumberVisible = isPhoneNumberVisible;
        }

        public void AddHobby(UserHobby hobby)
        {
            Hobbies.Add(hobby);
        }

        public void RemoveHobby(Guid hobbyId)
        {
            Hobbies.RemoveAll(h => h.Id == hobbyId);
        }

        public void UpdateHobby(UserHobby updatedHobby)
        {
            var hobby = Hobbies.Find(h => h.Id == updatedHobby.Id);
            if (hobby != null)
            {
                hobby.Name = updatedHobby.Name;
            }
        }

        public void AddSocialMediaLink(UserSocialMedia socialMediaLink)
        {
            SocialMediaLinks.Add(socialMediaLink);
        }

        public void RemoveSocialMediaLink(Guid socialMediaLinkId)
        {
            SocialMediaLinks.RemoveAll(sm => sm.Id == socialMediaLinkId);
        }

        public void UpdateSocialMediaLink(UserSocialMedia updatedSocialMediaLink)
        {
            var socialMediaLink = SocialMediaLinks.Find(sm => sm.Id == updatedSocialMediaLink.Id);
            if (socialMediaLink != null)
            {
                socialMediaLink.Name = updatedSocialMediaLink.Name;
            }
        }

        public void SetActivityInfo(DateTime? lastActiveAt, DateTime? modifiedAt, DateTime? deletedAt, int postCount, int commentCount, int visitCount)
        {
            LastActiveAt = lastActiveAt;
            ModifiedAt = modifiedAt;
            DeletedAt = deletedAt;
            PostCount = postCount;
            CommentCount = commentCount;
            VisitCount = visitCount;
        }
    }
}