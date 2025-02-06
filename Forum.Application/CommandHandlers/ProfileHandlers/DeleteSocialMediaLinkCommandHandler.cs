using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.CommandHandlers.ProfileHandlers
{
    public class DeleteSocialMediaLinkCommandHandler : IRequestHandler<DeleteSocialMediaLinkCommand, Result>
    {
        private readonly IProfileRepository _profileRepository;

        public DeleteSocialMediaLinkCommandHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Result> Handle(DeleteSocialMediaLinkCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _profileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
            {
                return Result.Failure("User profile not found.");
            }

            await _profileRepository.RemoveSocialMediaLinkAsync(request.SocialMediaLinkId);

            return Result.Success("Social media link deleted successfully.");
        }
    }
}