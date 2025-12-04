using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorCore.Report;
using Newtonsoft.Json;

namespace GramDominatorUI.Utility.InstalikerCommenter.LikeComments
{
    public class LikeCommentReports : IGdReportFactory
    {
        private static readonly ObservableCollection<LikeCommentReportDetails> LikeCommentReportModelCampaign =
            new ObservableCollection<LikeCommentReportDetails>();

        private static List<LikeCommentReportDetails> LikeCommentReportModelAccount { get; } =
            new List<LikeCommentReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<LikeCommentModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            LikeCommentReportModelCampaign.Clear();

            #region get data from InteractedPosts table and add to LikerReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    if (report.Status == "Success")
                        LikeCommentReportModelCampaign.Add(new LikeCommentReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            LikedMediaCode = report.PkOwner,
                            LikedMediaOwner = report.UsernameOwner,
                            Comments = report.Comment,
                            status = "Liked",
                            Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                            MediaType = report.MediaType.ToString()
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
                        ColumnHeaderText = "LangKeyMediaOwner".FromResourceDictionary(),
                        ColumnBindingText = "LikedMediaOwner"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(),
                        ColumnBindingText = "LikedMediaCode"
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

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(LikeCommentReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(LikeCommentReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            var sNo = 0;
            LikeCommentReportModelAccount.Clear();
            dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>().ForEach(
                report =>
                {
                    LikeCommentReportModelAccount.Add(new LikeCommentReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        ActivityType = ActivityType.LikeComment,
                        LikedMediaOwner = report.UsernameOwner,
                        LikedMediaCode = report.PkOwner,
                        MediaType = report.MediaType.ToString(),
                        Comments = report.Comment,
                        status = "Liked",
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });
                });

            return LikeCommentReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Activity Type, Account Username, Media Code, Media type, Media Owner, Comment, Status,Interaction Date";

                LikeCommentReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.LikedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.LikedMediaOwner + "," + report.Comments + "," +
                                    report.status + "," + report.Date);
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
                    "Activity Type, Account Username, Media Code, Media type, Media Owner,  Comment, Status,Interaction Date";

                LikeCommentReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.LikedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.LikedMediaOwner + "," + report.Comments + "," +
                                    report.status + "," + report.Date);
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