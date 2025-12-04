using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts;

namespace GramDominatorUI.Utility.StoryViews
{
    public class StoryViewReports : IGdReportFactory
    {
        private static readonly ObservableCollection<StoryReportDetails> StoryReportModelCampaign =
            new ObservableCollection<StoryReportDetails>();

        public StoryModel StoryModel { get; set; }
        private static List<StoryReportDetails> StoryReportModelAccount { get; } = new List<StoryReportDetails>();
        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Query Type, Query Value, Activity Type, Account Username, Story Username, Date,Status";

                StoryReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.ActivityType + "," +
                                    report.AccountUsername + "," +
                                    report.InteractedUsername + "," + report.Date + "," + report.Status);
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
                Header = "Query Type, Query Value, Activity Type, Account Username, Story Username, Date,Status";

                StoryReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.ActivityType + "," +
                                    report.AccountUsername + "," +
                                    report.InteractedUsername + "," + report.Date + "," + report.Status);
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

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            StoryModel = JsonConvert.DeserializeObject<StoryModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);
            // Need to be cleared data for adding into static variable.
            StoryReportModelCampaign.Clear();
            var sNo = 0;

            #region Report generate for Campaign activity or broadcast msg activity

            dataBase.Get<InteractedUsers>().ForEach(
                report =>
                {
                    StoryReportModelCampaign.Add(new StoryReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                        QueryType = report.QueryType,
                        Query = report.Query,
                        InteractedUsername = report.InteractedUsername,
                        Status = report.Status
                    });
                });
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
                        ColumnHeaderText = "LangKeyQueryType".FromResourceDictionary(), ColumnBindingText = "QueryType"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyQuery".FromResourceDictionary(), ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserStory".FromResourceDictionary(),
                        ColumnBindingText = "InteractedUsername"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyStatus".FromResourceDictionary(), ColumnBindingText = "Status"}
                };

            #endregion

            return new ObservableCollection<object>(StoryReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.StoryViewer.ToString()).ToList();
            // Need to be cleared data for adding into static variable.
            StoryReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
                StoryReportModelAccount.Add(
                    new StoryReportDetails
                    {
                        Id = ++sNo,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                        ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), report.ActivityType),
                        AccountUsername = report.Username,
                        InteractedUsername = report.InteractedUsername,
                        Status = report.Status
                    });

            dataBase.Get<InteractedPosts>().Where(x => x.ActivityType == ActivityType.StoryViewer).ForEach(
                report =>
                {
                    StoryReportModelAccount.Add(new StoryReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        InteractedUsername = report.UsernameOwner,
                        Status = report.Status
                    });
                });

            return StoryReportModelAccount;
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<StoryModel>(activitySettings).SavedQueries;
        }
    }
}