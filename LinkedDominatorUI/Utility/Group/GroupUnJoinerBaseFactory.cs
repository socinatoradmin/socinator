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
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.Group;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.Group
{
    public class GroupUnJoinerBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.GroupUnJoiner)
                .AddReportFactory(new GroupUnJoinerrReport())
                .AddViewCampaignFactory(new GroupUnJoinerViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class GroupUnJoinerrReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedGroupReportModel> InteractedGroupReportModel =
            new ObservableCollection<InteractedGroupReportModel>();

        public static List<InteractedGroups> AccountsInteractedGroups = new List<InteractedGroups>();

        public string activityType = ActivityType.GroupUnJoiner.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<GroupUnJoinerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedGroupReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var count = 0;

            #region get data from InteractedUsers table and add to InteractedGroupReportModel

            dataBase.GetInteractedGroups(activityType).ForEach(
                ReportItem =>
                {
                    InteractedGroupReportModel.Add(new InteractedGroupReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        ActivityType = ReportItem.ActivityType,
                        GroupName = ReportItem.GroupName,
                        GroupUrl = ReportItem.GroupUrl,
                        TotalMembers = ReportItem.TotalMembers,
                        CommunityType = ReportItem.CommunityType,
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Group Name", ColumnBindingText = "GroupName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Total Members", ColumnBindingText = "TotalMembers"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Community Type", ColumnBindingText = "CommunityType"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "UnJoin Date", ColumnBindingText = "InteractionDateTime"}
                };


            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedGroupReportModel);

            #endregion

            return new ObservableCollection<object>(InteractedGroupReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedGroups.Clear();
            IList reportDetails = dataBase.GetInteractedGroups(activityType).ToList();
            var count = 0;
            foreach (InteractedGroups ReportItem in reportDetails)
                AccountsInteractedGroups.Add(
                    new InteractedGroups
                    {
                        Id = ++count,
                        ActivityType = ReportItem.ActivityType,
                        GroupName = ReportItem.GroupName,
                        GroupUrl = ReportItem.GroupUrl,
                        TotalMembers = ReportItem.TotalMembers,
                        CommunityType = ReportItem.CommunityType,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );
            reportDetails = AccountsInteractedGroups.Select(user =>
                new
                {
                    user.Id,
                    user.ActivityType,
                    user.GroupName,
                    user.GroupUrl,
                    user.TotalMembers,
                    user.CommunityType,
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
                Header = "AccountEmail,ActivityType,GroupName,GroupUrl,TotalMembers,UnJoinedDate";

                InteractedGroupReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.AccountEmail + "," + ReportItem.ActivityType + "," +
                                    ReportItem.GroupName?.Replace(","," ") + "," + ReportItem.GroupUrl + "," + ReportItem.TotalMembers?.Replace(","," ") +
                                    "," + ReportItem.InteractionDateTime);
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
                Header = "ActivityType,GroupName,GroupUrl,TotalMembers,UnJoinedDate";

                AccountsInteractedGroups.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.ActivityType + "," + ReportItem.GroupName + "," + ReportItem.GroupUrl +
                                    "," +
                                    ReportItem.TotalMembers + "," + ReportItem.InteractionDatetime);
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

    public class GroupUnJoinerViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objGroupUnJoiner = GroupUnJoiner.GetSingeltonObjectGroupUnJoiner();
                objGroupUnJoiner.IsEditCampaignName = IsEditCampaignName;
                objGroupUnJoiner.CancelEditVisibility = CancelEditVisibility;
                objGroupUnJoiner.TemplateId = TemplateID;
                //  objGroupUnJoiner.CampaignName = campaignDetails.CampaignName;
                objGroupUnJoiner.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objGroupUnJoiner.CampaignName;
                objGroupUnJoiner.CampaignButtonContent = CampaignButtonContent;
                objGroupUnJoiner.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                        $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objGroupUnJoiner.GroupUnJoinerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objGroupUnJoiner.ObjViewModel.GroupUnJoinerModel =
                    templateDetails.ActivitySettings.GetActivityModel<GroupUnJoinerModel>(
                        objGroupUnJoiner.ObjViewModel.Model, true);

                objGroupUnJoiner.MainGrid.DataContext = objGroupUnJoiner.ObjViewModel;

                TabSwitcher.ChangeTabIndex(4, 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}