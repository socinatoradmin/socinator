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

namespace FaceDominatorUI.Utilities.Scraper.MarketPlaceScaper
{
    public class MarketPlaceScraperReports : IFdReportFactory
    {
        public static ObservableCollection<UserReportModel> InteractedUsersModel =
            new ObservableCollection<UserReportModel>();

        public static List<SendFriendAccountModel> AccountsInteractedUsers = new List<SendFriendAccountModel>();

        public static List<string> Data = new List<string>();

        public static List<string> CampaignData = new List<string>();

        private readonly string _activityType = ActivityType.ProfileScraper.ToString();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<ProfileScraperModel>(activitySettings).SavedQueries;
        }


        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase.Get<InteractedUsers>(x => x.ActivityType == _activityType).ToList();

            AccountsInteractedUsers.Clear();

            Data.Clear();

            var id = 1;

            foreach (InteractedUsers report in reportDetails)
                if (AccountsInteractedUsers.FirstOrDefault(x => x.UserId == report.UserId) == null)
                {
                    AccountsInteractedUsers.Add(
                        new SendFriendAccountModel
                        {
                            Id = id,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            ActivityType = ActivityType.ProfileScraper.ToString(),
                            UserId = report.UserId,
                            UserProfileUrl = FdConstants.FbHomeUrl + report.UserId,
                            UserName = report.Username,
                            InteractionDateTime = report.InteractionTimeStamp.EpochToDateTimeLocal()
                        });

                    var objfacebokuser = JsonConvert.DeserializeObject<FacebookUser>(report.DetailedUserInfo);

                    Data.Add(report.QueryType + ","
                                              + report.QueryValue.Replace(",", string.Empty) + ","
                                              + report.UserProfileUrl + ","
                                              + (string.IsNullOrEmpty(report.Username)
                                                  ? "NA"
                                                  : report.Username?.Replace(",", " ")) + ","
                                              + objfacebokuser.Gender + ","
                                              + report.InteractionTimeStamp.EpochToDateTimeLocal() + ","
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
                                              + (string.IsNullOrEmpty(objfacebokuser.DateOfBirth)
                                                  ? "NA"
                                                  : objfacebokuser.DateOfBirth?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.ContactNo)
                                                  ? "NA"
                                                  : objfacebokuser.ContactNo?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.ProfilePicUrl)
                                                  ? "NA"
                                                  : objfacebokuser.ProfilePicUrl?.Replace(",", " ")) + ","
                                              + (string.IsNullOrEmpty(objfacebokuser.RelationShip)
                                                  ? "NA"
                                                  : objfacebokuser.RelationShip?.Replace(",", " ")));

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
                //Header = "AccountEmail,QueryType,QueryValue,User Profile Url,Username,Gender,Date,University,Workplace,Currntcity,Hometown,BirthDate,Contactno,Profile Pic Url, Relationship";
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
                //Header = "QueryType,QueryValue,User Profile Url,Username,Gender,Date,University,Workplace,Currntcity,Hometown,BirthDate,Contactno,Profile Pic Url, Relationship";
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
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedUsersModel.Clear();

            CampaignData.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>()
                .ForEach(
                    report =>
                    {
                        InteractedUsersModel.Add(new UserReportModel
                        {
                            Id = report.Id,
                            AccountEmail = report.AccountEmail,
                            QueryType = report.QueryType,
                            QueryValue = report.QueryValue,
                            ActivityType = report.ActivityType,
                            UserId = report.UserId,
                            UserProfileUrl = report.UserProfileUrl,
                            UserName = report.Username,
                            DetailedUserInfo = report.DetailedUserInfo,
                            InteractionTimeStamp = report.InteractionDateTime,
                            Gender = report.Gender,
                            University = report.University,
                            Workplace = report.Workplace,
                            CurrentCity = report.CurrentCity,
                            HomeTown = report.HomeTown,
                            BirthDate = report.BirthDate,
                            ContactNo = report.ContactNo,
                            ProfilePic = report.ProfilePic
                        });

                        var objfacebokuser = JsonConvert.DeserializeObject<FacebookUser>(report.DetailedUserInfo);


                        CampaignData.Add(report.AccountEmail + "," + report.QueryType + ","
                                         + report.QueryValue?.Replace(",", string.Empty) + ","
                                         + report.UserProfileUrl + ","
                                         + (string.IsNullOrEmpty(report.Username)
                                             ? "NA"
                                             : report.Username?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.Gender)
                                             ? "NA"
                                             : report.Gender) + "," + report.InteractionDateTime + ","
                                         + (string.IsNullOrEmpty(report.University)
                                             ? "NA"
                                             : report.University?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.Workplace)
                                             ? "NA"
                                             : report.Workplace?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.CurrentCity)
                                             ? "NA"
                                             : report.CurrentCity?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.HomeTown)
                                             ? "NA"
                                             : report.HomeTown?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.BirthDate)
                                             ? "NA"
                                             : report.BirthDate?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.ContactNo)
                                             ? "NA"
                                             : report.ContactNo?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(report.ProfilePic)
                                             ? "NA"
                                             : report.ProfilePic?.Replace(",", " ")) + ","
                                         + (string.IsNullOrEmpty(objfacebokuser.RelationShip)
                                             ? "NA"
                                             : objfacebokuser.RelationShip?.Replace(",", " ")));
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
                        {ColumnHeaderText = "LangKeyUserID".FromResourceDictionary(), ColumnBindingText = "UserId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyFullName".FromResourceDictionary(), ColumnBindingText = "UserName"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyUserProfileUrl".FromResourceDictionary(),
                        ColumnBindingText = "UserProfileUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            // });

            //reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

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
            listResource.Add("LangKeyUsername");
            listResource.Add("LangKeyGender");
            listResource.Add("LangKeyDate");
            listResource.Add("LangKeyUniversity");
            listResource.Add("LangKeyWorkplace");
            listResource.Add("LangKeyCurrentcity");
            listResource.Add("LangKeyHomeTown");
            listResource.Add("LangKeyBirthDate");
            listResource.Add("LangKeyContactNo");
            listResource.Add("LangKeyProfilePic");
            listResource.Add("LangKeyRelationship");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}