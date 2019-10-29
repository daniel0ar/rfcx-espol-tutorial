using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using WebApplication.Models;
using WebApplication.Helpers;
using WebApplication.DbModels;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using WebApplication.IRepository;

namespace WebApplication.Services
{
    public interface IAuthService
    {
        User Authenticate(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly List<User> _users;
        private readonly AppSettings _appSettings;
        private readonly ObjectContext _context =null; 
        private IUserRepository _userRepository;

        public AuthService(IOptions<AppSettings> appSettings, IOptions<Settings> settings, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            _context = new ObjectContext(settings);
            _userRepository = userRepository;

            _users = _context.Users.Find(_ => true).ToList();

        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                // IssuedAt = DateTime.UtcNow probablemente sirva para setear el expiry. Revisar SecurityTokenDescriptor
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            //Update user with token
            _userRepository.Update(user.Username, user);

            // remove password before returning
            user.Password = null;

            return user;
        }

    }
}