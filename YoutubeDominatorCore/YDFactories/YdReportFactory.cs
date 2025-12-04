using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            var baseFactory = YoutubeInitialize.GetModuleLibrary(subModuleName);

            return baseFactory.YdUtilityFactory().YdReportFactory.GetSavedQuery(subModuleName, activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = YoutubeInitialize.GetModuleLibrary(activityType);

            return baseFactory.YdUtilityFactory().YdReportFactory
                .GetCampaignsReport(reportModel, queryDetails, campaignDetails);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = YoutubeInitialize.GetModuleLibrary(activityType);

            baseFactory.YdUtilityFactory().YdReportFactory.ExportReports(reportType, fileName);
        }
    }
}