using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.IdentityCommands
{
    public class ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }
}