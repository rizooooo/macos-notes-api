using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotesDotnet.Models
{
    public abstract class MongoEntity
    {
        // You can use [BsonId] here to ensure MongoDB treats it as the primary key
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}
