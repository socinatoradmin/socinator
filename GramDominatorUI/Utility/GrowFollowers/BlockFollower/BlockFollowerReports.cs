using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;

namespace GramDominatorUI.Utility.GrowFollowers.BlockFollower
{
    internal class BlockFollowerReports : IGdReportFactory
    {
        private static readonly ObservableCollection<BlockFollowerReportDetails> BlockFollowerReportModelCampaign =
            new ObservableCollection<BlockFollowerReportDetails>();

        private static List<BlockFollowerReportDetails> BlockFollowerReportModelAccount { get; } =
            new List<BlockFollowerReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BlockFollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            BlockFollowerReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to _followerReportModel

            var sNo = 0;
            dataBase.Get<InteractedUsers>().ForEach(
                report =>
                {
                    BlockFollowerReportModelCampaign.Add(new BlockFollowerReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        BlockedFollowerUsername = report.InteractedUsername,
                        BlockedFollowerUserId = report.InteractedUserId,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });
                });

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
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKey﻿﻿﻿﻿BlockedFollowerUsername".FromResourceDictionary(),
                        ColumnBindingText = "BlockedFollowerUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKey﻿﻿﻿﻿BlockedFollowerUserId".FromResourceDictionary(),
                        ColumnBindingText = "BlockedFollowerUserId"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(BlockFollowerReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(BlockFollowerReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.BlockFollower.ToString()).ToList();

            // Need to be cleared data for adding into static variable.
            BlockFollowerReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
                BlockFollowerReportModelAccount.Add(
                    new BlockFollowerReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), report.ActivityType),
                        AccountUsername = report.Username,
                        BlockedFollowerUsername = report.InteractedUsername,
                        BlockedFollowerUserId = report.InteractedUserId,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });

            return BlockFollowerReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity type, AccountName, Blocked Follower Username, Blocked Follower User Id, Date";

                BlockFollowerReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.BlockedFollowerUsername + "," +
                                    report.BlockedFollowerUserId + "," + report.Date);
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
                Header = "Activity type, Account Name, Blocked Follower Username, Blocked Follower User Id, Date";

                BlockFollowerReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.BlockedFollowerUsername + "," +
                                    report.BlockedFollowerUserId + "," + report.Date);
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