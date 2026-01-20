using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotesDotnet.Models;

namespace NotesDotnet.Services
{
    public class FolderService : CollectionService<Folder>, IFolderService
    {
        public FolderService(IOptions<NotesDatabaseSettings> notesDbSettings)
            : base(notesDbSettings) { }

        public async Task<bool> GetChildFoldersById(string id)
        {
            return await _collection.CountDocumentsAsync(item => item.ParentId == id) > 0;
        }

        public async Task<List<string>> GetFolderAncestry(string folderId)
        {
            List<string> ancestors = [];

            // 1. Get the starting folder safely
            var currentFolder = await _collection.Find(x => x.Id == folderId).FirstOrDefaultAsync();

            // Safety check: if folder doesn't exist or has no parent, return empty
            if (currentFolder == null || string.IsNullOrEmpty(currentFolder.ParentId))
            {
                return ancestors;
            }

            // 2. Start the loop with the first parent
            string? nextParentId = currentFolder.ParentId;

            while (!string.IsNullOrEmpty(nextParentId))
            {
                ancestors.Add(nextParentId);

                // Fetch the next folder up the chain
                // We only select ParentId to save bandwidth (Projection)
                var parent = await _collection
                    .Find(x => x.Id == nextParentId)
                    .Project(x => new { x.ParentId })
                    .FirstOrDefaultAsync();

                // Prepare for next iteration
                nextParentId = parent?.ParentId;
            }

            return ancestors;
        }

        public async Task<IEnumerable<Folder>> GetFoldersById(string id)
        {
            return await _collection.Find(item => item.ParentId == id).ToListAsync();
        }

        public async Task<IEnumerable<Folder>> GetParentFolders()
        {
            return await _collection.Find(item => item.ParentId == null).ToListAsync();
        }
    }
}
