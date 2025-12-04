using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return TumblrInitialize.GetModuleLibrary(subModuleName).TumblrUtilityFactory().TumblrReportFactory
                .GetSavedQuery(subModuleName, activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = TumblrInitialize.GetModuleLibrary(activityType);

            return baseFactory.TumblrUtilityFactory().TumblrReportFactory
                .GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = TumblrInitialize.GetModuleLibrary(activityType);

            baseFactory.TumblrUtilityFactory().TumblrReportFactory.ExportReports(reportType, fileName);
        }
    }
}