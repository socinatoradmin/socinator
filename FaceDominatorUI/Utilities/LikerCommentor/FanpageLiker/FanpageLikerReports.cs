using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDModel.LikerCommentorModel;
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

namespace FaceDominatorUI.Utilities.LikerCommentor.FanpageLiker
{
    public class FanpageLikerReports : IFdReportFactory
    {
        public static ObservableCollection<PageReportModel> InteractedPageModel =
            new ObservableCollection<PageReportModel>();

        public static List<PageReportAccountModel> AccountsInteractedPages = new List<PageReportAccountModel>();

        private readonly string _activityType = ActivityType.FanpageLiker.ToString();
        public string Header { get; set; } = string.Empty;


        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<FanpageLikerModel>(activitySettings).SavedQueries;
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
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyFanpageLikers".FromResourceDictionary(),
                        ColumnBindingText = "TotalLikers"
                    },
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
            //});
            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPageModel);

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
                        ActivityType = ActivityType.FanpageLiker.ToString(),
                        PageName = report.PageName,
                        PageUrl = report.PageUrl,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    }
                );

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
                //Header = "AccountEmail,QueryType,QueryValue,Page Name,Page Url,Total Likers,Page Type,Date";
                Header = PostsReportHeader();
                InteractedPageModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + ","
                                                        + report.QueryType + ","
                                                        + report.QueryValue + ","
                                                        + (string.IsNullOrEmpty(report.PageName)
                                                            ? "NA"
                                                            : report.PageName.Replace(",", " ")) + ","
                                                        + report.PageUrl + ","
                                                        + report.TotalLikers + ","
                                                        + report.PageType + ","
                                                        + report.InteractionTimeStamp);
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
                //Header = "QueryType,QueryValue,Page Name,Page Url,Total Likers,Page Type,Date";
                Header = PostsReportHeader(false);
                AccountsInteractedPages.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + ","
                                                     + report.QueryValue + ","
                                                     + (string.IsNullOrEmpty(report.PageName)
                                                         ? "NA"
                                                         : report.PageName.Replace(",", " ")) + ","
                                                     + report.PageUrl + ","
                                                     + report.TotalLikers + ","
                                                     + report.PageType + ","
                                                     + report.InteractionTimeStamp);
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
            listResource.Add("LangKeyPageName");
            listResource.Add("LangKeyPageUrl");
            listResource.Add("LangKeyTotalLikers");
            listResource.Add("LangKeyFanpageType");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}