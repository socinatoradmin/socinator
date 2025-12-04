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

namespace GramDominatorUI.Utility.GrowFollowers.FollowBack
{
    internal class FollowBackReports : IGdReportFactory
    {
        private static readonly ObservableCollection<FollowBackReportDetails> FollowBackReportModelCampaign =
            new ObservableCollection<FollowBackReportDetails>();

        private static List<FollowBackReportDetails> FollowBackReportModelAccount { get; } =
            new List<FollowBackReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FollowBackModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            FollowBackReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to FollowBackReporModel

            var sNo = 0;
            dataBase.Get<InteractedUsers>().ForEach(
                report =>
                {
                    FollowBackReportModelCampaign.Add(new FollowBackReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        FollowedBackUsername = report.InteractedUsername,
                        FollowedBackUserId = report.InteractedUserId,
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
                        ColumnHeaderText = "LangKeyFollowedBackUsername".FromResourceDictionary(),
                        ColumnBindingText = "FollowedBackUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFollowedBackUserId".FromResourceDictionary(),
                        ColumnBindingText = "FollowedBackUserId"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(FollowBackReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(FollowBackReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.FollowBack.ToString()).ToList();

            // Need to be cleared data for adding into static variable.
            FollowBackReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
                FollowBackReportModelAccount.Add(
                    new FollowBackReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), report.ActivityType),
                        AccountUsername = report.Username,
                        FollowedBackUsername = report.InteractedUsername,
                        FollowedBackUserId = report.InteractedUserId,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });

            return FollowBackReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity type, AccountName, Followed Back Username, Followed Back User Id, Date";

                FollowBackReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.FollowedBackUsername + "," +
                                    report.FollowedBackUserId + "," + report.Date);
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
                Header = "Activity type, Account Name, Followed Back Username, Followed Back User Id, Date";

                FollowBackReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.FollowedBackUsername + "," +
                                    report.FollowedBackUserId + "," + report.Date);
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