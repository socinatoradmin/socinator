using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.ReportModel;
using RedditDominatorCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace RedditDominatorUI.Utility.Messanger
{
    class AutoReplyReports : IRdReportFactory
    {

        public static ObservableCollection<InteractedUsersReportModel> AutoReplyReportModelCampaign =
            new ObservableCollection<InteractedUsersReportModel>();

        private static List<InteractedUsersReportModel> AutoReplyReportModelAccount { get; } =
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
                        "Account, Activity Type,InteractionDateTime,  Query Type, Query Value, InteractedUsername, Message ,Comment Karma,Created, Display Name, Is Following, Is Moderator, Is NSFW, Post Karma, URL ";

                    AutoReplyReportModelCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.SinAccUsername + "," + report.ActivityType + "," +
                                        report.InteractionDateTime + ","
                                        + report.QueryType + ","
                                        + report.QueryValue + "," + report.InteractedUsername + ","
                                        + report.Message + ","
                                        + report.CommentKarma + ","
                                        + report.Created + "," + report.DisplayName + ","
                                        + report.IsFollowing + ","
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
                        "Account, Activity Type,InteractionDateTime,  Query Type, Query Value, InteractedUsername, Message ,Comment Karma,Created, Display Name,  Is Following, Is Moderator, Is NSFW, Post Karma, URL ";

                    AutoReplyReportModelAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.SinAccUsername + "," + report.ActivityType + "," +
                                        report.InteractionDateTime + ","
                                        + report.QueryType + "," + report.QueryValue + "," + report.InteractedUsername +
                                        ","
                                        + report.Message + "," + report.CommentKarma + ","
                                        + report.Created + "," + report.DisplayName + ","
                                        + report.IsFollowing + ","
                                        + report.IsMod + "," + report.IsNsfw + ","
                                        + report.PostKarma + "," + report.Url + ",");
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
            Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            var columnId = 1;
            AutoReplyReportModelAccount.Clear();

            IList reportDetails = dbAccountService.GetInteractedUsers(ActivityType.AutoReplyToNewMessage).ToList();
            foreach (InteractedUsers report in reportDetails)
                AutoReplyReportModelAccount.Add(new InteractedUsersReportModel
                {
                    Id = columnId++,
                    ActivityType = report.ActivityType,
                    InteractionDateTime = report.InteractionDateTime,
                    InteractedUsername = report.InteractedUsername,
                    Message = report.Message,
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
            return AutoReplyReportModelAccount.Select(x => new
            {
                x.Id,
                x.ActivityType,
                x.InteractionDateTime,
                x.InteractedUsername,
                x.Message,
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

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                AutoReplyReportModelCampaign.Clear();

                #region Get data from InteractedUsers table and add to UnfollowerReportModel

                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedUsers>().ForEach(
                    report =>
                    {
                        AutoReplyReportModelCampaign.Add(new InteractedUsersReportModel
                        {
                            Id = report.Id,
                            ActivityType = report.ActivityType,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            InteractionDateTime = report.InteractionDateTime,
                            InteractedUsername = report.InteractedUsername,
                            Message = report.Message,
                            CommentKarma = report.CommentKarma,
                            Created = report.Created,
                            DisplayName = report.DisplayName,
                            IsEmployee = report.IsEmployee,
                            IsFollowing = report.IsFollowing,
                            IsMod = report.IsMod,
                            IsNsfw = report.IsNsfw,
                            PostKarma = report.PostKarma,
                            Url = report.Url,
                            SinAccUsername = report.SinAccUsername
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account Name ", ColumnBindingText = "SinAccUsername "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "ActivityType  ", ColumnBindingText = "ActivityType "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Time ", ColumnBindingText = "InteractionDateTime "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "User ", ColumnBindingText = "InteractedUsername "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Message", ColumnBindingText = "Message"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Comment Karma ", ColumnBindingText = "CommentKarma "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Created Time ", ColumnBindingText = "Created "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "DisplayName ", ColumnBindingText = "DisplayName "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isFollowing  ", ColumnBindingText = "IsFollowing "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isMod  ", ColumnBindingText = "IsMod "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isNSFW  ", ColumnBindingText = "IsNsfw "},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "PostKarma ", ColumnBindingText = "PostKarma "},
                        new GridViewColumnDescriptor {ColumnHeaderText = "URL ", ColumnBindingText = "Url "}
                    };

                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(AutoReplyReportModelCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(AutoReplyReportModelCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return new ObservableCollection<QueryInfo>();
        }
    }
}
