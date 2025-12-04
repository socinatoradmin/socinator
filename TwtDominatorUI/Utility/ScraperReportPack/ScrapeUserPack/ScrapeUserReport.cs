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

namespace TwtDominatorUI.Utility.ScraperReportPack.ScrapeUserPack
{
    internal class ScrapeUserReport : ITDReportFactory
    {
        private static readonly ObservableCollection<InteractedUserReport> ScrapeUserReportModel =
            new ObservableCollection<InteractedUserReport>();

        private static List<InteractedUserReport> AccountsInteractedUsers = new List<InteractedUserReport>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel ObjReports, CampaignDetails campaignDetails)
        {
            ScrapeUserReportModel.Clear();
            IDbCampaignService dboperation = new DbCampaignService(campaignDetails.CampaignId);
            // var dboperation = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);

            #region get data from InteractedUsers table and add to FollowerReportModel

            try
            {
                dboperation.GetAllInteractedUsers().ForEach(
                    report =>
                    {
                        ScrapeUserReportModel.Add(new InteractedUserReport
                        {
                            SlNo = report.Id,
                            SinAccUsername = report.SinAccUsername,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            UserId = report.InteractedUserId,
                            UserName = report.InteractedUsername,
                            UserFullName = report.InteractedUserFullName,
                            FollowersCount = report.FollowersCount,
                            FollowingsCount = report.FollowingsCount,
                            LikesCount = report.LikesCount,
                            TweetsCount = report.TweetsCount,
                            ProfilePicture = report.HasAnonymousProfilePicture == 1 ? "No" : "Yes",
                            FollowStatus = report.FollowStatus == 1 ? "Yes" : "No",
                            FollowBackStatus = report.FollowBackStatus == 1 ? "Yes" : "No",
                            Bio = string.IsNullOrEmpty(report.Bio) ? " NA " : report.Bio,
                            Privacy = report.IsPrivate == 1 ? "Protected" : "Public",
                            JoinedDate = report.JoinedDate.ToString(CultureInfo.InvariantCulture),
                            Location = string.IsNullOrEmpty(report.Location) ? " NA " : report.Location,
                            ProfilePicUrl = string.IsNullOrEmpty(report.ProfilePicUrl) ? " NA " : report.ProfilePicUrl,
                            Website = string.IsNullOrEmpty(report.Website) ? " NA " : report.Website,
                            InteractionDate = report.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

            #endregion

            #region Generate Reports column with data

            //     ObjReports.ReportCollection =
            //CollectionViewSource.GetDefaultView(ScrapeUserReportModel);


            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            ObjReports.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sl No", ColumnBindingText = "SlNo"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Scraped User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped User Name", ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped User FullName", ColumnBindingText = "UserFullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Joined on Twitter", ColumnBindingText = "JoinedDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followers Count", ColumnBindingText = "FollowersCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followings Count", ColumnBindingText = "FollowingsCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Tweets Count", ColumnBindingText = "TweetsCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Likes Count", ColumnBindingText = "LikesCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Profile pic", ColumnBindingText = "ProfilePicture"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Privacy", ColumnBindingText = "Privacy"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Bio", ColumnBindingText = "Bio"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Location", ColumnBindingText = "Location"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Profile Pic Link", ColumnBindingText = "ProfilePicUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Website", ColumnBindingText = "Website"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Following", ColumnBindingText = "FollowStatus"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Following back", ColumnBindingText = "FollowBackStatus"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped Date", ColumnBindingText = "InteractionDate"}
                };
            //   });
            return new ObservableCollection<object>(ScrapeUserReportModel);

            #endregion

            //return ScrapeUserReportModel.Count;
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = null;
            var SNo = 0;
            AccountsInteractedUsers = new List<InteractedUserReport>();
            dataBase.GetInteractedUsers(ActivityType.UserScraper)?.ForEach(
                report =>
                {
                    AccountsInteractedUsers.Add(new InteractedUserReport
                    {
                        SlNo = ++SNo,
                        SinAccUsername = report.SinAccUsername,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        UserName = report.InteractedUsername,
                        UserId = report.InteractedUserId,
                        UserFullName = report.InteractedUserFullName,
                        FollowersCount = report.FollowersCount,
                        FollowingsCount = report.FollowingsCount,
                        LikesCount = report.LikesCount,
                        TweetsCount = report.TweetsCount,
                        ProfilePicture = report.HasAnonymousProfilePicture == 1 ? "No" : "Yes",
                        FollowStatus = report.FollowStatus == 1 ? "Yes" : "No",
                        FollowBackStatus = report.FollowBackStatus == 1 ? "Yes" : "No",
                        Bio = string.IsNullOrEmpty(report.Bio) ? " NA " : report.Bio,
                        Privacy = report.IsPrivate == 1 ? "Protected" : "Public",
                        Verified = report.IsVerified == 1 ? "yes" : "No",
                        JoinedDate = report.JoinedDate.ToString(CultureInfo.InvariantCulture),
                        Location = string.IsNullOrEmpty(report.Location) ? " NA " : report.Location,
                        ProfilePicUrl = string.IsNullOrEmpty(report.ProfilePicUrl) ? " NA " : report.ProfilePicUrl,
                        Website = string.IsNullOrEmpty(report.Website) ? " NA " : report.Website,
                        InteractionDate = report.InteractionDateTime,
                        ProcessType = report.ProcessType,
                        MessageText = report.DirectMessage
                    });
                });

            #region UserSCraper

            // case ActivityType.UserScraper:
            reportDetails = AccountsInteractedUsers.Select(x =>
                new
                {
                    x.SlNo,
                    x.QueryType,
                    x.QueryValue,
                    x.UserName,
                    x.UserId,
                    x.UserFullName,
                    x.FollowersCount,
                    x.FollowingsCount,
                    x.LikesCount,
                    x.TweetsCount,
                    x.ProfilePicture,
                    x.Privacy,
                    x.Verified,
                    x.Bio,
                    x.JoinedDate,
                    x.Location,
                    x.ProfilePicUrl,
                    x.Website,
                    x.FollowStatus,
                    x.FollowBackStatus,
                    x.InteractionDate
                }).ToList();

            //this.CsvHeader = "Sl No,Query Type, Query Value,User Name, UserId,User Full Name,Followers Count,Followings Count,Likes Count,Tweets Count,Profile Pic,Follow Status,Follow Back Status,Bio,Privacy,Verified,Joined Date,Location,Profile Pic Url,Website,Scrape Date";
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
                    "Sl No,Account,Query Type,Query Value,Scraped User Id,Scraped User Name,Scraped User FullName,Joined on Twitter,Followers Count,Followings Count,Tweets Count,Likes Count,Profile pic,Privacy,User Bio,Location,Profile Pic Link,Website,Following,Following back,Scraped Date";

                ScrapeUserReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        var Bio = report.Bio.Replace("\n", " ").Replace(",", ".");
                        var Location = report.Location.Replace("\n", " ").Replace(",", ".");
                        csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.QueryType + "," +
                                    report.QueryValue + "," + report.UserId + "," + report.UserName + "," +
                                    report.UserFullName + "," + report.JoinedDate + "," + report.FollowersCount + "," +
                                    report.FollowingsCount + "," + report.TweetsCount + "," + report.LikesCount + "," +
                                    report.ProfilePicture + "," + report.Privacy + "," + Bio + "," + Location + "," +
                                    report.ProfilePicUrl + "," + report.Website + "," + report.FollowStatus + "," +
                                    report.FollowBackStatus + "," + report.InteractionDate);
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
                    "Sl No,Query Type,Query Value,Scraped User Id,Scraped User Name,Scraped User FullName,Joined on Twitter,Followers Count,Followings Count,Tweets Count,Likes Count,Profile pic,Privacy,User Bio,Location,Profile Pic Link,Website,Following,Following back,Scraped Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        var Bio = report.Bio.Replace("\n", " ").Replace(",", ".");
                        var Location = report.Location.Replace("\n", " ").Replace(",", ".");
                        csvData.Add(report.SlNo + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.UserId + "," + report.UserName + "," + report.UserFullName + "," +
                                    report.JoinedDate + "," + report.FollowersCount + "," + report.FollowingsCount +
                                    "," + report.TweetsCount + "," + report.LikesCount + "," + report.ProfilePicture +
                                    "," + report.Privacy + "," + Bio + "," + Location + "," + report.ProfilePicUrl +
                                    "," + report.Website + "," + report.FollowStatus + "," + report.FollowBackStatus +
                                    "," + report.InteractionDate);
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