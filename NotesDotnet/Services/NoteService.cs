using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotesDotnet.Models;

namespace NotesDotnet.Services
{
    public class NoteService : CollectionService<Note>, INoteService
    {
        public NoteService(IOptions<NotesDatabaseSettings> notesDbSettings)
            : base(notesDbSettings) { }

        public async Task<List<Note>> GetNotesByFolderId(string id)
        {
            return await _collection.Find(item => item.FolderId == id).ToListAsync();
        }
    }
}
