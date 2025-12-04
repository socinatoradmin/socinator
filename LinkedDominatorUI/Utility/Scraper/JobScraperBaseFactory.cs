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
    public class JobScraperBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.JobScraper)
                .AddReportFactory(new JobScraperReport())
                .AddViewCampaignFactory(new JobScraperViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class JobScraperReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedJobsReportModel> InteractedJobsReportModel =
            new ObservableCollection<InteractedJobsReportModel>();

        public static List<InteractedJobs> AccountsInteractedJobs = new List<InteractedJobs>();

        public string activityType = ActivityType.JobScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<JobScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedJobsReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var count = 0;

            #region get data from InteractedUsers table and add to InteractedJobsReportModel

            dataBase.GetInteractedJobs(activityType).ForEach(
                ReportItem =>
                {
                    InteractedJobsReportModel.Add(new InteractedJobsReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        JobTitle = ReportItem.JobTitle,
                        JobPostUrl = ReportItem.JobPostUrl,
                        DetailedInfo = ReportItem.DetailedInfo,
                        InteractedDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
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
                    new GridViewColumnDescriptor {ColumnHeaderText = "Job Title", ColumnBindingText = "JobTitle"},
                    new GridViewColumnDescriptor {ColumnHeaderText = "Job Post Url", ColumnBindingText = "JobPostUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped Date", ColumnBindingText = "InteractedDateTime"}
                };


            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedJobsReportModel);

            #endregion

            return new ObservableCollection<object>(InteractedJobsReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedJobs.Clear();
            IList reportDetails = dataBase.GetInteractedJobs(activityType).ToList();
            var count = 0;
            foreach (InteractedJobs ReportItem in reportDetails)
                AccountsInteractedJobs.Add(
                    new InteractedJobs
                    {
                        Id = ++count,
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        JobTitle = ReportItem.JobTitle,
                        JobPostUrl = ReportItem.JobPostUrl,
                        DetailedInfo = ReportItem.DetailedInfo,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );

            return AccountsInteractedJobs;
        }

        public void ExportReports(ActivityType activityType, string FileName, ReportType reportType)
        {
            var CsvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,JobPostUrl,CompanyName,CompanyWebsite,JobTitle,JobLocation,PostedOn,NumberOfApplicants,Industry,EmploymentType,SeniorityLevel,JobFunction,JobPosterProfileUrl,JobPosterFirstName,JobPosterLastName,DegreeOfConnection,ScrapedDate";

                InteractedJobsReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var objJobInfo =
                            JsonConvert.DeserializeObject<JobScraperDetailedInfo>(
                                Uri.UnescapeDataString(ReportItem.DetailedInfo));


                        CsvData.Add(ReportItem.AccountEmail + "," + objJobInfo.AccountUserFullName + "," +
                                    objJobInfo.AccountUserProfileUrl + "," +
                                    ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType +
                                    "," + ReportItem.JobPostUrl + "," +
                                    objJobInfo.CompanyName + "," + objJobInfo.CompanyWebsite + "," +
                                    objJobInfo.JobTitle + "," + objJobInfo.JobLocation + "," +
                                    objJobInfo.PostedOn + "," + objJobInfo.NumberOfApplicants + "," +
                                    objJobInfo.Industry + "," + objJobInfo.EmploymentType + "," +
                                    objJobInfo.SeniorityLevel + "," + objJobInfo.JobFunction + "," +
                                    objJobInfo.JobPosterProfileUrl + "," + objJobInfo.JobPosterFirstName + "," +
                                    objJobInfo.JobPosterLastName + "," +
                                    objJobInfo.DegreeOfConnection + "," + ReportItem.InteractedDateTime);
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
                    "QueryType,QueryValue,ActivityType,JobPostUrl,CompanyName,CompanyWebsite,JobTitle,JobLocation,PostedOn,NumberOfApplicants,Industry,EmploymentType,SeniorityLevel,JobFunction,JobPosterProfileUrl,JobPosterFirstName,JobPosterLastName,DegreeOfConnection,ScrapedDate";

                AccountsInteractedJobs.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var objJobInfo =
                            JsonConvert.DeserializeObject<JobScraperDetailedInfo>(
                                Uri.UnescapeDataString(ReportItem.DetailedInfo));
                        CsvData.Add(ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType +
                                    "," + ReportItem.JobPostUrl + "," +
                                    objJobInfo.CompanyName + "," + objJobInfo.CompanyWebsite + "," +
                                    objJobInfo.JobTitle + "," + objJobInfo.JobLocation + "," +
                                    objJobInfo.PostedOn + "," + objJobInfo.NumberOfApplicants + "," +
                                    objJobInfo.Industry + "," + objJobInfo.EmploymentType + "," +
                                    objJobInfo.SeniorityLevel + "," + objJobInfo.JobFunction + "," +
                                    objJobInfo.JobPosterProfileUrl + "," + objJobInfo.JobPosterFirstName + "," +
                                    objJobInfo.JobPosterLastName + "," +
                                    objJobInfo.DegreeOfConnection + "," + ReportItem.InteractionDatetime);
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

    public class JobScraperViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objJobScraper = JobScraper.GetSingeltonObjectJobScraper();
                objJobScraper.IsEditCampaignName = IsEditCampaignName;
                objJobScraper.CancelEditVisibility = CancelEditVisibility;
                objJobScraper.TemplateId = TemplateID;
                //  objJobScraper.CampaignName = campaignDetails.CampaignName;
                objJobScraper.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objJobScraper.CampaignName;
                objJobScraper.CampaignButtonContent = CampaignButtonContent;
                objJobScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objJobScraper.JobScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objJobScraper.ObjViewModel.JobScraperModel =
                    templateDetails.ActivitySettings.GetActivityModel<JobScraperModel>(objJobScraper.ObjViewModel
                        .Model);

                objJobScraper.MainGrid.DataContext = objJobScraper.ObjViewModel;

                TabSwitcher.ChangeTabIndex(5, 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}