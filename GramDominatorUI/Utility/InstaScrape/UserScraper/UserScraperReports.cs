using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GramDominatorUI.Utility.InstaScrape.UserScraper
{
    internal class UserScraperReports : IGdReportFactory
    {
        private static string templateId = string.Empty;

        private static readonly Dictionary<string, string> lstRequireddata = new Dictionary<string, string>();

        private static readonly ObservableCollection<UserScrapeReportDetails> UserScraperReportModelCampaign =
            new ObservableCollection<UserScrapeReportDetails>();

        private readonly ObservableCollection<UserScrapeReportDetails> UserScraperReportModel =
            new ObservableCollection<UserScrapeReportDetails>();

        private IAccountsFileManager _accountsFileManager;

        private List<string> uniqueUser = new List<string>();
        public ModuleSetting ModuleSetting { get; set; }

        private static List<UserScrapeReportDetails> UserScraperReportModelAccount { get; } =
            new List<UserScrapeReportDetails>();

        private static List<UserScrapeReportDetails> UserScraperReportModelAccount1 { get; } =
            new List<UserScrapeReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<UserScraperModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region  Dont delete these 2 line because we are getting template id from campaign deltils as staic it works on while exporting data

            templateId = string.Empty;
            templateId = campaignDetails.TemplateId;

            #endregion

            // Need to be cleared data for adding into static variable.
            UserScraperReportModelCampaign.Clear();

            #region get data from InteractedUsers table and add to UserScrapReportModel

            var sNo = 0;
            var data = new UserRequiredData();
            var Alldata = dataBase.Get<InteractedUsers>();
            foreach (var datas in Alldata)
            {
                if (!string.IsNullOrEmpty(datas.RequiredData)) data = GetRequiredData(datas.RequiredData);

                UserScraperReportModelCampaign.Add(new UserScrapeReportDetails
                {
                    Id = ++sNo,
                    AccountUsername = datas.Username,
                    QueryType = datas.QueryType,
                    QueryValue = datas.Query,
                    ScrapedUsername = datas.InteractedUsername,
                    ScrapedUserId = datas.InteractedUserId,
                    IsPrivate = datas.IsPrivate,
                    IsVerified = datas.IsVerified,
                    IsBusiness = datas.IsBusiness,
                    IsProfilePicAvailable = datas.IsProfilePicAvailable != null && datas.IsProfilePicAvailable.Value,
                    Date = datas.Date.EpochToDateTimeUtc().ToLocalTime(),
                    ProfilePicUrl = datas.ProfilePicUrl,
                    UserName = data.UserName,
                    UserId = data.UserId,
                    UserFullName = data.UserFullName,
                    IsFollowedAlready = data.IsFollowedAlready,
                    PostCount = data.PostCount != 0 ? Convert.ToString(data.PostCount) : string.Empty,
                    FollowerCount = data.FollowerCount != 0 ? Convert.ToString(data.FollowerCount) : string.Empty,
                    FollowingCount = data.FollowingCount != 0 ? Convert.ToString(data.FollowingCount) : string.Empty,
                    EamilId = data.EamilId,
                    ContactNo = data.ContactNo,
                    EngagementRate = data.EngagementRate,
                    CommentCount = data.CommentCount,
                    LikeCount = data.LikeCount,
                    TaggedUser = datas.TaggedUser,
                    Gender = datas.Gender,
                    Biography = data.Biography,
                    IsBusinessAccount = data.IsBusinessAccounts,
                    BusinessCategory = data.BusinessCategory
                });

                try
                {
                    if (datas.InteractedUserId == null)
                        continue;
                    var key = lstRequireddata.ContainsKey(datas.InteractedUserId);
                    if (!key) lstRequireddata.Add(datas.InteractedUserId, datas.RequiredData);
                    if (key)
                    {
                        lstRequireddata.TryGetValue(datas.InteractedUserId, out string reqData);
                        if(string.IsNullOrEmpty(reqData))
                            lstRequireddata[datas.InteractedUserId] = datas.RequiredData;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            #endregion

            #region Generate Reports column with data

            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKey﻿﻿﻿﻿Id".FromResourceDictionary(), ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyAccountUsername".FromResourceDictionary(),
                        ColumnBindingText = "AccountUsername"
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
                        ColumnHeaderText = "LangKeyScrapedUsername".FromResourceDictionary(),
                        ColumnBindingText = "ScrapedUsername"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyIsFollowedAlready".FromResourceDictionary(),
                        ColumnBindingText = "IsFollowedAlready"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyProfilePic".FromResourceDictionary(),
                        ColumnBindingText = "ProfilePicUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyTaggedUsers".FromResourceDictionary(),
                        ColumnBindingText = "TaggedUser"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyUsername".FromResourceDictionary(), ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyUserID".FromResourceDictionary(), ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyGender".FromResourceDictionary(), ColumnBindingText = "Gender"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserFullName".FromResourceDictionary(),
                        ColumnBindingText = "UserFullName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostCount".FromResourceDictionary(), ColumnBindingText = "PostCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFollowersCount".FromResourceDictionary(),
                        ColumnBindingText = "FollowerCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFollowingsCount".FromResourceDictionary(),
                        ColumnBindingText = "FollowingCount"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyEmailID".FromResourceDictionary(), ColumnBindingText = "EamilId"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyContactNo".FromResourceDictionary(), ColumnBindingText = "ContactNo"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyEngagementRate".FromResourceDictionary(),
                        ColumnBindingText = "EngagementRate"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentCount".FromResourceDictionary(),
                        ColumnBindingText = "CommentCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyLikesCount".FromResourceDictionary(), ColumnBindingText = "LikeCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyBiography".FromResourceDictionary(), ColumnBindingText = "Biography"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyBusinessAccounts".FromResourceDictionary(),
                        ColumnBindingText = "IsBusinessAccount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyBusinessCategory".FromResourceDictionary(),
                        ColumnBindingText = "BusinessCategory"
                    }
                };

            #endregion

            return new ObservableCollection<object>(UserScraperReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            var activityType = ActivityType.UserScraper.ToString();
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == activityType).ToList();
            // Need to be cleared data for adding into static variable.
            UserScraperReportModelAccount.Clear();
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var check = _accountsFileManager.GetAccountById(dataBase.DbOperations.AccountId);
            templateId = check.ActivityManager.LstModuleConfiguration[0].TemplateId;
            var sNo = 0;
            var data = new UserRequiredData();
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
            {
                if (!string.IsNullOrEmpty(report.RequiredData)) data = GetRequiredData(report.RequiredData);

                UserScraperReportModelAccount.Add(
                    new UserScrapeReportDetails
                    {
                        Id = ++sNo,
                        QueryType = report.QueryType,
                        QueryValue = report.Query,
                        ActivityType = ActivityType.UserScraper,
                        AccountUsername = report.Username,
                        ScrapedUsername = report.InteractedUsername,
                        ScrapedUserId = report.InteractedUserId,
                        IsPrivate = report.IsPrivate,
                        IsBusiness = report.IsBusiness,
                        IsVerified = report.IsVerified,
                        IsProfilePicAvailable = report.IsProfilePicAvailable ?? false,
                        IsFollowedAlready = data.IsFollowedAlready,
                        ProfilePicUrl = report.ProfilePicUrl,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                        UserName = data.UserName,
                        UserId = data.UserId,
                        UserFullName = data.UserFullName,
                        PostCount = data.PostCount != 0 ? Convert.ToString(data.PostCount) : string.Empty,
                        FollowerCount = data.FollowerCount != 0 ? Convert.ToString(data.FollowerCount) : string.Empty,
                        FollowingCount =
                            data.FollowingCount != 0 ? Convert.ToString(data.FollowingCount) : string.Empty,
                        EamilId = data.EamilId,
                        ContactNo = data.ContactNo,
                        EngagementRate = data.EngagementRate,
                        CommentCount = data.CommentCount,
                        LikeCount = data.LikeCount,
                        TaggedUser = report.TaggedUser,
                        Gender = report.Gender,
                        Biography = data.Biography,
                        IsBusinessAccount = data.IsBusinessAccounts,
                        BusinessCategory = data.BusinessCategory
                    });
                try
                {
                    if (report.InteractedUserId == null)
                        continue;
                    var key = lstRequireddata.ContainsKey(report.InteractedUserId);
                    if (!key) lstRequireddata.Add(report.InteractedUserId, report.RequiredData);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return UserScraperReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();
            var data = new UserRequiredData();
            var IsRequiredData = new IsUserRequiredData();
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var model = templatesFileManager.GetTemplateById(templateId);
            var Header = string.Empty;
            ModuleSetting = JsonConvert.DeserializeObject<ModuleSetting>(model.ActivitySettings);
            var userScraper = JsonConvert.DeserializeObject<UserScraperModel>(model.ActivitySettings);
            if (userScraper.IsChkRequiredData)
            {
                Header = "Query Type, Query, Activity type, Account Username";

                foreach (var isSelect in userScraper.ListUserRequiredData)
                {
                    if (isSelect == null || isSelect.ItemName == "All")
                        continue;
                    if (isSelect.IsSelected)
                        Header += "," + isSelect.ItemName;
                }
            }
            else
            {
                Header = "Query Type, Query, Activity type, Account Username, Interacted Username, " +
                         "Interaction User Id, Is Private, Is Verified, " +
                         "Is Profile Pic Available, Profile Pic Url, Scrape Date,Tagged User,Gender, Full Name, IsFollowed Already, Post Count, Follower Count, Following Count,Email Id,Contact Number,Engagement Rate(%),Number Of Comments,Number Of Like,Biography,Business Accounts,Business Category";
            }

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                try
                {
                    foreach (var report in UserScraperReportModelCampaign)
                        if (report.ScrapedUserId != null)
                        {
                            var requiredData = lstRequireddata[report.ScrapedUserId];
                            if (string.IsNullOrEmpty(requiredData) && !userScraper.IsChkRequiredData)
                            {
                                try
                                {
                                    csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.ActivityType +
                                                "," + report.AccountUsername +
                                                "," + report.ScrapedUsername + "," + report.ScrapedUserId + "," +
                                                report.IsPrivate + "," + report.IsVerified + "," +
                                                report.IsProfilePicAvailable + "," +
                                                report.ProfilePicUrl + "," + report.Date + "," + report.TaggedUser +
                                                "," + report.Gender);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }
                            else
                            {
                                var requiredDatas = lstRequireddata[report.ScrapedUserId];
                                if (!string.IsNullOrEmpty(requiredDatas))
                                {
                                    var datass = GetRequiredData1(requiredDatas, userScraper);
                                    if (ModuleSetting.UserFilterModel.IsUserMustHaveEmailIdAndPhoneNO
                                    ) //this is for temp basis
                                    {
                                        var emailId = string.Empty;
                                        var phoneNo = string.Empty;
                                        try
                                        {
                                            emailId = datass["Email Id"];
                                        }
                                        catch (Exception)
                                        {
                                            emailId = datass["Email_Id"];
                                        }

                                        try
                                        {
                                            phoneNo = datass["Contact Number"];
                                        }
                                        catch (Exception)
                                        {
                                            phoneNo = datass["Phone_Number"];
                                        }

                                        // string Business = datass["Business Acocunts"];
                                        if (!string.IsNullOrEmpty(emailId) || !string.IsNullOrEmpty(phoneNo))
                                        {
                                            var csv = GetCsvData(report, userScraper, ref Header, datass);
                                            csvData.Add(csv);
                                            continue;
                                        }
                                    }

                                    if (ModuleSetting.UserFilterModel.IsUserMustHaveEmailId)
                                    {
                                        var emailId = string.Empty;
                                        try
                                        {
                                            emailId = datass["Email Id"];
                                        }
                                        catch (Exception)
                                        {
                                            emailId = datass["Email_Id"];
                                        }

                                        // string phoneNo = datass["Contact Number"];
                                        if (!string.IsNullOrEmpty(emailId))
                                        {
                                            var csv = GetCsvData(report, userScraper, ref Header, datass);
                                            csvData.Add(csv);

                                            continue;
                                        }
                                    }

                                    if (ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO)
                                    {
                                        var phoneNo = string.Empty;
                                        try
                                        {
                                            phoneNo = datass["Contact Number"];
                                        }
                                        catch (Exception)
                                        {
                                            phoneNo = datass["Phone_Number"];
                                        }

                                        // string Business = datass["Business Acocunts"];
                                        if (!string.IsNullOrEmpty(phoneNo))
                                        {
                                            var csv = GetCsvData(report, userScraper, ref Header, datass);
                                            csvData.Add(csv);
                                            continue;
                                        }
                                    }

                                    if (ModuleSetting.UserFilterModel.IsUserMustHaveBusinessAccount)
                                    {
                                        var Business = datass["Business Acocunts"];
                                        if (!string.IsNullOrEmpty(Business) && Business.Contains("True"))
                                        {
                                            var csv = GetCsvData(report, userScraper, ref Header, datass);
                                            csvData.Add(csv);
                                        }
                                    }
                                    else if (!ModuleSetting.UserFilterModel.IsUserMustHaveEmailId &&
                                             !ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO &&
                                             !ModuleSetting.UserFilterModel.IsUserMustHaveBusinessAccount)
                                    {
                                        //var csv1 = GetCsvData(report, userScraper, ref Header, datass);
                                        var csv = GetCommonCsv(report);
                                        foreach (var dataList in userScraper.ListUserRequiredData)
                                        {
                                            if (dataList.IsSelected)
                                                csv += ","+ datass[dataList.ItemName];
                                        }
                                        csvData.Add(csv);
                                    }
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                }

                #region old code

                //UserScraperReportModelCampaign.ToList().ForEach(report =>
                //{       
                //    if (report.ScrapedUserId != null)
                //    {
                //        var requiredData = lstRequireddata[report.ScrapedUserId];
                //        if (string.IsNullOrEmpty(requiredData) && !userScraper.IsChkRequiredData)
                //        {
                //            try
                //            {
                //                csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.ActivityType + "," + report.AccountUsername +
                //                            "," + report.ScrapedUsername + "," + report.ScrapedUserId + "," + report.IsPrivate + "," + report.IsVerified + "," + report.IsProfilePicAvailable + "," +
                //                            report.ProfilePicUrl + "," + report.Date+","+report.TaggedUser+","+report.Gender);
                //            }
                //            catch (Exception ex)
                //            {
                //                Console.WriteLine(ex.StackTrace);
                //            }
                //        }
                //        else
                //        {
                //            var requiredDatas = lstRequireddata[report.ScrapedUserId];
                //            if (!string.IsNullOrEmpty(requiredDatas))
                //            {
                //               var datass = GetRequiredData1(requiredDatas, userScraper);
                //                if (ModuleSetting.UserFilterModel.IsUserMustHaveEmailIdAndPhoneNO)
                //                {
                //                    string emailId = datass["Email Id"];
                //                    string phoneNo = datass["Contact Number"];
                //                    // string Business = datass["Business Acocunts"];
                //                    if (!string.IsNullOrEmpty(emailId) || !string.IsNullOrEmpty(phoneNo))
                //                    {
                //                        string csv = GetCsvData(report, userScraper, ref Header, datass);
                //                        csvData.Add(csv);
                //                    }
                //                }
                //                if (ModuleSetting.UserFilterModel.IsUserMustHaveEmailId)
                //                {
                //                    string emailId = datass["Email Id"];
                //                   // string phoneNo = datass["Contact Number"];
                //                    if (!string.IsNullOrEmpty(emailId))
                //                    {
                //                        string csv = GetCsvData(report, userScraper, ref Header, datass);
                //                        csvData.Add(csv);
                //                    }
                //                }
                //                if (ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO)
                //                {
                //                    string phoneNo = datass["Contact Number"];
                //                    // string Business = datass["Business Acocunts"];
                //                    if (!string.IsNullOrEmpty(phoneNo))
                //                    {
                //                        string csv = GetCsvData(report, userScraper, ref Header, datass);
                //                        csvData.Add(csv);
                //                    }
                //                }
                //                if (ModuleSetting.UserFilterModel.IsUserMustHaveBusinessAccount)
                //                {
                //                    string Business = datass["Business Acocunts"];
                //                    if (!string.IsNullOrEmpty(Business) && Business.Contains("True"))
                //                    {
                //                        string csv = GetCsvData(report, userScraper, ref Header, datass);
                //                        csvData.Add(csv);
                //                    }             
                //                }
                //                else if(!ModuleSetting.UserFilterModel.IsUserMustHaveEmailId && !ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO && ! ModuleSetting.UserFilterModel.IsUserMustHaveBusinessAccount)
                //                {
                //                    string csv = GetCsvData(report, userScraper, ref Header, datass);
                //                    csvData.Add(csv);
                //                }
                //            }
                //        }
                //    }

                //}); 

                #endregion
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
            {
                Header = "Query Type, Query, Activity type, Account Username, Interacted Username, " +
                         "Interaction User Id, Is Private, Is Verified, " +
                         "Is Profile Pic Available, Profile Pic Url, Scrape Date,Tagged User,Gender, Full Name, IsFollowed Already, Post Count, Follower Count, Following Count,Email Id,Contact Number,Engagement Rate(%),Number Of Comments,Number Of Like,Biography,Business Accounts,Business Category";
                UserScraperReportModelAccount.ToList().ForEach(report =>
                {
                    if (!string.IsNullOrEmpty(report.BusinessCategory))
                    {
                    }

                    if (report.ScrapedUserId != null)
                    {
                        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.ActivityType + "," +
                                    report.AccountUsername +
                                    "," + report.ScrapedUsername + "," + report.ScrapedUserId + "," + report.IsPrivate +
                                    "," + report.IsVerified + "," + report.IsProfilePicAvailable +
                                    "," + report.ProfilePicUrl + "," + report.Date + "," + report.TaggedUser + "," +
                                    report.Gender + "," + (report.UserFullName ?? "") + "," + report.IsFollowedAlready +
                                    "," + report.PostCount + "," + report.FollowerCount + "," + report.FollowingCount +
                                    "," + (report.EamilId ?? "") + "," + (report.ContactNo ?? "") + "," +
                                    report.EngagementRate + "," + report.CommentCount + "," + report.LikeCount + "," +
                                    (string.IsNullOrEmpty(report.Biography)
                                        ? ""
                                        : "\"" + report.Biography.Replace("\n", "\t") + "\"") + "," +
                                    report.IsBusinessAccount + "," + (report.BusinessCategory ?? "") + ",");

                        #region Old Export Method with bug

                        //var requiredData = lstRequireddata[report.ScrapedUserId];
                        //if (!ModuleSetting.IsChkRequiredData)
                        //{
                        //    if (string.IsNullOrEmpty(requiredData))
                        //    {
                        //        try
                        //        {
                        //            csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.ActivityType + "," + report.AccountUsername +
                        //                        "," + report.ScrapedUsername + "," + report.ScrapedUserId + "," + report.IsPrivate +
                        //                        "," + report.IsBusiness + "," + report.IsVerified + "," + report.IsProfilePicAvailable +
                        //                        "," + report.ProfilePicUrl + "," + report.Date + "," + report.TaggedUser + "," + report.Gender);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Console.WriteLine(ex.StackTrace);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        data = GetRequiredData(requiredData);
                        //        csvData.Add(report.QueryType + "," + report.QueryValue + "," + report.ActivityType + "," + report.AccountUsername +
                        //                       "," + report.ScrapedUsername + "," + report.ScrapedUserId + "," + report.IsPrivate +
                        //                       "," + report.IsBusiness + "," + report.IsVerified + "," + report.IsProfilePicAvailable +
                        //                       "," + report.ProfilePicUrl + "," + report.Date + "," + report.TaggedUser + "," + report.Gender + "," + data.UserFullName + "," + data.IsFollowedAlready +
                        //                       "," + data.PostCount + "," + data.FollowerCount + "," + data.FollowingCount + "," + data.EamilId + "," + data.ContactNo
                        //                       + "," + data.EngagementRate + "," + data.CommentCount + "," + data.LikeCount + "," + data.Biography.Replace('\n', '\t'));
                        //    }

                        //}
                        //else
                        //{
                        //    var requiredDatas = lstRequireddata[report.ScrapedUserId];

                        //    if (!string.IsNullOrEmpty(requiredDatas))
                        //    {
                        //        var datass = GetRequiredData1(requiredDatas, userScraper);
                        //        if (ModuleSetting.UserFilterModel.IsUserMustHaveEmailIdAndPhoneNO)
                        //        {
                        //            string emailId = datass["Email Id"];
                        //            string phoneNo = datass["Contact Number"];
                        //            if (!string.IsNullOrEmpty(emailId) || !string.IsNullOrEmpty(phoneNo))
                        //            {
                        //                string csv = GetCsvData(report, userScraper, ref Header, datass);
                        //                csvData.Add(csv);
                        //            }
                        //        }
                        //        if (ModuleSetting.UserFilterModel.IsUserMustHaveEmailId)
                        //        {
                        //            string emailId = datass["Email Id"];
                        //            if (!string.IsNullOrEmpty(emailId))
                        //            {
                        //                string csv = GetCsvData(report, userScraper, ref Header, datass);
                        //                csvData.Add(csv);
                        //            }
                        //        }
                        //        if (ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO)
                        //        {
                        //            string phoneNo = datass["Contact Number"];
                        //            if (!string.IsNullOrEmpty(phoneNo))
                        //            {
                        //                string csv = GetCsvData(report, userScraper, ref Header, datass);
                        //                csvData.Add(csv);
                        //            }
                        //        }
                        //        else if (!ModuleSetting.UserFilterModel.IsUserMustHaveEmailId && !ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO && !ModuleSetting.UserFilterModel.IsUserMustHaveBusinessAccount)
                        //        {
                        //            string csv = GetCsvData(report, userScraper, ref Header, datass);
                        //            csvData.Add(csv);
                        //        }
                        //    }
                        //}

                        #endregion
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }

        private string GetCommonCsv(UserScrapeReportDetails report)
        {
            return report.QueryType + "," + report.QueryValue + "," + report.ActivityType + "," +
                      report.AccountUsername;
        }

        public UserRequiredData GetRequiredData(string ReqData)
        {
            var lstData = new UserRequiredData();
            var JResp = JObject.Parse(ReqData);
            lstData.ProfilePictureUrl = JResp["ProfilePictureUrl"].ToString();
            lstData.UserName = JResp["UserName"].ToString().Trim('"');
            lstData.UserId = JResp["UserId"].ToString().Trim('"');
            lstData.UserFullName = JResp["UserFullName"].ToString();
            lstData.IsFollowedAlready = Convert.ToBoolean(JResp["IsFollowedAlready"].ToString());
            lstData.PostCount = Convert.ToInt32(JResp["PostCount"].ToString());
            lstData.FollowerCount = Convert.ToInt32(JResp["FollowerCount"].ToString());
            lstData.FollowingCount = Convert.ToInt32(JResp["FollowingCount"].ToString());
            lstData.EamilId = JResp["Email_Id"].ToString();
            lstData.ContactNo = JResp["Phone_Number"].ToString();
            if (ReqData.Contains("EngagementRate"))
                lstData.EngagementRate = JResp["EngagementRate"].ToString() != null
                    ? JResp["EngagementRate"].ToString()
                    : string.Empty;
            if (ReqData.Contains("CommentCount"))
                lstData.CommentCount = JResp["CommentCount"].ToString() != null
                    ? JResp["CommentCount"].ToString()
                    : string.Empty;
            if (ReqData.Contains("LikeCount"))
                lstData.LikeCount =
                    JResp["LikeCount"].ToString() != null ? JResp["LikeCount"].ToString() : string.Empty;

            lstData.Biography = ReqData.Contains("Biography") ? JResp["Biography"].ToString() : "";
            lstData.IsBusinessAccounts =
                Convert.ToBoolean(ReqData.Contains("IsBusiness")
                    ? JResp["IsBusiness"].ToString()
                    : "False"); // lstData.Biography = JResp["Biography"].ToString();
            if (ReqData.Contains("BusinessCategory"))
                lstData.BusinessCategory = JResp["BusinessCategory"].ToString() != null
                    ? JResp["BusinessCategory"].ToString()
                    : string.Empty;
            return lstData;
        }

        public Dictionary<string, string> GetRequiredData1(string ReqData, UserScraperModel userScraperModels)
        {
            var RequiredData = new Dictionary<string, string>();
            try
            {
                var JResp = JObject.Parse(ReqData);
                var businessCategory = string.Empty;
                var businessCategoryToken = JResp["BusinessCategory"];
                if (businessCategoryToken != null)
                    businessCategory = businessCategoryToken.ToString();

                #region  this code is temporary basis ,it will be remove after releasing 2-3 setup

                if (userScraperModels.ListUserRequiredData[3].ItemName.Contains("Business Category"))
                {
                    var item = userScraperModels.ListUserRequiredData[3].ItemName;
                    var isSelect = userScraperModels.ListUserRequiredData[3].IsSelected;
                    userScraperModels.ListUserRequiredData.RemoveAt(3);
                    userScraperModels.ListUserRequiredData.Add(new UserScraperModel.UserRequiredData
                        {ItemName = item, IsSelected = isSelect});
                }

                #endregion

                RequiredData.Add(userScraperModels.ListUserRequiredData[1].ItemName,
                    JResp["ProfilePictureUrl"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[2].ItemName,
                    JResp["UserName"].ToString().Trim('"'));
                RequiredData.Add(userScraperModels.ListUserRequiredData[3].ItemName,
                    JResp["UserId"].ToString().Trim('"'));
                RequiredData.Add(userScraperModels.ListUserRequiredData[4].ItemName, JResp["UserFullName"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[5].ItemName,
                    JResp["IsFollowedAlready"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[6].ItemName, JResp["PostCount"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[7].ItemName, JResp["FollowerCount"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[8].ItemName,
                    JResp["FollowingCount"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[9].ItemName, JResp["Email_Id"].ToString());
                RequiredData.Add(userScraperModels.ListUserRequiredData[10].ItemName, JResp["Phone_Number"].ToString());
                if (ReqData.Contains("EngagementRate"))
                    RequiredData.Add(userScraperModels.ListUserRequiredData[11].ItemName,
                        JResp["EngagementRate"].ToString() != null ? JResp["EngagementRate"].ToString() : string.Empty);

                if (ReqData.Contains("CommentCount"))
                    RequiredData.Add(userScraperModels.ListUserRequiredData[12].ItemName,
                        JResp["CommentCount"].ToString() != null ? JResp["CommentCount"].ToString() : string.Empty);

                if (ReqData.Contains("LikeCount"))
                    RequiredData.Add(userScraperModels.ListUserRequiredData[13].ItemName,
                        JResp["LikeCount"].ToString() != null ? JResp["LikeCount"].ToString() : string.Empty);


                RequiredData.Add(userScraperModels.ListUserRequiredData[14].ItemName,
                    ReqData.Contains("Biography") ? JResp["Biography"].ToString() : "");
                if (ReqData.Contains("IsBusiness")) // userScraperModels.ListUserRequiredData.Count > 15
                    RequiredData.Add(userScraperModels.ListUserRequiredData[15].ItemName,
                        ReqData.Contains("IsBusiness") ? JResp["IsBusiness"].ToString() : "");

                if (ReqData.Contains("BusinessCategory"))
                    RequiredData.Add("Business Category", businessCategory != null ? businessCategory : string.Empty);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return RequiredData;
        }

        private string GetCsvData(UserScrapeReportDetails report, UserScraperModel userScraper, ref string Header,
            Dictionary<string, string> dicData)
        {
            var csv = report.QueryType + "," + report.QueryValue + "," + report.ActivityType + "," +
                      report.AccountUsername +
                      "," + report.ScrapedUsername + "," + report.ScrapedUserId + "," + report.IsPrivate + "," +
                      report.IsVerified + "," + report.ProfilePicUrl + "," + report.Date + "," + report.Gender + ",";

            try
            {
                for (var singleData = 0; singleData < userScraper.ListUserRequiredData.Count; singleData++)
                {
                    if (singleData == 0)
                        continue;

                    //string data1 = string.Empty;
                    //try
                    //{
                    //     data1 = dicData[userScraper.ListUserRequiredData[singleData].ItemName];
                    //}
                    //catch (Exception ex)
                    //{
                    //}
                    if (userScraper.ListUserRequiredData[singleData].IsSelected
                    ) //userScraper.ListUserRequiredData[singleData].IsSelected ||
                    {
                        var datas = dicData.Where(x => x.Key == userScraper.ListUserRequiredData[singleData].ItemName)
                            .ToList();
                        if (datas.Count == 0)
                            break;
                        if (datas[0].Key == "Biography")
                        {
                            var data = "\"" + datas[0].Value + "\"";
                            csv += data.Replace('\n', '\t') + ",";
                            continue;
                        }

                        if (ModuleSetting.UserFilterModel.IsUserMustHavePhoneNO ||
                            ModuleSetting.UserFilterModel.IsUserMustHaveEmailId ||
                            ModuleSetting.UserFilterModel.IsUserMustHaveBusinessAccount)
                        {
                        }

                        csv += datas[0].Value + ",";
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return csv;
        }
    }
}