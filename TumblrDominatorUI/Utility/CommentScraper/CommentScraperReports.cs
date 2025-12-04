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

namespace TumblrDominatorUI.Utility.CommentScraper
{
    public class CommentScraperReports : ITumblrReportFactory
    {
        public static ObservableCollection<CommentScraperReportDetails> InteractedPostModel =
            new ObservableCollection<CommentScraperReportDetails>();

        public static ObservableCollection<CommentScraperReportDetails> LstScrapedPosts =
            new ObservableCollection<CommentScraperReportDetails>();

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
                    InteractedPostModel.Add(new CommentScraperReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        PostUrl = report.PostUrl,
                        ProfileUrl = report.ProfileUrl,
                        UserName = report.InteractedUserName,
                        Type = report.Type,
                        CommentText = report.CommentText,
                        ReblogText = report.ReblogText
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Type", ColumnBindingText = "Type"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Comment Text", ColumnBindingText = "CommentText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Reblog Text", ColumnBindingText = "ReblogText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Post Url", ColumnBindingText = "PostUrl"},
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
                Header = "AccountName,Query Type, Query Value, Username, Date ,Type, CommentText, ReblogText,PostUrl";

                InteractedPostModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountName + "," + report.QueryType + "," + report.QueryValue + "," + report.UserName + "," +
                                    report.Date + "," + report.Type + "," + report.CommentText?.Replace("\r\n", " ")?.Replace("\n", " ")
                                    + "," + report.ReblogText?.Replace("\r\n", " ")?.Replace("\n", " ") + "," + report.PostUrl);
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
                Header = "Query Type, Query Value, Username, Date ,Type, CommentText, ReblogText";

                LstScrapedPosts.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.AccountName + "," +
                                    report.Date + "," + report.Type + "," + report.CommentText?.Replace("\r\n", " ")?.Replace("\n", " ")
                                    + "," + report.ReblogText?.Replace("\r\n", " ")?.Replace("\n", " "));
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
            LstScrapedPosts.Clear();
            IList reportDetails = dataBase.GetInteractedPosts(ActivityType.CommentScraper).ToList();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            foreach (InteractedPosts report in reportDetails)
            {
                _i = _i + 1;
                LstScrapedPosts.Add(
                    new CommentScraperReportDetails
                    {
                        Id = _i,
                        AccountName = report.AccountName,
                        QueryValue = report.QueryValue,
                        QueryType = report.QueryType,
                        PostUrl = report.PostUrl,
                        ProfileUrl = report.ProfileUrl,
                        UserName = report.InteractedUserName,
                        Type = report.Type,
                        CommentText = report.CommentText,
                        ReblogText = report.ReblogText,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime
                    });
            }

            return LstScrapedPosts;
        }
    }
}