using Microsoft.AspNetCore.Http;

namespace Forum.Application.Requests.Feed
{
    public class CreatePostRequest
    {
        public string CategoryId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public List<IFormFile> ImageFiles { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
    }
}
