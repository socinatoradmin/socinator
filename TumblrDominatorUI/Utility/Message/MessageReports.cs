using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorUI.Report;

namespace TumblrDominatorUI.Utility.Message
{
    public class MessageReports : ITumblrReportFactory
    {
        public static ObservableCollection<MessageReportDetails> InteractedUsersModel =
            new ObservableCollection<MessageReportDetails>();

        public static ObservableCollection<MessageReportDetails> AccountsInteractedUsers =
            new ObservableCollection<MessageReportDetails>();

        private int _i;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BroadcastMessagesModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            if (InteractedUsersModel.Count >= 0) InteractedUsersModel.Clear();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedUsers().ForEach(
                report =>
                {
                    InteractedUsersModel.Add(new MessageReportDetails
                    {
                        Id = report.Id,
                        AccountName = report.AccountEmail,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        DirectMessage = report.DirectMessage,
                        MessagedUser = report.InteractedUsername
                    });
                });

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Broadcast Message", ColumnBindingText = "DirectMessage"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Username", ColumnBindingText = "MessagedUser"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"}
                };


            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }
        //public IList GetsAccountReport(IDbAccountService dataBase)
        //{

        //}


        public void ExportReports(ReportType dataSelectionType, string fileName)
        {
            var csvData = new List<string>();
            var value = dataSelectionType.ToString();
            var reports = (ReportType)Enum.Parse(typeof(ReportType), value);

            #region Campaign reports

            if (reports == ReportType.Campaign)
            {
                Header = "Query Type, Query Value, Account Username, Broadcast Message,Username, Date";

                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.AccountName + "," +
                                    report.DirectMessage + "," + report.MessagedUser + "," + report.Date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reports == ReportType.Account)
            {
                Header = "Query Type, Query Value, Broadcast Message, Username, Date";

                AccountsInteractedUsers.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.DirectMessage + "," +
                                    report.MessagedUser + "," + report.Date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.GetInteractedUsers(ActivityType.BroadcastMessages).ToList();
            var forLocalTime = DateTime.Now - DateTime.UtcNow;
            foreach (InteractedUser report in reportDetails)
            {
                _i = _i + 1;
                AccountsInteractedUsers.Add(
                    new MessageReportDetails
                    {
                        Id = _i,
                        AccountName = report.UserName,
                        Date = report.InteractionTimeStamp.EpochToDateTimeUtc() + forLocalTime,
                        QueryType = report.QueryType,
                        Query = report.QueryValue,
                        DirectMessage = report.DirectMessage,
                        MessagedUser = report.InteractedUsername
                    });
            }

            return AccountsInteractedUsers;
        }
    }
}