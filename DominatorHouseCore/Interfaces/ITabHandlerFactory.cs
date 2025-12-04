#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface ITabHandlerFactory
    {
        List<TabItemTemplates> NetworkTabs { get; set; }

        void UpdateAccountCustomControl(SocialNetworks networks);
    }
}