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
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.Messenger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorUI.Utility.MessengerBaseFactories
{
    public class DeleteConversationBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DeleteConversations)
                .AddReportFactory(new DeleteConversationReport())
                .AddViewCampaignFactory(new DeleteConversationViewCampaign());

            return builder.LdUtilityFactory;
        }

        public class DeleteConversationReport : ILdReportFactory
        {
            public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
                new ObservableCollection<InteractedUsersReportModel>();

            public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

            public string activityType = ActivityType.DeleteConversations.ToString();
            public string Header { get; set; } = string.Empty;

            public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
            {
                return JsonConvert.DeserializeObject<DeleteConversationsModel>(activitySettings).SavedQueries;
            }

            public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
                List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
            {
                InteractedUsersReportModel.Clear();
                var dataBase = new DbCampaignService(campaignDetails.CampaignId);
                var count = 0;

                #region get data from InteractedUsers table and add to InteractedUsersReportModel

                dataBase.GetInteractedUsers(activityType).ForEach(
                    ReportItem =>
                    {
                        InteractedUsersReportModel.Add(new InteractedUsersReportModel
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
                            {ColumnHeaderText = "Interacted DateTime", ColumnBindingText = "InteractionDateTime"}
                    };


                // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersReportModel);

                #endregion

                return new ObservableCollection<object>(InteractedUsersReportModel);
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
                        user.InteractionDatetime
                    }).ToList();

                return reportDetails;
            }

            public void ExportReports(ActivityType activityType, string FileName, ReportType reportType)
            {
                var CsvData = new List<string>();

                #region Campaign reports

                switch (reportType)
                {
                    case ReportType.Campaign:
                        Header = "AccountEmail,ActivityType,UserFullName,UserProfileUrl,Message,Messaged DateTime";
                        var jArrayHandler = JsonJArrayHandler.GetInstance;
                        InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                        {
                            try
                            {
                                var jToken = JObject.Parse(ReportItem.DetailedUserInfo);
                                var messageContent = jArrayHandler.GetJTokenValue(jToken, "MessageContent");
                                CsvData.Add(
                                    $"{ReportItem.AccountEmail},{ReportItem.ActivityType},{ReportItem.UserFullName},{ReportItem.UserProfileUrl},{messageContent},{ReportItem.InteractionDateTime}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }
                        });
                        break;

                    case ReportType.Account:
                        Header = "ActivityType,UserFullName,UserProfileUrl,Message,Messaged DateTime";

                        AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                        {
                            try
                            {
                                CsvData.Add(
                                    $"{ReportItem.ActivityType},{ReportItem.UserFullName},{ReportItem.UserProfileUrl},{ReportItem.DetailedUserInfo},{ReportItem.InteractionDatetime}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }
                        });
                        break;
                }

                #endregion


                Utilities.ExportReports(FileName, Header, CsvData);
            }
        }

        public class DeleteConversationViewCampaign : ILdViewCampaign
        {
            public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
                bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent,
                string TemplateID)
            {
                try
                {
                    var objDeleteConversations = DeleteConversations.GetSingletonDeleteConversations();
                    objDeleteConversations.IsEditCampaignName = IsEditCampaignName;
                    objDeleteConversations.CancelEditVisibility = CancelEditVisibility;
                    objDeleteConversations.TemplateId = TemplateID;
                    //  objAutoReplyToNewMessage.CampaignName = campaignDetails.CampaignName;
                    objDeleteConversations.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                        ? campaignDetails.CampaignName
                        : objDeleteConversations.CampaignName;
                    objDeleteConversations.CampaignButtonContent = CampaignButtonContent;
                    objDeleteConversations.SelectedAccountCount =
                        campaignDetails.SelectedAccountList.Count +
                        $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                    objDeleteConversations.DeleteConversationsFooter.list_SelectedAccounts =
                        campaignDetails.SelectedAccountList;

                    objDeleteConversations.ObjViewModel.DeleteConversationsModel =
                        templateDetails.ActivitySettings.GetActivityModel<DeleteConversationsModel>(
                            objDeleteConversations.ObjViewModel.Model, true);

                    objDeleteConversations.MainGrid.DataContext = objDeleteConversations.ObjViewModel;

                    TabSwitcher.ChangeTabIndex(2, 4);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }
    }
}