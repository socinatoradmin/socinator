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

namespace TwtDominatorUI.Utility.ScraperReportPack.ScrapeTweetPack
{
    internal class ScrapeTweetReport : ITDReportFactory
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
            IDbCampaignService dboperation = new DbCampaignService(campaignDetails.CampaignId);
            // var dboperation = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);

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
                            TweetOwnerId = report.UserId,
                            Retweet = report.IsRetweet == 1 ? " Yes " : " No ",
                            TweetedText = report.TwtMessage,
                            TweetedDate = report.TweetedTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                                .ToString(CultureInfo.InvariantCulture),
                            LikeCount = report.LikeCount,
                            CommentCount = report.CommentCount,
                            RetweetCount = report.RetweetCount,
                            //   FollowStatus = report.FollowStatus == 1 ? " Yes " : " No ",
                            //   FollowBackStatus = report.FollowBackStatus == 1 ? " Yes " : " No ",
                            MediaPath = string.IsNullOrEmpty(report.MediaId) ? " NA " : report.MediaId,
                            InteractionDate = report.InteractionDate,
                            AlreadyRetweeted=report.IsAlreadyRetweeted== 1 ?" Yes ":" No ",
                            AlreadyLiked=report.IsAlreadyLiked == 1 ? " Yes " : " No "
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            #region Generate Reports column with data

            //    ObjReports.ReportCollection =
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Scraped Tweet Id", ColumnBindingText = "TweetId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Tweeted Text", ColumnBindingText = "TweetedText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Tweeted Time", ColumnBindingText = "TweetedDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Tweet Owner Name", ColumnBindingText = "TweetOwnerName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Tweet Owner Id", ColumnBindingText = "TweetOwnerId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Retweet Count", ColumnBindingText = "RetweetCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Retweet", ColumnBindingText = "Retweet"},
                    //  new GridViewColumnDescriptor { ColumnHeaderText = "Following", ColumnBindingText ="FollowStatus"},
                    //new GridViewColumnDescriptor { ColumnHeaderText = "Following Back", ColumnBindingText ="FollowBackStatus"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Media Path", ColumnBindingText = "MediaPath"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped Date", ColumnBindingText = "InteractionDate"}
                };
            //     });
            return new ObservableCollection<object>(ScrapeTweetReportModel);

            #endregion

            // return ScrapeUserReportModel.Count;
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = null;
            AccountsInteractedUsers = new List<InteractedTweetReport>();
            var SNo = 0;
            dataBase.GetInteractedPosts(ActivityType.TweetScraper)?.ForEach(
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

            #region tweetScraper

            // case ActivityType.TweetScraper:
            reportDetails = AccountsInteractedUsers.Select(x =>
                new
                {
                    x.SlNo,
                    x.QueryType,
                    x.QueryValue,
                    x.TweetId,
                    x.TweetOwnerName,
                    x.TweetedText,
                    x.TweetedDate,
                    x.LikeCount,
                    x.CommentCount,
                    x.RetweetCount,
                    //    x.FollowStatus,
                    //    x.FollowBackStatus,
                    //    x.Retweet,
                    x.AlreadyLiked,
                    x.AlreadyRetweeted,
                    x.MediaPath,
                    x.InteractionDate
                }).ToList();
            // CsvHeader = "Sl No,Query Type,Query Value,Tweet Id,Tweet Owner,Tweeted Text, Tweeted Date,Like Count,Comment Count,Retweet Count,Follow Status,Follow Back Status,Retweet,Already Liked,Already Retweeted,Media Path,Scrape Date";
            // break;

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
                    "Sl No,Account,Query Type,Query Value,Tweet Id,Tweeted Text,Tweeted Date,Tweet Owner Name,Tweet Owner Id,Likes Count,Retweets Count,Comments Count,Retweeted,Liked,Media Path,Scraped Date";

                ScrapeTweetReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        var ImagePath = report.MediaPath.Replace("\n", " : ");
                        var TweetedText = report.TweetedText.Replace("\n", " ").Replace(",", ".");
                        csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.QueryType + "," +
                                    report.QueryValue + ",'" + report.TweetId + "',\"" +
                                    TweetedText.Replace("\"", "\"\"") + "\"," + report.TweetedDate + "," +
                                    report.TweetOwnerName + ",'" + report.TweetOwnerId + "'," + report.LikeCount + "," +
                                    report.RetweetCount + "," + report.CommentCount + "," + report.AlreadyRetweeted + "," +
                                    report.AlreadyLiked + "," + ImagePath + "," + report.InteractionDate);
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
                    "Sl No,Query Type,Query Value,Tweet Id,Tweeted Text,Tweeted Date,Tweet Owner Name,Tweet Owner Id,Likes Count,Retweets Count,Comments Count,Retweeted,Liked,Media Path,Scraped Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        var ImagePath = report.MediaPath.Replace("\n", " : ");
                        var TweetedText = report.TweetedText.Replace("\n", " ").Replace(",", ".");
                        csvData.Add(report.SlNo + "," + report.QueryType + "," + report.QueryValue + ",'" +
                                    report.TweetId + "',\"" + TweetedText.Replace("\"", "\"\"") + "\"," +
                                    report.TweetedDate + "," + report.TweetOwnerName + ",'" + report.TweetOwnerId +
                                    "'," + report.LikeCount + "," + report.RetweetCount + "," + report.CommentCount +
                                    "," + report.Retweet + "," + report.AlreadyLiked + "," + ImagePath + "," +
                                    report.InteractionDate);
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

        //public IList GetsAccountReport(IDbAccountService dataBase)
        //{
        //    throw new NotImplementedException();
        //}
    }
}