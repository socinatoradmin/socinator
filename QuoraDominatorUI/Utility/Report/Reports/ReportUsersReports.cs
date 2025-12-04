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
    public class ReportUsersReports : IQdReportFactory
    {
        private static readonly ObservableCollection<ReportUserReport> ReportUserReportModelList =
            new ObservableCollection<ReportUserReport>();

        private static readonly ObservableCollection<InteractedUser> LstAcountReport =
            new ObservableCollection<InteractedUser>();

        public string Header { get; set; }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ReportUserModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstCurrentQueries, CampaignDetails campaignDetails)
        {
            try
            {
                ReportUserReportModelList.Clear();

                #region get data from InteractedUsers table and add to ReportUserReportModel

                var dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);
                dbCampaignService.GetAllInteractedUsers()?.ForEach(
                    report =>
                    {
                        ReportUserReportModelList.Add(new ReportUserReport
                        {
                            Id = report.Id,
                            AccountName = report.SinAccUsername,
                            Date = report.InteractionDateTime,
                            QueryType = report.QueryType,
                            Query = report.QueryValue,
                            Username = report.InteractedUsername
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
                        new GridViewColumnDescriptor {ColumnHeaderText = "Reported", ColumnBindingText = "Username"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ObservableCollection<object>(ReportUserReportModelList);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            try
            {
                LstAcountReport.Clear();

                IList reportDetails = dbAccountService.GetInteractedUsers(ActivityType.ReportUsers).ToList();
                foreach (InteractedUsers item in reportDetails)
                    LstAcountReport.Add(
                        new InteractedUser
                        {
                            Id = item.Id,
                            QueryType = item.QueryType,
                            Query = item.Query,
                            InteractedUsername = item.InteractedUsername,
                            FollowedBack = item.FollowedBack,
                            Date = item.Date.EpochToDateTimeUtc()
                        }
                    );
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return LstAcountReport;
        }

        public void ExportReports(ActivityType subModule, string fileName, ReportType dataSelectionType)
        {
            try
            {
                var csvData = new List<string>();
                if (dataSelectionType == ReportType.Campaign)
                {
                    Header = "AccountName,QueryType,Query,Reported user,Date";
                    ReportUserReportModelList.ForEach(report =>
                    {
                        csvData.Add(report.AccountName + "," + report.QueryType + "," + report.Query + "," +
                                    report.Username + "," + report.Date);
                    });
                }
                else
                {
                    Header = "Id,QueryType,Query,Reported user,Date";
                    LstAcountReport.ForEach(report =>
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