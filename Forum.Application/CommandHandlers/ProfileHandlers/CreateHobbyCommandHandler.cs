using AutoMapper;
using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.CommandHandlers.ProfileHandlers
{
    public class CreateHobbyCommandHandler : IRequestHandler<CreateHobbyCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;

        public CreateHobbyCommandHandler(IMapper mapper, IProfileRepository profileRepository)
        {
            _mapper = mapper;
            _profileRepository = profileRepository;
        }

        public async Task<Result> Handle(CreateHobbyCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _profileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
            {
                return Result.Failure("User profile not found.");
            }

            var currentHobbiesCount = await _profileRepository.GetHobbiesCountByUserIdAsync(request.UserId);

            if (currentHobbiesCount >= 10)
            {
                return Result.Failure("You can only have up to 10 hobbies.");
            }

            var hobby = _mapper.Map<UserHobby>(request);
            hobby.ProfileId = userProfile.Id;

            await _profileRepository.AddHobbyAsync(request.UserId, hobby);

            return Result.Success("Hobby created successfully.");
        }

    }
}