namespace Forum.Application.DTOs
{
    public class CategoryWithPostsDto
    {
        public CategoryDto Category { get; set; }
        public List<PostDto> Posts { get; set; }
    }
}