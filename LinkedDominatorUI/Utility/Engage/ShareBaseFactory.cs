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
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.LDViews.Engage;
using Newtonsoft.Json;

namespace LinkedDominatorUI.Utility.Engage
{
    public class ShareBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Share)
                .AddReportFactory(new ShareReport())
                .AddViewCampaignFactory(new ShareViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class ShareReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedPostsReportModel> InteractedPostsReportModel =
            new ObservableCollection<InteractedPostsReportModel>();

        public static List<EnagageReportModel> AccountsInteractedPosts = new List<EnagageReportModel>();

        public string ActivityType = DominatorHouseCore.Enums.ActivityType.Share.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ShareModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            InteractedPostsReportModel.Clear();
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedPosts table and add to InteractedPostsReportModel

            var count = 0;
            dataBase.GetInteractedPosts(ActivityType).ForEach(
                reportItem =>
                {
                    InteractedPostsReportModel.Add(new InteractedPostsReportModel
                    {
                        Id = ++count,
                        AccountEmail = reportItem.AccountEmail,
                        QueryType = reportItem.QueryType,
                        QueryValue = reportItem.QueryValue,
                        ActivityType = reportItem.ActivityType,
                        PostOwnerFullName = reportItem.PostOwnerFullName?.Replace(","," "),
                        PostOwnerProfileUrl = reportItem.PostOwnerProfileUrl,
                        ConnectionType = reportItem.ConnectionType,
                        PostedDateTime = reportItem.PostedTime.EpochToDateTimeUtc().ToLocalTime(),
                        PostLink = reportItem.PostLink,
                        PostDescription = reportItem.PostDescription?.Replace(","," ")?.Replace("\t"," ")?.Replace("\r\n"," ")?.Replace("\n"," "),
                        LikeCount = reportItem.LikeCount,
                        CommentCount = reportItem.CommentCount,
                        InteractionDateTime = reportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
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
                        ColumnHeaderText = "LangKeyPostOwnerName".FromResourceDictionary(),
                        ColumnBindingText = "PostOwnerFullName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostOwnerURl".FromResourceDictionary(),
                        ColumnBindingText = "PostOwnerProfileUrl"
                    },
                    //new GridViewColumnDescriptor {ColumnHeaderText = "Connection ype",ColumnBindingText="ConnectionType" },
                    // new GridViewColumnDescriptor {ColumnHeaderText = "Posted Date Time",ColumnBindingText="PostedDateTime" },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostUrl".FromResourceDictionary(), ColumnBindingText = "PostLink"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostDescription".FromResourceDictionary(),
                        ColumnBindingText = "PostDescription"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyLikeCount".FromResourceDictionary(), ColumnBindingText = "LikeCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentCount".FromResourceDictionary(),
                        ColumnBindingText = "CommentCount"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyInteractionDateTime".FromResourceDictionary(),
                        ColumnBindingText = "InteractionDateTime"
                    }
                };


            //   reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsReportModel);

            #endregion

            return new ObservableCollection<object>(InteractedPostsReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedPosts.Clear();
            IList reportDetails = null; // dataBase.GetInteractedPosts(CurrentActivityType).ToList();
            var count = 0;
            foreach (var reportItem in dataBase.GetInteractedPosts(ActivityType).ToList())
                AccountsInteractedPosts.Add(
                    new EnagageReportModel
                    {
                        Id = ++count,
                        QueryType = reportItem.QueryType,
                        QueryValue = reportItem.QueryValue,
                        ActivityType = reportItem.ActivityType,
                        PostOwnerFullName = reportItem.PostOwnerFullName,
                        PostOwnerProfileUrl = reportItem.PostOwnerProfileUrl,
                        PostOwnerUrl = reportItem.PostOwnerProfileUrl,
                        ConnectionType = reportItem.ConnectionType,
                        PostedDateTime = reportItem.PostedDateTime,
                        PostLink = reportItem.PostLink,
                        PostDescription = reportItem.PostDescription,
                        LikeCount = reportItem.LikeCount,
                        CommentCount = reportItem.CommentCount,
                        InteractionDatetime = reportItem.InteractionDatetime
                    }
                );

            reportDetails = AccountsInteractedPosts.Select(x =>
                new
                {
                    x.Id,
                    x.QueryType,
                    x.QueryValue,
                    x.ActivityType,
                    x.PostOwnerFullName,
                    x.PostOwnerUrl,
                    x.ConnectionType,
                    x.PostedDateTime,
                    x.PostLink,
                    x.PostDescription,
                    x.LikeCount,
                    x.CommentCount,
                    x.InteractionDatetime
                }).ToList();

            return reportDetails;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            var utils = new LdUtils();
            if (reportType == ReportType.Campaign)
            {
                Header = utils.PostsReportHeader();
                //"Account Email,Query Type,Query Value,Activity Type,Post Owner FullName,Post Owner Url,Post Link,Post Description,Like Count,Comment Count,Interaction DateTime";

                InteractedPostsReportModel.ToList().ForEach(reportItem =>
                {
                    try
                    {
                        csvData.Add(utils.PostsReportCSVData(reportItem));
                        //csvData.Add(reportItem.AccountEmail + "," + reportItem.QueryType + "," + reportItem.QueryValue + "," +
                        //            reportItem.ActivityType + "," + reportItem.PostOwnerFullName + "," + reportItem.PostOwnerProfileUrl + "," +
                        //              reportItem.PostLink + ",\"" + reportItem.PostDescription.Replace("\"", "\"\"") + "\"," + reportItem.LikeCount + "," + reportItem.CommentCount + "," +
                        //            reportItem.InteractionDateTime);
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
                Header = utils
                    .PostsReportHeader(
                        false); //"Query Type,Query Value,Activity Type,Post Owner FullName,Post Owner Url,Post Link,Post Description,Like Count,Comment Count,Interaction DateTime";

                AccountsInteractedPosts.ToList().ForEach(reportItem =>
                {
                    try
                    {
                        csvData.Add(EnagageReportUtils.ReportCSVData(reportItem));
                        //csvData.Add(reportItem.QueryType + "," + reportItem.QueryValue + "," +
                        //          reportItem.ActivityType + "," + reportItem.PostOwnerFullName + "," + reportItem.PostOwnerProfileUrl +
                        //           "," + reportItem.PostLink +
                        //            "," + reportItem.PostDescription + "," + reportItem.LikeCount + "," + reportItem.CommentCount + "," +
                        //          reportItem.InteractionDatetime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            Utilities.ExportReports(fileName, Header, csvData);
        }
    }

    public class ShareViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            try
            {
                var objShare = Share.GetSingeltonObjectShare();
                objShare.IsEditCampaignName = isEditCampaignName;
                objShare.CancelEditVisibility = cancelEditVisibility;
                objShare.TemplateId = templateId;
                //objShare.CampaignName = campaignDetails.CampaignName;
                objShare.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objShare.CampaignName;
                objShare.CampaignButtonContent = campaignButtonContent;
                objShare.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objShare.ShareFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

                objShare.ObjViewModel.ShareModel =
                    templateDetails.ActivitySettings.GetActivityModel<ShareModel>(objShare.ObjViewModel.Model);

                objShare.MainGrid.DataContext = objShare.ObjViewModel;

                TabSwitcher.ChangeTabIndex(3, 2);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    //we implemented company posts in share module so now it is not only remain profile it is 
    // also become companyUrl therefore added field 'PostOwnerUrl'  for show in report header
    public class EnagageReportModel : InteractedPosts
    {
        public string PostOwnerUrl { get; set; }
    }

    public class EnagageReportUtils
    {
        public static string ReportCSVData(EnagageReportModel model, bool addMyComment = false)
        {
            return
                $"{model.QueryType},{model.QueryValue},{model.ActivityType},{model.PostOwnerFullName},{model.PostOwnerProfileUrl}," +
                $"{model.PostLink},{model.PostDescription.AsCsvData()},"
                + (addMyComment ? $"{model.MyComment.AsCsvData()}," : "") +
                $"{model.LikeCount},{model.CommentCount},{model.InteractionDatetime}";
        }
    }
}