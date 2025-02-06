namespace Forum.Application.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Slug { get; set; }
        public int SortOrder { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public bool IsActive { get; set; }
    }
}
