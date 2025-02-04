using CommonLayer.Models;
using ManagerLayer.Interface;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CollaboratorController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;
        private readonly FundoDBContext dBContext;
        private readonly ICollaboratorManager _collaboratorManager;
        private readonly ILogger<CollaboratorController> _logger;
        private readonly EmailService _emailService;
        private readonly IUserManager userManager;
        private readonly INotesManager notesManager;
        private readonly IBus _bus;

        public CollaboratorController(IUserManager userManager,INotesManager notesManager,IBus _bus,EmailService _emailService, IDistributedCache distributedCache, FundoDBContext dBContext, ICollaboratorManager collaboratorManager, ILogger<CollaboratorController> logger)
        {
            this.distributedCache = distributedCache;
            this.dBContext = dBContext;
            this._emailService = _emailService;
            _collaboratorManager = collaboratorManager;
            _logger = logger;
            this.userManager = userManager;
            this.notesManager = notesManager;
            this._bus = _bus;

        }
        private int? GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            _logger.LogInformation("Attempting to get user ID from claims");
            return int.TryParse(idClaim, out int userId) ? userId : (int?)null;
        }

        [Authorize]
        [HttpPost("add")]
        public IActionResult AddCollaborator(int noteId, string email)
        {
            try
            {
                _logger.LogInformation($"Adding collaborator {email} to note {noteId}");


                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _collaboratorManager.CreateCollab(noteId, email);
                if (result == null)
                {
                    return BadRequest(new { success = false, message = "Failed to add collaborator!" });
                }

              var notescollab =  notesManager.GetNotes(userId.Value, noteId);
              var ownerUser = dBContext.User.FirstOrDefault(u=>u.Id == userId.Value);  

                try
                {
                    _emailService.SendCollaborationEmail(
                        email,
                        notescollab.Title,  
                        ownerUser.FirstName        
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError($"Failed to send email: {emailEx.Message}");
                }

                try
                {
                    // Create the message
                    var collaborationMessage = new CollaborationMessage
                    {
                        NoteId = noteId,
                        NoteTitle = notescollab.Title,
                        CollaboratorEmail = email,
                        OwnerName = ownerUser.FirstName
                    };

                    // Send to RabbitMQ
                    var uri = new Uri("rabbitmq://localhost/FundooNotesCollaborationQueue");
                    var endPoint = _bus.GetSendEndpoint(uri).Result;
                    endPoint.Send(collaborationMessage);
                }
                catch (Exception mqEx)
                {
                    _logger.LogError($"Failed to send to RabbitMQ: {mqEx.Message}");
                }
               
              return Ok(new { success = true, message = "Collaborator added successfully!", data = result });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding collaborator: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }
        [Authorize]
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
        [Authorize]
        [HttpDelete("delete")]
        public IActionResult RemoveCollaborator(int collabId)
        {
            try
            {
                _logger.LogInformation($"Removing collaborator {collabId} ");
                var result = _collaboratorManager.DeleteCollab(collabId);

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

        [Authorize]
        [HttpGet("redis")]
        public async Task<IActionResult> GetAllCollabUsingRedisCache()
        {
            var cacheKey = "NotesList";
            string SerializedNotes;
            var CollabList = new List<CollaboratorEntity>();
            var redisNotes = await distributedCache.GetAsync(cacheKey);
            if (redisNotes != null)
            {
                SerializedNotes = Encoding.UTF8.GetString(redisNotes);
                CollabList = JsonConvert.DeserializeObject<List<CollaboratorEntity>>(SerializedNotes);
            }
            else
            {
                CollabList = dBContext.Collaboratortable.ToList();
                SerializedNotes = JsonConvert.SerializeObject(CollabList);
                redisNotes = Encoding.UTF8.GetBytes(SerializedNotes);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                await distributedCache.SetAsync(cacheKey, redisNotes, options);
            }
            return Ok(CollabList);

        }
    }

}
