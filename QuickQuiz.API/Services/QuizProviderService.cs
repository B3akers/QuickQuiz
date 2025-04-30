using MongoDB.Bson;
using MongoDB.Driver;
using QuickQuiz.API.Database;
using QuickQuiz.API.Database.Structures;
using QuickQuiz.API.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickQuiz.API.Services
{
    public class QuizProviderService : IQuizProvider
    {
        private readonly MongoContext _mongoContext;
        public QuizProviderService(MongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        public async Task<List<Category>> GetCategoriesAsync(IEnumerable<string> include, IEnumerable<string> skip)
        {
            var find = new BsonDocument[] {
                new BsonDocument("$match", new BsonDocument("$and", new BsonArray{
                    new BsonDocument("_id", new BsonDocument("$nin", new BsonArray( skip.Select(ObjectId.Parse) ))),
                    new BsonDocument("_id", new BsonDocument("$in", new BsonArray( include.Select(ObjectId.Parse) )))
                }))
            };

            return (await(await _mongoContext.Categories.AggregateAsync<Category>(find)).ToListAsync());
        }

        public async Task<List<Category>> GetRandomCategoriesAsync(int number, int minimumQuestionCount, IEnumerable<string> skip)
        {
            var find = new BsonDocument[] {
                new BsonDocument("$match", new BsonDocument("$and", new BsonArray{
                    new BsonDocument("_id", new BsonDocument("$nin", new BsonArray( skip.Select(ObjectId.Parse) ))),
                    new BsonDocument("QuestionCount", new BsonDocument("$gte", minimumQuestionCount) )
                })),
                new BsonDocument("$sample", new BsonDocument("size", number))
            };

            return (await (await _mongoContext.Categories.AggregateAsync<Category>(find)).ToListAsync());
        }

        public async Task<List<Question>> GetRandomQuestionsFromCategoryAsync(string categoryId, int number, IEnumerable<string> skip)
        {
            var find = new BsonDocument[] {
                new BsonDocument("$match", new BsonDocument("$and", new BsonArray{ new BsonDocument("_id", new BsonDocument("$nin", new BsonArray(skip.Select(ObjectId.Parse)))), new BsonDocument("Categories", ObjectId.Parse(categoryId)) })),
                new BsonDocument("$sample", new BsonDocument("size", number))
            };

            return (await (await _mongoContext.Questions.AggregateAsync<Question>(find)).ToListAsync());
        }

        public async Task IncreasePopularityAsync(string categoryId, long count)
        {
            await _mongoContext.Categories.UpdateOneAsync(x => x.Id == categoryId, Builders<Category>.Update.Inc(x => x.Popularity, count));
        }
    }
}
