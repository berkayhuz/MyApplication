using Forum.Application.Aggregates;
using Forum.Application.DTOs;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Forum.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Forum.Application.Interfaces
{
    public interface IProfileRepository
    {
        Task<UserProfileAggregate> GetByUserIdAsync(Guid userId);
        Task<UserpostDto> GetByUserProfileAsync(Guid userId);
        Task<Result> UpdateProfileAsync(UserProfileAggregate userProfile, IFormFile? profileImage, IFormFile? coverImage);
        Task AddAsync(UserProfileAggregate userProfile);
        Task AddHobbyAsync(Guid userId, UserHobby hobby);
        Task RemoveHobbyAsync(Guid hobbyId);
        Task<int> GetHobbiesCountByUserIdAsync(Guid userId);
        Task<int> GetSocialMediaLinksCountByUserIdAsync(Guid userId);
        Task AddSocialMediaLinkAsync(Guid userId, UserSocialMedia socialMediaLink);
        Task RemoveSocialMediaLinkAsync(Guid socialMediaLinkId);
        Task<List<UserHobby>> GetHobbiesByUserIdAsync(Guid userId);
        Task<List<UserSocialMedia>> GetSocialMediaLinksByUserIdAsync(Guid userId);
    }
}
