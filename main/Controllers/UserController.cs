﻿using Azure.Core;
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
            _userService = userService;
            _context = context;
        }

        //[HttpPost("register")]
        //public async Task<IActionResult> Register(RegisterRequest req)
        //{
        //    var result = await _userService.RegisterAsync(req);
        //    return Ok(result);
        //}

        [HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    var token = await _userService.LoginAsync(request); // calls your method

        //    // ✅ Set the cookie here using the refresh token
        //    Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.None,
        //        Expires = DateTime.UtcNow.AddMinutes(30),
        //        Path = "/"
        //    });

        //    // ✅ Return access token to frontend
        //    return Ok(new
        //    {
        //        accessToken = token.AccessToken,
        //        role = token.Role
        //    });
        //}


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token");

            var newToken = await _userService.RefreshTokenAsync(refreshToken);
            return Ok(newToken);
        }

        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    var result = await _userService.LogoutAsync();
        //    Response.Cookies.Delete("refreshToken");
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
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

