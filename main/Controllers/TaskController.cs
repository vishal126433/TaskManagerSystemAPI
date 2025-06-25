using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;
using System.Threading.Tasks;
using TaskManager.Services;
using TaskManager.Services.Tasks;
using TaskManager.Services.Tasks.FileUpload;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITaskUploadService _taskUploadService;
        private readonly ITaskService _taskService;

       


        public TasksController(AppDbContext db, ITaskUploadService taskUploadService, ITaskService taskService)
        {
            _db = db;
            _taskUploadService = taskUploadService;
            _taskService = taskService;

        }



        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateTask(int userId, [FromBody] TaskItem task)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(userId, task);
                return Ok(new { message = "Task created successfully", task = createdTask });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("statuslist")]
        public async Task<IActionResult> GetStatusList()
        {
            var statusList = await _taskService.GetStatusListAsync();
            return Ok(statusList);
        }

        [HttpGet("typelist")]
        public async Task<IActionResult> GetTypeList()
        {
            var typeList = await _taskService.GetTypeListAsync();
            return Ok(typeList);
        }



        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await _taskUploadService.ProcessExcelAsync(file);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(new { message = "Tasks uploaded successfully", count = result.Count });
        }




        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted)
                return NotFound(new { message = "Task not found." });

            return Ok(new { message = "Task deleted successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
        {
            var result = await _taskService.UpdateTaskAsync(id, updatedTask);
            if (result == null)
                return NotFound(new { message = "Task not found." });

            return Ok(new { message = "Task updated successfully.", task = result });
        }

    }
}
