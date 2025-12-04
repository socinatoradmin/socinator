using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Reports;
using QuoraDominatorCore.Reports.AccountConfigReport;

namespace QuoraDominatorUI.Utility.Report.GrowFollower
{
    public class UnfollowReports : IQdReportFactory
    {
        private static readonly ObservableCollection<UnfollowReport> UnfollowerReportModel =
            new ObservableCollection<UnfollowReport>();

        private static readonly ObservableCollection<UnfollowedUser> UnfollowerAccountModel =
            new ObservableCollection<UnfollowedUser>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return new ObservableCollection<QueryInfo>();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            UnfollowerReportModel.Clear();
            try
            {
                #region get data from UnfollowedUsers table and add to UnfollowerReportModel

                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetAllUnfollowedUsers()?.ForEach(
                    report =>
                    {
                        UnfollowerReportModel.Add(new UnfollowReport
                        {
                            Id = report.Id,
                            Date = report.InteractionDate.EpochToDateTimeUtc(),
                            Username = report.Username,
                            FollowedBack = report.FollowedBack,
                            AccountName = report.FullName
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Id", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "AccountName", ColumnBindingText = "AccountName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "FollowedBack", ColumnBindingText = "FollowedBack"}
                    };

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(UnfollowerReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            UnfollowerAccountModel.Clear();

            try
            {
                // IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.QdTables.Accounts.UnfollowedUsers>();
                IList reportDetails = dbAccountService.GetUnfollowedUser().ToList();
                foreach (UnfollowedUsers item in reportDetails)
                    UnfollowerAccountModel.Add(
                        new UnfollowedUser
                        {
                            Id = item.Id,
                            //FilterArgument = item.FilterArgument,
                            //FilterTypeSql = item.FilterTypeSql,
                            FollowedBack = item.FollowedBack,
                            //FollowedBackDate = DateTimeUtilities.EpochToDateTimeUtc(item.FollowedBackDate),
                            InteractionDate = item.InteractionDate.EpochToDateTimeUtc(),
                            Username = item.UnfollowedUsername
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return UnfollowerAccountModel;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName, Unfollower, Date, Is Followed Back";
                    UnfollowerReportModel.ForEach(report =>
                    {
                        csvData.Add(report.AccountName + "," + report.Username + "," + report.Date + "," +
                                    report.FollowedBack);
                    });
                }
                else
                {
                    Header = "Id, Unfollower, Date, Is Followed Back";

                    UnfollowerAccountModel.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.Username + "," + report.InteractionDate + "," +
                                    report.FollowedBack);
                    });
                }

                Utilities.ExportReports(fileName, Header, csvData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}