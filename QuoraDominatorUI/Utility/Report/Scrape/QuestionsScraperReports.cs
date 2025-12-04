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

namespace QuoraDominatorUI.Utility.Report.Scrape
{
    public class QuestionsScraperReports : IQdReportFactory
    {
        private static readonly ObservableCollection<QuestionReport> QuestionScraperReportModel =
            new ObservableCollection<QuestionReport>();

        private static readonly ObservableCollection<InteractedQuestions> LstScrapedQuestions =
            new ObservableCollection<InteractedQuestions>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<QuestionsScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            try
            {
                QuestionScraperReportModel.Clear();

                #region get data from InteractedUsers table and add to FollowerReportModel

                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetInteractedQuestion()?.ForEach(
                    report =>
                    {
                        QuestionScraperReportModel.Add(new QuestionReport
                        {
                            Id = report.Id,
                            ActivityType = ActivityType.QuestionsScraper.ToString(),
                            InteractionDateTime =report.InteractionDateTime==null ? DateTime.Now:report.InteractionDateTime,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            QuestionUrl = report.QuestionUrl,
                            AccountUsername = report.Accountusername
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
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Question Url", ColumnBindingText = "QuestionUrl"},
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

            return new ObservableCollection<object>(QuestionScraperReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            try
            {
                LstScrapedQuestions.Clear();
                IList reportDetails = dbAccountService.GetInteractedQuestion().ToList();
                foreach (InteractedQuestion report in reportDetails)
                    LstScrapedQuestions.Add(
                        new InteractedQuestions
                        {
                            Id = report.Id,
                            Date = DateTime.Now,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            QuestionUrl = report.QuestionUrl
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstScrapedQuestions;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName,QueryType,Query Value,QuestionUrl,Date";
                    QuestionScraperReportModel.ForEach(report =>
                    {
                        csvData.Add(report.AccountUsername + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.QuestionUrl +
                                    "," + report.InteractionDateTime);
                    });
                }
                else
                {
                    Header = "Id,QueryType,Query Value,QuestionUrl,Date";
                    LstScrapedQuestions.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.QuestionUrl +
                                    "," + report.Date);
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