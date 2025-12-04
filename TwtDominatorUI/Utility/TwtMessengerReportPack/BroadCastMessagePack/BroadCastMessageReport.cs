using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using TwtDominatorCore.Interface;
using TwtDominatorCore.Report;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.BroadCastMessagePack
{
    internal class BroadCastMessageReport : ITDReportFactory
    {
        private static readonly ObservableCollection<InteractedUserReport> ScrapeUserReportModel =
            new ObservableCollection<InteractedUserReport>();

        private static List<InteractedUserReport> AccountsInteractedUsers = new List<InteractedUserReport>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings)
        {
            return JsonConvert.DeserializeObject<MessageModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel ObjReports, CampaignDetails campaignDetails)
        {
            ScrapeUserReportModel.Clear();
            // var dboperation = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);
            IDbCampaignService dboperation = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            try
            {
                // List<InteractedUsers> tstPost = dboperation.GetAllInteractedUsers();
                dboperation.GetAllInteractedUsers().ForEach(
                    report =>
                    {
                        ScrapeUserReportModel.Add(new InteractedUserReport
                        {
                            SlNo = report.Id,
                            SinAccUsername = report.SinAccUsername,
                            QueryType = report.QueryType,
                            UserName = report.InteractedUsername,
                            UserId = report.InteractedUserId,
                            MessageText = report.DirectMessage,
                            MediaPath = report.MediaPath,
                            InteractionDate = report.InteractionDateTime
                        });
                    });
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region Generate Reports column with data

            //     ObjReports.ReportCollection =
            //CollectionViewSource.GetDefaultView(ScrapeUserReportModel);

            ObjReports.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sl No", ColumnBindingText = "SlNo"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Name", ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Message Text", ColumnBindingText = "MessageText"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Media Path", ColumnBindingText = "MediaPath"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"}
                };
            return new ObservableCollection<object>(ScrapeUserReportModel);

            #endregion

            // return ScrapeUserReportModel.Count;
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = null;
            var SNo = 0;
            AccountsInteractedUsers = new List<InteractedUserReport>();

            dataBase.GetInteractedUsers(ActivityType.BroadcastMessages)?.ForEach(
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
                        MessageText = report.DirectMessage,
                        MediaPath = report.MediaPath
                    });
                });

            #region Broadcast Message

            //  case ActivityType.BroadcastMessages:
            reportDetails = AccountsInteractedUsers.Select(x =>
                new
                {
                    x.SlNo,
                    x.UserName,
                    x.UserId,
                    x.MessageText,
                    x.MediaPath,
                    x.InteractionDate
                }).ToList();
            //this.CsvHeader = "Sl no,Query Type,User Name,UserId,Message Text,Sending Date";

            //break;

            #endregion

            return reportDetails;
        }

        public void ExportReports(string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Sl No,Account,Query Type,User ID,User Name,Message Text,Media Path,Message Date";

                ScrapeUserReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.QueryType + ",'" +
                                    report.UserId + "'," + report.UserName + ",\"" +
                                    report.MessageText.Replace("\"", "\"\"").Replace("\r\n", " ") + "\"," +
                                    report.MediaPath + "," + report.InteractionDate);
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
                Header = "Sl No,Query Type,User ID,User Name,Message Text,Media Path,Message Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SlNo + "," + report.QueryType + ",'" + report.UserId + "'," +
                                    report.UserName + ",\"" +
                                    report.MessageText.Replace("\"", "\"\"").Replace("\r\n", " ") + "\"," +
                                    report.MediaPath + "," + report.InteractionDate);
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