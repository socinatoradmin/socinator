using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace RedditDominatorUI.Utility.AutoActivity
{
    public class AutoActivityReport : IRdReportFactory
    {
        public string Header { get; set; } = string.Empty;
        public static ObservableCollection<RedditAutoActivityModel> AutoActivityReports { get; set; } = new ObservableCollection<RedditAutoActivityModel>();
        public List<RedditAutoActivityModel> AutoActivityAccountResports { get; set; } = new List<RedditAutoActivityModel>();
        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Sr.No,Activity Type,Post ID,Post Url,Community Url,Profile Url,Interacted Date,Created Date,Following,Joined, Upvoted, Downvoted";

                    AutoActivityReports.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(
                                report.Id + "," + report.ActivityType + "," + report.PostId + ","
                                + report.PostUrl + "," + report.CommunityUrl + "," + report.ProfileUrl + ","
                                + report.InteractedDate + ","
                                + report.Created + "," + report.IsFollowing + ","
                                + report.IsJoined + "," + report.IsUpvoted + "," + report.IsDownvoted);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "Sr.No,Activity Type,Post ID,Post Url,Community Url,Profile Url,Interacted Date,Created Date,Following,Joined, Upvoted, Downvoted";

                    AutoActivityAccountResports.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(
                                report.Id + "," + report.ActivityType + "," + report.PostId + ","
                                + report.PostUrl + "," + report.CommunityUrl + "," + report.ProfileUrl + ","
                                + report.InteractedDate + ","
                                + report.Created + "," + report.IsFollowing + ","
                                + report.IsJoined + "," + report.IsUpvoted + "," + report.IsDownvoted);
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
            AutoActivityAccountResports.Clear();
            IList reportdetails = dbAccountService.GetInteractedAutoActivityPosts(ActivityType.AutoActivity).ToList();
            foreach (InteractedAutoActivityPost report in reportdetails)
                AutoActivityAccountResports.Add(new RedditAutoActivityModel
                {
                    Id = columnId++,
                    ActivityType = report.ActivityType,
                    InteractedDate = report.InteractedDate,
                    Created = report.Created,
                    PostId = report.PostId,
                    PostUrl = report.PostUrl,
                    IsDownvoted = report.IsDownvoted,
                    IsFollowing = report.IsFollowing,
                    IsJoined = report.IsJoined,
                    IsUpvoted = report.IsUpvoted,
                    CommunityUrl = report.CommunityUrl,
                    ProfileUrl = report.ProfileUrl
                });
            return AutoActivityAccountResports.Select(x =>
                new
                {
                    x.Id,
                    x.ActivityType,
                    x.InteractedDate,
                    x.Created,
                    x.PostId,
                    x.PostUrl,
                    x.IsDownvoted,
                    x.IsFollowing,
                    x.IsJoined,
                    x.IsUpvoted,
                    x.CommunityUrl,
                    x.ProfileUrl
                }).ToList();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                AutoActivityReports.Clear();

                #region

                // Get data from InteractedPost table and add to SubRedditModel
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedAutoActivityPostCampaign>().ForEach(
                    report =>
                    {
                        AutoActivityReports.Add(new RedditAutoActivityModel
                        {
                            Id = report.Id,
                            PostId = report.PostId,
                            PostUrl = report.PostUrl,
                            ActivityType = report.ActivityType,
                            UserName = report.UserName,
                            IsFollowing = report.IsFollowing,
                            IsJoined = report.IsJoined,
                            IsDownvoted = report.IsDownvoted,
                            IsUpvoted = report.IsUpvoted,
                            InteractedDate = report.InteractedDate,
                            CommunityUrl = report.CommunityUrl,
                            Created = report.Created,
                            ProfileUrl = report.ProfileUrl
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Post ID", ColumnBindingText = "PostId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Post Url", ColumnBindingText = "PostUrl"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "User Name", ColumnBindingText = "UserName"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Following", ColumnBindingText = "IsFollowing"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Joined", ColumnBindingText = "IsJoined"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Downvoted", ColumnBindingText = "IsDownvoted"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Upvoted", ColumnBindingText = "IsUpvoted"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Interacted Date", ColumnBindingText = "InteractedDate"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Created Date", ColumnBindingText = "Created"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Community Url", ColumnBindingText = "CommunityUrl"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Profile Url", ColumnBindingText = "ProfileUrl"}
                    };
                reportModel.ReportCollection = CollectionViewSource.GetDefaultView(AutoActivityReports);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(AutoActivityReports);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<PostAutoActivityModel>(activitySettings).SavedQueries;
        }
    }
}
