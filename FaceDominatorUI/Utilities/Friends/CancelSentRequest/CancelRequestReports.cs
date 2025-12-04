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

namespace FaceDominatorUI.Utilities.Friends.CancelSentRequest
{
    public class CancelRequestReports : IFdReportFactory
    {
        public static ObservableCollection<UnfriendReportModel> InteractedUsersModel =
            new ObservableCollection<UnfriendReportModel>();

        public static List<CancelequestReport> ListUnfriendReportAccount = new List<CancelequestReport>();


        public static List<string> Data = new List<string>();

        public static List<string> CampaignData = new List<string>();

        private readonly string _activityType = ActivityType.WithdrawSentRequest.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CancelSentRequestModel>(activitySettings).SavedQueries;
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
                    InteractedUsersModel.Add(new UnfriendReportModel
                    {
                        Id = report.Id,
                        AccountEmail = report.AccountEmail,
                        ActivityType = ActivityType.WithdrawSentRequest.ToString(),
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
                        ColumnHeaderText = "LangKeyRequestCancelledFor".FromResourceDictionary(),
                        ColumnBindingText = "UserName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserProfileUrl".FromResourceDictionary(),
                        ColumnBindingText = "UserProfileUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyRequestedDate".FromResourceDictionary(),
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
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>(x =>
                    x.ActivityType == _activityType).ToList();

            ListUnfriendReportAccount.Clear();

            ListUnfriendReportAccount = new List<CancelequestReport>();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers report in reportDetails)
            {
                ListUnfriendReportAccount.Add(
                    new CancelequestReport
                    {
                        Id = id,
                        ActivityType = ActivityType.WithdrawSentRequest.ToString(),
                        UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                        UserId = report.UserId,
                        UserName = report.Username,
                        RequestedDate = report.InteractionTimeStamp.EpochToDateTimeUtc(),
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
                //Header = "Account Email,Activity Type,User Profile Url,Request Cancelled for,Requested Date,Interaction Date";
                Header = PostsReportHeader();
                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        //       csvData.Add(PostsReportCSVData(report));
                        csvData.Add(report.AccountEmail + "," + report.ActivityType + ","
                                    + report.UserProfileUrl + ","
                                    + report.UserName + ","
                                    + report.RequestedDate + ","
                                    + report.InteractionTimeStamp);
                        //      csvData.Clear();
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
                //Header = "Activity Type,User Profile Url,Request Cancelled for,Requested Date,Interaction Date";
                Header = PostsReportHeader(false);
                ListUnfriendReportAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + ","
                                                        + $"{FdConstants.FbHomeUrl}{report.UserId}" + ","
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
            listResource.Add("LangKeyRequestCancelledFor");
            listResource.Add("LangKeyRequestedDate");
            listResource.Add("LangKeyInteractionDateTime");

            return listResource.ReportHeaderFromResourceDict();
        }

        public string PostsReportCSVData(UnfriendReportModel model, bool addAccount = true)
        {
            return string.Join(addAccount ? $"{model.AccountEmail}," : "", $"{model.ActivityType},"
                , $"{FdConstants.FbHomeUrl}{model.UserId},", $"{model.UserName},"
                , $"{model.RequestedDate},", $"{model.InteractionTimeStamp},");
        }
    }
}