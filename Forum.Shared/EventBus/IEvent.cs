namespace Forum.Shared.EventBus
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime CreatedDate { get; }
    }
}
