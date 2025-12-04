using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using GramDominatorCore.GDModel;
using System.Collections;
using GramDominatorCore.GDLibrary.DAL;
using System.Windows.Data;
using GramDominatorCore.Report;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
namespace GramDominatorUI.Utility.GrowFollowers.MakeCloseFriend
{
    public class CloseFriendReport: IGdReportFactory
    {
        private static readonly ObservableCollection<CloseFriendReportDetails> CloseFriendReportDetailsModelCampaign =
            new ObservableCollection<CloseFriendReportDetails>();

        private static List<CloseFriendReportDetails> CloseFriendReportDetailsModelAccount { get; } =
            new List<CloseFriendReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CloseFriendModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            CloseFriendReportDetailsModelCampaign.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModelCampaign

            var sNo = 0;
            dataBase.Get<MakeCloseFriendCampaign>().ForEach(
                report =>
                {
                    CloseFriendReportDetailsModelCampaign.Add(new CloseFriendReportDetails
                    {
                        Id = ++sNo,
                        AccountUserName = report.AccountUserName,
                        InteractedDate = report.InteractedDate,
                        UserName = report.UserName,
                        ActivityType = report.ActivityType,
                        IsCloseFriend = report.IsCloseFriend
                    });
                });

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(CloseFriendReportDetailsModelCampaign);

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKey﻿﻿﻿﻿Id".FromResourceDictionary(), ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyAccountUsername".FromResourceDictionary(),
                        ColumnBindingText = "AccountUserName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "Closed Friend Username",
                        ColumnBindingText = "UserName"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "InteractedDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Is Closed Friend", ColumnBindingText = "IsCloseFriend"}
                };

            #endregion

            return new ObservableCollection<object>(CloseFriendReportDetailsModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UnfollowedUsers>()
                .ToList();

            // Need to be cleared data for adding into static variable.
            CloseFriendReportDetailsModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.MakeCloseFriendAccount report in reportDetails)
                CloseFriendReportDetailsModelAccount.Add(
                    new CloseFriendReportDetails
                    {
                        Id = ++sNo,
                        AccountUserName = report.AccountUserName,
                        InteractedDate = report.InteractedDate.EpochToDateTimeUtc().ToLocalTime(),
                        UserName = report.UserName,
                        ActivityType = report.ActivityType,
                        IsCloseFriend = report.IsCloseFriend
                    });

            return CloseFriendReportDetailsModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity Type, Account Username, Closed Friend Username, Interacted Date,Closed Friend";

                CloseFriendReportDetailsModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUserName + "," +
                                    report.UserName + "," + report.InteractedDate + "," + report.IsCloseFriend);
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
                Header = "Activity Type, Account Username, Closed Friend Username, Interacted Date,Closed Friend";

                CloseFriendReportDetailsModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUserName + "," +
                                    report.UserName + "," + report.InteractedDate + "," + report.IsCloseFriend);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}
