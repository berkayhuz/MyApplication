using Microsoft.Extensions.Caching.Distributed;

namespace Forum.Application.Identity
{
    public class UserActivityService
    {
        private readonly IDistributedCache _distributedCache;

        public UserActivityService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task MarkUserAsActiveAsync(Guid userId)
        {
            var cacheValue = "true";
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            await _distributedCache.SetStringAsync(userId.ToString(), cacheValue, options);
        }

        public async Task MarkUserAsInactiveAsync(Guid userId)
        {
            await _distributedCache.RemoveAsync(userId.ToString());
        }

        public async Task<bool> IsUserActiveAsync(Guid userId)
        {
            var result = await _distributedCache.GetStringAsync(userId.ToString());
            return result == "true";
        }
    }

}
