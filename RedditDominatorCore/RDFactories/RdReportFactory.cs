using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using RedditDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDFactories
{
    public class RdReportFactory : IReportFactory
    {
        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return RedditInitialize.GetModuleLibrary(subModuleName).RdUtilityFactory().RdReportFactory
                .GetSavedQuery(subModuleName, activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(DominatorHouseCore.Models.ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = RedditInitialize.GetModuleLibrary(activityType);
            return baseFactory.RdUtilityFactory().RdReportFactory
                .GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var baseFactory = RedditInitialize.GetModuleLibrary(subModule);

            baseFactory.RdUtilityFactory().RdReportFactory.ExportReports(subModule, fileName, reportType);
        }
    }
}