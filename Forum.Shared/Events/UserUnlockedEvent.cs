using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserUnlockedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }

        public UserUnlockedEvent(Guid userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
        }
    }

}
