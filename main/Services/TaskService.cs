using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.DTOs;
using TaskManager.Helpers;
using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TaskService> _logger;
        private readonly IUserService _userService;

        public TaskService(AppDbContext db, ILogger<TaskService> logger, IUserService userService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), ResponseMessages.Message.DBcontextNull);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), ResponseMessages.Message.LoggerNull);
            _userService = userService ?? throw new InvalidOperationException(ResponseMessages.Message.NoUserServiceInitialised);

        }

        public async Task<TaskItem> CreateTaskAsync(int? userId, TaskItem task)

        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.AttemptTaskCreateId, userId);
                if (string.IsNullOrWhiteSpace(task.Name) ||
                    string.IsNullOrWhiteSpace(task.Description) ||
                    !task.Duedate.HasValue ||
                    string.IsNullOrWhiteSpace(task.Type) ||
                    string.IsNullOrWhiteSpace(task.Priority) ||
                    string.IsNullOrWhiteSpace(task.Status))
                {
                    _logger.LogWarning(ResponseMessages.Message.TaskCreationFailed, userId);
                    throw new ArgumentException(ResponseMessages.Message.AllFields);
                }

                task.UserId = (userId == 0) ? null : userId;
                task.State = task.Status switch
                {
                    "new" or "in progress" => TaskStates.Open,
                    "completed" => TaskStates.Closed,
                    _ => task.Status.ToLower()
                };

                _db.Tasks.Add(task);
                await _db.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.Message.TaskCreationLog, userId, task);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.TaskCreationFail, userId);
                throw;
            }
        }
        public async Task<TaskStatusCount> GetTaskStatusCountsAsync()
        {
            return new TaskStatusCount
            {
                Completed = await _db.Tasks.CountAsync(t => t.Status.ToLower() == "completed"),
                Pending = await _db.Tasks.CountAsync(t => t.Status.ToLower() == "in progress"),
                New = await _db.Tasks.CountAsync(t => t.Status.ToLower() == "new")
            };
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.AttemptTaskDeletedId, id);

                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {
                    _logger.LogWarning(ResponseMessages.Message.TaskDeletedNotFound, id);
                    return false;
                }

                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.Message.TaskDeletedId, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ResponseMessages.Message.TaskDeletedIdError, id);
                throw;
            }
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updatedTask)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.AttemptTaskUpdateId, id);

                var existingTask = await _db.Tasks.FindAsync(id);
                if (existingTask == null)
                {
                    _logger.LogWarning(ResponseMessages.Message.TaskUpdateNotFound, id);
                    return null;
                }

                existingTask.Name = updatedTask.Name;
                existingTask.Description = updatedTask.Description;
                existingTask.Duedate = updatedTask.Duedate;
                existingTask.Type = updatedTask.Type;
                existingTask.Priority = updatedTask.Priority;
                existingTask.AssignedTo = updatedTask.AssignedTo;


                existingTask.Status = updatedTask.Status;

                // Update State based on the new Status
                existingTask.State = updatedTask.Status switch
                {
                    "new" or "in progress" => TaskStates.Open,
                    "completed" => TaskStates.Closed,
                    _ => updatedTask.Status.ToLower()
                };

                await _db.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.Message.TaskUpdatedId, id);
                return existingTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.TaskUpdatedIdError, id);
                throw;
            }
        }


        public async Task<List<string>> GetStatusListAsync()
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchStatusList);
                return await _db.Statuses.Select(s => s.Name.ToLower()).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.FetchStatusListError);
                throw;
            }
        }
        public async Task<List<string>> GetPriorityListAsync()
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchPriorityList);
                return await _db.Priority.Select(s => s.Name.ToLower()).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.FetchPriorityListError);
                throw;
            }
        }
        public async Task<List<string>> GetTypeListAsync()
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchTypeList);
                return await _db.Types.Select(s => s.Name.ToLower()).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.FetchTypeListError);
                throw;
            }
        }

        public async Task<List<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchTask, userId);
                return await _db.Tasks.Where(t => t.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.FetchTaskuseridError, userId);
                throw;
            }
        }

        public async Task<(List<TaskItem> Tasks, int TotalCount)> GetTasksAsync(int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchpaginatedTask, pageNumber, pageSize);

                var query = _db.Tasks.AsQueryable();
                var totalCount = await query.CountAsync();

                var tasks = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                _logger.LogInformation("Fetched {Count} tasks", tasks.Count);
                return (tasks, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.FetchpaginatedTaskError);
                throw;
            }
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchTaskCount);
                return await _db.Tasks.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.FetchTaskError);
                throw;
            }
        }

        public async Task<List<TaskItem>> SearchTasksByUserAsync(int userId, string query)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.SearchTaskUserId, userId, query);
                return await _db.Tasks
                    .Where(t => t.UserId == userId && (t.Name.Contains(query) || t.Description.Contains(query)))
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.SearchTaskUserIdError, userId);
                throw;
            }
        }

        public async Task<List<TaskItem>> SearchTasksAsync(string query)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.SearchTask, query);
                return await _db.Tasks
                    .Where(t => t.Name.Contains(query) || t.Description.Contains(query))
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.SearchTaskError);
                throw;
            }
        }

        public async Task<object> UploadTasksAsync(List<TaskImport> tasks)
        {
            var errors = new List<string>();
            int successCount = 0;

            foreach (var dto in tasks)
            {
                // Date validation
                if (!DateTime.TryParse(dto.DueDate, out var parsedDueDate))
                {
                    errors.Add($"Invalid date format for task '{dto.Name}'");
                    continue;
                }

                // User assignment
                int userId = 0;
                string assignedUsername = string.Empty;

                if (!string.IsNullOrWhiteSpace(dto.AssignTo) && int.TryParse(dto.AssignTo, out int parsedUserId))
                {
                    var user = await _userService.GetUserByIdAsync(parsedUserId);
                    if (user != null)
                    {
                        assignedUsername = user.Username;
                        userId = parsedUserId;
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

                // Create task entity
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
                    await CreateTaskAsync(task.UserId, task);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create task '{TaskName}'", dto.Name);
                    errors.Add($"Failed to create task '{dto.Name}': {ex.Message}");
                }
            }

            return new
            {
                message = "Tasks processed.",
                successCount,
                errorCount = errors.Count,
                errors
            };
        }

    }
}
