using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.QDFactories
{
    public class QdReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return QuoraInitialize.GetModuleLibrary(subModuleName).QdUtilityFactory().QdReportFactory
                .GetSavedQuery(subModuleName, activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = QuoraInitialize.GetModuleLibrary(activityType);

            return baseFactory.QdUtilityFactory().QdReportFactory
                .GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var baseFactory = QuoraInitialize.GetModuleLibrary(subModule);

            baseFactory.QdUtilityFactory().QdReportFactory.ExportReports(subModule, fileName, reportType);
        }
    }
}