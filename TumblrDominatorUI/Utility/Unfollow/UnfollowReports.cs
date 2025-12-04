using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorUI.Report;

namespace TumblrDominatorUI.Utility.Unfollow
{
    public class UnfollowReports : ITumblrReportFactory
    {
        public static ObservableCollection<UnfollowReportDetails> InteractedUsersModel =
            new ObservableCollection<UnfollowReportDetails>();

        public static ObservableCollection<UnfollowReportDetails> AccountsInteractedUsers =
            new ObservableCollection<UnfollowReportDetails>();

        private int _i;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            if (InteractedUsersModel.Count >= 0) InteractedUsersModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to UnfollowReportModel

            dataBase.GetAllUnfollowedUsers().ForEach(
                report =>
                {
                    InteractedUsersModel.Add(new UnfollowReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.UserName,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        UnfollowedUsername = report.InteractedUsername
                    });
                });
            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Unfollowed User", ColumnBindingText = "UnfollowedUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                };

            //   reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();
            var value = dataSelectionType.ToString();
            var reports = (ReportType)Enum.Parse(typeof(ReportType), value);

            #region Campaign reports

            if (reports == ReportType.Campaign)
            {
                Header = "Account Username, UnFollowed Username, Date";

                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountName + "," + report.UnfollowedUsername + "," + report.Date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reports == ReportType.Account)
            {
                Header = " Account Username, Unfollowed Username, Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountName + "," + report.UnfollowedUsername + "," + report.Date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetAccountReport(IDbAccountService dataBase)
        {
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            IList reportDetails = dataBase.GetUnfollowedUsers(ActivityType.Unfollow).ToList();

            foreach (UnFollowedUser report in reportDetails)
            {
                _i = _i + 1;
                AccountsInteractedUsers.Add(
                    new UnfollowReportDetails
                    {
                        Id = _i,
                        AccountName = report.UserName,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        UnfollowedUsername = report.InteractedUsername
                    });
            }

            return AccountsInteractedUsers;
        }
    }
}