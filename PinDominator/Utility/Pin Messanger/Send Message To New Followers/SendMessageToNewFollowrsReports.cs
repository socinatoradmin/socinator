using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.Interface;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Report;

namespace PinDominator.Utility.Pin_Messanger.Send_Message_To_New_Followers
{
    public class SendMessageToNewFollowrsReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportDetails> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportDetails>();

        private static readonly List<InteractedUsersReportDetails> LstInteractedUserAccount =
            new List<InteractedUsersReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedUsersReportModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
            var id = 1;
            var sendMessageToFollowerActivity = ActivityType.SendMessageToFollower.ToString();

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            dbCampaignService.Get<InteractedUsers>(x => x.ActivityType == sendMessageToFollowerActivity).ForEach(
                report =>
                {
                    InteractedUsersReportModel.Add(new InteractedUsersReportDetails
                    {
                        Id = id++,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        Type = report.Type,
                        SinAccUsername = report.SinAccUsername,
                        FollowedBack = report.FollowedBack,
                        InteractionTime =
                            (report.InteractionTime.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        ActivityType = report.ActivityType,
                        Username = report.Username,
                        InteractedUsername = report.InteractedUsername,
                        InteractedUserId = report.InteractedUserId,
                        FollowersCount = report.FollowersCount,
                        FollowingsCount = report.FollowingsCount,
                        PinsCount = report.PinsCount,
                        FullName = report.FullName,
                        HasAnonymousProfilePicture = report.HasAnonymousProfilePicture,
                        ProfilePicUrl = report.ProfilePicUrl,
                        Bio = report.Bio,
                        BoardDescription = report.BoardDescription,
                        BoardName = report.BoardName,
                        BoardUrl = report.BoardUrl,
                        Website = report.Website,
                        DirectMessage = report.DirectMessage
                    });
                });

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Type", ColumnBindingText = "Type"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followed Back", ColumnBindingText = "FollowedBack"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Time", ColumnBindingText = "InteractionTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Username", ColumnBindingText = "InteractedUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted User Id", ColumnBindingText = "InteractedUserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followers Count", ColumnBindingText = "FollowersCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followings Count", ColumnBindingText = "FollowingsCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Pins Count", ColumnBindingText = "PinsCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Full Name", ColumnBindingText = "FullName"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "Has Anonymous Profile Picture",
                        ColumnBindingText = "HasAnonymousProfilePicture"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Profile Pic Url", ColumnBindingText = "ProfilePicUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Bio", ColumnBindingText = "Bio"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Board Description", ColumnBindingText = "BoardDescription"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Name", ColumnBindingText = "BoardName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Url", ColumnBindingText = "BoardUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Website", ColumnBindingText = "Website"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Message", ColumnBindingText = "DirectMessage"}
                };

            #endregion

            return new ObservableCollection<object>(InteractedUsersReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstInteractedUserAccount.Clear();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.SendMessageToFollower.ToString()).ToList();
            var id = 1;
            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers item in reportDetails)
                LstInteractedUserAccount.Add(
                    new InteractedUsersReportDetails
                    {
                        Id = id++,
                        Type = item.Type,
                        Query = item.Query,
                        QueryType = item.QueryType,
                        FollowedBack = item.FollowedBack,
                        ActivityType = item.ActivityType,
                        InteractedUsername = item.InteractedUsername,
                        InteractedUserId = item.InteractedUserId,
                        InteractionTime =
                            (item.InteractionTime.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        FullName = item.FullName,
                        FollowersCount = item.FollowersCount,
                        FollowingsCount = item.FollowingsCount,
                        PinsCount = item.PinsCount,
                        TriesCount = item.TriesCount,
                        Website = item.Website,
                        Bio = item.Bio,
                        HasAnonymousProfilePicture = item.HasAnonymousProfilePicture,
                        ProfilePicUrl = item.ProfilePicUrl,
                        BoardDescription = item.BoardDescription,
                        BoardName = item.BoardName,
                        BoardUrl = item.BoardUrl,
                        DirectMessage = item.DirectMessage
                    }
                );

            return LstInteractedUserAccount.Select(x =>
                new
                {
                    x.Id,
                    x.Type,
                    x.Query,
                    x.QueryType,
                    x.FollowedBack,
                    x.ActivityType,
                    x.InteractedUsername,
                    x.InteractedUserId,
                    x.InteractionTime,
                    x.FullName,
                    x.FollowersCount,
                    x.PinsCount,
                    x.TriesCount,
                    x.Website,
                    x.Bio,
                    x.HasAnonymousProfilePicture,
                    x.ProfilePicUrl,
                    x.BoardDescription,
                    x.BoardName,
                    x.BoardUrl,
                    x.DirectMessage
                }).ToList();
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header =
                    "Query,Query Type,Followed Back,Activity Type,Username,User Id,Interaction Time,Full Name,Followers Count," +
                    "Followings Count,Pins Count,Tries Count,Website,Bio,Has Anonymous Profile Picture,Profile Pic Url";

                LstInteractedUserAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Query + "," + report.QueryType + "," + report.FollowedBack + "," +
                                    report.ActivityType
                                    + "," + report.Username + "," + report.InteractedUsername + ",'" +
                                    report.InteractedUserId + "'," + report.InteractionTime
                                    + "," + report.FullName + "," + report.FollowersCount + "," +
                                    report.FollowingsCount + "," + report.PinsCount
                                    + "," + report.TriesCount + "," + report.Website + "," +
                                    report.Bio.Replace(',', '.') + "," + report.HasAnonymousProfilePicture
                                    + "," + report.ProfilePicUrl);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header =
                    "Id,Query,Query Type,Followed Back,Interaction Time,Activity Type,Username,Interacted Username,Interacted User Id," +
                    "Followers Count,Followings Count,Pins Count,Tries Count,Full Name,Has Anonymous Profile Picture,Profile Pic Url,Bio,Website";

                InteractedUsersReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id + "," + report.Query + "," + report.QueryType + "," +
                                    report.FollowedBack + "," + report.InteractionTime + ","
                                    + report.ActivityType + "," + report.Username + "," + report.InteractedUsername +
                                    "," + report.InteractedUserId + ","
                                    + report.FollowersCount + "," + report.FollowingsCount + "," + report.PinsCount +
                                    "," + report.TriesCount + "," + report.FullName + ","
                                    + report.HasAnonymousProfilePicture + "," + report.ProfilePicUrl + "," +
                                    report.Bio?.Replace(',', '.') + ","
                                    + report.Website);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}