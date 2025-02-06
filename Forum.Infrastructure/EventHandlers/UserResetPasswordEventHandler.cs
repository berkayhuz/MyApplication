using Forum.Infrastructure.Notifications;
using Forum.Shared.Events;

namespace Forum.Infrastructure.EventHandlers
{
    public class UserResetPasswordEventHandler
    {
        private readonly EmailNotificationService _emailNotificationService;

        public UserResetPasswordEventHandler(EmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public async Task HandleAsync(UserResetPasswordEvent @event)
        {
             await _emailNotificationService.SendPasswordResetConfirmationEmailAsync(@event.UserEmail);
        }
    }
}
