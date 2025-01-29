using CommonLayer.Models;
using ManagerLayer.Interface;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using System.Threading.Tasks;
using System;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IBus bus;

        public UserController(IUserManager userManager, IBus _bus)
        {
            _userManager = userManager;
            bus=_bus;
        }

        [HttpPost]
        [Route("reg")]
        public IActionResult Register(RegisterModel model)
        {
            if (_userManager.EmailExists(model.Email))
            {
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email already exist" });
            }
            else
            {
                var result = _userManager.Registeration(model);
                if (result != null)
                {
                    return Ok(new ResponseModel<User> { Success = true, Message = "Registered successfully", Data = result });
                }
                else
                {
                    return BadRequest(new ResponseModel<User> { Success = false, Message = "Registeration Failed !!" });
                }
            }
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginModel model)
        {
            var res=_userManager.Login(model);
            if (res != null)
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "login successful", Data = res });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { Success = false, Message = " Login failed " });
            }
        }

        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                ForgetPasswordModel forgotPasswordModel = _userManager.ForgetPassword(email);
                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);

                Uri uri = new Uri("rabbitmq://localhost/FunDooNotesEmailQueue");
                var endPoint = await bus.GetSendEndpoint(uri);
                await endPoint.Send(forgotPasswordModel);

                return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Please provide valid email" });
            }
        }



    }
}
