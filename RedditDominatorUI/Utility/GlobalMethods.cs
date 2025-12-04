using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler;
using System.Collections.ObjectModel;
using DominatorUIUtility.CustomControl;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using RedditDominatorUI.CustomControl;
using RedditDominatorUI.RDViews.Engage;
using RedditDominatorCore.RedditModel;
using RedditDominatorUI.RDViews.UrlScraper;
using RedditDominatorUI.RDViews.Voting;
using RedditDominatorCore.RDEnums;
using RedditDominatorUI.RDViews.Grow_Followers;

using RedditDominatorCore.Report;
using DominatorHouseCore.DatabaseHandler.RdTables;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using System.Windows.Data;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;

namespace RedditDominatorUI.Utility
{
    public static class GlobalMethods
    {
        internal static void ShowUserFilterControl(SearchQueryControl _queryControl)
        {
            UserFiltersControl ObjUserFiltersControl = new UserFiltersControl();
            ObjUserFiltersControl.UserFilter.SaveCloseButtonVisible = true;
            Dialog ObjDialog = new Dialog();
            Window FilterWindow = ObjDialog.GetMetroWindow(ObjUserFiltersControl, "Filter");
            ObjUserFiltersControl.SaveButton.Click += (sender, e) =>
              {
                  _queryControl.CurrentQuery.CustomFilters = JsonConvert.SerializeObject(ObjUserFiltersControl.UserFilter);
                  FilterWindow.Close();
              };
            FilterWindow.Show();
        }
        public static void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType)
        {

            var AccountDetails = AccountsFileManager.GetAll();
            var OldAccountDetails = AccountsFileManager.GetAll();

            // this list contain detail of accounts which are already having setting

            ErrorModelControl objErrorModelControl = new ErrorModelControl();

            List<DominatorAccountModel> accountHavingSetting =
                        AccountDetails.Where(
                            x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == moduleType)
                                                     ?.TemplateId != null)?.ToList() ?? new List<DominatorAccountModel>();

            #region Setting  WarningText according to Module type

            switch (moduleType)
            {
                case ActivityType.Like:
                    objErrorModelControl.WarningText = Application.Current.FindResource("langLikeWarning").ToString();
                    break;
            }

            #endregion

            
            List<string> UnSelectedAccountForModification = new List<string>();
            
