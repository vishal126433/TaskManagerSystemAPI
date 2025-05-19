using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Data;
using System.Linq;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var userNames = _db.Users
                .Select(u => new { username = u.Username }) // Make sure Username exists in User model
                .ToList();

            return Ok(userNames);
        }
    }
}
