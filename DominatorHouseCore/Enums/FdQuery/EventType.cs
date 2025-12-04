#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum EventType
    {
        [Description("Create Private Event")] CreatePrivateEvent = 1,
        [Description("Create Public Event")] CreatePublicEvent = 2
    }
}