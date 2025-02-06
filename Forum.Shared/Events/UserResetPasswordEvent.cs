using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserResetPasswordEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public string UserEmail { get; }

        public UserResetPasswordEvent(string userEmail)
        {
            Id = Guid.NewGuid();
            UserEmail = userEmail;
        }
    }
}
