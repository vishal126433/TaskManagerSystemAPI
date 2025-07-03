using TaskManager.Models;

namespace TaskManager.Services.Notifications
{

    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }

}


