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

namespace GramDominatorUI.Utility.InstaScrape.DownloadPhotos
{
    internal class DownloadPhotosReports : IGdReportFactory
    {
        private static readonly ObservableCollection<DownloadPhotoReportDetails> DownloadPhotosReportModelCampaign =
            new ObservableCollection<DownloadPhotoReportDetails>();

        private static List<DownloadPhotoReportDetails> DownloadPhotosReportModelAccount { get; } =
            new List<DownloadPhotoReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<DownloadPhotosModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            // Need to be cleared data for adding into static variable.
            DownloadPhotosReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to _followerReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    DownloadPhotosReportModelCampaign.Add(new DownloadPhotoReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        ScrapedMediaCode = report.PkOwner,
                        ScrapedMediaOwnerUsername = report.UsernameOwner,
                        MediaType = report.MediaType.ToString(),
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        CommentCount = report.TotalComment,
                        LikeCount = report.TotalLike,
                        Location = report.PostLocation,
                        PostDate = report.TakenAt.EpochToDateTimeUtc(),
                        PostUrl = report.PostUrl
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
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(),
                        ColumnBindingText = "ScrapedMediaCode"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyScrapedMediaOwner".FromResourceDictionary(),
                        ColumnBindingText = "ScrapedMediaOwnerUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentCount".FromResourceDictionary(),
                        ColumnBindingText = "CommentCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyLikesCount".FromResourceDictionary(), ColumnBindingText = "LikeCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostLocation".FromResourceDictionary(),
                        ColumnBindingText = "Location"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostUrl".FromResourceDictionary(), ColumnBindingText = "PostUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostDate".FromResourceDictionary(), ColumnBindingText = "PostDate"}
                };

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(DownloadPhotosReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(DownloadPhotosReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.PostScraper).ToList();

            // Need to be cleared data for adding into static variable.
            DownloadPhotosReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                DownloadPhotosReportModelAccount.Add(
                    new DownloadPhotoReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = ActivityType.PostScraper,
                        AccountUsername = report.Username,
                        ScrapedMediaCode = report.PkOwner,
                        MediaType = report.MediaType.ToString(),
                        ScrapedMediaOwnerUsername = report.UsernameOwner,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        CommentCount = report.TotalComment,
                        LikeCount = report.TotalLike,
                        Location = report.PostLocation,
                        PostDate = report.TakenAt.EpochToDateTimeUtc(),
                        PostUrl = report.PostUrl
                    });

            return DownloadPhotosReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date,Comment Count,Like Count,Location,Post Date,Post Url";

                DownloadPhotosReportModelCampaign.ToList().ForEach(report =>
                {
                    if (report.Location != null)
                        if (report.Location.Contains(","))
                            report.Location = report.Location.Replace(",", " ");
                    try
                    {
                        csvData.Add(report.ActivityType + ", " + report.AccountUsername + ", " +
                                    report.ScrapedMediaCode + "," +
                                    report.MediaType + ", " + report.ScrapedMediaOwnerUsername + ", " + report.Date +
                                    ", " + report.CommentCount + ", " + report.LikeCount + ", " + report.Location +
                                    "," + report.PostDate + "," + report.PostUrl);
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
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date,Comment Count,Like Count,Location,Post Date,Post Url";

                DownloadPhotosReportModelAccount.ToList().ForEach(report =>
                {
                    if (report.Location != null)
                        if (report.Location.Contains(","))
                            report.Location = report.Location.Replace(",", " ");
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.ScrapedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.ScrapedMediaOwnerUsername + "," + report.Date +
                                    "," + report.CommentCount + "," + report.LikeCount + "," + report.Location + "," +
                                    report.PostDate + "," + report.PostUrl);
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