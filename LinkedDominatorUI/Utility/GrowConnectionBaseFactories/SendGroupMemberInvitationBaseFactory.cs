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
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.GrowConnection;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.GrowConnectionBaseFactories
{
    
    public class SendGroupMemberInvitationBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendGroupInvitations)
                .AddReportFactory(new SendGroupMemberInvitationReport())
                .AddViewCampaignFactory(new SendGroupMemberInvitationViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class SendGroupMemberInvitationReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public string activityType = ActivityType.SendGroupInvitations.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<SendInvitationToGroupMemberModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedUsersReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var count = 0;

            #region get data from InteractedUsers table and add to InteractedUsersReportMode

            dataBase.GetInteractedUsers(activityType).ForEach(
                ReportItem =>
                {
                    //var queryDetails = lstQueryDetails.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                    //if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                    //{
                    InteractedUsersReportModel.Add(new InteractedUsersReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User FullName", ColumnBindingText = "UserFullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User ProfileUrl", ColumnBindingText = "UserProfileUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Requested DateTime", ColumnBindingText = "InteractionDateTime"}
                };


            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersReportModel);

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
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );

            reportDetails = AccountsInteractedUsers.Select(user =>
                new
                {
                    user.Id,
                    user.QueryValue,
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
                Header = "AccountEmail,QueryValue,ActivityType,UserFullName,UserProfileUrl,Requested DateTime";

                InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.AccountEmail + "," + ReportItem.QueryValue.Replace("\"", "\"\"") +
                                    "\"," +
                                    ReportItem.ActivityType + "," + Utils.RemoveHtmlTags(ReportItem.UserFullName) +
                                    ",\"" + ReportItem.UserProfileUrl.Replace("\"", "\"\"") + "\"," +
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
                Header =
                    "QueryValue,ActivityType,UserFullName,UserProfileUrl,Requested DateTime";

                AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.QueryValue.Replace("\"", "\"\"") + "\"," +
                                    ReportItem.ActivityType + "," + Utils.RemoveHtmlTags(ReportItem.UserFullName) +
                                    ",\"" + ReportItem.UserProfileUrl.Replace("\"", "\"\"") + "\"," +
                                    ReportItem.InteractionDatetime);
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

    public class SendGroupMemberInvitationViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objSendGroupMemberInvitation = SendGroupInvitation.GetSingletonSendGroupInvitation();
                objSendGroupMemberInvitation.IsEditCampaignName = IsEditCampaignName;
                objSendGroupMemberInvitation.CancelEditVisibility = CancelEditVisibility;
                objSendGroupMemberInvitation.TemplateId = TemplateID;
                //objConnectionRequest.CampaignName = campaignDetails.CampaignName;
                objSendGroupMemberInvitation.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objSendGroupMemberInvitation.CampaignName;
                objSendGroupMemberInvitation.CampaignButtonContent = CampaignButtonContent;
                objSendGroupMemberInvitation.SelectedAccountCount =
                    campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objSendGroupMemberInvitation.SendGroupInvitationFooter.list_SelectedAccounts =
                    campaignDetails.SelectedAccountList;
                objSendGroupMemberInvitation.ObjViewModel.SendInvitationToGroupMemberModel =
                    templateDetails.ActivitySettings.GetActivityModel<SendInvitationToGroupMemberModel>(
                        objSendGroupMemberInvitation.ObjViewModel.Model);
                objSendGroupMemberInvitation.MainGrid.DataContext = objSendGroupMemberInvitation.ObjViewModel;

                TabSwitcher.ChangeTabIndex(1, 7);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
