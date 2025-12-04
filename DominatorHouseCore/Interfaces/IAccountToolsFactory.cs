#region

using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountToolsFactory
    {
        UserControl GetStartupToolsView();

        IEnumerable<ActivityType> GetImportantActivityTypes();

        IEnumerable<ActivityType> GetOtherActivityTypes();

        string RecentlySelectedAccount { get; set; }
    }
}