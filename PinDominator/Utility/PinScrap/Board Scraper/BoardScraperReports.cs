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

namespace PinDominator.Utility.PinScrap.Board_Scraper
{
    public class BoardScraperReports : IPdReportFactory
    {
        public static ObservableCollection<InteractedBoardsReportDetails> InteractedBoardsReportModel =
            new ObservableCollection<InteractedBoardsReportDetails>();

        private static readonly List<InteractedBoardsReportDetails> LstInteractedBoardAccount =
            new List<InteractedBoardsReportDetails>();

        private readonly TimeSpan _forLocalTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BoardScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            InteractedBoardsReportModel.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedPosts table and add to InteractedPostsReportModel

            var id = 1;
            dbCampaignService.Get<InteractedBoards>(x => x.OperationType == ActivityType.BoardScraper).ForEach(
                report =>
                {
                    InteractedBoardsReportModel.Add(new InteractedBoardsReportDetails
                    {
                        Id = id++,
                        BoardId = report.BoardId,
                        SinAccUsername = report.SinAccUsername,
                        BoardName = report.BoardName,
                        BoardUrl = report.BoardUrl,
                        InteractionDate =
                            (report.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        OperationType = report.OperationType,
                        UserId = report.UserId,
                        Username = report.Username,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        BoardDescription = report.BoardDescription,
                        FollowerCount = report.FollowerCount,
                        PinCount = report.PinCount
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Board Url", ColumnBindingText = "BoardUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "User Id", ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Board Description", ColumnBindingText = "BoardDescription"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Follower Count", ColumnBindingText = "FollowerCount"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "PinCount", ColumnBindingText = "PinCount"}
                };
            //});

            #endregion

            return new ObservableCollection<object>(InteractedBoardsReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedBoards>()
                .Where(x => x.OperationType == ActivityType.BoardScraper).ToList();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedBoards item in reportDetails)
                LstInteractedBoardAccount.Add(
                    new InteractedBoardsReportDetails
                    {
                        Id = id++,
                        Username = item.Username,
                        QueryType = item.QueryType,
                        Query = item.Query,
                        BoardId = item.BoardId,
                        BoardName = item.BoardName,
                        BoardUrl = item.BoardUrl,
                        BoardDescription = item.BoardDescription,
                        InteractionDate =
                            (item.InteractionDate.EpochToDateTimeUtc() + _forLocalTime).ToString(CultureInfo
                                .InvariantCulture),
                        PinCount = item.PinCount,
                        FollowerCount = item.FollowerCount,
                        OperationType = item.OperationType,
                        UserId = item.UserId
                    }
                );
            return LstInteractedBoardAccount.Select(x =>
                new
                {
                    x.Id,
                    x.Username,
                    x.QueryType,
                    x.Query,
                    x.BoardId,
                    x.BoardName,
                    x.BoardUrl,
                    x.BoardDescription,
                    x.InteractionDate,
                    x.PinCount,
                    x.FollowerCount,
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
                Header =
                    "Username,QueryType,Query,BoardId,BoardName,BoardUrl,BoardDescription,InteractionDate,PinCount,FollowerCount,OperationType,UserId";

                LstInteractedBoardAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Username + "," + report.QueryType + "," + report.Query + "," +
                                    report.BoardId + "," + report.BoardName + "," + report.BoardUrl + "," +
                                    report.BoardDescription?.Replace(',', '.') + "," + report.InteractionDate + "," +
                                    report.PinCount + "," + report.FollowerCount + "," + report.OperationType + ",'" +
                                    report.UserId + "'");
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
                    "Username,QueryType,Query,Interacted Username,BoardId,BoardName,BoardUrl,BoardDescription,InteractionDate,PinCount,FollowerCount,OperationType,UserId";

                InteractedBoardsReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Username + "," + report.QueryType + "," + report.Query + "," +
                                    report.Username + "," + report.BoardId + "," +
                                    report.BoardName + "," + report.BoardUrl + "," +
                                    report.BoardDescription?.Replace(',', '.') + "," +
                                    report.InteractionDate + "," + report.PinCount + "," + report.FollowerCount + "," +
                                    report.OperationType + ",'" + report.UserId + "'");
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