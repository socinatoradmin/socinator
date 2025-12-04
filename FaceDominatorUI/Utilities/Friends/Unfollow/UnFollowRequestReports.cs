using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FdReports;
using FaceDominatorCore.Interface;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.Utilities.Friends.Unfollow
{
    public class UnFollowRequestReports : IFdReportFactory
    {
        public static ObservableCollection<UnfollowReportModel> InteractedUsersModel =
            new ObservableCollection<UnfollowReportModel>();

        public static List<UnfollowReportAccountModel> ListUnfollowReportAccount =
            new List<UnfollowReportAccountModel>();

        public static List<string> Data = new List<string>();
        public static List<string> CampaignData = new List<string>();
        private readonly string _activityType = ActivityType.Unfollow.ToString();

        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();
            #region Campaign Reports
            if (reportType == ReportType.Campaign)
            {
                Header = PostsReportHeader();
                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + "," + report.ActivityType + ","
                                    + report.UserProfileUrl + ","
                                    + (string.IsNullOrEmpty(report.UserName) ? "N/A" : report.UserName) + ","
                                    + report.RequestedDate + ","
                                    + report.InteractionTimeStamp);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            #endregion
            #region Account Reports
            if (reportType == ReportType.Account)
            {
                Header = PostsReportHeader(false);
                ListUnfollowReportAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + ","
                                                        + report.UserProfileUrl + ","
                                                        + (string.IsNullOrEmpty(report.UserName) ? "N/A" : report.UserName) + ","
                                                        + report.RequestedDate + ","
                                                        + report.InteractionTimeStamp);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            #endregion
            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            InteractedUsersModel.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModel
            dataBase.GetAllInteractedData<InteractedUsers>().ForEach(
                report =>
                {
                    if (InteractedUsersModel.FirstOrDefault(x => x.InteractionTimeStamp == report.InteractionDateTime
                                                                 && x.AccountEmail == report.AccountEmail &&
                                                                 x.UserProfileUrl == FdConstants.FbHomeUrl +
                                                                 report.UserId)
                        == null)
                        InteractedUsersModel.Add(new UnfollowReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            ActivityType = ActivityType.Unfollow.ToString(),
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = string.IsNullOrEmpty(report.Username) ? "N/A" : report.Username,
                            RequestedDate = report.InteractionTimeStamp.EpochToDateTimeUtc(),
                            InteractionTimeStamp = report.InteractionDateTime
                        });
                });
            #endregion
            #region Generate Reports Column with Data
            reportModel.GridViewColumn =
                new ObservableCollection<DominatorHouseCore.Utility.GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = $"{"LangKeyAccount".FromResourceDictionary()}{"LangKeyEmail".FromResourceDictionary()}",
                        ColumnBindingText = "AccountEmail"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUnfollowWith".FromResourceDictionary(),
                        ColumnBindingText = "UserName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserProfileUrl".FromResourceDictionary(),
                        ColumnBindingText = "UserProfileUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyConnectedDate".FromResourceDictionary(),
                        ColumnBindingText = "RequestedDate"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            var id = 1;

            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>(x =>
                    x.ActivityType == _activityType).ToList();

            ListUnfollowReportAccount = new List<UnfollowReportAccountModel>();

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers report in reportDetails)
            {
                if (ListUnfollowReportAccount.FirstOrDefault(x => x.InteractionTimeStamp == report.InteractionDateTime
                                                                  && x.UserProfileUrl ==
                                                                  FdConstants.FbHomeUrl + report.UserId)
                    == null)
                    ListUnfollowReportAccount.Add(
                        new UnfollowReportAccountModel
                        {
                            Id = id,
                            ActivityType = ActivityType.Unfollow.ToString(),
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserId = report.UserId,
                            UserName = string.IsNullOrEmpty(report.Username) ? "N/A" : report.Username,
                            ConnectedDate = report.InteractionTimeStamp.EpochToDateTimeUtc(),
                            InteractionTimeStamp = report.InteractionDateTime
                        });

                id++;
            }

            return ListUnfollowReportAccount;
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowFriendModel>(activitySettings).SavedQueries;
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyActivityType");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyUnfollowWith");
            listResource.Add("LangKeyConnectedDate");
            listResource.Add("LangKeyInteractionDateTime");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}
