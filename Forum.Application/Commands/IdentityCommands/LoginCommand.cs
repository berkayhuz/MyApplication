using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.IdentityCommands
{
    public class LoginCommand : IRequest<Result>
    {
        public string? Username{ get; set; }
        public string? Email{ get; set; }
        public string Password { get; set; }
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email);
        }
    }
}
