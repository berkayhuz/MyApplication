using AutoMapper;
using Forum.Application.DTOs;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.FeedEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers.Feed
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IFeedRepository _feedRepository;
        private readonly IMapper _mapper;

        public CategoryController(IFeedRepository feedRepository, IMapper mapper)
        {
            _feedRepository = feedRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = await _feedRepository.GetAllCategoriesAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById(string id, CancellationToken cancellationToken)
        {
            var category = await _feedRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpGet("by-name/{name}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryByName(string name, CancellationToken cancellationToken)
        {
            var category = await _feedRepository.GetCategoryByNameAsync(name);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost("create-category")]
        [Authorize]
        public async Task<IActionResult> AddCategory([FromForm] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<CategoryEntity>(request);
            var result = await _feedRepository.AddCategoryAsync(category, request.ImageFile);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(string id, [FromForm] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<CategoryEntity>(request);
            category.Id = id;
            var result = await _feedRepository.UpdateCategoryAsync(id, category, request.ImageFile);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(string id, CancellationToken cancellationToken)
        {
            var result = await _feedRepository.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}