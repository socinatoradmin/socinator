using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.Interface;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Report;

namespace PinDominator.Utility.CreateAccount
{
    public class AccountCreatorReports : IPdReportFactory
    {
        public static ObservableCollection<CreateAccountReportDetails> LstCreateAccCampaign =
            new ObservableCollection<CreateAccountReportDetails>();

        private static readonly List<CreateAccountReportDetails> LstCreateAccAccount =
            new List<CreateAccountReportDetails>();

        private readonly TimeSpan _localTime = DateTime.Now - DateTime.UtcNow;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AccountCreatorModel>(activitySettings).SavedQueries;
        }

        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (dataSelectionType == ReportType.Campaign)
            {
                Header = "Email, Password, Age, Gender, Interaction Date, ActivityType";

                LstCreateAccCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Email + "," + report.Password + "," + report.Age + "," +
                                    report.Gender + "," + report.InteractionDate + "," + report.ActivityType + "," +
                                    report.Status);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            #region Account reports

            if (dataSelectionType == ReportType.Account)
            {
                Header = "Email, Password, Age, Gender, Interaction Date, ActivityType";

                LstCreateAccAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.Email + "," + report.Password + "," + report.Age + "," +
                                    report.Gender + "," + report.InteractionDate + "," + report.ActivityType + "," +
                                    report.Status);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            LstCreateAccCampaign.Clear();
            IDbCampaignService dbCampaignService = new DbCampaignService(campaignDetails.CampaignId);

            #region Get data from Create table and add to CreateAccountReportDetails

            var id = 1;
            var actCreateAccount = ActivityType.CreateAccount.ToString();
            dbCampaignService.Get<DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.CreateAccount>(x =>
                x.ActivityType == actCreateAccount).ForEach(
                report =>
                {
                    LstCreateAccCampaign.Add(new CreateAccountReportDetails
                    {
                        Id = id++,
                        Email = report.Email,
                        Password = report.Password,
                        Age = report.Age,
                        Gender = report.Gender,
                        ActivityType = report.ActivityType,
                        Status = report.Status,
                        InteractionDate =
                            (report.InteractionDate.EpochToDateTimeUtc() + _localTime).ToString(CultureInfo
                                .InvariantCulture)
                    });
                });

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Email", ColumnBindingText = "Email"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Password", ColumnBindingText = "Password"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Age", ColumnBindingText = "Age"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Gender", ColumnBindingText = "Gender"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"}
                };

            #endregion

            return new ObservableCollection<object>(LstCreateAccCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            var actType = ActivityType.CreateAccount.ToString();
            var createAcc = ((int) ActivityType.CreateAccount).ToString();
            IList reportDetails = dataBase.Get<InteractedPosts>()
                .Where(x => x.OperationType == actType || x.OperationType == createAcc).ToList();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.CreateAccount item in reportDetails)
                LstCreateAccAccount.Add(
                    new CreateAccountReportDetails
                    {
                        Id = id++,
                        Email = item.Email,
                        Password = item.Password,
                        Age = item.Age,
                        Gender = item.Gender,
                        ActivityType = item.ActivityType,
                        InteractionDate =
                            (item.InteractionDate.EpochToDateTimeUtc() + _localTime).ToString(CultureInfo
                                .InvariantCulture)
                    }
                );
            return LstCreateAccAccount.Select(x =>
                new
                {
                    x.Email,
                    x.Password,
                    x.Age,
                    x.Gender,
                    x.InteractionDate,
                    x.ActivityType
                }).ToList();
        }
    }
}