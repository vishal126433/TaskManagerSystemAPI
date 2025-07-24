using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using TaskManager.DTOs;
using TaskManager.Helpers;

namespace TaskManager.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TaskService> _logger;

        public TaskService(AppDbContext db, ILogger<TaskService> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "DbContext cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "logger cannot be null.");
        }

        public async Task<TaskItem> CreateTaskAsync(int? userId, TaskItem task)

        {
            try
            {
                _logger.LogInformation("Attempting to create task for user {UserId}", userId);
                if (string.IsNullOrWhiteSpace(task.Name) ||
                    string.IsNullOrWhiteSpace(task.Description) ||
                    !task.Duedate.HasValue ||
                    string.IsNullOrWhiteSpace(task.Type) ||
                    string.IsNullOrWhiteSpace(task.Priority) ||
                    string.IsNullOrWhiteSpace(task.Status))
                {
                    _logger.LogWarning("Task creation failed: missing required fields for user {UserId}", userId);
                    throw new ArgumentException("All fields are required.");
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

                _logger.LogInformation("Task created successfully for user {UserId}: {@Task}", userId, task);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task for user {UserId}", userId);
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
                _logger.LogInformation("Attempting to delete task with Id {TaskId}", id);

                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {
                    _logger.LogWarning("Delete failed: task with Id {TaskId} not found", id);
                    return false;
                }

                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Task with Id {TaskId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with Id {TaskId}", id);
                throw;
            }
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updatedTask)
        {
            try
            {
                _logger.LogInformation("Attempting to update task with Id {TaskId}", id);

                var existingTask = await _db.Tasks.FindAsync(id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Update failed: task with Id {TaskId} not found", id);
                    return null;
                }

                existingTask.Name = updatedTask.Name;
                existingTask.Description = updatedTask.Description;
                existingTask.Duedate = updatedTask.Duedate;
                existingTask.Type = updatedTask.Type;
                existingTask.Priority = updatedTask.Priority;

                existingTask.Status = updatedTask.Status;

                // Update State based on the new Status
                existingTask.State = updatedTask.Status switch
                {
                    "new" or "in progress" => TaskStates.Open,
                    "completed" => TaskStates.Closed,
                    _ => updatedTask.Status.ToLower()
                };

                await _db.SaveChangesAsync();

                _logger.LogInformation("Task with Id {TaskId} updated successfully", id);
                return existingTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with Id {TaskId}", id);
                throw;
            }
        }


        public async Task<List<string>> GetStatusListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching status list");
                return await _db.Statuses.Select(s => s.Name.ToLower()).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching status list");
                throw;
            }
        }
        public async Task<List<string>> GetPriorityListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching priority list");
                return await _db.Priority.Select(s => s.Name.ToLower()).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching priority list");
                throw;
            }
        }
        public async Task<List<string>> GetTypeListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching type list");
                return await _db.Types.Select(s => s.Name.ToLower()).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching type list");
                throw;
            }
        }

        public async Task<List<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching tasks for user {UserId}", userId);
                return await _db.Tasks.Where(t => t.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tasks for user {UserId}", userId);
                throw;
            }
        }

        public async Task<(List<TaskItem> Tasks, int TotalCount)> GetTasksAsync(int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                _logger.LogInformation("Fetching paginated tasks: Page {PageNumber}, PageSize {PageSize}", pageNumber, pageSize);

                var query = _db.Tasks.AsQueryable();
                var totalCount = await query.CountAsync();

                var tasks = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                _logger.LogInformation("Fetched {Count} tasks", tasks.Count);
                return (tasks, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated tasks");
                throw;
            }
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all tasks (for count summary)");
                return await _db.Tasks.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all tasks");
                throw;
            }
        }

        public async Task<List<TaskItem>> SearchTasksByUserAsync(int userId, string query)
        {
            try
            {
                _logger.LogInformation("Searching tasks for user {UserId} with query '{Query}'", userId, query);
                return await _db.Tasks
                    .Where(t => t.UserId == userId && (t.Name.Contains(query) || t.Description.Contains(query)))
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tasks for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<TaskItem>> SearchTasksAsync(string query)
        {
            try
            {
                _logger.LogInformation("Searching tasks with query '{Query}'", query);
                return await _db.Tasks
                    .Where(t => t.Name.Contains(query) || t.Description.Contains(query))
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tasks");
                throw;
            }
        }
    }
}
