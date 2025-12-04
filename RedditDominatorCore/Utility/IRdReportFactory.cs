using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorCore.RDLibrary.DAL;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.Utility
{
    public interface IRdReportFactory
    {
        string Header { get; set; }
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(DominatorHouseCore.Models.ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ActivityType subModule, string fileName, ReportType reportType);

        IList GetAccountReport(IDbAccountService dataBase);
    }
}