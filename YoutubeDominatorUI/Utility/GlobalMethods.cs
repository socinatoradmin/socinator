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
//using PinDominatorCore.Report;
//using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorUIUtility.CustomControl;
using YoutubeDominatorUI.YDViews.Engage;
using YoutubeDominatorUI.CustomControl;
using YoutubeDominatorCore.YoutubeModel;
using YoutubeDominatorUI.YDViews.GrowSubscribers;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using YoutubeDominatorUI.YDViews.Scraper;

using DominatorHouseCore.DatabaseHandler.YdTables;
using YoutubeDominatorCore.YDEnums;
using DominatorHouseCore.DatabaseHandler.GplusTables.Accounts;
using YoutubeDominatorCore.Report;
using System.Windows.Data;
using YoutubeDominatorUI.YDViews.WatchVideo;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;

namespace YoutubeDominatorUI.Utility
{
//    public static class GlobalMethods
//    {
//        internal static void ShowUserFilterControl(SearchQueryControl _queryControl)
//        {
//            UserFiltersControl ObjUserFiltersControl = new UserFiltersControl();
//            //ObjUserFiltersControl.UserFilter.SaveCloseButtonVisible = true;
//            Dialog ObjDialog = new Dialog();
//            Window FilterWindow = ObjDialog.GetMetroWindow(ObjUserFiltersControl, "Filter");
//            ObjUserFiltersControl.SaveButton.Click += (sender, e) =>
//              {
//                  _queryControl.CurrentQuery.CustomFilters = JsonConvert.SerializeObject(ObjUserFiltersControl.UserFilter);
//                  FilterWindow.Close();
//              };
//            FilterWindow.Show();
//        }
//        public static void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType)
//        {

//            var AccountDetails = AccountsFileManager.GetAll();
//            var OldAccountDetails = AccountsFileManager.GetAll();

//            // this list contain detail of accounts which are already having setting

//            ErrorModelControl objErrorModelControl = new ErrorModelControl();

//            List<DominatorAccountModel> accountHavingSetting =
//                        AccountDetails.Where(
//                            x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == moduleType)
//                                                     ?.TemplateId != null)?.ToList() ?? new List<DominatorAccountModel>();

//            #region Setting  WarningText according to Module type

//            switch (moduleType)
//            {
//                //case ActivityType.Follow:
//                //    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithFollowConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                //    break;
//                //case ActivityType.Unfollow:
//                //    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithUnfollowConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                //    break;
//                case ActivityType.Like:
//                    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithLikeConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                    break;
//                    //case ActivityType.Comment:
//                    //    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithCommentConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                    //    break;
//                    //case ActivityType.Repost:
//                    //    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithRepostConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                    //    break;
//                    //case ActivityType.DownloadScraper:
//                    //    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithPhotoscraperConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                    //    break;
//                    //case ActivityType.UserScraper:
//                    //    objErrorModelControl.WarningText = Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithUserscraperConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString();
//                    //    break;
//            }

//            #endregion


//            //this list contains list of all account which are not selected for modification
//            List<string> UnSelectedAccountForModification = new List<string>();

//            //it will check if any account having setting or not 
//            if (accountHavingSetting.Count > 0)
//            {
//                try
//                {
//                    try
//                    {
//                        lstSelectedAccounts.ForEach(account => objErrorModelControl.Accounts.Add(new ErrorModelControl()
//                        {
//                            UserName = accountHavingSetting.FirstOrDefault(x => x.AccountBaseModel.UserName == account).AccountBaseModel.UserName
//                        })
//                        );
//                    }
//                    catch (Exception exx)
//                    {


//                    }

//                    //Check if account is running with campaign or not if any account running with campaign then it will show ErrorModel
//                    //there you can update campaign
//                    if (objErrorModelControl.Accounts.Count > 0)
//                    {
//                        Dialog objDialog = new Dialog();
//                        var WarningWindow = objDialog.GetMetroWindow(objErrorModelControl, "Warning");

//                        //if we want to replace prvious setting we need to click save button
//                        objErrorModelControl.BtnSave.Click += (senders, Events) =>
//                        {
//                            try
//                            {
//                                //Getting account that are not required to update
//                                UnSelectedAccountForModification = objErrorModelControl.Accounts.Where(x => x.IsChecked == false).Select(x => x.UserName).ToList();

//                                //To remove the account which we don't want to update with new Configuration
//                                UnSelectedAccountForModification.ForEach(item => lstSelectedAccounts.Remove(item));

//                                //it will check if account is updated or not if updated then will delete account and save that updated details
//                                UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

//                                //To update campaign file calling UpdateCampaignBinFile() method
//                                UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
//                                WarningWindow.Visibility = Visibility.Hidden;
//                                WarningWindow.Close();
//                            }
//                            catch (Exception Ex)
//                            {
//                                Ex.DebugLog();
//                            }
//                        };
//                        objErrorModelControl.BtnCancel.Click += (senders, Events) =>
//                        {
//                            WarningWindow.Close();
//                        };
//                        WarningWindow.ShowDialog();
//                    }

//                    else // if account is not running with any campaign then it will save 

//                    {
//                        //it will check if account is update or not if updated then will delete account and save that updated details
//                        UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

//                        //To update campaign file calling UpdateCampaignBinFile() method
//                        UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.StackTrace);
//                }
//            }
//            else // if account is not running with any campaign then it will save 

//            {
//                //it will check if account is update or not if updated then will delete account and save that updated details
//                UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

//                //To update campaign file calling UpdateCampaignBinFile() method
//                UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
//            }

//        }

//        public static bool UpdateSelectedAccountDetails(IEnumerable<DominatorAccountModel> allAccountDetails, List<string> listSelectedAccounts, ActivityType moduleType)
//        {
//            bool isAccountDetailsUpdated = false;

//            List<DominatorAccountModel> selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);
//            foreach (var account in allAccountDetails)
//            {
//                if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
//                    continue;


//                isAccountDetailsUpdated = true;
//                try
//                {
//                    if (account.ActivityManager.RunningTime == null)
//                        account.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

//                    var moduleConfiguration = account.ActivityManager.LstModuleConfiguration?.FirstOrDefault(y => y.ActivityType == moduleType);
//                    if (moduleConfiguration == null)
//                    {
//                        moduleConfiguration = new ModuleConfiguration() { ActivityType = moduleType };
//                        account.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
//                    }


//                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
//                    moduleConfiguration.IsEnabled = true;
//                    moduleConfiguration.Status = "Active";

//                    #region Update TemplateId and Module for RunningTime for required Model

//                    switch (moduleType)
//                    {
//                        #region Like
//                        case ActivityType.Like:
//                            Like objLike = Like.GetSingeltonObjectLike();
//                            moduleConfiguration.TemplateId = objLike.TemplateId;

//                            objLike.ObjViewModel.likeModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Like.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objLike.ObjViewModel.likeModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion Like

//                        #region Unlike
//                        case ActivityType.Unlike:
//                            Dislike objDislike = Dislike.GetSingeltonObjectDislike();
//                            moduleConfiguration.TemplateId = objDislike.TemplateId;

//                            objDislike.ObjViewModel.dislikeModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Unlike.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objDislike.ObjViewModel.dislikeModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion Unlike

//                        #region LikeComment
//                        case ActivityType.LikeComment:
//                            LikeComment objLikeComment = LikeComment.GetSingeltonObjectLikeComment();
//                            moduleConfiguration.TemplateId = objLikeComment.TemplateId;

//                            objLikeComment.ObjViewModel.likeCommentModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Comment.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objLikeComment.ObjViewModel.likeCommentModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion LikeComment

//                        #region Comment
//                        case ActivityType.Comment:
//                            Comment objComment = Comment.GetSingeltonObjectComment();
//                            moduleConfiguration.TemplateId = objComment.TemplateId;

//                            objComment.ObjViewModel.commentModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Comment.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objComment.ObjViewModel.commentModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion Comment

//                        #region DeleteComment
//                        case ActivityType.DeleteComment:
//                            DeleteComment objDeleteComment = DeleteComment.GetSingeltonObjectDeleteComment();
//                            moduleConfiguration.TemplateId = objDeleteComment.TemplateId;

//                            objDeleteComment.ObjViewModel.DeleteCommentModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Comment.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objDeleteComment.ObjViewModel.DeleteCommentModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion DeleteComment

//                        #region Subscribe
//                        case ActivityType.Subscribe:
//                            Subscribe objSubscribe = Subscribe.GetSingeltonObjectSubscribe();
//                            moduleConfiguration.TemplateId = objSubscribe.TemplateId;

//                            objSubscribe.ObjViewModel.subscribeModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Subscribe.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objSubscribe.ObjViewModel.subscribeModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion Subscribe

//                        #region Unsubscribe
//                        case ActivityType.Unsubscribe:
//                            Unsubscribe objUnsubscribe = Unsubscribe.GetSingeltonObjectUnsubscribe();
//                            moduleConfiguration.TemplateId = objUnsubscribe.TemplateId;
//                            objUnsubscribe.ObjViewModel.UnsubscribeModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Unsubscribe.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objUnsubscribe.ObjViewModel.UnsubscribeModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion Unsubscribe

//                        #region Message
//                        case ActivityType.BroadcastMessages:
//                            Message objMessage = Message.GetSingeltonObjectMessage();
//                            moduleConfiguration.TemplateId = objMessage.TemplateId;
//                            objMessage.ObjViewModel.messageModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Unsubscribe.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objMessage.ObjViewModel.messageModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion Message

//                        #region ViewIncreaser
//                        case ActivityType.ViewIncreaser:
//                            ViewIncreaser objViewIncreaser = ViewIncreaser.GetSingeltonObjectViewIncreaser();
//                            moduleConfiguration.TemplateId = objViewIncreaser.TemplateId;

//                            objViewIncreaser.ObjViewModel.viewIncreaserModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.Comment.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objViewIncreaser.ObjViewModel.viewIncreaserModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion ViewIncreaser

//                        #region UserScraper
//                        case ActivityType.UserScraper:
//                            UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
//                            moduleConfiguration.TemplateId = objUserScraper.TemplateId;

//                            objUserScraper.ObjViewModel.userScraperModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.UserScraper.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objUserScraper.ObjViewModel.userScraperModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion UserScraper

//                        #region PostScraper
//                        case ActivityType.PostScraper:
//                            PostScraper objPostScraper = PostScraper.GetSingeltonObjectPostScraper();
//                            moduleConfiguration.TemplateId = objPostScraper.TemplateId;

//                            objPostScraper.ObjViewModel.postScraperModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.PostScraper.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objPostScraper.ObjViewModel.postScraperModel.JobConfiguration.RunningTime;
//                            break;
//                        #endregion PostScraper

//                        #region ChannelScraper
//                        case ActivityType.ChannelScraper:
//                            ChannelScraper objChannelScraper = ChannelScraper.GetSingeltonObjectChannelScraper();
//                            moduleConfiguration.TemplateId = objChannelScraper.TemplateId;

//                            objChannelScraper.ObjViewModel.channelScraperModel.JobConfiguration.RunningTime.ForEach(x =>
//                            {
//                                foreach (var timingRange in x.Timings)
//                                {
//                                    timingRange.Module = ActivityType.ChannelScraper.ToString();
//                                }

//                                //var dayWise = item.ActivityManager.RunningTime?.FirstOrDefault(activity => x.Day == activity.Day);

//                                //dayWise?.AddMultiTimeRange(x.Timings.ToList());
//                            });
//                            account.ActivityManager.RunningTime =
//                                objChannelScraper.ObjViewModel.channelScraperModel.JobConfiguration.RunningTime;
//                            break;
//                            #endregion ChannelScraper
//                    }

//                    #endregion Update TemplateId and Module for RunningTime for required Model

                    
//                    //moduleConfiguration.IsTemplateMadeByCampaignMode = true;
//                    //account.IsCretedFromNormalMode = true;
//                    selectedAccounts.Add(account);

//                    var socinatorAccountBuilder = new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
//                        .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
//                        .AddOrUpdateCookies(account.Cookies)
//                        .SaveToBinFile();
//                }
//                catch (Exception ex)
//                {
//                    ex.DebugLog();
//                }
//            }

//            // save all accounts and schedule actitvities of selected accounts            
//            foreach (var account in selectedAccounts)
//            {
//                DominatorScheduler.ScheduleTodayJobs(account, SocialNetworks.Youtube, moduleType);
//                DominatorScheduler.ScheduleForEachModule(moduleToIgnore: moduleType, account: account, network: SocialNetworks.Youtube);
//            }

//            return isAccountDetailsUpdated;
//        }
//        public static void UpdateCampaignBinFile(IEnumerable<DominatorAccountModel> allAccountDetails, List<string> lstSelectedAccounts, ActivityType moduleType)
//        {
//            var campaignsList = CampaignsFileManager.Get();

//            if (campaignsList.Count == 0)
//                return;

//            try
//            {
//                foreach (var selectedAccount in lstSelectedAccounts)
//                {
//                    var SelectedAccount = allAccountDetails.FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccount);
//                    var TemplateId = SelectedAccount.ActivityManager.LstModuleConfiguration
//                        .FirstOrDefault(y => y.ActivityType == moduleType).TemplateId;

//                    foreach (var campaign in campaignsList)
//                    {
//                        if (campaign.TemplateId == TemplateId)
//                        {
//                            campaign.SelectedAccountList.Remove(selectedAccount);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.ErrorLog();
//            }

//            // Save campaigns back
//            CampaignsFileManager.Save(campaignsList);
//        }

//        public static void AddNewCampaign(List<string> listSelectedAccounts, ActivityType moduleType)
//        {

//            string CampaignName = String.Empty;
//            string TemplateId = String.Empty;
//            string MainModule = string.Empty;
//            string SubModule = string.Empty;
//            Object newObject = null;

//            #region Update CampaignName,TemplateId,MainModule and SubModule for Respective Module

//            switch (moduleType)
//            {
//                #region Like
//                case ActivityType.Like:
//                    Like objLike = Like.GetSingeltonObjectLike();
//                    newObject = objLike;
//                    CampaignName = objLike.CampaignName;
//                    TemplateId = objLike.TemplateId;
//                    MainModule = Enums.YdMainModule.Like.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.Like.ToString();
//                    break;
//                #endregion Like

//                #region Unlike
//                case ActivityType.Unlike:
//                    Dislike objDislike = Dislike.GetSingeltonObjectDislike();
//                    newObject = objDislike;
//                    CampaignName = objDislike.CampaignName;
//                    TemplateId = objDislike.TemplateId;
//                    MainModule = Enums.YdMainModule.Unlike.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.Unlike.ToString();
//                    break;
//                #endregion Like

//                #region LikeComment
//                case ActivityType.LikeComment:
//                    LikeComment objLikeComments = LikeComment.GetSingeltonObjectLikeComment();
//                    newObject = objLikeComments;
//                    CampaignName = objLikeComments.CampaignName;
//                    TemplateId = objLikeComments.TemplateId;
//                    MainModule = Enums.YdMainModule.LikeComment.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.LikeComment.ToString();
//                    break;
//                #endregion LikeComment

//                #region Comment
//                case ActivityType.Comment:
//                    Comment objComment = Comment.GetSingeltonObjectComment();
//                    newObject = objComment;
//                    CampaignName = objComment.CampaignName;
//                    TemplateId = objComment.TemplateId;
//                    MainModule = Enums.YdMainModule.Comment.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.Comment.ToString();
//                    break;
//                #endregion Comment

//                #region DeleteComment
//                case ActivityType.DeleteComment:
//                    DeleteComment objDeleteComment = DeleteComment.GetSingeltonObjectDeleteComment();
//                    newObject = objDeleteComment;
//                    CampaignName = objDeleteComment.CampaignName;
//                    TemplateId = objDeleteComment.TemplateId;
//                    MainModule = Enums.YdMainModule.DeleteComment.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.DeleteComment.ToString();
//                    break;
//                #endregion DeleteComment

//                #region Subscribe
//                case ActivityType.Subscribe:
//                    Like objSubscribe = Like.GetSingeltonObjectLike();
//                    newObject = objSubscribe;
//                    CampaignName = objSubscribe.CampaignName;
//                    TemplateId = objSubscribe.TemplateId;
//                    MainModule = Enums.YdMainModule.Subscribe.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.Subscribe.ToString();
//                    break;
//                #endregion Subscribe

//                #region Unsubscribe
//                case ActivityType.Unsubscribe:
//                    Unsubscribe objUnsubscribe = Unsubscribe.GetSingeltonObjectUnsubscribe();
//                    newObject = objUnsubscribe;
//                    CampaignName = objUnsubscribe.CampaignName;
//                    TemplateId = objUnsubscribe.TemplateId;
//                    MainModule = Enums.YdMainModule.Unsubscribe.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.Unsubscribe.ToString();
//                    break;
//                #endregion Unsubscribe

//                #region Message
//                case ActivityType.BroadcastMessages:
//                    Message objMessage = Message.GetSingeltonObjectMessage();
//                    newObject = objMessage;
//                    CampaignName = objMessage.CampaignName;
//                    TemplateId = objMessage.TemplateId;
//                    MainModule = Enums.YdMainModule.Message.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.BroadcastMessages.ToString();
//                    break;
//                #endregion Message

//                #region ViewIncreaser
//                case ActivityType.ViewIncreaser:
//                    ViewIncreaser objViewIncreaser = ViewIncreaser.GetSingeltonObjectViewIncreaser();
//                    newObject = objViewIncreaser;
//                    CampaignName = objViewIncreaser.CampaignName;
//                    TemplateId = objViewIncreaser.TemplateId;
//                    MainModule = Enums.YdMainModule.ViewIncreaser.ToString();
//                    MainModule = "Engage";
//                    SubModule = ActivityType.ViewIncreaser.ToString();
//                    break;
//                #endregion ViewIncreaser

//                #region PostScraper
//                case ActivityType.PostScraper:
//                    PostScraper objPostScraper = PostScraper.GetSingeltonObjectPostScraper();
//                    newObject = objPostScraper;
//                    CampaignName = objPostScraper.CampaignName;
//                    TemplateId = objPostScraper.TemplateId;
//                    MainModule = Enums.YdMainModule.PostScraper.ToString();
//                    MainModule = "Scraper";
//                    SubModule = ActivityType.PostScraper.ToString();
//                    break;
//                #endregion Postscraper

//                #region UserScraper

//                case ActivityType.UserScraper:
//                    UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
//                    newObject = objUserScraper;
//                    CampaignName = objUserScraper.CampaignName;
//                    TemplateId = objUserScraper.TemplateId;
//                    MainModule = Enums.YdMainModule.UserScraper.ToString();
//                    MainModule = "Scraper";
//                    SubModule = ActivityType.UserScraper.ToString();
//                    break;

//                #endregion UserScraper

//                #region ChannelScraper

//                case ActivityType.ChannelScraper:
//                    ChannelScraper objChannelScraper = ChannelScraper.GetSingeltonObjectChannelScraper();
//                    newObject = objChannelScraper;
//                    CampaignName = objChannelScraper.CampaignName;
//                    TemplateId = objChannelScraper.TemplateId;
//                    MainModule = Enums.YdMainModule.ChannelScraper.ToString();
//                    MainModule = "Scraper";
//                    SubModule = ActivityType.ChannelScraper.ToString();
//                    break;

//                    #endregion ChannelScraper

//            }

//            #endregion Update CampaignName,TemplateId,MainModule and SubModule for Respective Module

//            var campaignDetails = new CampaignDetails()
//            {
//                CampaignName = CampaignName,
//                MainModule = MainModule,
//                SubModule = SubModule,
//                SocialNetworks = SocialNetworks.Youtube,
//                SelectedAccountList = listSelectedAccounts,
//                TemplateId = TemplateId,
//                CreationDate = DateTimeUtilities.GetEpochTime(),
//                Status = "Active",
//                LastEditedDate = DateTimeUtilities.GetEpochTime(),
//            };

//            var campaignList = CampaignsFileManager.Get();

//            // If campaign with such name already exists
//            if (campaignList != null && campaignList.Any(x => x.CampaignName == CampaignName))
//            {
//                string warningMessege = "This account is already running with " + moduleType + " configuration from another campaign. Saving this settings will override previous settings and remove this account from the campaign.\r\nWould you still like to proceed?";

//                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(newObject, "Warning",
//                        warningMessege, MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton());

//                if (dialogResult == MessageDialogResult.Negative)
//                    return;

//                // Update campaign
//                foreach (var campaign in campaignList)
//                {
//                    if (campaign.CampaignName == CampaignName)
//                    {
//                        campaign.CampaignName = CampaignName;
//                        campaign.MainModule = MainModule;
//                        campaign.SubModule = SubModule;
//                        campaign.SocialNetworks = SocialNetworks.Youtube;
//                        campaign.SelectedAccountList = listSelectedAccounts;
//                        campaign.TemplateId = TemplateId;
//                        campaign.CreationDate = DateTimeUtilities.GetEpochTime();
//                        campaign.Status = "Active";
//                        campaign.LastEditedDate = DateTimeUtilities.GetEpochTime();
//                    }
//                }
//            }
//            else
//            {
//                campaignList.Add(campaignDetails);

//                // create new database for campaign
//                //DataBaseHandler.CreateDataBase(campaignDetails.CampaignId, SocialNetworks.Youtube, DatabaseType.CampaignType);
//            }


//            // update Campaign with new campaign
//            CampaignsFileManager.Add(campaignDetails);

//            foreach (var userName in listSelectedAccounts)
//            {
//                DominatorAccountModel dominatorAccount = AccountsFileManager.GetAccount(userName,SocialNetworks.Youtube);
//            //    DominatorScheduler.ScheduleTodayJobs(dominatorAccount, SocialNetworks.Youtube, moduleType);
//            }


//#if DEBUG
//            var testResult = CampaignsFileManager.Get();
//#endif
//        }

//        #region need to work on this method
//        //        public static void SaveSettingFromGrowthMode(Object moduleToSave, string UserName, string moduleType)
//        //        {
//        //            string TemplateId = string.Empty;
//        //            // Getting details of account
//        //            var accounts = AccountsFileManager.GetAll();

//        //            // serializing account detail to AccountDetails bin file
//        //            foreach (var account in accounts)
//        //            {
//        //                if (account.AccountBaseModel.UserName == UserName)
//        //                {

//        //                    #region if account created from normal mode functionality

//        //                    if (account.IsCretedFromNormalMode)
//        //                    {
//        //                        account.IsCretedFromNormalMode = false;

//        //                        //Getting details of account having the user name  as selected account
//        //                        var SelectedAccountDetails = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == UserName);

//        //                        TemplateId = SelectedAccountDetails.ActivityManager.LstModuleConfiguration
//        //                           .FirstOrDefault(y => y.ActivityType.ToString() == moduleType.ToString()).TemplateId;
//        //                        CampaignsFileManager.DeleteSelectedAccount(TemplateId, UserName);


//        //                        #region Add new template

//        //                        AddNewTemplate(moduleToSave, UserName, moduleType, account);

//        //                        #endregion

//        //                    }
//        //                    #endregion
//        //                    else
//        //                    {
//        //                        if (string.IsNullOrEmpty(account.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType.ToString() == moduleType).TemplateId))
//        //                        {
//        //                            #region Add new template

//        //                            AddNewTemplate(moduleToSave, UserName, moduleType, account);

//        //                            #endregion
//        //                        }
//        //                        else
//        //                        {
//        //                            #region Updating existing template

//        //                            var tid = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType.ToString() == moduleType).TemplateId;
//        //                            TemplatesFileManager.UpdateActivitySettings(tid,
//        //                                JsonConvert.SerializeObject(moduleToSave));


//        //                            #endregion
//        //                        }
//        //                    }
//        //                }
//        //            }




//        //#if DEBUG
//        //            var xxx = TemplatesFileManager.Get();
//        //#endif
//        //            DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!", MessageDialogStyle.Affirmative);
//        //        }

//        //        private static void AddNewTemplate(object moduleToSave, string UserName, string moduleType, DominatorAccountModel account)
//        //        {
//        //            var objTemplateModel = new TemplateModel();
//        //            account.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType.ToString() == moduleType).TemplateId =
//        //                objTemplateModel.SaveTemplate(moduleToSave,
//        //               moduleType, SocialNetworks.Youtube,
//        //               UserName + "_" + moduleType + "_Template");
//        //        } 
//        #endregion
//    }
    public static class CampaignHelper
    {

