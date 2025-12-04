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
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.Messenger;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.MessengerBaseFactories
{
    public class BroadcastMessagesBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new BroadcastMessagesReport())
                .AddViewCampaignFactory(new BroadcastMessagesViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class BroadcastMessagesReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();
        private static readonly string success = ActivityStatus.Success.ToString();

        public string activityType = ActivityType.BroadcastMessages.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BroadcastMessagesModel>(activitySettings).SavedQueries;
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
                        InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime(),
                        Status = ReportItem.Status
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
                        {ColumnHeaderText = "Messaged DateTime", ColumnBindingText = "InteractionDateTime"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
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
                        InteractionDatetime = ReportItem.InteractionDatetime,
                        Status = string.IsNullOrWhiteSpace(ReportItem.Status) ? success : ReportItem.Status
                    }
                );
            reportDetails = AccountsInteractedUsers.Select(user =>
                new
                {
                    user.Id,
                    user.ActivityType,
                    user.UserFullName,
                    user.UserProfileUrl,
                    user.InteractionDatetime,
                    user.Status
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
                    Header = "AccountEmail,ActivityType,UserFullName,UserProfileUrl,Message,Messaged DateTime,Status";

                    InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                    {
                        try
                        {
                            var status = string.IsNullOrWhiteSpace(ReportItem.Status) ? success : ReportItem.Status;
                            CsvData.Add(
                                $"{ReportItem.AccountEmail},{ReportItem.ActivityType},{ReportItem.UserFullName},{ReportItem.UserProfileUrl},{ReportItem.DetailedUserInfo},{ReportItem.InteractionDateTime},{status}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;

                case ReportType.Account:
                    Header = "ActivityType,UserFullName,UserProfileUrl,Message,Messaged DateTime,Status";

                    AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                    {
                        try
                        {
                            var status = string.IsNullOrWhiteSpace(ReportItem.Status) ? success : ReportItem.Status;
                            CsvData.Add(
                                $"{ReportItem.ActivityType},{ReportItem.UserFullName},{ReportItem.UserProfileUrl},{ReportItem.DetailedUserInfo},{ReportItem.InteractionDatetime},{status}");
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

    public class BroadcastMessagesViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objBroadcastMessages = BroadcastMessages.GetSingletonBroadcastMessages();
                objBroadcastMessages.IsEditCampaignName = IsEditCampaignName;
                objBroadcastMessages.CancelEditVisibility = CancelEditVisibility;
                objBroadcastMessages.TemplateId = TemplateID;
                // objBroadcastMessages.CampaignName = campaignDetails.CampaignName;
                objBroadcastMessages.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objBroadcastMessages.CampaignName;
                objBroadcastMessages.CampaignButtonContent = CampaignButtonContent;
                objBroadcastMessages.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                            $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objBroadcastMessages.BrodCastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objBroadcastMessages.ObjViewModel.BroadcastMessagesModel =
                    templateDetails.ActivitySettings.GetActivityModel<BroadcastMessagesModel>(objBroadcastMessages
                        .ObjViewModel.Model);

                objBroadcastMessages.MainGrid.DataContext = objBroadcastMessages.ObjViewModel;

                TabSwitcher.ChangeTabIndex(2, 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}