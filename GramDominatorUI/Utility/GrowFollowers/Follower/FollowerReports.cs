using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonServiceLocator;
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

namespace GramDominatorUI.Utility.GrowFollowers.Follower
{
    public class FollowerReports : FollowerModel, IGdReportFactory
    {
        private static readonly ObservableCollection<FollowReportDetails> FollowerReportModelCampaign =
            new ObservableCollection<FollowReportDetails>();

        public FollowerModel FollowerModel { get; set; }

        private static List<FollowReportDetails> FollowReportModelAccount { get; } = new List<FollowReportDetails>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FollowerModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            reportModel.FollowRate = true;

            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            FollowerModel = JsonConvert.DeserializeObject<FollowerModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);
            // Need to be cleared data for adding into static variable.
            FollowerReportModelCampaign.Clear();
            var sNo = 0;

            #region Report generate for Campaign activity or broadcast msg activity

            dataBase.Get<InteractedUsers>().ForEach(
                report =>
                {
                    FollowerReportModelCampaign.Add(new FollowReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                        QueryType = report.QueryType,
                        Query = report.Query,
                        InteractedUsername = report.InteractedUsername,
                        InteractedUserId = report.InteractedUserId,
                        Status = report.Status
                    });
                });
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
                        {ColumnHeaderText = "LangKeyQuery".FromResourceDictionary(), ColumnBindingText = "Query"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFollower".FromResourceDictionary(),
                        ColumnBindingText = "InteractedUsername"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyStatus".FromResourceDictionary(), ColumnBindingText = "Status"}
                };

            #endregion

            #region Report generate for Like and comment activity after follow action           

            if (FollowerModel.IsChkLikeUsersLatestPost || FollowerModel.ChkCommentOnUserLatestPostsChecked)
            {
                dataBase.Get<InteractedPosts>().ForEach(
                    report =>
                    {
                        FollowerReportModelCampaign.Add(new FollowReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            QueryType = report.QueryType,
                            Query = report.QueryValue,
                            Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                            InteractedUsername = report.UsernameOwner,
                            Status = report.Status,
                            MediaCode = report.PkOwner
                        });
                    });
                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "AccountUsername", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query", ColumnBindingText = "Query"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Follower", ColumnBindingText = "InteractedUsername"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "MediaCode", ColumnBindingText = "MediaCode"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                    };
            }

            #endregion

            return new ObservableCollection<object>(FollowerReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == ActivityType.Follow.ToString()).ToList();
            // Need to be cleared data for adding into static variable.
            FollowReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers report in reportDetails)
                FollowReportModelAccount.Add(
                    new FollowReportDetails
                    {
                        Id = ++sNo,
                        Query = report.Query,
                        QueryType = report.QueryType,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                        ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), report.ActivityType),
                        AccountUsername = report.Username,
                        InteractedUsername = report.InteractedUsername,
                        InteractedUserId = report.InteractedUserId,
                        Status = report.Status
                    });

            dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.Follow).ForEach(
                    report =>
                    {
                        FollowReportModelAccount.Add(new FollowReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            QueryType = report.QueryType,
                            Query = report.QueryValue,
                            Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                            InteractedUsername = report.UsernameOwner,
                            MediaCode = report.PkOwner,
                            Status = report.Status
                        });
                    });

            return FollowReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Query Type, Query Value, Activity Type, Account Username, Followed Username, Followed User Id, Date,MediaCode,Status";

                FollowerReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.ActivityType + "," +
                                    report.AccountUsername + "," +
                                    report.InteractedUsername + "," + report.InteractedUserId + "," + report.Date +
                                    "," + report.MediaCode + "," + report.Status);
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
                    "Query Type, Query Value, Activity Type, Account Username, Followed Username, Followed User Id, Date,MediaCode,Status";

                FollowReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + "," + report.Query + "," + report.ActivityType + "," +
                                    report.AccountUsername + "," +
                                    report.InteractedUsername + "," + report.InteractedUserId + "," + report.Date +
                                    "," + report.MediaCode + "," + report.Status);
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

        //public List<FollowReportDetails> GetFollowDetails()
        //{
        //    return new List<FollowReportDetails>();
        //}
    }
}