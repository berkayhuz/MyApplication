using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.ProfileCommands
{
    public class CreateSocialMediaLinkCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}