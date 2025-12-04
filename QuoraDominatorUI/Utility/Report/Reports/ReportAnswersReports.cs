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

namespace QuoraDominatorUI.Utility.Report.Reports
{
    public class ReportAnswersReports : IQdReportFactory
    {
        private static readonly ObservableCollection<AnswersReport> AnswersReportModel =
            new ObservableCollection<AnswersReport>();

        private static readonly ObservableCollection<InteractedAnswer> LstAnswersReport =
            new ObservableCollection<InteractedAnswer>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ReportAnswerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            AnswersReportModel.Clear();
            try
            {
                #region get data from InteracteractedAnswers table and add to AnswersReportModel

                var activity = ActivityType.ReportAnswers.ToString();
                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetInteractedAnswers()?.Where(x => x.ActivityType == activity).ForEach(
                    report =>
                    {
                        AnswersReportModel.Add(new AnswersReport
                        {
                            Id = report.Id,
                            ActivityType = ActivityType.ReportAnswers.ToString(),
                            InteractionDateTime = report.InteractionDateTime,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            AnswersUrl = report.AnswersUrl,
                            AnsweredUserName = report.AnsweredUserName,
                            AccountUsername = report.Accountusername
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Answer Url", ColumnBindingText = "AnswersUrl"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Date", ColumnBindingText = "InteractionDateTime"}
                    };

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(AnswersReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            try
            {
                LstAnswersReport.Clear();
                IList reportDetails = dbAccountService.GetInteractedAnswers().ToList();

                foreach (InteractedAnswers item in reportDetails)
                    LstAnswersReport.Add(
                        new InteractedAnswer
                        {
                            Id = item.Id,
                            Date = item.InteractionDateTime,
                            QueryType = item.QueryType,
                            QueryValue = item.QueryValue,
                            AnswerUrl = item.AnswersUrl,
                            Answered = item.AnsweredUserName
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstAnswersReport;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "Answered User Name,Answers Url,QueryType,Query,Date";
                    AnswersReportModel.ForEach(report =>
                    {
                        csvData.Add(report.AnsweredUserName + "," + report.AnswersUrl + "," + report.QueryType +
                                    "," + report.QueryValue + "," + report.InteractionDateTime);
                    });
                }
                else
                {
                    Header = "Id,AnswersUrl,Answered,QueryType,Query,Date";
                    LstAnswersReport.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.AnswerUrl + "," + report.Answered + "," +
                                    report.QueryType + "," + report.QueryValue + "," + report.Date);
                    });
                }

                Utilities.ExportReports(fileName, Header, csvData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}