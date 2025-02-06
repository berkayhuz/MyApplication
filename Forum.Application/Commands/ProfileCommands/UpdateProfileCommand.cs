using Forum.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Forum.Application.Commands.ProfileCommands
{
    public class UpdateProfileCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? CoverImage { get; set; }
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
