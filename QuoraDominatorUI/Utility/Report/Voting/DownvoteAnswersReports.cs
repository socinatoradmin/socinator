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
    public class DownvoteAnswersReports : IQdReportFactory
    {
        public static readonly ObservableCollection<AnswersReport> AnswerScraperReportModel =
            new ObservableCollection<AnswersReport>();


        private static readonly ObservableCollection<InteractedAnswer> LastDownvotedAnswers =
            new ObservableCollection<InteractedAnswer>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AnswersScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            try
            {
                AnswerScraperReportModel.Clear();
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);

                #region get data from InteractedUsers table and add to FollowerReportModel

                dataBase.Get<InteractedAnswers>()?.ForEach(
                    report =>
                    {
                        AnswerScraperReportModel.Add(new AnswersReport
                        {
                            Id = report.Id,
                            ActivityType = ActivityType.DownvoteAnswers.ToString(),
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
                LastDownvotedAnswers.Clear();
                IList reportDetails = dbAccountService.GetInteractedAnswers(ActivityType.DownvoteAnswers).ToList();

                foreach (DominatorHouseCore.DatabaseHandler.QdTables.Accounts.InteractedAnswers report in reportDetails)
                    LastDownvotedAnswers.Add(
                        new InteractedAnswer
                        {
                            Id = report.Id,
                            Date = report.InteractionDateTime,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            AnswerUrl = report.AnswersUrl,
                            Answered = report.AnsweredUserName
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LastDownvotedAnswers;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "ActivityType,AccountName,Query,QueryType,AnswerUrl,Date";
                    AnswerScraperReportModel.ForEach(report =>
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.QueryValue + "," +
                                    report.QueryType + "," + report.AnswersUrl + "," + report.InteractionDateTime);
                    });
                }
                else
                {
                    Header = "Id,Query,QueryType,AnswerUrl,Answered,Date";
                    LastDownvotedAnswers.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryValue + "," +
                                    report.QueryType + "," + report.AnswerUrl + "," + report.Answered +
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