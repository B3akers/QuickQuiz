using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace QuickQuiz.API.Database.Structures
{
    public class QuestionReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string QuestionId { get; set; }
        public int Reason { get; set; }
        public string SenderIp { get; set; }
    }
}
