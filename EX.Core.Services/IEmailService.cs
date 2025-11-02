using System.Threading.Tasks;

namespace EX.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendEmailToRoleAsync(string role, string subject, string message);
        Task SendEmailToRoleExcludingAsync(string role, string subject, string message, int excludeUserId);
        Task SendEmailToRoleExcludingEmailAsync(string role, string subject, string message, string excludeEmail);
        Task SendEmailToUserAsync(int userId, string subject, string message);
    }
}