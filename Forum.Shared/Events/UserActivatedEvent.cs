using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserActivatedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        public DateTime ActivatedAt { get; }

        public UserActivatedEvent(Guid userId, string username, string email)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Username = username;
            Email = email;
            ActivatedAt = DateTime.UtcNow;
        }
    }
}
