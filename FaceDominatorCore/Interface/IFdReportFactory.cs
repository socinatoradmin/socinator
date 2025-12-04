using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDLibrary.DAL;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.Interface
{
    public interface IFdReportFactory
    {
        ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ActivityType activityType, string fileName, ReportType reportType);

        IList GetsAccountReport(DbAccountService dataBase);

        string Header { get; set; }
    }
}