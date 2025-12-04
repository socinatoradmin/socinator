using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts;

namespace GramDominatorUI.Utility.AddStory
{
    public class AddStoryReport: IGdReportFactory
    {
        private static readonly ObservableCollection<StoryReportDetails> AddStoryReportModelCampaign =
            new ObservableCollection<StoryReportDetails>();

        public AddInstaStoryModel AddStoryModel { get; set; }
        private static List<StoryReportDetails> AddStoryReportModelAccount { get; } = new List<StoryReportDetails>();
        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Query Type, Query Value, Activity Type, Account Username, Story Username, Date,Status";

                AddStoryReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + ActivityType.AddStory + "," +
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

                AddStoryReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + ActivityType.AddStory + "," +
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
            AddStoryModel = JsonConvert.DeserializeObject<AddInstaStoryModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);
            // Need to be cleared data for adding into static variable.
            AddStoryReportModelCampaign.Clear();
            var sNo = 0;

            #region Report generate for Campaign activity or broadcast msg activity

            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    AddStoryReportModelCampaign.Add(new StoryReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        InteractedUsername = report.OriginalMediaOwner,
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

            return new ObservableCollection<object>(AddStoryReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.AddStory).ToList();
            // Need to be cleared data for adding into static variable.
            AddStoryReportModelAccount.Clear();
            var sNo = 0;
            foreach (InteractedPosts report in reportDetails)
                AddStoryReportModelAccount.Add(
                    new StoryReportDetails
                    {
                        Id = ++sNo,
                        Query = report.QueryValue,
                        QueryType = report.QueryType,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        ActivityType = report.ActivityType,
                        AccountUsername = report.Username,
                        InteractedUsername = report.OriginalMediaOwner,
                        Status = report.Status
                    });

            dataBase.Get<InteractedPosts>().Where(x => x.ActivityType == ActivityType.AddStory).ForEach(
                report =>
                {
                    AddStoryReportModelAccount.Add(new StoryReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        InteractedUsername = report.OriginalMediaOwner,
                        Status = report.Status
                    });
                });

            return AddStoryReportModelAccount;
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AddInstaStoryModel>(activitySettings).SavedQueries;
        }
    }
}
