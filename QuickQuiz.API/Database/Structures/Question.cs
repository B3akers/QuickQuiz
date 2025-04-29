using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace QuickQuiz.API.Database.Structures
{
    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public int CorrectAnswer { get; set; }
        public List<string> Answers { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Categories { get; set; }
    }
}
