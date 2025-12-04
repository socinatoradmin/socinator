using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoutubeDominatorCore.YoutubeLibrary.DAL;

namespace YoutubeDominatorCore.Interface
{
    public interface IYdReportFactory
    {
        string Header { get; set; }
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ReportType dataSelectionType, string fileName);

        IList GetsAccountReport(IDbAccountService dataBase);
    }
}