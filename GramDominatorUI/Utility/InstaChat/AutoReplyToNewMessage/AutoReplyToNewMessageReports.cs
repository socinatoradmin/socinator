using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;

namespace GramDominatorUI.Utility.InstaChat.AutoReplyToNewMessage
{
    internal class AutoReplyToNewMessageReports : IGdReportFactory
    {
        private static List<AutoReplyToNewMessagesReportDetails> AutoReplyToNewMessagesReportModelCampaign { get; } =
            new List<AutoReplyToNewMessagesReportDetails>();

        private static List<AutoReplyToNewMessagesReportDetails> AutoReplyToNewMessagesReportModelAccount { get; } =
            new List<AutoReplyToNewMessagesReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            AutoReplyToNewMessagesReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to AutoReplyToNewMessagesReportModelCampaign

            var sNo = 0;
            dataBase.Get<InteractedUsers>().ForEach(
                report =>
                {
                    AutoReplyToNewMessagesReportModelCampaign.Add(new AutoReplyToNewMessagesReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        MessageReceiverUsername = report.InteractedUsername,
                        MessageReceiverUserId = report.InteractedUserId,
                        Message = report.DirectMessage,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });
                });

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKey﻿﻿﻿﻿Id".FromResourceDictionary(), ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyAccountUsername".FromResourceDictionary(),
                        ColumnBindingText = "AccountUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMessageReceiverUsername".FromResourceDictionary(),
                        ColumnBindingText = "MessageReceiverUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMessageReceiverUserId".FromResourceDictionary(),
                        ColumnBindingText = "MessageReceiverUserId"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyMessage".FromResourceDictionary(), ColumnBindingText = "Message"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };
            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(AutoReplyToNewMessagesReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(AutoReplyToNewMessagesReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.AutoReplyToNewMessage.ToString()).ToList();

            // Need to be cleared data for adding into static variable.
            AutoReplyToNewMessagesReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
                AutoReplyToNewMessagesReportModelAccount.Add(
                    new AutoReplyToNewMessagesReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        MessageReceiverUsername = report.InteractedUsername,
                        MessageReceiverUserId = report.InteractedUserId,
                        Message = report.DirectMessage,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });

            return AutoReplyToNewMessagesReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity Type, Account Username, Message Replied Username, Message Replied User Id, Replied Message, Date";

                AutoReplyToNewMessagesReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.MessageReceiverUsername + "," + report.MessageReceiverUserId + "," +
                                    report.Message.Replace("\r\n", " ") + "," + report.Date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
            {
                Header =
                    "Activity Type, Account Username, Message Replied Username, Message Replied User Id, Replied Message, Date";

                AutoReplyToNewMessagesReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.MessageReceiverUsername + "," + report.MessageReceiverUserId + "," +
                                    report.Message.Replace("\r\n", " ") + "," + report.Date);
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
    }
}