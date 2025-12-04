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

namespace TumblrDominatorUI.Utility.Reblog
{
    public class ReblogReports : ITumblrReportFactory
    {
        public static ObservableCollection<ReblogReportDetails> InteractedUsersModel =
            new ObservableCollection<ReblogReportDetails>();

        public static ObservableCollection<ReblogReportDetails> AccountsInteractedUsers =
            new ObservableCollection<ReblogReportDetails>();

        private int _i;
        public string AccountUserId = string.Empty;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ReblogModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            if (InteractedUsersModel.Count >= 0) InteractedUsersModel.Clear();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to ReblogReportModel

            dataBase.GetAllInteractedPosts().ForEach(
                report =>
                {
                    InteractedUsersModel.Add(new ReblogReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        ContentId = report.ContentId,
                        PostUrl = report.PostUrl,
                        ReblogUrl = report.ReblogUrl
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Post Url", ColumnBindingText = "PostUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Reblog Url", ColumnBindingText = "ReblogUrl"},
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
                Header = "Query Type, Query Value, Account Username,Post Url, Reblogged Post, Date";

                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.AccountName + "," +
                                    report.PostUrl + "," + report.ReblogUrl + "," + report.Date);
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
                Header = "Query Type, Query Value ,Post Url, Reblogged Post, Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," +
                                    report.PostUrl + "," + report.ReblogUrl + "," + report.Date);
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
            AccountsInteractedUsers = new ObservableCollection<ReblogReportDetails>();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            IList reportDetails = dataBase.GetInteractedPosts(ActivityType.Reblog).ToList();

            foreach (InteractedPosts report in reportDetails)
            {
                _i = _i + 1;
                AccountsInteractedUsers.Add(
                    new ReblogReportDetails
                    {
                        Id = _i,
                        AccountName = report.Comments,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        PostOwner = report.InteractedUserName,
                        Query = report.QueryValue,
                        PostUrl = report.PostUrl
                    });
            }

            return AccountsInteractedUsers;
        }
    }
}