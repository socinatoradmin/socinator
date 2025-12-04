namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface ITumblrSettings
    {
        bool IsTagUser { get; set; }

        string TagUserList { get; set; }
    }
}