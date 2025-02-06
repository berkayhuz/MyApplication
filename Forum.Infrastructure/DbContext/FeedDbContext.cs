using Forum.Domain.Entities.FeedEntities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Forum.Infrastructure.DbContext
{
    public class FeedDbContext
    {
        private readonly IMongoDatabase _database;

        public FeedDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<CategoryEntity> Categories
            => _database.GetCollection<CategoryEntity>("Categories");

        public IMongoCollection<PostEntity> Posts
            => _database.GetCollection<PostEntity>("Posts");
    }
}
