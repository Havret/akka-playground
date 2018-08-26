using MongoDB.Driver;
using Storage.Service.Book;

namespace Storage.Service
{
    public class StorageContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public StorageContext()
        {
            _client = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://localhost"));
            _database = _client.GetDatabase("Storage");
        }

        public IMongoCollection<BookReadModel> Books => _database.GetCollection<BookReadModel>("books");
        public IMongoCollection<ProjectionOffset> ProjectionOffsets => _database.GetCollection<ProjectionOffset>("projectionOffsets");
    }
}