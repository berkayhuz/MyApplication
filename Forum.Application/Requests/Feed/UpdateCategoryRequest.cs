using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Forum.Api.Controllers
{
    public class UpdateCategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int SortOrder { get; set; }
        [Required]
        public string MetaTitle { get; set; }
        [Required]
        public string MetaDescription { get; set; }
        [Required]
        public string Keywords { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public IFormFile ImageFile { get; set; }
    }
}