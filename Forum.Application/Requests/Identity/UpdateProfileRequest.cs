using Microsoft.AspNetCore.Http;

namespace Forum.Application.Requests.Identity
{
    public class UpdateProfileRequest
    {
        public IFormFile ProfileImageUrl { get; set; } 
        public IFormFile CoverImageUrl { get; set; } 
        public string Bio { get; set; }
        public string Content { get; set; }
        public string CustomStatus { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
    }

}
