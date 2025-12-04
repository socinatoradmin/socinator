using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDModel.ScraperModel;
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

namespace FaceDominatorUI.Utilities.Scraper.GroupScraper
{
    internal class GroupScraperReports : IFdReportFactory
    {
        public static ObservableCollection<GroupReportModel> InteractedGroupModel =
            new ObservableCollection<GroupReportModel>();

        public static List<GroupReportAccountModel> AccountsInteractedGroups = new List<GroupReportAccountModel>();

        private readonly string _activityType = ActivityType.GroupScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<GroupScraperModel>(activitySettings).SavedQueries;
        }


        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedGroups>(x => x.ActivityType == _activityType).ToList();


            AccountsInteractedGroups.Clear();

            var id = 1;

            foreach (InteractedGroups report in reportDetails)
            {
                AccountsInteractedGroups.Add(
                    new GroupReportAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        GroupName = report.GroupName,
                        GroupUrl = report.GroupUrl,
                        MembershipStatus = report.MembershipStatus,
                        TotalMembers = report.TotalMembers,
                        GroupType = report.GroupType,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    }
                );

                id++;
            }

            return AccountsInteractedGroups;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Group Name,Group Url,Membership Status,Total Members,Date";
                Header = PostsReportHeader();
                InteractedGroupModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + "," + report.QueryType + "," + report.QueryValue + ","
                                    + $"{report.GroupName?.Replace(",", " ")}" + ","
                                    + report.GroupUrl + ","
                                    + (string.IsNullOrEmpty(report.MembershipStatus)
                                        ? "NA"
                                        : report.MembershipStatus?.Replace(",", " ")) + ","
                                    + (string.IsNullOrEmpty(report.TotalMembers)
                                        ? "NA"
                                        : report.TotalMembers?.Replace(",", " ")) + ","
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
                //Header = "QueryType,QueryValue,Group Name,Group Url,Membership Status,Total Members,Date";
                Header = PostsReportHeader(false);
                AccountsInteractedGroups.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + ","
                                    + $"{report.GroupName}" + ","
                                    + report.GroupUrl + ","
                                    + (string.IsNullOrEmpty(report.MembershipStatus)
                                        ? "NA"
                                        : report.MembershipStatus?.Replace(",", " ")) + ","
                                    + (string.IsNullOrEmpty(report.TotalMembers)
                                        ? "NA"
                                        : report.TotalMembers?.Replace(",", " ")) + ","
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

            #region get data from InteractedUsers table and add to FollowerReportModel

            Header = "AccountEmail,QueryType,QueryValue,Group Name,Group Url,Membership Status,Total Members,Date";

            InteractedGroupModel.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModel

            dataBase.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups>()
                .ForEach(
                    report =>
                    {
                        InteractedGroupModel.Add(new GroupReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            GroupName = report.GroupName,
                            GroupUrl = report.GroupUrl,
                            MembershipStatus = report.MembershipStatus,
                            TotalMembers = report.TotalMembers,
                            InteractionTimeStamp = report.InteractionDateTime
                        });
                        //}
                    });

            #endregion


            //campaign.SelectedAccountList.ToList().ForEach(x =>
            // {
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
                        ColumnHeaderText = "LangKeyQueryType".FromResourceDictionary(), ColumnBindingText = "QueryType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryValue".FromResourceDictionary(),
                        ColumnBindingText = "QueryValue"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyGroupName".FromResourceDictionary(), ColumnBindingText = "GroupName"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyGroupUrl".FromResourceDictionary(), ColumnBindingText = "GroupUrl"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMembershipStatus".FromResourceDictionary(),
                        ColumnBindingText = "MembershipStatus"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyTotalMembers".FromResourceDictionary(),
                        ColumnBindingText = "TotalMembers"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            // });
            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedGroupModel);

            #endregion

            return new ObservableCollection<object>(InteractedGroupModel);
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyGroupName");
            listResource.Add("LangKeyGroupUrl");
            listResource.Add("LangKeyMembershipStatus");
            listResource.Add("LangKeyTotalMembers");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}