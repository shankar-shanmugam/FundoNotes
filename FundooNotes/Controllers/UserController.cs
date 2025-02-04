﻿using CommonLayer.Models;
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
using Microsoft.Extensions.Logging;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IBus bus;
        private readonly ILogger<UserController> _log;

        public UserController(IUserManager userManager, IBus _bus,ILogger<UserController> log)
        {
            _userManager = userManager;
            bus = _bus;
            _log = log;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                _log.LogInformation("Register User started");
                if (model == null)
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid registration details provided" });

                if (_userManager.EmailExists(model.Email))
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email already exists" });

                var result = _userManager.Registeration(model);

                if (result != null)
                {
                    return Ok(new ResponseModel<User> { Success = true, Message = "Registered successfully", Data = result });
                }

                return BadRequest(new ResponseModel<User> { Success = false, Message = "Registration failed" });
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                _log.LogInformation("Login started.............!!");
                if (model == null)
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid login details" });

                var result = _userManager.Login(model);
                if (result != null)
                {
                    return Ok(new ResponseModel<string> { Success = true, Message = "Login successful", Data = result });
                }
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email is required" });

                ForgetPasswordModel forgotPasswordModel = _userManager.ForgetPassword(email);
                if (forgotPasswordModel == null)
              
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "User with this email does not exist" });

                // send email
                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);

                // Send to RabbitMQ
                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await bus.GetSendEndpoint(uri);
                await endPoint.Send(forgotPasswordModel);

                return Ok(new { Success = true, Message = "Password reset email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred while processing your request", Data = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordModel request)
        {
            try
            {
                // To inspect the claim types and values ex: custom_email:shankar@gmail.com
                //var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                //return Ok(new { success = false, claims });
                string email = User.Claims.FirstOrDefault(c => c.Type == "custom_email")?.Value;

                if (email == null)
                {
                    return BadRequest(new { success = false, message = "Invalid or expired token" });
                }

                var result = _userManager.ResetPassword(email, request);

                if (result)
                {
                    return Ok(new { success = true, message = "Password reset successful" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Password reset unsuccessful" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while resetting the password. Please try again later.", Data = ex.Message });
            }
        }

    }
}
