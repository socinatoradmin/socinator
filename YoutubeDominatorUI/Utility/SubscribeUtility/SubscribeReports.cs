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
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;

namespace YoutubeDominatorUI.Utility.SubscribeUtility
{
    public class SubscribeReports : IYdReportFactory
    {
        public static ObservableCollection<InteractedChannelsReport> InteractedChannelsModel =
            new ObservableCollection<InteractedChannelsReport>();

        public static ObservableCollection<InteractedChannelsReport> AccountsInteractedChannels =
            new ObservableCollection<InteractedChannelsReport>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<SubscribeModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var id = 1;
            InteractedChannelsModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to SubscribeReportModel

            dbCampaignService.GetAllInteractedChannels().ForEach(
                report =>
                {
                    var time =
                        (report.InteractionTimeStamp.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                            .InvariantCulture);
                    InteractedChannelsModel.Add(new InteractedChannelsReport
                    {
                        Id = id++,
                        QueryValue = report.QueryValue,
                        QueryType = report.QueryType,
                        ActivityType = report.ActivityType,
                        AccountUsername = report.AccountUsername,
                        AccountChannelId = report.MyChannelId,
                        ChannelJoinedDate = report.ChannelJoinedDate,
                        ChannelLocation = report.ChannelLocation,
                        ChannelProfilePic = report.ChannelProfilePic,
                        ChannelUrl = report.ChannelUrl,
                        ChannelDescription = report.ChannelDescription,
                        ExternalLinks = report.ExternalLinks,
                        InteractedChannelId = report.InteractedChannelId,
                        InteractedChannelName = report.InteractedChannelName,
                        InteractionTime = time,
                        SubscriberCount = report.SubscriberCount,
                        IsSubscribed = report.IsSubscribed,
                        ViewsCount = report.ViewsCount,
                        VideosCount = report.VideosCount,
                        InteractedCommentUrl = report.InteractedCommentUrl
                    });
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
                        {ColumnHeaderText = "Interacted ChannelName", ColumnBindingText = "InteractedChannelName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted ChannelId", ColumnBindingText = "InteractedChannelId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Channel Url", ColumnBindingText = "ChannelUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Subscriber Count", ColumnBindingText = "SubscriberCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Is Subscribed", ColumnBindingText = "IsSubscribed"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Channel Description", ColumnBindingText = "ChannelDescription"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Channel JoinedDate", ColumnBindingText = "ChannelJoinedDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Channel Location", ColumnBindingText = "ChannelLocation"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Channel ProfilePic", ColumnBindingText = "ChannelProfilePic"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Time", ColumnBindingText = "InteractionTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "External Links", ColumnBindingText = "ExternalLinks"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "Interacted Comment Url(if any)", ColumnBindingText = "InteractedCommentUrl"
                    }
                };

            #endregion

            return new ObservableCollection<object>(InteractedChannelsModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            var id = 1;
            AccountsInteractedChannels.Clear();
            var actType = ActivityType.Subscribe.ToString();
            IList reportDetails = dataBase.Get<InteractedChannels>().Where(x => x.ActivityType == actType).ToList();

            foreach (InteractedChannels report in reportDetails)
            {
                var time =
                    (report.InteractionTimeStamp.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                        .InvariantCulture);

                AccountsInteractedChannels.Add(
                    new InteractedChannelsReport
                    {
                        Id = id++,
                        QueryValue = report.QueryValue,
                        QueryType = report.QueryType,
                        ActivityType = report.ActivityType,
                        AccountUsername = report.AccountUsername,
                        AccountChannelId = report.MyChannelId,
                        ChannelJoinedDate = report.ChannelJoinedDate,
                        ChannelLocation = report.ChannelLocation,
                        ChannelProfilePic = report.ChannelProfilePic,
                        ChannelUrl = report.ChannelUrl,
                        ChannelDescription = report.ChannelDescription,
                        ExternalLinks = report.ExternalLinks,
                        InteractedChannelId = report.InteractedChannelId,
                        InteractedChannelName = report.InteractedChannelName,
                        InteractionTime = time,
                        SubscriberCount = report.SubscriberCount,
                        ViewsCount = report.ViewsCount,
                        VideosCount = report.VideosCount,
                        InteractedCommentUrl = report.InteractedCommentUrl
                    });
            }

            return AccountsInteractedChannels;
        }

        public void ExportReports(ReportType reportType, string fileName)
        {
            var csvData = new List<string>();
            var utilities = new YoutubeUtilities();
            var addInteractedCommentUrl = false;
            if (reportType == ReportType.Account)
            {
                addInteractedCommentUrl = AccountsInteractedChannels.ToList()
                    .Any(x => !string.IsNullOrWhiteSpace(x.InteractedCommentUrl));

                AccountsInteractedChannels.ToList().ForEach(report =>
                {
                    csvData.Add(utilities.CreateCsvDataForChannel(report, addInteractedCommentUrl));
                });
            }
            else if (reportType == ReportType.Campaign)
            {
                addInteractedCommentUrl = InteractedChannelsModel.ToList()
                    .Any(x => !string.IsNullOrWhiteSpace(x.InteractedCommentUrl));

                InteractedChannelsModel.ToList().ForEach(report =>
                {
                    csvData.Add(utilities.CreateCsvDataForChannel(report, addInteractedCommentUrl));
                });
            }

            Header = utilities.HeaderStringForChannelReport(addInteractedCommentUrl);

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}