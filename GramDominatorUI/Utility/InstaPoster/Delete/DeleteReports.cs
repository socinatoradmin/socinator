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

namespace GramDominatorUI.Utility.InstaPoster.Delete
{
    internal class DeleteReports : IGdReportFactory
    {
        private static readonly ObservableCollection<DeletePostReportDetails> DeletePostReportModelCampaign =
            new ObservableCollection<DeletePostReportDetails>();

        private static List<DeletePostReportDetails> DeletePostReportModelAccount { get; } =
            new List<DeletePostReportDetails>();

        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<DeletePostModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> lstQueryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            // Need to be cleared data for adding into static variable.
            DeletePostReportModelCampaign.Clear();

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


            #region get data from InteractedUsers table and add to _followerReportModel

            var sNo = 0;
            dataBase.Get<InteractedPosts>().ForEach(
                report =>
                {
                    DeletePostReportModelCampaign.Add(new DeletePostReportDetails
                    {
                        Id = ++sNo,
                        AccountUsername = report.Username,
                        MediaCode = report.PkOwner,
                        MediaType = report.MediaType,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
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
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaCode".FromResourceDictionary(), ColumnBindingText = "MediaCode"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyDate".FromResourceDictionary(), ColumnBindingText = "Date"}
                };

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(DeletePostReportModelCampaign);

            #endregion

            return new ObservableCollection<object>(DeletePostReportModelCampaign);
        }

        public IList GetsAccountReport(IDbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts>()
                .Where(x => x.ActivityType == ActivityType.DeletePost).ToList();

            // Need to be cleared data for adding into static variable.
            DeletePostReportModelAccount.Clear();
            var sNo = 0;
            foreach (DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts report in reportDetails)
                DeletePostReportModelAccount.Add(
                    new DeletePostReportDetails
                    {
                        Id = ++sNo,
                        ActivityType = report.ActivityType,
                        AccountUsername = report.Username,
                        MediaCode = report.PkOwner,
                        MediaType = report.MediaType,
                        Date = report.InteractionDate.EpochToDateTimeUtc().ToLocalTime()
                    });

            return DeletePostReportModelAccount;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header = "Activity type, Account Name, Media Code, Media Type, Date";

                DeletePostReportModelCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.MediaCode + "," +
                                    report.MediaType + "," +
                                    report.Date);
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
                Header = "Activity type, Account Name, Media Code, Media Type, Date";

                DeletePostReportModelAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.ActivityType + "," + report.AccountUsername + "," + report.MediaCode + "," +
                                    report.MediaType + "," + report.Date);
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