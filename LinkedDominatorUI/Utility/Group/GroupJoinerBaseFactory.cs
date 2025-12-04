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
    public class GroupJoinerBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ConnectionRequest)
                .AddReportFactory(new GroupJoinerReport())
                .AddViewCampaignFactory(new GroupJoinerViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class GroupJoinerReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedGroupReportModel> InteractedGroupReportModel =
            new ObservableCollection<InteractedGroupReportModel>();

        public static List<InteractedGroups> AccountsInteractedGroups = new List<InteractedGroups>();

        public string activityType = ActivityType.GroupJoiner.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<GroupJoinerModel>(activitySettings).SavedQueries;
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
                    var queryDetails = lstQueryDetails.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                    if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        InteractedGroupReportModel.Add(new InteractedGroupReportModel
                        {
                            Id = ++count,
                            AccountEmail = ReportItem.AccountEmail,
                            QueryType = ReportItem.QueryType,
                            QueryValue = ReportItem.QueryValue,
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
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKey﻿﻿﻿﻿Id".FromResourceDictionary(), ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyAccount".FromResourceDictionary(), ColumnBindingText = "AccountEmail"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryType".FromResourceDictionary(), ColumnBindingText = "QueryType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryValue".FromResourceDictionary(),
                        ColumnBindingText = "QueryValue"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyGroupName".FromResourceDictionary(), ColumnBindingText = "GroupName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyTotalMembers".FromResourceDictionary(),
                        ColumnBindingText = "TotalMembers"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommunityType".FromResourceDictionary(),
                        ColumnBindingText = "CommunityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyJoinRequestDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionDateTime"
                    }
                };


            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedGroupReportModel);

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
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        GroupName = ReportItem.GroupName,
                        GroupUrl = ReportItem.GroupUrl,
                        TotalMembers = ReportItem.TotalMembers,
                        CommunityType = ReportItem.CommunityType,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );

            return AccountsInteractedGroups;
        }

        public void ExportReports(ActivityType activityType, string FileName, ReportType reportType)
        {
            var CsvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "AccountEmail,QueryType,QueryValue,ActivityType,GroupName,GroupUrl,TotalMembers,JoinRequestDate";

                InteractedGroupReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue +
                                    "," + ReportItem.ActivityType + "," +
                                    ReportItem.GroupName.AsCsvData() + "," + ReportItem.GroupUrl + "," + ReportItem.TotalMembers +
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
                Header = "QueryType,QueryValue,ActivityType,GroupName,GroupUrl,TotalMembers,JoinRequestDate";

                AccountsInteractedGroups.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        CsvData.Add(ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType +
                                    "," +
                                    ReportItem.GroupName.AsCsvData() + "," + ReportItem.GroupUrl + "," + ReportItem.TotalMembers +
                                    "," + ReportItem.InteractionDatetime);
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

    public class GroupJoinerViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objGroupJoiner = GroupJoiner.GetSingeltonObjectGroupJoiner();
                objGroupJoiner.IsEditCampaignName = IsEditCampaignName;
                objGroupJoiner.CancelEditVisibility = CancelEditVisibility;
                objGroupJoiner.TemplateId = TemplateID;
                // objGroupJoiner.CampaignName = campaignDetails.CampaignName;
                objGroupJoiner.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objGroupJoiner.CampaignName;
                objGroupJoiner.CampaignButtonContent = CampaignButtonContent;
                objGroupJoiner.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                      $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objGroupJoiner.GroupJoinerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objGroupJoiner.ObjViewModel.GroupJoinerModel =
                    templateDetails.ActivitySettings.GetActivityModel<GroupJoinerModel>(objGroupJoiner.ObjViewModel
                        .Model);

                objGroupJoiner.MainGrid.DataContext = objGroupJoiner.ObjViewModel;

                TabSwitcher.ChangeTabIndex(4, 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}