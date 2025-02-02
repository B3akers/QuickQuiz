using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace QuickQuiz.API.Database.Structures
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public long QuestionCount { get; set; }
        public long Popularity { get; set; }
    }
}
