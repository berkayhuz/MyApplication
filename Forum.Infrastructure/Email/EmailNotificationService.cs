using Forum.Domain.Models;
using Forum.Infrastructure.Email;

namespace Forum.Infrastructure.Notifications
{
    public class EmailNotificationService
    {
        private readonly IMailService _emailSender;
        public EmailNotificationService(IMailService emailSender)
        {
            _emailSender = emailSender;
        }
        
        public async Task SendPasswordResetConfirmationEmailAsync(string email)
        {
            var subject = "Password Reset Confirmation";
            var body = $"Hello, your password has been successfully reset. If you did not request this change, please contact support immediately.";

            var mailRequest = new MailRequest
            {
                ToEmails = new List<string> { email },
                Subject = subject,
                Body = body
            };
            await _emailSender.SendEmailAsync(mailRequest);
        }

        public async Task SendForgotPasswordConfirmationEmailAsync(string email, string token)
        {
            var forgotLink = $"https://berkayhuz.com/forgot-password?token={token}&email={email}";

            var mailRequest = new MailRequest
            {
                ToEmails = new List<string> { email },
                Subject = "Password Reset Request",
                Body = $"Please reset your password by clicking here: <a href='{forgotLink}'>Reset Password</a>"
            };

            await _emailSender.SendEmailAsync(mailRequest);
        }

        public async Task SendConfirmationEmailAsync(string email, string token)
        {
            var confirmationLink = $"https://berkayhuz.com/confirm-email?token={token}&email={email}";

            var mailRequest = new MailRequest
            {
                ToEmails = new List<string> { email },
                Subject = "Email Confirmation",
                Body = $"Please reset your password by clicking here: <a href='{confirmationLink}'>link</a>"
            };
            await _emailSender.SendEmailAsync(mailRequest);
        }
    }
}
