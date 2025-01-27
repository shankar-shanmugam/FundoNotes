using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly FundoDBContext _dbContext;

        public UserRepository(FundoDBContext context)
        {
            _dbContext = context;
        }

        public User RegisterUser(RegisterModel model)
        {
            User user = new User();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Password = model.Password;
            user.DOB = model.DOB;
            user.Gender = model.Gender;
            _dbContext.Add(user);
            _dbContext.SaveChanges();
            return user;
        }
    }
}
