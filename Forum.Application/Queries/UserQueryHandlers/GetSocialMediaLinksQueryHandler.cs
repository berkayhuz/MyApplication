using Forum.Application.Common.DTOs;
using Forum.Application.Interfaces;
using Forum.Application.Queries.UserQueries;
using MediatR;

namespace Forum.Application.Queries.UserQueryHandlers
{
    public class GetSocialMediaLinksQueryHandler : IRequestHandler<GetSocialMediaLinksQuery, List<UserSocialMediaDto>>
    {
        private readonly IProfileRepository _profileRepository;

        public GetSocialMediaLinksQueryHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<List<UserSocialMediaDto>> Handle(GetSocialMediaLinksQuery request, CancellationToken cancellationToken)
        {
            var socialMediaLinks = await _profileRepository.GetSocialMediaLinksByUserIdAsync(request.UserId);

            return socialMediaLinks.Select(h => new UserSocialMediaDto
            {
                Id = h.Id,
                Name = h.Name
            }).ToList();
        }
    }
}
