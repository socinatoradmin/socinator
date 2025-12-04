#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IReportFactory
    {
        /// <summary>
        ///     To get the saved query details via moduleName and activity settings
        /// </summary>
        /// <param name="subModuleName">pass the submodule name like follower, send friend request, and so on</param>
        /// <param name="activitySettings">pass the activity settings as the string which was saved already in bin file</param>
        /// <returns>returns all saved query details for the respective submodule and activity settings</returns>
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ActivityType activityType, string fileName, ReportType reportType);
    }
}