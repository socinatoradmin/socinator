using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.Report;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorUI.Utility.LikeCommentUtility
{
    public class LikeCommentReports : IYdReportFactory
    {
        public static ObservableCollection<InteractedPostsReport> InteractedPostsModel =
            new ObservableCollection<InteractedPostsReport>();

        public static ObservableCollection<InteractedPostsReport> AccountsInteractedPosts =
            new ObservableCollection<InteractedPostsReport>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<LikeCommentModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var id = 1;
            InteractedPostsModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to LikeCommentReportModel

            dbCampaignService.GetAllInteractedPosts().ForEach(
                report =>
                {
                    if (!string.IsNullOrWhiteSpace(report.ChannelId))
                    {
                        var time =
                            (report.InteractionTimeStamp.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture);
                        InteractedPostsModel.Add(new InteractedPostsReport
                        {
                            Id = id++,
                            ActivityType = ActivityType.LikeComment.ToString(),
                            AccountUsername = report.AccountUsername,
                            AccountChannelId = report.MyChannelId,
                            MyCommentedText = report.MyCommentedText,
                            CommentId = report.CommentId,
                            ChannelId = report.ChannelId,
                            ChannelName = report.ChannelName,
                            CommentCount = report.CommentCount,
                            InteractionTime = time,
                            LikeCount = report.LikeCount,
                            Reaction = report.ReactionOnPost,
                            PublishedDate = report.PublishedDate,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            VideoDuration = TimeSpan.FromSeconds(report.VideoLength).ToString(),
                            VideoUrl = report.VideoUrl,
                            ViewsCount = report.ViewsCount,
                            VideoTitle = report.PostTitle,
                            InteractedCommentUrl = report.InteractedCommentUrl,
                            IsSubscribed = report.IsSubscribed,
                            PostDescription = report.PostDescription
                        });
                    }
                });

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Id", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account ChannelId", ColumnBindingText = "AccountChannelId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Title", ColumnBindingText = "VideoTitle"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Liked Comment Text", ColumnBindingText = "MyCommentedText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "CommentId", ColumnBindingText = "CommentId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Reacted on Comment", ColumnBindingText = "Reaction"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Comment Posted", ColumnBindingText = "PublishedDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Commenter Channel Name", ColumnBindingText = "ChannelName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Commenter Channel Id", ColumnBindingText = "ChannelId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Comment's Likes Count", ColumnBindingText = "LikeCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Video Views Count", ColumnBindingText = "ViewsCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Video Comments Count", ColumnBindingText = "CommentCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Video Duration(hh:mm:ss)", ColumnBindingText = "VideoDuration"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Is channel subscribed", ColumnBindingText = "IsSubscribed"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "Interacted Comment Url(if any)", ColumnBindingText = "InteractedCommentUrl"
                    }
                };

            #endregion

            return new ObservableCollection<object>(InteractedPostsModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            var id = 1;
            AccountsInteractedPosts.Clear();
            IList reportDetails = dataBase.Get<InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.LikeComment.ToString()).ToList();

            foreach (InteractedPosts item in reportDetails)
            {
                var time =
                    (item.InteractionTimeStamp.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                        .InvariantCulture);
                AccountsInteractedPosts.Add(
                    new InteractedPostsReport
                    {
                        Id = id++,
                        ActivityType = ActivityType.LikeComment.ToString(),
                        AccountUsername = item.AccountUsername,
                        AccountChannelId = item.MyChannelId,
                        MyCommentedText = item.MyCommentedText,
                        CommentId = item.CommentId,
                        ChannelId = item.ChannelId,
                        ChannelName = item.ChannelName,
                        CommentCount = item.CommentCount,
                        InteractionTime = time,
                        LikeCount = item.LikeCount,
                        Reaction = item.ReactionOnPost,
                        PublishedDate = item.PublishedDate,
                        QueryType = item.QueryType,
                        QueryValue = item.QueryValue,
                        VideoDuration = TimeSpan.FromSeconds(item.VideoLength).ToString(),
                        VideoUrl = item.VideoUrl,
                        ViewsCount = item.ViewsCount,
                        VideoTitle = item.PostTitle,
                        InteractedCommentUrl = item.InteractedCommentUrl,
                        PostDescription = item.PostDescription
                    });
            }

            return AccountsInteractedPosts;
        }

        public void ExportReports(ReportType reportType, string fileName)
        {
            var csvData = new List<string>();
            var utilities = new YoutubeUtilities();
            if (reportType == ReportType.Account)
                AccountsInteractedPosts.ToList().ForEach(report =>
                {
                    csvData.Add(utilities.CreateCsvDataForCommentsOfPost(report));
                });
            else if (reportType == ReportType.Campaign)
                InteractedPostsModel.ToList().ForEach(report =>
                {
                    csvData.Add(utilities.CreateCsvDataForCommentsOfPost(report));
                });

            Header = utilities.HeaderStringForCommentsReport();

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}