using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class EmailConfirmedEvent : IEvent
    {
        public Guid UserId { get; }
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public string Username { get; }
        public string Email { get; }

        public EmailConfirmedEvent(Guid userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
        }
    }
}