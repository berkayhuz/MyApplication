using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserLoginEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public string UserEmail { get; }
        public DateTime LoggedDate { get; }

        public UserLoginEvent(string userEmail, DateTime loggedDate)
        {
            Id = Guid.NewGuid();
            UserEmail = userEmail;
            LoggedDate = loggedDate;
        }
    }
}
