using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EX.Core.Interfaces;
using EX.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace EX.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                _logger.LogInformation($"Starting to send email to {toEmail} with subject: {subject}");
                
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"] ?? "Asteel Flash RFQ System";
                var enableSsl = bool.TryParse(_configuration["EmailSettings:EnableSsl"], out var ssl) ? ssl : true;
                var useDefaultCredentials = bool.TryParse(_configuration["EmailSettings:UseDefaultCredentials"], out var useDefaultCreds) ? useDefaultCreds : false;

                _logger.LogInformation($"SMTP Configuration - Host: {smtpHost}, Port: {smtpPort}, Username: {smtpUsername}, FromEmail: {fromEmail}, EnableSsl: {enableSsl}, UseDefaultCredentials: {useDefaultCredentials}");

                if (string.IsNullOrEmpty(toEmail))
                {
                    _logger.LogWarning("Cannot send email: recipient email is null or empty");
                    return;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = useDefaultCredentials
                };

                if (!useDefaultCredentials)
                {
                    if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                    {
                        _logger.LogError("SMTP credentials are not configured but UseDefaultCredentials is false.");
                        throw new InvalidOperationException("SMTP credentials are required when UseDefaultCredentials is false.");
                    }

                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                _logger.LogInformation($"Attempting to send email via SMTP...");
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}. Error: {ex.Message}");
                throw;
            }
        }


        public async Task SendEmailToRoleAsync(string role, string subject, string message)
        {
            try
            {
                _logger.LogInformation($"Starting to send emails to all users with role: {role}");
                
                var users = _unitOfWork.GetRepository<User>()
                    .GetAll().Where(u => u.Role.ToString() == role).ToList();

                _logger.LogInformation($"Found {users.Count} users with role {role}");
                
                foreach (var user in users)
                {
                    _logger.LogInformation($"User: {user.NomUser}, Email: {user.Email}, Role: {user.Role}");
                }

                var emailTasks = users.Select(user => SendEmailAsync(user.Email, subject, message));
                await Task.WhenAll(emailTasks);

                _logger.LogInformation($"Emails sent to all {users.Count} users with role {role}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send emails to users with role {role}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailToUserAsync(int userId, string subject, string message)
        {
            try
            {
                _logger.LogInformation($"Starting to send email to user with ID: {userId}");
                
                var user = _unitOfWork.GetRepository<User>().Get(userId);
                if (user != null)
                {
                    _logger.LogInformation($"Found user: {user.NomUser}, Email: {user.Email}, Role: {user.Role}");
                    
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        await SendEmailAsync(user.Email, subject, message);
                    }
                    else
                    {
                        _logger.LogWarning($"User {user.NomUser} (ID: {userId}) has no email address configured");
                    }
                }
                else
                {
                    _logger.LogWarning($"User with ID {userId} not found in database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to user with ID {userId}. Error: {ex.Message}");
                throw;
            }
        }
    }
}