using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
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
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.Utility.PinScrap.User_Scraper
{
    public class UserScraperReports : IPdReportFactory
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
            var userScraperActivity = ActivityType.UserScraper.ToString();
            dbCampaignService.Get<InteractedUsers>(x => x.ActivityType == userScraperActivity).ForEach(
                report =>
                {
                    InteractedUsersReportModel.Add(new InteractedUsersReportDetails
                    {
                        Id = id++,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        //Type = report.Type,
                        Username=report.Username,
                        SinAccUsername = report.SinAccUsername,
                        FollowedBack = report.FollowedBack,
                        InteractionTime =
                            (report.InteractionTime.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        ActivityType = report.ActivityType,
                        InteractedUsername = report.InteractedUsername,
                        InteractedUserId = report.InteractedUserId,
                        FollowersCount = report.FollowersCount,
                        FollowingsCount = report.FollowingsCount,
                        PinsCount = report.PinsCount,
                        FullName = report.FullName,
                        HasAnonymousProfilePicture = report.HasAnonymousProfilePicture,
                        ProfilePicUrl = report.ProfilePicUrl,
                        Bio = report.Bio,
                        Website = report.Website
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
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followed Back", ColumnBindingText = "FollowedBack"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Time", ColumnBindingText = "InteractionTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Website", ColumnBindingText = "Website"}
                };
            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersReportModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstInteractedUserAccount.Clear();
            var userScraperActivity = ActivityType.UserScraper.ToString();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == userScraperActivity).ToList();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers item in reportDetails)
                if (item.Type == PinterestIdentityType.User.ToString())
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
                            //TriesCount = item.TriesCount,
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
                            Username = item.Username,
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
                    x.InteractionTime,
                    x.FullName,
                    x.FollowersCount,
                    x.FollowingsCount,
                    x.PinsCount,
                    x.Website,
                    x.Bio,
                    x.HasAnonymousProfilePicture,
                    x.ProfilePicUrl,
                    x.BoardDescription,
                    x.BoardName,
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
                    "Query, QueryType, Followed Back, Activity Type, Interacted Username, Interacted User Id, Interaction Time," +
                    "Full Name, Followers Count, Followings Count, Pins Count, Website, Bio, Has Anonymous Profile Picture, Profile Pic Url";

                LstInteractedUserAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report?.Query + "," + (report?.QueryType + "," + report.FollowedBack + ","
                                    + report.ActivityType + "," + report.InteractedUsername + ",'"
                                    + report.InteractedUserId + "'," + report.InteractionTime + "," + report?.FullName +
                                    ","
                                    + report?.FollowersCount + "," + report?.FollowingsCount + "," + report?.PinsCount +
                                    ","
                                    + report?.Website + "," + report?.Bio.AsCsvData() + ","
                                   + report?.HasAnonymousProfilePicture + "," + report?.ProfilePicUrl));
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
                    "Username, Query, QueryType, Followed Back, Activity Type, Interacted Username, Interacted User Id, " +
                    "Interaction Time, Full Name, Followers Count, Followings Count, Pins Count, Website, Bio, " +
                    "Has Anonymous Profile Picture, Profile Pic Url";

                InteractedUsersReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report?.Username + "," + report?.Query + "," + report?.QueryType + ","
                                    + report?.FollowedBack + "," + report?.ActivityType + ","
                                    + report?.InteractedUsername + ",'" + report?.InteractedUserId + "',"
                                    + report?.InteractionTime + "," + report?.FullName + ","
                                    + report?.FollowersCount + "," + report?.FollowingsCount + "," + report?.PinsCount +
                                    ","
                                    + report?.Website + "," + report.Bio?.AsCsvData() + ","
                                    + report?.HasAnonymousProfilePicture + "," + report?.ProfilePicUrl);
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