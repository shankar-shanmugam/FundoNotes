using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesManager _notesManager;

        public NotesController(INotesManager notesManager)
        {
            this._notesManager = notesManager;
        }

        [Authorize]
        [HttpPost]
        [Route("Create")]
        public IActionResult CreateNotes(NotesModel notesModel)
        {
            try
            {
                int userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                var notesResult = _notesManager.CreateNotes(notesModel, userId);
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
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var listNotesResult = _notesManager.GetAllNotes(userId);

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
                if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var notesResult = _notesManager.GetNotes(userId, noteId);

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
                // if something went wrong in conversion it will be invoked
                if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var UpdatedNote=_notesManager.UpdateNotes(userId, noteId,model);
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
                // if something went wrong in conversion it will be invoked
                if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var IsDeleted = _notesManager.DeleteNote(userId, noteId);

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


    }
}
