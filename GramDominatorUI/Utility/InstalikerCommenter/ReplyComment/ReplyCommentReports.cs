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

namespace GramDominatorUI.Utility.InstalikerCommenter.ReplyComment
{
    public class ReplyCommentReports : IGdReportFactory
    {
        private static readonly ObservableCollection<ReplyCommentReportDetails> ReplyCommentReportModelCampaign =
            new ObservableCollection<ReplyCommentReportDetails>();

        public ReplyCommentModel ReplyCommentModel { get; set; }

        private static List<ReplyCommentReportDetails> CommentReportModelAccount { get; } =
            new List<ReplyCommentReportDetails>();

        public string Header { get; set; } = string.Empty;


        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Comment Text, Interaction Date, status";

                ReplyCommentReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        if (report.Comments == null)
                            report.Comments = "";
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.MediaCode + "," +
                                    report.MediaType + "," + report.CommentOwnerName + "," +
                                    report.Comments.Replace("\r\n", " ") + "," + report.Date + "," + report.status);
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
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Comment Text, Interaction Date, status";

                CommentReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        if (report.Comments == null)
                            report.Comments = "";
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.MediaCode + "," +
                                    report.MediaType + "," + report.CommentOwnerName + "," +
                                    report.Comments.Replace("\r\n", " ") + "," + report.Date + "," + report.status);
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

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            ReplyCommentModel = JsonConvert.DeserializeObject<ReplyCommentModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == campaignDetails.TemplateId)?.ActivitySettings);

            // Need to be cleared data for adding into static variable.
            ReplyCommentReportModelCampaign.Clear();

            #region get data from InteractedPosts table and add to CommenterReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    if (report.Status == "Success")
                        ReplyCommentReportModelCampaign.Add(new ReplyCommentReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            MediaCode = report.PkOwner,
                            CommentOwnerName = report.UsernameOwner,
                            ActivityType = report.ActivityType,
                            Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                            MediaType = report.MediaType.ToString(),
                            Comments = report.Comment,
                            status = "Reply Commented"
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
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(), ColumnBindingText = "MediaCode"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaOwner".FromResourceDictionary(),
                        ColumnBindingText = "CommentOwnerName"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyComment".FromResourceDictionary(), ColumnBindingText = "Comments"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyStatus".FromResourceDictionary(), ColumnBindingText = "status"}
                };

            #endregion

            return new ObservableCollection<object>(ReplyCommentReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.ReplyToComment).ToList();

            // Need to be cleared data for adding into static variable.
            CommentReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                CommentReportModelAccount.Add(new ReplyCommentReportDetails
                {
                    Id = ++sNo,
                    AccountUsername = report.Username,
                    MediaCode = report.PkOwner,
                    CommentOwnerName = report.UsernameOwner,
                    ActivityType = report.ActivityType,
                    Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                    MediaType = report.MediaType.ToString(),
                    Comments = report.Comment,
                    status = "Reply Commented"
                });
            return CommentReportModelAccount;
        }

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return new ObservableCollection<QueryInfo>();
        }
    }
}