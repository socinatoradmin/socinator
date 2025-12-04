using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Reports;
using QuoraDominatorCore.Reports.AccountConfigReport;

namespace QuoraDominatorUI.Utility.Report.Messages
{
    public class BroadcastMessagesReports : IQdReportFactory
    {
        private static readonly ObservableCollection<BroadCastMessageReport> BroadCastMessageReportModel =
            new ObservableCollection<BroadCastMessageReport>();

        private static readonly ObservableCollection<InteractedMessages> BroadCastMessageAccountModel =
            new ObservableCollection<InteractedMessages>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BroadcastMessagesModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            BroadCastMessageReportModel.Clear();

            try
            {
                #region get data from InteracteractedMessage table and add to FollowerReportModel

                var activity = ActivityType.BroadcastMessages.ToString();

                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetInteractedMessage()?.Where(x => x.ActivityType == activity).ForEach(
                    report =>
                    {
                        BroadCastMessageReportModel.Add(new BroadCastMessageReport
                        {
                            Id = report.Id,
                            Account = report.SinAccUsername,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            InteractionDateTime = report.InteractionDate,
                            Message = report.Message,
                            UserName = report.Username
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
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "Account"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "UserName", ColumnBindingText = "UserName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Message", ColumnBindingText = "Message"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Date", ColumnBindingText = "InteractionDateTime"}
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(BroadCastMessageReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            BroadCastMessageAccountModel.Clear();
            try
            {
                IList reportDetails = dbAccountService.GetInteractedMessage().ToList();
                foreach (InteractedMessage report in reportDetails)
                    BroadCastMessageAccountModel.Add(new InteractedMessages
                    {
                        Id = report.Id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        InteractionDateTime = report.InteractionDate,
                        Message = report.Message,
                        MessagedUserName = report.Username
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return BroadCastMessageAccountModel;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();

                #region Campaign reports

                if (dataSelectionType.ToString() == ReportType.Campaign.ToString())
                {
                    Header = "Account Name,Query Type,Query,User Name,Message,Date";
                    BroadCastMessageReportModel.ForEach(report =>
                    {
                        csvData.Add(report.Account + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.UserName + "," +
                                    report.Message + "," + report.InteractionDateTime);
                    });
                }

                #endregion

                #region Account reports

                else
                {
                    Header = "Id,Query Type,Query,User Name,Message,Date";
                    BroadCastMessageAccountModel.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," +
                                    report.QueryType + "," + report.QueryValue + "," + report.MessagedUserName + "," +
                                    report.Message + "," + report.InteractionDateTime);
                    });
                }

                Utilities.ExportReports(fileName, Header, csvData);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}