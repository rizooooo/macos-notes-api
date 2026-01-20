using System.ComponentModel.DataAnnotations.Schema;

namespace NotesDotnet.Models
{
    public class Folder : MongoEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Folder? ParentFolder { get; set; }

        public bool HasChildren { get; set; } = false;
    }
}
