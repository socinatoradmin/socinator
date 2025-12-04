using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Reports.AccountConfigReport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoraDominatorUI.Utility.Report.Voting
{
    public class UpvotePostsReport : IQdReportFactory
    {
        private static readonly ObservableCollection<InteractedPost> PostDetailModel =
            new ObservableCollection<InteractedPost>();
        private static readonly ObservableCollection<InteractedPost> LstInteractedPost =
            new ObservableCollection<InteractedPost>();
        public string Header { get; set; }

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            try
            {
                var csvData = new List<string>();
                if (reportType == ReportType.Campaign)
                {
                    Header = "AccountName,QueryType,Query,PostUrl,Upvote Count,Share Count,Views Count,Owner FollowerCount,Interaction Time";
                    PostDetailModel.ForEach(report =>
                    {
                        csvData.Add(report.AccountName + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.PostUrl + "," + report.UpvoteCount + "," + report.ShareCount + ","
                                    + report.ViewsCount + "," + report.PostOwnerFollowerCount +","+report.InteractionDateTime);
                    });
                }
                else
                {
                    Header = "Id,QueryType,Query,PostUrl,Upvote Count,Share Count,Views Count,Owner FollowerCount,Interaction Time";
                    LstInteractedPost.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.PostUrl + "," + report.UpvoteCount + "," + report.ShareCount + ","
                                    + report.ViewsCount + "," + report.PostOwnerFollowerCount +","+report.InteractionDateTime);
                    });
                }

                Utilities.ExportReports(fileName, Header, csvData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            LstInteractedPost.Clear();
            try
            {
                IList reportDetails = dbAccountService.GetInteractedPosts(ActivityType.UpvotePost).ToList();
                foreach (DominatorHouseCore.DatabaseHandler.QdTables.Accounts.InteractedPosts report in reportDetails)
                    LstInteractedPost.Add(
                        new InteractedPost
                        {
                            Id = report.Id,
                            InteractionDateTime = report.InteractionDate,
                            QueryValue = report.QueryValue,
                            PostUrl = report.PostUrl,
                            UpvoteCount = report.UpvoteCount,
                            ShareCount = report.ShareCount,
                            ViewsCount = report.ViewsCount,
                            CommentCount = report.CommentCount,
                            PostOwnerFollowerCount = report.PostOwnerFollowerCount,
                            PostOwnerName = report.PostOwnerName
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstInteractedPost;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            PostDetailModel.Clear();
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);

                #region get data from InteractedUsers table and add to FollowerReportModel

                dataBase.Get<InteractedPosts>()?.ForEach(
                    report =>
                    {
                        PostDetailModel.Add(new InteractedPost
                        {
                            Id = report.Id,
                            AccountName=report.SinAccUsername,
                            QueryType=report.QueryType,
                            QueryValue=report.QueryValue,
                            ActivityType = ActivityType.UpvotePost.ToString(),
                            PostUrl = report.PostUrl,
                            UpvoteCount = report.LikeCount,
                            ShareCount = report.ShareCount,
                            ViewsCount = report.ViewsCount,
                            CommentCount = report.CommentCount,
                            PostOwnerFollowerCount = report.PostOwnerFollowerCount,
                            PostOwnerName=report.PostOwnerName,
                            InteractionDateTime=report.InteractionDate
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "PostUrl", ColumnBindingText = "PostUrl"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "UpvoteCount", ColumnBindingText = "UpvoteCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "ShareCount", ColumnBindingText = "ShareCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "ViewsCount", ColumnBindingText = "ViewsCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "CommentCount", ColumnBindingText = "CommentCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "PostOwner FollowerCount", ColumnBindingText = "PostOwnerFollowerCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "PostOwnerName", ColumnBindingText = "PostOwnerName"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Interacted Time", ColumnBindingText = "InteractionDateTime"}
                    };

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(PostDetailModel);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UpvotePostsModel>(activitySettings).SavedQueries;
        }
    }
}
