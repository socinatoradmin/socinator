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

namespace QuoraDominatorUI.Utility.Report.Answer
{
    public class AnswerOnQuestionReports : IQdReportFactory
    {
        private static readonly ObservableCollection<AnswerOnQuestionReport> AnswerOnQuestionReport =
            new ObservableCollection<AnswerOnQuestionReport>();

        private static readonly ObservableCollection<InteractedAnswer> LstofAnswerOnQuestion =
            new ObservableCollection<InteractedAnswer>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AnswerQuestionModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            AnswerOnQuestionReport.Clear();

            try
            {
                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

                #region get data from InteractedUsers table and add to FollowerReportModel

                dbCampaignService.GetInteractedAnswers()?.ForEach(
                    report =>
                    {
                        AnswerOnQuestionReport.Add(new AnswerOnQuestionReport
                        {
                            Id = report.Id,
                            AccountName = report.Accountusername,
                            Date = report.InteractionDateTime,
                            QueryType = report.QueryType,
                            Query = report.QueryValue,
                            Username = report.AnsweredUserName,
                            AnswerUrl = report.AnswersUrl
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Answers Url", ColumnBindingText = "AnswerUrl"}
                    };

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(AnswerOnQuestionReport);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            LstofAnswerOnQuestion.Clear();

            try
            {
                IList reportDetails = dbAccountService.GetInteractedAnswers(ActivityType.AnswerOnQuestions).ToList();

                foreach (InteractedAnswers item in reportDetails)
                    LstofAnswerOnQuestion.Add(new InteractedAnswer
                        {
                            Id = item.Id,
                            Date = item.InteractionDateTime,
                            QueryType = item.QueryType,
                            QueryValue = item.QueryValue,
                            AnswerUrl = item.AnswersUrl,
                            Answered = item.Accountusername
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstofAnswerOnQuestion;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName,Query Type,Query value,Answers Url,Date";
                    AnswerOnQuestionReport.ForEach(report =>
                    {
                        csvData.Add(report.AccountName + "," + report.QueryType + "," + report.Query + "," +
                                    report.AnswerUrl + "," + report.Date);
                    });
                }
                else
                {
                    Header = "Id,Query Type,Query value,Answers Url,Date";
                    LstofAnswerOnQuestion.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.AnswerUrl + "," + report.Date);
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