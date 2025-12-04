using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.Interface;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Report;

namespace PinDominator.Utility.Pin_Try_Comment.Comment
{
    public class CommentReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedPinsReportDetails> InteractedPostsReportModel =
            new ObservableCollection<InteractedPinsReportDetails>();

        private static readonly List<InteractedPinsReportDetails> LstInteractedPostAccount =
            new List<InteractedPinsReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;

        public CommentModel CommentModel { get; set; }
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            InteractedPostsReportModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            CommentModel = JsonConvert.DeserializeObject<CommentModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);

            #region get data from InteractedPosts table and add to InteractedPostsReportModel

            var id = 1;
            var commentAct = ActivityType.Comment.ToString();
            var likeCommentAct = ActivityType.LikeComment.ToString();
            var commentActInt = ((int) ActivityType.Comment).ToString();
            var likeCommentActInt = ((int) ActivityType.LikeComment).ToString();
            var activityTypePostComment = "Post" + ActivityType.Comment;

            dbCampaignService.Get<InteractedPosts>(x => x.OperationType == commentAct ||
                                                        x.OperationType == commentActInt ||
                                                        x.OperationType == activityTypePostComment ||
                                                        x.OperationType == likeCommentActInt ||
                                                        x.OperationType == likeCommentAct).ForEach(
                report =>
                {
                    InteractedPostsReportModel.Add(new InteractedPinsReportDetails
                    {
                        Id = id++,
                        OriginalPinId = report.PinId,
                        SinAccUsername = report.SinAccUsername,
                        MediaString = report.MediaString,
                        PinDescription = report.PinDescription,
                        TryCount = report.TryCount,
                        CommentCount = report.CommentCount,
                        SourceBoard = report.SourceBoard,
                        PinWebUrl = report.PinWebUrl,
                        SourceBoardName = report.SourceBoardName,
                        InteractionDate =
                            (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        MediaType = report.MediaType,
                        OperationType = report.OperationType,
                        UserId = report.UserId,
                        Username = report.Username,
                        Comment = report.Comment,
                        CommentId = report.CommentId,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        Status = report.OperationType == ActivityType.LikeComment.ToString() ||
                                 report.OperationType == likeCommentActInt ? "Liked Comment" :
                            report.OperationType == "Post" + ActivityType.Comment ? "Post Commented" : "Commented"
                    });
                });

            #endregion

            #region Generate Report for follow action

            if (CommentModel.IsChkFollowUserAfterComment)
            {
                dbCampaignService.Get<InteractedUsers>().ForEach(
                    report =>
                    {
                        InteractedPostsReportModel.Add(new InteractedPinsReportDetails
                        {
                            Id = id++,
                            Query = report.Query,
                            QueryType = report.QueryType,
                            SinAccUsername = report.SinAccUsername,
                            InteractionDate =
                                (report.InteractionTime.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                    .InvariantCulture),
                            OperationType = ActivityType.Follow.ToString(),
                            Username = report.InteractedUsername,
                            Status = "Followed"
                        });
                    });
            }

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Pin Id", ColumnBindingText = "OriginalPinId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Media String", ColumnBindingText = "MediaString"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Pin Description", ColumnBindingText = "PinDescription"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Try Count", ColumnBindingText = "TryCount"},
                    //new GridViewColumnDescriptor
                    //    {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Board Id", ColumnBindingText = "BoardId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "PinWeb Url", ColumnBindingText = "PinWebUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Name", ColumnBindingText = "SourceBoardName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Media Type", ColumnBindingText = "MediaType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Comment", ColumnBindingText = "Comment"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                };
            if (InteractedPostsReportModel.Any(x => x.OperationType == ActivityType.LikeComment.ToString()))
                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Pin Id", ColumnBindingText = "PinId"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Media String", ColumnBindingText = "MediaString"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Pin Description", ColumnBindingText = "PinDescription"},
                        //new GridViewColumnDescriptor {ColumnHeaderText = "Try Count", ColumnBindingText = "TryCount"},
                        //new GridViewColumnDescriptor
                        //    {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                        //new GridViewColumnDescriptor {ColumnHeaderText = "Board Id", ColumnBindingText = "BoardId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "PinWeb Url", ColumnBindingText = "PinWebUrl"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Board Name", ColumnBindingText = "SourceBoardName"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Media Type", ColumnBindingText = "MediaType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Comment", ColumnBindingText = "Comment"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Comment Id", ColumnBindingText = "CommentId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                    };

            #endregion

            return new ObservableCollection<object>(InteractedPostsReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstInteractedPostAccount.Clear();
            var actType = ActivityType.Comment.ToString();
            var commentActInt = ((int) ActivityType.Comment).ToString();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts>()
                .Where(x => x.OperationType == actType || x.OperationType == commentActInt).ToList();
            var id = 1;
            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts item in reportDetails)
                LstInteractedPostAccount.Add(
                    new InteractedPinsReportDetails
                    {
                        Id = id++,
                        Username = item.Username,
                        QueryType = item.QueryType,
                        Query = item.Query,
                        SourceBoard = item.SourceBoard,
                        SourceBoardName = item.SourceBoardName,
                        CommentCount = item.CommentCount,
                        InteractionDate =
                            (item.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        MediaString = item.MediaString,
                        MediaType = item.MediaType,
                        OperationType = item.OperationType,
                        PinDescription = item.PinDescription,
                        OriginalPinId = item.PinId,
                        PinWebUrl = item.PinWebUrl,
                        TryCount = item.TryCount,
                        UserId = item.UserId,
                        Comment = item.Comment
                    }
                );
            return LstInteractedPostAccount.Select(x =>
                new
                {
                    x.Id,
                    x.Username,
                    x.QueryType,
                    x.Query,
                    x.SourceBoard,
                    x.SourceBoardName,
                    x.CommentCount,
                    x.InteractionDate,
                    x.MediaString,
                    x.MediaType,
                    x.OperationType,
                    x.PinDescription,
                    x.OriginalPinId,
                    x.PinWebUrl,
                    x.TryCount,
                    x.UserId,
                    x.Comment
                }).ToList();
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header =
                    "Username,Query Type,Query,Interacted Username,Board Name,Interaction Date,Media String,Media Type," +
                    "Operation Type,Pin Description,Pin Id,Pin Web Url,Interacted User Id,Commented Text";

                InteractedPostsReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SinAccUsername + "," + report.QueryType + "," + report.Query + "," +
                                    report.Username + "," + report.SourceBoardName + "," + report.InteractionDate + "," +
                                    report.MediaString + "," + report.MediaType + "," + report.OperationType + "," +
                                    report.PinDescription?.Replace(',', '.') + "," +
                                    report.OriginalPinId + "," + report.PinWebUrl + ","+
                                    report.UserId + ","+report.Comment.Replace(","," ")+"");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header =
                    "Query Type,Query,Interacted Username,Board Id,Board Name,Comment Count,Interaction Date,Media String,Media Type," +
                    "Operation Type,Pin Description,Pin Id,Pin Web Url,Try Count,Interacted User Id";

                LstInteractedPostAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.Username + "," +
                                    report.SourceBoard + "," +
                                    report.SourceBoardName + "," + report.CommentCount + "," + report.InteractionDate +
                                    "," + report.MediaString + "," +
                                    report.MediaType + "," + report.OperationType + "," +
                                    report.PinDescription?.Replace(',', '.') + "," +
                                    report.OriginalPinId + "," + report.PinWebUrl + "," + report.TryCount + "," +
                                    report.UserId + "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }
}