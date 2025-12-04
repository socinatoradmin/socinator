using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.InviterModel;
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
using CampaignInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups;

namespace FaceDominatorUI.Utilities.Groups.MakeGroupAdmin
{
    internal class MakeGroupAdminReport : IFdReportFactory
    {
        public static ObservableCollection<GroupInviterReportModel> InterectedGroupDetailsCollection =
            new ObservableCollection<GroupInviterReportModel>();

        public static List<GroupInviterReportAccountModel> AccountsInteractedUser =
            new List<GroupInviterReportAccountModel>();

        private readonly string _activityType = ActivityType.MakeAdmin.ToString();


        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,Admin Request Sent To,Admin for Group Name,Admin for Group Url,Date";
                Header = PostsReportHeader();
                InterectedGroupDetailsCollection.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + ","
                                                        + report.UserProfileUrl + ","
                                                        + (string.IsNullOrEmpty(report.GroupName)
                                                            ? "NA"
                                                            : report.GroupName.Replace(",", " ")) + ","
                                                        + report.GroupUrl + ","
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
                //Header = "Admin Request Sent To,Admin for Group Name,Admin for Group Url,Date";
                Header = PostsReportHeader(false);
                AccountsInteractedUser.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.UserProfileUrl + ","
                                                          + report.GroupName + ","
                                                          + report.GroupUrl + ","
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

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InterectedGroupDetailsCollection.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<CampaignInteractedGroups>().ForEach(
                report =>
                {
                    InterectedGroupDetailsCollection.Add(new GroupInviterReportModel
                    {
                        Id = report.Id,
                        AccountEmail = report.AccountEmail,
                        UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                        UserId = report.UserId,
                        GroupName = report.GroupName,
                        GroupUrl = report.GroupUrl,
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
                        ColumnHeaderText = "LangKeyUserProfileUrl".FromResourceDictionary(),
                        ColumnBindingText = "UserProfileUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyAdminForGroupUrl".FromResourceDictionary(),
                        ColumnBindingText = "GroupUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyAdminForGroupName".FromResourceDictionary(),
                        ColumnBindingText = "GroupName"
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

            return new ObservableCollection<object>(InterectedGroupDetailsCollection);
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedUsers>(x => x.ActivityType == _activityType).ToList();

            AccountsInteractedUser.Clear();

            var id = 1;

            foreach (InteractedUsers report in reportDetails)
                try
                {
                    var pageDetails = JsonConvert.DeserializeObject<GroupDetails>(report.DetailedUserInfo);

                    AccountsInteractedUser.Add(
                        new GroupInviterReportAccountModel
                        {
                            Id = id,
                            ActivityType = ActivityType.GroupInviter.ToString(),
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal(),
                            GroupName = pageDetails.GroupName,
                            GroupUrl = pageDetails.GroupUrl,
                            IsInvitedWithNote = string.IsNullOrEmpty(report.QueryValue) ? "No" : "Yes",
                            Note = report.QueryValue
                        }
                    );

                    id++;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return AccountsInteractedUser;
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<GroupInviterModel>(activitySettings).SavedQueries;
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyRequestSentTo");
            listResource.Add("LangKeyGroupName");
            listResource.Add("LangKeyGroupUrl");
            listResource.Add("LangKeyInteractionDateTime");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}