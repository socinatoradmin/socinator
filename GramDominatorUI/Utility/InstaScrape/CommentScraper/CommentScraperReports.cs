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

namespace GramDominatorUI.Utility.InstaScrape.CommentScraper
{
    public class CommentScraperReports : IGdReportFactory
    {
        private static readonly ObservableCollection<CommentScraperReportDetails> CommentScraperReportModelCampaign =
            new ObservableCollection<CommentScraperReportDetails>();

        private static List<CommentScraperReportDetails> CommentScraperReportModelAccount { get; } =
            new List<CommentScraperReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentScraperModel>(activitySettings).SavedQueries;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date,Comment Owner Name,Comment Owner Id, Status,Comment";

                CommentScraperReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.LikedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.LikedMediaOwner + "," + report.Date + "," +
                                    report.CommentOwnerName + "," + report.CommentOwnerId + "," + report.status + "," +
                                    report.Comments);
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
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date,Comment Owner Name,Comment Owner Id, Status,Comment";

                CommentScraperReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.LikedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.LikedMediaOwner + "," + report.Date +
                                    report.Comments.Last() + report.CommentOwnerName + "," + report.CommentOwnerId +
                                    "," + report.status + "," + report.Comments);
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

            // Need to be cleared data for adding into static variable.
            CommentScraperReportModelCampaign.Clear();

            #region get data from InteractedPosts table and add to LikerReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    CommentScraperReportModelCampaign.Add(new CommentScraperReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        LikedMediaCode = report.PkOwner,
                        LikedMediaOwner = report.UsernameOwner,
                        Comments = report.Comment,
                        CommentOwnerName = report.CommentOwnerName,
                        CommentOwnerId = report.CommentOwnerId,
                        status = "Scraped",
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        MediaType = report.MediaType.ToString()
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
                        ColumnBindingText = "LikedMediaOwner"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(),
                        ColumnBindingText = "LikedMediaCode"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyCommentDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyScrapeMediaType".FromResourceDictionary(),
                        ColumnBindingText = "MediaType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentOwnerUsername".FromResourceDictionary(),
                        ColumnBindingText = "CommentOwnerName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentOwnerUserId".FromResourceDictionary(),
                        ColumnBindingText = "CommentOwnerId"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyStatus".FromResourceDictionary(), ColumnBindingText = "status"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyComment".FromResourceDictionary(), ColumnBindingText = "Comments"}
                };

            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(CommentScraperReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(CommentScraperReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.CommentScraper).ToList();

            // Need to be cleared data for adding into static variable.
            CommentScraperReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                CommentScraperReportModelAccount.Add(
                    new CommentScraperReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        ActivityType = ActivityType.CommentScraper,
                        LikedMediaOwner = report.UsernameOwner,
                        LikedMediaCode = report.PkOwner,
                        MediaType = report.MediaType.ToString(),
                        Comments = report.Comment,
                        CommentOwnerName = report.CommentOwnerName,
                        CommentOwnerId = report.CommentOwnerId,
                        status = "Scraped",
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });

            return CommentScraperReportModelAccount;
        }
    }
}