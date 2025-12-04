using System.Collections;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.Interface
{
    public interface ITDReportFactory
    {
        ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings);

        ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            CampaignDetails campaignDetails);

        void ExportReports(string fileName, ReportType reportType);

        IList GetsAccountReport(IDbAccountService dataBase);
    }
}