namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IRedditSettings
    {
        bool IsNsfw { get; set; }

        bool IsOriginalContent { get; set; }

        bool IsSpoiler { get; set; }

        bool IsDisableSendingReplies { get; set; }
    }
}