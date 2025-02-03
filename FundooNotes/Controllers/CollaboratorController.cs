using ManagerLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorManager _collaboratorManager;
        private readonly ILogger<CollaboratorController> _logger;

        public CollaboratorController(ICollaboratorManager collaboratorManager, ILogger<CollaboratorController> logger)
        {
            _collaboratorManager = collaboratorManager;
            _logger = logger;
        }

        [HttpPost("add")]
        public IActionResult AddCollaborator(int noteId, string email)
        {
            try
            {
                _logger.LogInformation($"Adding collaborator {email} to note {noteId}");
                var result = _collaboratorManager.CreateCollab(noteId, email);

                if (result != null)
                    return Ok(new { success = true, message = "Collaborator added successfully!", data = result });

                return BadRequest(new { success = false, message = "Failed to add collaborator!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding collaborator: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }

        [HttpGet("get")]
        public IActionResult GetCollaborators(int noteId)
        {
            try
            {
                _logger.LogInformation($"Fetching collaborators for note {noteId}");
                var result = _collaboratorManager.RetrieveCollab(noteId);

                if (result != null && result.Count > 0)
                    return Ok(new { success = true, data = result });

                return NotFound(new { success = false, message = "No collaborators found for this note!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving collaborators: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }

        [HttpDelete("delete")]
        public IActionResult RemoveCollaborator(int collabId, int userId)
        {
            try
            {
                _logger.LogInformation($"Removing collaborator {collabId} by user {userId}");
                var result = _collaboratorManager.DeleteCollab(collabId, userId);

                if (result)
                    return Ok(new { success = true, message = "Collaborator removed successfully!" });

                return BadRequest(new { success = false, message = "Failed to remove collaborator!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing collaborator: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }
    }

}
