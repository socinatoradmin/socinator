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

namespace RedditDominatorUI.Utility.Voting
{
    public class UpVotingReport : IRdReportFactory
    {
        public static ObservableCollection<UpvoteReportModel> UpvoteReportModelCampaign =
            new ObservableCollection<UpvoteReportModel>();

        private static List<UpvoteReportModelAccount> UpvoteReportModelAccount { get; } =
            new List<UpvoteReportModelAccount>();

        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Account, QueryType, ActivityType, Interacted Time, Comments Count, IsCrosspostable, IsStickied, Post Owner, Score, Is Hidden, Is Spoiler, Is Nsfw, PostId, ViewCount , Permalink,Created, Title , Is OriginalContent";

                    UpvoteReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.AccountUsername + "," + report.QueryType + "," + report.ActivityType +
                                        "," + report.InteractionTimeStamp + ","
                                        + report.CommentsCount + ","
                                        + report.IsCrosspostable + "," + report.IsStickied + ","
                                        + report.Author + ","
                                        + report.Score + "," + report.Hidden + ","
                                        + report.IsSpoiler + "," + report.IsNsfw + ","
                                        + report.PostId + "," + report.ViewCount + ","
                                        + report.Permalink + "," + report?.Created.ToString() + ","
                                        + report.Title?.Replace(",", " ").Replace("\r\n", " ").Replace("\n", " ") + ","
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
                        "QueryType, ActivityType, Interacted Time, Comments Count, IsCrosspostable, IsStickied, Post Owner, Score, Is Hidden,Is Spoiler, Is Nsfw,PostId, ViewCount ,Permalink,Created, Title ,Is OriginalContent";

                    UpvoteReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.QueryType + "," + report.ActivityType + "," +
                                        report.InteractionTimeStamp + ","
                                        + report.NumComments + ","
                                        + report.IsCrosspostable + "," + report.IsStickied + ","
                                        + report.Author + ","
                                        + report.Score + "," + report.Hidden + ","
                                        + report.IsSpoiler + "," + report.IsNsfw + ","
                                        + report.PostId + "," + report.ViewCount + ","
                                        + report.Permalink + "," + report?.Created.ToString() + ","
                                        + report.Title.Replace(",", " ").Replace("\r\n", " ").Replace("\n", " ") + ","
                                        + report.IsOriginalContent + ",");
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
            UpvoteReportModelAccount.Clear();
            IList reportDetails = dbAccountService.GetInteractedPosts(ActivityType.Upvote).ToList();
            foreach (InteractedPost report in reportDetails)
                UpvoteReportModelAccount.Add(new UpvoteReportModelAccount
                {
                    Id = columnId++,
                    QueryType = report.QueryType,
                    ActivityType = report.ActivityType,
                    InteractionTimeStamp = report.InteractionDateTime,
                    CommentsCount = report.NumComments,
                    //Caption = report.Caption,
                    IsCrosspostable = report.IsCrosspostable,
                    IsStickied = report.IsStickied,
                    Author = report.InteractedUserName,
                    Score = report.Score,
                    Hidden = report.Hidden,
                    IsSpoiler = report.IsSpoiler,
                    IsNsfw = report.IsNsfw,
                    PostId = report.PostId,
                    ViewCount = report.ViewCount,
                    Permalink = report.Permalink,
                    Created = report.Created.EpochToDateTimeUtc(),
                    Title = report.Title,
                    IsOriginalContent = report.IsOriginalContent
                });
            return UpvoteReportModelAccount;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                ConstantVariable.GetCampaignDb);
            UpvoteReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModel
            var SrNo = 0;
            dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>(x =>
                x.Status == "Success").ForEach(
                report =>
                {
                    UpvoteReportModelCampaign.Add(new UpvoteReportModel
                    {
                        Id = ++SrNo,
                        AccountUsername = report.SinAccUsername,
                        QueryType = report.QueryType,
                        ActivityType = report.ActivityType,
                        InteractionTimeStamp = report.InteractionDateTime,
                        CommentsCount = report.NumComments,
                        //Caption = report.Caption,
                        IsCrosspostable = report.IsCrosspostable,
                        IsStickied = report.IsStickied,
                        Author = report.InteractedUserName,
                        Score = report.Score,
                        Hidden = report.Hidden,
                        IsSpoiler = report.IsSpoiler,
                        IsNsfw = report.IsNsfw,
                        PostId = report.PostId,
                        ViewCount = report.ViewCount,
                        Permalink = report.Permalink,
                        Created = report.Created.EpochToDateTimeUtc(),
                        Title = report.Title,
                        IsOriginalContent = report.IsOriginalContent,
                        Status = report.Status
                    });
                });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "AccountUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted Time", ColumnBindingText = "InteractionTimeStamp"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Comments Count", ColumnBindingText = "CommentsCount"},
                    //new GridViewColumnDescriptor { ColumnHeaderText = "Caption" , ColumnBindingText = "Caption"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "isCrosspostable ", ColumnBindingText = "IsCrosspostable"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "isStickied ", ColumnBindingText = "IsStickied"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Post Username", ColumnBindingText = "Author"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Score", ColumnBindingText = "Score"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "isHidden", ColumnBindingText = "Hidden"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "isSpoiler ", ColumnBindingText = "IsSpoiler"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "isNSFW ", ColumnBindingText = "IsNsfw"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Post Id", ColumnBindingText = "PostId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Views Count", ColumnBindingText = "ViewCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Permalink", ColumnBindingText = "Permalink"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Created Time", ColumnBindingText = "Created"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Title", ColumnBindingText = "Title"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "isOriginalContent ", ColumnBindingText = "IsOriginalContent"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Status ", ColumnBindingText = "Status"}
                };
            //});
            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(UpvoteReportModelCampaign);

            #endregion

            //return UpvoteReportModelCampaign.Count;
            return new ObservableCollection<object>(UpvoteReportModelCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UpvoteModel>(activitySettings).SavedQueries;
        }
    }
}