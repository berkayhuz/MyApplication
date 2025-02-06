using AutoMapper;
using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Interfaces;
using Forum.Application.Requests.Identity;
using Forum.Domain.Models;
using MediatR;

namespace Forum.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public IdentityService(IMapper mapper, IMediator mediator) {_mapper = mapper; _mediator = mediator;}
        public async Task<Result> ProcessRequestAsync<TRequest, TCommand>(TRequest request, Func<TRequest, TCommand> mapFunction, string errorMessage) where TCommand : IRequest<Result>
        {
            ValidateInputs(request);

            var command = mapFunction(request);

            return await _mediator.Send(command).ConfigureAwait(false);
        }
        public async Task<Result> RegisterAsync(RegisterRequest request)
        {
            return await ProcessRequestAsync(request, req => _mapper.Map<CreateUserCommand>(req), "Registration failed.");
        }
        public async Task<Result> LoginAsync(LoginRequest request)
        {
            return await ProcessRequestAsync(request, req => _mapper.Map<LoginCommand>(req), "Login failed.");
        }
        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            return await ProcessRequestAsync(request, req => _mapper.Map<ForgotPasswordCommand>(req), "Forgot password failed.");
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return await ProcessRequestAsync(request, req => _mapper.Map<ResetPasswordCommand>(req), "Reset password failed.");
        }
        public async Task<Result> VerifyEmailAsync(VerifyEmailRequest request)
        {
            return await ProcessRequestAsync(request, req => _mapper.Map<VerifyEmailCommand>(req), "Email verification failed.");
        }
        private Result ValidateInputs(object input)
        {
            if (input == null)
            {
                return Result.Failure("Input cannot be null.");
            }

            var properties = input.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(input) as string;

                if (value == null || string.IsNullOrWhiteSpace(value))
                {
                    if (input is LoginRequest loginRequest)
                    {
                        if (string.IsNullOrWhiteSpace(loginRequest.Username) && string.IsNullOrWhiteSpace(loginRequest.Email))
                        {
                            return Result.Failure("Either Username or Email must be provided.");
                        }
                    }
                    else
                    {
                        return Result.Failure($"The {property.Name} field must not be null or empty.");
                    }
                }
            }

            return Result.Success();
        }
    }
}