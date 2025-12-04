using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Reports;

namespace QuoraDominatorUI.Utility.Report.Scrape
{
    public class AnswersScraperReports : IQdReportFactory
    {
        private static readonly ObservableCollection<AnswersReport> AnswerScraperReportModel =
            new ObservableCollection<AnswersReport>();

        private static readonly ObservableCollection<InteractedAnswers> LstScrapedAnswers =
            new ObservableCollection<InteractedAnswers>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AnswersScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            AnswerScraperReportModel.Clear();
            try
            {
                #region get data from InteractedUsers table and add to FollowerReportModel

                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetInteractedAnswers()?.ForEach(
                    report =>
                    {
                        AnswerScraperReportModel.Add(new AnswersReport
                        {
                            Id = report.Id,
                            ActivityType = ActivityType.AnswersScraper.ToString(),
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
                            {ColumnHeaderText = "Answer Url", ColumnBindingText = "AnswersUrl"},
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

            return new ObservableCollection<object>(AnswerScraperReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            try
            {
                LstScrapedAnswers.Clear();

                IList reportDetails = dbAccountService.GetInteractedAnswers().ToList();
                foreach (DominatorHouseCore.DatabaseHandler.QdTables.Accounts.InteractedAnswers report in reportDetails)
                    LstScrapedAnswers.Add(
                        new InteractedAnswers
                        {
                            Id = report.Id,
                            ActivityType = ActivityType.AnswersScraper.ToString(),
                            InteractionDateTime = DateTime.Now,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            AnswersUrl = report.AnswersUrl
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstScrapedAnswers;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "Id,AccountName,Query Type,Query Value,AnswersUrl,Date";
                    AnswerScraperReportModel.ForEach(report =>
                    {
                        csvData.Add(report.Id + ","+report.AccountUsername+"," + report.QueryType + "," + report.QueryValue + "," +
                                    report.AnswersUrl + "," + report.InteractionDateTime);
                    });
                }
                else
                {
                    Header = "Id,AccountName,Query Type,Query Value,AnswersUrl,Date";
                    LstScrapedAnswers.ForEach(report =>
                    {
                        csvData.Add(report.Id + ","+report.Accountusername + ","+ report.QueryType + "," + report.QueryValue + "," +
                                    report.AnswersUrl + "," + report.InteractionDateTime);
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