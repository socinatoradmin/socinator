using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.ReportModel;
using RedditDominatorCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace RedditDominatorUI.Utility.SubscriberChannels
{
    public class SubscribeReport : IRdReportFactory
    {
        public static ObservableCollection<SubRedditReportModel> SubscriberReportModelsCampaign =
            new ObservableCollection<SubRedditReportModel>();

        private static List<SubRedditReportModel> SubscriberReportModelsAccount { get; } =
            new List<SubRedditReportModel>();

        public string Header { get; set; } = string.Empty;


        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    //Header =
                    //"Sr. No,Activity Type,Title,Time Interacted,Sub Reddit name,Type ,Subscribers,PublicDescription,isNSFW,URL,isQuarantined ,SubRedditId ,CommunityIcon,Display Text";
                    Header = "Sr.No,Subscribed Account,Query Type,Activity Type,Time Interacted,Query Value,Sub Reddit Name,Subscribe Count,subscribed,Description,isNSFW,URL,isQuarantine,Sub RedditID,DisplayText";
                    //Header = "Sr.No,sbuscribed Account,Query Type,Activity type,CommunityIcon,DisplayText,SubscribedId,Interacted Time,isNSFW,isQuarantine,Name,PublicDescription,QueryType,QueryValue,SubRedditId,Subscribers";
                    SubscriberReportModelsCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + ","
                                        + report.QueryType + ","
                                        + report.ActivityType + ","
                                        + report.InteractionTimeStamp + ","
                                        + report.QueryValue + ","
                                        + report.Name?.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Subscribers + ","
                                        + report.UserIsSubscriber + ","
                                        + report.PublicDescription?.Replace(",", " ")?.Replace("\r\n", "")?.Replace("\n\n", "") + ","
                                        + report.IsNsfw + ","
                                        + report.Url + ","
                                        + report.IsQuarantined + ","
                                        + report.SubRedditId + ","
                                        + report.DisplayText);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        Header = "Sr.No,Subscribed Account,Query Type,Activity Type,Time Interacted,Query Value,Sub Reddit Name,Subscribe Count,subscribed,Description,isNSFW,URL,isQuarantine,Sub RedditID,DisplayText";

                    SubscriberReportModelsAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + ","
                                        + report.QueryType + ","
                                        + report.ActivityType + ","
                                        + report.InteractionTimeStamp + ","
                                        + report.QueryValue + ","
                                        + report.Name?.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Subscribers + ","
                                        + report.UserIsSubscriber + ","
                                        + report.PublicDescription?.Replace(",", " ")?.Replace("\r\n", "") + ","
                                        + report.IsNsfw + ","
                                        + report.Url + ","
                                        + report.IsQuarantined + ","
                                        + report.SubRedditId + ","
                                        + report.DisplayText);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reportType), reportType, null);
            }

            #endregion

            #region Account reports

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetAccountReport(IDbAccountService dbAccountService)
        {
            var columnId = 1;
            SubscriberReportModelsAccount.Clear();
            // Get data from InteractedSubreddit table and add to SubRedditModel
            IList reportDetails = dbAccountService.GetInteractedSubreddit(ActivityType.Subscribe).ToList();
            foreach (InteractedSubreddit report in reportDetails)
                SubscriberReportModelsAccount.Add(new SubRedditReportModel
                {
                    Id = columnId++,
                    AccountUsername = report.SinAccUsername,
                    QueryType = report.QueryType,
                    ActivityType = report.ActivityType,
                    QueryValue = report.QueryValue,
                    InteractionTimeStamp = report.InteractionDateTime,
                    Title = report.title,
                    Name = report.name,
                    Type = report.type,
                    Subscribers = report.subscribers,
                    PublicDescription = report.publicDescription,
                    UserIsSubscriber = report.userIsSubscriber,
                    Url = report.url,
                    IsNsfw = report.isNSFW,
                    SubRedditId = report.SubscribeId,
                    IsQuarantined = report.isQuarantined,
                    DisplayText = report.displayText,
                    CommunityIcon = report.communityIcon
                });
            return SubscriberReportModelsAccount;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                SubscriberReportModelsCampaign.Clear();

                #region

                // Get data from InteractedSubreddit table and add to SubRedditModel
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedSubreddit>().ForEach(
                    report =>
                    {
                        SubscriberReportModelsCampaign.Add(new SubRedditReportModel
                        {
                            Id = report.Id,
                            AccountUsername = report.SinAccUsername,
                            QueryType = report.QueryType,
                            ActivityType = report.ActivityType,
                            QueryValue = report.QueryValue,
                            InteractionTimeStamp = report.InteractionDateTime,
                            Title = report.title,
                            Name = report.name,
                            Type = report.type,
                            Subscribers = report.subscribers,
                            PublicDescription = report.publicDescription,
                            UserIsSubscriber = report.userIsSubscriber,
                            Url = report.url,
                            IsNsfw = report.isNSFW,
                            SubRedditId = report.SubscribeId,
                            IsQuarantined = report.isQuarantined,
                            DisplayText = report.displayText,
                            CommunityIcon = report.communityIcon
                        });
                    });

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Subscribed Account", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Time Interacted", ColumnBindingText = "InteractionTimeStamp"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "SubReddit Title", ColumnBindingText = "Title"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sub Reddit name", ColumnBindingText = "Name"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Subscribers Count ", ColumnBindingText = "Subscribers"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Description ", ColumnBindingText = "PublicDescription"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Subscribed", ColumnBindingText = "UserIsSubscriber"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "URL", ColumnBindingText = "Url"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isNSFW", ColumnBindingText = "IsNsfw"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "SubRedditId ", ColumnBindingText = "SubRedditId"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isQuarantined ", ColumnBindingText = "IsQuarantined"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Display Text", ColumnBindingText = "DisplayText"}
                    };
                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(SubscriberReportModelsCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            //return SubscriberReportModelsCampaign.Count;
            return new ObservableCollection<object>(SubscriberReportModelsCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<SubscribeModel>(activitySettings).SavedQueries;
        }
    }
}