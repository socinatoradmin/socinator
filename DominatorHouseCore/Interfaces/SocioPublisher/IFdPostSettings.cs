namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IFdPostSettings
    {
        bool IsReplaceDescriptionSelected { get; set; }

        string PostReplaceDescription { get; set; }
    }
}