using AutoMapper;
using Forum.Application.Interfaces;
using Forum.Application.Requests.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers.Identity
{
    [Route("api/identity/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtHelper;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthorizationController(IMediator mediator, IMapper mapper, IIdentityService identityService, IJwtTokenGenerator jwtHelper, IUserRepository userRepository)
        {
            _mediator = mediator;
            _mapper = mapper;
            _identityService = identityService;
            _jwtHelper = jwtHelper;
            _userRepository = userRepository;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request == null || !request.IsValid())
            {
                return BadRequest("Username or email is required.");
            }

            var result = await _identityService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return Unauthorized(result.Message);
            }

            return Ok(new { Message = result.Message, Data = result.Data });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            var result = await _identityService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] VerifyEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest("Invalid token.");
            }

            var result = await _identityService.VerifyEmailAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _identityService.ForgotPasswordAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest("Invalid token.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("New password is required.");
            }

            var result = await _identityService.ResetPasswordAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok("Password has been successfully reset.");
        }
    }
}
