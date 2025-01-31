using CommonLayer.Models;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface IUserRepository
    {
        public User RegisterUser(RegisterModel model);
        public string Login(LoginModel model);
        public bool EmailExists(string email);
        public ForgetPasswordModel ForgetPassword(string email);
        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel);
    }
}
