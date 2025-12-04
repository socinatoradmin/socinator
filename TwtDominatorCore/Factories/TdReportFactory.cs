using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;

namespace TwtDominatorCore.Factories
{
    public class TdReportFactory : IReportFactory
    {
        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<object> GetReportDetail(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            throw new NotImplementedException();
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            throw new NotImplementedException();
        }
    }
}