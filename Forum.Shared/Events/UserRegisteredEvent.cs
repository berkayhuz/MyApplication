using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserRegisteredEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public string Email { get; }
        public string Token { get; }

        public UserRegisteredEvent(string email, string token)
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            Email = email;
            Token = token;
        }
    }
}
