namespace DominatorHouseCore.Interfaces
{
    public interface IEvent
    {
        string EventId { get; set; }
        string EventName { get; set; }
        string OwnerId { get; set; }
    }
}