#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IGlobalInteractionDetails
    {
        GlobalInteractionDataModel this[SocialNetworks networks, ActivityType activityType] { get; }

        void AddInteractedData(SocialNetworks networks, ActivityType activityType, string interactedData);

        void RemoveIfExist(SocialNetworks networks, ActivityType activityType, string interactedData);

        List<string> GetInteractedData(SocialNetworks networks, ActivityType activityType);
    }
}