        /// <summary>
        /// Callback on Edit/Duplicate campaign from Campaigns screen
        /// </summary>
        /// <param name="templateDetails"></param>
        /// <param name="campaignDetails"></param>
        /// <param name="IsEditCampaignName"></param>
        /// <param name="CancelEditVisibility"></param>
        /// <param name="CampaignButtonContent"></param>
        /// <param name="TemplateID"></param>
       
        public static void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails, bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);
                switch (ActivityType)
                {
                    case ActivityType.Like:
                        Like objLike = Like.GetSingeltonObjectLike();
                        objLike.IsEditCampaignName = IsEditCampaignName;
                        objLike.CancelEditVisibility = CancelEditVisibility;
                        objLike.TemplateId = TemplateID;
                        objLike.CampaignName = campaignDetails.CampaignName;
                        objLike.CampaignButtonContent = CampaignButtonContent;
                        objLike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objLike.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objLike.ObjViewModel.likeModel = JsonConvert.DeserializeObject<LikeModel>(templateDetails.ActivitySettings);
                        objLike.MainGrid.DataContext = objLike.ObjViewModel.likeModel;
                        TabSwitcher.ChangeTabIndex(1, 0);
                        break;
                    case ActivityType.Unlike:
                        Dislike objDislike = Dislike.GetSingeltonObjectDislike();
                        objDislike.IsEditCampaignName = IsEditCampaignName;
                        objDislike.CancelEditVisibility = CancelEditVisibility;
                        objDislike.TemplateId = TemplateID;
                        objDislike.CampaignName = campaignDetails.CampaignName;
                        objDislike.CampaignButtonContent = CampaignButtonContent;
                        objDislike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objDislike.DislikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objDislike.ObjViewModel.dislikeModel = JsonConvert.DeserializeObject<DislikeModel>(templateDetails.ActivitySettings);
                        objDislike.MainGrid.DataContext = objDislike.ObjViewModel.dislikeModel;
                        TabSwitcher.ChangeTabIndex(1, 1);
                        break;
                    case ActivityType.Comment:
                        Comment objComment = Comment.GetSingeltonObjectComment();
                        objComment.IsEditCampaignName = IsEditCampaignName;
                        objComment.CancelEditVisibility = CancelEditVisibility;
                        objComment.TemplateId = TemplateID;
                        objComment.CampaignName = campaignDetails.CampaignName;
                        objComment.CampaignButtonContent = CampaignButtonContent;
                        objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objComment.ObjViewModel.commentModel = JsonConvert.DeserializeObject<CommentModel>(templateDetails.ActivitySettings);
                        objComment.MainGrid.DataContext = objComment.ObjViewModel.commentModel;
                        TabSwitcher.ChangeTabIndex(1, 2);
                        break;
                    case ActivityType.LikeComment:
                        LikeComment objLikeComment = LikeComment.GetSingeltonObjectLikeComment();
                        objLikeComment.IsEditCampaignName = IsEditCampaignName;
                        objLikeComment.CancelEditVisibility = CancelEditVisibility;
                        objLikeComment.TemplateId = TemplateID;
                        objLikeComment.CampaignName = campaignDetails.CampaignName;
                        objLikeComment.CampaignButtonContent = CampaignButtonContent;
                        objLikeComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objLikeComment.LikeCommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objLikeComment.ObjViewModel.likeCommentModel = JsonConvert.DeserializeObject<LikeCommentModel>(templateDetails.ActivitySettings);
                        objLikeComment.MainGrid.DataContext = objLikeComment.ObjViewModel.likeCommentModel;
                        TabSwitcher.ChangeTabIndex(1, 3);
                        break;
                    //case ActivityType.LikeToComment:
                    //    LikeToComment objLikeToComment = LikeToComment.GetSingeltonObjectLikeToComment();
                    //    objLikeToComment.IsEditCampaignName = IsEditCampaignName;
                    //    objLikeToComment.CancelEditVisibility = CancelEditVisibility;
                    //    objLikeToComment.TemplateId = TemplateID;
                    //    objLikeToComment.CampaignName = campaignDetails.CampaignName;
                    //    objLikeToComment.CampaignButtonContent = CampaignButtonContent;
                    //    objLikeToComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                    //    objLikeToComment.LikeToCommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                    //    objLikeToComment.ObjViewModel.LikeToCommentModel = JsonConvert.DeserializeObject<LikeToCommentModel>(templateDetails.ActivitySettings);
                    //    objLikeToComment.MainGrid.DataContext = objLikeToComment.ObjViewModel.LikeToCommentModel;
                    //    TabSwitcher.ChangeTabIndex(1, 4);
                    //    break;
                    case ActivityType.Subscribe:
                        Subscribe objSubscribe = Subscribe.GetSingeltonObjectSubscribe();
                        objSubscribe.IsEditCampaignName = IsEditCampaignName;
                        objSubscribe.CancelEditVisibility = CancelEditVisibility;
                        objSubscribe.TemplateId = TemplateID;
                        objSubscribe.CampaignName = campaignDetails.CampaignName;
                        objSubscribe.CampaignButtonContent = CampaignButtonContent;
                        objSubscribe.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objSubscribe.SubscribeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objSubscribe.ObjViewModel.subscribeModel = JsonConvert.DeserializeObject<SubscribeModel>(templateDetails.ActivitySettings);
                        objSubscribe.MainGrid.DataContext = objSubscribe.ObjViewModel.subscribeModel;
                        TabSwitcher.ChangeTabIndex(2, 0);
                        break;
                    case ActivityType.Unsubscribe:
                        Unsubscribe objUnsubscribe = Unsubscribe.GetSingeltonObjectUnsubscribe();
                        objUnsubscribe.IsEditCampaignName = IsEditCampaignName;
                        objUnsubscribe.CancelEditVisibility = CancelEditVisibility;
                        objUnsubscribe.TemplateId = TemplateID;
                        objUnsubscribe.CampaignName = campaignDetails.CampaignName;
                        objUnsubscribe.CampaignButtonContent = CampaignButtonContent;
                        objUnsubscribe.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objUnsubscribe.UnSubscribeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objUnsubscribe.ObjViewModel.UnsubscribeModel = JsonConvert.DeserializeObject<UnsubscribeModel>(templateDetails.ActivitySettings);
                        objUnsubscribe.MainGrid.DataContext = objUnsubscribe.ObjViewModel.UnsubscribeModel;
                        TabSwitcher.ChangeTabIndex(2, 1);
                        break;
                    case ActivityType.BroadcastMessages:
                        Message objMessage = Message.GetSingeltonObjectMessage();
                        objMessage.IsEditCampaignName = IsEditCampaignName;
                        objMessage.CancelEditVisibility = CancelEditVisibility;
                        objMessage.TemplateId = TemplateID;
                        objMessage.CampaignName = campaignDetails.CampaignName;
                        objMessage.CampaignButtonContent = CampaignButtonContent;
                        objMessage.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objMessage.MessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objMessage.ObjViewModel.messageModel = JsonConvert.DeserializeObject<MessageModel>(templateDetails.ActivitySettings);
                        objMessage.MainGrid.DataContext = objMessage.ObjViewModel.messageModel;
                        TabSwitcher.ChangeTabIndex(2, 2);
                        break;
                    case ActivityType.PostScraper:
                        PostScraper objPostScraper = PostScraper.GetSingeltonObjectPostScraper();
                        objPostScraper.IsEditCampaignName = IsEditCampaignName;
                        objPostScraper.CancelEditVisibility = CancelEditVisibility;
                        objPostScraper.TemplateId = TemplateID;
                        objPostScraper.CampaignName = campaignDetails.CampaignName;
                        objPostScraper.CampaignButtonContent = CampaignButtonContent;
                        objPostScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objPostScraper.PostScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objPostScraper.ObjViewModel.postScraperModel = JsonConvert.DeserializeObject<PostScraperModel>(templateDetails.ActivitySettings);
                        objPostScraper.MainGrid.DataContext = objPostScraper.ObjViewModel.postScraperModel;
                        TabSwitcher.ChangeTabIndex(3, 0);
                        break;
                    case ActivityType.ChannelScraper:
                        ChannelScraper objChannelScraper = ChannelScraper.GetSingeltonObjectChannelScraper();
                        objChannelScraper.IsEditCampaignName = IsEditCampaignName;
                        objChannelScraper.CancelEditVisibility = CancelEditVisibility;
                        objChannelScraper.TemplateId = TemplateID;
                        objChannelScraper.CampaignName = campaignDetails.CampaignName;
                        objChannelScraper.CampaignButtonContent = CampaignButtonContent;
                        objChannelScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objChannelScraper.ChannelScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objChannelScraper.ObjViewModel.channelScraperModel = JsonConvert.DeserializeObject<ChannelScraperModel>(templateDetails.ActivitySettings);
                        objChannelScraper.MainGrid.DataContext = objChannelScraper.ObjViewModel.channelScraperModel;
                        TabSwitcher.ChangeTabIndex(3, 1);
                        break;
                    case ActivityType.UserScraper:
                        UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
                        objUserScraper.IsEditCampaignName = IsEditCampaignName;
                        objUserScraper.CancelEditVisibility = CancelEditVisibility;
                        objUserScraper.TemplateId = TemplateID;
                        objUserScraper.CampaignName = campaignDetails.CampaignName;
                        objUserScraper.CampaignButtonContent = CampaignButtonContent;
                        objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objUserScraper.ObjViewModel.userScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);
                        objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel.userScraperModel;
                        TabSwitcher.ChangeTabIndex(3, 2);
                        break;
                    case ActivityType.ViewIncreaser:
                        ViewIncreaser objViewIncreaser = ViewIncreaser.GetSingeltonObjectViewIncreaser();
                        objViewIncreaser.IsEditCampaignName = IsEditCampaignName;
                        objViewIncreaser.CancelEditVisibility = CancelEditVisibility;
                        objViewIncreaser.TemplateId = TemplateID;
                        objViewIncreaser.CampaignName = campaignDetails.CampaignName;
                        objViewIncreaser.CampaignButtonContent = CampaignButtonContent;
                        objViewIncreaser.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objViewIncreaser.ViewIncreaserFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objViewIncreaser.ObjViewModel.viewIncreaserModel = JsonConvert.DeserializeObject<ViewIncreaserModel>(templateDetails.ActivitySettings);
                        objViewIncreaser.MainGrid.DataContext = objViewIncreaser.ObjViewModel.viewIncreaserModel;
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
        static ObservableCollection<InteractedPostsReport> InteractedPostsModel = new ObservableCollection<InteractedPostsReport>();
        static ObservableCollection<InteractedChannelsReport> InteractedChannelsModel = new ObservableCollection<InteractedChannelsReport>();
        static ObservableCollection<InteractedUsersReport> InteractedUsersModel = new ObservableCollection<InteractedUsersReport>();
        static string Header = string.Empty;
        /// <summary>
        /// this method will return header for csv file 
        /// </summary>
        /// <returns></returns>
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

