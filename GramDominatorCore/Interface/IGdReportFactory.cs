using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.Report;
using DominatorUIUtility.CustomControl;

namespace GramDominatorCore.Interface
{
   public interface IGdReportFactory
    {
        ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(ActivityType activityType, string fileName, ReportType reportType);

        IList GetsAccountReport(IDbAccountService dataBase);
      //  List<FollowReportDetails> GetFollowDetails(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        string Header { get; set; }

         


    }
}
