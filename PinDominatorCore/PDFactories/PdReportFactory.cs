using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using PinDominatorCore.Utility;

namespace PinDominatorCore.PDFactories
{
    public class PdReportFactory : IReportFactory
    {
        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = PinterestInitialize.GetModuleLibrary(activityType);

            return baseFactory.PdUtilityFactory().PdReportFactory
                .GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            var baseFactory = PinterestInitialize.GetModuleLibrary(subModuleName);

            return baseFactory.PdUtilityFactory().PdReportFactory.GetSavedQuery(subModuleName, activitySettings);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = PinterestInitialize.GetModuleLibrary(activityType);

            baseFactory.PdUtilityFactory().PdReportFactory.ExportReports(reportType, fileName);
        }
    }
}