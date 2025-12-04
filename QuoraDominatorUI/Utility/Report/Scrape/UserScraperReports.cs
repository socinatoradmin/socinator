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

namespace QuoraDominatorUI.Utility.Report.Scrape
{
    public class UserScraperReports : IQdReportFactory
    {
        private static readonly ObservableCollection<UserScraperReport> UserScraperReportModel =
            new ObservableCollection<UserScraperReport>();

        private static readonly ObservableCollection<InteractedUser> LstScrapedUsers =
            new ObservableCollection<InteractedUser>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UserScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            try
            {
                UserScraperReportModel.Clear();
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);

                #region get data from InteractedUsers table and add to FollowerReportModel

                dataBase.Get<InteractedUsers>()?.ForEach(
                    report =>
                    {
                        UserScraperReportModel.Add(new UserScraperReport
                        {
                            Id = report.Id,
                            AccountName = report.SinAccUsername,
                            Date = report.InteractionDateTime,
                            QueryType = report.QueryType,
                            Query = report.QueryValue,
                            Username = report.InteractedUsername,
                            FollowerCount = report.FollowersCount,
                            FollowingCount = report.FollowingsCount,
                            QuestionCount = report.QuestionCount,
                            AnswerCount = report.AnswerCount,
                            PostCount = report.PostCount
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
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Follower Count", ColumnBindingText = "FollowerCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Following Count", ColumnBindingText = "FollowingCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Question Count", ColumnBindingText = "QuestionCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Answer Count", ColumnBindingText = "AnswerCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Post Count", ColumnBindingText = "PostCount"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "Username"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(UserScraperReportModel);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            try
            {
                LstScrapedUsers.Clear();
                IList reportDetails = dbAccountService.GetInteractedUsers(ActivityType.UserScraper).ToList();
                if (reportDetails.Count == 0)
                    return new List<InteractedUser>();

                foreach (DominatorHouseCore.DatabaseHandler.QdTables.Accounts.InteractedUsers item in reportDetails)
                    LstScrapedUsers.Add(
                        new InteractedUser
                        {
                            Id = item.Id,
                            Date = item.Date.EpochToDateTimeUtc(),
                            QueryType = item.QueryType,
                            Query = item.Query,
                            InteractedUsername = item.InteractedUsername
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstScrapedUsers;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName,QueryType,Query,Follower Count,Following Count,Question Count,Answer Count,Post Count,UserName,Date";
                    UserScraperReportModel.ForEach(report =>
                    {
                        csvData.Add(report.AccountName + "," + report.QueryType + "," + report.Query + "," +
                                   report.FollowerCount + "," + report.FollowingCount + "," + report.QuestionCount + "," + report.AnswerCount + "," + report.PostCount + "," + report.Username + "," + report.Date);
                    });
                }
                else
                {
                    Header = "Id,QueryType,Query,UserName,Date";
                    LstScrapedUsers.ForEach(report =>
                    {
                        csvData.Add(report.Id + "," + report.QueryType + "," + report.Query + "," +
                                    report.InteractedUsername + "," + report.Date);
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