using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotesDotnet.Models
{
    public class Note : MongoEntity
    {
        public required string Title { get; set; }
        public required string Content { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string FolderId { get; set; }

        public Folder? Folder { get; set; }
    }
}
