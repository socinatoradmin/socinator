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
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.Scraper;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.Scraper
{
    public class CompanyScraperBaseFactory : ILdBaseFactory
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
        // public static List<InteractedCompanyReportModel> AccountsInteractedCompaniesModel = new List<InteractedCompanyReportModel>();

        public static List<InteractedCompanies> AccountsInteractedCompanies = new List<InteractedCompanies>();

        public string ActivityType = DominatorHouseCore.Enums.ActivityType.CompanyScraper.ToString();
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

            dataBase.GetInteractedCompanies(ActivityType).ForEach(
                reportItem =>
                {
                    var deSerializedData =
                        JsonConvert.DeserializeObject<CompanyScraperDetailedInfo>(reportItem.DetailedInfo);

                    InteractedCompanyReportModel.Add(new InteractedCompanyReportModel
                    {
                        Id = ++count,
                        AccountEmail = reportItem.AccountEmail,
                        QueryType = reportItem.QueryType,
                        QueryValue = reportItem.QueryValue,
                        ActivityType = reportItem.ActivityType,
                        CompanyName = reportItem.CompanyName,
                        CompanyUrl = reportItem.CompanyUrl,
                        TotalEmployees = reportItem.TotalEmployees,
                        Industry = reportItem.Industry,
                        IsFollowed = reportItem.IsFollowed,
                        CompanyLogoUrl = deSerializedData.CompanyLogoUrl,
                        DetailedCompanyScraperInfo = reportItem.DetailedInfo,
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Company Name", ColumnBindingText = "CompanyName"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Company Url", ColumnBindingText = "CompanyUrl"},
                    // new GridViewColumnDescriptor {ColumnHeaderText = "Company Logo",ColumnBindingText="CompanyLogoUrl" },
                    new GridViewColumnDescriptor {ColumnHeaderText = "Industry", ColumnBindingText = "Industry"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Is Followed", ColumnBindingText = "IsFollowed"},
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
            // AccountsInteractedCompaniesModel.Clear();
            IList reportDetails = dataBase.GetInteractedCompanies(ActivityType).ToList();
            var count = 0;
            foreach (InteractedCompanies reportItem in reportDetails)
                AccountsInteractedCompanies.Add(
                    new InteractedCompanies
                    {
                        Id = ++count,
                        QueryType = reportItem.QueryType,
                        // AccountEmail = reportItem.
                        QueryValue = reportItem.QueryValue,
                        ActivityType = reportItem.ActivityType,
                        CompanyName = reportItem.CompanyName,
                        CompanyUrl = reportItem.CompanyUrl,
                        TotalEmployees = reportItem.TotalEmployees,
                        Industry = reportItem.Industry,
                        IsFollowed = reportItem.IsFollowed,
                        DetailedInfo = reportItem.DetailedInfo,
                        InteractionDatetime = reportItem.InteractionDatetime
                    }
                );

            return AccountsInteractedCompanies;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();


            switch (reportType)
            {
                #region Campaign reports

                case ReportType.Campaign:
                    Header =
                        "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,CompanyName,CompanyUrl,Industry,IsFollowed,CompanyDescription,Specialties,CompanySize,Website,FoundationDate,Headquarter,Company Logo,Company Other Locations,ScrapedDate";

                    InteractedCompanyReportModel.ToList().ForEach(reportItem =>
                    {
                        try
                        {
                            var objCompanyScraperInfo =
                                JsonConvert.DeserializeObject<CompanyScraperDetailedInfo>(
                                    Uri.UnescapeDataString(reportItem.DetailedCompanyScraperInfo));

                            csvData.Add(reportItem.AccountEmail + "," + objCompanyScraperInfo.AccountUserFullName +
                                        "," +
                                        objCompanyScraperInfo.AccountUserProfileUrl + "," + reportItem.QueryType + "," +
                                        reportItem.QueryValue + "," +
                                        reportItem.ActivityType + "," + reportItem.CompanyName + "," +
                                        reportItem.CompanyUrl + "," +
                                        reportItem.Industry + "," + reportItem.IsFollowed + ",\"" +
                                        objCompanyScraperInfo.CompanyDescription.Replace("\"", "\"\"") + "\",\"" +
                                        objCompanyScraperInfo.Specialties.Replace("\n", "").Trim('[').Trim(']')
                                            .Replace("\"", "\"\"") + "\"," + objCompanyScraperInfo.CompanySize + "," +
                                        objCompanyScraperInfo.Website + "," +
                                        objCompanyScraperInfo.FoundationDate + "," + objCompanyScraperInfo.Headquarter +
                                        "," + objCompanyScraperInfo.CompanyLogoUrl + "," +
                                        $"\"{objCompanyScraperInfo.OtherLocations.Replace("\"", "\"\"")}\"," +
                                        reportItem.InteractionDateTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;

                #endregion

                #region Account reports

                case ReportType.Account:
                    Header =
                        "QueryType,QueryValue,ActivityType,CompanyName,CompanyUrl,Industry,IsFollowed,CompanyDescription,Specialties,CompanySize,Website,FoundationDate,Headquarter,Logo Data,Company Other Locations,ScrapedDate";

                    AccountsInteractedCompanies.ToList().ForEach(reportItem =>
                    {
                        try
                        {
                            var objCompanyScraperInfo =
                                JsonConvert.DeserializeObject<CompanyScraperDetailedInfo>(
                                    Uri.UnescapeDataString(reportItem.DetailedInfo));

                            csvData.Add(reportItem.QueryType + "," + reportItem.QueryValue + "," +
                                        reportItem.ActivityType + "," + reportItem.CompanyName + "," +
                                        reportItem.CompanyUrl + "," +
                                        reportItem.Industry + "," + reportItem.IsFollowed + "," +
                                        objCompanyScraperInfo.CompanyDescription + "," +
                                        objCompanyScraperInfo.Specialties.Replace("\n", "").Trim('[').Trim(']') + "," +
                                        objCompanyScraperInfo.CompanySize + "," + objCompanyScraperInfo.Website + "," +
                                        objCompanyScraperInfo.FoundationDate + "," + objCompanyScraperInfo.Headquarter +
                                        "," + objCompanyScraperInfo.CompanyLogoUrl + "," +
                                        $"\"{objCompanyScraperInfo.OtherLocations.Replace("\"", "\"\"")}\"," +
                                        reportItem.InteractionDatetime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;

                #endregion
            }


            Utilities.ExportReports(fileName, Header, csvData);
        }
    }

    public class CompanyScraperViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            try
            {
                var objCompanyScraper = CompanyScraper.GetSingeltonObjectCompanyScraper();
                objCompanyScraper.IsEditCampaignName = isEditCampaignName;
                objCompanyScraper.CancelEditVisibility = cancelEditVisibility;
                objCompanyScraper.TemplateId = templateId;
                // objCompanyScraper.CampaignName = campaignDetails.CampaignName;
                objCompanyScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objCompanyScraper.CampaignName;
                objCompanyScraper.CampaignButtonContent = campaignButtonContent;
                objCompanyScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                         $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objCompanyScraper.CompanyScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objCompanyScraper.ObjViewModel.CompanyScraperModel =
                    templateDetails.ActivitySettings.GetActivityModel<CompanyScraperModel>(objCompanyScraper
                        .ObjViewModel.Model);

                objCompanyScraper.MainGrid.DataContext = objCompanyScraper.ObjViewModel;

                TabSwitcher.ChangeTabIndex(5, 2);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}