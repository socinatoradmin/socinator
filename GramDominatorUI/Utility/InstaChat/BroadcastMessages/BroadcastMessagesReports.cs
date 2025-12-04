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

namespace GramDominatorUI.Utility.InstaChat.BroadcastMessages
{
    internal class BroadcastMessagesReports : IGdReportFactory
    {
        private static readonly ObservableCollection<BroadcastMessageReportDetails> BroadcastMessageReportModelCampaign
            = new ObservableCollection<BroadcastMessageReportDetails>();

        private static List<BroadcastMessageReportDetails> BroadcastMessageReportModelAccount { get; } =
            new List<BroadcastMessageReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BroadcastMessagesModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            BroadcastMessageReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to BroadcastMessageReportModel

            var sNo = 0;
            dataBase.Get<InteractedUsers>().ForEach(
                report =>
                {
                    BroadcastMessageReportModelCampaign.Add(new BroadcastMessageReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        MessageReceiverUsername = report.InteractedUsername,
                        MessageReceiverUserId = report.InteractedUserId,
                        Message = report.DirectMessage?.Replace(",",""),
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

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(BroadcastMessageReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(BroadcastMessageReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.BroadcastMessages.ToString()).ToList();

            // Need to be cleared data for adding into static variable.
            BroadcastMessageReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
                BroadcastMessageReportModelAccount.Add(
                    new BroadcastMessageReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), report.ActivityType),
                        AccountUsername = report.Username,
                        MessageReceiverUsername = report.InteractedUsername,
                        MessageReceiverUserId = report.InteractedUserId,
                        Message = report.DirectMessage,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime()
                    });

            return BroadcastMessageReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Account Name, Activity type, Message Receiver Username, Message Receiver UserId, Message, Date";

                BroadcastMessageReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountUsername + "," + report.ActivityType + "," +
                                    report.MessageReceiverUsername + "," +
                                    report.MessageReceiverUserId + "," + report.Message.Replace("\r\n", " ") + "," +
                                    report.Date);
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
                    "Activity type, Account Name, Message Receiver Username, Message Receiver UserId, Message, Date";

                BroadcastMessageReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.MessageReceiverUsername + "," +
                                    report.MessageReceiverUserId + "," + report.Message.Replace("\r\n", " ") + "," +
                                    report.Date);
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