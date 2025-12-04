using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;

namespace GramDominatorUI.Utility.GrowFollowers.Unfollower
{
    internal class UnfollowerReports : IGdReportFactory
    {
        private static readonly ObservableCollection<UnfollowReportDetails> UnfollowerReportModelCampaign =
            new ObservableCollection<UnfollowReportDetails>();

        private static List<UnfollowReportDetails> UnfollowReportModelAccount { get; } =
            new List<UnfollowReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnfollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            UnfollowerReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModelCampaign

            var sNo = 0;
            dataBase.Get<UnfollowedUsers>().ForEach(
                report =>
                {
                    UnfollowerReportModelCampaign.Add(new UnfollowReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.AccountUsername,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        UnfollowedUsername = report.UnfollowedUsername
                    });
                });

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(UnfollowerReportModelCampaign);

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
                        ColumnBindingText = "AccountUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "UnfollowedUsername".FromResourceDictionary(),
                        ColumnBindingText = "UnfollowedUsername"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(UnfollowerReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(UnfollowerReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UnfollowedUsers>()
                .ToList();

            // Need to be cleared data for adding into static variable.
            UnfollowReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.UnfollowedUsers report in reportDetails)
                UnfollowReportModelAccount.Add(
                    new UnfollowReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.AccountUsername,
                        ActivityType = ActivityType.Unfollow,
                        UnfollowedUsername = report.UnfollowedUsername,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });

            return UnfollowReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity Type, Account Username, Unfollowed Username, Date";

                UnfollowerReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(ActivityType.Unfollow + "," + report.AccountUsername + "," +
                                    report.UnfollowedUsername + "," + report.Date);
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
                Header = "Activity Type, Account Username, Unfollowed Username, Date";

                UnfollowReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(ActivityType.Unfollow + "," + report.AccountUsername + "," +
                                    report.UnfollowedUsername + "," + report.Date);
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