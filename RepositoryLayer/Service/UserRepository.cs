using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly FundoDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserRepository(FundoDBContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public bool EmailExists(string email) =>

            _dbContext.User.Any(e => e.Email == email);

        public User RegisterUser(RegisterModel model)
        {
            try
            {
                User user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = EncodePassword(model.Password),
                    DOB = model.DOB,
                    Gender = model.Gender
                };

                _dbContext.Add(user);
                _dbContext.SaveChanges();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user", ex);
            }
        }

        public string Login(LoginModel model)
        {
            try
            {
                var user = _dbContext.User.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                    return null;

                string decodedPassword = Decode(user.Password);
                return decodedPassword == model.Password ? GenerateToken(user.Email, user.Id) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login", ex);
            }
        }

        private string GenerateToken(string Email, int userId)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim("custom_email", Email),
                    new Claim("id", userId.ToString())
                };
                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(50),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating JWT token", ex);
            }
        }



        public ForgetPasswordModel ForgetPassword(string email)
        {
            try
            {
                var user = GetUserByEmail(email);
                if (user == null)
                    throw new Exception("User does not exist for the requested email");

                return new ForgetPasswordModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = GenerateToken(user.Email, user.Id)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing forgot password request", ex);
            }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                return _dbContext.User.FirstOrDefault(user => user.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by email", ex);
            }
        }

        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var user = GetUserByEmail(email);
                if (user == null)
                    throw new Exception("User not found");

                if (resetPasswordModel.NewPassword != resetPasswordModel.ConfirmPassword)
                    throw new Exception("Password mismatch");

                user.Password = EncodePassword(resetPasswordModel.NewPassword);
                _dbContext.User.Update(user);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetting password", ex);
            }
        }


        public static string EncodePassword(string password)
        {

            byte[] encData_byte = new byte[password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;

        }
        public string Decode(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }


    }
}