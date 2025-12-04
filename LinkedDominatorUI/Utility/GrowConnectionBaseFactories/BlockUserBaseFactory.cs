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
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.GrowConnection;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.GrowConnectionBaseFactories
{
    public class BlockUserBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BlockUser)
                .AddReportFactory(new BlockUserReport())
                .AddViewCampaignFactory(new BlockUserViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class BlockUserReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public string ActivityType = DominatorHouseCore.Enums.ActivityType.BlockUser.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BlockUserModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedUsersReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            var count = 0;
            dataBase.GetInteractedUsers(ActivityType).ForEach(
                reportItem =>
                {
                    InteractedUsersReportModel.Add(new InteractedUsersReportModel
                    {
                        Id = ++count,
                        AccountEmail = reportItem.AccountEmail,
                        ActivityType = reportItem.ActivityType,
                        UserFullName = reportItem.UserFullName,
                        UserProfileUrl = reportItem.UserProfileUrl,
                        DetailedUserInfo = string.Empty,
                        InteractionDateTime = reportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
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
                        {ColumnHeaderText = "Blocked Date", ColumnBindingText = "InteractionDateTime"}
                };


            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersReportModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedUsers.Clear();
            IList reportDetails = dataBase.GetInteractedUsers(ActivityType).ToList();
            var count = 0;
            foreach (InteractedUsers reportItem in reportDetails)
                AccountsInteractedUsers.Add(
                    new InteractedUsers
                    {
                        Id = ++count,
                        ActivityType = reportItem.ActivityType,
                        UserFullName = reportItem.UserFullName,
                        UserProfileUrl = reportItem.UserProfileUrl,
                        InteractionDatetime = reportItem.InteractionDatetime
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

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "AccountEmail,ActivityType,UserFullName,UserProfileUrl,BlockedDate";

                InteractedUsersReportModel.ToList().ForEach(reportItem =>
                {
                    try
                    {
                        csvData.Add(
                            $"{reportItem.AccountEmail},{reportItem.ActivityType},{reportItem.UserFullName},{reportItem.UserProfileUrl},{reportItem.InteractionDateTime}");
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
                Header = "ActivityType,UserFullName,UserProfileUrl,BlockedDate";

                AccountsInteractedUsers.ToList().ForEach(reportItem =>
                {
                    try
                    {
                        csvData.Add(reportItem.ActivityType + "," + reportItem.UserFullName + "," +
                                    reportItem.UserProfileUrl + "," + reportItem.InteractionDatetime);
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

    public class BlockUserViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            try
            {
                var objectBlockUser = BlockUser.GetSingletonObjectBlockUser();
                objectBlockUser.IsEditCampaignName = isEditCampaignName;
                objectBlockUser.CancelEditVisibility = cancelEditVisibility;
                objectBlockUser.TemplateId = templateId;
                objectBlockUser.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objectBlockUser.CampaignName;
                objectBlockUser.CampaignButtonContent = campaignButtonContent;
                objectBlockUser.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                       $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objectBlockUser.BlockUserFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objectBlockUser.ObjViewModel.BlockUserModel =
                    templateDetails.ActivitySettings.GetActivityModel<BlockUserModel>(
                        objectBlockUser.ObjViewModel.Model, true);

                objectBlockUser.MainGrid.DataContext = objectBlockUser.ObjViewModel;

                TabSwitcher.ChangeTabIndex(1, 8);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}