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
    internal class UnSubscribeReport : IRdReportFactory
    {
        public static ObservableCollection<SubRedditReportModel> UnSubscriberReportModelsCampaign =
            new ObservableCollection<SubRedditReportModel>();

        private static List<SubRedditReportModelAccount> UnSubscriberReportModelsAccount { get; } =
            new List<SubRedditReportModelAccount>();

        public string Header { get; set; } = string.Empty;


        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Sr. No,Subscribed Account,Activity Type,Time Interacted,SubReddit Title,Sub Reddit name,Type,Subscribers Count ,Description ,Subscribed,URL,isNSFW,SubRedditId ,isQuarantined ,Display Text,CommunityIcon";

                    UnSubscriberReportModelsCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + ","
                                        + report.ActivityType + ","
                                        + report.InteractionTimeStamp + ","
                                        + report.Title.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Name.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Type.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Subscribers + ","
                                        + report.PublicDescription.Replace(",", " ").Replace("\r\n", " ")
                                            .Replace("\n", " ") + ","
                                        + report.UserIsSubscriber + ","
                                        + report.Url + ","
                                        + report.IsNsfw + ","
                                        + report.SubRedditId + ","
                                        + report.IsQuarantined + ","
                                        + report.DisplayText + ","
                                        + report.CommunityIcon + ",");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "Sr. No,Subscribed Account,Activity Type,Time Interacted,SubReddit Title,Sub Reddit name,Type,Subscribers Count ,Description ,Subscribed,URL,isNSFW,SubRedditId ,isQuarantined ,Display Text,CommunityIcon";

                    UnSubscriberReportModelsAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + ","
                                        + report.ActivityType + ","
                                        + report.InteractionTimeStamp + ","
                                        + report.Title.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Name.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Type.Replace(",", " ").Replace("\r\n", " ") + ","
                                        + report.Subscribers + ","
                                        + report.PublicDescription.Replace(",", " ").Replace("\r\n", " ")
                                            .Replace("\n", " ") + ","
                                        + report.UserIsSubscriber + ","
                                        + report.Url + ","
                                        + report.IsNsfw + ","
                                        + report.SubRedditId + ","
                                        + report.IsQuarantined + ","
                                        + report.DisplayText + ","
                                        + report.CommunityIcon + ",");
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
            UnSubscriberReportModelsAccount.Clear();
            // Get data from InteractedSubreddit table and add to SubRedditModel
            IList reportDetails = dbAccountService.GetInteractedSubreddit(ActivityType.UnSubscribe).ToList();
            foreach (InteractedSubreddit report in reportDetails)
                UnSubscriberReportModelsAccount.Add(new SubRedditReportModelAccount
                {
                    Id = columnId++,
                    AccountUsername = report.SinAccUsername,
                    ActivityType = report.ActivityType,
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
            return UnSubscriberReportModelsAccount;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                UnSubscriberReportModelsCampaign.Clear();

                #region

                // Get data from InteractedSubreddit table and add to SubRedditModel
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedSubreddit>().ForEach(
                    report =>
                    {
                        UnSubscriberReportModelsCampaign.Add(new SubRedditReportModel
                        {
                            Id = report.Id,
                            AccountUsername = report.SinAccUsername,
                            ActivityType = report.ActivityType,
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
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
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
                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(UnSubscriberReportModelsCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            //return UnSubscriberReportModelsCampaign.Count;
            return new ObservableCollection<object>(UnSubscriberReportModelsCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UnSubscribeModel>(activitySettings).SavedQueries;
        }
    }
}