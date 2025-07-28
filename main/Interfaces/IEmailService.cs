using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);

    }
}
