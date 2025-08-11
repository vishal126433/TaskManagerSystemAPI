using AuthService.Data;
using Microsoft.Extensions.Options;
using TaskManager.Helpers;
using TaskManager.Models;
using TaskManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Services
{
    public class TaskStateService : ITaskStateService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TaskStateService> _logger;
        private readonly IEmailService _emailService;
        private readonly TaskNotificationSettings _settings;



        public TaskStateService(AppDbContext dbContext, ILogger<TaskStateService> logger, IEmailService emailService, IOptions<TaskNotificationSettings> settings)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext), ResponseMessages.Message.DBcontextNull);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), ResponseMessages.Message.LoggerNull);
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService), ResponseMessages.Message.EmailServiceNull);
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings.Value), ResponseMessages.Message.SettingsNull);


        }

        public async Task UpdateTaskStatesAsync(CancellationToken cancellationToken)
        {
            var now = DateHelper.Today;
            var nextDay = DateHelper.Tomorrow;

            var allTasks = await _dbContext.Tasks.ToListAsync(cancellationToken);
            _logger.LogInformation(ResponseMessages.Message.TaskStateService);

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
