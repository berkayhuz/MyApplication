namespace Forum.Shared.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        void Subscribe<TEvent>(Func<TEvent, Task> eventHandler) where TEvent : IEvent;
    }
}
