using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDFactories
{
    public class TDReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            var activityType = subModuleName;

            var baseFactory = TDInitialise.GetModuleLibrary(activityType);

            return baseFactory.TDUtilityFactory().TDReportFactory.GetSavedQuery(activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails,
            CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = TDInitialise.GetModuleLibrary(activityType);

            return baseFactory.TDUtilityFactory().TDReportFactory
                .GetCampaignsReport(reportModel, campaignDetails);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = TDInitialise.GetModuleLibrary(activityType);

            baseFactory.TDUtilityFactory().TDReportFactory.ExportReports(fileName, reportType);
        }
    }
}