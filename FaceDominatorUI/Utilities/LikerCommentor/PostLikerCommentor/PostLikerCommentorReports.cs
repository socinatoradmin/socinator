using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FdReports;
using FaceDominatorCore.Interface;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostLikerCommentor
{
    public class PostLikerCommentorReports : IFdReportFactory
    {
        public static ObservableCollection<PostReportModel> InteractedPostModel =
            new ObservableCollection<PostReportModel>();

        public static List<PostReportAccountModel> AccountsInteractedPosts = new List<PostReportAccountModel>();
        public string Header { get; set; } = string.Empty;


        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<PostLikerCommentorModel>(activitySettings).SavedQueries;
        }

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);


            #region get data from InteractedUsers table and add to UnfollowerReportModel

            dataBase.GetAllInteractedData<InteractedPosts>().ForEach(
                report =>
                {
                    InteractedPostModel.Add(new PostReportModel
                    {
                        Id = report.Id,
                        AccountEmail = report.AccountEmail,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        PostId = report.PostId,
                        PostUrl = FdConstants.FbHomeUrl + report.PostId,
                        Comments = report.Comment,
                        LikeType = report.LikeType,
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
                        {ColumnHeaderText = "LangKeyPostUrl".FromResourceDictionary(), ColumnBindingText = "PostUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyComment".FromResourceDictionary(), ColumnBindingText = "Comments"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyLikeType".FromResourceDictionary(), ColumnBindingText = "LikeType"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //  });
            //  reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostModel);

            #endregion

            return new ObservableCollection<object>(InteractedPostModel);
        }


        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>(x =>
                    x.ActivityType == ActivityType.PostLikerCommentor.ToString()).ToList();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts report in reportDetails)
            {
                AccountsInteractedPosts.Add(
                    new PostReportAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        ActivityType = ActivityType.PostLikerCommentor.ToString(),
                        PostUrl = FdConstants.FbHomeUrl + report.PostId,
                        Comments = report.Comments,
                        LikeType = report.LikeType,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeUtc()
                    });

                id++;
            }

            return AccountsInteractedPosts;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Post_Url,Comment_Text,Like_type,Date";
                Header = PostsReportHeader();
                InteractedPostModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + "," + report.QueryType + ","
                                    + report.QueryValue + ","
                                    + $"{FdConstants.FbHomeUrl}{report.PostId}" + ","
                                    + report.Comments.Replace(",", string.Empty).Replace("\r\n", " ")
                                        .Replace("\n", " ") + ","
                                    + report.LikeType + ","
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
                //Header = "QueryType,QueryValue,Post_Url,Comment_Text,Like_type,Date";
                Header = PostsReportHeader(false);
                AccountsInteractedPosts.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + ","
                                                     + report.QueryValue + ","
                                                     + $"{FdConstants.FbHomeUrl}{report.PostId}" + ","
                                                     + report.Comments.Replace(",", string.Empty).Replace("\r\n", " ")
                                                         .Replace("\n", " ") + ","
                                                     + report.LikeType + ","
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

            var saveFiledialog = new SaveFileDialog
            {
                Filter = "CSV file (.csv)|*.csv",

                FileName = fileName
            };

            if (saveFiledialog.ShowDialog() == true)
            {
                var filename = saveFiledialog.FileName;

                DominatorHouseCore.Utility.Utilities.ExportReports(filename, Header, csvData);
            }
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyPostUrl");
            listResource.Add("LangKeyCommentText");
            listResource.Add("LangKeyLikeType");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}