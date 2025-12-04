using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FdReports;
using FaceDominatorCore.Interface;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.FanpageScraper
{
    public class FanpageScraperReports : IFdReportFactory
    {
        public static ObservableCollection<PageReportModel> InteractedPageModel =
            new ObservableCollection<PageReportModel>();

        public static List<PageReportAccountModel> AccountsInteractedPages = new List<PageReportAccountModel>();

        public static List<string> Data = new List<string>();

        private readonly string _activityType = ActivityType.FanpageScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FanpageScraperModel>(activitySettings).SavedQueries;
        }

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedPageModel.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModel

            dataBase.GetAllInteractedData<InteractedPages>().ForEach(
                report =>
                {
                    InteractedPageModel.Add(new PageReportModel
                    {
                        Id = report.Id,
                        AccountEmail = report.AccountEmail,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        PageName = report.PageName,
                        PageUrl = report.PageUrl,
                        TotalLikers = report.TotalLikers,
                        PageType = report.PageType,
                        DetailedPageInfo = report.PageFullDetails,
                        InteractionTimeStamp = report.InteractionDateTime
                    });
                });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            // {
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText =
                            $"{"LangKeyAccount".FromResourceDictionary()} {"LangKeyEmail".FromResourceDictionary()}",
                        ColumnBindingText = "AccountEmail"
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
                        ColumnHeaderText = "LangKeyFanpageName".FromResourceDictionary(), ColumnBindingText = "PageName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFanpageUrl".FromResourceDictionary(), ColumnBindingText = "PageUrl"
                    },
                    new GridViewColumnDescriptor {ColumnHeaderText = "Total Likers", ColumnBindingText = "TotalLikers"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFanpageType".FromResourceDictionary(), ColumnBindingText = "PageType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            // });
            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPageModel);

            #endregion

            return new ObservableCollection<object>(InteractedPageModel);
        }


        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages>(x =>
                    x.ActivityType == _activityType).ToList();

            AccountsInteractedPages.Clear();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages report in reportDetails)
            {
                AccountsInteractedPages.Add(
                    new PageReportAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        ActivityType = ActivityType.FanpageScraper.ToString(),
                        PageName = report.PageName,
                        PageUrl = report.PageUrl,
                        PageType = report.PageType,
                        TotalLikers = report.TotalLikers,
                        MembershipStatus = report.MembershipStatus,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    });

                var objFanpageDetails = JsonConvert.DeserializeObject<FanpageDetails>(report.PageFullDetails);

                if (!Data.Contains(objFanpageDetails.FanPageID))
                    Data.Add(report.QueryType + ","
                                              + report.QueryValue + ","
                                              + $"{FdConstants.FbHomeUrl}{objFanpageDetails.FanPageID}" + ","
                                              + objFanpageDetails.FanPageName + ","
                                              + report.TotalLikers + ","
                                              + objFanpageDetails.RatingCount + ","
                                              + objFanpageDetails.RatingValue + ","
                                              + objFanpageDetails.IsLikedByFriend + ","
                                              + objFanpageDetails.IsVerifiedPage + ","
                                              + (string.IsNullOrEmpty(objFanpageDetails.PhoneNumber)
                                                  ? "NA"
                                                  : objFanpageDetails.PhoneNumber?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objFanpageDetails.FanPageCategory)
                                                  ? "NA"
                                                  : objFanpageDetails.FanPageCategory?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objFanpageDetails.FanPageDescription)
                                                  ? "NA"
                                                  : objFanpageDetails.FanPageDescription?.Replace(",", string.Empty)
                                                      ?.Replace("\r\n", " ")?.Replace("\n", " ")) + ","
                                              + objFanpageDetails.FanpageFollowerCount + ","
                                              + (string.IsNullOrEmpty(objFanpageDetails.FanPageMainCategoryName)
                                                  ? "NA"
                                                  : objFanpageDetails.FanPageMainCategoryName?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objFanpageDetails.Address)
                                                  ? "NA"
                                                  : objFanpageDetails.Address?.Replace(",", string.Empty)
                                                      ?.Replace("\r\n", " ")?.Replace("\n", " ")) + ","
                                              + (string.IsNullOrEmpty(objFanpageDetails.WebAddresss)
                                                  ? "NA"
                                                  : objFanpageDetails.WebAddresss?.Replace(",", " ")) + ","
                                              + report.InteractionTimeStamp.EpochToDateTimeLocal() + ",");

                id++;
            }

            return AccountsInteractedPages;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Page Url,Page Name,Total Likers,Rating Count,Rating Value,Is Liked, Is Verified,Phone Number, Page Category,Page Description,Follower Count,Main Category, Address, web Address, Date";
                Header = PostsReportHeader();
                InteractedPageModel.ToList().ForEach(report =>
                {
                    try
                    {
                        var objFanpageDetails = JsonConvert.DeserializeObject<FanpageDetails>(report.DetailedPageInfo);


                        csvData.Add(report.AccountEmail + ","
                                                        + report.QueryType + ","
                                                        + report.QueryValue + ","
                                                        + $"{FdConstants.FbHomeUrl}{objFanpageDetails.FanPageID}" + ","
                                                        + objFanpageDetails.FanPageName + ","
                                                        + report.TotalLikers + ","
                                                        + objFanpageDetails.RatingCount + ","
                                                        + objFanpageDetails.RatingValue + ","
                                                        + objFanpageDetails.IsLikedByFriend + ","
                                                        + objFanpageDetails.IsVerifiedPage + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.PhoneNumber)
                                                            ? "NA"
                                                            : objFanpageDetails.PhoneNumber?.Replace(",", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.FanPageCategory)
                                                            ? "NA"
                                                            : objFanpageDetails.FanPageCategory?.Replace(",", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.FanPageDescription)
                                                            ? "NA"
                                                            : objFanpageDetails.FanPageDescription
                                                                ?.Replace(",", string.Empty)?.Replace("\r\n", " ")
                                                                ?.Replace("\n", " ")) + ","
                                                        + objFanpageDetails.FanpageFollowerCount + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails
                                                            .FanPageMainCategoryName)
                                                            ? "NA"
                                                            : objFanpageDetails.FanPageMainCategoryName?.Replace(",",
                                                                " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.Address)
                                                            ? "NA"
                                                            : objFanpageDetails.Address?.Replace(",", string.Empty)
                                                                ?.Replace("\r\n", " ")?.Replace("\n", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.WebAddresss)
                                                            ? "NA"
                                                            : objFanpageDetails.WebAddresss?.Replace(",", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.Email)
                                                            ? "NA"
                                                            : objFanpageDetails.Email?.Replace(",", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.Instagram)
                                                            ? "NA"
                                                            : objFanpageDetails.Instagram?.Replace(",", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.Twitter)
                                                            ? "NA"
                                                            : objFanpageDetails.Twitter?.Replace(",", " ")) + ","
                                                        + (string.IsNullOrEmpty(objFanpageDetails.WhatsApp)
                                                            ? "NA"
                                                            : objFanpageDetails.WhatsApp?.Replace(",", " ")) + ","
                                                        + report.InteractionTimeStamp + ",");

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
                //Header = "QueryType,QueryValue,Page Name,Page Url,Total Likers,Rating Count,Rating Value,Is Liked, Is Verified,Phone Number, Page Category,Page Description,Follower Count,Main Category, Address, Web Address,Date";
                Header = PostsReportHeader(false);
                Data.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyPageUrl");
            listResource.Add("LangKeyPageName");
            listResource.Add("LangKeyTotalLikers");
            listResource.Add("LangKeyRatingCount");
            listResource.Add("LangKeyRatingValue");
            listResource.Add("LangKeyIsLiked");
            listResource.Add("LangKeyIsVerified");
            listResource.Add("LangKeyPhoneNumber");
            listResource.Add("LangKeyPageCategory");
            listResource.Add("LangKeyPageDescription");
            listResource.Add("LangKeyFollowerCount");
            listResource.Add("LangKeyMainCategory");
            listResource.Add("LangKeyAddress");
            listResource.Add("LangKeywebAddress");
            listResource.Add("LangKeyEmail");
            listResource.Add("LangKeyInstagram");
            listResource.Add("LangKeyTwitter");
            listResource.Add("LangKeyWhatsApp");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}