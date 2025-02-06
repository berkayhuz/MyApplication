using AutoMapper;
using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Identity;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.User;
using Forum.Domain.Models;
using Forum.Shared.EventBus;
using Forum.Shared.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<UserAggregate> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly UserActivityService _userActivityService;
    private readonly IJwtBlacklistService _jwtBlacklistService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<UserAggregate> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IEventBus eventBus,
        IMapper mapper,
        UserActivityService userActivityService,
        IJwtBlacklistService jwtBlacklistService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _eventBus = eventBus;
        _mapper = mapper;
        _userActivityService = userActivityService;
        _jwtBlacklistService = jwtBlacklistService;
    }

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(request.Username, request.Email);

        if (user == null) return Result.Failure("Invalid username, email, or password.");

        var lockoutError = CheckUserLockout(user);
        if (lockoutError != null) return lockoutError;

        var emailConfirmationError = CheckEmailConfirmation(user);
        if (emailConfirmationError != null) return emailConfirmationError;

        var passwordError = VerifyPassword(user, request.Password);
        if (passwordError != null)
        {
            await IncrementFailedLoginAttempts(user);
            return passwordError;
        }

        await BlacklistOldToken(user);

        var token = await GenerateJwtTokenAsync(user);

        var handler = new JwtSecurityTokenHandler();

        var jwtToken = handler.ReadJwtToken(token.AccessToken);

        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        await HandleSuccessfulLoginAsync(user, jtiClaim);

        await _userActivityService.MarkUserAsActiveAsync(user.Id);

        return Result.Success("User login successful.", new { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken });
    }

    private async Task HandleSuccessfulLoginAsync(UserAggregate user, string jtiClaim)
    {
        var userLoggedInEvent = new UserLoginEvent(user.Email.ToString(), DateTime.UtcNow);
        await _eventBus.PublishAsync(userLoggedInEvent);

        user.UpdateJti(jtiClaim);

        await _userRepository.UpdateAsync(user);

        if (!string.IsNullOrEmpty(user.PreviousJti))
        {
            await _jwtBlacklistService.AddToBlacklistAsync(user.PreviousJti, DateTime.UtcNow.AddDays(30));
        }

        user.UnlockAccount();
        await _userRepository.UpdateAsync(user);
    }


    private async Task BlacklistOldToken(UserAggregate user)
    {
        
        var oldJti = user.CurrentJti;
        if (!string.IsNullOrEmpty(oldJti))
        {
            await _jwtBlacklistService.AddToBlacklistAsync(oldJti, DateTime.UtcNow.AddMinutes(30));
        }
    }

    private async Task<UserAggregate?> FindUserAsync(string userName, string email)
    {
        return await _userRepository.GetByAsync(u => u.UserName == userName || u.Email == email);
    }

    private Result CheckUserLockout(UserAggregate user)
    {
        if (user.LockoutEnabled && user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure("User account is locked. Please try again later.");
        return null;
    }

    private Result CheckEmailConfirmation(UserAggregate user)
    {
        if (!user.EmailConfirmed)
            return Result.Failure("Email not confirmed.");
        return null;
    }

    private Result VerifyPassword(UserAggregate user, string password)
    {
        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
        {
            return Result.Failure("Invalid username, email, or password.");
        }
        return null;
    }

    private async Task IncrementFailedLoginAttempts(UserAggregate user)
    {
        user.IncrementAccessFailedCount();
        await _userRepository.UpdateAsync(user);
    }

    private async Task<(string AccessToken, string RefreshToken)> GenerateJwtTokenAsync(UserAggregate user)
    {
        var userEntity = _mapper.Map<User>(user);
        return await _jwtTokenGenerator.GenerateTokensAsync(userEntity);
    }
}
