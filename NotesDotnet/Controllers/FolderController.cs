using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NotesDotnet.Models;
using NotesDotnet.Models.Dtos;
using NotesDotnet.Services;

namespace NotesDotnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpGet(Name = "GetFolders")]
        public async Task<FoldersReturnResponse> GetFolders([FromQuery] QueryGetFolders query)
        {
            var folders = await _folderService.GetParentFolders();
            List<string> ancestries = [];
            // 1. Create a list of Tasks without awaiting them yet
            var tasks = folders.Select(async item =>
            {
                item.HasChildren = await _folderService.GetChildFoldersById(item.Id);
            });

            if (!string.IsNullOrEmpty(query.FolderId))
            {
                ancestries = await _folderService.GetFolderAncestry(query.FolderId);
            }

            // 2. Run all tasks in parallel
            await Task.WhenAll(tasks);

            var folderDtos = folders
                .Select(f => new FolderDto
                {
                    Id = f.Id!,
                    Name = f.Name,
                    HasChildren = f.HasChildren,
                    ParentId = f.ParentId,
                })
                .ToList();

            return new FoldersReturnResponse
            {
                Folders = folderDtos,
                CurrentFolderAncestry = ancestries,
            };
        }

        [HttpGet("{id}", Name = "GetFolderById")]
        public async Task<ActionResult<FolderDto?>> GetFolderById(string? id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                if (id == null)
                {
                    return BadRequest("Folder name cannot be empty.");
                }

                var folder = await _folderService.GetAsync(id);
                // this is throwing an exception if current folder is not present

                if (folder == null)
                {
                    return NotFound("Folder not found");
                }

                var item = new FolderDto
                {
                    Id = folder.Id!,
                    Name = folder.Name,
                    HasChildren = folder.HasChildren,
                    ParentId = folder.ParentId!,
                    Ancestry = await _folderService.GetFolderAncestry(folder.Id!),
                };

                return Ok(item);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/folders", Name = "GetFoldersById")]
        public async Task<ActionResult<List<FolderDto?>>> GetFoldersById(string? id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                if (id == null)
                {
                    return BadRequest("Folder name cannot be empty.");
                }

                var folders = await _folderService.GetFoldersById(id);

                var tasks = folders.Select(async item =>
                {
                    item.HasChildren = await _folderService.GetChildFoldersById(item.Id);
                });

                // 2. Run all tasks in parallel
                await Task.WhenAll(tasks);

                var folderDtos = folders
                    .Select(f => new FolderDto
                    {
                        Id = f.Id!,
                        Name = f.Name,
                        HasChildren = f.HasChildren,
                        ParentId = f.ParentId!,
                        // Ancestry = await _folderService.GetFolderAncestry(f.Id!),
                    })
                    .ToList();

                return Ok(folderDtos);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "CreateFolder")]
        public async Task<ActionResult<FolderDto>> CreateFolder([FromBody] CreateFolderDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest("Folder name cannot be empty.");
                }

                if (
                    !string.IsNullOrWhiteSpace(dto.ParentId)
                    && !ObjectId.TryParse(dto.ParentId, out _)
                )
                {
                    return BadRequest("Invalid parent id format");
                }

                var newFolder = new Folder { Name = dto.Name, ParentId = dto.ParentId };

                await _folderService.CreateAsync(newFolder);

                return CreatedAtAction(nameof(GetFolderById), new { id = newFolder.Id }, newFolder);
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error creating folder");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}", Name = "UpdateFolder")]
        public async Task<ActionResult> UpdateFolder(string id, [FromBody] UpdateFolderDto dto)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest("Folder name cannot be empty.");
                }

                var currentFolder = await _folderService.GetAsync(id);

                if (currentFolder == null)
                {
                    return NotFound();
                }

                currentFolder.Name = dto.Name;

                await _folderService.UpdateAsync(
                    id,
                    new Folder
                    {
                        Id = currentFolder.Id,
                        ParentFolder = currentFolder.ParentFolder,
                        Name = currentFolder.Name,
                        Description = currentFolder.Description,
                        ParentId = currentFolder.ParentId,
                    }
                );

                return Ok(currentFolder);
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error creating folder");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}", Name = "DeleteFolder")]
        public async Task<ActionResult> DeleteFolder(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                var currentFolder = await _folderService.GetAsync(id);

                if (currentFolder == null)
                {
                    return NotFound();
                }

                await _folderService.RemoveAsync(id);

                // 3. MAGIC IS HERE: newFolder.Id is now populated
                return NoContent();
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error creating folder");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
