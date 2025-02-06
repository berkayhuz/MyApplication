using AutoMapper;
using Forum.Application.Common.DTOs;
using Forum.Application.DTOs;
using Forum.Application.Interfaces;
using Forum.Application.Queries.UserQueries;
using Forum.Application.Repositories;
using MediatR;

namespace Forum.Application.Queries.UserQueryHandlers
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _userProfileRepository;
        private readonly IFeedRepository _feedRepository;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(IFeedRepository feedRepository, IUserRepository userRepository, IProfileRepository userProfileRepository, IMapper mapper)
        {
            _feedRepository = feedRepository;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByAsync(u => u.UserName == request.Username);
            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            var userProfile = await _userProfileRepository.GetByUserIdAsync(user.Id);
            if (userProfile == null)
            {
                throw new KeyNotFoundException("Kullanıcı profili bulunamadı.");
            }

            var userPosts = await _feedRepository.GetPostsByUserIdAsync(user.Id.ToString(), 1, 50);

            var userProfileDto = _mapper.Map<UserProfileDto>(userProfile);

            userProfileDto.Username = user.Username;
            userProfileDto.Email = user.Email.ToString();
            userProfileDto.FirstName = user.FirstName;
            userProfileDto.LastName = user.LastName;

            userProfileDto.Hobbies = _mapper.Map<List<UserHobbyDto>>(userProfile.Hobbies);
            userProfileDto.SocialMediaLinks = _mapper.Map<List<UserSocialMediaDto>>(userProfile.SocialMediaLinks);

            var postDtos = _mapper.Map<List<PostDto>>(userPosts);
            foreach (var post in postDtos)
            {
                post.User = new UserpostDto
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImageUrl = userProfile.ProfileImageUrl
                };
            }
            userProfileDto.Posts = postDtos;

            return userProfileDto;
        }

    }
}
