using CommonLayer.Models;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Interface
{
    public interface IUserManager
    {
        public User Registeration(RegisterModel model);

        public string Login(LoginModel model);

        public bool EmailExists(string email);
        ForgetPasswordModel ForgetPassword(string email);
    }
}
