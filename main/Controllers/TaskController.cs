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
using TaskManager.Services.Tasks.DueDateChecker;

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



        public TasksController(AppDbContext db, ITaskUploadService taskUploadService, ITaskService taskService, ITaskStateService taskStateService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "DbContext cannot be null.");
            _taskUploadService = taskUploadService ?? throw new ArgumentNullException(nameof(taskUploadService), "taskUploadService cannot be null.");
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService), "taskService cannot be null.");
            _taskStateService = taskStateService ?? throw new ArgumentNullException(nameof(taskStateService), "taskStateService cannot be null.");


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
        [HttpGet]
        public async Task<IActionResult> GetTasks(int pageNumber = 1, int pageSize = 5)
        {
            var result = await _taskService.GetTasksAsync(pageNumber, pageSize);
            return Ok(new
            {
                tasks = result.Tasks,
                totalCount = result.TotalCount
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
