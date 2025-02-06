using Forum.Shared.EventBus;

namespace Forum.Shared.Events
{
    public class UserForgotPasswordEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public string UserEmail { get; }
        public string Token { get; }

        public UserForgotPasswordEvent(string userEmail, string token)
        {
            UserEmail = userEmail;
            Token = token;
        }
    }
}
