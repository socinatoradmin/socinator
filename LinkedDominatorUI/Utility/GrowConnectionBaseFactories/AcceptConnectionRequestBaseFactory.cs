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
    public class AcceptConnectionRequestBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AcceptConnectionRequest)
                .AddReportFactory(new AcceptConnectionRequestReport())
                .AddViewCampaignFactory(new AcceptConnectionRequestViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class AcceptConnectionRequestReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public string activityType = ActivityType.AcceptConnectionRequest.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<AcceptConnectionRequestModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedUsersReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            var count = 0;
            dataBase.GetInteractedUsers(activityType).ForEach(
                ReportItem =>
                {
                    InteractedUsersReportModel.Add(new InteractedUsersReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        ActivityType = ReportItem.ActivityType,
                        QueryType = ReportItem.QueryType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        DetailedUserInfo = string.Empty,
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User FullName", ColumnBindingText = "UserFullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User ProfileUrl", ColumnBindingText = "UserProfileUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Accepted Date", ColumnBindingText = "InteractionDateTime"}
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
                        QueryType = ReportItem.QueryType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );
            reportDetails = AccountsInteractedUsers.Select(user =>
                new
                {
                    user.Id,
                    user.ActivityType,
                    user.QueryType,
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
                Header = "AccountEmail,ActivityType,QueryType,UserFullName,UserProfileUrl,AcceptedDate";

                InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(
                            $"{ReportItem.AccountEmail},{ReportItem.ActivityType},{ReportItem.QueryType},{ReportItem.UserFullName},{ReportItem.UserProfileUrl},{ReportItem.InteractionDateTime}");
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
                Header = "ActivityType,QueryType,UserFullName,UserProfileUrl,AcceptedDate";

                AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.ActivityType + "," + ReportItem.QueryType + "," +
                                    ReportItem.UserFullName + "," +
                                    ReportItem.UserProfileUrl + "," + ReportItem.InteractionDatetime);
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

    public class AcceptConnectionRequestViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objAcceptConnectionRequest = AcceptConnectionRequest.GetSingeltonObjectAcceptConnectionRequest();
                objAcceptConnectionRequest.IsEditCampaignName = IsEditCampaignName;
                objAcceptConnectionRequest.CancelEditVisibility = CancelEditVisibility;
                objAcceptConnectionRequest.TemplateId = TemplateID;
                //objAcceptConnectionRequest.CampaignName = campaignDetails.CampaignName;
                objAcceptConnectionRequest.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objAcceptConnectionRequest.CampaignName;
                objAcceptConnectionRequest.CampaignButtonContent = CampaignButtonContent;
                objAcceptConnectionRequest.SelectedAccountCount =
                    campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objAcceptConnectionRequest.AcceptConnectionFooter.list_SelectedAccounts =
                    campaignDetails.SelectedAccountList;

                objAcceptConnectionRequest.ObjViewModel.AcceptConnectionRequestModel =
                    templateDetails.ActivitySettings.GetActivityModel<AcceptConnectionRequestModel>(
                        objAcceptConnectionRequest.ObjViewModel.Model, true);

                objAcceptConnectionRequest.MainGrid.DataContext = objAcceptConnectionRequest.ObjViewModel;

                TabSwitcher.ChangeTabIndex(1, 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}