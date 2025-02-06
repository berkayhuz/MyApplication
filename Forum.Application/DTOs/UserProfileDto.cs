using Forum.Application.DTOs;
using Forum.Domain.Enums;

namespace Forum.Application.Common.DTOs
{
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int PostCount { get; set; }
        public int CommentCount { get; set; }
        public int VisitCount { get; set; }
        public ProfileVisibility ProfileVisibility { get; set; }
        public List<UserSocialMediaDto>? SocialMediaLinks { get; set; }
        public List<UserHobbyDto>? Hobbies { get; set; }
        public List<PostDto> Posts { get; set; }
    }
}
