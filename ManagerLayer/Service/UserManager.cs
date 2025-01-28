using CommonLayer.Models;
using ManagerLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Service
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        public UserManager(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public User Registeration(RegisterModel model) => _userRepository.RegisterUser(model);

        public string Login(LoginModel model) => _userRepository.Login(model);

        public bool EmailExists(string email) => _userRepository.EmailExists(email);
       
        
        

        

    }
}
