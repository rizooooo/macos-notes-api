using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NotesDotnet.Models;
using NotesDotnet.Models.Dtos;
using NotesDotnet.Services;

namespace NotesDotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet(Name = "GetNotes")]
        public async Task<IEnumerable<Note>> GetNotes()
        {
            return await _noteService.GetAsync();
        }

        [HttpGet("{id}", Name = "GetNoteById")]
        public async Task<ActionResult<Note?>> GetNoteById(string? id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                if (id == null)
                {
                    return BadRequest("Note name cannot be empty.");
                }

                var note = await _noteService.GetAsync(id);
                // this is throwing an exception if current folder is not present

                if (note == null)
                {
                    return NotFound("Note not found");
                }

                return Ok(note);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("folder/{id}", Name = "GetNotesByFolderId")]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotesByFolderId(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                var note = await _noteService.GetNotesByFolderId(id);
                // this is throwing an exception if current folder is not present

                if (note == null)
                {
                    return NotFound("Note not found");
                }

                return Ok(note);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "CreateNote")]
        public async Task<ActionResult<Note>> CreateNote([FromBody] CreateUpdateNoteDto dto)
        {
            try
            {
                if (dto.FolderId == null)
                {
                    return BadRequest();
                }

                var note = new Note
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    FolderId = dto.FolderId,
                };

                await _noteService.CreateAsync(note);

                return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error creating folder");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}", Name = "UpdateNote")]
        public async Task<ActionResult> UpdateNote(string id, [FromBody] UpdateNoteDto dto)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _) || !ObjectId.TryParse(dto.FolderId, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                var note = await _noteService.GetAsync(id);

                if (note == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(dto.Title))
                {
                    note.Title = dto.Title;
                }

                if (!string.IsNullOrEmpty(dto.Content))
                {
                    note.Content = dto.Content;
                }

                if (!string.IsNullOrEmpty(dto.FolderId))
                {
                    note.FolderId = dto.FolderId;
                }

                await _noteService.UpdateAsync(
                    id,
                    new Note
                    {
                        Id = note.Id,
                        Content = note.Content,
                        Title = note.Title,
                        Folder = note.Folder,
                        FolderId = note.FolderId,
                    }
                );

                return Ok(note);
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error creating folder");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}", Name = "DeleteNote")]
        public async Task<ActionResult> DeleteNote(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid ID format. ID must be a 24-character Hex string.");
                }

                var note = await _noteService.GetAsync(id);

                if (note == null)
                {
                    return NotFound();
                }

                await _noteService.RemoveAsync(id);

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
