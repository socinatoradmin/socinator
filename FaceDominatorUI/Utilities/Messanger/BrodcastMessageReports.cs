using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
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

namespace FaceDominatorUI.Utilities.Messanger
{
    public class BrodcastMessageReports : IFdReportFactory
    {
        public static ObservableCollection<UserReportModel> InteractedUsersModel =
            new ObservableCollection<UserReportModel>();

        public static List<UserReportAccountModel> AccountsInteractedUser = new List<UserReportAccountModel>();

        public static List<string> Data = new List<string>();

        public static List<string> CampaignData = new List<string>();

        private readonly string _activityType = ActivityType.BroadcastMessages.ToString();
        public string Header { get; set; } = string.Empty;


        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<BrodcastMessageModel>(activitySettings).SavedQueries;
        }

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedUsersModel.Clear();

            CampaignData.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<InteractedUsers>().ForEach(
                report =>
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
                        InteractionTimeStamp = report.InteractionDateTime,
                        Gender = report.Gender,
                        University = report.University,
                        Workplace = report.Workplace,
                        CurrentCity = report.CurrentCity,
                        HomeTown = report.HomeTown,
                        BirthDate = report.BirthDate,
                        ContactNo = report.ContactNo
                    });

                    if (!string.IsNullOrEmpty(report.DetailedUserInfo))
                    {
                        var objfacebokuser = JsonConvert.DeserializeObject<FacebookUser>(report.DetailedUserInfo);

                        CampaignData.Add(report.AccountEmail + "," + report.QueryType + ","
                                         + report.QueryValue.Replace(",", string.Empty) + ","
                                         + report.UserProfileUrl + ","
                                         + (string.IsNullOrEmpty(report.Username)
                                             ? "NA"
                                             : report.Username.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(message.Key) ? "NA" : message.Key.Replace(",", " "))
                                         .Replace("\r\n", " ") + ","
                                         + (string.IsNullOrEmpty(message.Value)
                                             ? "NA"
                                             : message.Value.Replace(",", " ")) + ","
                                         + report.InteractionTimeStamp.EpochToDateTimeLocal() + ","
                                         + (string.IsNullOrEmpty(report.Gender)
                                         ? "NA" : report.Gender) + ","
                                         + (string.IsNullOrEmpty(report.University)
                                             ? "NA" : report.University.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.Workplace)
                                             ? "NA" : report.Workplace.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.CurrentCity)
                                             ? "NA" : report.CurrentCity.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.HomeTown)
                                             ? "NA" : report.HomeTown.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.BirthDate)
                                             ? "NA" : report.BirthDate) + ","
                                         + (string.IsNullOrEmpty(report.ContactNo)
                                             ? "NA" : report.ContactNo) + ","
                                         + (string.IsNullOrEmpty(objfacebokuser.ProfilePicUrl)
                                             ? "NA"
                                             : objfacebokuser.ProfilePicUrl.Replace(",", " ")));
                    }
                    else
                    {
                        CampaignData.Add(report.AccountEmail + "," + report.QueryType + ","
                                         + report.QueryValue + ","
                                         + report.UserProfileUrl + ","
                                         + report.Username + ","
                                         + (string.IsNullOrEmpty(message.Key)
                                             ? "NA"
                                             : message.Key.Replace(",", string.Empty).Replace("\r\n", " ")
                                                 .Replace("\n", " ")) + ","
                                         + message.Value + ","
                                         + report.InteractionTimeStamp);
                    }
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
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //});

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedUsersModel);
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>(x =>
                    x.ActivityType == _activityType).ToList();

            AccountsInteractedUser.Clear();

            var id = 1;

            Data.Clear();

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers report in reportDetails)
            {
                try
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
                            ActivityType = ActivityType.BroadcastMessages.ToString(),
                            UserId = report.UserId,
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            Message = message.Key,
                            UploadedMediaPath = message.Value,
                            InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                        }
                    );

                    if (!string.IsNullOrEmpty(report.DetailedUserInfo))
                    {
                        var objfacebokuser = JsonConvert.DeserializeObject<FacebookUser>(report.DetailedUserInfo);

                        Data.Add(report.QueryType + ","
                                                  + report.QueryValue?.Replace(",", string.Empty) + ","
                                                  + report.UserProfileUrl + ","
                                                  + (string.IsNullOrEmpty(report.Username)
                                                      ? "NA"
                                                      : report.Username?.Replace(",", " ")) + ","
                                                  + (string.IsNullOrEmpty(message.Key)
                                                      ? "NA"
                                                      : message.Key?.Replace(",", string.Empty)?.Replace("\r\n", " ")
                                                          ?.Replace("\n", " ") + ","
                                                                              + (string.IsNullOrEmpty(message.Value)
                                                                                  ? "NA"
                                                                                  : message.Value?.Replace(",", " ")) +
                                                                              ","
                                                                              + report.InteractionTimeStamp
                                                                                  .EpochToDateTimeLocal() + ","
                                                                              + objfacebokuser.Gender + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .University)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.University?.Replace(
                                                                                      ",", " ")) + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .WorkPlace)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.WorkPlace?.Replace(
                                                                                      ",", " ")) + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .Currentcity)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.Currentcity?.Replace(
                                                                                      ",", " ")) + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .Hometown)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.Hometown?.Replace(",",
                                                                                      " ")) + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .DateOfBirth)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.DateOfBirth?.Replace(
                                                                                      ",", " ")) + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .ContactNo)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.ContactNo?.Replace(
                                                                                      ",", " ")) + ","
                                                                              + (string.IsNullOrEmpty(objfacebokuser
                                                                                  .ProfilePicUrl)
                                                                                  ? "NA"
                                                                                  : objfacebokuser.ProfilePicUrl
                                                                                      ?.Replace(",", " "))));
                    }
                    else
                    {
                        Data.Add(report.QueryType + "," + report.QueryValue + ","
                                 + $"{FdConstants.FbHomeUrl}{report.UserId}"
                                 + "," + report.Username + ","
                                 + report.InteractionDateTime);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

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
                //Header = "AccountEmail,QueryType,QueryValue,User Profile Url,Message Sent To,Message Text,Media Path,Date,Gender,University,Workplace,Currntcity,Hometown,BirthDate,Contactno,Profilepic Url";
                Header = PostsReportHeader();
                CampaignData.ToList().ForEach(report =>
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
                //Header = "QueryType,QueryValue,User Profile Url,Message Sent To,Message Text,Media Path,Date,Gender,University,Workplace,Currntcity,Hometown,BirthDate,Contactno,Profilepic Url";
                Header = PostsReportHeader(false);
                Data.ToList().ForEach(report =>
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
            listResource.Add("LangKeyDate");
            listResource.Add("LangKeyGender");
            listResource.Add("LangKeyUniversity");
            listResource.Add("LangKeyWorkplace");
            listResource.Add("LangKeyCurrentcity");
            listResource.Add("LangKeyHomeTown");
            listResource.Add("LangKeyBirthDate");
            listResource.Add("LangKeyContactNo");
            listResource.Add("LangKeyProfilePic");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}