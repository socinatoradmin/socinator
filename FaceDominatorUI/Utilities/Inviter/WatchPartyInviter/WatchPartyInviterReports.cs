using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
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

namespace FaceDominatorUI.Utilities.Inviter.WatchPartyInviter
{
    public class WatchPartyInviterReports : IFdReportFactory
    {
        public static List<string> DataAccount = new List<string>();

        public static List<string> DataCampaign = new List<string>();

        private readonly string _activityType = ActivityType.WatchPartyInviter.ToString();

        public List<PostReportAccountModel> AccountsInteractedPostModel = new List<PostReportAccountModel>();

        public ObservableCollection<PostReportModel> InteractedPostModel = new ObservableCollection<PostReportModel>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<PostScraperModel>(activitySettings).SavedQueries;
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedPosts>(x => x.ActivityType == _activityType).ToList();

            AccountsInteractedPostModel.Clear();
            DataAccount.Clear();

            var id = 1;

            foreach (InteractedPosts report in reportDetails)
            {
                AccountsInteractedPostModel.Add(
                    new PostReportAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        ActivityType = ActivityType.WatchPartyInviter.ToString(),
                        PostUrl = FdConstants.FbHomeUrl + report.PostId,
                        InvitedTo = $"{FdConstants.FbHomeUrl}{report.WatchPartInvitedTo}",
                        InvitedToUserName = $"{report.WatchPartInvitedToUserName}",
                        InteractionTimeStamp = report.InteractionDateTime
                    }
                );


                if (DataAccount.FirstOrDefault(x => x.Contains(report.PostId) && x.Contains(report.QueryType)) == null)
                    DataAccount.Add(report.QueryType + ","
                                                     + report.QueryValue + ","
                                                     + report.PostId + ","
                                                     + $"{FdConstants.FbHomeUrl}{report.PostId}" + ","
                                                     + $"{FdConstants.FbHomeUrl}{report.WatchPartInvitedTo}" + ","
                                                     + $"{report.WatchPartInvitedToUserName}" + ","
                                                     + report.InteractionDateTime);

                id++;
            }


            return AccountsInteractedPostModel;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Post Id,Post Url,Invited To Profile Url, Invited To User Name,Date";
                Header = PostsReportHeader();
                DataCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report);
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
                //Header = "QueryType,QueryValue,Post Id,Post Url,Invited to url,Invited to User name,Date";
                Header = PostsReportHeader(false);
                DataAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report);
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

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedPostModel.Clear();
            DataCampaign.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts>()
                .ForEach(
                    report =>
                    {
                        var post = JsonConvert.DeserializeObject<FacebookPostDetails>(report.PostDescription);

                        InteractedPostModel.Add(new PostReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            PostId = post.Id,
                            PostUrl = FdConstants.FbHomeUrl + report.PostId,
                            InvitedTo = $"{FdConstants.FbHomeUrl}{report.WatchPartInvitedTo}",
                            InvitedToUserName = $"{report.WatchPartInvitedToUserName}",
                            InteractionTimeStamp = report.InteractionDateTime
                        });
                        if (DataCampaign.FirstOrDefault(x =>
                                x.Contains(report.PostId) && x.Contains(report.AccountEmail)) == null)
                            DataCampaign.Add(report.AccountEmail + "," + report.QueryType + ","
                                             + report.QueryValue + ","
                                             + report.PostId + ","
                                             + $"{FdConstants.FbHomeUrl}{report.PostId}" + ","
                                             + $"{FdConstants.FbHomeUrl}{report.WatchPartInvitedTo}" + ","
                                             + $"{report.WatchPartInvitedToUserName}" + ","
                                             + report.InteractionDateTime);
                    });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
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
                        {ColumnHeaderText = "LangKeyPostId".FromResourceDictionary(), ColumnBindingText = "PostId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostUrl".FromResourceDictionary(), ColumnBindingText = "PostUrl"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyInvitationSentTo".FromResourceDictionary(),
                        ColumnBindingText = "InvitedTo"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyInvitedToUsername".FromResourceDictionary(),
                        ColumnBindingText = "InvitedToUserName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //});

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostModel);

            #endregion

            return new ObservableCollection<object>(InteractedPostModel);
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyPostId");
            listResource.Add("LangKeyPostUrl");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyUserName");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}