using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
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

namespace GramDominatorUI.Utility.InstaPoster.Reposter
{
    internal class ReposterReports : IGdReportFactory
    {
        private static readonly ObservableCollection<ReposterReportDetails> ReposterReportModelCampaign =
            new ObservableCollection<ReposterReportDetails>();

        private static List<ReposterReportDetails> ReposterReportModelAccount { get; } =
            new List<ReposterReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<RePosterModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            ReposterReportModelCampaign.Clear();

            #region Update Existing Table with new Column

            try
            {
                if (campaignDetails.IsInteractedPostsUpdated)
                {
                    var query = "UPDATE InteractedPosts SET Status = 'Success' WHERE Status IS NULL";
                    dataBase._context.Database.ExecuteSqlCommand(query);
                    campaignDetails.IsInteractedPostsUpdated = true;

                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.Edit(campaignDetails);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            #region get data from InteractedUsers table and add to ReposterReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    ReposterReportModelCampaign.Add(new ReposterReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        OriginalMediaCode = report.OriginalMediaCode,
                        OriginalMediaOwner = report.OriginalMediaOwner,
                        GeneratedMediaCode = report.PkOwner,
                        MediaType = report.MediaType,
                        ReposterComment = report.Comment?.Replace("\r\n","\n")?.Replace("\n","\\n"),
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
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
                        ColumnHeaderText = "LangKeyOriginalMediaCode".FromResourceDictionary(),
                        ColumnBindingText = "OriginalMediaCode"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyOriginalMediaOwner".FromResourceDictionary(),
                        ColumnBindingText = "OriginalMediaOwner"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyGeneratedMediaCode".FromResourceDictionary(),
                        ColumnBindingText = "GeneratedMediaCode"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    },
                    //new GridViewColumnDescriptor
                    //{
                    //    ColumnHeaderText = "LangKeyReposterComment".FromResourceDictionary(),
                    //    ColumnBindingText = "ReposterComment"
                    //},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReposterReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(ReposterReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.Reposter).ToList();

            // Need to be cleared data for adding into static variable.
            ReposterReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                ReposterReportModelAccount.Add(
                    new ReposterReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = report.ActivityType,
                        AccountUsername = report.Username,
                        OriginalMediaCode = report.OriginalMediaCode,
                        OriginalMediaOwner = report.OriginalMediaOwner,
                        GeneratedMediaCode = report.PkOwner,
                        MediaType = report.MediaType,
                        ReposterComment = report.Comment,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });

            return ReposterReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity type, Account Name, Original Media Code, Original Media Owner, Generated Media Code, Media Type, Date";

                ReposterReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.OriginalMediaCode + "," +
                                    report.OriginalMediaOwner + "," + report.GeneratedMediaCode + "," +
                                    report.MediaType + "," + report.Date);
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
                Header =
                    "Activity type, Account Name, Original Media Code, Original Media Owner, Generated Media Code, Media Type, Date";

                ReposterReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.OriginalMediaCode + "," + report.OriginalMediaOwner + "," +
                                    report.GeneratedMediaCode + "," + report.MediaType + "," + report.Date);
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