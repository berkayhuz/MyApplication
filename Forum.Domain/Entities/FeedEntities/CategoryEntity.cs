using Forum.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Forum.Domain.Entities.FeedEntities
{
    public class CategoryEntity : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("image_url")]
        public string ImageUrl { get; set; }

        [BsonElement("slug")]
        public string Slug { get; set; }

        [BsonElement("sort_order")]
        public int SortOrder { get; set; }

        [BsonElement("meta_title")]
        public string MetaTitle { get; set; }

        [BsonElement("meta_description")]
        public string MetaDescription { get; set; }

        [BsonElement("keywords")]
        public string Keywords { get; set; }

        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("deleted_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DeletedAt { get; set; }

        [BsonElement("is_active")]
        public bool IsActive { get; set; }

        [BsonIgnoreIfNull]
        public ICollection<PostEntity> Posts { get; set; }

        public DocumentVersion Version { get; set; } = new DocumentVersion { Major = 1, Minor = 0, Patch = 0 };
    }
}
