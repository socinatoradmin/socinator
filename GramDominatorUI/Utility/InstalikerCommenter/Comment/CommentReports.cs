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

namespace GramDominatorUI.Utility.InstalikerCommenter.Comment
{
    internal class CommentReports : IGdReportFactory
    {
        private static readonly ObservableCollection<CommentReportDetails> CommentReportModelCampaign =
            new ObservableCollection<CommentReportDetails>();

        public CommentModel commentModel { get; set; }

        private static List<CommentReportDetails> CommentReportModelAccount { get; } = new List<CommentReportDetails>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<CommentModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            commentModel = JsonConvert.DeserializeObject<CommentModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);

            // Need to be cleared data for adding into static variable.
            CommentReportModelCampaign.Clear();

            #region Update Existing Table with new Column

            try
            {
                if (!campaignDetails.IsInteractedPostsUpdated)
                {
                    var query = "UPDATE InteractedPosts SET Status = 'Success' WHERE Status IS NULL";
                    dataBase?._context?.Database?.ExecuteSqlCommand(query);
                    campaignDetails.IsInteractedPostsUpdated = true;
                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.Edit(campaignDetails);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            #region get data from InteractedPosts table and add to CommenterReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    if (report.Status == "Success")
                        CommentReportModelCampaign.Add(new CommentReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            CommentedMediaCode = report.PkOwner,
                            CommentedMediaOwner = report.UsernameOwner,
                            Follower = "",
                            Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                            MediaType = report.MediaType.ToString(),
                            Comment = report.Comment,
                            Status = "Commented"
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
                        ColumnHeaderText = "LangKeyAccountUsername".FromResourceDictionary(),
                        ColumnBindingText = "AccountUsername"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(),
                        ColumnBindingText = "CommentedMediaCode"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaOwner".FromResourceDictionary(),
                        ColumnBindingText = "CommentedMediaOwner"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyFollower".FromResourceDictionary(), ColumnBindingText = "Follower"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyComment".FromResourceDictionary(), ColumnBindingText = "Comment"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyStatus".FromResourceDictionary(), ColumnBindingText = "Status"}
                };

            #endregion

            if (commentModel.IsChkLikePostAfterComment)
            {
                dataBase.Get<InteractedPosts>().ForEach(
                    report =>
                    {
                        if (report.Status == "Liked")
                            CommentReportModelCampaign.Add(new CommentReportDetails
                            {
                                Id = ++sNo,
                                AccountUsername = report.Username,
                                CommentedMediaCode = report.PkOwner,
                                CommentedMediaOwner = report.UsernameOwner,
                                Follower = "",
                                Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                                MediaType = report.MediaType.ToString(),
                                Status = "Liked"
                            });
                    });

                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Id", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "AccountUsername", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Media Code", ColumnBindingText = "CommentedMediaCode"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Media Owner", ColumnBindingText = "CommentedMediaOwner"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Follower", ColumnBindingText = "Follower"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Media Type", ColumnBindingText = "MediaType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Comment", ColumnBindingText = "Comment"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                    };
            }

            if (commentModel.IsChkFollowUserAfterComment)
            {
                var followDetails = dataBase.Get<InteractedUsers>().ToList();


                dataBase.Get<InteractedUsers>().ForEach(
                    report =>
                    {
                        CommentReportModelCampaign.Add(new CommentReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            Follower = report.InteractedUsername,
                            Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                            Status = "Followed"
                        });
                    });
                reportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "Id", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "AccountUsername", ColumnBindingText = "AccountUsername"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Media Code", ColumnBindingText = "CommentedMediaCode"},
                        new GridViewColumnDescriptor
                            {ColumnHeaderText = "Media Owner", ColumnBindingText = "CommentedMediaOwner"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Follower", ColumnBindingText = "Follower"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Date", ColumnBindingText = "Date"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Media Type", ColumnBindingText = "MediaType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Comment", ColumnBindingText = "Comment"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Status", ColumnBindingText = "Status"}
                    };
            }

            return new ObservableCollection<object>(CommentReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.Comment).ToList();

            // Need to be cleared data for adding into static variable.
            CommentReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
            {
                if (report.Status == "Liked")
                    continue;
                CommentReportModelAccount.Add(
                    new CommentReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = ActivityType.Comment,
                        AccountUsername = report.Username,
                        CommentedMediaCode = report.PkOwner,
                        MediaType = report.MediaType.ToString(),
                        CommentedMediaOwner = report.UsernameOwner,
                        Comment = report.Comment,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                        Status = "Commented"
                    });
            }


            dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.Comment && x.Status == "Liked").ForEach(
                    report =>
                    {
                        if (report.Status == "Liked")
                            CommentReportModelAccount.Add(new CommentReportDetails
                            {
                                Id = ++sNo,
                                AccountUsername = report.Username,
                                CommentedMediaCode = report.PkOwner,
                                CommentedMediaOwner = report.UsernameOwner,
                                Follower = "",
                                Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                                MediaType = report.MediaType.ToString(),
                                Status = report.Status
                            });
                    });


            var followDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>()
                .Where(x => x.ActivityType == Convert.ToString(ActivityType.Comment)).ToList();
            dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedUsers>().ForEach(
                report =>
                {
                    CommentReportModelAccount.Add(new CommentReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        Follower = report.InteractedUsername,
                        Date = report.Date.EpochToDateTimeUtc().ToLocalTime(),
                        Status = "Followed"
                    });
                });

            return CommentReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity Type, Account Username, Media Code, Media type, Media Owner,Follower, Comment Text, Interaction Date, status";

                CommentReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        if (report.Comment == null)
                            report.Comment = "";
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.CommentedMediaCode + "," +
                                    report.MediaType + "," + report.CommentedMediaOwner + "," + report.Follower + "," +
                                    report.Comment.Replace("\r\n", " ") + "," + report.Date + "," + report.Status);
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
                    "Activity Type, Account Username, Media Code, Media type, Media Owner,Follower, Comment Text, Interaction Date, status";

                CommentReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        if (report.Comment == null)
                            report.Comment = "";
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," +
                                    report.CommentedMediaCode + "," +
                                    report.MediaType + "," + report.CommentedMediaOwner + "," + report.Follower + "," +
                                    report.Comment.Replace("\r\n", " ") + "," + report.Date + "," + report.Status);
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
}