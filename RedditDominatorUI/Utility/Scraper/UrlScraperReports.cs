using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.ReportModel;
using RedditDominatorCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace RedditDominatorUI.Utility.Scraper
{
    public class PostScraperReports : IRdReportFactory
    {
        public static ObservableCollection<UrlScraperReportModel> UrlScraperReportModelCampaign =
            new ObservableCollection<UrlScraperReportModel>();

        private static List<UrlScraperReportModel> UrlScraperReportModelAccount { get; } =
            new List<UrlScraperReportModel>();

        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Account, ActivityType, Interacted Time, Comments Count, IsCrosspostable, IsStickied, Post Owner, Score, Is Hidden,Is Spoiler, Is Nsfw, PostId, ViewCount, Permalink, Created, Title, Is OriginalContent";

                    UrlScraperReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.AccountUsername + "," + report.ActivityType + "," +
                                        report.InteractionTimeStamp + ","
                                        + report.CommentsCount + "," + report.IsCrosspostable + "," +
                                        report.IsStickied + ","
                                        + report.Author + "," + report.Score + "," + report.Hidden + "," +
                                        report.IsSpoiler + ","
                                        + report.IsNsfw + "," + report.PostId + "," + report.ViewCount + "," +
                                        report.Permalink + ","
                                        + report.Created + "," + report.Title.Replace(",", " ").Replace("\r\n", " ") +
                                        ","
                                        + report.IsOriginalContent + ",");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "ActivityType,Interacted Time,Comments Count,IsCrosspostable,IsStickied,Post Owner,Score,Is Hidden,Is Spoiler,Is Nsfw,PostId,ViewCount,Permalink,Created,Title,Is OriginalContent";

                    UrlScraperReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.ActivityType + "," + report.InteractionTimeStamp + "," +
                                        report.CommentsCount + ","
                                        + report.IsSpoiler + "," + report.IsNsfw + "," + report.PostId + "," +
                                        report.ViewCount + ","
                                        + report.Permalink + "," + report.Created + "," + report.Author + "," +
                                        report.IsCrosspostable + ","
                                        + report.IsStickied + "," + report.Score + "," + report.Hidden + ","
                                        + report.Title.Replace(",", " ").Replace("\r\n", " ") + "," +
                                        report.IsOriginalContent + ",");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reportType), reportType, null);
            }

            #endregion

            #region Account reports

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            var columnId = 1;
            UrlScraperReportModelAccount.Clear();
            IList reportDetails = dbAccountService.GetInteractedPosts(ActivityType.UrlScraper).ToList();
            foreach (InteractedPost report in reportDetails)
                UrlScraperReportModelAccount.Add(new UrlScraperReportModel
                {
                    Id = columnId++,
                    ActivityType = report.ActivityType,
                    QueryType = report.QueryType,
                    InteractionTimeStamp = report.InteractionDateTime,
                    Author = report.InteractedUserName,
                    Title = report.Title,
                    Permalink = report.Permalink,
                    CommentsCount = report.NumComments,
                    IsCrosspostable = report.IsCrosspostable,
                    IsStickied = report.IsStickied,
                    Score = report.Score,
                    Hidden = report.Hidden,
                    IsSpoiler = report.IsSpoiler,
                    IsNsfw = report.IsNsfw,
                    PostId = report.PostId,
                    ViewCount = report.ViewCount,
                    Created = report.Created.EpochToDateTimeLocal(),
                    IsOriginalContent = report.IsOriginalContent
                });
            return UrlScraperReportModelAccount.Select(x => new
            {
                x.Id,
                x.ActivityType,
                x.InteractionTimeStamp,
                x.Author,
                x.Title,
                x.Permalink,
                x.CommentsCount,
                x.IsCrosspostable,
                x.IsStickied,
                x.Score,
                x.Hidden,
                x.IsSpoiler,
                x.IsNsfw,
                x.PostId,
                x.ViewCount,
                x.Created,
                x.IsOriginalContent
            }).ToList();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                UrlScraperReportModelCampaign.Clear();

                #region get data from InteractedUsers table and add to UnfollowerReportModel

                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>().ForEach(
                    report =>
                    {
                        UrlScraperReportModelCampaign.Add(new UrlScraperReportModel
                        {
                            Id = report.Id,
                            AccountUsername = report.SinAccUsername,
                            ActivityType = report.ActivityType,
                            QueryType = report.QueryType,
                            InteractionTimeStamp = report.InteractionDateTime,
                            Author = report.InteractedUserName,
                            Title = report.Title,
                            Permalink = report.Permalink,
                            CommentsCount = report.NumComments,
                            IsCrosspostable = report.IsCrosspostable,
                            IsStickied = report.IsStickied,
                            Score = report.Score,
                            Hidden = report.Hidden,
                            IsSpoiler = report.IsSpoiler,
                            IsNsfw = report.IsNsfw,
                            PostId = report.PostId,
                            ViewCount = report.ViewCount,
                            Created = report.Created.EpochToDateTimeLocal(),
                            IsOriginalContent = report.IsOriginalContent
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Username", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Interacted Time", ColumnBindingText = "InteractionTimeStamp"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Post Username", ColumnBindingText = "Author"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Title", ColumnBindingText = "Title"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Permalink", ColumnBindingText = "Permalink"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Comments Count", ColumnBindingText = "CommentsCount"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isCrosspostable ", ColumnBindingText = "IsCrosspostable"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isStickied ", ColumnBindingText = "IsStickied"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Score", ColumnBindingText = "Score"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isHidden", ColumnBindingText = "Hidden"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isSpoiler ", ColumnBindingText = "IsSpoiler"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isNSFW ", ColumnBindingText = "IsNsfw"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Post Id", ColumnBindingText = "PostId"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Views Count", ColumnBindingText = "ViewCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Created Time", ColumnBindingText = "Created"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isOriginalContent ", ColumnBindingText = "IsOriginalContent"}
                    };
                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(UrlScraperReportModelCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(UrlScraperReportModelCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UrlScraperModel>(activitySettings).SavedQueries;
        }
    }
}