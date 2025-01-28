using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
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

    }
}
