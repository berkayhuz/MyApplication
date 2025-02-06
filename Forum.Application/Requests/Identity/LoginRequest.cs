using System.ComponentModel.DataAnnotations;

namespace Forum.Application.Requests.Identity
{
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        [Required]
        public string Password { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email); 
        }
    }
}
