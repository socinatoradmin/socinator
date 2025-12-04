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
    public class CommentScraperReport : IRdReportFactory
    {
        public static ObservableCollection<CommentReportModel> CommentScraperReportModelCampaign =
            new ObservableCollection<CommentReportModel>();

        private static List<CommentReportModel> CommentScraperReportModelAccount { get; } =
            new List<CommentReportModel>();

        public string Header { get; set; } = string.Empty;


        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Sr.No, Account Name, Activity Type,Time Interacted, Author, CommentText,NumComments, NumCrossposts,  PostId, Permalink,Created, Score";

                    CommentScraperReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + "," + report.ActivityType
                                        + "," + report.InteractionTimeStamp + "," + report.Author + ","
                                        + report.CommentText.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.NumComments + "," + report.NumCrossposts + ","
                                        + report.PostId + ","
                                        + report.Permalink + "," + report.Created + ","
                                        + report.Score + ",");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "Sr.No, QueryType, Activity Type,Time Interacted, CommentText,NumComments, NumCrossposts,  PostId, Permalink,Created, Score";

                    CommentScraperReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.QueryType + "," + report.ActivityType
                                        + "," + report.InteractionTimeStamp + ","
                                        + report.CommentText.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.NumComments + "," + report.NumCrossposts + ","
                                        + report.PostId + ","
                                        + report.Permalink + "," + report.Created + ","
                                        + report.Score + ",");
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
            CommentScraperReportModelAccount.Clear();
            // Get data from InteractedPost table and add to SubRedditModel
            IList reportDetails = dbAccountService.GetInteractedPosts(ActivityType.CommentScraper).ToList();
            foreach (InteractedPost report in reportDetails)
                CommentScraperReportModelAccount.Add(new CommentReportModel
                {
                    Id = columnId++,
                    QueryType = report.QueryType,
                    ActivityType = report.ActivityType,
                    InteractionTimeStamp = report.InteractionDateTime,
                    CommentText = report.CommentText,
                    NumComments = report.NumComments,
                    NumCrossposts = report.NumCrossposts,
                    Score = report.Score,
                    PostId = report.PostId,
                    Permalink = report.Permalink,
                    Created = report.Created.EpochToDateTimeLocal()
                });
            return CommentScraperReportModelAccount.Select(x => new
            {
                x.Id,
                x.QueryType,
                x.ActivityType,
                x.InteractionTimeStamp,
                x.CommentText,
                x.NumComments,
                x.NumCrossposts,
                x.Score,
                x.PostId,
                x.Permalink,
                x.Created
            }).ToList();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                CommentScraperReportModelCampaign.Clear();

                #region

                // Get data from InteractedPost table and add to SubRedditModel
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>().ForEach(
                    report =>
                    {
                        CommentScraperReportModelCampaign.Add(new CommentReportModel
                        {
                            Id = report.Id,
                            AccountUsername = report.SinAccUsername,
                            Author = report.InteractedUserName,
                            ActivityType = report.ActivityType,
                            InteractionTimeStamp = report.InteractionDateTime,
                            CommentText = report.CommentText,
                            NumComments = report.NumComments,
                            NumCrossposts = report.NumCrossposts,
                            Score = report.Score,
                            PostId = report.PostId,
                            Permalink = report.Permalink,
                            Created = report.Created.EpochToDateTimeLocal()
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account Name", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Time Interacted", ColumnBindingText = "InteractionTimeStamp"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Name", ColumnBindingText = "Author"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "CommentText", ColumnBindingText = "CommentText"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Num of Comments ", ColumnBindingText = "NumComments"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Num of Cross Posts ", ColumnBindingText = "NumCrossposts"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Permalink", ColumnBindingText = "Permalink"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Post Id", ColumnBindingText = "PostId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Created", ColumnBindingText = "Created"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Score ", ColumnBindingText = "Score"}
                    };
                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(CommentScraperReportModelCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(CommentScraperReportModelCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentModel>(activitySettings).SavedQueries;
        }
    }
}