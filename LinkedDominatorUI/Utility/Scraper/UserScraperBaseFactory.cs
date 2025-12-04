using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using LinkedDominatorUI.LDViews.Scraper;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.Scraper
{
    public class UserScraperBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UserScraper)
                .AddReportFactory(new UserScraperReport())
                .AddViewCampaignFactory(new UserScraperViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class UserScraperReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedUsersReportModel> InteractedUsersReportModel =
            new ObservableCollection<InteractedUsersReportModel>();

        public static List<InteractedUsers> AccountsInteractedUsers = new List<InteractedUsers>();

        public static CampaignDetails CampaignDetails;

        public string activityType = ActivityType.UserScraper.ToString();

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

            dataBase.GetInteractedUsers(activityType).ForEach(
                ReportItem =>
                {
                    InteractedUsersReportModel.Add(new InteractedUsersReportModel
                    {
                        Id = ++count,
                        AccountEmail = ReportItem.AccountEmail,
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        DetailedUserInfo = ReportItem.DetailedUserInfo,
                        ConnectedDateTime = ReportItem.ConnectedTime,
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
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User FullName", ColumnBindingText = "UserFullName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "User ProfileUrl", ColumnBindingText = "UserProfileUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Connected Time", ColumnBindingText = "ConnectedDateTime"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "Scraped DateTime", ColumnBindingText = "InteractionDateTime"}
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
                        QueryType = ReportItem.QueryType,
                        QueryValue = ReportItem.QueryValue,
                        ActivityType = ReportItem.ActivityType,
                        UserFullName = ReportItem.UserFullName,
                        UserProfileUrl = ReportItem.UserProfileUrl,
                        DetailedUserInfo = ReportItem.DetailedUserInfo,
                        ConnectedTime = ReportItem.ConnectedTime,
                        InteractionDatetime = ReportItem.InteractionDatetime
                    }
                );

            return AccountsInteractedUsers;
        }

        public void ExportReports(ActivityType activityType, string FileName, ReportType reportType)
        {
            var CsvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                var templatesFileManager =
                    InstanceProvider.GetInstance<ITemplatesFileManager>();
                UserScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templatesFileManager.Get()
                    .FirstOrDefault(x => x.Id == CampaignDetails.TemplateId).ActivitySettings);
                if (UserScraperModel.IsCheckedVisitOnly || UserScraperModel.IsCheckedWithoutVisiting)
                {
                    #region WithoutVisiting and VistOnly Export

                    Header =
                        "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,UserFirstName,UserLastName,UserFullname,UserProfileUrl,UserProfilePicUrl,ConnectionType,HeadlineTitle,Location,Industry,ConnectedTime,ScrapedDateTime";

                    InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                    {
                        try
                        {
                            var objInfo =
                                JsonConvert.DeserializeObject<UserScraperDetailedInfo>(
                                    Uri.UnescapeDataString(ReportItem.DetailedUserInfo));

                            CsvData.Add(ReportItem.AccountEmail + "," + objInfo.AccountUserFullName + "," +
                                        objInfo.AccountUserProfileUrl + "," +
                                        ReportItem.QueryType + "," + ReportItem.QueryValue.AsCsvData() + "," +
                                        ReportItem.ActivityType + "," + objInfo.Firstname.AsCsvData() + "," +
                                        objInfo.Lastname.AsCsvData() + "," +
                                        ReportItem.UserFullName.AsCsvData() + "," +
                                        ReportItem.UserProfileUrl.AsCsvData() + "," +
                                        objInfo.ProfilePicUrl.AsCsvData() + "," + objInfo.ConnectionType + "," +
                                        objInfo.HeadlineTitle.AsCsvData() + "," + objInfo.Location.AsCsvData() + "," +
                                        objInfo.Industry.AsCsvData() + "," + ReportItem.ConnectedDateTime + "," +
                                        ReportItem.InteractionDateTime);
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
                    #region  Export For Scrap after Visting 

                    Header =
                        "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,UserFirstName,UserLastName,UserFullName,UserProfileUrl,HeadlineTitle,EmailId,PersonalPhoneNumber,PersonalWebsites,Birthdate,TwitterUrl,TitleCurrent,CompanyCurrent,CurrentCompanyUrl,CompanyDescription,CurrentCompanyWebsite,Location,Industry,ConnectionCount,Recommendation,Skill,Experience,EducationCollection,PastTitles,PastCompany,ConnectedTime,ScrapedDateTime";

                    InteractedUsersReportModel.ToList().ForEach(ReportItem =>
                    {
                        try
                        {
                            var objInfo =
                                JsonConvert.DeserializeObject<UserScraperDetailedInfo>(
                                    Uri.UnescapeDataString(ReportItem.DetailedUserInfo));

                            CsvData.Add(ReportItem.AccountEmail + "," + objInfo.AccountUserFullName + "," +
                                        objInfo.AccountUserProfileUrl + "," +
                                        ReportItem.QueryType + "," + ReportItem.QueryValue.AsCsvData() + "," +
                                        ReportItem.ActivityType + "," + objInfo.Firstname.AsCsvData() + "," +
                                        objInfo.Lastname.AsCsvData() + "," +
                                        ReportItem.UserFullName.AsCsvData() + "," + ReportItem.UserProfileUrl + "," +
                                        objInfo.HeadlineTitle.AsCsvData() + "," + objInfo.EmailId.AsCsvData() + "," +
                                        objInfo.PersonalPhoneNumber.AsCsvData() + "," +
                                        objInfo.PersonalWebsites.AsCsvData() + "," + objInfo.Birthdate + "," +
                                        objInfo.TwitterUrl + "," +
                                        objInfo.TitleCurrent.AsCsvData() + "," + objInfo.CompanyCurrent.AsCsvData() +
                                        "," + objInfo.CurrentCompanyUrl.AsCsvData() + "," +
                                        objInfo.CompanyDescription.AsCsvData() + "," +
                                        objInfo.CurrentCompanyWebsite.AsCsvData() + "," + objInfo.Location.AsCsvData() +
                                        "," + objInfo.Industry.AsCsvData() + "," + objInfo.Connection + "," +
                                        objInfo.Recommendation.AsCsvData() + "," + objInfo.Skill.AsCsvData() + "," +
                                        objInfo.Experience.AsCsvData() + "," + objInfo.EducationCollection.AsCsvData() +
                                        "," + objInfo.PastTitles.AsCsvData() + "," +
                                        objInfo.PastCompany.AsCsvData() + "," + objInfo.ConnectedTime + "," +
                                        ReportItem.InteractionDateTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });

                    #endregion
                }
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
                AccountsInteractedUsers.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var objInfo =
                            JsonConvert.DeserializeObject<UserScraperDetailedInfo>(
                                Uri.UnescapeDataString(ReportItem.DetailedUserInfo));
                        if (!objInfo.IsVisiting)
                        {
                            #region WithoutVisiting and VistOnly Export

                            if (string.IsNullOrEmpty(Header))
                                Header =
                                    "QueryType,QueryValue,ActivityType,UserFirstName,UserLastName,UserProfileUrl,UserProfilePicUrl,ConnectionType,HeadlineTitle,Location,Industry,ConnectedTime,ScrapedDateTime";

                            CsvData.Add(ReportItem.QueryType + "," + ReportItem.QueryValue.AsCsvData() + "," +
                                        ReportItem.ActivityType + "," +
                                        objInfo.Firstname.AsCsvData() + "," + objInfo.Lastname.AsCsvData() + "," +
                                        ReportItem.UserProfileUrl.AsCsvData() + "," +
                                        objInfo.ProfilePicUrl.AsCsvData() + "," + objInfo.ConnectionType.AsCsvData() +
                                        "," + objInfo.HeadlineTitle.AsCsvData() + "," + objInfo.Location.AsCsvData() +
                                        "," + objInfo.Industry.AsCsvData() + "," + ReportItem.ConnectedTime + "," +
                                        ReportItem.InteractionDatetime);

                            #endregion
                        }
                        else
                        {
                            #region Export For Scrap after Visting 

                            if (string.IsNullOrEmpty(Header))
                                Header =
                                    "QueryType,QueryValue,ActivityType,UserFirstName,UserLastName,UserProfileUrl,HeadlineTitle,EmailId,PersonalPhoneNumber,PersonalWebsites,Birthdate,TwitterUrl,TitleCurrent,CompanyCurrent,CurrentCompanyUrl,CompanyDescription,CurrentCompanyWebsite,Location,Industry,ConnectionCount,Recommendation,Skill,Experience,EducationCollection,PastTitles,PastCompany,ConnectedTime,ScrapedDateTime";
                            CsvData.Add(ReportItem.QueryType + "," + ReportItem.QueryValue.AsCsvData() + "," +
                                        ReportItem.ActivityType + "," +
                                        objInfo.Firstname.AsCsvData() + "," + objInfo.Lastname.AsCsvData() + "," +
                                        ReportItem.UserProfileUrl.AsCsvData() + "," +
                                        objInfo.HeadlineTitle.AsCsvData() + "," + objInfo.EmailId + "," +
                                        objInfo.PersonalPhoneNumber + "," + objInfo.PersonalWebsites.AsCsvData() + "," +
                                        objInfo.Birthdate.AsCsvData() + "," + objInfo.TwitterUrl.AsCsvData() + "," +
                                        objInfo.TitleCurrent.AsCsvData() + "," + objInfo.CompanyCurrent.AsCsvData() +
                                        "," + objInfo.CurrentCompanyUrl.AsCsvData() + "," +
                                        objInfo.CompanyDescription.AsCsvData() + "," +
                                        objInfo.CurrentCompanyWebsite.AsCsvData() + "," + objInfo.Location.AsCsvData() +
                                        "," + objInfo.Industry.AsCsvData() + "," + objInfo.Connection.AsCsvData() +
                                        "," +
                                        objInfo.Recommendation.AsCsvData() + "," + objInfo.Skill.AsCsvData() + "," +
                                        objInfo.Experience.AsCsvData() + "," + objInfo.EducationCollection.AsCsvData() +
                                        "," + objInfo.PastTitles.AsCsvData() + "," +
                                        objInfo.PastCompany.AsCsvData() + "," + objInfo.ConnectedTime.AsCsvData() +
                                        "," + ReportItem.InteractionDatetime);

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });

            #endregion

            Utilities.ExportReports(FileName, Header, CsvData);
        }
    }

    public class UserScraperViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
                objUserScraper.IsEditCampaignName = IsEditCampaignName;
                objUserScraper.CancelEditVisibility = CancelEditVisibility;
                objUserScraper.TemplateId = TemplateID;
                // objUserScraper.CampaignName = campaignDetails.CampaignName;
                objUserScraper.CampaignName = CampaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objUserScraper.CampaignName;
                objUserScraper.CampaignButtonContent = CampaignButtonContent;
                objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                      $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objUserScraper.ObjViewModel.UserScraperModel =
                    templateDetails.ActivitySettings.GetActivityModel<UserScraperModel>(objUserScraper.ObjViewModel
                        .Model);

                objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;

                TabSwitcher.ChangeTabIndex(5, 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}