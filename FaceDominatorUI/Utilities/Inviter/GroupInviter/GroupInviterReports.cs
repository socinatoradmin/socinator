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

namespace FaceDominatorUI.Utilities.Inviter.GroupInviter
{
    public class GroupInviterReports : IFdReportFactory
    {
        public static ObservableCollection<GroupInviterReportModel> InteractedUsersModel =
            new ObservableCollection<GroupInviterReportModel>();

        public static List<GroupInviterReportAccountModel> AccountsInteractedUser =
            new List<GroupInviterReportAccountModel>();

        private readonly string _activityType = ActivityType.GroupInviter.ToString();


        public string Header { get; set; } = string.Empty;


        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<GroupInviterModel>(activitySettings).SavedQueries;
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

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,User Profile Url,Invitation Sent To,Invitation for Group Name,Invitation for Group Url,Invited In Mesenger,Invited  With Note,Note,Date";
                Header = PostsReportHeader();
                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + ","
                                                        + report.UserProfileUrl + ","
                                                        + report.UserName + ","
                                                        + (string.IsNullOrEmpty(report.GroupName)
                                                            ? "NA"
                                                            : report.GroupName.Replace(",", " ")) + ","
                                                        + report.GroupUrl + ","
                                                        + report.IsInvitedWithNote + ","
                                                        + (string.IsNullOrEmpty(report.Note)
                                                            ? "NA"
                                                            : report.Note.Replace(",", string.Empty)
                                                                .Replace("\r\n", " ").Replace("\n", " ")) + ","
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
                //Header = "User Profile Url,Invitation Sent To,Invitation for Group Name,Invitation for Group Url,Invited In Mesenger,Invited  With Note,Note,Date";
                Header = PostsReportHeader(false);
                AccountsInteractedUser.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.UserProfileUrl + ","
                                                          + report.UserName + ","
                                                          + report.GroupName + ","
                                                          + report.GroupUrl + ","
                                                          + report.IsInvitedWithNote + ","
                                                          + (string.IsNullOrEmpty(report.Note)
                                                              ? "NA"
                                                              : report.Note.Replace(",", string.Empty)
                                                                  .Replace("\r\n", " ").Replace("\n", " ")) + ","
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

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedUsersModel.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>()
                .ForEach(
                    report =>
                    {
                        var pageDetails = JsonConvert.DeserializeObject<GroupDetails>(report.DetailedUserInfo);

                        InteractedUsersModel.Add(new GroupInviterReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            GroupName = pageDetails.GroupName,
                            GroupUrl = pageDetails.GroupUrl,
                            IsInvitedWithNote = string.IsNullOrEmpty(report.QueryValue) ? "No" : "Yes",
                            Note = report.QueryValue,
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
                        ColumnHeaderText = "LangKeyInvitationSentTo".FromResourceDictionary(),
                        ColumnBindingText = "UserName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyGroupName".FromResourceDictionary(), ColumnBindingText = "GroupName"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyGroupUrl".FromResourceDictionary(), ColumnBindingText = "GroupUrl"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangeKeyInvitedWithNote".FromResourceDictionary(),
                        ColumnBindingText = "IsInvitedWithNote"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyNote".FromResourceDictionary(), ColumnBindingText = "Note"},
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

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyInvitationSentTo");
            listResource.Add("LangKeyGroupName");
            listResource.Add("LangKeyGroupUrl");
            listResource.Add("LangeKeyInvitedWithNote");
            listResource.Add("LangKeyNotes");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}