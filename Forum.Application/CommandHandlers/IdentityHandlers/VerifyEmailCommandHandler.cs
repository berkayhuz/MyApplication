using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Models;
using Forum.Shared.EventBus;
using MediatR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Forum.Application.CommandHandlers.IdentityHandlers
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventBus _eventBus;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IJwtBlacklistService _jwtBlacklistService;

        public VerifyEmailCommandHandler(
            IJwtBlacklistService jwtBlacklistService,
            IJwtTokenGenerator jwtTokenGenerator,
            IUserRepository userRepository,
            IEventBus eventBus)
        {
            _jwtBlacklistService = jwtBlacklistService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return Result.Failure("Token cannot be null or empty.");
            }

            var (isValid, principal) = await _jwtTokenGenerator.ValidateTokenAsync(request.Token);
            if (!isValid || principal == null)
            {
                return Result.Failure("Invalid or expired token.");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Failure("Invalid token: User ID not found.");
            }

            var user = await _userRepository.GetByAsync(u => u.Id == userId.ToString());
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return Result.Failure("Email is already verified.");
            }

            user.EmailConfirmed = true;
            await _userRepository.UpdateAsync(user);

            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (!string.IsNullOrEmpty(jti))
            {
                await _jwtBlacklistService.AddToBlacklistAsync(jti, DateTime.UtcNow.AddDays(7));
            }

            return Result.Success("Email verification successful.");
        }
    }

}
