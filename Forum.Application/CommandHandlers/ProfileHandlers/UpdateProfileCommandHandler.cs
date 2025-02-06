using AutoMapper;
using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Common.DTOs;
using Forum.Application.Interfaces;
using Forum.Domain.Models;
using Forum.Infrastructure.FileStorage;
using MediatR;

namespace Forum.Application.CommandHandlers.ProfileHandlers
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;
        private readonly IS3FileStorageService _fileStorageService;

        public UpdateProfileCommandHandler(
            IMapper mapper,
            IProfileRepository profileRepository,
            IS3FileStorageService fileStorageService)
        {
            _mapper = mapper;
            _profileRepository = profileRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _profileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
            {
                return Result.Failure("Kullanıcı profili bulunamadı.");
            }

            var profileUpdateDto = _mapper.Map<ProfileUpdateDTO>(request);

            if (request.ProfileImage != null)
            {
                userProfile.ProfileImageUrl = await _fileStorageService.UploadImageAsync(request.ProfileImage);
            }

            if (request.CoverImage != null)
            {
                userProfile.CoverImageUrl = await _fileStorageService.UploadImageAsync(request.CoverImage);
            }

            userProfile.UpdateProfile(profileUpdateDto);

            await _profileRepository.UpdateProfileAsync(userProfile, request.ProfileImage, request.CoverImage);

            return Result.Success("Profil başarıyla güncellendi.");
        }
    }

}
