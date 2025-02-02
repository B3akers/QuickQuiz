using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuickQuiz.API.Database.Structures;
using QuickQuiz.API.Settings;

namespace QuickQuiz.API.Database
{
    public class MongoContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        public IMongoCollection<Category> Categories { get; private set; }

        public MongoContext(IOptions<MongoSettings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _database = _client.GetDatabase(settings.Value.DatabaseName);

            Categories = _database.GetCollection<Category>("categories");
        }
    }
}
