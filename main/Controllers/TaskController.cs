using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Data;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TasksController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateTask(int userId, [FromBody] TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Name) ||
                string.IsNullOrWhiteSpace(task.Description) ||
                string.IsNullOrWhiteSpace(task.Status))
            {
                return BadRequest("All fields are required.");
            }

            task.UserId = userId; // Assign userId from URL to task

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Task created successfully", task });
        }

        [HttpGet("statuslist")]
        public IActionResult GetStatusList()
        {
            var statusList = _db.Tasks
                                .Select(t => t.Status.ToLower())
                                .Distinct()
                                .ToList();

            return Ok(statusList);
        }
        [HttpGet("{userId}")]
        public IActionResult GetTasksByUserId(int userId)
        {
            var tasks = _db.Tasks.Where(t => t.UserId == userId).ToList();
            return Ok(tasks);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { message = "Task not found." });
            }

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Task deleted successfully." });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
        {
            var existingTask = await _db.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound(new { message = "Task not found." });
            }

            existingTask.Name = updatedTask.Name;
            existingTask.Description = updatedTask.Description;
            existingTask.Status = updatedTask.Status;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Task updated successfully.", task = existingTask });
        }

    }
}
