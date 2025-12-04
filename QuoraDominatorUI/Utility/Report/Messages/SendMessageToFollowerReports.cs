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
    public class SendMessageToFollowerReports : IQdReportFactory
    {
        private static readonly ObservableCollection<MessageReport> MessageReportModel =
            new ObservableCollection<MessageReport>();

        private static readonly ObservableCollection<InteractedMessageWithoutQuery> MessageAccountModel =
            new ObservableCollection<InteractedMessageWithoutQuery>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<SendMessageToFollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            MessageReportModel.Clear();
            try
            {
                #region get data from InteracteractedMessage table and add to FollowerReportModel

                var activity = ActivityType.SendMessageToFollower.ToString();
                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetInteractedMessage()?.Where(x => x.ActivityType == activity).ForEach(
                    report =>
                    {
                        MessageReportModel.Add(new MessageReport
                        {
                            Id = report.Id,
                            Account = report.SinAccUsername,
                            InteractionDateTime = DateTime.Now,
                            Message = report.Message,
                            UserName = report.Username
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "Account"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "UserName", ColumnBindingText = "UserName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Message", ColumnBindingText = "Message"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Date", ColumnBindingText = "InteractionDateTime"}
                    };

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(MessageReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            try
            {
                MessageAccountModel.Clear();
                IList reportDetails = dbAccountService.GetInteractedMessage().ToList();

                foreach (InteractedMessage item in reportDetails)
                    MessageAccountModel.Add(
                        new InteractedMessageWithoutQuery
                        {
                            Id = item.Id,
                            InteractionDateTime = DateTime.Now,
                            Message = item.Message,
                            UserName = item.Username
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return MessageAccountModel;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();


                #region Campaign reports

                if (dataSelectionType.ToString() == ReportType.Campaign.ToString())
                {
                    Header = "Account Name,Message,User Name,Date";
                    MessageReportModel.ForEach(report =>
                    {
                        csvData.Add(report.Account + "," + report.Message + "," + report.UserName + "," +
                                    report.InteractionDateTime);
                    });
                }

                #endregion

                #region Account reports

                else
                {
                    Header = "Id,User Name,Message,Date";
                    MessageAccountModel.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.UserName + "," + report.Message + "," +
                                    report.InteractionDateTime);
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