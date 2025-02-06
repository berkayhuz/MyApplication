using Forum.Application.Aggregates;
using Forum.Domain.Entities.User;
using System.Security.Claims;

namespace Forum.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<ClaimsPrincipal?> ValidateAuthorizationTokenAsync(string accessToken);
        Task<string> RefreshAccessTokenAsync(string refreshToken, string userId);
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
        Task<string> GenerateTokenAsync(UserAggregate userAggregate);
        Task<(bool IsValid, ClaimsPrincipal Principal)> ValidateTokenAsync(string token);
    }
}
