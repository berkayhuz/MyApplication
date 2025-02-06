namespace Forum.Application.Interfaces
{
    public interface IJwtBlacklistService
    {
        Task AddToBlacklistAsync(string jti, DateTime expiresAt);
        Task<bool> IsBlacklistedAsync(string jti);
    }

}
