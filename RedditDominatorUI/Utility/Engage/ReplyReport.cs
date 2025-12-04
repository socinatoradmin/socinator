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

namespace RedditDominatorUI.Utility.Engage
{
    public class ReplyReport : IRdReportFactory
    {
        public static ObservableCollection<CommentReportModel> ReplyReportModelCampaign =
            new ObservableCollection<CommentReportModel>();

        private static List<CommentReportModel> ReplyReportModelAccount { get; } = new List<CommentReportModel>();
        public string Header { get; set; } = string.Empty;


        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Sr.No, Account Name, Activity Type, Time Interacted, Replied Text, Name, Title, NumComments, NumCrossposts, Score, PostId, Permalink, Created";

                    ReplyReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + "," + report.ActivityType + "," +
                                        report.InteractionTimeStamp + ","
                                        + report.CommentText + "," + report.Author + "," +
                                        report.Title?.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.NumComments + "," + report.NumCrossposts + "," + report.Score + ","
                                        + report.PostId + "," + report.Permalink + "," + report.Created + ",");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "Sr.No, Account Name, Activity Type, Time Interacted, Replied Text, Name, Title, NumComments, NumCrossposts, Score, PostId, Permalink, Created";

                    ReplyReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.ActivityType + "," +
                                        report.InteractionTimeStamp + ","
                                        + report.CommentText + "," + report.Author + "," +
                                        report.Title.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.NumComments + "," + report.NumCrossposts + "," + report.Score + ","
                                        + report.PostId + "," + report.Permalink + "," + report.Created + ",");
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
            ReplyReportModelAccount.Clear();
            // Get data from InteractedPost table and add to SubRedditModel
            IList reportDetails = dbAccountService.GetInteractedPosts(ActivityType.Reply).ToList();

            foreach (InteractedPost report in reportDetails)
                ReplyReportModelAccount.Add(new CommentReportModel
                {
                    Id = columnId++,
                    QueryType = report.QueryType,
                    ActivityType = report.ActivityType,
                    InteractionTimeStamp = report.InteractionDateTime,
                    Title = report.Title,
                    CommentText = report.CommentText,
                    NumComments = report.NumComments,
                    NumCrossposts = report.NumCrossposts,
                    Score = report.Score,
                    PostId = report.PostId,
                    Permalink = report.Permalink,
                    Created = report.Created.EpochToDateTimeLocal()
                });
            return ReplyReportModelAccount.Select(x =>
                new
                {
                    x.Id,
                    x.QueryType,
                    x.ActivityType,
                    x.InteractionTimeStamp,
                    x.Title,
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
                ReplyReportModelCampaign.Clear();

                #region

                // get data from InteractedPost table and add to SubRedditModel
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>().ForEach(
                    report =>
                    {
                        ReplyReportModelCampaign.Add(new CommentReportModel
                        {
                            Id = report.Id,
                            AccountUsername = report.SinAccUsername,
                            QueryType = report.QueryType,
                            Author = report.InteractedUserName,
                            ActivityType = report.ActivityType,
                            InteractionTimeStamp = report.InteractionDateTime,
                            Title = report.Title,
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
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Time Interacted", ColumnBindingText = "InteractionTimeStamp"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Replied Text", ColumnBindingText = "CommentText"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Name", ColumnBindingText = "Author"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Title", ColumnBindingText = "Title"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Num of Comments ", ColumnBindingText = "NumComments"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Num of Cross Posts ", ColumnBindingText = "NumCrossposts"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Permalink", ColumnBindingText = "Permalink"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Post Id", ColumnBindingText = "PostId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Created", ColumnBindingText = "Created"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Score ", ColumnBindingText = "Score"}
                    };
                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReplyReportModelCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(ReplyReportModelCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ReplyModel>(activitySettings).SavedQueries;
        }
    }
}