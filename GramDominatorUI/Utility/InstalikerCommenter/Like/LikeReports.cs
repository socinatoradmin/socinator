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

namespace GramDominatorUI.Utility.InstalikerCommenter.Like
{
    internal class LikeReports : IGdReportFactory
    {
        private static readonly ObservableCollection<LikeReportDetails> LikeReportModelCampaign =
            new ObservableCollection<LikeReportDetails>();

        private static List<LikeReportDetails> LikeReportModelAccount { get; } = new List<LikeReportDetails>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<LikeModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            LikeReportModelCampaign.Clear();

            #region Update Existing Table with new Column

            try
            {
                if (!campaignDetails.IsInteractedPostsUpdated)
                {
                    var query = "UPDATE InteractedPosts SET Status = 'Success' WHERE Status IS NULL";
                    dataBase._context.Database.ExecuteSqlCommand(query);
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

            #region get data from InteractedPosts table and add to LikerReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    if (report.Status == "Success")
                        LikeReportModelCampaign.Add(new LikeReportDetails
                        {
                            Id = ++sNo,
                            AccountUsername = report.Username,
                            LikedMediaCode = report.PkOwner,
                            LikedMediaOwner = report.UsernameOwner,
                            Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime(),
                            MediaType = report.MediaType.ToString()
                        });
                    //}
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
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(LikeReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(LikeReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.Like).ToList();

            // Need to be cleared data for adding into static variable.
            LikeReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                LikeReportModelAccount.Add(
                    new LikeReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        ActivityType = ActivityType.Like,
                        LikedMediaOwner = report.UsernameOwner,
                        LikedMediaCode = report.PkOwner,
                        MediaType = report.MediaType.ToString(),
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });

            return LikeReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date";

                LikeReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.LikedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.LikedMediaOwner + "," + report.Date);
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
                Header = "Activity Type, Account Username, Media Code, Media type, Media Owner, Interaction Date";

                LikeReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.LikedMediaCode +
                                    "," +
                                    report.MediaType + "," + report.LikedMediaOwner + "," + report.Date);
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