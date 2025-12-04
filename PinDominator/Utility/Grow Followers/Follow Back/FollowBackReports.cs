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

namespace PinDominator.Utility.Grow_Followers.Follow_Back
{
    public class FollowBackReports : IPdReportFactory
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

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            var id = 1;
            var followBackActivity = ActivityType.FollowBack.ToString();
            dbCampaignService.Get<InteractedUsers>(x => x.ActivityType == followBackActivity).ForEach(
                report =>
                {
                    InteractedUsersReportModel.Add(new InteractedUsersReportDetails
                    {
                        Id = id++,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        InteractionTime =
                            (report.InteractionTime.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        ActivityType = report.ActivityType,
                        SinAccUsername = report.SinAccUsername,
                        InteractedUsername = report.InteractedUsername,
                        InteractedUserId = report.InteractedUserId,
                        DirectMessage = report.DirectMessage,
                        FollowersCount = report.FollowersCount,
                        FollowingsCount = report.FollowingsCount,
                        PinsCount = report.PinsCount,
                        FullName = report.FullName,
                        HasAnonymousProfilePicture = report.HasAnonymousProfilePicture,
                        ProfilePicUrl = report.ProfilePicUrl,
                        Bio = report.Bio
                    });
                });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Time", ColumnBindingText = "InteractionTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Username", ColumnBindingText = "InteractedUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted User Id", ColumnBindingText = "InteractedUserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted User Id", ColumnBindingText = "InteractedUserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followers Count", ColumnBindingText = "FollowersCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Direct Message", ColumnBindingText = "DirectMessage"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Pins Count", ColumnBindingText = "PinsCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Full Name", ColumnBindingText = "FullName"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "Has Anonymous Profile Picture",
                        ColumnBindingText = "HasAnonymousProfilePicture"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Profile Pic Url", ColumnBindingText = "ProfilePicUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Bio", ColumnBindingText = "Bio"}
                };
            //});

            #endregion

            return new ObservableCollection<object>(InteractedUsersReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstInteractedUserAccount.Clear();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.FollowBack.ToString()).ToList();
            var id = 1;
            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers item in reportDetails)
                LstInteractedUserAccount.Add(
                    new InteractedUsersReportDetails
                    {
                        Id = id++,
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
                        ProfilePicUrl = item.ProfilePicUrl
                    }
                );

            return LstInteractedUserAccount.Select(x =>
                new
                {
                    x.Id,
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
                    x.ProfilePicUrl
                }).ToList();
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header =
                    "Followed Back,Activity Type,User name, UserId,Interaction Time ,Full Name, Followers Count,Followings Count,Pins Count,Tries Count,Website,Bio,Has Anonymous Profile Picture, Profile Pic Url";

                InteractedUsersReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.FollowedBack + "," + report.ActivityType + "," + report.InteractedUsername +
                                    ",'" + report.InteractedUserId
                                    + "'," + report.InteractionTime + "," + report.FullName + "," +
                                    report.FollowersCount + "," + report.FollowingsCount
                                    + "," + report.PinsCount + "," + report.TriesCount + "," + report.Website + ","
                                    + report.Bio?.Replace(',', '.') + "," + report.HasAnonymousProfilePicture + "," +
                                    report.ProfilePicUrl);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header =
                    "Followed Back,Activity Type,User name, Interacted Username, Interacted UserId,Interaction Time ,Full Name, Followers Count,Followings Count,Pins Count,Tries Count,Website,Bio,Has Anonymous Profile Picture, Profile Pic Url";

                LstInteractedUserAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.FollowedBack + "," + report.ActivityType + "," + report.Username + "," +
                                    report.InteractedUsername
                                    + ",'" + report.InteractedUserId + "'," + report.InteractionTime + "," +
                                    report.FullName + "," + report.FollowersCount
                                    + "," + report.FollowingsCount + "," + report.PinsCount + "," + report.TriesCount +
                                    "," + report.Website + ","
                                    + report.Bio?.Replace(',', '.') + "," + report.HasAnonymousProfilePicture + "," +
                                    report.ProfilePicUrl);
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