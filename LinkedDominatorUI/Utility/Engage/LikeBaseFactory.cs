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
    public class LikeBaseFactory : ILdBaseFactory
    {
        public ILdUtilityFactory LdUtilityFactory()
        {
            var utilityFactory = new LdUtilityFactory();

            var builder = new LdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Like)
                .AddReportFactory(new LikeReport())
                .AddViewCampaignFactory(new LikeViewCampaign());

            return builder.LdUtilityFactory;
        }
    }

    public class LikeReport : ILdReportFactory
    {
        public static ObservableCollection<InteractedPostsReportModel> InteractedPostsReportModel =
            new ObservableCollection<InteractedPostsReportModel>();

        public static List<EnagageReportModel> AccountsInteractedPosts = new List<EnagageReportModel>();
        public string ActivityType = DominatorHouseCore.Enums.ActivityType.Like.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<LikeModel>(activitySettings).SavedQueries;
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
                        PostOwnerFullName = reportItem.PostOwnerFullName,
                        PostOwnerProfileUrl = reportItem.PostOwnerProfileUrl,
                        ConnectionType = reportItem.ConnectionType,
                        PostedDateTime = reportItem.PostedTime.EpochToDateTimeUtc().ToLocalTime(),
                        PostLink = reportItem.PostLink,
                        PostDescription = reportItem.PostDescription,
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
            #endregion

            return new ObservableCollection<object>(InteractedPostsReportModel);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            AccountsInteractedPosts.Clear();
            IList reportDetails = dataBase.GetInteractedPosts(ActivityType).ToList();
            var count = 0;
            foreach (InteractedPosts reportItem in reportDetails)
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
                        //PostedDateTime = reportItem.PostedDateTime,
                        MediaType = reportItem.MediaType,
                        PostLink = reportItem.PostLink,
                        // PostTitle = reportItem.PostTitle,
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

            #region Campaign and account reports

            var utils = new LdUtils();
            switch (reportType)
            {
                case ReportType.Campaign:
                    Header = utils
                        .PostsReportHeader(); // "Account Email,Query Type,Query Value,Activity Type,Post Owner FullName,Post Owner Url,Post Link,Post Description,Like Count,Comment Count,Interaction DateTime";

                    InteractedPostsReportModel.ToList().ForEach(reportItem =>
                    {
                        try
                        {
                            csvData.Add(utils.PostsReportCSVData(reportItem));
                            //csvData.Add(reportItem.AccountEmail + "," + reportItem.QueryType + "," + reportItem.QueryValue + "," +
                            //            reportItem.ActivityType + "," + reportItem.PostOwnerFullName + "," + reportItem.PostOwnerProfileUrl + "," +
                            //            reportItem.PostLink + "," + reportItem.PostDescription + "," + reportItem.LikeCount + "," + reportItem.CommentCount + "," +
                            //            reportItem.InteractionDateTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;

                case ReportType.Account:
                    Header = utils
                        .PostsReportHeader(
                            false); //"Query Type,Query Value,Activity Type,Post Owner FullName,Post Owner Url,Post Link,Post Description,Like Count,Comment Count,Interaction DateTime";

                    AccountsInteractedPosts.ToList().ForEach(reportItem =>
                    {
                        try
                        {
                            csvData.Add(EnagageReportUtils.ReportCSVData(reportItem));
                            //csvData.Add(reportItem.QueryType + "," + reportItem.QueryValue + "," +
                            //            reportItem.ActivityType + "," + reportItem.PostOwnerFullName + "," + reportItem.PostOwnerProfileUrl + "," +
                            //             reportItem.PostLink + "," + reportItem.PostTitle + "," + reportItem.PostDescription + "," + reportItem.LikeCount + "," + reportItem.CommentCount + "," +
                            //            reportItem.InteractionDatetime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
            }

            #endregion


            Utilities.ExportReports(fileName, Header, csvData);
        }
    }

    public class LikeViewCampaign : ILdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            try
            {
                var objLike = Like.GetSingeltonObjectLike();
                objLike.IsEditCampaignName = isEditCampaignName;
                objLike.CancelEditVisibility = cancelEditVisibility;
                objLike.TemplateId = templateId;
                // objLike.CampaignName = campaignDetails.CampaignName;
                objLike.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : objLike.CampaignName;
                objLike.CampaignButtonContent = campaignButtonContent;
                objLike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                objLike.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                objLike.ObjViewModel.LikeModel =
                    templateDetails.ActivitySettings.GetActivityModel<LikeModel>(objLike.ObjViewModel.Model);
                objLike.MainGrid.DataContext = objLike.ObjViewModel;
                TabSwitcher.ChangeTabIndex(3, 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}