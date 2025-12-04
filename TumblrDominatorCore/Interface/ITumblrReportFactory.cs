using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TumblrDominatorCore.TumblrLibrary.DAL;

namespace TumblrDominatorCore.Interface
{
    public interface ITumblrReportFactory
    {
        string Header { get; set; }
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ReportType dataSelectionType, string fileName);

        //void ExportReports(ActivityType subModule, string fileName, ReportType reportType);
        IList GetAccountReport(IDbAccountService dataBase);
    }
}