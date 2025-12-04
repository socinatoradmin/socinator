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

namespace TumblrDominatorUI.Utility.PostScraper
{
    public class PostScraperReports : ITumblrReportFactory
    {
        public static ObservableCollection<PostScraperReportDetails> InteractedPostModel =
            new ObservableCollection<PostScraperReportDetails>();

        public static ObservableCollection<PostScraperReportDetails> AccountScrapedPosts =
            new ObservableCollection<PostScraperReportDetails>();

        private int _i;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<PostScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            if (InteractedPostModel.Count >= 0) InteractedPostModel.Clear();

            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedPosts table and add to ScraperReportModel

            dataBase.GetAllInteractedPosts().ForEach(
                report =>
                {
                    InteractedPostModel.Add(new PostScraperReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        UserName = report.InteractedUserName,
                        ProfileUrl = report.ProfileUrl,
                        PostUrl = report.PostUrl,
                        NotesCount = report.NotesCount,
                        LikesCount = report.LikeCount,
                        ReblogCount = report.ReblogCount
                    });
                });

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostModel);

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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Post", ColumnBindingText = "PostUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Total Notes", ColumnBindingText = "NotesCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Total Likes", ColumnBindingText = "LikesCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Total Reblogs", ColumnBindingText = "ReblogCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                };

            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedPostModel);
        }


        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            var value = dataSelectionType.ToString();
            var reports = (ReportType)Enum.Parse(typeof(ReportType), value);

            #region Campaign reports

            if (reports == ReportType.Campaign)
            {
                Header = "Query Type, Query Value, Username, Date ,PostUrl, Total Notes, LikesCount, ReblogCount";

                InteractedPostModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.AccountName + "," +
                                    report.Date + "," + report.PostUrl + "," + report.NotesCount
                                    + "," + report.LikesCount + "," + report.ReblogCount);
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
                Header = "Query Type, Query Value, Username, Date ,Post, Total Notes, LikesCount, ReblogCount";

                AccountScrapedPosts.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.AccountName + "," +
                                    report.Date + "," + report.PostUrl + "," + report.NotesCount
                                    + "," + report.LikesCount + "," + report.ReblogCount);
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
            AccountScrapedPosts.Clear();
            IList reportDetails = dataBase.GetInteractedPosts(ActivityType.PostScraper).ToList();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            foreach (InteractedPosts report in reportDetails)
            {
                _i = _i + 1;
                AccountScrapedPosts.Add(
                    new PostScraperReportDetails
                    {
                        Id = _i,
                        AccountName = report.AccountName,
                        QueryValue = report.QueryValue,
                        QueryType = report.QueryType,
                        UserName = report.InteractedUserName,
                        ProfileUrl = report.ProfileUrl,
                        PostUrl = report.PostUrl,
                        NotesCount = report.NotesCount,
                        LikesCount = report.LikeCount,
                        ReblogCount = report.ReblogCount,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime
                    });
            }

            return AccountScrapedPosts;
        }
    }
}