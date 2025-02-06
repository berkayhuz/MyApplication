using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserDeactivatedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        public DateTime DeactivatedAt { get; }

        public UserDeactivatedEvent(Guid userId, string username, string email)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Username = username;
            Email = email;
            DeactivatedAt = DateTime.UtcNow;
        }
    }
}
