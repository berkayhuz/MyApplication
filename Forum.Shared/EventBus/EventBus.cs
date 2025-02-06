namespace Forum.Shared.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly List<Func<IEvent, Task>> _subscribers = new List<Func<IEvent, Task>>();
        private readonly Dictionary<Type, List<Func<IEvent, Task>>> _typedSubscribers = new Dictionary<Type, List<Func<IEvent, Task>>>();

        public void Subscribe<TEvent>(Func<TEvent, Task> eventHandler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (!_typedSubscribers.ContainsKey(eventType))
            {
                _typedSubscribers[eventType] = new List<Func<IEvent, Task>>();
            }
            _typedSubscribers[eventType].Add(e => eventHandler((TEvent)e));
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (_typedSubscribers.ContainsKey(eventType))
            {
                foreach (var handler in _typedSubscribers[eventType])
                {
                    Console.WriteLine($"Publishing event of type {eventType.Name}");
                    await handler(@event);
                }
            }
        }
    }
}
