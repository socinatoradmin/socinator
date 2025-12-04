using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.SalesNavigatorUserScraper;
using Newtonsoft.Json;
using UserScraperModel = LinkedDominatorCore.LDModel.SalesNavigatorScraper.UserScraperModel;

namespace LinkedDominatorUI.Utility.Scraper
{
    public class SalesUserScraperFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SalesNavigatorUserScraper)
                .AddReportFactory(new SalesUserScraperReport())
                .AddViewCampaignFactory(new SalesUserScraperViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class SalesUserScraperReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public static CampaignDetails CampaignDetails;

        public string ActivityType = DominatorHouseCore.Enums.ActivityType.SalesNavigatorUserScraper.ToString();

        public UserScraperModel UserScraperModel;
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UserScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedUsersReportModel.Clear();
            CampaignDetails = campaignDetails;
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var count = 0;

            #region get data from InteractedUsers table and add to InteractedUsersReportModel

            dataBase.GetInteractedUsers(ActivityType).ForEach(
                reportItem =>
                {
                    var data =
                        JsonConvert.DeserializeObject<LinkedinUser>(reportItem
                            .DetailedUserInfo);
                    InteractedUsersReportModel.Add(new InteractedUsersReportModel
                    {
                        Id = ++count,
                        AccountEmail = reportItem.AccountEmail,
                        QueryType = reportItem.QueryType,
                        QueryValue = reportItem.QueryValue,
                        ActivityType = reportItem.ActivityType,
                        UserFullName = data.FullName,
                        UserProfileUrl = reportItem.UserProfileUrl,
                        DetailedUserInfo = reportItem.DetailedUserInfo,
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
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User FullName", ColumnBindingText = "UserFullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User ProfileUrl", ColumnBindingText = "UserProfileUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped DateTime", ColumnBindingText = "InteractionDateTime"}
                };


            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersReportModel);

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
                        QueryType = reportItem.QueryType,
                        QueryValue = reportItem.QueryValue,
                        ActivityType = reportItem.ActivityType,
                        UserFullName = reportItem.UserFullName,
                        UserProfileUrl = reportItem.UserProfileUrl,
                        DetailedUserInfo = reportItem.DetailedUserInfo,
                        InteractionDatetime = reportItem.InteractionDatetime
                    }
                );

            return AccountsInteractedUsers;
        }


        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();


            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                UserScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templatesFileManager.Get()
                    .FirstOrDefault(x => x.Id == CampaignDetails.TemplateId).ActivitySettings);
                if (UserScraperModel.IsCheckedWithoutVisiting || UserScraperModel.IsCheckedVisitOnly)
                {
                    Header =
                        "AccountEmail,QueryType,QueryValue,ActivityType,Full Name,Member Id,SalesNavigator Profile Url,Profile Url,Profile Pic Url ,Connection Type,Number Of SharedConnections,Location,Headline Title,Current CompanyName,Interaction Datetime";
                    InteractedUsersReportModel.ToList().ForEach(reportItem =>
                    {
                        try
                        {
                            var objInfo =
                                JsonConvert.DeserializeObject<SalesNavigatorScraperDetails>(
                                    Uri.UnescapeDataString(reportItem.DetailedUserInfo));

                            csvData.Add(reportItem.AccountEmail + "," + reportItem.QueryType + "," +
                                        reportItem.QueryValue.AsCsvData() + "," + reportItem.ActivityType + "," +
                                        objInfo.FullName.AsCsvData() + "," + objInfo.MemberId + "," +
                                        objInfo.SalesNavigatorProfileUrl.AsCsvData() + "," +
                                        objInfo.ProfileUrl.AsCsvData() + "," +
                                        objInfo.ProfilePicUrl + "," + objInfo.ConnectionType + "," +
                                        objInfo.NumberOfSharedConnections + "," + objInfo.Location.AsCsvData() + "," +
                                        objInfo.HeadlineTitle.AsCsvData() + "," + objInfo.CurrentCompany.AsCsvData() +
                                        "," +
                                        reportItem.InteractionDateTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                }
                else
                {
                    if (UserScraperModel.IsCheckedOnlyDetailsRequiredToSendConnectionRequest)
                    {
                        #region For OnlyDetailsRequiredToSendConnectionRequest

                        Header =
                            "AccountEmail,QueryType,QueryValue,ActivityType,Full Name ,Member Id,SalesNavigator Profile Url,Details Required To SendConnectionRequest,Interaction Datetime";
                        InteractedUsersReportModel.ToList().ForEach(reportItem =>
                        {
                            try
                            {
                                var objInfo =
                                    JsonConvert.DeserializeObject<SalesNavigatorScraperDetails>(
                                        Uri.UnescapeDataString(reportItem.DetailedUserInfo));

                                csvData.Add(reportItem.AccountEmail + "," + reportItem.QueryType + "," +
                                            reportItem.QueryValue.AsCsvData() + "," + reportItem.ActivityType + "," +
                                            objInfo.FullName.AsCsvData() + "," + objInfo.MemberId + "," +
                                            objInfo.SalesNavigatorProfileUrl.AsCsvData() + "," +
                                            objInfo.DetailsRequiredToSendConnectionRequest.AsCsvData() + "," +
                                            reportItem.InteractionDateTime);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }
                        });

                        #endregion
                    }
                    else
                    {
                        #region For All Details

                        Header =
                            "AccountEmail,QueryType,QueryValue,ActivityType,Full Name ,Member Id,SalesNavigator Profile Url, Profile Url,Details Required To SendConnectionRequest,Profile Pic Url ,Connection Type ,Number Of Connections ,Location ,Industry ,Headline Title ,Current Title ,Current CompanyName ,Current CompanyId ,Current Company IndustryType ,Current Company Location ,Current CompanySize ,Current Companylogo ,Current Company FoundingYear ,Current Company Description ,Current Company Headquaters ,Current Company Website ,Profile Summary ,Email ,Phone Number ,Past Titles ,Experience ,Past Campanies ,Skills ,Education ,Interaction Datetime";
                        InteractedUsersReportModel.ToList().ForEach(reportItem =>
                        {
                            try
                            {
                                var objInfo =
                                    JsonConvert.DeserializeObject<SalesNavigatorScraperDetails>(
                                        Uri.UnescapeDataString(reportItem.DetailedUserInfo));

                                csvData.Add(reportItem.AccountEmail + "," + reportItem.QueryType + ",\"" +
                                            reportItem.QueryValue.Replace("\"", "\"\"") + "\"," +
                                            reportItem.ActivityType + "," +
                                            objInfo.FullName.AsCsvData() + "," + objInfo.MemberId.AsCsvData() + ",\"" +
                                            objInfo.SalesNavigatorProfileUrl.Replace("\"", "\"\"") + "\"," +
                                            objInfo.ProfileUrl.AsCsvData() + "," +
                                            objInfo.DetailsRequiredToSendConnectionRequest.AsCsvData() + "," +
                                            objInfo.ProfilePicUrl.AsCsvData() + "," + objInfo.ConnectionType + "," +
                                            objInfo.NumberOfConnections + "," +
                                            Regex.Replace(objInfo.Location, "[\n\r,]", " ") + "," +
                                            objInfo.Industry.AsCsvData() + "," +
                                            Regex.Replace(objInfo.HeadlineTitle.AsCsvData(), "[\n\r,]", " ") + "," +
                                            objInfo.CurrentTitle.AsCsvData() + "," +
                                            objInfo.CurrentCompany.AsCsvData() + "," + objInfo.CurrentCompanyId + "," +
                                            objInfo.CurrentCompanyIndustryType.AsCsvData() + "," +
                                            objInfo.CurrentCompanyLocation.AsCsvData() + "," +
                                            objInfo.CurrentCompanySize + "," + objInfo.CurrentCompanyLogo.AsCsvData() +
                                            "," + objInfo.CurrentCompanyFoundingYear + "," +
                                            objInfo.CurrentCompanyDescription.AsCsvData() + "," +
                                            Regex.Replace(objInfo.CurrentCompanyHeadquarters, "[\n\r,]", " ") + "," +
                                            Regex.Replace(objInfo.CurrentCompanyWebsite, "[\n\r,]", " ") + "," +
                                            Regex.Replace(objInfo.ProfileSummary, "[\n\r,]", " ") + "," +
                                            objInfo.Email + "," + objInfo.PhoneNumber + "," +
                                            Regex.Replace(objInfo.PastTitles, "[\n\r,]", " ") + "," +
                                            Regex.Replace(objInfo.Experience, "[\n\r,]", " ") + "," +
                                            objInfo.PastCompanies.AsCsvData() + ",\"" +
                                            Regex.Replace(objInfo.Skills, "[\n\r,]", "") + "\"," +
                                            Regex.Replace(objInfo.Education, "[\n\r,]", "") + "," +
                                            reportItem.InteractionDateTime);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }
                        });

                        #endregion
                    }
                }
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
                AccountsInteractedUsers.ToList().ForEach(reportItem =>
                {
                    try
                    {
                        var objInfo =
                            JsonConvert.DeserializeObject<SalesNavigatorScraperDetails>(
                                Uri.UnescapeDataString(reportItem.DetailedUserInfo));
                        if (!objInfo.IsVisiting)
                        {
                            if (string.IsNullOrEmpty(Header))
                                Header =
                                    "QueryType,QueryValue,ActivityType,Full Name,Member Id,SalesNavigator Profile Url,Profile Url,Profile Pic Url ,Connection Type,Number Of SharedConnections,Location,Headline Title,Current CompanyName,Interaction Datetime";
                            csvData.Add(reportItem.QueryType + "," + reportItem.QueryValue.AsCsvData() + "," +
                                        reportItem.ActivityType + "," +
                                        objInfo.FullName.AsCsvData() + "," + objInfo.MemberId.AsCsvData() + "," +
                                        objInfo.SalesNavigatorProfileUrl.AsCsvData() + "," + objInfo.ProfileUrl.AsCsvData() + "," +
                                        objInfo.ProfilePicUrl.AsCsvData() + "," + objInfo.ConnectionType.AsCsvData() + "," +
                                        objInfo.NumberOfSharedConnections.AsCsvData() + "," + objInfo.Location.AsCsvData() + "," +
                                        objInfo.HeadlineTitle.AsCsvData() + "," + objInfo.CurrentCompany.AsCsvData() + "," +
                                        reportItem.InteractionDatetime);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Header))
                                Header =
                                    "QueryType,QueryValue,ActivityType,Full Name ,Member Id,SalesNavigator Profile Url,Details Required To SendConnectionRequest,Profile Pic Url ,Connection Type ,Number Of Connections ,Location ,Industry ,Headline Title ,Current Title ,Current CompanyName ,Current CompanyId ,Current Company IndustryType ,Current Company Location ,Current CompanySize ,Current Companylogo ,Current Company FoundingYear ,Current Company Description ,Current Company Headquaters ,Current Company Website ,Profile Summary ,Email ,Phone Number ,Past Titles ,Experience ,Past Campanies ,Skills ,Education ,Interaction Datetime";
                            csvData.Add(reportItem.QueryType + "," + reportItem.QueryValue.AsCsvData() + "," +
                                        reportItem.ActivityType + "," +
                                        objInfo.FullName.AsCsvData() + "," + objInfo.MemberId.AsCsvData() + "," +
                                        objInfo.SalesNavigatorProfileUrl.AsCsvData() + "," +
                                        objInfo.DetailsRequiredToSendConnectionRequest.AsCsvData() + "," + objInfo.ProfilePicUrl.AsCsvData() +
                                        "," + objInfo.ConnectionType.AsCsvData() + "," +
                                        objInfo.NumberOfConnections.AsCsvData() + "," + objInfo.Location.AsCsvData()+ "," + objInfo.Industry.AsCsvData() +
                                        "," + objInfo.HeadlineTitle.AsCsvData() + "," +
                                        objInfo.CurrentTitle.AsCsvData() + "," + objInfo.CurrentCompany.AsCsvData() + "," +
                                        objInfo.CurrentCompanyId.AsCsvData() + "," + objInfo.CurrentCompanyIndustryType.AsCsvData() + "," +
                                        objInfo.CurrentCompanyLocation.AsCsvData() + "," + objInfo.CurrentCompanySize.AsCsvData() + "," +
                                        objInfo.CurrentCompanyLogo.AsCsvData() + "," + objInfo.CurrentCompanyFoundingYear.AsCsvData() + "," +
                                        objInfo.CurrentCompanyDescription.AsCsvData() + "," + objInfo.CurrentCompanyHeadquarters.AsCsvData() +
                                        "," + objInfo.CurrentCompanyWebsite.AsCsvData() + "," + objInfo.ProfileSummary.AsCsvData() + "," +
                                        objInfo.Email.AsCsvData() + "," + objInfo.PhoneNumber.AsCsvData() + "," + objInfo.PastTitles.AsCsvData() + "," +
                                        objInfo.Experience.AsCsvData() + "," + objInfo.PastCompanies.AsCsvData() + "," + objInfo.Skills.AsCsvData() + "," +
                                        objInfo.Education.AsCsvData() + "," + reportItem.InteractionDatetime);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }

    public class SalesUserScraperViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            try
            {
                var objUserScraper = UserScraper.GetSingeltonObjectSalesUserScraper();
                objUserScraper.IsEditCampaignName = isEditCampaignName;
                objUserScraper.CancelEditVisibility = cancelEditVisibility;
                objUserScraper.TemplateId = templateId;
                //objUserScraper.CampaignName = campaignDetails.CampaignName;
                objUserScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objUserScraper.CampaignName;
                objUserScraper.CampaignButtonContent = campaignButtonContent;
                objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                      $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objUserScraper.ObjViewModel.UserScraperModel =
                    templateDetails.ActivitySettings.GetActivityModel<UserScraperModel>(objUserScraper.ObjViewModel
                        .Model);

                objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;

                TabSwitcher.ChangeTabIndex(7, 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}