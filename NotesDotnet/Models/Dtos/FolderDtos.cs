namespace NotesDotnet.Models.Dtos
{
    public class FoldersReturnResponse
    {
        public required List<FolderDto> Folders { get; set; }
        public List<string>? CurrentFolderAncestry { get; set; }
    }

    public class QueryGetFolders
    {
        public string? FolderId { get; set; }
    }

    public class CreateFolderDto
    {
        public required string Name { get; set; }
        public string? ParentId { get; set; }
    }

    public class UpdateFolderDto
    {
        public required string Name { get; set; }
    }

    public class FolderDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string ParentId { get; set; }
        public bool HasChildren { get; set; }

        public List<string>? Ancestry { get; set; }
    }
}
