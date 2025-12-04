using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
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

namespace FaceDominatorUI.Utilities.Friends.IncommingFriendRequest
{
    public class IncommingFriendRequestReports : IFdReportFactory
    {
        public static ObservableCollection<UserReportModel> InteractedUsersModel =
            new ObservableCollection<UserReportModel>();

        public static List<UserReportAccountModel> AccountsInteractedUsers = new List<UserReportAccountModel>();

        public static List<string> Data = new List<string>();

        public static List<string> CampaignData = new List<string>();

        private readonly string _activityType = ActivityType.IncommingFriendRequest.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<IncommingFriendRequestModel>(activitySettings).SavedQueries;
        }


        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedUsers>(x => x.ActivityType == _activityType).ToList();


            AccountsInteractedUsers.Clear();

            Data.Clear();

            var id = 1;

            foreach (InteractedUsers report in reportDetails)
            {
                AccountsInteractedUsers.Add(
                    new UserReportAccountModel
                    {
                        Id = id,
                        QueryType = report.QueryType,
                        QueryValue = report.QueryValue,
                        UserId = report.UserId,
                        ActivityType = _activityType,
                        UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                        UserName = report.Username,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    }
                );

                if (!string.IsNullOrEmpty(report.DetailedUserInfo))
                {
                    var objfacebokuser = JsonConvert.DeserializeObject<FacebookUser>(report.DetailedUserInfo);

                    Data.Add(report.QueryType + ","
                                              + report.UserProfileUrl + ","
                                              + (string.IsNullOrEmpty(report.Username)
                                                  ? "NA"
                                                  : report.Username?.Replace(",", " ")) + ","
                                              + report.InteractionTimeStamp.EpochToDateTimeLocal() + ","
                                              + objfacebokuser.Gender + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.University)
                                                  ? "NA"
                                                  : objfacebokuser.University?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.WorkPlace)
                                                  ? "NA"
                                                  : objfacebokuser.WorkPlace?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.Currentcity)
                                                  ? "NA"
                                                  : objfacebokuser.Currentcity?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.Hometown)
                                                  ? "NA"
                                                  : objfacebokuser.Hometown?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.ProfilePicUrl)
                                                  ? "NA"
                                                  : objfacebokuser.ProfilePicUrl?.Replace(",", " ")));
                }
                else
                {
                    Data.Add(report.QueryType + "," + report.QueryValue + ","
                             + report.ActivityType + ","
                             + $"{FdConstants.FbHomeUrl}{report.UserId}" + ","
                             + report.Username + ","
                             + report.InteractionDateTime);
                }

                id++;
            }

            return AccountsInteractedUsers;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,Activity,User Profile Url,User Name,Date,Gender,University,Workplace,Currntcity,Hometown,Profilepic Url";

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
                //Header = "Activity,User Profile Url,User Name,Date,Gender,University,Workplace,Currntcity,Hometown,Profilepic Url";

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

        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            //Header = "Account Email,Activity Type,User Profile Url,Username,Interaction Date";
            Header = PostsReportHeader1();
            InteractedUsersModel.Clear();

            CampaignData.Clear();

            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            #region get data from InteractedUsers table and add to UnfollowerReportModel

            dataBase.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>()
                .ForEach(
                    report =>
                    {
                        InteractedUsersModel.Add(new UserReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            QueryType = report.QueryType,
                            ActivityType = report.ActivityType,
                            UserId = report.UserId,
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            Gender = report.Gender,
                            University = report.University,
                            Workplace = report.Workplace,
                            CurrentCity = report.CurrentCity,
                            HomeTown = report.HomeTown,
                            BirthDate = report.BirthDate,
                            ContactNo = report.ContactNo,
                            ProfilePic = report.ProfilePic,
                            InteractionTimeStamp = report.InteractionDateTime
                        });

                        if (!string.IsNullOrEmpty(report.DetailedUserInfo))
                        {
                            var objfacebokuser = JsonConvert.DeserializeObject<FacebookUser>(report.DetailedUserInfo);

                            CampaignData.Add(report.AccountEmail + "," + report.QueryType + ","
                                             + report.UserProfileUrl + ","
                                             + (string.IsNullOrEmpty(report.Username)
                                                 ? "NA"
                                                 : report.Username.Replace(",", " ")) + ","
                                             + report.InteractionTimeStamp.EpochToDateTimeLocal() + ","
                                             + (string.IsNullOrEmpty(report.Gender)
                                             ? "NA" : report.Gender) + ","
                                             + (string.IsNullOrEmpty(report.University)
                                                 ? "NA"
                                                 : report.University.Replace(",", " ")) + ","
                                             + (string.IsNullOrEmpty(report.Workplace)
                                                 ? "NA"
                                                 : report.Workplace.Replace(",", " ")) + ","
                                             + (string.IsNullOrEmpty(report.CurrentCity)
                                                 ? "NA"
                                                 : report.CurrentCity.Replace(",", " ")) + ","
                                             + (string.IsNullOrEmpty(report.HomeTown)
                                                 ? "NA"
                                                 : report.HomeTown.Replace(",", " ")) + ","
                                             + (string.IsNullOrEmpty(report.ProfilePic)
                                                 ? "NA"
                                                 : report.ProfilePic.Replace(",", " ")));
                        }
                        else
                        {
                            CampaignData.Add(report.QueryType + "," + report.QueryValue + ","
                                             + report.ActivityType.ToString() + ","
                                             + $"{FdConstants.FbHomeUrl}{report.UserId}" + ","
                                             + report.Username + ","
                                             + report.InteractionDateTime);
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
                        ColumnHeaderText = "LangKeyActivityType".FromResourceDictionary(),
                        ColumnBindingText = "ActivityType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserProfileUrl".FromResourceDictionary(),
                        ColumnBindingText = "UserProfileUrl"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyUserName".FromResourceDictionary(), ColumnBindingText = "UserName"},
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

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyActivityType");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyUserName");
            listResource.Add("LangKeyDate");
            listResource.Add("LangKeyGender");
            listResource.Add("LangKeyUniversity");
            listResource.Add("LangKeyWorkplace");
            listResource.Add("LangKeyCurrentcity");
            listResource.Add("LangKeyHomeTown");
            listResource.Add("LangKeyProfilePic");

            return listResource.ReportHeaderFromResourceDict();
        }

        public string PostsReportHeader1(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyActivityType");
            listResource.Add("LangKeyUserProfileUrl");
            listResource.Add("LangKeyUserName");
            listResource.Add("LangKeyInteractionDateTime");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}