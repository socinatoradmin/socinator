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

namespace PinDominator.Utility.PinPoster.RePin
{
    public class RePinReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedPinsReportDetails> LstInteractedPinsCampaign =
            new ObservableCollection<InteractedPinsReportDetails>();

        private static readonly List<InteractedPinsReportDetails> LstInteractedPinAccount =
            new List<InteractedPinsReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;

        public RePinModel RePinModel { get; set; }
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<RePinModel>(activitySettings).SavedQueries;
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header = "Username,Query Type,Query,Interacted Username,Source Board Id,Source Board Name," +
                         "Destination Board Id, Comment Count,Interaction Date,Media String,Media Type," +
                         "Operation Type,Pin Description,Pin Id,Pin Web Url,Interacted User Id";

                LstInteractedPinsCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.SinAccUsername + "," + report.QueryType + "," + report.Query + "," +
                                    report.Username + "," + report.SourceBoard + "," + report.SourceBoardName + "," +
                                    report.DestinationBoard + "," + report.CommentCount + "," + report.InteractionDate +
                                    "," +
                                    report.MediaString + "," + report.MediaType + "," + report.OperationType + "," +
                                    report.PinDescription?.Replace(',', '.') + ",'" +
                                    report.OriginalPinId + "'," + report.PinWebUrl + ",'" +
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
                Header = "Query Type,Query,Interacted Username,Source Board Id,Source Board Name," +
                         "Destination Board Id,Comment Count,Interaction Date,Media String,Media Type," +
                         "Operation Type,Pin Description,Pin Id,Pin Web Url,Interacted User Id";

                LstInteractedPinAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.Username + "," +
                                    report.SourceBoard + "," + report.SourceBoardName + "," + report.DestinationBoard +
                                    "," +
                                    report.CommentCount + "," + report.InteractionDate + "," + report.MediaString +
                                    "," +
                                    report.MediaType + "," + report.OperationType + "," +
                                    report.PinDescription?.Replace(',', '.') + ",'" +
                                    report.OriginalPinId + "'," + report.PinWebUrl + "," + ",'" +
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
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            RePinModel = JsonConvert.DeserializeObject<RePinModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);

            #region get data from InteractedPosts table and add to InteractedPostsReportModel

            var id = 1;
            var actRepin = ActivityType.Repin.ToString();
            var rePinActInt = ((int) ActivityType.Repin).ToString();
            dbCampaignService.Get<InteractedPosts>(x =>
                x.OperationType == actRepin || x.OperationType == rePinActInt).ForEach(
                report =>
                {
                    LstInteractedPinsCampaign.Add(new InteractedPinsReportDetails
                    {
                        Id = id++,
                        OriginalPinId = report.PinId,
                        GeneratedPinId = report.GeneratedPinId,
                        SinAccUsername = report.SinAccUsername,
                        //BoardLabel = report.BoardLabel,
                        MediaString = report.MediaString,
                        PinDescription = report.PinDescription,
                        //TryCount = report.TryCount,
                        CommentCount = report.CommentCount,
                        SourceBoard = report.SourceBoard,
                        PinWebUrl = report.PinWebUrl,
                        SourceBoardName = report.SourceBoardName,
                        DestinationBoard = report.DestinationBoard,
                        InteractionDate =
                            (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        MediaType = report.MediaType,
                        OperationType = report.OperationType,
                        UserId = report.UserId,
                        Username = report.Username,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        Status = "Repinned"
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
                        {ColumnHeaderText = "Generated Pin Id", ColumnBindingText = "GeneratedPinId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Board Label", ColumnBindingText = "BoardLabel"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Media String", ColumnBindingText = "MediaString"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Pin Description", ColumnBindingText = "PinDescription"},
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Try Count", ColumnBindingText = "TryCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Source Board Id", ColumnBindingText = "SourceBoard"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Source Board Name", ColumnBindingText = "SourceBoardName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Destination Board", ColumnBindingText = "DestinationBoard"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "PinWeb Url", ColumnBindingText = "PinWebUrl"},
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

            #region Generate Report for Try and Comment activity after follow action

            if (RePinModel.ChkTryOnPinAfterRepinChecked || RePinModel.ChkCommentOnPinAfterRepinChecked)
            {
                var status = string.Empty;
                dbCampaignService.Get<InteractedPosts>().ForEach(
                    report =>
                    {
                        var tryActInt = ((int) ActivityType.Try).ToString();
                        var commentActInt = ((int) ActivityType.Comment).ToString();
                        if (report.OperationType == "Try" || report.OperationType == tryActInt)
                            status = "Tried";
                        else if (report.OperationType == "Comment" || report.OperationType == commentActInt)
                            status = "Commented";
                        if (!string.IsNullOrEmpty(status))
                            LstInteractedPinsCampaign.Add(new InteractedPinsReportDetails
                            {
                                Id = id++,
                                OriginalPinId = report.PinId,
                                SinAccUsername = report.SinAccUsername,
                                BoardLabel = report.BoardLabel,
                                MediaString = report.MediaString,
                                PinDescription = report.PinDescription,
                                //TryCount = report.TryCount,
                                CommentCount = report.CommentCount,
                                SourceBoard = report.SourceBoard,
                                PinWebUrl = report.PinWebUrl,
                                SourceBoardName = report.SourceBoardName,
                                DestinationBoard = report.DestinationBoard,
                                InteractionDate =
                                    (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                        .InvariantCulture),
                                MediaType = report.MediaType,
                                OperationType = report.OperationType,
                                UserId = report.UserId,
                                Username = report.Username,
                                Query = report.Query,
                                QueryType = report.QueryType,
                                Status = status
                            });
                        status = null;
                    });

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Pin Id", ColumnBindingText = "PinId"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                        //new GridViewColumnDescriptor
                        //    {ColumnHeaderText = "Board Label", ColumnBindingText = "BoardLabel"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Media String", ColumnBindingText = "MediaString"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Pin Description", ColumnBindingText = "PinDescription"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Try Count", ColumnBindingText = "TryCount"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Source Board Id", ColumnBindingText = "SourceBoard"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Source Board Name", ColumnBindingText = "SourceBoardName"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Destination Board", ColumnBindingText = "DestinationBoard"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "PinWeb Url", ColumnBindingText = "PinWebUrl"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Media Type", ColumnBindingText = "MediaType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                    };
            }

            #endregion

            return new ObservableCollection<object>(LstInteractedPinsCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            LstInteractedPinAccount.Clear();
            var actType = ActivityType.Repin.ToString();
            var rePinActInt = ((int) ActivityType.Repin).ToString();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts>()
                .Where(x => x.OperationType == actType || x.OperationType == rePinActInt).ToList();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts item in reportDetails)
                LstInteractedPinAccount.Add(
                    new InteractedPinsReportDetails
                    {
                        Id = id++,
                        Username = item.Username,
                        QueryType = item.QueryType,
                        Query = item.Query,
                        BoardLabel = item.BoardLabel,
                        SourceBoard = item.SourceBoard,
                        SourceBoardName = item.SourceBoardName,
                        DestinationBoard = item.DestinationBoard,
                        CommentCount = item.CommentCount,
                        InteractionDate =
                            (item.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        MediaString = item.MediaString,
                        MediaType = item.MediaType,
                        OperationType = item.OperationType,
                        PinDescription = item.PinDescription,
                        OriginalPinId = item.PinId,
                        GeneratedPinId = item.GeneratedPinId,
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
                    x.BoardLabel,
                    x.SourceBoard,
                    x.SourceBoardName,
                    x.DestinationBoard,
                    x.InteractionDate,
                    x.MediaString,
                    x.MediaType,
                    x.OperationType,
                    x.PinDescription,
                    x.OriginalPinId,
                    x.GeneratedPinId,
                    x.PinWebUrl,
                    x.TryCount,
                    x.UserId
                }).ToList();
        }
    }
}