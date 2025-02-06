using AutoMapper;
using Forum.Api.Filters;
using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Common.DTOs;
using Forum.Application.Interfaces;
using Forum.Application.Queries.UserQueries;
using Forum.Application.Requests.Identity;
using Forum.Application.Requests.Member;
using Forum.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers.Member
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public MemberController(IMapper mapper ,IMediator mediator, IJwtTokenGenerator jwtTokenGenerator)
        {
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mediator = mediator;
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(string userName)
        {
            var query = new GetUserProfileQuery(userName);
            var userProfile = await _mediator.Send(query);

            if (userProfile == null)
            {
                return NotFound("Profile not found.");
            }

            if (!User.Identity?.IsAuthenticated ?? false)
            {
                if (userProfile.ProfileVisibility == ProfileVisibility.Private ||
                    userProfile.ProfileVisibility == ProfileVisibility.FriendsOnly)
                {
                    return Ok("Hidden profile.");
                }
            }
            return Ok(userProfile);
        }

        [Authorize]
        [HttpPut("update")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];

            var command = _mapper.Map<UpdateProfileCommand>(request);
            command.UserId = userId;

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        #region Hobbies
        [Authorize]
        [HttpPost("create-hobby")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> AddHobby([FromBody] CreateHobbyRequest request, CancellationToken cancellationToken)
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];

            var command = _mapper.Map<CreateHobbyCommand>(request);
            command.UserId = userId;

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
        [Authorize]
        [HttpGet("hobbies")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> GetHobbies()
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];

            var query = new GetHobbiesQuery { UserId = userId };

            var hobbies = await _mediator.Send(query);

            return Ok(hobbies);
        }
        [Authorize]
        [HttpDelete("delete-hobby/{hobbyId}")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> DeleteHobby(Guid hobbyId)
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];
            var command = new DeleteHobbyCommand
            {
                UserId = userId,
                HobbyId = hobbyId
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }

            return BadRequest(result);
        }
        #endregion

        #region Social Media Links
        [Authorize]
        [HttpPost("create-social-links")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> AddSocialLinks([FromBody] CreateSocialLinksRequest request, CancellationToken cancellationToken)
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];

            var command = _mapper.Map<CreateSocialMediaLinkCommand>(request);
            command.UserId = userId;

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
        [Authorize]
        [HttpGet("social-links")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> GetSocialLinks()
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];

            var query = new GetSocialMediaLinksQuery { UserId = userId };

            var hobbies = await _mediator.Send(query);

            return Ok(hobbies);
        }
        [Authorize]
        [HttpDelete("delete-social-links/{socialLinksId}")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> DeleteSocialLinks(Guid socialLinksId)
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];
            var command = new DeleteSocialMediaLinkCommand
            {
                UserId = userId,
                SocialMediaLinkId = socialLinksId
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }

            return BadRequest(result);
        }
        #endregion
    }
}