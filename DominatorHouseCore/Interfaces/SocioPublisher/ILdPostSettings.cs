#region

using DominatorHouseCore.Enums.SocioPublisher;

#endregion

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface ILdPostSettings
    {
        LdGroupPostType GroupPostType { get; set; }
    }
}