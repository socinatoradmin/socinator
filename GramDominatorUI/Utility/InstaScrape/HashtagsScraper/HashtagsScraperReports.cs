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

namespace GramDominatorUI.Utility.InstaScrape.HashtagsScraper
{
    internal class HashtagsScraperReports : IGdReportFactory
    {
        private static readonly ObservableCollection<HashtagScrapeReportDetails> HashtagsScraperReportModelCampaign =
            new ObservableCollection<HashtagScrapeReportDetails>();

        private static List<HashtagScrapeReportDetails> HashtagsScraperReportModelAccount { get; } =
            new List<HashtagScrapeReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<HashtagsScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            HashtagsScraperReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to _followerReportModel

            var sNo = 0;
            dataBase.Get<HashtagScrape>().ForEach(
                report =>
                {
                    HashtagsScraperReportModelCampaign.Add(new HashtagScrapeReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.AccountUsername,
                        Keyword = report.Keyword,
                        HashtagName = report.HashtagName,
                        HashtagId = report.HashtagId,
                        MediaCount = report.MediaCount,
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
                        ColumnHeaderText = "LangKeyHashtagName".FromResourceDictionary(),
                        ColumnBindingText = "HashtagName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyHashtagId".FromResourceDictionary(), ColumnBindingText = "HashtagId"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaCount".FromResourceDictionary(),
                        ColumnBindingText = "MediaCount"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(HashtagsScraperReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(HashtagsScraperReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.HashtagScrape>()
                .Where(x => x.ActivityType == ActivityType.HashtagsScraper).ToList();

            // Need to be cleared data for adding into static variable.
            HashtagsScraperReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.HashtagScrape report in reportDetails)
                HashtagsScraperReportModelAccount.Add(
                    new HashtagScrapeReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = report.ActivityType,
                        AccountUsername = report.AccountUsername,
                        Keyword = report.Keyword,
                        HashtagName = report.HashtagName,
                        HashtagId = report.HashtagId,
                        MediaCount = report.MediaCount,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });

            return HashtagsScraperReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "AccountName, Activity type, Keyword, Hashtag Name, Hashtag Id, Media Count, Date";

                HashtagsScraperReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountUsername + "," + report.ActivityType + "," + report.Keyword + "," +
                                    report.HashtagName + "," + report.HashtagId + "," + report.MediaCount + "," +
                                    report.Date);
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
                Header = "AccountName, Activity type, Keyword, Hashtag Name, Hashtag Id, Media Count, Date";

                HashtagsScraperReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.Keyword + "," +
                                    report.HashtagName + "," + report.HashtagId + "," + report.MediaCount + "," +
                                    report.Date);
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