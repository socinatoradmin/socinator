using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorCore.LDLibrary.DAL;

namespace LinkedDominatorCore.Interfaces
{
    public interface ILdReportFactory
    {
        string Header { get; set; }
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ActivityType activityType, string fileName, ReportType reportType);

        IList GetsAccountReport(IDbAccountService dataBase);
    }
}