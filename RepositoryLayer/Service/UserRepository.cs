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

        public bool EmailExists(string email)=>
        
            _dbContext.User.Any(e=>e.Email == email);
        

        public User RegisterUser(RegisterModel model)
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
        public string Login(LoginModel model)
        {
            var user = _dbContext.User.FirstOrDefault(u => u.Email ==model.Email);

            if (user==null)
            {
                return null;
            }
            string decodedPassword = Decode(user.Password);
            if (decodedPassword == model.Password)
            {
                 return GenerateToken(user); ;
            }
            return null;
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Email),
                 new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
               _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public ForgetPasswordModel ForgetPassword(string email)
        {
             var user =   _dbContext.User.FirstOrDefault(user => user.Email == email);
          
            if (user != null)
            {
                ForgetPasswordModel forgotPasswordModel = new ForgetPasswordModel();
                forgotPasswordModel.UserId = user.Id;
                forgotPasswordModel.Email = user.Email;
                forgotPasswordModel.Token = GenerateToken(user);
                return forgotPasswordModel;
            }
            else
                throw new Exception("User Not Exist for requested email!!!");
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