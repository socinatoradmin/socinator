using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.Report;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeModel;

namespace YoutubeDominatorUI.Utility.UserScraperUtility
{
    public class UserScraperReports : IYdReportFactory
    {
        public string Header { get; set; } = string.Empty;

        List<string> CsvData = new List<string>();

        public static ObservableCollection<InteractedChannelsReport> InteractedChannelsModel = new ObservableCollection<InteractedChannelsReport>();

        public static List<InteractedChannelsReport> AccountsInteractedChannels = new List<InteractedChannelsReport>();

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
            => JsonConvert.DeserializeObject<UserScraperModel>(activitySettings).SavedQueries;

        public int GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            int id = 1;
            InteractedChannelsModel.Clear();
            IDbCampaignService _dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            StringBuilder Sb_C = new StringBuilder();

            #region get data from InteractedUsers table and add to UserScraperReportModel
            _dbCampaignService.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedChannels>().ForEach(
           report =>
           {
               InteractedChannelsModel.Add(new InteractedChannelsReport()
               {
                   Id = id++,
                   QueryValue = report.QueryValue,
                   QueryType = report.QueryType,
                   ActivityType = report.ActivityType,
                   AccountUsername = report.AccountUsername,
                   ChannelJoinedDate = report.ChannelJoinedDate,
                   ChannelLocation = report.ChannelLocation,
                   ChannelProfilePic = report.ChannelProfilePic,
                   ChannelUrl = report.ChannelUrl,
                   ChannelDescription = report.ChannelDescription,
                   ExternalLinks = report.ExternalLinks,
                   InteractedChannelId = report.InteractedChannelId,
                   InteractedChannelName = report.InteractedChannelName,
                   InteractionTimeStamp = report.InteractionTimeStamp,
                   Message = report.MessageToChannelOwner,
                   SubscriberCount = report.SubscriberCount,
                   VideosCount = report.VideosCount,
                   ViewsCount = report.ViewsCount,
                   SubscribeStatus = report.SubscribeStatus
               });

               Sb_C.Append(report.Id.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.QueryValue.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.QueryType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ActivityType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.AccountUsername.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ChannelJoinedDate.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ChannelLocation.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ChannelProfilePic.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ChannelUrl.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ChannelDescription.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ExternalLinks.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.InteractedChannelId.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.InteractedChannelName.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.InteractionTimeStamp.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.MessageToChannelOwner.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.SubscriberCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.VideosCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.ViewsCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
               Sb_C.Append(report.SubscribeStatus.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim());
               CsvData.Add(Sb_C.ToString());
           });
            #endregion

            #region Generate Reports column with data
            
            reportModel.GridViewColumn =
            new ObservableCollection<GridViewColumnDescriptor>
            {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Status", ColumnBindingText = "SubscribeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Account Username", ColumnBindingText = "AccountUsername" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "External Links", ColumnBindingText = "ExternalLinks" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelId", ColumnBindingText = "InteractedChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelName", ColumnBindingText = "InteractedChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Message To ChannelOwner", ColumnBindingText = "MessageToChannelOwner" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscriber Count", ColumnBindingText = "SubscriberCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Description", ColumnBindingText = "ChannelDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel JoinedDate", ColumnBindingText = "ChannelJoinedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Location", ColumnBindingText = "ChannelLocation" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel ProfilePic", ColumnBindingText = "ChannelProfilePic" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Url", ColumnBindingText = "ChannelUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Videos Count", ColumnBindingText = "VideosCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
            };

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedChannelsModel);

            #endregion

            return InteractedChannelsModel.Count;
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            int id = 1;
            AccountsInteractedChannels.Clear();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedChannels>().Where(x => x.ActivityType == ActivityType.UserScraper.ToString()).ToList();

            CsvData = new List<string>();

            foreach (DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedChannels report in reportDetails)
            {
                StringBuilder Sb_C = new StringBuilder();
                AccountsInteractedChannels.Add(
                                       new InteractedChannelsReport
                                       {
                                           Id = id++,
                                           QueryValue = report.QueryValue,
                                           QueryType = report.QueryType,
                                           ActivityType = report.ActivityType,
                                           AccountUsername = report.AccountUsername,
                                           ChannelJoinedDate = report.ChannelJoinedDate,
                                           ChannelLocation = report.ChannelLocation,
                                           ChannelProfilePic = report.ChannelProfilePic,
                                           ChannelUrl = report.ChannelUrl,
                                           ChannelDescription = report.ChannelDescription,
                                           ExternalLinks = report.ExternalLinks,
                                           InteractedChannelId = report.InteractedChannelId,
                                           InteractedChannelName = report.InteractedChannelName,
                                           InteractionTimeStamp = report.InteractionTimeStamp,
                                           Message = report.MessageToChannelOwner,
                                           SubscriberCount = report.SubscriberCount,
                                           VideosCount = report.VideosCount,
                                           ViewsCount = report.ViewsCount,
                                           SubscribeStatus = report.SubscribeStatus
                                       });
                Sb_C.Append(report.Id.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.QueryValue.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.QueryType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ActivityType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.AccountUsername.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ChannelJoinedDate.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ChannelLocation.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ChannelProfilePic.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ChannelUrl.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ChannelDescription.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ExternalLinks.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.InteractedChannelId.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.InteractedChannelName.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.InteractionTimeStamp.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.MessageToChannelOwner.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.SubscriberCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.VideosCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.ViewsCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim() + ",");
                Sb_C.Append(report.SubscribeStatus.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim());
                CsvData.Add(Sb_C.ToString());
            }
            
            return AccountsInteractedChannels;
        }

        public void ExportReports(ReportType reportType, string fileName)
        {
            Header = YoutubeDominatorCore.YDEnums.Enums.ChannelHeaderStringAccountReport();

            var csvData = new List<string>();

            #region Account reports

            if (reportType == ReportType.Account)
            {

                AccountsInteractedChannels.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                            + "," + report.QueryValue.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                            + "," + report.QueryType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                            + "," + report.ActivityType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.AccountUsername.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.ChannelJoinedDate.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + ",'" + report.ChannelLocation.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "'," + report.ChannelProfilePic.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.ChannelUrl.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.ChannelDescription.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.ExternalLinks.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.InteractedChannelId.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.InteractedChannelName.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.InteractionTimeStamp.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.Message.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.SubscriberCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.VideosCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.ViewsCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                        + "," + report.SubscribeStatus.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim());
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {

                InteractedChannelsModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                             + "," + report.QueryValue.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                             + "," + report.QueryType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                             + "," + report.ActivityType.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.AccountUsername.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.ChannelJoinedDate.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + ",'" + report.ChannelLocation.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "'," + report.ChannelProfilePic.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.ChannelUrl.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.ChannelDescription.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.ExternalLinks.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.InteractedChannelId.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.InteractedChannelName.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.InteractionTimeStamp.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.Message.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.SubscriberCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.VideosCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.ViewsCount.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()
                         + "," + report.SubscribeStatus.ToString().Replace(",", ":-:").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim());
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

            }

            #endregion

            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}
