using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Services;
using WebApplication.Models;
using WebApplication.IRepository;
using WebApplication.Repository;
using System;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    //[ApiController]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleTodos)]
        [HttpGet("[controller]/index")]
        public IActionResult Index()
        {
            ViewBag.UsersInDB =  _userRepository.GetAll();
            ViewBag.Role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            return View();
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpGet("[controller]/update/{userId}")]
        public IActionResult Update(string userId)
        {
            ViewBag.UserToEdit = _userRepository.GetById(userId);
            return View();
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpPost("api/[controller]/update/{userId}")]
        public IActionResult Update(string userId, [FromBody]User user)
        {
            user.UserId = userId;
            Console.WriteLine(userId);
            var updated = _userRepository.UpdateById(userId, user);
            Console.WriteLine(updated);
            Console.WriteLine(user.Username);
            if (!updated)
            {
                return BadRequest(new { message = "User not updated" });
                
            }
            user.Password = null;
            return Ok(user);
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpPost]
        public IActionResult Create(string Username, string Password, string Role, string Name, string Email) 
        {
            var user = new User();
            user.Username = Username; user.Password = Password; user.Role = Role; user.Name = Name; user.Email = Email;
            _userRepository.AddAsync(user);
            return Redirect("/users/index");
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpPost("api/[controller]/new")]
        public IActionResult New([FromBody]User user)
        {
            Console.WriteLine(user.Username);
            return Ok(_userRepository.AddAsync(user));
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userRepository.GetAll();
            return Ok(users);
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleAdministrador)]
        [HttpDelete("api/[controller]/{userId}")]
        public bool Delete(string userId)
        {
            
            bool valor = _userRepository.Remove(userId);
            return valor;
        }
    }
}
