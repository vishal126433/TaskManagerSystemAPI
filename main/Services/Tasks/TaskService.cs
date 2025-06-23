


using System;
using System.Threading.Tasks;
using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

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
                string.IsNullOrWhiteSpace(task.Status))
            {
                throw new ArgumentException("All fields are required.");
            }

            task.UserId = userId;
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
        public async Task<List<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _db.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        
    }
}
