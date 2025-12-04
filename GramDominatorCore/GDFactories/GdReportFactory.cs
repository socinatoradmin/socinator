using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using GramDominatorCore.Utility;
using GramDominatorCore.Report;

namespace GramDominatorCore.GDFactories
{
    public class GdReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            var baseFactory = InstagramInitialize.GetModuleLibrary(subModuleName);
            string subModuleNames = subModuleName.ToString();
            return baseFactory.GdUtilityFactory().GdReportFactory.GetSavedQuery(subModuleNames, activitySettings);
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            var baseFactory = InstagramInitialize.GetModuleLibrary(activityType);
           
            return baseFactory.GdUtilityFactory().GdReportFactory.GetCampaignsReport(reportModel, queryDetails, campaignDetails);
            
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var baseFactory = InstagramInitialize.GetModuleLibrary(activityType);

            baseFactory.GdUtilityFactory().GdReportFactory.ExportReports(activityType, fileName, reportType);
        }

    }
}
