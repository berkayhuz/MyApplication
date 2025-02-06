namespace Forum.Application.Common.DTOs
{
    public class ProfileUpdateDTO
    {
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
    }
}
