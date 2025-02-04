using CommonLayer.Models;
using ManagerLayer.Interface;
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
    public class LabelController : ControllerBase
    {
        private readonly FundoDBContext dBContext;
        private readonly ILabelManager _label;
        private readonly ILogger<LabelController> _logger;
        private readonly IDistributedCache distributedCache;

        public LabelController(FundoDBContext dBContext, ILabelManager label, ILogger<LabelController> logger, IDistributedCache distributedCache)
        {
            this.dBContext = dBContext;
            _label = label;
            _logger = logger;
            this.distributedCache = distributedCache;
        }

        private int? GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            _logger.LogInformation("Attempting to get user ID from claims");
            return int.TryParse(idClaim, out int userId) ? userId : (int?)null;
        }

        [Authorize]
        [HttpPost("CreateLabel")]
        public IActionResult AddLabel(int noteId, string labelName)
        {
            try
            {
                _logger.LogInformation("Creating new label. NoteId: {NoteId}, LabelName: {LabelName}", noteId, labelName);

                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _label.CreateLabel(noteId, userId.Value, labelName);
                if (result == null)
                {
                    _logger.LogWarning("Failed to create label for NoteId: {NoteId}, UserId: {UserId}", noteId, userId);
                    return BadRequest(new { success = false, message = "Label not added ", });
                }

                _logger.LogInformation("Label created successfully. LabelId: {LabelId}, NoteId: {NoteId}", result.LabelId, noteId);
                return Ok(new ResponseModel<LabelEntity> { Success = true, Message = "Label added successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating label. NoteId: {NoteId}, Error: {Error}", noteId, ex.Message);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpPut("editLabel")]
        public IActionResult EditLabel(int labelId, string labelName)
        {
            try
            {
                _logger.LogInformation("Editing label. LabelId: {LabelId}, NewName: {LabelName}", labelId, labelName);

                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _label.EditLabel(labelId, labelName, userId.Value);
                if (result == null)
                {
                    _logger.LogWarning("Failed to edit label. LabelId: {LabelId}, UserId: {UserId}", labelId, userId);
                    return BadRequest(new { success = false, message = "Label not added ", });
                }

                _logger.LogInformation("Label updated successfully. LabelId: {LabelId}", labelId);
                return Ok(new ResponseModel<LabelEntity> { Success = true, Message = "Label updated successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing label. LabelId: {LabelId}, Error: {Error}", labelId, ex.Message);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpDelete("DeleteLabel")]
        public IActionResult DeleteLabel(int labelId)
        {
            try
            {
                _logger.LogInformation("Deleting label for NoteId: {NoteId}", labelId);

                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _label.DeleteLabel(labelId, userId.Value);
                if (!result)
                {
                    _logger.LogWarning("Failed to delete label. NoteId: {NoteId}, UserId: {UserId}", labelId, userId);
                    return BadRequest(new { success = false, message = "Label not added s", });
                }

                _logger.LogInformation("Label deleted successfully. NoteId: {NoteId}", labelId);
                return Ok(new ResponseModel<bool> { Success = true, Message = "Label Deleted successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting label. NoteId: {NoteId}, Error: {Error}", labelId, ex.Message);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpGet("RetrieveLabel")]
        public IActionResult RetrieveLabel(int labelId)
        {
            try
            {
                _logger.LogInformation("Retrieving label. LabelId: {LabelId}", labelId);

                int? userId = GetUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - Invalid or missing user ID");
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }

                var result = _label.RetrieveLabel(labelId, userId.Value);
                if (result == null)
                {
                    _logger.LogWarning("Label not found. LabelId: {LabelId}, UserId: {UserId}", labelId, userId);
                    return BadRequest(new { success = false, message = "Label not added" });
                }

                _logger.LogInformation("Label retrieved successfully. LabelId: {LabelId}", labelId);
                return Ok(new ResponseModel<LabelEntity> { Success = true, Message = "Label Retrieved successfully", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving label. LabelId: {LabelId}, Error: {Error}", labelId, ex.Message);
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }

        [Authorize]
        [HttpGet("redis")]
        public async Task<IActionResult> GetAllLabelsUsingRedisCache()
        {
            var cacheKey = "LabelList";
            string SerializedNotes;
            var labelList = new List<LabelEntity>();
            var redisNotes = await distributedCache.GetAsync(cacheKey);
            if (redisNotes != null)
            {
                SerializedNotes = Encoding.UTF8.GetString(redisNotes);
                labelList = JsonConvert.DeserializeObject<List<LabelEntity>>(SerializedNotes);
            }
            else
            {
                labelList = dBContext.Labeltable.ToList();
                SerializedNotes = JsonConvert.SerializeObject(labelList);
                redisNotes = Encoding.UTF8.GetBytes(SerializedNotes);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                await distributedCache.SetAsync(cacheKey, redisNotes, options);
            }
            return Ok(labelList);

        }
    }
}
