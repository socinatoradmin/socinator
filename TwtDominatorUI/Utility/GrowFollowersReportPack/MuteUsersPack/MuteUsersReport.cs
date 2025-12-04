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

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.MuteUsersPack
{
    internal class MuteUsersReport : ITDReportFactory
    {
        private static readonly ObservableCollection<InteractedUserReport> ScrapeUserReportModel =
            new ObservableCollection<InteractedUserReport>();

        private static List<InteractedUserReport> AccountsInteractedUsers = new List<InteractedUserReport>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings)
        {
            return JsonConvert.DeserializeObject<MuteModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, CampaignDetails campaignDetails)
        {
            ScrapeUserReportModel.Clear();
            // var dboperation = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);
            IDbCampaignService dboperation = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            try
            {
                ScrapeUserReportModel.Clear();
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
                            ProfilePicture = report.HasAnonymousProfilePicture == 1 ? "No" : "Yes",
                            Privacy = report.IsPrivate == 1 ? "Protected" : "Public",
                            InteractionDate = report.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                    });

                #region Generate Reports column with data

                //    reportModel.ReportCollection =
                //CollectionViewSource.GetDefaultView(ScrapeUserReportModel);

                //ScrapeUserReportModel.ToList().ForEach(x =>
                //{
                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sl No", ColumnBindingText = "SlNo"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account", ColumnBindingText = "SinAccUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Mute User Id", ColumnBindingText = "UserId"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Mute User Name", ColumnBindingText = "UserName"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Profile pic", ColumnBindingText = "ProfilePicture"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Privacy", ColumnBindingText = "Privacy"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Muted Date", ColumnBindingText = "InteractionDate"}
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(ScrapeUserReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedUsers = new List<InteractedUserReport>();

            IList reportDetails = null;
            var SNo = 0;

            dataBase.GetInteractedUsers(ActivityType.Mute)?.ForEach(
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

            #region Mute

            //case ActivityType.Mute:
            reportDetails = AccountsInteractedUsers.Select(x =>
                new
                {
                    x.SlNo,
                    x.QueryType,
                    x.QueryValue,
                    x.UserName,
                    x.UserId,
                    x.ProfilePicture,
                    x.Privacy,
                    x.Verified,
                    x.InteractionDate
                }).ToList();
            //this.CsvHeader = "Sl no,Query Type,Query Value,Muted User Name,Muted UserId,Profile Pic,Privacy,Verified,Muted Date";

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
                Header = Header =
                    "Sl No,Account,Query Type,Query Value,Muted User Id,Muted User Name,Profile pic,Privacy,Muted Date";

                ScrapeUserReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.QueryType + "," +
                                    report.QueryValue + ",'" + report.UserId + "'," + report.UserName + "," +
                                    report.ProfilePicture + "," + report.Privacy + "," + report.InteractionDate);
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
                    "Sl no,Query Type,Query Value,Muted User Name,Muted UserId,Profile Pic,Privacy,Verified,Muted Date";

                AccountsInteractedUsers.ToList().ForEach(x =>
                {
                    try
                    {
                        csvData.Add(x.SlNo + "," + x.QueryType + "," + x.QueryValue + "," + x.UserName + ",'" +
                                    x.UserId + "'," + x.ProfilePicture + "," + x.Privacy + "," + x.Verified + "," +
                                    x.InteractionDate.ToString(CultureInfo.InvariantCulture));
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