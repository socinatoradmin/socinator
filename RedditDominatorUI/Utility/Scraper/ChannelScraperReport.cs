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

namespace RedditDominatorUI.Utility.Scraper
{
    public class ChannelScraperReport : IRdReportFactory
    {
        public static ObservableCollection<SubRedditReportModel> ChannelScraperReportModelsCampaign =
            new ObservableCollection<SubRedditReportModel>();

        private static List<SubRedditReportModel> ChannelScraperReportModelsAccount { get; } =
            new List<SubRedditReportModel>();

        public string Header { get; set; } = string.Empty;

        public void ExportReports(ActivityType subModule, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            switch (reportType)
            {
                case ReportType.Campaign:
                    Header =
                        "Sr. No,Account,Activity Type,Time Interacted,Title,Sub Reddit name,Type,Subscribers Count ,Description ,Subscribed,URL,isNSFW,ChannelId ,isQuarantined ,Display Text,CommunityIcon";

                    ChannelScraperReportModelsCampaign.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(
                                        (report.Id) + "," +
                                        (report.AccountUsername ?? string.Empty) + "," +
                                        (report.ActivityType ?? string.Empty) + "," +
                                        (report.InteractionTimeStamp) + "," +
                                        ((report.Title ?? string.Empty).Replace(",", " ").Replace("\r\n", " ")) + "," +
                                        ((report.Name ?? string.Empty).Replace(",", " ").Replace("\r\n", " ")) + "," +
                                        ((report.Type ?? string.Empty).Replace(",", " ").Replace("\r\n", " ")) + "," +
                                        (report.Subscribers) + "," +
                                        ((report.PublicDescription ?? string.Empty).Replace(",", " ").Replace("\r\n", " ").Replace("\n", " ")) + "," +
                                        (report.UserIsSubscriber) + "," +
                                        (report.Url ?? string.Empty) + "," +
                                        (report.IsNsfw) + "," +
                                        (report.SubRedditId ?? string.Empty) + "," +
                                        (report.IsQuarantined) + "," +
                                        (report.DisplayText ?? string.Empty) + "," +
                                        (report.CommunityIcon ?? string.Empty) + ","
                                    );

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
                case ReportType.Account:
                    Header =
                        "Sr. No,Account,Activity Type,Time Interacted,Title,Sub Reddit name,Type,Subscribers Count ,Description ,Subscribed,URL,isNSFW,ChannelId ,isQuarantined ,Display Text,CommunityIcon";

                    ChannelScraperReportModelsAccount.ToList().ForEach(report =>
                    {
                        try
                        {
                            csvData.Add(report.Id + "," + report.AccountUsername + ","+
                                        (report.ActivityType ?? string.Empty) + "," +
                                        (report.InteractionTimeStamp) + "," +
                                        ((report.Title ?? string.Empty).Replace(",", " ").Replace("\r\n", " ")) + "," +
                                        ((report.Name ?? string.Empty).Replace(",", " ").Replace("\r\n", " ")) + "," +
                                        ((report.Type ?? string.Empty).Replace(",", " ").Replace("\r\n", " ")) + "," +
                                        (report.Subscribers) + "," +
                                        ((report.PublicDescription ?? string.Empty).Replace(",", " ").Replace("\r\n", " ").Replace("\n", " ")) + "," +
                                        (report.UserIsSubscriber) + "," +
                                        (report.Url ?? string.Empty) + "," +
                                        (report.IsNsfw) + "," +
                                        (report.SubRedditId ?? string.Empty) + "," +
                                        (report.IsQuarantined) + "," +
                                        (report.DisplayText ?? string.Empty) + "," +
                                        (report.CommunityIcon ?? string.Empty) + ","
                                        );
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
            ChannelScraperReportModelsAccount.Clear();
            //  Get data from InteractedSubreddit table and add to SubRedditModel
            IList reportDetails = dbAccountService.GetInteractedSubreddit(ActivityType.ChannelScraper).ToList();
            foreach (InteractedSubreddit report in reportDetails)
                ChannelScraperReportModelsAccount.Add(new SubRedditReportModel
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
            return ChannelScraperReportModelsAccount.Select(x => new
            {
                x.Id,
                x.ActivityType,
                x.InteractionTimeStamp,
                x.Title,
                x.Name,
                x.Type,
                x.Subscribers,
                x.PublicDescription,
                x.UserIsSubscriber,
                x.Url,
                x.IsNsfw,
                x.IsQuarantined,
                x.DisplayText,
                x.CommunityIcon
            }).ToList();
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            try
            {
                var dataBase = new DbOperations(campaignDetails.CampaignId, campaignDetails.SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                ChannelScraperReportModelsCampaign.Clear();

                #region

                // get data from InteractedSubreddit table and add to SubRedditModel
                dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedSubreddit>().ForEach(
                    report =>
                    {
                        ChannelScraperReportModelsCampaign.Add(new SubRedditReportModel
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

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Sr. No", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Account", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Time Interacted", ColumnBindingText = "InteractionTimeStamp"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Title", ColumnBindingText = "Title"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "name", ColumnBindingText = "Name"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Subscribers Count ", ColumnBindingText = "Subscribers"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Description ", ColumnBindingText = "PublicDescription"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Subscribed", ColumnBindingText = "UserIsSubscriber"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "URL", ColumnBindingText = "Url"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "isNSFW", ColumnBindingText = "IsNsfw"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "ChannelId ", ColumnBindingText = "SubRedditId"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "isQuarantined ", ColumnBindingText = "IsQuarantined"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Display Text", ColumnBindingText = "DisplayText"}
                    };
                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(ChannelScraperReportModelsCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return new ObservableCollection<object>(ChannelScraperReportModelsCampaign);
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ChannelScraperModel>(activitySettings).SavedQueries;
        }
    }
}