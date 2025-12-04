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

namespace TumblrDominatorUI.Utility.UserScraper
{
    public class UserScraperReports : ITumblrReportFactory
    {
        public static ObservableCollection<UserScraperReportDetails> InteractedUsersModel =
            new ObservableCollection<UserScraperReportDetails>();

        public static ObservableCollection<UserScraperReportDetails> AccountsInteractedUsers =
            new ObservableCollection<UserScraperReportDetails>();

        private int _i;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UserScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            if (InteractedUsersModel.Count >= 0) InteractedUsersModel.Clear();

            var forLocalTime = DateTime.Now - DateTime.UtcNow;

            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to ScraperReportModel

            dataBase.GetAllInteractedUsers().ForEach(
                report =>
                {
                    InteractedUsersModel.Add(new UserScraperReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        ProfileUrl = report.UserProfileUrl,
                        InteractedUserName = report.UserFullName
                    });
                });

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            #region Generate Reports column with data

            // campaign.SelectedAccountList.ToList().ForEach(x =>
            // {
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "UserName", ColumnBindingText = "InteractedUserName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "ProfileUrl", ColumnBindingText = "ProfileUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                };

            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

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
                Header = "Query Type, Query Value, Account Username, Username, ProfileUrl, Date";

                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.AccountName + "," +
                                    report.InteractedUserName
                                    + "," + report.ProfileUrl + "," + report.Date);
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
                Header = "Query Type, Query Value, Account Username, Username, ProfileUrl";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.AccountName + "," +
                                    report.InteractedUserName
                                    + "," + report.ProfileUrl);
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
            AccountsInteractedUsers.Clear();
            IList reportDetails = dataBase.GetInteractedUsers(ActivityType.UserScraper).ToList();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            foreach (InteractedUser report in reportDetails)
            {
                _i = _i + 1;
                AccountsInteractedUsers.Add(
                    new UserScraperReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        ProfileUrl = report.UserProfileUrl,
                        InteractedUserName = report.UserFullName
                    });
            }

            return AccountsInteractedUsers;
        }
    }
}