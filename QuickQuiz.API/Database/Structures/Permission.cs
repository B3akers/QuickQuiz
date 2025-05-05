using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace QuickQuiz.API.Database.Structures
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
