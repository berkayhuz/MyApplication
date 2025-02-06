using Forum.Application.Common.DTOs;

namespace Forum.Application.DTOs
{
    public class PostDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public List<string> ImageUrls { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public UserpostDto User { get; set; }
        public PostCategoryDto PostCategory { get; set; }
    }
}