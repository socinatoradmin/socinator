#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IQueryFollowedControl
    {
        void AssignReportDetailsToModel(ObservableCollection<object> FollowRepoertList, CampaignDetails Campaign);
    }
}