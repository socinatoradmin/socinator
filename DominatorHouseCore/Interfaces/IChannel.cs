namespace DominatorHouseCore.Interfaces
{
    public interface IChannel
    {
        string ChannelId { get; set; }

        string ChannelName { get; set; }
        string ProfilePicUrl { get; set; }
    }
}