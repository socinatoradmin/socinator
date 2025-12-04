using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominatorCore.PDLibrary.DAL;

namespace PinDominatorCore.Interface
{
    public interface IPdReportFactory
    {
        string Header { get; set; }
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ReportType dataSelectionType, string fileName);

        IList GetsAccountReport(IDbAccountService dataBase);
    }
}