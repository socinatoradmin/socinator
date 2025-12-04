namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface ITdPostSettings
    {
        bool IsDeletePostAfterHours { get; set; }

        int DeletePostAfterHours { get; set; }

        bool IsMentionUser { get; set; }

        string MentionUserList { get; set; }
    }
}