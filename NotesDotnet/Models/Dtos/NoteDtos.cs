using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesDotnet.Models.Dtos
{
    public class CreateUpdateNoteDto
    {
        public required string Title { get; set; }
        public string? Content { get; set; }
        public required string FolderId { get; set; }
    }

    public class UpdateNoteDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? FolderId { get; set; }
    }
}
