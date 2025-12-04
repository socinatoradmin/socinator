using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.Factories
{
    public class LdReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            var baseFactory = LinkedInInitialize.GetModuleLibrary(subModuleName);

            return baseFactory.LdUtilityFactory().LdReportFactory.GetSavedQuery(subModuleName, activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = LinkedInInitialize.GetModuleLibrary(ActivityType);

            return baseFactory.LdUtilityFactory().LdReportFactory
                .GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = LinkedInInitialize.GetModuleLibrary(activityType);

            baseFactory.LdUtilityFactory().LdReportFactory.ExportReports(activityType, fileName, reportType);
        }
    }
}