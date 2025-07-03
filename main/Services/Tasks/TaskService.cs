using System;
using System.Threading.Tasks;
using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Helpers;

namespace TaskManager.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;

        public TaskService(AppDbContext db)
        {
            _db = db;
        }
       
        public async Task<TaskItem> CreateTaskAsync(int userId, TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Name) ||
                string.IsNullOrWhiteSpace(task.Description) ||
                !task.Duedate.HasValue ||
                string.IsNullOrWhiteSpace(task.Type) ||

                string.IsNullOrWhiteSpace(task.Status))

            {
                throw new ArgumentException("All fields are required.");
            }

            task.UserId = userId;

            //  Set State based on Status
            switch (task.Status)
            {
                case "New":
                case "In Progress":
                    task.State = TaskStates.Open;
                    break;
                case "Completed":
                    task.State = TaskStates.Closed;
                    break;
                default:
                    task.State = task.Status.ToLower(); // fallback for custom statuses
                    break;
            }

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return false;

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updatedTask)
        {
            var existingTask = await _db.Tasks.FindAsync(id);
            if (existingTask == null) return null;

            existingTask.Name = updatedTask.Name;
            existingTask.Description = updatedTask.Description;
            existingTask.Duedate = updatedTask.Duedate;
            existingTask.Type = updatedTask.Type;

            existingTask.Status = updatedTask.Status;

            await _db.SaveChangesAsync();
            return existingTask;
        }
        public async Task<List<string>> GetStatusListAsync()
        {
            return await _db.Statuses
                .Select(s => s.Name.ToLower())
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetTypeListAsync()
        {
            return await _db.Types
                .Select(s => s.Name.ToLower())
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _db.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
        

        public async Task<(List<TaskItem> Tasks, int TotalCount)> GetTasksAsync(int pageNumber = 1, int pageSize = 5)
        {
            var query = _db.Tasks.AsQueryable();
            var totalCount = await query.CountAsync();
            var tasks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tasks, totalCount);
        }
        public async Task<List<TaskItem>> SearchTasksByUserAsync(int userId, string query)
        {
            return await _db.Tasks
                .Where(t => t.UserId == userId &&
                            (t.Name.Contains(query) || t.Description.Contains(query)))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<TaskItem>> SearchTasksAsync(string query)
        {
            return await _db.Tasks
                .Where(t => t.Name.Contains(query) || t.Description.Contains(query))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }


        private IActionResult Ok(object value)
        {
            throw new NotImplementedException();
        }


    }
}
