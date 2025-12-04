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

namespace FaceDominatorUI.Utilities.Friends.Unfriend
{
    public class UnFriendRequestReports : IFdReportFactory
    {
        public static ObservableCollection<UnfriendReportModel> InteractedUsersModel =
            new ObservableCollection<UnfriendReportModel>();

        public static List<UnfriendReportAccountModel> ListUnfriendReportAccount =
            new List<UnfriendReportAccountModel>();


        public static List<string> Data = new List<string>();

        public static List<string> CampaignData = new List<string>();

        private readonly string _activityType = ActivityType.Unfriend.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfriendModel>(activitySettings).SavedQueries;
        }

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
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
                        InteractedUsersModel.Add(new UnfriendReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            ActivityType = ActivityType.Unfriend.ToString(),
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            RequestedDate = report.InteractionTimeStamp.EpochToDateTimeUtc(),
                            InteractionTimeStamp = report.InteractionDateTime
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
                    {
                        ColumnHeaderText =
                            $"{"LangKeyAccount".FromResourceDictionary()} {"LangKeyEmail".FromResourceDictionary()}",
                        ColumnBindingText = "AccountEmail"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUnfriendWith".FromResourceDictionary(),
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
            //});
            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }


        public IList GetsAccountReport(DbAccountService dataBase)
        {
            var id = 1;

            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>(x =>
                    x.ActivityType == _activityType).ToList();

            ListUnfriendReportAccount = new List<UnfriendReportAccountModel>();

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers report in reportDetails)
            {
                if (ListUnfriendReportAccount.FirstOrDefault(x => x.InteractionTimeStamp == report.InteractionDateTime
                                                                  && x.UserProfileUrl ==
                                                                  FdConstants.FbHomeUrl + report.UserId)
                    == null)
                    ListUnfriendReportAccount.Add(
                        new UnfriendReportAccountModel
                        {
                            Id = id,
                            ActivityType = ActivityType.Unfriend.ToString(),
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserId = report.UserId,
                            UserName = report.Username,
                            ConnectedDate = report.InteractionTimeStamp.EpochToDateTimeUtc(),
                            InteractionTimeStamp = report.InteractionDateTime
                        });

                id++;
            }

            return ListUnfriendReportAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "Account Email,Activity Type,User Profile Url,Unfriend With,Connected Date,Interaction Date";
                Header = PostsReportHeader();
                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + "," + report.ActivityType + ","
                                    + report.UserProfileUrl + ","
                                    + report.UserName + ","
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

            #region Account reports

            if (reportType == ReportType.Account)
            {
                //Header = "Activity Type,User Profile Url,Unfriend With,Connected Date,Interaction Date";
                Header = PostsReportHeader(false);
                ListUnfriendReportAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + ","
                                                        + report.UserProfileUrl + ","
                                                        + report.UserName + ","
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

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyActivityType");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyUnfriendWith");
            listResource.Add("LangKeyConnectedDate");
            listResource.Add("LangKeyInteractionDateTime");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}