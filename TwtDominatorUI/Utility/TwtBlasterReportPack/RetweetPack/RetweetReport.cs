using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using TwtDominatorCore.Interface;
using TwtDominatorCore.Report;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.RetweetPack
{
    internal class RetweetReport : ITDReportFactory
    {
        private static readonly ObservableCollection<InteractedTweetReport> ScrapeTweetReportModel =
            new ObservableCollection<InteractedTweetReport>();

        private static List<InteractedTweetReport> AccountsInteractedUsers = new List<InteractedTweetReport>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel ObjReports, CampaignDetails campaignDetails)
        {
            ScrapeTweetReportModel.Clear();
            // var dboperation = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);
            IDbCampaignService dboperation = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            try
            {
                dboperation.GetAllInteractedPosts().ForEach(
                    report =>
                    {
                        ScrapeTweetReportModel.Add(new InteractedTweetReport
                        {
                            SlNo = report.Id,
                            SinAccUsername = report.SinAccUsername,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            TweetId = report.TweetId,
                            TweetOwnerName = report.Username,
                            Retweet = report.IsRetweet == 1 ? " Yes " : " No ",
                            TweetedText = report.TwtMessage,
                            TweetedDate = report.TweetedTimeStamp.EpochToDateTimeUtc()
                                .ToString(CultureInfo.InvariantCulture),
                            FollowStatus = report.FollowStatus == 1 ? " Yes " : " No ",
                            FollowBackStatus = report.FollowBackStatus == 1 ? " Yes " : " No ",
                            InteractionDate = report.InteractionDate
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            #region Generate Reports column with data

            //     ObjReports.ReportCollection =
            //CollectionViewSource.GetDefaultView(ScrapeTweetReportModel);


            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            ObjReports.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sl No", ColumnBindingText = "SlNo"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Retweeted Tweet Id", ColumnBindingText = "TweetId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Tweeted Text", ColumnBindingText = "TweetedText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Tweeted Date", ColumnBindingText = "TweetedDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Tweet Owner", ColumnBindingText = "TweetOwnerName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Retweet", ColumnBindingText = "Retweet"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Following", ColumnBindingText = "FollowStatus"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Following Back", ColumnBindingText = "FollowBackStatus"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Retweeted Date", ColumnBindingText = "InteractionDate"}
                };
            //   });
            return new ObservableCollection<object>(ScrapeTweetReportModel);

            #endregion

            //return ScrapeUserReportModel.Count;
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = null;
            var SNo = 0;
            AccountsInteractedUsers = new List<InteractedTweetReport>();

            var temp = dataBase.GetInteractedPosts(ActivityType.Retweet).Where(x => x.QueryType != null);
            temp?.ForEach(
                report =>
                {
                    AccountsInteractedUsers.Add(new InteractedTweetReport
                    {
                        SlNo = ++SNo,
                        // SinAccUsername = report.SinAccUsername,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        TweetId = report.TweetId,
                        TweetOwnerName = report.Username,
                        TweetOwnerId = report.UserId,
                        Retweet = report.IsRetweet == 1 ? " Yes " : " No ",
                        TweetedText = report.TwtMessage,
                        TweetedDate = report.TweetedTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                            .ToString(CultureInfo.InvariantCulture),
                        LikeCount = report.LikeCount,
                        CommentCount = report.CommentCount,
                        RetweetCount = report.RetweetCount,
                        FollowStatus = report.FollowStatus == 1 ? " Yes " : " No ",
                        FollowBackStatus = report.FollowBackStatus == 1 ? " Yes " : " No ",
                        MediaPath = string.IsNullOrEmpty(report.MediaId) ? " NA " : report.MediaId,
                        InteractionDate = report.InteractionDate,
                        AlreadyLiked = report.IsAlreadyLiked == 1 ? "Yes" : "No",
                        AlreadyRetweeted = report.IsAlreadyRetweeted == 1 ? "Yes" : "No",
                        CommentedText = report.CommentedText,
                        ProcessType = report.ProcessType,
                        Category = report.MediaType.ToString()
                    });
                });


            #region retweet

            //  case ActivityType.Retweet:
            reportDetails = AccountsInteractedUsers.Select(x =>
                new
                {
                    x.SlNo,
                    x.QueryType,
                    x.QueryValue,
                    x.TweetId,
                    x.TweetedText,
                    x.TweetOwnerName,
                    x.TweetedDate,
                    x.MediaPath,
                    x.FollowStatus,
                    x.FollowBackStatus,
                    x.ProcessType,
                    x.InteractionDate
                }).ToList();
            // CsvHeader = "Sl No,Query Type,Query Value,Tweet Id,Tweeted Text,Tweet Owner,Tweeted Date, Media Path,Follow Status,Follow Back Status,Process Type,Retweeted Date";
            //break;

            #endregion

            return reportDetails;
        }

        public void ExportReports(string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Sl No,Account,Query Type,Query Value,Retweeted Tweet Id,Tweeted Text,Tweet Owner,Retweet,Following,Following Back,Retweeted Date";

                ScrapeTweetReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.QueryType + "," +
                                    report.QueryValue + ",'" + report.TweetId + "',\"" +
                                    report.TweetedText.Replace("\"", "\"\"") + "\"," + report.TweetOwnerName + "," +
                                    report.Retweet + "," + report.FollowStatus + "," + report.FollowBackStatus + "," +
                                    report.InteractionDate);
                        ;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
            {
                Header =
                    "Sl No,Query Type,Query Value,Tweet Id,Tweeted Text,Tweet Owner,Tweeted Date, Media Path,Follow Status,Follow Back Status,Process Type,Retweeted Date";

                AccountsInteractedUsers.ToList().ForEach(x =>
                {
                    try
                    {
                        csvData.Add(x.SlNo + "," + x.QueryType + "," + x.QueryValue + ",'" + x.TweetId + "',\"" +
                                    x.TweetedText.Replace("\"", "\"\"") + "\"," + x.TweetOwnerName + "," +
                                    x.TweetedDate +
                                    ",\"" + x.MediaPath.Replace("\"", "\"\"") + "\"," + x.FollowStatus + "," +
                                    x.FollowBackStatus + "," + x.ProcessType + "," + x.InteractionDate.ToString());
                        ;
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
    }
}