using AutoMapper;
using Forum.Application.Commands.FeedCommands;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.FeedEntities;
using Forum.Domain.Models;
using Forum.Infrastructure.FileStorage;
using MediatR;

namespace Forum.Application.CommandHandlers.FeedHandlers
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IFeedRepository _feedRepository;
        private readonly IS3FileStorageService _fileStorageService;

        public CreatePostCommandHandler(IMapper mapper, IUserRepository userRepository, IFeedRepository feedRepository, IS3FileStorageService fileStorageService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _feedRepository = feedRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var category = await _feedRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category == null)
            {
                return Result.Failure("Category not found.");
            }

            var post = _mapper.Map<PostEntity>(request);
            post.UserId = user.Id.ToString();

            foreach (var imageFile in request.ImageFiles)
            {
                var imageUrl = await _fileStorageService.UploadImageAsync(imageFile);
                post.ImageUrls.Add(imageUrl);
            }

            await _feedRepository.AddPostAsync(post);

            return Result.Success("Post created successfully.");
        }
    }
}
