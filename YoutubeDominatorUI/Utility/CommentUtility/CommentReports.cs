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

namespace YoutubeDominatorUI.Utility.CommentUtility
{
    public class CommentReports : IYdReportFactory
    {
        public static ObservableCollection<InteractedPostsReport> InteractedPostsModel =
            new ObservableCollection<InteractedPostsReport>();

        public static ObservableCollection<InteractedPostsReport> AccountsInteractedPosts =
            new ObservableCollection<InteractedPostsReport>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var id = 1;
            InteractedPostsModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to CommentReportModel

            dbCampaignService.GetAllInteractedPosts().ForEach(
                report =>
                {
                    if (report != null)
                    {
                        var time =
                            (report.InteractionTimeStamp.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture);

                        InteractedPostsModel.Add(new InteractedPostsReport
                        {
                            Id = id++,
                            ActivityType = ActivityType.Comment.ToString(),
                            AccountUsername = report.AccountUsername,
                            AccountChannelId = report.MyChannelId,
                            ChannelId = report.ChannelId,
                            ChannelName = report.ChannelName,
                            CommentCount = report.CommentCount,
                            MyCommentedText = report.MyCommentedText,
                            DislikeCount = report.DislikeCount,
                            InteractionTime = time,
                            CommentId = report.CommentId,
                            LikeCount = report.LikeCount,
                            Reaction = report.ReactionOnPost,
                            PostDescription = report.PostDescription,
                            PublishedDate = report.PublishedDate,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            SubscribeCount = report.SubscribeCount,
                            IsSubscribed = report.IsSubscribed,
                            VideoDuration = TimeSpan.FromSeconds(report.VideoLength).ToString(),
                            VideoUrl = report.VideoUrl,
                            ViewsCount = report.ViewsCount,
                            VideoTitle = report.PostTitle,
                            InteractedCommentUrl = report.InteractedCommentUrl
                        });
                    }
                });

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Id", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "AccountUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account ChannelId", ColumnBindingText = "AccountChannelId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "My commented Text", ColumnBindingText = "MyCommentedText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "CommentId", ColumnBindingText = "CommentId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Title", ColumnBindingText = "VideoTitle"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Reaction(Like) On Post", ColumnBindingText = "Reaction"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Likes Count", ColumnBindingText = "LikeCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Video Duration(hh:mm:ss)", ColumnBindingText = "VideoDuration"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "OwnerChannelId", ColumnBindingText = "ChannelId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "OwnerChannelName", ColumnBindingText = "ChannelName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Is channel subscribed", ColumnBindingText = "IsSubscribed"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTime"},
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
                .Where(x => x.ActivityType == ActivityType.Comment.ToString()).ToList();

            foreach (InteractedPosts item in reportDetails)
            {
                var time =
                    (item.InteractionTimeStamp.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                        .InvariantCulture);
                AccountsInteractedPosts.Add(
                    new InteractedPostsReport
                    {
                        Id = id++,
                        QueryValue = item.QueryValue,
                        QueryType = item.QueryType,
                        ActivityType = item.ActivityType,
                        AccountUsername = item.AccountUsername,
                        AccountChannelId = item.MyChannelId,
                        ViewsCount = item.ViewsCount,
                        ChannelId = item.ChannelId,
                        ChannelName = item.ChannelName,
                        CommentCount = item.CommentCount,
                        MyCommentedText = item.MyCommentedText,
                        CommentId = item.CommentId,
                        DislikeCount = item.DislikeCount,
                        LikeCount = item.LikeCount,
                        Reaction = item.ReactionOnPost,
                        PostDescription = item.PostDescription,
                        PublishedDate = item.PublishedDate,
                        SubscribeCount = item.SubscribeCount,
                        IsSubscribed = item.IsSubscribed,
                        VideoDuration = TimeSpan.FromSeconds(item.VideoLength).ToString(),
                        VideoUrl = item.VideoUrl,
                        InteractionTime = time,
                        VideoTitle = item.PostTitle,
                        InteractedCommentUrl = item.InteractedCommentUrl
                    });
            }

            return AccountsInteractedPosts;
        }

        public void ExportReports(ReportType reportType, string fileName)
        {
            var csvData = new List<string>();
            var utilities = new YoutubeUtilities();
            var addInteractedCommentUrl = false;
            if (reportType == ReportType.Account)
            {
                addInteractedCommentUrl = AccountsInteractedPosts.ToList()
                    .Any(x => !string.IsNullOrWhiteSpace(x.InteractedCommentUrl));
                AccountsInteractedPosts.ToList().ForEach(report =>
                {
                    csvData.Add(utilities.CreateCsvDataForPost(report, true, addCommentId: true,
                        addInteractedCommentUrl: addInteractedCommentUrl));
                });
            }
            else if (reportType == ReportType.Campaign)
            {
                addInteractedCommentUrl = InteractedPostsModel.ToList()
                    .Any(x => !string.IsNullOrWhiteSpace(x.InteractedCommentUrl));
                InteractedPostsModel.ToList().ForEach(report =>
                {
                    csvData.Add(utilities.CreateCsvDataForPost(report, true, addCommentId: true,
                        addInteractedCommentUrl: addInteractedCommentUrl));
                });
            }

            Header = utilities.HeaderStringForPostReport(true, addCommentId: true,
                addInteractedCommentUrl: addInteractedCommentUrl);
            Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}