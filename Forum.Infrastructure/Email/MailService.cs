using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Forum.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Forum.Infrastructure.Email
{
    public interface IMailService
    {
        Task<Result> SendEmailAsync(MailRequest mailRequest);
    }
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly MailSettings _mailSettings;
        private readonly IAmazonSimpleEmailService _mailService;
        public MailService(IOptions<MailSettings> mailSettings,
            IAmazonSimpleEmailService mailService, IConfiguration configuration)
        {
            _configuration = configuration;
            _mailSettings = mailSettings.Value;
            _mailService = mailService;
        }
        public async Task<Result> SendEmailAsync(MailRequest mailRequest)
        {
            if (string.IsNullOrEmpty(mailRequest.Body))
            {
                return Result.Failure("Email body cannot be empty.");
            }
            if (string.IsNullOrEmpty(mailRequest.Subject))
            {
                return Result.Failure("Email subject cannot be empty.");
            }
            if (mailRequest.ToEmails == null || !mailRequest.ToEmails.Any() || mailRequest.ToEmails.Contains(""))
            {
                return Result.Failure("Recipient email addresses cannot be empty.");
            }

            var validEmails = mailRequest.ToEmails
                .Where(email => IsValidEmail(email))
                .ToList();

            if (!validEmails.Any())
            {
                return Result.Failure("No valid email addresses found.");
            }

            var sender = string.IsNullOrEmpty(_mailSettings.DisplayName)
                ? _mailSettings.Mail
                : $"\"{_mailSettings.DisplayName}\" <{_mailSettings.Mail}>";

            if (string.IsNullOrEmpty(_mailSettings.Mail))
            {
                return Result.Failure("Sender email address cannot be empty.");
            }

            if (validEmails == null || !validEmails.Any())
            {
                return Result.Failure("Recipient email addresses cannot be empty.");
            }

            var mailBody = new Body(new Content(mailRequest.Body));
            if (mailBody == null || string.IsNullOrEmpty(mailBody.Text?.Data))
            {
                return Result.Failure("Email message cannot be empty.");
            }

            var message = new Message(new Content(mailRequest.Subject), mailBody);
            if (message == null || string.IsNullOrEmpty(message.Subject?.Data) || string.IsNullOrEmpty(message.Body?.Text?.Data))
            {
                return Result.Failure("Email message is invalid.");
            }

            var destination = new Destination(validEmails);
            if (destination == null || !destination.ToAddresses.Any())
            {
                return Result.Failure("Recipient email addresses cannot be empty.");
            }

            var request = new SendEmailRequest
            {
                Source = sender,
                Destination = destination,
                Message = message
            };

            await _mailService.SendEmailAsync(request);
            return Result.Success("Email sent successfully.");
        }
        private bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }
    }
}