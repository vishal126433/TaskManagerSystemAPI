
using System.Threading.Tasks;
using AuthService.Models;

namespace TaskManager.Services.Tasks
{
    public interface ITaskService
    {
        /// <summary>
        /// create a task for a specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<TaskItem> CreateTaskAsync(int userId, TaskItem task);
        /// <summary>
        /// Delete task for a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteTaskAsync(int id);
        /// <summary>
        /// update task for a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedTask"></param>
        /// <returns></returns>
        Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updatedTask);
        /// <summary>
        /// disply the list of statuses of tasks
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetStatusListAsync();
        /// <summary>
        /// display the list of types of tasks
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetTypeListAsync(); 
        /// <summary>
        /// display the list of type of tasks
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<TaskItem>> GetTasksByUserIdAsync(int userId);
        /// <summary>
        /// display the all tasks in the project in pagination form
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<(List<TaskItem> Tasks, int TotalCount)> GetTasksAsync(int pageNumber = 1, int pageSize = 5);

        /// <summary>
        /// Search tasks by user ID and query string
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<TaskItem>> SearchTasksByUserAsync(int userId, string query);

        /// <summary>
        /// Search tasks by query string across all users
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<TaskItem>> SearchTasksAsync(string query);
        /// <summary>
        /// display the list of priority of tasks
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetPriorityListAsync();
    }
}
