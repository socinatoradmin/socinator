using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
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
using System.Windows.Data;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class SendGreetingsToFriendsReport : IFdReportFactory
    {
        public static ObservableCollection<UserReportModel> InteractedUsersModel =
            new ObservableCollection<UserReportModel>();

        public static List<UserReportAccountModel> AccountsInteractedUser = new List<UserReportAccountModel>();

        private readonly string _activityType = ActivityType.SendGreetingsToFriends.ToString();
        public string Header { get; set; } = string.Empty;


        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<MessageRecentFriendsModel>(activitySettings).SavedQueries;
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedUsers>(x => x.ActivityType == _activityType).ToList();

            AccountsInteractedUser.Clear();

            var id = 1;

            foreach (InteractedUsers report in reportDetails)
            {
                KeyValuePair<string, string> message;
                try
                {
                    message = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(report.DetailedUserInfo);
                }
                catch (Exception ex)
                {
                    message = new KeyValuePair<string, string>(report.DetailedUserInfo, "");
                    ex.DebugLog();
                }

                AccountsInteractedUser.Add(
                    new UserReportAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        ActivityType = ActivityType.SendGreetingsToFriends.ToString(),
                        UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                        UserId = report.UserId,
                        UserName = report.Username,
                        Message = message.Key,
                        UploadedMediaPath = message.Value,
                        IsPublishedPostOnTimeline = report.IsPublishedToWall,
                        PostDescription = report.PostDescription,
                        PublishedPostUrl = report.PublishedUrl,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    }
                );

                id++;
            }

            return AccountsInteractedUser;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,User Profile Url,Message Sent To,Message Text,Media Path,Published On Timeline,Post Description,Published Post Url,Date";
                Header = PostsReportHeader();
                InteractedUsersModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + "," + report.QueryType + ","
                                    + report.QueryValue + ","
                                    + report.UserProfileUrl + ","
                                    + report.UserName + ","
                                    + (string.IsNullOrEmpty(report.Message)
                                        ? "NA"
                                        : report.Message?.Replace(",", string.Empty)?.Replace("\r\n", " ")
                                            ?.Replace("\n", " ")) + ","
                                    + report.UploadedMediaPath + ","
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
                //Header = "QueryType,QueryValue,User Profile Url,Message Sent To,Message Text,Media Path,Published On Timeline,Post Description,Published Post Url,Date";
                Header = PostsReportHeader(false);
                AccountsInteractedUser.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.QueryType + ","
                                                     + report.QueryValue + ","
                                                     + report.UserProfileUrl + ","
                                                     + report.UserName + ","
                                                     + (string.IsNullOrEmpty(report.Message)
                                                         ? "NA"
                                                         : report.Message.Replace(",", string.Empty)
                                                             .Replace("\r\n", " ").Replace("\n", " ")) + ","
                                                     + report.UploadedMediaPath + ","
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

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedUsersModel.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>()
                .ForEach(
                    report =>
                    {
                        KeyValuePair<string, string> message;
                        try
                        {
                            message = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(
                                report.DetailedUserInfo);
                        }
                        catch (Exception ex)
                        {
                            message = new KeyValuePair<string, string>(report.DetailedUserInfo, "");
                            ex.DebugLog();
                        }

                        InteractedUsersModel.Add(new UserReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            Message = message.Key,
                            UploadedMediaPath = message.Value,
                            IsPublishedPostOnTimeline = report.IsPublishedToWall,
                            PostDescription = report.PostDescription,
                            PublishedPostUrl = report.PublishedUrl,
                            InteractionTimeStamp = report.InteractionDateTime
                        });
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
                    {
                        ColumnHeaderText = "LangKeyUserProfileUrl".FromResourceDictionary(),
                        ColumnBindingText = "UserProfileUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyRequestSentTo".FromResourceDictionary(),
                        ColumnBindingText = "UserName"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyTextMessage".FromResourceDictionary(), ColumnBindingText = "Message"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMedia".FromResourceDictionary(),
                        ColumnBindingText = "UploadedMediaPath"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPublishedOnTimeline".FromResourceDictionary(),
                        ColumnBindingText = "IsPublishedPostOnTimeline"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostDescription".FromResourceDictionary(),
                        ColumnBindingText = "PostDescription"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPublishedUrl".FromResourceDictionary(),
                        ColumnBindingText = "PublishedPostUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //});

            reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyMessageSentTo");
            listResource.Add("LangKeyMessageText");
            listResource.Add("LangKeyMediaPath");
            listResource.Add("LangKeyPublishedOnTimeline");
            listResource.Add("LangKeyPostDescription");
            listResource.Add("LangKeyPublishedPostUrl");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}