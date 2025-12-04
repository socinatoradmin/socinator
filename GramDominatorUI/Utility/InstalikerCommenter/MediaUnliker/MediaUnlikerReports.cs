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

namespace GramDominatorUI.Utility.InstalikerCommenter.MediaUnliker
{
    internal class MediaUnlikerReports : IGdReportFactory
    {
        private static readonly ObservableCollection<MediaUnlikerReportDetails> MediaUnlikerReportModelCampaign =
            new ObservableCollection<MediaUnlikerReportDetails>();

        private static List<MediaUnlikerReportDetails> MediaUnlikerReportModelAccount { get; } =
            new List<MediaUnlikerReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<MediaUnlikerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            MediaUnlikerReportModelCampaign.Clear();

            #region Update Existing Table with new Column

            try
            {
                if (!campaignDetails.IsInteractedPostsUpdated)
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

            #region get data from InteractedPosts table and add to MediaUnlikerReportModelCampaign

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    MediaUnlikerReportModelCampaign.Add(new MediaUnlikerReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        MediaCode = report.PkOwner,
                        MediaOwner = report.UsernameOwner,
                        MediaType = report.MediaType,
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
                        ColumnHeaderText = "LangKeyMediaOwner".FromResourceDictionary(),
                        ColumnBindingText = "MediaOwner"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(), ColumnBindingText = "MediaCode"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    }
                };

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(MediaUnlikerReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(MediaUnlikerReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.Unlike).ToList();

            // Need to be cleared data for adding into static variable.
            MediaUnlikerReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                MediaUnlikerReportModelAccount.Add(
                    new MediaUnlikerReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = ActivityType.Unlike,
                        AccountUsername = report.Username,
                        MediaCode = report.PkOwner,
                        MediaOwner = report.UsernameOwner,
                        MediaType = report.MediaType,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });

            return MediaUnlikerReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date";

                MediaUnlikerReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.MediaCode + "," +
                                    report.MediaType + "," + report.MediaOwner + "," + report.Date);
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
                Header = "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date";

                MediaUnlikerReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.MediaCode + "," +
                                    report.MediaType + "," + report.MediaOwner + "," + report.Date);
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