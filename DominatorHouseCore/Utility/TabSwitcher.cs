#region

using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;

#endregion

namespace DominatorHouseCore.Utility
{
    public class TabSwitcher
    {
        public static Action GoToCampaign { get; set; }

        public static Action<int, int?> ChangeTabIndex { get; set; } = (i, j) =>
            GlobusLogHelper.log.Error("ChangeTabIndex wasn't set");


        public static Action<int, SocialNetworks, string> ChangeTabWithNetwork { get; set; }
    }
}