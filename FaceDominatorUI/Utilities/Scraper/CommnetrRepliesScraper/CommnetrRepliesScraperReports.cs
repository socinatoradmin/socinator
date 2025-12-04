using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FdReports;
using FaceDominatorCore.Interface;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.CommnetrRepliesScraper
{
    public class CommnetrRepliesScraperReports : IFdReportFactory
    {
        public static ObservableCollection<CommentRepliesReportModel> InteractedCommentModel =
            new ObservableCollection<CommentRepliesReportModel>();

        public static List<CommentRepliesReportModelAccountModel> AccountsInteractedComments =
            new List<CommentRepliesReportModelAccountModel>();

        private readonly string _activityType = ActivityType.CommentScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentRepliesScraperModel>(activitySettings).SavedQueries;
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedCommentReplies>(x => x.ActivityType == _activityType).ToList();

            AccountsInteractedComments.Clear();

            var id = 1;

            foreach (InteractedCommentReplies report in reportDetails)
            {
                AccountsInteractedComments.Add(
                    new CommentRepliesReportModelAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        CommentUrl = report.CommentUrl,
                        ReplyCommentText = WebUtility.HtmlDecode(report.ReplyCommentText)?.Replace(",", " ")
                            ?.Replace("\n", " "),
                        ReplyCommenterID = report.ReplyCommentId,
                        PostId = report.CommentPostUrl,
                        CommentTimeWithDate = report.ReplyCommentTimeWithDate,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    });

                id++;
            }

            return AccountsInteractedComments;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Comment Url,Comment Text,Commenter Id,Post Id,Comment Liker Count,Comment DateTime";
                Header = PostsReportHeader();
                InteractedCommentModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + ","
                                                        + report.QueryType + ","
                                                        + report.QueryValue + ","
                                                        + report.PostId + ","
                                                        + report.CommentUrl + ","
                                                        + report.ReplyCommentId + ","
                                                        + (string.IsNullOrEmpty(report.ReplyCommentText)
                                                            ? "NA"
                                                            : WebUtility.HtmlDecode(report.ReplyCommentText
                                                                ?.Replace(",", string.Empty)?.Replace("\r\n", " ")
                                                                ?.Replace("\n", " "))) + ","
                                                        + report.ReplyCommenterID + ","
                                                        + report.InteractionTimeStamp);
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
                //Header = "QueryType,QueryValue,Comment Url,Comment Text,Commenter Id,Post Id,Comment Liker Count,Comment DateTime";
                Header = PostsReportHeader(false);
                AccountsInteractedComments.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + ","
                                                     + report.QueryValue + ","
                                                     + report.PostId + ","
                                                     + report.CommentUrl + ","
                                                     + report.ReplyCommentId + ","
                                                     + (string.IsNullOrEmpty(report.ReplyCommentText)
                                                         ? "NA"
                                                         : WebUtility.HtmlDecode(report.ReplyCommentText
                                                             ?.Replace(",", string.Empty)?.Replace("\r\n", " ")
                                                             ?.Replace("\n", " "))) + ","
                                                     + report.ReplyCommenterID + ","
                                                     + report.InteractionTimeStamp);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedCommentModel.Clear();

            Header = "AccountEmail,QueryType,QueryValue,Comment Url,Reply Comment Text,Reply Commenter Id,Post Id,Date";

            #region get data from InteractedUsers table and add to UnfollowerReportModel

            dataBase
                .GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedCommentReplies>()
                .ForEach(
                    report =>
                    {
                        InteractedCommentModel.Add(new CommentRepliesReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            CommentUrl = report.CommentUrl,
                            ReplyCommentId = report.ReplyCommentId,
                            ReplyCommentText = WebUtility.HtmlDecode(report.ReplyCommentText).Replace(",", string.Empty)
                                .Replace("\r\n", " ").Replace("\n", " "),
                            ReplyCommenterID = report.ReplyCommenterId,
                            PostId = report.CommentPostUrl,
                            CommentTimeWithDate = report.ReplyCommentTimeWithDate,
                            InteractionTimeStamp = report.InteractionDateTime
                        });
                    });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            // {
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText =
                            $"{"LangKeyAccount".FromResourceDictionary()} {"LangKeyEmail".FromResourceDictionary()}",
                        ColumnBindingText = "AccountEmail"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryType".FromResourceDictionary(), ColumnBindingText = "QueryType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryValue".FromResourceDictionary(),
                        ColumnBindingText = "QueryValue"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentUrl".FromResourceDictionary(),
                        ColumnBindingText = "CommentUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyReplyCommentId".FromResourceDictionary(),
                        ColumnBindingText = "ReplyCommentId"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyReplyCommentText".FromResourceDictionary(),
                        ColumnBindingText = "ReplyCommentText"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommenterId".FromResourceDictionary(),
                        ColumnBindingText = "ReplyCommenterID"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostId".FromResourceDictionary(), ColumnBindingText = "PostId"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentDate".FromResourceDictionary(),
                        ColumnBindingText = "CommentTimeWithDate"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //});
            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedCommentModel);

            #endregion

            return new ObservableCollection<object>(InteractedCommentModel);
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyPostId");
            listResource.Add("LangKeyCommentUrl");
            listResource.Add("LangKeyReplyCommentId");
            listResource.Add("LangKeyReplyCommentText");
            listResource.Add("LangKeyCommenterId");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}