            InteractedPostsModel.Clear();
            switch (campaign.SubModule)
            {

                #region Like
                case "Like":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>().ForEach(
                   report =>
                   {
                       InteractedPostsModel.Add(new InteractedPostsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.Like.ToString(),
                           AccountUsername = report.AccountUsername,
                           ChannelId = report.ChannelId,
                           ChannelName = report.ChannelName,
                           ChannelUserId = report.ChannelUserId,
                           ChannelUserName = report.ChannelUserName,
                           CommentCount = report.CommentCount,
                           CommentsPresent = report.CommentsPresent,
                           CommentToVideo = report.CommentToVideo,
                           DislikeCount = report.DislikeCount,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           LikeCount = report.LikeCount,
                           LikeStatus = report.LikeStatus,
                           PostDescription = report.PostDescription,
                           PublishedDate = report.PublishedDate,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           ReplyToComment = report.ReplyToComment,
                           SubscribeCount = report.SubscribeCount,
                           SubscribeStatus = report.SubscribeStatus,
                           VideoDuration = report.VideoDuration,
                           VideoUrl = report.VideoUrl,
                           ViewsCount = report.ViewsCount,
                       });
                   });
                    #endregion

                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Status", ColumnBindingText = "LikeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Id", ColumnBindingText = "ChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Name", ColumnBindingText = "ChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserId", ColumnBindingText = "ChannelUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserName", ColumnBindingText = "ChannelUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Present", ColumnBindingText = "CommentsPresent" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment To Video", ColumnBindingText = "CommentToVideo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reply To Comment", ColumnBindingText = "ReplyToComment" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Duration", ColumnBindingText = "VideoDuration" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    return InteractedPostsModel.Count;
                    #endregion
                    break;
                #endregion Like

