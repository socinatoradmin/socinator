using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.FollowerPack
{
    internal class FollowerReport : ITDReportFactory
    {
        private static readonly ObservableCollection<InteractedUserReport> ScrapeUserReportModel =
            new ObservableCollection<InteractedUserReport>();

        private static List<InteractedUserReport> AccountsInteractedUsers = new List<InteractedUserReport>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings)
        {
            return JsonConvert.DeserializeObject<FollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, CampaignDetails campaignDetails)
        {
            ScrapeUserReportModel.Clear();
            // var getModuleSetting = DominatorHouseCore.FileManagers.AccountsFileManager.GetAccountById(AccountId);//.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == currentDataContext.Title);
            // var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);
            IDbCampaignService dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedUsers().ForEach(
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
                        FollowBackStatus = report.FollowBackStatus == 1 ? "Yes" : "No",
                        Privacy = report.IsPrivate == 1 ? "Protected" : "Public",
                        InteractionDate = report.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                    });
                });

            #endregion

            #region Generate Reports column with data

            //ScrapeUserReportModel.ToList().ForEach(x =>
            //{
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sl No", ColumnBindingText = "SlNo"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Followed User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followed User Name", ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Profile pic", ColumnBindingText = "ProfilePicture"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Privacy", ColumnBindingText = "Privacy"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Following back", ColumnBindingText = "FollowBackStatus"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followed Date", ColumnBindingText = "InteractionDate"}
                };
            //  });

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(ScrapeUserReportModel);

            #endregion

            return new ObservableCollection<object>(ScrapeUserReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedUsers = new List<InteractedUserReport>();

            IList reportDetails = null;
            var SNo = 0;

            try
            {
                dataBase.GetInteractedUsers(ActivityType.Follow).Where(x => x.QueryType != null)?.ForEach(
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
                            //    Verified = report.IsVerified == 1 ? "yes" : "No",
                            JoinedDate = report.JoinedDate.ToString(),
                            Location = string.IsNullOrEmpty(report.Location) ? " NA " : report.Location,
                            ProfilePicUrl = string.IsNullOrEmpty(report.ProfilePicUrl) ? " NA " : report.ProfilePicUrl,
                            Website = string.IsNullOrEmpty(report.Website) ? " NA " : report.Website,
                            InteractionDate = report.InteractionDateTime
                            // ProcessType = report.ProcessType,
                            // MessageText = report.DirectMessage
                        });
                    });

                #region Follow

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
                        //  x.Verified,
                        x.FollowBackStatus,
                        // x.ProcessType,
                        x.InteractionDate
                    }).ToList();

                // string  CsvHeader = "Sl no,Query Type,Query Value,Followed User Name,Followed UserId,Profile Pic,Privacy,Verified,Follow Back Status,Process Type,Followed Date";

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return reportDetails;
        }

        public void ExportReports(string fileName, ReportType reportType)
        {
            try
            {
                var csvData = new List<string>();

                #region Campaign reports

                if (reportType == ReportType.Campaign)
                {
                    Header =
                        "Sl No,Account,Query Type,Query Value,Followed User Id,Followed User Name,Profile pic,Privacy,Following back,Followed Date";

                    ScrapeUserReportModel.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.QueryType + "," +
                                        report.QueryValue + ",'" + report.UserId + "'," + report.UserName + "," +
                                        report.ProfilePicture + "," + report.Privacy + "," + report.FollowBackStatus +
                                        "," + report.InteractionDate);
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
                        "Sl no,Query Type,Query Value,Followed User Name,Followed UserId,Profile Pic,Privacy,Follow Back Status,Followed Date";

                    AccountsInteractedUsers.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.SlNo + "," + report.QueryType + "," + report.QueryValue + "," +
                                        report.UserName + ",'" + report.UserId + "'," + report.ProfilePicture + "," +
                                        report.Privacy + "," + report.FollowBackStatus + "," + report.InteractionDate);
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}