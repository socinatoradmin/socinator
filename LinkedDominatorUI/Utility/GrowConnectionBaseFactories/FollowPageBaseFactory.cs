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
    public class FollowPageBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FollowPages)
                .AddReportFactory(new FollowPageReport())
                .AddViewCampaignFactory(new FollowPageViewCampaign());

            return builder.LdUtilityFactory;
        }

        public class FollowPageReport : ILdReportFactory
        {
            public static ObservableCollection<InteractedPageReportModel> InteractedPageReportModel =
                new ObservableCollection<InteractedPageReportModel>();

            public static List<InteractedPage> AccountsInteractedUsers = new List<InteractedPage>();

            public string activityType = ActivityType.FollowPages.ToString();
            public string Header { get; set; } = string.Empty;

            public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
            {
                return JsonConvert.DeserializeObject<FollowPagesModel>(activitySettings).SavedQueries;
            }

            public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
                List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
            {
                InteractedPageReportModel.Clear();
                var dataBase = new DbCampaignService(campaignDetails.CampaignId);
                var count = 0;

                #region get data from InteractedUsers table and add to InteractedUsersReportMode

                dataBase.GetInteractedPages(activityType).ForEach(
                    ReportItem =>
                    {
                        //var queryDetails = lstQueryDetails.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                        //if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        //{
                        InteractedPageReportModel.Add(new InteractedPageReportModel
                        {
                            Id = ++count,
                            AccountEmail = ReportItem.AccountEmail,
                            QueryType = ReportItem.QueryType,
                            QueryValue = ReportItem.QueryValue,
                            ActivityType = ReportItem.ActivityType,
                            TotalEmployees = ReportItem.TotalEmployees,
                            FollowerCount = ReportItem.FollowerCount,
                            PageName = ReportItem.PageName,
                            PageUrl = ReportItem.PageUrl,
                            InteractionDateTime = ReportItem.InteractionDatetime
                        });
                    });

                #endregion

                #region Generate Reports column with data

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Page Name", ColumnBindingText = "PageName"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Page Url", ColumnBindingText = "PageUrl"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Requested DateTime", ColumnBindingText = "InteractionDateTime"}
                    };


                //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersReportModel);

                #endregion

                return new ObservableCollection<object>(InteractedPageReportModel);
            }

            public IList GetsAccountReport(IDbAccountService dataBase)
            {
                AccountsInteractedUsers.Clear();
                IList reportDetails = dataBase.GetInteractedPages(activityType).ToList();
                var count = 0;
                foreach (InteractedPage ReportItem in reportDetails)
                    AccountsInteractedUsers.Add(
                        new InteractedPage
                        {
                            Id = ++count,
                            QueryType = ReportItem.QueryType,
                            QueryValue = ReportItem.QueryValue,
                            ActivityType = ReportItem.ActivityType,
                            PageName = ReportItem.PageName,
                            PageUrl = ReportItem.PageUrl,
                            TotalEmployees = ReportItem.TotalEmployees,
                            PageId = ReportItem.PageId,
                            FollowerCount = ReportItem.FollowerCount,
                            InteractionDatetime = ReportItem.InteractionDatetime
                        }
                    );

                reportDetails = AccountsInteractedUsers.Select(user =>
                    new
                    {
                        user.Id,
                        user.QueryType,
                        user.QueryValue,
                        user.ActivityType,
                        user.PageName,
                        user.PageUrl,
                        user.PageId,
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
                        "AccountEmail,QueryType,QueryValue,ActivityType,PageName,PageUrl,PageFollowerCount,TotalEmployees,FollowDateTime";

                    InteractedPageReportModel.ToList().ForEach(ReportItem =>
                    {
                        try
                        {
                            CsvData.Add(ReportItem.AccountEmail + "," + ReportItem.QueryType + ",\"" +
                                        ReportItem.QueryValue.Replace("\"", "\"\"") + "\"," +
                                        ReportItem.ActivityType + "," +
                                        ReportItem.PageName.Replace("\"", "\"\"")?.Replace(",", " ") + ",\"" +
                                        ReportItem.PageUrl.Replace("\"", "\"\"") + "\"," + ReportItem.FollowerCount +
                                        "," + ReportItem.TotalEmployees + "," +
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
                        "QueryType,QueryValue,ActivityType,PageName,PageUrl,PageFollowerCount, TotalEmployees,FollowDateTime";

                    AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                    {
                        try
                        {
                            CsvData.Add(ReportItem.QueryType + ",\"" + ReportItem.QueryValue.Replace("\"", "\"\"") +
                                        "\"," +
                                        ReportItem.ActivityType + "," +
                                        ReportItem.PageName.Replace("\"", "\"\"")?.Replace(",", " ") + ",\"" +
                                        ReportItem.PageUrl.Replace("\"", "\"\"") + "\"," +
                                        ReportItem.FollowerCount + "," + ReportItem.TotalEmployees + "," +
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


        public class FollowPageViewCampaign : ILdViewCampaign
        {
            public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
                bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent,
                string TemplateID)
            {
                try
                {
                    var objConnectionRequest = FollowPage.GetSingeltonObjectFollowPages();
                    objConnectionRequest.IsEditCampaignName = IsEditCampaignName;
                    objConnectionRequest.CancelEditVisibility = CancelEditVisibility;
                    objConnectionRequest.TemplateId = TemplateID;
                    //  objConnectionRequest.CampaignName = campaignDetails.CampaignName;
                    objConnectionRequest.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                        ? campaignDetails.CampaignName
                        : objConnectionRequest.CampaignName;
                    objConnectionRequest.CampaignButtonContent = CampaignButtonContent;
                    objConnectionRequest.SelectedAccountCount =
                        campaignDetails.SelectedAccountList.Count +
                        $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                    objConnectionRequest.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                    objConnectionRequest.ObjViewModel.FollowPagesModel =
                        templateDetails.ActivitySettings.GetActivityModel<FollowPagesModel>(objConnectionRequest
                            .ObjViewModel.Model);

                    objConnectionRequest.MainGrid.DataContext = objConnectionRequest.ObjViewModel;

                    TabSwitcher.ChangeTabIndex(1, 5);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }
    }
}