using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<NotesController> _logger;
        private readonly IDistributedCache distributedCache;
        private readonly FundoDBContext dBContext;

        public NotesController(INotesManager notesManager,ILogger<NotesController> logger,IDistributedCache distributedCache,FundoDBContext dBContext)
        {
            this._notesManager = notesManager;
            _logger = logger;
            this.distributedCache = distributedCache;
            this.dBContext = dBContext;
        }

        [Authorize]
        [HttpPost("Create")]
        public IActionResult CreateNotes(NotesModel notesModel)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var notesResult = _notesManager.CreateNotes(notesModel, userId.Value);
                if (notesResult != null)
                {
                    return Ok(new { success = true, message = "Notes Creation Successful ", data = notesResult });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Notes Creation Unsuccessful" });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = $"error occured:{ex.Message} " });
            }

        }
        [Authorize]
        [HttpGet("RetrieveAllNotes")]
        public IActionResult GetAllNotes()
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var listNotesResult = _notesManager.GetAllNotes(userId.Value);

                if (!listNotesResult.Any())
                {
                    return NotFound(new { success = false, message = "No notes found for this user." });
                }

                return Ok(new ResponseModel<List<NotesEntity>>
                {
                    Success = true,
                    Message = "All notes retrieved successfully",
                    Data = listNotesResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Exception occurred: {ex.Message}" });
            }
        }
        [Authorize]
        [HttpGet("RetrieveNote")]
        public IActionResult RetrieveNotes(int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var notesResult = _notesManager.GetNotes(userId.Value, noteId);

                if (notesResult == null)
                {
                    return NotFound(new { success = false, message = $"No note found with ID {noteId} for user {userId}." });
                }

                return Ok(new ResponseModel<NotesEntity>
                {
                    Success = true,
                    Message = "Note retrieved successfully.",
                    Data = notesResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPut("UpdateNotes")]
        public IActionResult UpdateNotes(int noteId,NotesModel model)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var UpdatedNote=_notesManager.UpdateNotes(userId.Value, noteId,model);
                if (UpdatedNote == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"No note found with ID {noteId} for user {userId}."
                    });
                }
                return Ok(new ResponseModel<NotesEntity> { Success = true,Data = UpdatedNote,Message=$"Notes:{noteId} updated successfully for user {userId} " });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpDelete("DeleteNote")]
        public IActionResult DeleteNote(int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var IsDeleted = _notesManager.DeleteNote(userId.Value, noteId);

                if (!IsDeleted)
                {
                    return NotFound(new { success = true, message = "Notes not found or not deleted" });
                }
                return Ok(new { success = true, message = $"Deleted the Notes:{noteId} Successfully " });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Pin")]
        public IActionResult PinNotes(int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                bool result = _notesManager.IsPinUnPinNotes(userId.Value, noteId);

                if (result)
                    return Ok(new { success = true, message = "Pin updated successfully." });

                return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        // Helper method to get User ID from claims
        private int? GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return int.TryParse(idClaim, out int userId) ? userId : (int?)null;
        }

        [Authorize]
        [HttpPatch("Archive")]
        public IActionResult ArchiveNotes(int noteId)
        {
            try
            {

                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                bool result = _notesManager.ToggleArchive(userId.Value, noteId);

                if (result)
                    return Ok(new { success = true, message = "Archive status updated successfully." });

                return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }
        [Authorize]
        [HttpPatch("Trash")]
        public IActionResult TrashNotes(int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                bool result = _notesManager.ToggleTrash(userId.Value, noteId);

                if (result)
                    return Ok(new { success = true, message = "Trash status updated successfully." });

                return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Image")]
        public IActionResult ImageNotes(string image,int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _notesManager.ImageNotes(image, noteId, userId.Value);
                if (result == null)
                {
                    return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
                }
                return Ok(new ResponseModel<string> { Success = true, Message = "Image Added successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("BackgroundColor")]
        public IActionResult ColorForNotes(string color, int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _notesManager.BackgroundColor(color, noteId, userId.Value);
                if (result == null)
                {
                    return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
                }
                return Ok(new ResponseModel<string> { Success = true, Message = "color Added successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPatch("Remainder")]
        public IActionResult AddRemainder(DateTime remainder, int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _notesManager.AddRemainder(remainder, noteId, userId.Value);
                if (result == null)
                {
                    return NotFound(new { success = false, message = $"Note {noteId} not found for user {userId.Value}." });
                }
                return Ok(new ResponseModel<string> { Success = true, Message = "Remainder Added successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while retrieving the note:{ex.Message}." });
            }
        }
        [Authorize]
        [HttpGet("redis")]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "NotesList";
            string SerializedNotes;
             var notesList=   new List<NotesEntity>();
              var redisNotes= await distributedCache.GetAsync(cacheKey);
            if(redisNotes != null)
            {
                SerializedNotes=Encoding.UTF8.GetString(redisNotes);
                notesList=JsonConvert.DeserializeObject<List<NotesEntity>>(SerializedNotes);
            }
            else
            {
                notesList=dBContext.Notestable.ToList();
                SerializedNotes=JsonConvert.SerializeObject(notesList);
                redisNotes=Encoding.UTF8.GetBytes(SerializedNotes);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                await distributedCache.SetAsync(cacheKey,redisNotes, options);
            }
        return Ok(notesList);

        }


    }
}
