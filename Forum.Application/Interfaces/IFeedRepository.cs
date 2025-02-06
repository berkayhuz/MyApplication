using Forum.Application.DTOs;
using Forum.Domain.Entities.FeedEntities;
using Forum.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Forum.Application.Interfaces
{
    public interface IFeedRepository
    {
        Task<List<CategoryEntity>> GetAllCategoriesAsync();
        Task<CategoryEntity> GetCategoryByIdAsync(string id);
        Task<CategoryEntity> GetCategoryByNameAsync(string name);
        Task<Result> AddCategoryAsync(CategoryEntity category, IFormFile imageFile);
        Task<Result> UpdateCategoryAsync(string id, CategoryEntity category, IFormFile imageFile);
        Task<bool> DeleteCategoryAsync(string id);
        Task<List<PostDto>> GetPostsByCategorySlugAsync(string slug, int pageNumber, int pageSize = 75);
        Task<List<PostDto>> GetPostsByUserIdAsync(string userId, int pageNumber, int pageSize = 75);
        Task<Result> AddPostAsync(PostEntity post);
        Task<Result> UpdatePostAsync(string id, PostEntity post);
    }
}