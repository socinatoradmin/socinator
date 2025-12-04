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
using LinkedDominatorCore.DetailedInfo;
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
    public class SendGreetingsToConnectionsBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendGreetingsToConnections)
                .AddReportFactory(new SendGreetingsToConnectionsReport())
                .AddViewCampaignFactory(new SendGreetingsToConnectionsViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class SendGreetingsToConnectionsReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public string activityType = ActivityType.SendGreetingsToConnections.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<SendGreetingsToConnectionsModel>(activitySettings).SavedQueries;
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
                        {ColumnHeaderText = "Interaction DateTime", ColumnBindingText = "InteractionDateTime"}
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
                Header = "AccountEmail,ActivityType,UserFullName,UserProfileUrl,Greeting,InteractionDateTime";

                InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var objInfo =
                            JsonConvert.DeserializeObject<SendGreetingsToConnectionsDetailedInfo>(
                                Uri.UnescapeDataString(ReportItem.DetailedUserInfo));

                        CsvData.Add(ReportItem.AccountEmail + "," + ReportItem.ActivityType + "," +
                                    ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," +
                                    objInfo.FinalGreeting + "," + ReportItem.InteractionDateTime);
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
                Header = "ActivityType,UserFullName,UserProfileUrl,Greeting,InteractionDateTime";

                AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var objInfo =
                            JsonConvert.DeserializeObject<SendGreetingsToConnectionsDetailedInfo>(
                                Uri.UnescapeDataString(ReportItem.DetailedUserInfo));
                        CsvData.Add(ReportItem.ActivityType + "," + ReportItem.UserFullName + "," +
                                    ReportItem.UserProfileUrl + "," +
                                    objInfo.FinalGreeting + "," + ReportItem.InteractionDatetime);
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

    public class SendGreetingsToConnectionsViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objSendGreetingsToConnections = SendGreetingsToConnections.GetSingeltonSendGreetingsToConnections();
                objSendGreetingsToConnections.IsEditCampaignName = IsEditCampaignName;
                objSendGreetingsToConnections.CancelEditVisibility = CancelEditVisibility;
                objSendGreetingsToConnections.TemplateId = TemplateID;
                //  objSendGreetingsToConnections.CampaignName = campaignDetails.CampaignName;
                objSendGreetingsToConnections.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objSendGreetingsToConnections.CampaignName;
                objSendGreetingsToConnections.CampaignButtonContent = CampaignButtonContent;
                objSendGreetingsToConnections.SelectedAccountCount =
                    campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objSendGreetingsToConnections.SendGreetingsToConnectionsFooter.list_SelectedAccounts =
                    campaignDetails.SelectedAccountList;

                objSendGreetingsToConnections.ObjViewModel.SendGreetingsToConnectionsModel =
                    templateDetails.ActivitySettings.GetActivityModel<SendGreetingsToConnectionsModel>(
                        objSendGreetingsToConnections.ObjViewModel.Model, true);

                objSendGreetingsToConnections.MainGrid.DataContext = objSendGreetingsToConnections.ObjViewModel;

                TabSwitcher.ChangeTabIndex(2, 3);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}