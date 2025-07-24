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
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.Services.Tasks.DueDateChecker;
using TaskManager.DTOs;
using TaskManager.Services.Users;
using TaskManager.Helpers;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITaskUploadService _taskUploadService;
        private readonly ITaskService _taskService;
        private readonly ITaskStateService _taskStateService;
        private readonly IUserService _userService;



        public TasksController(AppDbContext db, ITaskUploadService taskUploadService, ITaskService taskService, ITaskStateService taskStateService, IUserService userService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "DbContext cannot be null.");
            _taskUploadService = taskUploadService ?? throw new ArgumentNullException(nameof(taskUploadService), "taskUploadService cannot be null.");
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService), "taskService cannot be null.");
            _taskStateService = taskStateService ?? throw new ArgumentNullException(nameof(taskStateService), "taskStateService cannot be null.");
            _userService = userService ?? throw new ArgumentNullException(nameof(userService), "taskService cannot be null.");

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
        [HttpGet("prioritylist")]
        public async Task<IActionResult> GetPriorityList()
        {
            var priorityList = await _taskService.GetPriorityListAsync();
            return Ok(priorityList);
        }

        [HttpGet("typelist")]
        public async Task<IActionResult> GetTypeList()
        {
            var typeList = await _taskService.GetTypeListAsync();
            return Ok(typeList);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchTasks(int userId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var tasks = await _taskService.SearchTasksByUserAsync(userId, query);
            return Ok(tasks);
        }

       
        [HttpGet("searchTasks")]
        public async Task<IActionResult> SearchTasks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var tasks = await _taskService.SearchTasksAsync(query);
            return Ok(tasks);
        }

        [HttpPost("upload-json")]
        public async Task<IActionResult> UploadJson([FromBody] List<TaskImport> tasks)
        {
            if (tasks == null || !tasks.Any())
                return BadRequest("No tasks provided");

            var errors = new List<string>();
            int successCount = 0;

            foreach (var dto in tasks)
            {
                if (!DateTime.TryParse(dto.DueDate, out var parsedDueDate))
                {
                    errors.Add($"Invalid date format for task '{dto.Name}'");
                    continue;
                }

                int userId = 0;
                string assignedUsername = string.Empty;

                if (!string.IsNullOrWhiteSpace(dto.AssignTo) && int.TryParse(dto.AssignTo, out int parsedUserId))
                {
                    var user = await _userService.GetUserByIdAsync(parsedUserId);
                    if (user != null)
                    {
                        userId = parsedUserId;
                        assignedUsername = user.Username;
                    }
                    else
                    {
                        errors.Add($"User not found for AssignTo '{dto.AssignTo}' in task '{dto.Name}'");
                        continue;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dto.AssignTo))
                {
                    errors.Add($"Invalid AssignTo value '{dto.AssignTo}' for task '{dto.Name}'");
                    continue;
                }

                var task = new TaskItem
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Duedate = parsedDueDate,
                    Status = dto.Status,
                    State = dto.Status switch
                    {
                        "new" or "in progress" => TaskStates.Open,
                        "completed" => TaskStates.Closed,
                        _ => dto.Status.ToLower()
                    },
                    Type = dto.Type,
                    Priority = dto.Priority,
                    UserId = userId,
                    AssignedTo = assignedUsername
                };

                try
                {
                    await _taskService.CreateTaskAsync(task.UserId, task);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to create task '{dto.Name}': {ex.Message}");
                }
            }

            return Ok(new
            {
                message = "Tasks processed.",
                successCount,
                errorCount = errors.Count,
                errors
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var parseResult = await _taskUploadService.ParseTasksFromFileAsync(file);

            if (!parseResult.Success)
                return BadRequest(parseResult.ErrorMessage);

            // Just return the parsed tasks without saving them
            return Ok(new
            {
                message = "Tasks parsed successfully",
                count = parseResult.ParsedTasks.Count,
                data = parseResult.ParsedTasks
            });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetTasks(int pageNumber = 1, int pageSize = 5)
        {
            var result = await _taskService.GetTasksAsync(pageNumber, pageSize);
            var counts = await _taskService.GetTaskStatusCountsAsync();

            return Ok(new
            {
                tasks = result.Tasks,
                totalCount = result.TotalCount,
                completedCount = counts.Completed,
                pendingCount = counts.Pending,
                newCount = counts.New
            });
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

        [HttpPost("run")]
        public async Task<IActionResult> RunDueDateCheck(CancellationToken cancellationToken)
        {
            await _taskStateService.UpdateTaskStatesAsync(cancellationToken);
            return Ok(new { message = "Task due dates checked and updated." });
        }

    }
}