            if (accountHavingSetting.Count > 0)
            {
                try
                {
                    try
                    {
                        lstSelectedAccounts.ForEach(account => objErrorModelControl.Accounts.Add(new ErrorModelControl()
                        {
                            UserName = accountHavingSetting.FirstOrDefault(x => x.AccountBaseModel.UserName == account).AccountBaseModel.UserName
                        })
                        );
                    }
                    catch (Exception exx)
                    {


                    }

                    //Check if account is running with campaign or not if any account running with campaign then it will show ErrorModel
                    //there you can update campaign
                    if (objErrorModelControl.Accounts.Count > 0)
                    {
                        Dialog objDialog = new Dialog();
                        var WarningWindow = objDialog.GetMetroWindow(objErrorModelControl, "Warning");

                        //if we want to replace prvious setting we need to click save button
                        objErrorModelControl.BtnSave.Click += (senders, Events) =>
                        {
                            try
                            {
                                //Getting account that are not required to update
                                UnSelectedAccountForModification = objErrorModelControl.Accounts.Where(x => x.IsChecked == false).Select(x => x.UserName).ToList();

                                //To remove the account which we don't want to update with new Configuration
                                UnSelectedAccountForModification.ForEach(item => lstSelectedAccounts.Remove(item));

                                //it will check if account is updated or not if updated then will delete account and save that updated details
                                UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

                                //To update campaign file calling UpdateCampaignBinFile() method
                                UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
                                WarningWindow.Visibility = Visibility.Hidden;
                                WarningWindow.Close();
                            }
                            catch (Exception Ex)
                            {
                                Ex.DebugLog();
                            }
                        };
                        objErrorModelControl.BtnCancel.Click += (senders, Events) =>
                        {
                            WarningWindow.Close();
                        };
                        WarningWindow.ShowDialog();
                    }

                    else // if account is not running with any campaign then it will save 

                    {
                        //it will check if account is update or not if updated then will delete account and save that updated details
                        UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

                        //To update campaign file calling UpdateCampaignBinFile() method
                        UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else // if account is not running with any campaign then it will save 

            {
                //it will check if account is update or not if updated then will delete account and save that updated details
                UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

                //To update campaign file calling UpdateCampaignBinFile() method
                UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
            }

        }

        public static bool UpdateSelectedAccountDetails(IEnumerable<DominatorAccountModel> allAccountDetails, List<string> listSelectedAccounts, ActivityType moduleType)
        {
            bool isAccountDetailsUpdated = false;

            List<DominatorAccountModel> selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);
            foreach (var account in allAccountDetails)
            {
                if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
                    continue;


                isAccountDetailsUpdated = true;
                try
                {
                    if (account.ActivityManager.RunningTime == null)
                        account.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

                    var moduleConfiguration = account.ActivityManager.LstModuleConfiguration?.FirstOrDefault(y => y.ActivityType == moduleType);
                    if (moduleConfiguration == null)
                    {
                        moduleConfiguration = new ModuleConfiguration() { ActivityType = moduleType };
                        account.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
                    }


                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";

                    #region Update TemplateId and Module for RunningTime for required Model

                    switch (moduleType)
                    {
                        case ActivityType.Comment:
                            Comment objLike = Comment.GetSingletonObjectComment();
                            moduleConfiguration.TemplateId = objLike.TemplateId;

                            objLike.ObjViewModel.Comment_Model.JobConfiguration.RunningTime.ForEach(x =>
                            {
                                foreach (var timingRange in x.Timings)
                                {
                                    timingRange.Module = ActivityType.Like.ToString();
                                }
                            });
                            account.ActivityManager.RunningTime =
                                objLike.ObjViewModel.Comment_Model.JobConfiguration.RunningTime;
                            break;

                        case ActivityType.UrlScraper:
                            RedditDominatorUI.RDViews.UrlScraper.UrlScraper objUrlScraper = RedditDominatorUI.RDViews.UrlScraper.UrlScraper.GetSingletonObjectUrlScraper();
                            moduleConfiguration.TemplateId = objUrlScraper.TemplateId;

                            objUrlScraper.ObjViewModel.UrlScraperModel.JobConfiguration.RunningTime.ForEach(x =>
                            {
                                foreach (var timingRange in x.Timings)
                                {
                                    timingRange.Module = ActivityType.UrlScraper.ToString();
                                }
                            });
                            account.ActivityManager.RunningTime =
                                objUrlScraper.ObjViewModel.UrlScraperModel.JobConfiguration.RunningTime;
                            break;
                        case ActivityType.UpvoteAnswers:
                            Upvote objUpvote = Upvote.GetSingletonObjectUpvote();
                            moduleConfiguration.TemplateId = objUpvote.TemplateId;

                            objUpvote.ObjViewModel.UpvoteModel.JobConfiguration.RunningTime.ForEach(x =>
                            {
                                foreach (var timingRange in x.Timings)
                                {
                                    timingRange.Module = ActivityType.UpvoteAnswers.ToString();
                                }
                            });
                            account.ActivityManager.RunningTime =
                                objUpvote.ObjViewModel.UpvoteModel.JobConfiguration.RunningTime;
                            break;
                        case ActivityType.Follow:
                            Follow objFollow = Follow.GetSingletonObjectFollow();
                            moduleConfiguration.TemplateId = objFollow.TemplateId;

                            objFollow.ObjViewModel.FollowModel.JobConfiguration.RunningTime.ForEach(x =>
                            {
                                foreach (var timingRange in x.Timings)
                                {
                                    timingRange.Module = ActivityType.UpvoteAnswers.ToString();
                                }
                            });
                            account.ActivityManager.RunningTime =
                                objFollow.ObjViewModel.FollowModel.JobConfiguration.RunningTime;
                            break;
                        case ActivityType.DownvoteAnswers:
                            Downvote objDownvote = Downvote.GetSingletonObjectDownvote();
                            moduleConfiguration.TemplateId = objDownvote.TemplateId;

                            objDownvote.ObjViewModel.DownvoteModel.JobConfiguration.RunningTime.ForEach(x =>
                            {
                                foreach (var timingRange in x.Timings)
                                {
                                    timingRange.Module = ActivityType.DownvoteAnswers.ToString();
                                }
                            });
                            account.ActivityManager.RunningTime =
                                objDownvote.ObjViewModel.DownvoteModel.JobConfiguration.RunningTime;
                            break;
                    }
                    #endregion


                    moduleConfiguration.IsTemplateMadeByCampaignMode = true;
                    selectedAccounts.Add(account);

                    var socinatorAccountBuilder = new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                .AddOrUpdateCookies(account.Cookies)
                .SaveToBinFile();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            // save all accounts and schedule actitvities of selected accounts            
            foreach (var account in selectedAccounts)
            {
                DominatorScheduler.ScheduleTodayJobs(account, SocialNetworks.Reddit, moduleType);
                DominatorScheduler.ScheduleForEachModule(moduleToIgnore: moduleType, account: account, network: SocialNetworks.Reddit);
            }

            return isAccountDetailsUpdated;
        }
        public static void UpdateCampaignBinFile(IEnumerable<DominatorAccountModel> allAccountDetails, List<string> lstSelectedAccounts, ActivityType moduleType)
        {
            var campaignsList = CampaignsFileManager.Get();

            if (campaignsList.Count == 0)
                return;

            try
            {
                foreach (var selectedAccount in lstSelectedAccounts)
                {
                    var SelectedAccount = allAccountDetails.FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccount);
                    var TemplateId = SelectedAccount.ActivityManager.LstModuleConfiguration
                        .FirstOrDefault(y => y.ActivityType == moduleType).TemplateId;

                    foreach (var campaign in campaignsList)
                    {
                        if (campaign.TemplateId == TemplateId)
                        {
                            campaign.SelectedAccountList.Remove(selectedAccount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            // Save campaigns back
            CampaignsFileManager.Save(campaignsList);
        }

        public static void AddNewCampaign(List<string> listSelectedAccounts, ActivityType moduleType)
        {

            string CampaignName = String.Empty;
            string TemplateId = String.Empty;
            string MainModule = string.Empty;
            string SubModule = string.Empty;
            Object newObject = null;

            #region Update CampaignName,TemplateId,MainModule and SubModule for Respective Module

            switch (moduleType)
            {
                case ActivityType.UrlScraper:
                    RedditDominatorUI.RDViews.UrlScraper.UrlScraper objUrlScraper = RedditDominatorUI.RDViews.UrlScraper.UrlScraper.GetSingletonObjectUrlScraper();
                    newObject = objUrlScraper;
                    CampaignName = objUrlScraper.CampaignName;
                    TemplateId = objUrlScraper.TemplateId;
                    //MainModule = Enums.PdMainModule.TryComment.ToString();
                    MainModule = "UrlScraper";
                    SubModule = ActivityType.UrlScraper.ToString();
                    break;
                case ActivityType.UpvoteAnswers:
                    Upvote objUpvote = Upvote.GetSingletonObjectUpvote();
                    newObject = objUpvote;
                    CampaignName = objUpvote.CampaignName;
                    TemplateId = objUpvote.TemplateId;
                    MainModule = Enums.RDMainModule.Voting.ToString();
                    MainModule = "Upvote";
                    SubModule = ActivityType.UpvoteAnswers.ToString();
                    break;
                case ActivityType.DownvoteAnswers:
                    Downvote objDownvote = Downvote.GetSingletonObjectDownvote();
                    newObject = objDownvote;
                    CampaignName = objDownvote.CampaignName;
                    TemplateId = objDownvote.TemplateId;
                    MainModule = Enums.RDMainModule.Voting.ToString();
                    MainModule = "Upvote";
                    SubModule = ActivityType.DownvoteAnswers.ToString();
                    break;
            }
            #endregion

            var campaignDetails = new CampaignDetails()
            {
                CampaignName = CampaignName,
                MainModule = MainModule,
                SubModule = SubModule,
                SocialNetworks = SocialNetworks.Reddit,
                SelectedAccountList = listSelectedAccounts,
                TemplateId = TemplateId,
                CreationDate = DateTimeUtilities.GetEpochTime(),
                Status = "Active",
                LastEditedDate = DateTimeUtilities.GetEpochTime(),
            };

            var campaignList = CampaignsFileManager.Get();

            // If campaign with such name already exists
            if (campaignList != null && campaignList.Any(x => x.CampaignName == CampaignName))
            {
                string warningMessege = "This account is already running with " + moduleType + " configuration from another campaign. Saving this settings will override previous settings and remove this account from the campaign.\r\nWould you still like to proceed?";

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(newObject, "Warning",
                        warningMessege, MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton());

                if (dialogResult == MessageDialogResult.Negative)
                    return;

                // Update campaign
                foreach (var campaign in campaignList)
                {
                    if (campaign.CampaignName == CampaignName)
                    {
                        campaign.CampaignName = CampaignName;
                        campaign.MainModule = MainModule;
                        campaign.SubModule = SubModule;
                        campaign.SocialNetworks = SocialNetworks.Reddit;
                        campaign.SelectedAccountList = listSelectedAccounts;
                        campaign.TemplateId = TemplateId;
                        campaign.CreationDate = DateTimeUtilities.GetEpochTime();
                        campaign.Status = "Active";
                        campaign.LastEditedDate = DateTimeUtilities.GetEpochTime();
                    }
                }
            }
            else
            {
                campaignList.Add(campaignDetails);

                // create new database for campaign
               // DataBaseHandler.CreateDataBase(campaignDetails.CampaignId, SocialNetworks.Reddit, DatabaseType.CampaignType);
            }


            // update Campaign with new campaign
            CampaignsFileManager.Add(campaignDetails);

            foreach (var userName in listSelectedAccounts)
            {


                DominatorAccountModel dominatorAccount = AccountsFileManager.GetAccount(userName,SocialNetworks.Reddit);

                DominatorScheduler.ScheduleTodayJobs(dominatorAccount, SocialNetworks.Reddit, moduleType);
            }


#if DEBUG
            var testResult = CampaignsFileManager.Get();
#endif
        }

      
    }
    public static class CampaignHelper
    {

        public static void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails, bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

                switch (ActivityType)
                {
                    case ActivityType.Comment:

                        Comment objComment = Comment.GetSingletonObjectComment();
                        objComment.IsEditCampaignName = IsEditCampaignName;
                        objComment.CancelEditVisibility = CancelEditVisibility;
                        objComment.CampaignButtonContent = CampaignButtonContent;
                        objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objComment.TemplateId = TemplateID;
                        objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objComment.CampaignName = campaignDetails.CampaignName;
                        objComment.ObjViewModel.Comment_Model = JsonConvert.DeserializeObject<Comment_Model>(templateDetails.ActivitySettings);
                        objComment.MainGrid.DataContext = objComment.ObjCommentViewModel.Comment_Model;
                        TabSwitcher.ChangeTabIndex(4, 0);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
            }
        }
    }
    public static class ReportHelper
    {
        static ObservableCollection<InteractedPostReportDetails> InteractedPostsModel = new ObservableCollection<InteractedPostReportDetails>();
        static ObservableCollection<InteractedUsersReportDetails> InteractedUsersModel = new ObservableCollection<InteractedUsersReportDetails>();
        //static ObservableCollection<InteractedSubreddit> InteractedSubredditModel = new ObservableCollection<InteractedSubreddit>();

        static string Header = string.Empty;
       
        internal static string GetHeader()
        {
            return Header;
        }

        /// <summary>
        /// ObjReports=>this is Report UI object
        /// lstCurrentQueries=>this is dictionary of current campaign QueryValue(key) and QueryType(value)
        /// dataBase=>this is DataBaseConnectionCodeFirst.DataBaseConnection Object
        /// campaign=>this is CampaignDetails Object
        /// this method return report details
        /// </summary>
        /// <returns></returns>

        internal static int GetReportDetail(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations dataBase, CampaignDetails campaign)
         {
            InteractedUsersModel.Clear();
            InteractedPostsModel.Clear();
            //UnfollowedUsersReportModel.Clear();
            //InteractedBoardsReportModel.Clear();
            TimeSpan ForLocalTime = DateTime.Now - DateTime.UtcNow;
            //if (lstCurrentQueries.Any(x => x.Value == "CustomBoard") && campaign.SubModule == "Follow")
            //{
            //    campaign.SubModule = "BoardScraper";
            //}
            switch (campaign.SubModule)
            {

                #region Follow
                case "Follow":
                    //case:"UnFollow";
                    //case:"Delete";
                    //case:"Reply";
                    //case:"Comment";
                    //case:"UrlScraper";
                    //case "Upvote":
                    //case "Downvote":
                    Header = "Id,Query,Query Type, Interaction Time, Activity Type,Username,Interacted Username";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    try
                    {
                        dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedUsers>()?.ForEach(
                                 report =>
                                 {
                                     InteractedUsersModel.Add(new InteractedUsersReportDetails()
                                     {
                                         Username = report.Username,
                                         InteractedUsername = report.InteractedUsername,
                           //Id = report.RedditId,
                           //  BoardId = report.,
                           ActivityType = ActivityType.Follow.ToString(),
                                         Query = report.Query,
                                         QueryType = report.QueryType,
                                         InteractionTime = report.InteractionTime
                                     });
                                 });
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    #endregion

                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                    objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query", ColumnBindingText = "Query" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted Username", ColumnBindingText = "InteractedUsername" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Username", ColumnBindingText = "Username" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction Time", ColumnBindingText = "InteractionTime" },
                    };
                    //});

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);
                    return InteractedUsersModel.Count;
                #endregion
                #endregion Like

                case "UrlScraper":

                    Header = "Id,Query,Query Type, Interaction Time, Activity Type,Username,Interacted Username,Title, CommentUrl,PointsCount, CommentsCount, PostCreationDate, BriefInfo, RedditDescription";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    try
                    {
                        dataBase.Get<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>()?.ForEach(
                                 report =>
                                 {
                                     InteractedPostsModel.Add(new InteractedPostReportDetails()
                                     {
                                         CommentsCount = report.CommentsCount,
                                         CommentUrl = report.CommentUrl,
                                         OperationType = ActivityType.Follow,
                                         //Query = report.Query,
                                         //QueryType = report.QueryType,
                                         InteractionDate = report.InteractionDate,
                                         BriefInfo = report.BriefInfo,
                                         PointsCount = report.PointsCount,
                                         PostCreationDate = report.PostCreationDate,
                                         RedditDescription = report.RedditDescription,
                                         Title = report.Title
                                     });
                                 });
                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion

                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                    objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query", ColumnBindingText = "Query" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Operation Type", ColumnBindingText = "OperationType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Count", ColumnBindingText = "CommentsCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Url", ColumnBindingText = "CommentUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction Date", ColumnBindingText = "InteractionDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Brief Info", ColumnBindingText = "BriefInfo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Points Count", ColumnBindingText = "PointsCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Creation Date", ColumnBindingText = "PostCreationDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reddit Description", ColumnBindingText = "RedditDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Title", ColumnBindingText = "Title" },
                    };
                    //});

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    return InteractedPostsModel.Count;
                    #endregion
            }
            return objReports.ReportModel.ReportCollection.Cast<object>().Count();
        }

        internal static ObservableCollection<QueryInfo> GetSavedQuery(string ModuleType, string ActivitySettings)
        {
            ObservableCollection<QueryInfo> lstSavedQuery = null;

            #region getting list of Saved Query according to Module type

            switch (ModuleType)
            {
                case "Follow":
                    lstSavedQuery = JsonConvert.DeserializeObject<RedditDominatorCore.RDModel.FollowModel>(ActivitySettings).SavedQueries;
                    break;

                case "UrlScraper":
                    lstSavedQuery = JsonConvert.DeserializeObject<RedditDominatorCore.RDModel.UrlScraperModel>(ActivitySettings).SavedQueries;
                    break;
            }

            #endregion

            return lstSavedQuery;
        }

        internal static void ExportReports(string ModuleType, string filename)
        {
            switch (ModuleType)
            {
                case "Upvote":
                    InteractedUsersModel.ToList().ForEach(report =>
                    {
                        try
                        {
                            var csvData = report.Username + "," + report.QueryType + "," + report.Username + "," + report.Username + "," + report.Username;

                            using (var streamWriter = new StreamWriter(filename, true))
                            {
                                streamWriter.WriteLine(csvData);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    });
                    break;
            }
        }
    }
}
