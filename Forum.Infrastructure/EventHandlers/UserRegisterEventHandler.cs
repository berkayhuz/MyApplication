using Forum.Infrastructure.Notifications;
using Forum.Shared.Events;

namespace Forum.Infrastructure.EventHandlers
{
    public class UserRegisterEventHandler
    {
        private readonly EmailNotificationService _emailNotificationService;

        public UserRegisterEventHandler(EmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public async Task HandleAsync(UserRegisteredEvent @event)
        {
            await _emailNotificationService.SendConfirmationEmailAsync(@event.Email, @event.Token);
        }
    }
}
