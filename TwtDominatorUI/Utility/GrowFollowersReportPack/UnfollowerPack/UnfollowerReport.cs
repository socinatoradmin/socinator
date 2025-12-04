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

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.UnfollowerPack
{
    internal class UnfollowerReport : ITDReportFactory
    {
        private static readonly ObservableCollection<UnfollowReport> ScrapeUserReportModel =
            new ObservableCollection<UnfollowReport>();

        private static List<UnfollowReport> AccountsInteractedUsers = new List<UnfollowReport>();
        public string Header { get; set; } = string.Empty;


        public ObservableCollection<object> GetCampaignsReport(ReportModel ObjReports, CampaignDetails campaignDetails)
        {
            ScrapeUserReportModel.Clear();

            // var dboperation = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks, ConstantVariable.GetCampaignDb);

            IDbCampaignService dboperation = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            try
            {
                dboperation.GetAllUnfollowedUsers().ForEach(
                    report =>
                    {
                        ScrapeUserReportModel.Add(new UnfollowReport
                        {
                            SlNo = report.Id,
                            SinAccUsername = report.SinAccUsername,
                            UserId = report.UserId,
                            UserName = report.Username,
                            SourceType = report.SourceType,
                            UnfollowSource = report.UnfollowSource,
                            FollowBackStatus = report.FollowBackStatus == 1 ? "Yes" : "No",
                            UnfollowedDate = report.InteractionDate
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            #region Generate Reports column with data

            //ObjReports.ReportCollection = CollectionViewSource.GetDefaultView(ScrapeUserReportModel);


            ObjReports.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sl No", ColumnBindingText = "SlNo"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Unfollow Source", ColumnBindingText = "UnfollowSource"},
                    // new GridViewColumnDescriptor { ColumnHeaderText = "Source Type", ColumnBindingText ="SourceType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Unfollowed UserName", ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Unfollowed UserID", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Unfollowed Date", ColumnBindingText = "UnfollowedDate"}
                };
            return new ObservableCollection<object>(ScrapeUserReportModel);

            #endregion
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = null;
            var SNo = 0;
            AccountsInteractedUsers = new List<UnfollowReport>();

            dataBase.GetUnfollowedUsers()?.ForEach(
                report =>
                {
                    AccountsInteractedUsers.Add(new UnfollowReport
                    {
                        SlNo = ++SNo,
                        SourceType = report.SourceType,
                        UnfollowSource = "  " + report.UnfollowSource + "  ",
                        UserId = report.UserId,
                        UserName = report.Username,
                        UnfollowedDate = report.InteractionDate
                    });
                });

            #region Unfollow

            reportDetails = AccountsInteractedUsers.Select(x =>
                new
                {
                    x.SlNo,
                    x.UnfollowSource,
                    //  x.SourceType,
                    x.UserName,
                    x.UserId,
                    x.UnfollowedDate
                }).ToList();

            #endregion

            return reportDetails;
        }

        public void ExportReports(string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Sl No,Account,Unfollow Source,Unfollowed UserName,Unfollowed UserID,Unfollowed Date";

                ScrapeUserReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SlNo + "," + report.SinAccUsername + "," + report.UnfollowSource + "," +
                                    report.UserName + ",'" + report.UserId + "'," + report.UnfollowedDate.ToString());
                        ;
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
                Header = "Sl no,Unfollow Source,User Name,User Id,Unfollowed Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SlNo + "," + report.UnfollowSource + "," + report.UserName + ",'" +
                                    report.UserId + "'," + report.UnfollowedDate);
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

        public ObservableCollection<QueryInfo> GetSavedQuery(string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowerModel>(activitySettings).SavedQueries;
        }
    }
}