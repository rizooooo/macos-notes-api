using NotesDotnet.Models;

namespace NotesDotnet.Services
{
    public interface INoteService : ICollectionService<Note>
    {
        Task<List<Note>> GetNotesByFolderId(string id);
    }
}
