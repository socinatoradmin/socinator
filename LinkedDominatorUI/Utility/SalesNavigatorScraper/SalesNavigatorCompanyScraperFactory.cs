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
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.LDModel.SalesNavigatorScraper;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.SalesNavigatorScraper;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.SalesNavigatorScraper
{
    public class SalesNavigatorCompanyScraperFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CompanyScraper)
                .AddReportFactory(new CompanyScraperReport())
                .AddViewCampaignFactory(new CompanyScraperViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class CompanyScraperReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedCompanyReportModel> InteractedCompanyReportModel =
            new ObservableCollection<InteractedCompanyReportModel>();

        public static List<InteractedCompanies> AccountsInteractedCompanies = new List<InteractedCompanies>();

        public string activityType = ActivityType.SalesNavigatorCompanyScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CompanyScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedCompanyReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var count = 0;

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            dataBase.GetInteractedCompanies(activityType).ForEach(
                ReportItem =>
                {
                    //var queryDetails = lstQueryDetails.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                    //if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                    //{
                    InteractedCompanyReportModel.Add(new InteractedCompanyReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        CompanyName = ReportItem.CompanyName,
                        CompanyUrl = ReportItem.CompanyUrl,
                        TotalEmployees = ReportItem.TotalEmployees,
                        Industry = ReportItem.Industry,
                        DetailedCompanyScraperInfo = ReportItem.DetailedInfo,
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Company Name", ColumnBindingText = "CompanyName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Company Url", ColumnBindingText = "CompanyUrl"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Industry", ColumnBindingText = "Industry"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped Date", ColumnBindingText = "InteractionDateTime"}
                };


            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedCompanyReportModel);

            #endregion

            return new ObservableCollection<object>(InteractedCompanyReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedCompanies.Clear();
            IList reportDetails = dataBase.GetInteractedCompanies(activityType).ToList();
            var count = 0;
            foreach (InteractedCompanies ReportItem in reportDetails)
                AccountsInteractedCompanies.Add(
                    new InteractedCompanies
                    {
                        Id = ++count,
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        CompanyName = ReportItem.CompanyName,
                        CompanyUrl = ReportItem.CompanyUrl,
                        TotalEmployees = ReportItem.TotalEmployees,
                        Industry = ReportItem.Industry,
                        DetailedInfo = ReportItem.DetailedInfo,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );

            return AccountsInteractedCompanies;
        }

        public void ExportReports(ActivityType activityType, string FileName, ReportType reportType)
        {
            var CsvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "AccountEmail,QueryType,QueryValue,ActivityType,CompanyName,CompanyUrl,Industry,Location,CompanySize,Company logo,FoundationDate,CompanyDescription,Headquarter,Website,ScrapedDate";
                InteractedCompanyReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        //string CSVData = AccountUsed + "," + SearchUrl + "," + CompanyName + "," + CompanyUrl + "," + IndustryType + "," + Location + "," + CompanySize + "," + Companylogo + "," + FoundingYear + "," + Description + "," + Headquaters + "," + Website + "," + ScrappedDate.Replace("/", "-") + "," + Status.Replace(",", " ") + "," + Campaign_Name.Replace("/", "_").Replace(":", "_");
                        var objSalesNavigatorScraperDetails =
                            JsonConvert.DeserializeObject<SalesNavigatorScraperDetails>(
                                Uri.UnescapeDataString(ReportItem.DetailedCompanyScraperInfo));
                        CsvData.Add(ReportItem.AccountEmail + " ," + ReportItem.QueryType + "," +
                                    ReportItem.QueryValue.AsCsvData() + "," +
                                    ReportItem.ActivityType + "," + ReportItem.CompanyName.AsCsvData() + "," +
                                    ReportItem.CompanyUrl.AsCsvData() + "," +
                                    ReportItem.Industry.AsCsvData() + "," + objSalesNavigatorScraperDetails.CurrentCompanyLocation.AsCsvData() +
                                    "," + (objSalesNavigatorScraperDetails.CurrentCompanySize.Equals("N/A")?ReportItem.TotalEmployees.AsCsvData():objSalesNavigatorScraperDetails.CurrentCompanySize.AsCsvData())+ "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyLogo.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyFoundingYear.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyDescription.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyHeadquarters.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyWebsite.AsCsvData() + "," +
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
                    "QueryType,QueryValue,ActivityType,CompanyName,CompanyUrl,Industry,Location,CompanySize,Company logo,FoundationDate,CompanyDescription,Headquarter,Website,ScrapedDate";

                AccountsInteractedCompanies.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var objSalesNavigatorScraperDetails =
                            JsonConvert.DeserializeObject<SalesNavigatorScraperDetails>(
                                Uri.UnescapeDataString(ReportItem.DetailedInfo));
                        CsvData.Add(ReportItem.QueryType + "," + ReportItem.QueryValue.AsCsvData() + "," +
                                    ReportItem.ActivityType + "," + ReportItem.CompanyName.AsCsvData() + "," +
                                    ReportItem.CompanyUrl.AsCsvData() + "," +
                                    ReportItem.Industry.AsCsvData() + "," + objSalesNavigatorScraperDetails.CurrentCompanyLocation.AsCsvData() +
                                    "," + objSalesNavigatorScraperDetails.CurrentCompanySize.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyLogo.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyFoundingYear.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyDescription.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyHeadquarters.AsCsvData() + "," +
                                    objSalesNavigatorScraperDetails.CurrentCompanyWebsite.AsCsvData() + "," +
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

    public class CompanyScraperViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objCompanyScraper = CompanyScraper.GetSingletonObjectCompanyScraper();
                objCompanyScraper.IsEditCampaignName = IsEditCampaignName;
                objCompanyScraper.CancelEditVisibility = CancelEditVisibility;
                objCompanyScraper.TemplateId = TemplateID;
                // objCompanyScraper.CampaignName = campaignDetails.CampaignName;
                objCompanyScraper.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objCompanyScraper.CampaignName;
                objCompanyScraper.CampaignButtonContent = CampaignButtonContent;
                objCompanyScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                         $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objCompanyScraper.CompanyScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objCompanyScraper.ObjViewModel.CompanyScraperModel =
                    templateDetails.ActivitySettings.GetActivityModel<CompanyScraperModel>(objCompanyScraper
                        .ObjViewModel.Model);

                objCompanyScraper.MainGrid.DataContext = objCompanyScraper.ObjViewModel;

                TabSwitcher.ChangeTabIndex(7, 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}