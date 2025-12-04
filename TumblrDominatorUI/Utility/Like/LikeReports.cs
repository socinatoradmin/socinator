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

namespace TumblrDominatorUI.Utility.Like
{
    public class LikeReports : ITumblrReportFactory
    {
        public static ObservableCollection<LikeReportDetails> InteractedUsersModel =
            new ObservableCollection<LikeReportDetails>();

        public static ObservableCollection<AccountReportEngage> AccountsInteractedUsers =
            new ObservableCollection<AccountReportEngage>();

        private int _i;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<LikeModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            if (InteractedUsersModel.Count >= 0) InteractedUsersModel.Clear();

            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to LikeReportModel

            dataBase.GetAllInteractedPosts().ForEach(
                report =>
                {
                    InteractedUsersModel.Add(new LikeReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        ContentId = report.PostUrl
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Like", ColumnBindingText = "ContentId"},
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
                Header = "Query Type, Query Value, Account Username, Liked Username, Date";

                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.AccountName + "," +
                                    report.ContentId.ToString() + "," + report.Date);
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
                Header = "Query Type, Query Value, Liked Username, Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," +
                                    report.PostOwner + "," + report.Date);
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
            IList reportDetails = dataBase.GetInteractedPosts(ActivityType.Like).ToList();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            foreach (InteractedPosts report in reportDetails)
            {
                _i = _i + 1;
                AccountsInteractedUsers.Add(
                    new AccountReportEngage
                    {
                        Id = _i,
                        MediaType = report.MediaType,
                        Query = report.QueryValue,
                        QueryType = report.QueryType,
                        PostOwner = report.InteractedUserName,
                        PostUrl = report.PostUrl,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime
                    });
            }

            return AccountsInteractedUsers;
        }
    }
}