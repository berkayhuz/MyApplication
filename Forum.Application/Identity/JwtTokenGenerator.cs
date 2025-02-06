using Forum.Application.Aggregates;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Forum.Infrastructure.Identity
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtBlacklistService _jwtBlacklistService;
        private readonly ILogger<JwtTokenGenerator> _logger;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly SecurityKey _signingKey;
        private readonly Dictionary<string, string> _refreshTokens = new(); 

        public JwtTokenGenerator(
            IConfiguration configuration,
            IJwtBlacklistService jwtBlacklistService,
            ILogger<JwtTokenGenerator> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtBlacklistService = jwtBlacklistService ?? throw new ArgumentNullException(nameof(jwtBlacklistService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT için geçerli bir SecretKey belirtilmemiş.");
            }

            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var tokenExpiryHours = int.TryParse(_configuration["Jwt:TokenExpiryHours"], out int expiryHours) ? expiryHours : 1;
            var expires = DateTime.UtcNow.AddHours(tokenExpiryHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();
            _refreshTokens[user.Id.ToString()] = refreshToken;

            return (accessToken, refreshToken);
        }

        public async Task<string?> RefreshAccessTokenAsync(string refreshToken, string userId)
        {
            if (!_refreshTokens.TryGetValue(userId, out var storedRefreshToken) || storedRefreshToken != refreshToken)
            {
                _logger.LogWarning("Invalid refresh token attempt.");
                return null;
            }

            var user = new User
            {
                Id = userId,
                FirstName = "DefaultFirstName",
                LastName = "DefaultLastName"
            };

            var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);

            await _jwtBlacklistService.AddToBlacklistAsync(refreshToken, DateTime.UtcNow.AddDays(30));

            _refreshTokens[userId] = newRefreshToken;

            return newAccessToken;
        }


        public async Task<string> GenerateTokenAsync(UserAggregate userAggregate)
        {
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, userAggregate.Username),
                new Claim(JwtRegisteredClaimNames.Email, userAggregate.Email.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Sub, userAggregate.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userAggregate.Id.ToString())
            };

            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var tokenExpiryHours = int.TryParse(_configuration["Jwt:TokenExpiryHours"], out int expiryHours) ? expiryHours : 1;
            var expires = DateTime.UtcNow.AddHours(tokenExpiryHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var confirmationToken = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(confirmationToken);
        }
        public async Task<ClaimsPrincipal?> ValidateAuthorizationTokenAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentNullException(nameof(accessToken));

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token doğrulama hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool IsValid, ClaimsPrincipal Principal)> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (string.IsNullOrWhiteSpace(token))
            {
                return (false, null);
            }

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, null);
                }

                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (!string.IsNullOrEmpty(jti) && await _jwtBlacklistService.IsBlacklistedAsync(jti))
                {
                    _logger.LogWarning("Token is blacklisted.");
                    return (false, null);
                }

                return (true, principal);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                return (false, null);
            }
        }

    }
}
