using Forum.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Forum.Domain.Entities.FeedEntities
{
    public class PostEntity : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("category_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [BsonIgnore]
        public CategoryEntity CategoryEntity { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("slug")]
        public string Slug { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("image_urls")]
        public List<string> ImageUrls { get; set; } = new List<string>();

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
        public DateTime UpdatedAt { get; set; }

        [BsonElement("deleted_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DeletedAt { get; set; }

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("is_archived")]
        public bool IsArchived { get; set; } = false;

        public DocumentVersion Version { get; set; } = new DocumentVersion { Major = 1, Minor = 0, Patch = 0 };
    }
}