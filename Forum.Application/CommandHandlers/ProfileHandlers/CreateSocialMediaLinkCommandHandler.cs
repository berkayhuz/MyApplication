using AutoMapper;
using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.CommandHandlers.ProfileHandlers
{
    public class CreateSocialMediaLinkCommandHandler : IRequestHandler<CreateSocialMediaLinkCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;

        public CreateSocialMediaLinkCommandHandler(IMapper mapper, IProfileRepository profileRepository)
        {
            _mapper = mapper;
            _profileRepository = profileRepository;
        }

        public async Task<Result> Handle(CreateSocialMediaLinkCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _profileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
            {
                return Result.Failure("User profile not found.");
            }

            var currentSocialLinksCount = await _profileRepository.GetSocialMediaLinksCountByUserIdAsync(request.UserId);

            if (currentSocialLinksCount >= 10)
            {
                return Result.Failure("You can only have up to 10 social links.");
            }

            var socialMediaLink = _mapper.Map<UserSocialMedia>(request);

            socialMediaLink.ProfileId = userProfile.Id;

            await _profileRepository.AddSocialMediaLinkAsync(request.UserId, socialMediaLink);

            return Result.Success("Social media link created successfully.");
        }

    }
}