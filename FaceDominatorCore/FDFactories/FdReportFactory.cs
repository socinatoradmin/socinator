using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FaceDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDFactories
{
    public class FdReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
            => FacebookInitialize.GetModuleLibrary(subModuleName).FdUtilityFactory().FdReportFactory.GetSavedQuery(subModuleName, activitySettings);

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = FacebookInitialize.GetModuleLibrary(activityType);

            return baseFactory.FdUtilityFactory().FdReportFactory.GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = FacebookInitialize.GetModuleLibrary(activityType);

            baseFactory.FdUtilityFactory().FdReportFactory.ExportReports(activityType, fileName, reportType);
        }

    }
}
