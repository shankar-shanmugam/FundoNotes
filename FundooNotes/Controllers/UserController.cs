using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Http;
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
           var result= _userManager.Registeration(model);
            if(result != null)
            {
                return Ok(new ResponseModel<User> { Success = true,Message="Registered successfully",Data=result });
            }
            else
            {
                return BadRequest(new ResponseModel<User> { Success = false, Message = "Registeration Failed !!" });
            }


        }


    }
}
