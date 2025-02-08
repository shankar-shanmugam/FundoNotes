using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NLog;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesManager _notesManager;
        private readonly ILogger _logger;
        private readonly IDistributedCache distributedCache;
        private readonly FundoDBContext dBContext;

        public NotesController(INotesManager notesManager, IDistributedCache distributedCache, FundoDBContext dBContext)
        {
            _notesManager = notesManager;
            _logger = LogManager.GetCurrentClassLogger();
            this.distributedCache = distributedCache;
            this.dBContext = dBContext;
        }

        [Authorize]
        [HttpPost("Create")]
        public IActionResult CreateNotes(NotesModel notesModel)
        {
            try
            {
                _logger.Info("Attempting to create new note");
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var notesResult = _notesManager.CreateNotes(notesModel, userId.Value);
                if (notesResult != null)
                {
                    _logger.Info($"Note created successfully. NoteId: {notesResult.NotesId}");
                    return Ok(new { success = true, message = "Notes Creation Successful ", data = notesResult });
                }
                else
                {
                    _logger.Warn("Failed to create note for user {UserId}", userId);
                    return BadRequest(new { success = false, message = "Notes Creation Unsuccessful" });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating note");
                return StatusCode(500, new { success = false, message = $"error occured:{ex.Message} " });
            }
        }

        [Authorize]
        [HttpGet("RetrieveAllNotes")]
        public IActionResult GetAllNotes()
        {
            try
            {
                _logger.Info("Attempting to retrieve all notes");
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var listNotesResult = _notesManager.GetAllNotes(userId.Value);

                if (!listNotesResult.Any())
                {
                    _logger.Info("No notes found for user {UserId}", userId);
                    return NotFound(new { success = false, message = "No notes found for this user." });
                }

                _logger.Info("Retrieved {Count} notes for user {UserId}", listNotesResult.Count, userId);
                return Ok(new ResponseModel<List<NotesEntity>>
                {
                    Success = true,
                    Message = "All notes retrieved successfully",
                    Data = listNotesResult
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving notes");
                return StatusCode(500, new { success = false, message = $"Exception occurred: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet("RetrieveNote")]
        public IActionResult RetrieveNotes(int noteId)
        {
            try
            {
                _logger.Info("Attempting to retrieve note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var notesResult = _notesManager.GetNotes(userId.Value, noteId);

                if (notesResult == null)
                {
                    _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId);
                    return NotFound(new { success = false, message = $"No note found with ID {noteId} for user {userId}." });
                }

                _logger.Info("Successfully retrieved note {NoteId}", noteId);
                return Ok(new ResponseModel<NotesEntity>
                {
                    Success = true,
                    Message = "Note retrieved successfully.",
                    Data = notesResult
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPut("UpdateNotes")]
        public IActionResult UpdateNotes(int noteId, NotesModel model)
        {
            try
            {
                _logger.Info("Attempting to update note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var UpdatedNote = _notesManager.UpdateNotes(userId.Value, noteId, model);
                if (UpdatedNote == null)
                {
                    _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId);
                    return NotFound(new
                    {
                        success = false,
                        message = $"No note found with ID {noteId} for user {userId}."
                    });
                }
                _logger.Info("Successfully updated note {NoteId}", noteId);
                return Ok(new ResponseModel<NotesEntity> { Success = true, Data = UpdatedNote, Message = $"Notes:{noteId} updated successfully for user {userId} " });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpDelete("DeleteNote")]
        public IActionResult DeleteNote(int noteId)
        {
            try
            {
                _logger.Info("Attempting to delete note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var IsDeleted = _notesManager.DeleteNote(userId.Value, noteId);

                if (!IsDeleted)
                {
                    _logger.Warn("Note {NoteId} not found or not deleted for user {UserId}", noteId, userId);
                    return NotFound(new { success = true, message = "Notes not found or not deleted" });
                }
                _logger.Info("Successfully deleted note {NoteId}", noteId);
                return Ok(new { success = true, message = $"Deleted the Notes:{noteId} Successfully " });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while deleting note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Pin")]
        public IActionResult PinNotes(int noteId)
        {
            try
            {
                _logger.Info("Attempting to toggle pin status for note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                bool result = _notesManager.IsPinUnPinNotes(userId.Value, noteId);

                if (result)
                {
                    _logger.Info("Successfully updated pin status for note {NoteId}", noteId);
                    return Ok(new { success = true, message = "Pin updated successfully." });
                }

                _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId.Value);
                return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating pin status for note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        private int? GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            _logger.Info("Attempting to get user ID from claims");
            return int.TryParse(idClaim, out int userId) ? userId : (int?)null;
        }

        [Authorize]
        [HttpPatch("Archive")]
        public IActionResult ArchiveNotes(int noteId)
        {
            try
            {
                _logger.Info("Attempting to toggle archive status for note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                bool result = _notesManager.ToggleArchive(userId.Value, noteId);

                if (result)
                {
                    _logger.Info("Successfully updated archive status for note {NoteId}", noteId);
                    return Ok(new { success = true, message = "Archive status updated successfully." });
                }

                _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId.Value);
                return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating archive status for note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Trash")]
        public IActionResult TrashNotes(int noteId)
        {
            try
            {
                _logger.Info("Attempting to toggle trash status for note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                bool result = _notesManager.ToggleTrash(userId.Value, noteId);

                if (result)
                {
                    _logger.Info("Successfully updated trash status for note {NoteId}", noteId);
                    return Ok(new { success = true, message = "Trash status updated successfully." });
                }

                _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId.Value);
                return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating trash status for note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Image")]
        public IActionResult ImageNotes(string image, int noteId)
        {
            try
            {
                _logger.Info("Attempting to add image to note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _notesManager.ImageNotes(image, noteId, userId.Value);
                if (result == null)
                {
                    _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId.Value);
                    return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
                }
                _logger.Info("Successfully added image to note {NoteId}", noteId);
                return Ok(new ResponseModel<string> { Success = true, Message = "Image Added successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while adding image to note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("BackgroundColor")]
        public IActionResult ColorForNotes(string color, int noteId)
        {
            try
            {
                _logger.Info("Attempting to update background color for note {NoteId}", noteId);
                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.Warn("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _notesManager.BackgroundColor(color, noteId, userId.Value);
                if (result == null)
                {
                    _logger.Warn("Note {NoteId} not found for user {UserId}", noteId, userId.Value);
                    return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
                }
                _logger.Info("Successfully updated background color for note {NoteId}", noteId);
                return Ok(new ResponseModel<string> { Success = true, Message = "color Added successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating background color for note {NoteId}", noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Remainder")]
        public IActionResult AddRemainder(DateTime remainder, int noteId)
        {
            try
            {
                _logger.Info(" Entered Into Add remainder method ");
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _notesManager.AddRemainder(remainder, noteId, userId.Value);
                if (result == null)
                {
                    _logger.Warn($"Note id:{noteId} not found,so result is null ");
                    return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
                }
                _logger.Info("Remainder added successfully");
                return Ok(new ResponseModel<string> { Success = true, Message = "Remainder Added successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message,"Error occured while Adding Remainder to note {noteId}",noteId);
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }
        [Authorize]
        [HttpGet("redis")]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "NotesList";
            string SerializedNotes;
            var notesList = new List<NotesEntity>();
            var redisNotes = await distributedCache.GetAsync(cacheKey);
            if (redisNotes != null)
            {
                SerializedNotes = Encoding.UTF8.GetString(redisNotes);
                notesList = JsonConvert.DeserializeObject<List<NotesEntity>>(SerializedNotes);
            }
            else
            {
                notesList = dBContext.Notestable.ToList();
                SerializedNotes = JsonConvert.SerializeObject(notesList);
                redisNotes = Encoding.UTF8.GetBytes(SerializedNotes);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                await distributedCache.SetAsync(cacheKey, redisNotes, options);
            }
            return Ok(notesList);

        }


    }
}
