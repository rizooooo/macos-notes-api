using NotesDotnet.Models;

namespace NotesDotnet.Services
{
    public interface IFolderService : ICollectionService<Folder>
    {
        Task<bool> GetChildFoldersById(string id);

        Task<IEnumerable<Folder>> GetFoldersById(string id);

        Task<IEnumerable<Folder>> GetParentFolders();

        Task<List<string>> GetFolderAncestry(string id);
    }
}
