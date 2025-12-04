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
using Newtonsoft.Json;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Reports;
using QuoraDominatorCore.Reports.AccountConfigReport;

namespace QuoraDominatorUI.Utility.Report.GrowFollower
{
    public class FollowReports : IQdReportFactory
    {
        private static readonly ObservableCollection<FollowerReport> FollowerReportModel =
            new ObservableCollection<FollowerReport>();

        private static readonly ObservableCollection<InteractedUser> LstfollowedUser =
            new ObservableCollection<InteractedUser>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            FollowerReportModel.Clear();
            try
            {
                #region get data from InteractedUsers table and add to FollowerReportModel
                var activity = ActivityType.Follow.ToString();
                var campaignId = string.IsNullOrEmpty(reportModel.CampaignId) ? campaignDetails.CampaignId : reportModel.CampaignId;
                var dbCampaignService = new DbCampaignService(campaignId);
                dbCampaignService.GetAllInteractedUsers()?.Where(x => x.ActivityType == activity).ForEach(
                //dbCampaignService.GetAllInteractedUsers()?.ForEach(
                    report =>
                    {
                         //f (!FollowerReportModel.Any(x => x.Id == report.Id))
                        FollowerReportModel.Add(new FollowerReport
                        {
                            Id = report.Id,
                            AccountName = report.SinAccUsername,
                            Date = report.InteractionDateTime,
                            QueryType = report.QueryType,
                            Query = report.QueryValue,
                            Username = report.InteractedUsername,
                            FollowedBack = report.FollowBackStatus
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
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Follower", ColumnBindingText = "Username"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(FollowerReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            LstfollowedUser.Clear();

            try
            {
                IList reportDetails = dbAccountService.GetInteractedUsers(ActivityType.Follow).ToList();

                foreach (InteractedUsers item in reportDetails)
                    LstfollowedUser.Add(
                        new InteractedUser
                        {
                            Id = item.Id,
                            InteractedUsername = item.InteractedUsername,
                            Date = item.Date.EpochToDateTimeLocal(),
                            QueryType = item.QueryType,
                            Query = item.Query,
                            FollowedBack = item.FollowedBack
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstfollowedUser;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName,QueryType,Query,Follower,Followed Back,Date";
                    FollowerReportModel.ForEach(report =>
                    {
                        csvData.Add(report.AccountName + "," + report.QueryType + "," + report.Query + "," +
                                    report.Username + "," + report.FollowedBack + "," + report.Date);
                    });
                }
                else
                {
                    Header = "Id,QueryType,Query,Follower,Followed Back,Date";
                    LstfollowedUser.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryType + "," + report.Query + "," +
                                    report.InteractedUsername + "," + report.FollowedBack + "," + report.Date);
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