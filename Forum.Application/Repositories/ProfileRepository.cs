using AutoMapper;
using Forum.Application.Aggregates;
using Forum.Application.Common.DTOs;
using Forum.Application.DTOs;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Forum.Domain.Models;
using Forum.Infrastructure.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Persistence
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ProfileDbContext _context;
        private readonly IMapper _mapper;
        private readonly IS3FileStorageService _fileStorageService;
        private readonly IUserRepository _userRepository;

        public ProfileRepository(IUserRepository userRepository ,ProfileDbContext context, IMapper mapper, IS3FileStorageService fileStorageService)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task<UserProfileAggregate> GetByUserIdAsync(Guid userId)
        {
            var userProfileEntity = await _context.UserProfiles
                .Include(up => up.Hobbies)
                .Include(up => up.SocialMediaLinks)
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfileEntity == null)
            {
                return null;
            }

            return _mapper.Map<UserProfileAggregate>(userProfileEntity);
        }

        public async Task<UserpostDto> GetByUserProfileAsync(Guid userId)
        {
            var userProfileEntity = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfileEntity == null)
            {
                return null;
            }

            var userAggregate = await _userRepository.GetByAsync(u => u.Id == userProfileEntity.UserId.ToString());

            if (userAggregate == null)
            {
                return null;
            }

            var userpostDto = _mapper.Map<UserpostDto>(userProfileEntity);
            _mapper.Map(userAggregate, userpostDto);

            return userpostDto;
        }



        public async Task<Result> UpdateProfileAsync(UserProfileAggregate userProfile, IFormFile? profileImage, IFormFile? coverImage)
        {
            if (userProfile == null)
            {
                return Result.Failure("UserProfile cannot be null.");
            }

            var userProfileEntity = _mapper.Map<UserProfile>(userProfile);

            var existingUserProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userProfile.UserId);

            if (existingUserProfile != null)
            {
                if (profileImage != null)
                {
                    var profileImageUrl = await _fileStorageService.UploadImageAsync(profileImage);
                    userProfileEntity.ProfileImageUrl = profileImageUrl;
                }

                if (coverImage != null)
                {
                    var coverImageUrl = await _fileStorageService.UploadImageAsync(coverImage);
                    userProfileEntity.CoverImageUrl = coverImageUrl;
                }

                _context.Entry(existingUserProfile).CurrentValues.SetValues(userProfileEntity);
            }
            else
            {
                if (profileImage != null)
                {
                    var profileImageUrl = await _fileStorageService.UploadImageAsync(profileImage);
                    userProfileEntity.ProfileImageUrl = profileImageUrl;
                }

                if (coverImage != null)
                {
                    var coverImageUrl = await _fileStorageService.UploadImageAsync(coverImage);
                    userProfileEntity.CoverImageUrl = coverImageUrl;
                }

                await _context.UserProfiles.AddAsync(userProfileEntity);
            }

            await _context.SaveChangesAsync();
            return Result.Success("Profile updated successfully.");
        }

        public async Task AddAsync(UserProfileAggregate userProfile)
        {
            var userEntity = _mapper.Map<UserProfile>(userProfile);
            await _context.UserProfiles.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        #region Hobbies
        public async Task AddHobbyAsync(Guid userId, UserHobby hobby)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (userProfile != null)
            {
                hobby.ProfileId = userProfile.Id;
                await _context.UserHobbies.AddAsync(hobby);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveHobbyAsync(Guid hobbyId)
        {
            var hobby = await _context.UserHobbies.FindAsync(hobbyId);
            if (hobby != null)
            {
                _context.UserHobbies.Remove(hobby);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetHobbiesCountByUserIdAsync(Guid userId)
        {
            var hobbies = await _context.UserHobbies.Where(h => h.ProfileId == userId).ToListAsync();
            return hobbies.Count;
        }
        public async Task<List<UserHobby>> GetHobbiesByUserIdAsync(Guid userId)
        {
            var userProfile = await _context.UserProfiles
                .Include(up => up.Hobbies)
                .FirstOrDefaultAsync(up => up.UserId == userId);

            return userProfile?.Hobbies ?? new List<UserHobby>();
        }
        #endregion
        #region Social Links
        public async Task<int> GetSocialMediaLinksCountByUserIdAsync(Guid userId)
        {
            var socialMedias = await _context.UserSocialMedias.Where(h => h.ProfileId == userId).ToListAsync();
            return socialMedias.Count;
        }
        public async Task AddSocialMediaLinkAsync(Guid userId, UserSocialMedia socialMediaLink)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (userProfile != null)
            {
                socialMediaLink.ProfileId = userProfile.Id;
                await _context.UserSocialMedias.AddAsync(socialMediaLink);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveSocialMediaLinkAsync(Guid socialMediaLinkId)
        {
            var socialMediaLink = await _context.UserSocialMedias.FindAsync(socialMediaLinkId);
            if (socialMediaLink != null)
            {
                _context.UserSocialMedias.Remove(socialMediaLink);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserSocialMedia>> GetSocialMediaLinksByUserIdAsync(Guid userId)
        {
            var userProfile = await _context.UserProfiles
                .Include(up => up.SocialMediaLinks)
                .FirstOrDefaultAsync(up => up.UserId == userId);

            return userProfile?.SocialMediaLinks ?? new List<UserSocialMedia>();
        }
        #endregion
    }
}
