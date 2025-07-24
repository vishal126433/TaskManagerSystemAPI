using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using AuthService.Models;
using AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using TaskManager.Services.Users;


namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public UsersController(IUserService userService, AppDbContext context)
        {
            if (userService == null)
                throw new InvalidOperationException("UserService is not initialized. Please check the dependency injection configuration.");

            if (context == null)
                throw new InvalidOperationException("AppDbContext is not initialized. Please check the dependency injection configuration.");

            _userService = userService;
            _context = context;
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token");

            var newToken = await _userService.RefreshTokenAsync(refreshToken);
            return Ok(newToken);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

       [HttpPost("toggle-active/{id}")]
public async Task<IActionResult> ToggleActiveStatus(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null) return NotFound();

    user.IsActive = !user.IsActive;
    await _context.SaveChangesAsync();

    return Ok(new { user.IsActive });
}

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username, Email, and Password are required.");
            }

            var user = await _userService.CreateUserAsync(request);
            return Ok(new
            {
                message = "User created successfully",
                user = new { user.Username, user.Email, user.Role }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _userService.UpdateUserAsync(id, updatedUser);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User updated successfully", user });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User deleted successfully." });
        }

    }
}

