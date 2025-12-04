using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.Interface;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Report;

namespace PinDominator.Utility.PinPoster.DeletePin
{
    public class DeletePinReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedPinsReportDetails> LstInteractedPinsCampaign =
            new ObservableCollection<InteractedPinsReportDetails>();

        private static readonly List<InteractedPinsReportDetails> LstInteractedPinAccount =
            new List<InteractedPinsReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<DeletePinModel>(activitySettings).SavedQueries;
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header =
                    "Username,Query Type,Query,Interacted Username,Board Id,Board Name,Comment Count,Interaction Date,Media String,Media Type," +
                    "Operation Type,Pin Description,Pin Id,Pin Web Url,Try Count,Interacted User Id";

                LstInteractedPinsCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SinAccUsername + "," + report.QueryType + "," + report.Query + "," +
                                    report.Username + "," +
                                    report.SourceBoard + "," + report.SourceBoardName + "," + report.CommentCount +
                                    "," + report.InteractionDate + "," +
                                    report.MediaString + "," + report.MediaType + "," + report.OperationType + "," +
                                    report.PinDescription?.Replace(',', '.') + ",'" +
                                    report.OriginalPinId + "'," + report.PinWebUrl + "," + report.TryCount + ",'" +
                                    report.UserId + "'");
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

                LstInteractedPinAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.Username + "," +
                                    report.SourceBoard + "," +
                                    report.SourceBoardName + "," + report.CommentCount + "," + report.InteractionDate +
                                    "," + report.MediaString + "," +
                                    report.MediaType + "," + report.OperationType + "," +
                                    report.PinDescription?.Replace(',', '.') + ",'" +
                                    report.OriginalPinId + "'," + report.PinWebUrl + "," + report.TryCount + ",'" +
                                    report.UserId + "'");
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

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            LstInteractedPinsCampaign.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedPosts table and add to InteractedPostsReportModel

            var id = 1;
            var actDeletePin = ActivityType.DeletePin.ToString();
            var deleteActInt = ((int) ActivityType.Delete).ToString();
            dbCampaignService.Get<InteractedPosts>(x =>
                x.OperationType == actDeletePin || x.OperationType == deleteActInt).ForEach(
                report =>
                {
                    LstInteractedPinsCampaign.Add(new InteractedPinsReportDetails
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
                        Query = report.Query,
                        QueryType = report.QueryType
                    });
                });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Try Count", ColumnBindingText = "TryCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Id", ColumnBindingText = "BoardId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "PinWeb Url", ColumnBindingText = "PinWebUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Board Name", ColumnBindingText = "SourceBoardName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Media Type", ColumnBindingText = "MediaType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"}
                };
            //});

            #endregion

            return new ObservableCollection<object>(LstInteractedPinsCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            var actType = ActivityType.DeletePin.ToString();
            var deleteActInt = ((int) ActivityType.Delete).ToString();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts>()
                .Where(x => x.OperationType == actType || x.OperationType == deleteActInt).ToList();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts item in reportDetails)
                LstInteractedPinAccount.Add(
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
                        UserId = item.UserId
                    }
                );
            return LstInteractedPinAccount.Select(x =>
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
                    x.UserId
                }).ToList();
        }
    }
}