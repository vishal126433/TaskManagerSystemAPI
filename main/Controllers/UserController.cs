using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthService.Models;
using AuthService.Data;
using TaskManager.Interfaces;
using TaskManager.Helpers;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new InvalidOperationException("UserService not initialized.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(ApiResponse<string>.SingleError(ResponseMessages.User.NoRefreshToken));

            var newToken = await _userService.RefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<object>.SuccessResponse(newToken));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<object>.SuccessResponse(users));
        }

        [HttpPost("toggle-active/{id}")]
        public async Task<IActionResult> ToggleActiveStatus(int id)
        {
            var result = await _userService.ToggleUserActiveStatusAsync(id);
            if (result == null)
                return NotFound(ApiResponse<string>.SingleError(ResponseMessages.User.NotFound));

            return Ok(ApiResponse<object>.SuccessResponse(new { IsActive = result }));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(ApiResponse<string>.SingleError(ResponseMessages.User.RequiredFieldsMissing));
            }

            var user = await _userService.CreateUserAsync(request);
            return Ok(ApiResponse<object>.SuccessResponse(new { user.Username, user.Email, user.Role },200,ResponseMessages.User.Created));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _userService.UpdateUserAsync(id, updatedUser);
            if (user == null)
                return NotFound(ApiResponse<string>.SingleError(ResponseMessages.User.NotFound));

            return Ok(ApiResponse<object>.SuccessResponse(
                user,
                message: ResponseMessages.User.Updated));
        }


        [HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    var success = await _userService.DeleteUserAsync(id);
    if (!success)
        return NotFound(ApiResponse<string>.SingleError(ResponseMessages.User.NotFound));

    return Ok(ApiResponse<string>.SuccessResponse(
        data: null,
        message: ResponseMessages.User.Deleted));
}

    }
}
