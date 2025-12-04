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

namespace LinkedDominatorUI.Utility.MessengerBaseFactories
{
    public class SendMessageToNewConnectionBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendMessageToNewConnection)
                .AddReportFactory(new SendMessageToNewConnectionReport())
                .AddViewCampaignFactory(new SendMessageToNewConnectionViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class SendMessageToNewConnectionReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public string activityType = ActivityType.SendMessageToNewConnection.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<SendMessageToNewConnectionModel>(activitySettings).SavedQueries;
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
                        {ColumnHeaderText = "Messaged DateTime", ColumnBindingText = "InteractionDateTime"}
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

            if (reportType == ReportType.Campaign)
            {
                Header = "AccountEmail,ActivityType,UserFullName,UserProfileUrl,Message,Messaged DateTime";

                InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.AccountEmail + "," + ReportItem.ActivityType + "," +
                                    ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," +
                                    ReportItem.DetailedUserInfo + "," + ReportItem.InteractionDateTime);
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
                Header = "ActivityType,UserFullName,UserProfileUrl,Message,Messaged DateTime";

                AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.ActivityType + "," + ReportItem.UserFullName + "," +
                                    ReportItem.UserProfileUrl + "," +
                                    ReportItem.DetailedUserInfo + "," + ReportItem.InteractionDatetime);
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

    public class SendMessageToNewConnectionViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objSendMessageToNewConnection = SendMessageToNewConnection.GetSingeltonSendMessageToNewConnection();
                objSendMessageToNewConnection.IsEditCampaignName = IsEditCampaignName;
                objSendMessageToNewConnection.CancelEditVisibility = CancelEditVisibility;
                objSendMessageToNewConnection.TemplateId = TemplateID;
                //objSendMessageToNewConnection.CampaignName = campaignDetails.CampaignName;
                objSendMessageToNewConnection.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objSendMessageToNewConnection.CampaignName;
                objSendMessageToNewConnection.CampaignButtonContent = CampaignButtonContent;
                objSendMessageToNewConnection.SelectedAccountCount =
                    campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objSendMessageToNewConnection.SendMessageToNewConnectionFooter.list_SelectedAccounts =
                    campaignDetails.SelectedAccountList;

                objSendMessageToNewConnection.ObjViewModel.SendMessageToNewConnectionModel =
                    templateDetails.ActivitySettings.GetActivityModel<SendMessageToNewConnectionModel>(
                        objSendMessageToNewConnection.ObjViewModel.Model, true);

                objSendMessageToNewConnection.MainGrid.DataContext = objSendMessageToNewConnection.ObjViewModel;

                TabSwitcher.ChangeTabIndex(2, 2);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}