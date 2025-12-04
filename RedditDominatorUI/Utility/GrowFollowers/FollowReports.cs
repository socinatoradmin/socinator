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

namespace RedditDominatorUI.Utility.GrowFollowers
{
    public class FollowReports : IRdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> FollowReportModelCampaign =
            new ObservableCollection<InteractedUsersReportModel>();

        private static List<InteractedUsersReportModel> FollowReportModelAccount { get; } =
            new List<InteractedUsersReportModel>();

        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Account, Activity Type, Query Type, Query Value, InteractedUsername, Comment Karma,Created, Display Name, Is Employee, Is Following, Is Moderator, Is NSFW, Post Karma, URL ";

                    FollowReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.SinAccUsername + "," + report.ActivityType + ","
                                        + report.QueryType + ","
                                        + report.QueryValue + "," + report.InteractedUsername + ","
                                        + report.CommentKarma + ","
                                        + report.Created + "," + report.DisplayName + ","
                                        + report.IsEmployee + "," + report.IsFollowing + ","
                                        + report.IsMod + "," + report.IsNsfw + ","
                                        + report.PostKarma + "," + report.Url + ",");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "Activity Type, InteractionDateTime, Query Type, Query Value, InteractedUsername, Comment Karma,Created, Display Name, Is Employee, Is Following, Is Moderator, Is NSFW, Post Karma, URL ";

                    FollowReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.ActivityType + "," + report.InteractionDateTime + ","
                                        + report.QueryType + ","
                                        + report.QueryValue + "," + report.InteractedUsername + ","
                                        + report.CommentKarma + ","
                                        + report.Created + "," + report.DisplayName + ","
                                        + report.IsEmployee + "," + report.IsFollowing + ","
                                        + report.IsMod + "," + report.IsNsfw + ","
                                        + report.PostKarma + "," + report.Url + ",")
                                ;
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
            FollowReportModelAccount.Clear();
            IList reportDetails = dbAccountService.GetInteractedUsers(ActivityType.Follow).ToList();
            foreach (InteractedUsers report in reportDetails)
                FollowReportModelAccount.Add(new InteractedUsersReportModel
                {
                    Id = columnId++,
                    QueryType = report.QueryType,
                    ActivityType = report.ActivityType,
                    QueryValue = report.QueryValue,
                    InteractionDateTime = report.InteractionDateTime,
                    InteractedUsername = report.InteractedUsername,
                    CommentKarma = report.CommentKarma,
                    Created = report.Created,
                    DisplayName = report.DisplayName,
                    IsEmployee = report.IsEmployee,
                    IsFollowing = report.IsFollowing,
                    IsMod = report.IsMod,
                    IsNsfw = report.IsNsfw,
                    PostKarma = report.PostKarma,
                    Url = report.Url
                });
            return FollowReportModelAccount.Select(x => new
            {
                x.Id,
                x.ActivityType,
                x.QueryType,
                x.QueryValue,
                x.InteractionDateTime,
                x.InteractedUsername,
                x.CommentKarma,
                x.Created,
                x.DisplayName,
                x.IsEmployee,
                x.IsFollowing,
                x.IsMod,
                x.IsNsfw,
                x.PostKarma,
                x.Url
            }).ToList();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                FollowReportModelCampaign.Clear();

                #region Get data from InteractedUsers table and add to UnfollowerReportModel

                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedUsers>().ForEach(
                    report =>
                    {
                        FollowReportModelCampaign.Add(new InteractedUsersReportModel
                        {
                            Id = report.Id,
                            SinAccUsername = report.SinAccUsername,
                            QueryType = report.QueryType,
                            ActivityType = report.ActivityType,
                            QueryValue = report.QueryValue,
                            InteractionDateTime = report.InteractionDateTime,
                            InteractedUsername = report.InteractedUsername,
                            CommentKarma = report.CommentKarma,
                            Created = report.Created,
                            DisplayName = report.DisplayName,
                            IsEmployee = report.IsEmployee,
                            IsFollowing = report.IsFollowing,
                            IsMod = report.IsMod,
                            IsNsfw = report.IsNsfw,
                            PostKarma = report.PostKarma,
                            Url = report.Url
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "AccountFollowed ", ColumnBindingText = "SinAccUsername "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Query Type ", ColumnBindingText = "QueryType "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "ActivityType  ", ColumnBindingText = "ActivityType "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Query Value ", ColumnBindingText = "QueryValue "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Time ", ColumnBindingText = "InteractionDateTime "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Followed User ", ColumnBindingText = "InteractedUsername "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Comment Karma ", ColumnBindingText = "CommentKarma "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Created Time ", ColumnBindingText = "Created "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "DisplayName ", ColumnBindingText = "DisplayName "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isEmployee  ", ColumnBindingText = "IsEmployee "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isFollowing  ", ColumnBindingText = "IsFollowing "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isMod  ", ColumnBindingText = "IsMod "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isNSFW  ", ColumnBindingText = "IsNsfw "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "PostKarma ", ColumnBindingText = "PostKarma "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "URL ", ColumnBindingText = "Url "}
                    };

                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(FollowReportModelCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(FollowReportModelCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FollowModel>(activitySettings).SavedQueries;
        }
    }
}