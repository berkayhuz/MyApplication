using MongoDB.Bson;
using MongoDB.Driver;

namespace Forum.Infrastructure.Migrations
{
    public class Migration
    {
        private readonly IMongoDatabase _database;

        public Migration(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task MigrateAsync()
        {
            await CreatePostCollectionAsync();
            await CreateCategoryCollectionAsync();
        }

        private async Task CreatePostCollectionAsync()
        {
            var collectionName = "posts";
            if (!CollectionExists(collectionName))
            {
                await _database.CreateCollectionAsync(collectionName);
            }

            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var indexKeys = Builders<BsonDocument>.IndexKeys
                .Ascending("user_id")
                .Ascending("category_id")
                .Ascending("slug")
                .Ascending("created_at");

            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);

            await collection.Indexes.CreateOneAsync(indexModel);
        }

        private async Task CreateCategoryCollectionAsync()
        {
            var collectionName = "categories";
            if (!CollectionExists(collectionName))
            {
                await _database.CreateCollectionAsync(collectionName);
            }

            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var indexKeys = Builders<BsonDocument>.IndexKeys
                .Ascending("slug")
                .Ascending("sort_order");

            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);

            await collection.Indexes.CreateOneAsync(indexModel);
        }

        private bool CollectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }
    }
}
