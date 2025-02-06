using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.CommandHandlers.ProfileHandlers
{
    public class DeleteHobbyCommandHandler : IRequestHandler<DeleteHobbyCommand, Result>
    {
        private readonly IProfileRepository _profileRepository;

        public DeleteHobbyCommandHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Result> Handle(DeleteHobbyCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _profileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
            {
                return Result.Failure("User profile not found.");
            }

            await _profileRepository.RemoveHobbyAsync(request.HobbyId);

            return Result.Success("Hobby deleted successfully.");
        }
    }
}