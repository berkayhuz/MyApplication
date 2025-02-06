using Forum.Application.Aggregates;
using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Models;
using Forum.Shared.EventBus;
using Forum.Shared.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Forum.Application.CommandHandlers.IdentityHandlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<UserAggregate> _passwordHasher;
        private readonly IEventBus _eventBus;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IJwtBlacklistService _jwtBlacklistService;

        public ResetPasswordCommandHandler(IJwtBlacklistService jwtBlacklistService, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IPasswordHasher<UserAggregate> passwordHasher, IEventBus eventBus)
        {
            _jwtBlacklistService = jwtBlacklistService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Result.Failure("Fields cannot be null or empty.");
            }

            var (isTokenValid, principal) = await _jwtTokenGenerator.ValidateTokenAsync(request.Token);
            if (!isTokenValid)
            {
                return Result.Failure("Invalid or expired token.");
            }

            var isTokenBlacklisted = await _jwtBlacklistService.IsBlacklistedAsync(request.Token);
            if (isTokenBlacklisted)
            {
                return Result.Failure("This token has already been used or blacklisted.");
            }

            var expirationClaim = principal?.FindFirst(JwtRegisteredClaimNames.Exp);
            if (expirationClaim != null && DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime)
            {
                return Result.Failure("Token has expired.");
            }
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                return Result.Failure("Invalid token payload.");
            }

            var user = await _userRepository.GetByAsync(u => u.Id == userIdClaim);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            if (!IsValidPassword(request.NewPassword))
            {
                return Result.Failure("Password must be at least 8 characters long, contain both upper and lower case letters, and at least one number.");
            }

            var currentPasswordHash = user.PasswordHash;
            if (_passwordHasher.VerifyHashedPassword(user, currentPasswordHash, request.NewPassword) == PasswordVerificationResult.Success)
            {
                return Result.Failure("New password cannot be the same as the current password.");
            }

            var hashedPassword = _passwordHasher.HashPassword(user, request.NewPassword);
            user.ChangePassword(hashedPassword);

            await _userRepository.UpdateAsync(user);

            await _jwtBlacklistService.AddToBlacklistAsync(request.Token, DateTime.UtcNow.AddHours(1));

            var userResetPasswordEvent = new UserResetPasswordEvent(user.Email.Value);
            await _eventBus.PublishAsync(userResetPasswordEvent);

            return Result.Success("Reset password successfully.");
        }


        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            bool hasUpperChar = password.Any(c => char.IsUpper(c));
            bool hasLowerChar = password.Any(c => char.IsLower(c));
            bool hasDigit = password.Any(c => char.IsDigit(c));
            bool hasSpecialChar = password.Any(c => "!@#$%^&*()_+=-".Contains(c));

            return hasUpperChar && hasLowerChar && hasDigit && hasSpecialChar;
        }
    }
}
