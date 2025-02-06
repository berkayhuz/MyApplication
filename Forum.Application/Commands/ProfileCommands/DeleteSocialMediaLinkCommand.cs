using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.ProfileCommands
{
    public class DeleteSocialMediaLinkCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public Guid SocialMediaLinkId { get; set; }
    }
}