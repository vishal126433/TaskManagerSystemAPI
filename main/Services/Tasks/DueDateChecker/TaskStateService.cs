using AuthService.Data;
using TaskManager.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Services;
using TaskManager.Services.Notifications;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TaskManager.Models;

namespace TaskManager.Services.Tasks.DueDateChecker
{
    public class TaskStateService : ITaskStateService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TaskStateService> _logger;
        private readonly IEmailService _emailService;
        private readonly TaskNotificationSettings _settings;



        public TaskStateService(AppDbContext dbContext, ILogger<TaskStateService> logger, IEmailService emailService, IOptions<TaskNotificationSettings> settings)
        {
            _dbContext = dbContext;
            _logger = logger;
            _emailService = emailService;
            _settings = settings.Value;


        }

        public async Task UpdateTaskStatesAsync(CancellationToken cancellationToken)
        {
            var now = DateHelper.Today;
            var nextDay = DateHelper.Tomorrow;

            var allTasks = await _dbContext.Tasks.ToListAsync(cancellationToken);

        


            foreach (var task in allTasks)
            {
                var user = await _dbContext.Users.FindAsync(task.UserId);
                if (user == null || string.IsNullOrEmpty(user.Email)) continue;

                var shouldSendEmail = false;
                var nowUtc = DateTime.UtcNow;

                if (task.Duedate?.Date == now || task.Duedate?.Date == nextDay)
                {
                    task.State = TaskStates.Due;

                    if (!task.LastNotificationSentAt.HasValue || (nowUtc - task.LastNotificationSentAt.Value).TotalHours > _settings.EmailNotificationIntervalHours)
                    {
                        shouldSendEmail = true;
                    }
                }
                else if (task.Duedate?.Date < now)
                {
                    task.State = TaskStates.Overdue;

                    if (!task.LastNotificationSentAt.HasValue || (nowUtc - task.LastNotificationSentAt.Value).TotalHours > _settings.EmailNotificationIntervalHours)
                    {
                        shouldSendEmail = true;
                    }
                }

               

                if (shouldSendEmail)
                {
                   
                    var statusText = task.State == TaskStates.Overdue
                        ? TaskStates.Overdue
                        : TaskStates.Due;

                    var email = new EmailMessage
                    {
                        ToEmail = user.Email,
                        Subject = statusText,
                        Body = $"Your task '{task.Name}' is {statusText} on {task.Duedate:yyyy-MM-dd}"
                    };

                    await _emailService.SendEmailAsync(email);


                    task.LastNotificationSentAt = nowUtc;
                }

            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
