using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Entity;
using System;
using System.Linq;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelManager _label;
        private readonly ILogger<LabelController> _logger;

        public LabelController(ILabelManager label,ILogger<LabelController> logger)
        {
            _label = label;
            _logger = logger;
        }
        private int? GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return int.TryParse(idClaim, out int userId) ? userId : (int?)null;
        }

        [Authorize]
        [HttpPost("CreateLabel")]
        public IActionResult AddLabel(int noteId,string labelName)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
               var result= _label.CreateLabel(noteId,userId.Value,labelName);
                if (result == null)
                {
                    return BadRequest(new { success = false, message = "Label not added ", });
                }

                return Ok(new ResponseModel<LabelEntity> { Success=true,Message="Label added successfully",Data=result});
            }
            catch(Exception  ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }
        }
        [Authorize]
        [HttpPut("editLabel")]
        public IActionResult EditLabel(int labelId,string labelName)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var result = _label.EditLabel(labelId, labelName, userId.Value);
                if (result == null)
                {
                    return BadRequest(new { success = false, message = "Label not added ", });
                }

                return Ok(new ResponseModel<LabelEntity> { Success = true, Message = "Label updated successfully", Data = result });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }

        }

        [Authorize]
        [HttpDelete("DeleteLabel")]
        public IActionResult DeleteLabel(int noteId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var result = _label.DeleteLabel(noteId, userId.Value);
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Label not added s", });
                }

                return Ok(new ResponseModel<bool> { Success = true, Message = "Label Deleted successfully", Data = result });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }

        }

        [Authorize]
        [HttpGet("RetrieveLabel")]
        public IActionResult RetrieveLabel(int labelId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user ID." });
                }
                var result = _label.RetrieveLabel(labelId, userId.Value);
                if (result == null)
                {
                    return BadRequest(new { success = false, message = "Label not added" });
                }

                return Ok(new ResponseModel<LabelEntity> { Success = true, Message = "Label Retrieved successfully", Data = result });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred :{ex.Message}." });
            }

        }
    }
}
