using AutoMapper;
using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.User;
using Forum.Domain.Models;
using Forum.Shared.EventBus;
using Forum.Shared.Events;
using MediatR;

namespace Forum.Application.CommandHandlers.IdentityHandlers
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IEventBus _eventBus;

        public ForgotPasswordCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IMapper mapper, IUserRepository userRepository, IEventBus eventBus)
        {
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result.Failure("Email cannot be null or empty.");
            }

            var userAggregate = await _userRepository.GetByAsync(e => e.Email == request.Email);
            if (userAggregate == null)
            {
                return Result.Failure("User with this email does not exist.");
            }

            var token = await _jwtTokenGenerator.GenerateTokenAsync(userAggregate);

            var userForgotPasswordEvent = new UserForgotPasswordEvent(request.Email, token);
            await _eventBus.PublishAsync(userForgotPasswordEvent);

            return Result.Success("Forgot password email successfully sent.");
        }
    }
}
