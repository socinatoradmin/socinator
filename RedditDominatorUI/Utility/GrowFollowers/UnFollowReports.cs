using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.ReportModel;
using RedditDominatorCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace RedditDominatorUI.Utility.GrowFollowers
{
    public class UnFollowReports : IRdReportFactory
    {
        public static ObservableCollection<UnfollowedUsersReportModel> UnfollowReportModelCampaign =
            new ObservableCollection<UnfollowedUsersReportModel>();

        private static List<UnfollowedUsersReportModel> UnfollowReportModelAccount { get; } =
            new List<UnfollowedUsersReportModel>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return new ObservableCollection<QueryInfo>();
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header = "Account, Interaction Date, Unfollowed Username, Unfollowed UserID, Unfollowed FullName";

                    UnfollowReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.SinAccUsername + "," + report.InteractionDateTime + "," + report.Username +
                                        ","
                                        + report.UserId + ","
                                        + report.FullName
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header = "Interaction Date, Unfollowed Username, Unfollowed UserID, Unfollowed FullName";

                    UnfollowReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.InteractionDateTime + "," + report.Username + ","
                                        + report.UserId + ","
                                        + report.FullName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reportType), reportType, null);
            }

            #endregion

            #region Account reports

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            var columnId = 1;
            UnfollowReportModelAccount.Clear();
            IList reportDetails = dbAccountService.GetUnfollowedUsers().ToList();
            foreach (UnfollowedUsers report in reportDetails)
                UnfollowReportModelAccount.Add(new UnfollowedUsersReportModel
                {
                    Id = columnId++,
                    InteractionDateTime = DateTimeUtilities.EpochToDateTimeUtc(report.InteractionDate),

                    Username = report.Username,
                    UserId = report.UserId,
                    FullName = report.FullName
                });
            return UnfollowReportModelAccount.Select(x => new
            {
                x.Id,
                x.InteractionDateTime,
                x.Username,
                x.UserId,
                x.FullName
            }).ToList();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                ConstantVariable.GetCampaignDb);
            UnfollowReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to LikeReportModel

            try
            {
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.UnfollowedUsers>()?.ForEach(
                    report =>
                    {
                        UnfollowReportModelCampaign.Add(new UnfollowedUsersReportModel
                        {
                            Id = report.Id,
                            SinAccUsername = report.SinAccUsername,
                            InteractionDateTime = DateTimeUtilities.EpochToDateTimeLocal(report.InteractionDate),
                            Username = report.Username,
                            UserId = report.UserId,
                            FullName = report.FullName
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Name", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Date", ColumnBindingText = "InteractionDateTime"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Unfollowed User", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Unfollowed UserId", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Full Name", ColumnBindingText = "FullName"}
                };

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(UnfollowReportModelCampaign);
            return new ObservableCollection<object>(UnfollowReportModelCampaign);
        }
    }
}