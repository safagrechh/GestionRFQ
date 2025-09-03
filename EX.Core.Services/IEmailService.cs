using System.Threading.Tasks;

namespace EX.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendEmailToRoleAsync(string role, string subject, string message);
        Task SendEmailToUserAsync(int userId, string subject, string message);
    }
}