using AutoMapper;
using Forum.Application.Common.DTOs;
using Forum.Application.Interfaces;
using Forum.Application.Queries.UserQueries;
using MediatR;

namespace Forum.Application.Queries.UserQueryHandlers
{
    public class GetHobbiesQueryHandler : IRequestHandler<GetHobbiesQuery, List<UserHobbyDto>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public GetHobbiesQueryHandler(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<List<UserHobbyDto>> Handle(GetHobbiesQuery request, CancellationToken cancellationToken)
        {
            var hobbies = await _profileRepository.GetHobbiesByUserIdAsync(request.UserId);

            return _mapper.Map<List<UserHobbyDto>>(hobbies);
        }
    }

}
