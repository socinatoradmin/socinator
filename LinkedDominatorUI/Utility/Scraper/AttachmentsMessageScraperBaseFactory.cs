using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.LDModel.Scraper;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.Scraper;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.Scraper
{
    public class AttachmentsMessageScraperBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AttachmnetsMessageScraper)
                .AddReportFactory(new AttachmentsMessageScraperReport())
                .AddViewCampaignFactory(new AttachmentsMessageScraperViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class AttachmentsMessageScraperReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> MessageConversationReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public static CampaignDetails CampaignDetails;

        public string activityType = ActivityType.AttachmnetsMessageScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<MessageConversationScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            MessageConversationReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var count = 0;

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            dataBase.GetInteractedUsers(activityType).ForEach(
                ReportItem =>
                {
                    MessageConversationReportModel.Add(new InteractedUsersReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        ActivityType = ReportItem.ActivityType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        DetailedUserInfo = ReportItem.DetailedUserInfo,
                        InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                    });
                });

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User FullName", ColumnBindingText = "UserFullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User ProfileUrl", ColumnBindingText = "UserProfileUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Message with Attachments", ColumnBindingText = "DetailedUserInfo"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Interacted DateTime", ColumnBindingText = "InteractionDateTime"}
                };


            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedCompanyReportModel);

            #endregion

            return new ObservableCollection<object>(MessageConversationReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedUsers.Clear();
            IList reportDetails = dataBase.GetInteractedUsers(activityType).ToList();
            var count = 0;
            foreach (InteractedUsers ReportItem in reportDetails)
                AccountsInteractedUsers.Add(
                    new InteractedUsers
                    {
                        Id = ++count,
                        ActivityType = ReportItem.ActivityType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        DetailedUserInfo = ReportItem.DetailedUserInfo,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );
            reportDetails = AccountsInteractedUsers.Select(user =>
                new
                {
                    user.Id,
                    user.ActivityType,
                    user.UserFullName,
                    user.UserProfileUrl,
                    user.DetailedUserInfo,
                    user.InteractionDatetime
                }).ToList();

            return reportDetails;
        }

        public void ExportReports(ActivityType activityType, string FileName, ReportType reportType)
        {
            var CsvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "AccountEmail,ActivityType,UserFullName,UserProfileUrl,Message with Attachments,Interacted DateTime";

                MessageConversationReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.AccountEmail + "," +
                                    ReportItem.ActivityType + "," +
                                    ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," +
                                    $"\"{ReportItem.DetailedUserInfo.Replace("\n", " ")}\"" + "," +
                                    ReportItem.InteractionDateTime);
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
                Header = "ActivityType,UserName,UserProfileUrl,Message with Attachments,Interacted DateTime";

                MessageConversationReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var data = string.Join(",", ReportItem.ActivityType,
                            ReportItem.UserFullName, ReportItem.UserProfileUrl,
                            $"\"{ReportItem.DetailedUserInfo.Replace("\n", " ")}\"", ReportItem.InteractionDateTime);
                        CsvData.Add(data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            Utilities.ExportReports(FileName, Header, CsvData);
        }
    }

    public class AttachmentsMessageScraperViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objJobScraper = AttachmentsMessageScraper.GetSingletonMessageConversationScraper();
                objJobScraper.IsEditCampaignName = IsEditCampaignName;
                objJobScraper.CancelEditVisibility = CancelEditVisibility;
                objJobScraper.TemplateId = TemplateID;
                //objJobScraper.CampaignName = campaignDetails.CampaignName;
                objJobScraper.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objJobScraper.CampaignName;
                objJobScraper.CampaignButtonContent = CampaignButtonContent;
                objJobScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objJobScraper.AttachmentsMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objJobScraper.ObjViewModel.MessageConversationScraperModel =
                    templateDetails.ActivitySettings.GetActivityModel<MessageConversationScraperModel>(objJobScraper
                        .ObjViewModel.Model);

                objJobScraper.MainGrid.DataContext = objJobScraper.ObjViewModel;

                TabSwitcher.ChangeTabIndex(5, 3);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}