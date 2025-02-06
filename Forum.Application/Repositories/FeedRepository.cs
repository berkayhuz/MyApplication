using AutoMapper;
using FluentValidation;
using Forum.Application.DTOs;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.FeedEntities;
using Forum.Domain.Models;
using Forum.Infrastructure.DbContext;
using Forum.Infrastructure.FileStorage;
using Forum.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace Forum.Application.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private readonly FeedDbContext _context;
        private readonly IMapper _mapper;
        private readonly IS3FileStorageService _fileStorageService;
        private readonly IValidator<CategoryEntity> _categoryValidator;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;

        public FeedRepository(IUserRepository userRepository, IProfileRepository profileRepository, FeedDbContext context, IMapper mapper, IS3FileStorageService fileStorageService, IValidator<CategoryEntity> categoryValidator)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _categoryValidator = categoryValidator;
        }
        #region Category Methods
        public async Task<List<CategoryEntity>> GetAllCategoriesAsync()
        {
            var filter = Builders<CategoryEntity>.Filter.Eq(c => c.IsActive, true);
            var sort = Builders<CategoryEntity>.Sort.Ascending(c => c.SortOrder);
            return await _context.Categories.Find(filter).Sort(sort).ToListAsync();
        }

        public async Task<CategoryEntity> GetCategoryByIdAsync(string id)
        {
            return await _context.Categories.Find(category => category.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CategoryEntity> GetCategoryByNameAsync(string name)
        {
            var filter = Builders<CategoryEntity>.Filter.And(
                Builders<CategoryEntity>.Filter.Eq(c => c.Name, name),
                Builders<CategoryEntity>.Filter.Eq(c => c.IsActive, true)
            );
            return await _context.Categories.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Result> AddCategoryAsync(CategoryEntity category, IFormFile imageFile)
        {
            var validationResult = await _categoryValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            category.Slug = SlugHelper.GenerateSlug(category.Name);

            var existingCategory = await _context.Categories
                .Find(c => c.Name == category.Name && c.Slug == category.Slug && c.IsActive)
                .FirstOrDefaultAsync();

            if (existingCategory != null)
            {
                return Result.Failure("A category with the same name and slug already exists.");
            }

            category.SortOrder = await GetUniqueSortOrder(category.SortOrder);

            if (imageFile != null)
            {
                category.ImageUrl = await _fileStorageService.UploadImageAsync(imageFile);
            }

            await _context.Categories.InsertOneAsync(category);
            return Result.Success("Category added successfully.", category);
        }

        public async Task<Result> UpdateCategoryAsync(string id, CategoryEntity category, IFormFile imageFile)
        {
            var validationResult = await _categoryValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            category.Slug = SlugHelper.GenerateSlug(category.Name);

            var existingCategory = await _context.Categories.Find(c =>
                c.Name == category.Name && c.Slug == category.Slug && c.Id != id && c.IsActive).FirstOrDefaultAsync();

            if (existingCategory != null)
            {
                return Result.Failure("A category with the same name and slug already exists.");
            }

            category.SortOrder = await GetUniqueSortOrder(category.SortOrder, id);

            if (imageFile != null)
            {
                category.ImageUrl = await _fileStorageService.UploadImageAsync(imageFile);
            }
            else
            {
                var currentCategory = await _context.Categories.Find(c => c.Id == id).FirstOrDefaultAsync();
                category.ImageUrl = currentCategory.ImageUrl;
            }

            var result = await _context.Categories.ReplaceOneAsync(c => c.Id == id, category);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return Result.Success("Category updated successfully.", category);
            }
            return Result.Failure("Failed to update category.");
        }

        public async Task<bool> DeleteCategoryAsync(string name)
        {
            var result = await _context.Categories.DeleteOneAsync(category => category.Slug == name);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        #endregion

        public async Task<List<PostDto>> GetPostsByCategorySlugAsync(string slug, int pageNumber, int pageSize = 75)
        {
            var category = await _context.Categories
                                         .Find(c => c.Slug == slug && c.IsActive)
                                         .Project(c => new { c.Id, c.Name, c.Slug, c.ImageUrl })
                                         .FirstOrDefaultAsync();

            if (category == null) return new List<PostDto>();

            var filter = Builders<PostEntity>.Filter.And(
                Builders<PostEntity>.Filter.Eq(p => p.CategoryId, category.Id),
                Builders<PostEntity>.Filter.Eq(p => p.IsArchived, false),
                Builders<PostEntity>.Filter.Eq(p => p.IsActive, true)
            );

            var skip = (pageNumber - 1) * pageSize;

            var posts = await _context.Posts.Find(filter)
                                            .Skip(skip)
                                            .Limit(pageSize)
                                            .Project(post => new PostEntity
                                            {
                                                UserId = post.UserId,
                                                CategoryId = post.CategoryId,
                                                Title = post.Title,
                                                Slug = post.Slug,
                                                Content = post.Content,
                                                ImageUrls = post.ImageUrls,
                                                MetaTitle = post.MetaTitle,
                                                MetaDescription = post.MetaDescription,
                                                Keywords = post.Keywords,
                                                CreatedAt = post.CreatedAt,
                                                IsActive = post.IsActive,
                                                IsArchived = post.IsArchived
                                            })
                                            .ToListAsync();

            var postDtos = new List<PostDto>();


            foreach (var post in posts)
            {
                var userProfileAggregate = await _profileRepository.GetByUserProfileAsync(Guid.Parse(post.UserId));

                if (userProfileAggregate == null) continue;

                var postDto = _mapper.Map<PostDto>(post);
                postDto.User = _mapper.Map<UserpostDto>(userProfileAggregate);

                postDto.PostCategory = new PostCategoryDto
                {
                    Name = category.Name,
                    Slug = category.Slug,
                    ImageUrl = category.ImageUrl
                };

                postDtos.Add(postDto);
            }

            return postDtos;
        }

        public async Task<List<PostDto>> GetPostsByUserIdAsync(string userId, int pageNumber, int pageSize = 75)
        {
            var filter = Builders<PostEntity>.Filter.And(
               Builders<PostEntity>.Filter.Eq(p => p.UserId, userId),
               Builders<PostEntity>.Filter.Eq(p => p.IsArchived, false),
               Builders<PostEntity>.Filter.Eq(p => p.IsActive, true)
           );

            var skip = (pageNumber - 1) * pageSize;

            var posts = await _context.Posts.Find(filter)
                                             .Skip(skip)
                                             .Limit(pageSize)
                                             .Project(post => new PostEntity
                                             {
                                                 UserId = post.UserId,
                                                 CategoryId = post.CategoryId,
                                                 Title = post.Title,
                                                 Slug = post.Slug,
                                                 Content = post.Content,
                                                 ImageUrls = post.ImageUrls,
                                                 MetaTitle = post.MetaTitle,
                                                 MetaDescription = post.MetaDescription,
                                                 Keywords = post.Keywords,
                                                 CreatedAt = post.CreatedAt,
                                                 IsActive = post.IsActive,
                                                 IsArchived = post.IsArchived
                                             })
                                             .ToListAsync();

            var postDtos = new List<PostDto>();

            foreach (var post in posts)
            {
                var category = await _context.Categories
                                             .Find(c => c.Id == post.CategoryId && c.IsActive)
                                             .Project(c => new { c.Name, c.Slug, c.ImageUrl })
                                             .FirstOrDefaultAsync();

                if (category == null) continue;

                var postDto = _mapper.Map<PostDto>(post);

                postDto.PostCategory = new PostCategoryDto
                {
                    Name = category.Name,
                    Slug = category.Slug,
                    ImageUrl = category.ImageUrl
                };

                postDtos.Add(postDto);
            }

            return postDtos;
        }

        #region Add & Update Post Methods 
        public async Task<Result> AddPostAsync(PostEntity post)
        {
            await _context.Posts.InsertOneAsync(post);
            return Result.Success("Post added successfully.", post);
        }
        public async Task<Result> UpdatePostAsync(string id, PostEntity post)
        {
            var result = await _context.Posts.ReplaceOneAsync(p => p.Id == id, post);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return Result.Success("Post updated successfully.", post);
            }
            return Result.Failure("Failed to update post.");
        }
        #endregion
        #region Private Methods
        private async Task<int> GetUniqueSortOrder(int sortOrder, string id = null)
        {
            var filter = Builders<CategoryEntity>.Filter.And(
                Builders<CategoryEntity>.Filter.Eq(c => c.IsActive, true),
                Builders<CategoryEntity>.Filter.Ne(c => c.Id, id)
            );

            var sortOrderCategory = await _context.Categories.Find(filter).SortByDescending(c => c.SortOrder).FirstOrDefaultAsync();
            if (sortOrderCategory != null && sortOrderCategory.SortOrder >= sortOrder)
            {
                return sortOrderCategory.SortOrder + 1;
            }
            return sortOrder;
        }
        #endregion
    }
}