using Forum.Domain.ValueObjects;

namespace Forum.Application.Common.DTOs
{
    public class CreateUserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmailAddress Email { get; set; }
        public string PasswordHash { get; set; }
    }

}
