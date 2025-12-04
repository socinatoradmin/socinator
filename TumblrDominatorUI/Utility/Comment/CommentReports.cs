using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorUI.Report;

namespace TumblrDominatorUI.Utility.Comment
{
    public class CommentReports : ITumblrReportFactory
    {
        public static ObservableCollection<CommentUserDetails> InteractedUsersModel =
            new ObservableCollection<CommentUserDetails>();

        private int _i;

        public ObservableCollection<InteractedPosts> AccountsInteractedUsers =
            new ObservableCollection<InteractedPosts>();

        public string Header { get; set; } = string.Empty;

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            var value = dataSelectionType.ToString();
            var reports = (ReportType)Enum.Parse(typeof(ReportType), value);

            #region Campaign reports

            if (reports == ReportType.Campaign)
            {
                Header = "Query Type, Query Value, Account Username, PostID, Comment, Date";

                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.AccountName + "," +
                                    report.ContentId.ToString() + "," + report.Comments + "," + report.Date);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reports == ReportType.Account)
            {
                Header = "Query Type, Query Value, PostID, Comment, Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," +
                                    report.ContentId + "," + report.Comments + "," + report.InteractionTimeStamp);
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
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            if (InteractedUsersModel.Count >= 0) InteractedUsersModel.Clear();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedPosts().ForEach(
                report =>
                {
                    InteractedUsersModel.Add(new CommentUserDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        ContentId = report.PostUrl,
                        Comments = report.Comments
                    });
                });

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "PostID", ColumnBindingText = "ContentId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Comments", ColumnBindingText = "Comments"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                };

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }


        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentModel>(activitySettings).SavedQueries;
        }

        public IList GetAccountReport(IDbAccountService dataBase)
        {
            try
            {
                IList reportDetails = dataBase.GetInteractedPosts(ActivityType.Comment).ToList();

                foreach (InteractedPosts report in reportDetails)
                {
                    _i = _i + 1;
                    AccountsInteractedUsers.Add(
                        new InteractedPosts
                        {
                            Id = _i,
                            ActivityType = report.ActivityType,
                            MediaType = report.MediaType,
                            QueryValue = report.QueryValue,
                            QueryType = report.QueryType,
                            ContentId = report.ContentId,
                            Comments = report.Comments,
                            InteractionTimeStamp =
                                report
                                    .InteractionTimeStamp // (DateTimeUtilities.EpochToDateTimeUtc(report.InteractionTimeStamp) + ForLocalTime)
                        });
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return AccountsInteractedUsers;
        }
    }
}