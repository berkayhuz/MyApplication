using Forum.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Forum.Infrastructure.Identity
{
    public class JwtBlacklistService : IJwtBlacklistService
    {
        private readonly IDatabase _database;
        private readonly ILogger<JwtBlacklistService> _logger;
        private readonly int _blacklistExpirationMinutes;

        public JwtBlacklistService(IConnectionMultiplexer redis, IConfiguration configuration, ILogger<JwtBlacklistService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (redis == null)
            {
                throw new ArgumentNullException(nameof(redis), "Redis bağlantısı sağlanamadı! JWT blacklist özelliği devre dışı bırakılıyor.");
            }

            _database = redis.GetDatabase();

            _blacklistExpirationMinutes = int.TryParse(configuration["Jwt:BlacklistExpirationMinutes"], out int minutes) ? minutes : 60;
        }

        public async Task AddToBlacklistAsync(string jti, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                _logger.LogError("Boş veya geçersiz JTI (Token ID) kara listeye eklenemez.");
                return;
            }

            var key = $"blacklisted_jti:{jti}";

            var expiration = expiresAt > DateTime.UtcNow
                ? expiresAt - DateTime.UtcNow
                : TimeSpan.FromMinutes(_blacklistExpirationMinutes);

            await _database.StringSetAsync(key, "blacklisted", expiration);
            _logger.LogInformation($"Token JTI {jti} kara listeye eklendi. {expiration.TotalMinutes} dakika boyunca geçersiz olacak.");
        }

        public async Task<bool> IsBlacklistedAsync(string jti)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                _logger.LogWarning("Boş veya geçersiz JTI kontrol edilemez.");
                return false;
            }

            var key = $"blacklisted_jti:{jti}";
            return await _database.KeyExistsAsync(key);
        }
    }
}
