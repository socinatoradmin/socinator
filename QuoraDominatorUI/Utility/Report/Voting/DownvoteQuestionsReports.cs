using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Reports;
using QuoraDominatorCore.Reports.AccountConfigReport;

namespace QuoraDominatorUI.Utility.Report.Voting
{
    public class DownvoteQuestionsReports : IQdReportFactory
    {
        private static readonly ObservableCollection<QuestionReport> QuestionScraperReportModel =
            new ObservableCollection<QuestionReport>();

        private static readonly ObservableCollection<InteractedQuestions> LastDownvotedQuestions =
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
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);

                #region get data from InteractedUsers table and add to FollowerReportModel

                dataBase.Get<InteractedQuestion>()?.ForEach(
                    report =>
                    {
                        QuestionScraperReportModel.Add(new QuestionReport
                        {
                            Id = report.Id,
                            ActivityType = ActivityType.DownvoteQuestions.ToString(),
                            InteractionDateTime = report.InteractionDateTime,
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
                LastDownvotedQuestions.Clear();
                IList reportDetails = dbAccountService.GetInteractedQuestion(ActivityType.DownvoteQuestions).ToList();
                foreach (DominatorHouseCore.DatabaseHandler.QdTables.Accounts.InteractedQuestion report in reportDetails
                )
                    LastDownvotedQuestions.Add(
                        new InteractedQuestions
                        {
                            Id = report.Id,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            QuestionUrl = report.QuestionUrl,
                            Date = report.InteractionDateTime
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LastDownvotedQuestions;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName,QueryType,Query,Date,QuestionUrl";
                    QuestionScraperReportModel.ForEach(report =>
                    {
                        csvData.Add(report.AccountUsername + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.InteractionDateTime + "," + report.QuestionUrl);
                    });
                }
                else
                {
                    Header = "ID,QueryType,Query,Date,QuestionUrl";
                    LastDownvotedQuestions.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryType + "," + report.QueryValue + "," +
                                    report.Date + "," + report.QuestionUrl);
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