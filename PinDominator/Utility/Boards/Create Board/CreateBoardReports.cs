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

namespace PinDominator.Utility.Boards.Create_Board
{
    public class CreateBoardReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedBoardsReportDetails> InteractedBoardsReportModel =
            new ObservableCollection<InteractedBoardsReportDetails>();

        private readonly List<InteractedBoardsReportDetails> _accountsInteractedUsers =
            new List<InteractedBoardsReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BoardModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedBoardsReportModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedPosts table and add to InteractedPostsReportModel

            var id = 1;
            dbCampaignService.Get<InteractedBoards>(x => x.OperationType == ActivityType.CreateBoard).ForEach(
                report =>
                {
                    InteractedBoardsReportModel.Add(new InteractedBoardsReportDetails
                    {
                        Id = id++,
                        BoardId = report.BoardId,
                        SinAccUsername = report.SinAccUsername,
                        BoardName = report.BoardName,
                        InteractionDate =
                            (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        OperationType = report.OperationType,
                        UserId = report.SinAccId,
                        Username = report.SinAccUsername,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        BoardDescription = report.BoardDescription,
                        BoardSection=report.BoardSection
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Id", ColumnBindingText = "BoardId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Account Username", ColumnBindingText = "SinAccUsername"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Name", ColumnBindingText = "BoardName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Board Description", ColumnBindingText = "BoardDescription"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Board Section", ColumnBindingText = "BoardSection"}
                };
            //});

            #endregion

            return new ObservableCollection<object>(InteractedBoardsReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedBoards>()
                .Where(x => x.OperationType == ActivityType.CreateBoard).ToList();

            var id = 1;
            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedBoards item in reportDetails)
                _accountsInteractedUsers.Add(
                    new InteractedBoardsReportDetails
                    {
                        Id = id++,
                        Username = item.Username,
                        QueryType = item.QueryType,
                        Query = item.Query,
                        BoardId = item.BoardId,
                        BoardName = item.BoardName,
                        BoardDescription = item.BoardDescription,
                        InteractionDate =
                            (item.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        OperationType = item.OperationType,
                        UserId = item.UserId
                    });

            return _accountsInteractedUsers.Select(x =>
                new
                {
                    x.Id,
                    x.Username,
                    x.QueryType,
                    x.Query,
                    x.BoardId,
                    x.BoardName,
                    x.BoardDescription,
                    x.InteractionDate,
                    x.OperationType,
                    x.UserId
                }).ToList();
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header = "QueryType,Query,BoardId,BoardName,BoardDescription,InteractionDate,OperationType,UserId,Board Sections";

                _accountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.BoardId + "," +
                                    report.BoardName + "," +
                                    report.BoardDescription != null
                            ? report.BoardDescription.Replace(',', '.')
                            : null + "," +
                              report.InteractionDate + "," + report.OperationType + "," + report.UserId+","+report.BoardSection?.Replace(',', '.') + "/");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header =
                    "Username,QueryType,Query,BoardId,BoardName,BoardDescription,InteractionDate,OperationType,UserId,Board Sections";

                InteractedBoardsReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        report.BoardDescription = !string.IsNullOrEmpty(report.BoardDescription)
                            ? report.BoardDescription.Replace(',', '.')
                            : "NA";
                        report.Query = report.Query != null ? report.Query : "NA";

                        csvData.Add(report.Username + "," + report.QueryType + "," + report.Query + "," +
                                    report.BoardId + "," + report.BoardName + "," + report.BoardDescription + "," +
                                    report.InteractionDate + "," + report.OperationType + "," + report.UserId +","+report.BoardSection?.Replace(',', '.') + "/");
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