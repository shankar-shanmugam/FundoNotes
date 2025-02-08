using CommonLayer.Models;
using ManagerLayer.Interface;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NLog;
using FundooNotes.Helpers;
using Microsoft.AspNetCore.Http;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IBus bus;
        private readonly ILogger _logger;

        public UserController(IUserManager userManager, IBus _bus)
        {
            _userManager = userManager;
            bus = _bus;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                _logger.Info("Register User started");
                if (model == null)
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid registration details provided" });

                if (_userManager.EmailExists(model.Email))
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email already exists" });

                var result = _userManager.Registeration(model);

                if (result != null)
                {
                    _logger.Info($"User registration successful for email: {model.Email}");
                    return Ok(new ResponseModel<User> { Success = true, Message = "Registered successfully", Data = result });
                }

                _logger.Warn($"Registration failed for email: {model.Email}");
                return BadRequest(new ResponseModel<User> { Success = false, Message = "Registration failed" });
            }
            catch (AppException ex)
            {
                _logger.Error(ex, $"Error during user registration: {ex.Message}");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                _logger.Info("Login attempt started");
                if (model == null)
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid login details" });

                var result = _userManager.Login(model);
                if (result != null)
                {
                    _logger.Info($"Login successful for user: {model.Email}");
                    return Ok(new ResponseModel<string> { Success = true, Message = "Login successful", Data = result });
                }
                _logger.Warn($"Failed login attempt for user: {model.Email}");
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error during login: {ex.Message}");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                _logger.Info($"Password reset requested for email: {email}");
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email is required" });

                ForgetPasswordModel forgotPasswordModel = _userManager.ForgetPassword(email);
                if (forgotPasswordModel == null)
                {
                    _logger.Warn($"Password reset failed - user not found for email: {email}");
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "User with this email does not exist" });
                }

                // send email
                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);

                // Send to RabbitMQ
                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await bus.GetSendEndpoint(uri);
                await endPoint.Send(forgotPasswordModel);

                _logger.Info($"Password reset email sent successfully to: {email}");
                return Ok(new { Success = true, Message = "Password reset email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error during password reset request: {ex.Message}");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred while processing your request", Data = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordModel request)
        {
            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == "custom_email")?.Value;

                if (email == null)
                {
                    _logger.Warn("Password reset attempted with invalid or expired token");
                    return BadRequest(new { success = false, message = "Invalid or expired token" });
                }

                _logger.Info($"Password reset attempt for user: {email}");
                var result = _userManager.ResetPassword(email, request);

                if (result)
                {
                    _logger.Info($"Password reset successful for user: {email}");
                    return Ok(new { success = true, message = "Password reset successful" });
                }
                else
                {
                    _logger.Warn($"Password reset failed for user: {email}");
                    return BadRequest(new { success = false, message = "Password reset unsuccessful" });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error during password reset: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while resetting the password. Please try again later.", Data = ex.Message });
            }
        }
       
    }
}
