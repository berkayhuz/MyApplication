using AutoMapper;
using Forum.Application.Aggregates;
using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Common.DTOs;
using Forum.Application.Interfaces;
using Forum.Domain.Models;
using Forum.Shared.EventBus;
using Forum.Shared.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Forum.Application.CommandHandlers.IdentityHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IPasswordHasher<UserAggregate> _passwordHasher;
        private readonly IEventBus _eventBus;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(
            IMapper mapper,
            IUserRepository userRepository,
            IProfileRepository profileRepository,
            IPasswordHasher<UserAggregate> passwordHasher,
            IEventBus eventBus,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _passwordHasher = passwordHasher;
            _eventBus = eventBus;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByAsync(u => u.UserName == request.Username))
            {
                return Result.Failure("Username is already taken.");
            }

            if (await _userRepository.ExistsByAsync(u => u.Email == request.Email))
            {
                return Result.Failure("Email is already taken.");
            }

            var userCreateDto = _mapper.Map<CreateUserDTO>(request);

            var userAggregate = new UserAggregate(userCreateDto, _mapper);

            var hashedPassword = _passwordHasher.HashPassword(userAggregate, request.Password);
            userAggregate.ChangePassword(hashedPassword);

            await _userRepository.AddAsync(userAggregate);

            var userProfile = new UserProfileAggregate(Guid.NewGuid(), userAggregate.Id);
            
            var profileUpdateDTO = _mapper.Map<ProfileUpdateDTO>(request);
            userProfile.UpdateProfile(profileUpdateDTO);

            await _profileRepository.AddAsync(userProfile);

            var emailConfirmationToken = await _jwtTokenGenerator.GenerateTokenAsync(userAggregate);

            var userRegisteredEvent = new UserRegisteredEvent(userAggregate.Email.ToString(), emailConfirmationToken);
            await _eventBus.PublishAsync(userRegisteredEvent);

            return Result.Success("User registered successfully.");
        }
    }
}