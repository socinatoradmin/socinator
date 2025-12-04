using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.Interface;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Report;

namespace PinDominator.Utility.Grow_Followers.Follow
{
    public class FollowReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportDetails> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportDetails>();

        private static readonly List<InteractedUsersReportDetails> LstInteractedUserAccount =
            new List<InteractedUsersReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;

        public FollowerModel FollowerModel { get; set; }
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
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            FollowerModel = JsonConvert.DeserializeObject<FollowerModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            var id = 1;
            var followActivity = ActivityType.Follow.ToString();
            dbCampaignService.Get<InteractedUsers>(x => x.ActivityType == followActivity).ForEach(
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
                        Status = "Followed"
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
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Type", ColumnBindingText = "Type"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Followed Back", ColumnBindingText = "FollowedBack"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Time", ColumnBindingText = "InteractionTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Username", ColumnBindingText = "InteractedUsername"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Interacted User Id", ColumnBindingText = "InteractedUserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followers Count", ColumnBindingText = "FollowersCount"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Followings Count", ColumnBindingText = "FollowingsCount"},
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
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Board Description", ColumnBindingText = "BoardDescription"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Board Name", ColumnBindingText = "BoardName"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Board Url", ColumnBindingText = "BoardUrl"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Website", ColumnBindingText = "Website"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                };

            #endregion

            #region Generate Report for Try and Comment activity after follow action

            if (FollowerModel.ChkTryUserLatestPostsChecked || FollowerModel.ChkCommentOnUserLatestPostsChecked)
            {
                var status = string.Empty;

                dbCampaignService.Get<InteractedPosts>().ForEach(
                    report =>
                    {
                        var UserDetails = InteractedUsersReportModel.FirstOrDefault(x => x.Username == report.Username);
                        var tryActInt = ((int) ActivityType.Try).ToString();
                        var commentActInt = ((int) ActivityType.Comment).ToString();
                        if (report.OperationType == "Try" || report.OperationType == tryActInt)
                            status = "Tried";
                        else if (report.OperationType.ToString() == "Comment" || report.OperationType == commentActInt)
                            status = "Commented";
                        InteractedUsersReportModel.Add(new InteractedUsersReportDetails
                        {
                            Id = id++,
                            Query = report.Query,
                            QueryType = report.QueryType,
                            Type = "Pin",
                            InteractedUsername=report.Username,
                            SinAccUsername = report.SinAccUsername,
                            FollowersCount=UserDetails.FollowersCount,
                            FollowingsCount=UserDetails.FollowingsCount,
                            PinsCount=UserDetails.PinsCount,
                            FullName=UserDetails.FullName,
                            ProfilePicUrl=UserDetails.ProfilePicUrl,
                            Bio=UserDetails.Bio,
                            Website=UserDetails.Website,
                            HasAnonymousProfilePicture=UserDetails.HasAnonymousProfilePicture,
                            InteractedUserId=UserDetails.InteractedUserId,
                            InteractionTime =
                                (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                    .InvariantCulture),
                            ActivityType = report.OperationType.ToString(),
                            Username = report.Username,
                            PinId = report.PinId,
                            Status = status
                        });
                    });

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Type", ColumnBindingText = "Type"},
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
                        new GridViewColumnDescriptor {ColumnHeaderText = "Pin Id", ColumnBindingText = "PinId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                    };
            }

            #endregion

            return new ObservableCollection<object>(InteractedUsersReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstInteractedUserAccount.Clear();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.Follow.ToString()).ToList();
            //lstInteractedUser.Clear();
            var id = 1;
            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers item in reportDetails)
                if (item.Type == "User")
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
                            BoardUrl = item.BoardUrl
                        }
                    );
                else
                    LstInteractedUserAccount.Add(
                        new InteractedUsersReportDetails
                        {
                            Id = id++,
                            Type = item.Type,
                            Query = item.Query,
                            QueryType = item.QueryType,
                            ActivityType = item.ActivityType,
                            InteractedUsername = item.Username,
                            InteractedUserId = item.InteractedUserId,
                            BoardUrl = item.BoardUrl,
                            BoardDescription = item.BoardDescription,
                            InteractionTime =
                                (item.InteractionTime.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                    .InvariantCulture),
                            BoardName = item.BoardName,
                            FollowersCount = item.FollowersCount,
                            PinsCount = item.PinsCount
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
                    x.FollowingsCount,
                    x.PinsCount,
                    x.TriesCount,
                    x.Website,
                    x.Bio,
                    x.HasAnonymousProfilePicture,
                    x.ProfilePicUrl,
                    x.BoardName,
                    x.BoardDescription,
                    x.BoardUrl
                }).ToList();
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header =
                    "Id,Type,Query,Query Type,Followed Back,Interaction Time,Activity Type,Username,User Id,Followers Count,Followings Count,Pins Count,Full Name,Has Anonymous Profile Picture,Profile Pic Url,Bio,Board Description,Board Name,Board Url,Website";

                LstInteractedUserAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id + "," + report.Type + "," + report.Query + "," + report.QueryType + "," +
                                    report.FollowedBack + "," + report.InteractionTime
                                    + "," + report.ActivityType + "," + report.InteractedUsername + "," +
                                    report.InteractedUserId + "," + report.FollowersCount + ","
                                    + report.FollowingsCount + "," + report.PinsCount + "," + report.FullName + "," +
                                    report.HasAnonymousProfilePicture + ","
                                    + report.ProfilePicUrl + "," + report.Bio?.Replace(',', '.') + "," +
                                    report.BoardDescription?.Replace(',', '.') + ","
                                    + report.BoardName?.Replace(',', '.') + ","
                                    + report.BoardUrl?.Replace(',', '.') + "," + report.Website);
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
                    "Id,Type,Query,Query Type,Followed Back,Interaction Time,Activity Type,Username,Interacted Username,Interacted User Id,Followers Count,Followings Count,Pins Count,Full Name,Has Anonymous Profile Picture,Profile Pic Url,Bio,Website";

                InteractedUsersReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id + "," + report.Type + "," + report.Query + "," + report.QueryType + "," +
                                    report.FollowedBack + "," + report.InteractionTime
                                    + "," + report.ActivityType + "," + report.SinAccUsername + "," +
                                    report.InteractedUsername + "," + report.InteractedUserId + "," +
                                    report.FollowersCount + "," + report.FollowingsCount + "," + report.PinsCount +
                                    "," + report.FullName + "," +
                                    report.HasAnonymousProfilePicture + "," + report.ProfilePicUrl + "," +
                                    report.Bio?.Replace(',', '.') + ","+ report.Website);
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