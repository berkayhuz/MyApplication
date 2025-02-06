using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.IdentityCommands
{
    public class VerifyEmailCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
    }
}
