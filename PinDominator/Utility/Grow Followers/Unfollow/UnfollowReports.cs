using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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

namespace PinDominator.Utility.Grow_Followers.Unfollow
{
    public class UnfollowReports : IPdReportFactory
    {
        public static ObservableCollection<UnfollowedUsersReportDetails> UnfollowedUsersReportModel =
            new ObservableCollection<UnfollowedUsersReportDetails>();

        private static readonly List<UnfollowedUsersReportDetails> LstUnfollowedUser =
            new List<UnfollowedUsersReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            UnfollowedUsersReportModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            var id = 1;
            dbCampaignService.Get<UnfollowedUsers>().ForEach(
                report =>
                {
                    UnfollowedUsersReportModel.Add(new UnfollowedUsersReportDetails
                    {
                        Id = id++,
                        FollowedBack = report.FollowedBack,
                        Username = report.Username,
                        UserId = report.UserId,
                        InteractionDate =
                            (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        FullName = report.FullName,
                        OperationType = report.OperationType,
                        SinAccId = report.SinAccId,
                        SinAccUsername = report.SinAccUsername
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
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Followed Back", ColumnBindingText = "FollowedBack"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Full Name", ColumnBindingText = "FullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sin Account Id", ColumnBindingText = "SinAccId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Sin Account Username", ColumnBindingText = "SinAccUsername"}
                };
            //});

            #endregion

            return new ObservableCollection<object>(UnfollowedUsersReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstUnfollowedUser.Clear();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.UnfollowedUsers>()
                .Where(x => x.OperationType == ActivityType.Unfollow).ToList();
            var id = 1;
            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.UnfollowedUsers item in reportDetails)
                LstUnfollowedUser.Add(
                    new UnfollowedUsersReportDetails
                    {
                        Id = id++,
                        FollowedBack = item.FollowedBack,
                        InteractionDate =
                            (item.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        OperationType = item.OperationType,
                        Username = item.Username,
                        UserId = item.UserId,
                        FullName = item.FullName
                    }
                );

            return LstUnfollowedUser.Select(x =>
                new
                {
                    x.Id,
                    x.FollowedBack,
                    x.InteractionDate,
                    x.OperationType,
                    x.Username,
                    x.UserId,
                    x.FullName
                }).ToList();
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header = "Id,Followed Back,Interaction Date,Operation Type,Username,User Id,Full Name";

                LstUnfollowedUser.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id + "," + report.FollowedBack + "," + report.InteractionDate + "," +
                                    report.OperationType.ToString()
                                    + "," + report.Username + "," + report.UserId + "," + report.FullName);
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
                    "Id,Followed Back,Interaction Date,Operation Type,Username,User Id,Full Name,Sin Account Id,Sin Account Username";

                UnfollowedUsersReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Id + "," + report.FollowedBack + "," + report.InteractionDate + "," +
                                    report.OperationType.ToString()
                                    + "," + report.Username + "," + report.UserId + "," + report.FullName + "," +
                                    report.SinAccId + "," + report.SinAccUsername);
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