                #region Unlike
                case "Unlike":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>().ForEach(
                   report =>
                   {
                       InteractedPostsModel.Add(new InteractedPostsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.Unlike.ToString(),
                           AccountUsername = report.AccountUsername,
                           ChannelId = report.ChannelId,
                           ChannelName = report.ChannelName,
                           ChannelUserId = report.ChannelUserId,
                           ChannelUserName = report.ChannelUserName,
                           CommentCount = report.CommentCount,
                           CommentsPresent = report.CommentsPresent,
                           CommentToVideo = report.CommentToVideo,
                           DislikeCount = report.DislikeCount,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           LikeCount = report.LikeCount,
                           LikeStatus = report.LikeStatus,
                           PostDescription = report.PostDescription,
                           PublishedDate = report.PublishedDate,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           ReplyToComment = report.ReplyToComment,
                           SubscribeCount = report.SubscribeCount,
                           SubscribeStatus = report.SubscribeStatus,
                           VideoDuration = report.VideoDuration,
                           VideoUrl = report.VideoUrl,
                           ViewsCount = report.ViewsCount,

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Status", ColumnBindingText = "LikeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Id", ColumnBindingText = "ChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Name", ColumnBindingText = "ChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserId", ColumnBindingText = "ChannelUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserName", ColumnBindingText = "ChannelUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Present", ColumnBindingText = "CommentsPresent" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment To Video", ColumnBindingText = "CommentToVideo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reply To Comment", ColumnBindingText = "ReplyToComment" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Duration", ColumnBindingText = "VideoDuration" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion Unlike

                #region Comment
                case "Comment":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>().ForEach(
                   report =>
                   {
                       InteractedPostsModel.Add(new InteractedPostsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.Comment.ToString(),
                           AccountUsername = report.AccountUsername,
                           ChannelId = report.ChannelId,
                           ChannelName = report.ChannelName,
                           ChannelUserId = report.ChannelUserId,
                           ChannelUserName = report.ChannelUserName,
                           CommentCount = report.CommentCount,
                           CommentsPresent = report.CommentsPresent,
                           CommentToVideo = report.CommentToVideo,
                           DislikeCount = report.DislikeCount,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           LikeCount = report.LikeCount,
                           LikeStatus = report.LikeStatus,
                           PostDescription = report.PostDescription,
                           PublishedDate = report.PublishedDate,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           ReplyToComment = report.ReplyToComment,
                           SubscribeCount = report.SubscribeCount,
                           SubscribeStatus = report.SubscribeStatus,
                           VideoDuration = report.VideoDuration,
                           VideoUrl = report.VideoUrl,
                           ViewsCount = report.ViewsCount,

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Status", ColumnBindingText = "LikeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Id", ColumnBindingText = "ChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Name", ColumnBindingText = "ChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserId", ColumnBindingText = "ChannelUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserName", ColumnBindingText = "ChannelUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Present", ColumnBindingText = "CommentsPresent" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment To Video", ColumnBindingText = "CommentToVideo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reply To Comment", ColumnBindingText = "ReplyToComment" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Duration", ColumnBindingText = "VideoDuration" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion Comment

                #region LikeComment
                case "LikeComment":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>().ForEach(
                   report =>
                   {
                       InteractedPostsModel.Add(new InteractedPostsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.LikeComment.ToString(),
                           AccountUsername = report.AccountUsername,
                           ChannelId = report.ChannelId,
                           ChannelName = report.ChannelName,
                           ChannelUserId = report.ChannelUserId,
                           ChannelUserName = report.ChannelUserName,
                           CommentCount = report.CommentCount,
                           CommentsPresent = report.CommentsPresent,
                           CommentToVideo = report.CommentToVideo,
                           DislikeCount = report.DislikeCount,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           LikeCount = report.LikeCount,
                           LikeStatus = report.LikeStatus,
                           PostDescription = report.PostDescription,
                           PublishedDate = report.PublishedDate,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           ReplyToComment = report.ReplyToComment,
                           SubscribeCount = report.SubscribeCount,
                           SubscribeStatus = report.SubscribeStatus,
                           VideoDuration = report.VideoDuration,
                           VideoUrl = report.VideoUrl,
                           ViewsCount = report.ViewsCount,

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "LikeComment Status", ColumnBindingText = "LikeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Id", ColumnBindingText = "ChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Name", ColumnBindingText = "ChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserId", ColumnBindingText = "ChannelUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserName", ColumnBindingText = "ChannelUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Present", ColumnBindingText = "CommentsPresent" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment To Video", ColumnBindingText = "CommentToVideo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reply To Comment", ColumnBindingText = "ReplyToComment" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Duration", ColumnBindingText = "VideoDuration" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion LikeComment

                #region DeleteComment
                case "DeleteComment":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>().ForEach(
                   report =>
                   {
                       InteractedPostsModel.Add(new InteractedPostsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.DeleteComment.ToString(),
                           AccountUsername = report.AccountUsername,
                           ChannelId = report.ChannelId,
                           ChannelName = report.ChannelName,
                           ChannelUserId = report.ChannelUserId,
                           ChannelUserName = report.ChannelUserName,
                           CommentCount = report.CommentCount,
                           CommentsPresent = report.CommentsPresent,
                           CommentToVideo = report.CommentToVideo,
                           DislikeCount = report.DislikeCount,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           LikeCount = report.LikeCount,
                           LikeStatus = report.LikeStatus,
                           PostDescription = report.PostDescription,
                           PublishedDate = report.PublishedDate,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           ReplyToComment = report.ReplyToComment,
                           SubscribeCount = report.SubscribeCount,
                           SubscribeStatus = report.SubscribeStatus,
                           VideoDuration = report.VideoDuration,
                           VideoUrl = report.VideoUrl,
                           ViewsCount = report.ViewsCount,
                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "DeleteComment Status", ColumnBindingText = "LikeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Id", ColumnBindingText = "ChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Name", ColumnBindingText = "ChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserId", ColumnBindingText = "ChannelUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserName", ColumnBindingText = "ChannelUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Present", ColumnBindingText = "CommentsPresent" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment To Video", ColumnBindingText = "CommentToVideo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reply To Comment", ColumnBindingText = "ReplyToComment" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Duration", ColumnBindingText = "VideoDuration" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion DeleteComment

                #region Subscribe
                case "Subscribe":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to SubscribeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedChannels>().ForEach(
                   report =>
                   {
                       InteractedChannelsModel.Add(new InteractedChannelsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.Subscribe.ToString(),
                           AccountUsername = report.AccountUsername,
                           ExternalLinks = report.ExternalLinks,
                           InteractedChannelId = report.InteractedChannelId,
                           InteractedChannelName = report.InteractedChannelName,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           Message = report.MessageToChannelOwner,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           SubscriberCount = report.SubscriberCount,
                           SubscribeStatus = report.SubscribeStatus,
                           ChannelDescription = report.ChannelDescription,
                           ChannelJoinedDate = report.ChannelJoinedDate,
                           ChannelLocation = report.ChannelLocation,
                           ChannelProfilePic = report.ChannelProfilePic,
                           ChannelUrl = report.ChannelUrl,
                           VideosCount = report.VideosCount,
                           ViewsCount = report.ViewsCount,

                           //ID = report.Id,
                           //VideoId = report.VideoId,
                           //VideoDuration = report.VideoDuration

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedChannelsModel);
                    #region Generate Reports column with data
                    
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Status", ColumnBindingText = "SubscribeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Account Username", ColumnBindingText = "AccountUsername" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "External Links", ColumnBindingText = "ExternalLinks" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelId", ColumnBindingText = "InteractedChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelName", ColumnBindingText = "InteractedChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Message To ChannelOwner", ColumnBindingText = "MessageToChannelOwner" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscriber Count", ColumnBindingText = "SubscriberCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Description", ColumnBindingText = "ChannelDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel JoinedDate", ColumnBindingText = "ChannelJoinedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Location", ColumnBindingText = "ChannelLocation" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel ProfilePic", ColumnBindingText = "ChannelProfilePic" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Url", ColumnBindingText = "ChannelUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Videos Count", ColumnBindingText = "VideosCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    #endregion
                    break;
                #endregion Subscribe

                #region Unsubscribe
                case "Unsubscribe":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to SubscribeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedChannels>().ForEach(
                   report =>
                   {
                       InteractedChannelsModel.Add(new InteractedChannelsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.Unsubscribe.ToString(),
                           AccountUsername = report.AccountUsername,
                           ExternalLinks = report.ExternalLinks,
                           InteractedChannelId = report.InteractedChannelId,
                           InteractedChannelName = report.InteractedChannelName,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           Message = report.MessageToChannelOwner,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           SubscriberCount = report.SubscriberCount,
                           SubscribeStatus = report.SubscribeStatus,
                           ChannelDescription = report.ChannelDescription,
                           ChannelJoinedDate = report.ChannelJoinedDate,
                           ChannelLocation = report.ChannelLocation,
                           ChannelProfilePic = report.ChannelProfilePic,
                           ChannelUrl = report.ChannelUrl,
                           VideosCount = report.VideosCount,
                           ViewsCount = report.ViewsCount,

                           //ID = report.Id,
                           //VideoId = report.VideoId,
                           //VideoDuration = report.VideoDuration

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedChannelsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Status", ColumnBindingText = "SubscribeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Account Username", ColumnBindingText = "AccountUsername" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "External Links", ColumnBindingText = "ExternalLinks" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelId", ColumnBindingText = "InteractedChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelName", ColumnBindingText = "InteractedChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Message To ChannelOwner", ColumnBindingText = "MessageToChannelOwner" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscriber Count", ColumnBindingText = "SubscriberCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Description", ColumnBindingText = "ChannelDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel JoinedDate", ColumnBindingText = "ChannelJoinedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Location", ColumnBindingText = "ChannelLocation" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel ProfilePic", ColumnBindingText = "ChannelProfilePic" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Url", ColumnBindingText = "ChannelUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Videos Count", ColumnBindingText = "VideosCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion Unsubscribe

                #region Message
                case "Message":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to SubscribeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedChannels>().ForEach(
                   report =>
                   {
                       InteractedChannelsModel.Add(new InteractedChannelsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.BroadcastMessages.ToString(),
                           AccountUsername = report.AccountUsername,
                           ExternalLinks = report.ExternalLinks,
                           InteractedChannelId = report.InteractedChannelId,
                           InteractedChannelName = report.InteractedChannelName,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           Message = report.MessageToChannelOwner,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           SubscriberCount = report.SubscriberCount,
                           SubscribeStatus = report.SubscribeStatus,
                           ChannelDescription = report.ChannelDescription,
                           ChannelJoinedDate = report.ChannelJoinedDate,
                           ChannelLocation = report.ChannelLocation,
                           ChannelProfilePic = report.ChannelProfilePic,
                           ChannelUrl = report.ChannelUrl,
                           VideosCount = report.VideosCount,
                           ViewsCount = report.ViewsCount,

                           //ID = report.Id,
                           //VideoId = report.VideoId,
                           //VideoDuration = report.VideoDuration

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedChannelsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Status", ColumnBindingText = "SubscribeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Account Username", ColumnBindingText = "AccountUsername" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "External Links", ColumnBindingText = "ExternalLinks" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelId", ColumnBindingText = "InteractedChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted ChannelName", ColumnBindingText = "InteractedChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Message To ChannelOwner", ColumnBindingText = "MessageToChannelOwner" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscriber Count", ColumnBindingText = "SubscriberCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Description", ColumnBindingText = "ChannelDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel JoinedDate", ColumnBindingText = "ChannelJoinedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Location", ColumnBindingText = "ChannelLocation" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel ProfilePic", ColumnBindingText = "ChannelProfilePic" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Url", ColumnBindingText = "ChannelUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Videos Count", ColumnBindingText = "VideosCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion Message

                #region PostScraper
                case "PostScraper":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to LikeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>().ForEach(
                   report =>
                   {
                       InteractedPostsModel.Add(new InteractedPostsReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.PostScraper.ToString(),
                           AccountUsername = report.AccountUsername,
                           ChannelId = report.ChannelId,
                           ChannelName = report.ChannelName,
                           ChannelUserId = report.ChannelUserId,
                           ChannelUserName = report.ChannelUserName,
                           CommentCount = report.CommentCount,
                           CommentsPresent = report.CommentsPresent,
                           CommentToVideo = report.CommentToVideo,
                           DislikeCount = report.DislikeCount,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           LikeCount = report.LikeCount,
                           LikeStatus = report.LikeStatus,
                           PostDescription = report.PostDescription,
                           PublishedDate = report.PublishedDate,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           ReplyToComment = report.ReplyToComment,
                           SubscribeCount = report.SubscribeCount,
                           SubscribeStatus = report.SubscribeStatus,
                           VideoDuration = report.VideoDuration,
                           VideoUrl = report.VideoUrl,
                           ViewsCount = report.ViewsCount,

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostsModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "PostScraper Status", ColumnBindingText = "LikeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Id", ColumnBindingText = "ChannelId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel Name", ColumnBindingText = "ChannelName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserId", ColumnBindingText = "ChannelUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Channel UserName", ColumnBindingText = "ChannelUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment Count", ColumnBindingText = "CommentCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comments Present", ColumnBindingText = "CommentsPresent" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Comment To Video", ColumnBindingText = "CommentToVideo" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Dislike Count", ColumnBindingText = "DislikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Like Count", ColumnBindingText = "LikeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Post Description", ColumnBindingText = "PostDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Published Date", ColumnBindingText = "PublishedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Reply To Comment", ColumnBindingText = "ReplyToComment" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscribe Count", ColumnBindingText = "SubscribeCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Duration", ColumnBindingText = "VideoDuration" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Video Url", ColumnBindingText = "VideoUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                #endregion PostScraper

                #region UserScraper
                case "UserScraper":
                    //Header = "AccountName,QueryType,Query,VideoId,Date";

                    #region get data from InteractedUsers table and add to SubscribeReportModel
                    dataBase.Get<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedUsers>().ForEach(
                   report =>
                   {
                       InteractedUsersModel.Add(new InteractedUsersReport()
                       {
                           Id = report.Id,
                           ActivityType = ActivityType.UserScraper.ToString(),
                           AccountUsername = report.AccountUsername,
                           ExternalLinks = report.ExternalLinks,
                           InteractedUserId = report.InteractedUserId,
                           InteractedUserName = report.InteractedUserName,
                           InteractionTimeStamp = report.InteractionTimeStamp,
                           MessageToChannelOwner = report.MessageToChannelOwner,
                           QueryType = report.QueryType,
                           QueryValue = report.QueryValue,
                           SubscriberCount = report.SubscriberCount,
                           SubscribeStatus = report.SubscribeStatus,
                           UserDescription = report.UserDescription,
                           UserJoinedDate = report.UserJoinedDate,
                           UserLocation = report.UserLocation,
                           UserProfilePic = report.UserProfilePic,
                           UserUrl = report.UserUrl,
                           VideosCount = report.VideosCount,
                           ViewsCount = report.ViewsCount,

                           //ID = report.Id,
                           //VideoId = report.VideoId,
                           //VideoDuration = report.VideoDuration

                       });
                   });
                    #endregion

                    objReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);
                    #region Generate Reports column with data

                    //campaign.SelectedAccountList.ToList().ForEach(x =>
                    //{
                        objReports.ReportModel.GridViewColumn =
                        new ObservableCollection<GridViewColumnDescriptor>
                        {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Id", ColumnBindingText = "Id" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "UserScraper Status", ColumnBindingText = "SubscribeStatus" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Account Username", ColumnBindingText = "AccountUsername" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "External Links", ColumnBindingText = "ExternalLinks" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted UserId", ColumnBindingText = "InteractedUserId" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interacted UserName", ColumnBindingText = "InteractedUserName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Interaction TimeStamp", ColumnBindingText = "InteractionTimeStamp" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Message To ChannelOwner", ColumnBindingText = "MessageToChannelOwner" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Subscriber Count", ColumnBindingText = "SubscriberCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "User Description", ColumnBindingText = "UserDescription" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "User JoinedDate", ColumnBindingText = "UserJoinedDate" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "User Location", ColumnBindingText = "UserLocation" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "User ProfilePic", ColumnBindingText = "UserProfilePic" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "User Url", ColumnBindingText = "UserUrl" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Videos Count", ColumnBindingText = "VideosCount" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Views Count", ColumnBindingText = "ViewsCount" },
                        };
                    //});
                    #endregion
                    break;
                    #endregion UserScraper

            }
            return objReports.ReportModel.ReportCollection.Cast<object>().Count();
        }

        internal static ObservableCollection<QueryInfo> GetSavedQuery(string ModuleType, string ActivitySettings)
        {
            ObservableCollection<QueryInfo> lstSavedQuery = null;

            #region getting list of Saved Query according to Module type

            switch (ModuleType)
            {
                case "Like":
                    lstSavedQuery = JsonConvert.DeserializeObject<LikeModel>(ActivitySettings).SavedQueries;
                    break;
                    break;
                case "Comment":

                    break;
                case "Repost":

                    break;
                case "DownloadScraper":

                    break;
                case "UserScraper":

                    break;
            }

            #endregion

            return lstSavedQuery;
        }

        internal static void ExportReports(string ModuleType, string filename)
        {
            switch (ModuleType)
            {
                case "Follow":
                    InteractedUsersModel.ToList().ForEach(report =>
                    {
                        try
                        {
                            var csvData = report.AccountUsername + "," + report.QueryType + "," + report.QueryValue + "," + report.InteractedUserName + "," + report.InteractionTimeStamp;

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
                case "Unfollow":

                    break;
                case "Like":

                    break;
                case "Comment":

                    break;
                case "Repost":

                    break;
                case "DownloadScraper":

                    break;
                case "UserScraper":

                    break;
            }
        }
    }
}
