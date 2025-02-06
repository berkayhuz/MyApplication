using Forum.Infrastructure.Notifications;
using Forum.Shared.Events;

namespace Forum.Infrastructure.EventHandlers
{
    public class UserForgotPasswordEventHandler
    {
        private readonly EmailNotificationService _emailNotificationService;

        public UserForgotPasswordEventHandler(EmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public async Task HandleAsync(UserForgotPasswordEvent @event)
        {
            await _emailNotificationService.SendForgotPasswordConfirmationEmailAsync(@event.UserEmail, @event.Token);
        }
    }
}
