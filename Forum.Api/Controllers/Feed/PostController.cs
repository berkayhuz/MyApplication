using AutoMapper;
using Forum.Api.Filters;
using Forum.Application.Commands.FeedCommands;
using Forum.Application.DTOs;
using Forum.Application.Interfaces;
using Forum.Application.Requests.Feed;
using Forum.Domain.Entities.FeedEntities;
using Forum.Infrastructure.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers.Feed
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFeedRepository _feedRepository;
        private readonly IMapper _mapper;
        private readonly IS3FileStorageService _fileStorageService;

        public PostController(IMediator mediator,IFeedRepository feedRepository, IMapper mapper, IS3FileStorageService fileStorageService)
        {
            _mediator = mediator;
            _feedRepository = feedRepository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }
        [Authorize]
        [HttpPost("create-post")]
        [ServiceFilter(typeof(JwtTokenValidationFilter))]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
        {
            var userId = (Guid)Request.HttpContext.Items["userId"];
            var command = _mapper.Map<CreatePostCommand>(request);
            command.UserId = userId.ToString();

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, [FromForm] UpdatePostRequest request, CancellationToken cancellationToken)
        {
            var post = _mapper.Map<PostEntity>(request);
            post.Id = id;

            foreach (var imageFile in request.ImageFiles)
            {
                var imageUrl = await _fileStorageService.UploadImageAsync(imageFile);
                post.ImageUrls.Add(imageUrl);
            }

            var result = await _feedRepository.UpdatePostAsync(id, post);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var postDto = _mapper.Map<PostDto>(post);
            return Ok(postDto);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetPostsByCategorySlug(string slug, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 75, CancellationToken cancellationToken = default)
        {
            var posts = await _feedRepository.GetPostsByCategorySlugAsync(slug, pageNumber, pageSize);
            if (posts == null || !posts.Any())
            {
                return NotFound("No posts found for this category.");
            }

            var postDtos = _mapper.Map<List<PostDto>>(posts);
            return Ok(postDtos);
        }

    }
}