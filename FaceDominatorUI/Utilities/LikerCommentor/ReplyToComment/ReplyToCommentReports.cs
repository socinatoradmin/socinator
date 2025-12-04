using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDModel.LikerCommentorModel;
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

namespace FaceDominatorUI.Utilities.LikerCommentor.ReplyToComment
{
    public class ReplyToCommentReports : IFdReportFactory
    {
        public static ObservableCollection<CommentReportModel> InteractedCommentModel =
            new ObservableCollection<CommentReportModel>();

        public static List<CommentReportAccountModel>
            AccountsInteractedComments = new List<CommentReportAccountModel>();

        private readonly string _activityType = ActivityType.ReplyToComment.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ReplyToCommentModel>(activitySettings).SavedQueries;
        }

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedCommentModel.Clear();

            Header =
                "AccountEmail,QueryType,QueryValue,Comment Url,Comment Text,Commenter Id,ReplyFor Comment, User Mentions, Post Id,Comment DateTime,Comment Liker Count,Date";

            #region get data from InteractedUsers table and add to UnfollowerReportModel
            dataBase.GetAllInteractedData<InteractedComments>().ForEach(
                report =>
                {
                    InteractedCommentModel.Add(new CommentReportModel
                    {
                        Id = report.Id,
                        AccountEmail = report.AccountEmail,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        CommentUrl = report.CommentUrl,
                        CommentText = WebUtility.HtmlDecode(report.CommentText)?.Replace(",", " ")?.Replace("\n", " "),
                        CommenterId = report.CommenterId,
                        ReplyForComment = report.ReplyForComment,
                        Mention = string.IsNullOrEmpty(report.Mentions) ? "N/A" : report.Mentions,
                        PostId = report.CommentPostId,
                        ReactionType = report.HasLikedByUser,
                        CommentTimeWithDate = report.CommentTimeWithDate,
                        ReactionCountOnComment = report.CommetLikeCount,
                        ReactAsPageId = report.LikeAsPageId,
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
                        ColumnHeaderText = "LangKeyCommentText".FromResourceDictionary(),
                        ColumnBindingText = "CommentText"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommenterId".FromResourceDictionary(),
                        ColumnBindingText = "CommenterId"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyReplyForComment".FromResourceDictionary(),
                        ColumnBindingText = "ReplyForComment"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserMentions".FromResourceDictionary(), ColumnBindingText = "Mention"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostId".FromResourceDictionary(), ColumnBindingText = "PostId"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyReactionType".FromResourceDictionary(),
                        ColumnBindingText = "ReactionType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentDate".FromResourceDictionary(),
                        ColumnBindingText = "CommentTimeWithDate"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText =
                            $"{"LangKeyCommentLiker".FromResourceDictionary()} {"LangKeyCount".FromResourceDictionary()}",
                        ColumnBindingText = "ReactionCountOnComment"
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

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>(x =>
                    x.ActivityType == _activityType).ToList();

            AccountsInteractedComments.Clear();

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments report in reportDetails)
                AccountsInteractedComments.Add(
                    new CommentReportAccountModel
                    {
                        Id = report.Id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        CommentUrl = report.CommentUrl,
                        CommentText = WebUtility.HtmlDecode(report.CommentText)?.Replace(",", string.Empty)
                            ?.Replace("\r\n", " ")?.Replace("\n", " "),
                        CommenterId = report.CommenterId,
                        ReplyForComment = report.ReplyForComment,
                        Mention = string.IsNullOrEmpty(report.Mentions) ? "N/A" : report.Mentions,
                        PostId = report.CommentPostId,
                        ReactionType = report.HasLikedByUser,
                        CommentTimeWithDate = report.CommentTimeWithDate,
                        ReactionCountOnComment = report.CommetLikeCount,
                        ReactAsPageId = report.LikeAsPageId,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    });

            return AccountsInteractedComments;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Comment Url,Comment Text,Commenter Id,Reply For Comment, User Mention,Post Id,Reaction Type,Comment Liker Count,React as Page Id,Comment DateTime";
                Header = PostsReportHeader();
                InteractedCommentModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + "," + report.QueryType + ","
                                    + report.QueryValue + ","
                                    + $"{report.CommentUrl}" + ","
                                    + (string.IsNullOrEmpty(report.CommentText)
                                        ? "NA"
                                        : WebUtility.HtmlDecode(report.CommentText.Replace(",", string.Empty)
                                            .Replace("\r\n", " ").Replace("\n", " "))) + ","
                                    + report.CommenterId + ","
                                    + report.ReplyForComment.Replace(",", " ").Replace("\r\n", "") + ","
                                    + report.Mention + ","
                                    + report.PostId + ","
                                    + report.ReactionType + ","
                                    + report.ReactionCountOnComment + ","
                                    + report.ReactAsPageId + ","
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
                //Header = "QueryType,QueryValue,Comment Url,Comment Text,Commenter Id,Reply For Comment, User Mention,Post Id,Comment Liker Count,React as Page Id,Comment DateTime";
                Header = PostsReportHeader(false);
                AccountsInteractedComments.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + ","
                                                     + report.QueryValue + ","
                                                     + $"{report.CommentUrl}" + ","
                                                     + (string.IsNullOrEmpty(report.CommentText)
                                                         ? "NA"
                                                         : WebUtility.HtmlDecode(report.CommentText.Replace(",", " "))
                                                     ) + ","
                                                     + report.CommenterId + ","
                                                     + report.ReplyForComment.Replace(",", " ").Replace("\r\n", "") +
                                                     ","
                                                     + report.Mention + ","
                                                     + report.PostId + ","
                                                     + report.ReactionType + ","
                                                     + report.ReactionCountOnComment + ","
                                                     + report.ReactAsPageId + ","
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

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyCommentUrl");
            listResource.Add("LangKeyCommentText");
            listResource.Add("LangKeyCommenterId");
            listResource.Add("LangKeyReplyForComment");
            listResource.Add("LangKeyUserMentions");
            listResource.Add("LangKeyPostId");
            listResource.Add("LangKeyReactionType");
            listResource.Add("LangKeyCommentLikerCount");
            listResource.Add("LangKeyReactAsPageId");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}