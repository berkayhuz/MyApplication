using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.IdentityCommands
{
    public class ResetPasswordCommand : IRequest<Result>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
