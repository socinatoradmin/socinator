using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using Unity;

namespace PinDominatorCore.PDLibrary.Processors
{
    public abstract class BasePinterestProcessor : IQueryProcessor
    {
        private static ConcurrentDictionary<string, object> _lockObjects = new ConcurrentDictionary<string, object>();
        protected readonly IDbCampaignService CampaignService;
        protected readonly IDbAccountService DbAccountService;
        protected readonly IDbGlobalService DbGlobalService;
        protected readonly IPdJobProcess JobProcess;
        protected readonly IPinFunction PinFunction;
        protected IPdBrowserManager BrowserManager;
        protected ModuleSetting ModuleSetting;
        protected TemplateModel TemplateModel;
        protected ITemplatesFileManager TemplatesFileManager;
        protected ISoftwareSettingsFileManager SoftWareSettingsFileManager;
        protected List<Func<PinterestPin, bool>> FilterPinActionList { get; set; } = new List<Func<PinterestPin, bool>>();
        protected List<Func<PinterestUser, bool>> FilterUserActionList { get; set; } = new List<Func<PinterestUser, bool>>();

        protected BasePinterestProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct)
        {
            IAccountScopeFactory accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            BrowserManager = jobProcess.BrowserManager;
            SoftWareSettingsFileManager = accountScopeFactory[jobProcess.AccountId].Resolve<ISoftwareSettingsFileManager>();
            JobProcess = jobProcess;
            DbGlobalService = globalService;
            DbAccountService = jobProcess.DbAccountService;
            CampaignService = campaignService;
            PinFunction = objPinFunct;
            ModuleSetting = jobProcess.ModuleSetting;
            TemplatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            if (!string.IsNullOrEmpty(JobProcess?.TemplateId))
                TemplateModel = TemplatesFileManager.GetTemplateById(JobProcess.TemplateId);
            SetActivatedPinFilter();
            SetActivatedUserFilter();
        }

        protected ActivityType ActivityType => JobProcess.ActivityType;

        public void Start(QueryInfo queryInfo)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var jobProcessResult = new JobProcessResult();
                if (JobProcess.ActivityType != ActivityType.AcceptBoardInvitation && JobProcess.ActivityType != ActivityType.Unfollow
                    && JobProcess.ActivityType != ActivityType.FollowBack && JobProcess.ActivityType != ActivityType.CreateBoard
                    && JobProcess.ActivityType != ActivityType.AutoReplyToNewMessage && JobProcess.ActivityType != ActivityType.SendMessageToFollower
                    && JobProcess.ActivityType != ActivityType.DeletePin && JobProcess.ActivityType != ActivityType.EditPin
                    && JobProcess.ActivityType != ActivityType.SendBoardInvitation)
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                           JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                           $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");

                Process(queryInfo, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        protected void SetActivatedUserFilter()
        {
            try
            {
                var userFilt = new PdFilters.UserFilterFunctions(ModuleSetting);

                if (ModuleSetting.UserFilterModel.IgnoreNoProfilePicUsers)
                    FilterUserActionList.Add(userFilt.IsAnonymousProfilePicture);

                if (ModuleSetting.UserFilterModel.FilterFollowersCount)
                    FilterUserActionList.Add(userFilt.IsFollowersCountInRange);

                if (ModuleSetting.UserFilterModel.FilterFollowingsCount)
                    FilterUserActionList.Add(userFilt.IsFollowingsCountInRange);

                if (ModuleSetting.UserFilterModel.FilterMinimumFollowRatio)
                    FilterUserActionList.Add(userFilt.CheckForMinimumFollowRatio);

                if (ModuleSetting.UserFilterModel.FilterMaximumFollowRatio)
                    FilterUserActionList.Add(userFilt.CheckForMaximumFollowRatio);

                if (ModuleSetting.UserFilterModel.FilterSpecificFollowRatio)
                    FilterUserActionList.Add(userFilt.CheckFollowRatioInSpecificRange);

                if (ModuleSetting.UserFilterModel.FilterPostCounts)
                    FilterUserActionList.Add(userFilt.IsPinCountInRange);

                if (ModuleSetting.UserFilterModel.UserHasInvalidWord)
                    FilterUserActionList.Add(userFilt.IsBioContainRestrictedWords);

                if (ModuleSetting.UserFilterModel.FilterMinimumCharacterInBio)
                    FilterUserActionList.Add(userFilt.FilterByBioCharacterLength);

                if (ModuleSetting.UserFilterModel.IgnoreNonEnglishUser)
                    FilterUserActionList.Add(userFilt.IsNonEnglishUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool FilterUserApply(PinterestUser pinterestUser, int numberOfScrapedResults)
        {
            var filtered = false;

            if (pinterestUser == null)
                return true;

            if (FilterUserActionList.Count > 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    String.Format("LangKeyFilteringUser".FromResourceDictionary(), pinterestUser.Username));

                foreach (var filterMethod in FilterUserActionList)
                    try
                    {
                        if (filterMethod(pinterestUser))
                        {
                            filtered = true;
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Filter Not Matched");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }

            return filtered;
        }

        public bool FilterPinApply(PinterestPin pinterestPin, int numberOfScrapedResults)
        {
            var filtered = false;

            if (pinterestPin == null)
                return true;

            if (FilterPinActionList.Count > 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    String.Format("LangKeyFilteringPin".FromResourceDictionary(), pinterestPin.PinId));

                foreach (var filterMethod in FilterPinActionList)
                    try
                    {
                        if (filterMethod(pinterestPin))
                        {
                            filtered = true;
                            if (filterMethod.Method.Name.Equals("IsImagePostOverAged"))
                                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Filter Not Matched With Post Age - {pinterestPin.PublishDate}");
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Filter Not Matched");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        return true;
                    }
            }

            return filtered;
        }
        
        protected void SetActivatedPinFilter()
        {
            try
            {
                var pinFilt = new PdFilters.PinFilterFunctions(ModuleSetting);

                if (ModuleSetting.PostFilterModel.FilterPostAge)
                    FilterPinActionList.Add(pinFilt.IsImagePostOverAged);
                if (ModuleSetting.PostFilterModel.FilterComments)
                    FilterPinActionList.Add(pinFilt.IsCommentsCountInRange);

                if (ModuleSetting.PostFilterModel.FilterTried)
                    FilterPinActionList.Add(pinFilt.IsTryCountInRange);

                if (ModuleSetting.PostFilterModel.PostCategory.FilterPostCategory)
                    FilterPinActionList.Add(pinFilt.IsPostTypeIgnored);

                if (ModuleSetting.PostFilterModel.FilterRestrictedPostCaptionList)
                    FilterPinActionList.Add(pinFilt.IsRestrictedPost);

                if (ModuleSetting.PostFilterModel.FilterAcceptedPostCaptionList)
                    FilterPinActionList.Add(pinFilt.IsAcceptedPost);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected List<string> FilterBlackListUser(TemplateModel templateModel, List<string> list)
        {
            try
            {
                FollowerModel followerModel = null;
                AutoReplyToNewMessageModel autoReplyToNewMessageModel = null;
                BroadcastMessagesModel broadcastMessagesModel = null;
                SendMessageToNewFollowersModel sendMessageToNewFollowersModel = null;
                TryModel tryModel = null;
                CommentModel commentModel = null;

                switch (ActivityType)
                {
                    case ActivityType.Follow:
                        followerModel = JsonConvert.DeserializeObject<FollowerModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.AutoReplyToNewMessage:
                        autoReplyToNewMessageModel =
                            JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.BroadcastMessages:
                        broadcastMessagesModel =
                            JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.SendMessageToFollower:
                        sendMessageToNewFollowersModel =
                            JsonConvert.DeserializeObject<SendMessageToNewFollowersModel>(
                                templateModel.ActivitySettings);
                        break;

                    case ActivityType.Try:
                        tryModel = JsonConvert.DeserializeObject<TryModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.Comment:
                        commentModel = JsonConvert.DeserializeObject<CommentModel>(templateModel.ActivitySettings);
                        break;
                }

                var lstBlackListUserGlobal = DbGlobalService.GetBlackListedUser();
                var lstBlackListUserPrivate = DbAccountService.GetPrivateBlacklist().ToList();

                if (followerModel != null && followerModel.IsChkSkipBlackListedUser ||
                    broadcastMessagesModel != null && broadcastMessagesModel.IsChkSkipBlackListedUser ||
                    autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkSkipBlackListedUser ||
                    sendMessageToNewFollowersModel != null && sendMessageToNewFollowersModel.IsChkSkipBlackListedUser ||
                    tryModel != null && tryModel.IsChkSkipBlackListedUser ||
                    commentModel != null && commentModel.IsChkSkipBlackListedUser)
                {
                    if (followerModel != null && followerModel.IsChkGroupBlackList ||
                        broadcastMessagesModel != null && broadcastMessagesModel.IsChkGroupBlackList ||
                        autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkGroupBlackList ||
                        sendMessageToNewFollowersModel != null && sendMessageToNewFollowersModel.IsChkGroupBlackList ||
                        tryModel != null && tryModel.IsChkGroupBlackList ||
                        commentModel != null && commentModel.IsChkGroupBlackList)
                        lstBlackListUserGlobal.ForEach(x =>
                        {
                            if (list.Contains(x.UserName))
                                list.Remove(x.UserName);
                        });
                    if (followerModel != null && followerModel.IsChkPrivateBlackList ||
                        broadcastMessagesModel != null && broadcastMessagesModel.IsChkPrivateBlackList ||
                        autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkPrivateBlackList ||
                        sendMessageToNewFollowersModel != null &&
                        sendMessageToNewFollowersModel.IsChkPrivateBlackList ||
                        tryModel != null && tryModel.IsChkPrivateBlackList ||
                        commentModel != null && commentModel.IsChkPrivateBlackList)
                        lstBlackListUserPrivate.ForEach(x =>
                        {
                            if (list.Contains(x.UserName))
                                list.Remove(x.UserName);
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        protected List<Friendships> FilterBlackListUser(TemplateModel templateModel, List<Friendships> list)
        {
            try
            {
                FollowerModel followerModel = null;
                AutoReplyToNewMessageModel autoReplyToNewMessageModel = null;
                BroadcastMessagesModel broadcastMessagesModel = null;
                SendMessageToNewFollowersModel sendMessageToNewFollowersModel = null;
                TryModel tryModel = null;
                CommentModel commentModel = null;

                switch (ActivityType)
                {
                    case ActivityType.Follow:
                        followerModel = JsonConvert.DeserializeObject<FollowerModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.AutoReplyToNewMessage:
                        autoReplyToNewMessageModel =
                            JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.BroadcastMessages:
                        broadcastMessagesModel =
                            JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.SendMessageToFollower:
                        sendMessageToNewFollowersModel =
                            JsonConvert.DeserializeObject<SendMessageToNewFollowersModel>(
                                templateModel.ActivitySettings);
                        break;

                    case ActivityType.Try:
                        tryModel = JsonConvert.DeserializeObject<TryModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.Comment:
                        commentModel = JsonConvert.DeserializeObject<CommentModel>(templateModel.ActivitySettings);
                        break;
                }

                var lstBlackListUserGlobal = DbGlobalService.GetBlackListedUser();
                var lstBlackListUserPrivate = DbAccountService.GetPrivateBlacklist().ToList();

                if (followerModel != null && followerModel.IsChkSkipBlackListedUser ||
                    broadcastMessagesModel != null && broadcastMessagesModel.IsChkSkipBlackListedUser ||
                    autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkSkipBlackListedUser ||
                    sendMessageToNewFollowersModel != null && sendMessageToNewFollowersModel.IsChkSkipBlackListedUser ||
                    tryModel != null && tryModel.IsChkSkipBlackListedUser ||
                    commentModel != null && commentModel.IsChkSkipBlackListedUser)
                {
                    if (followerModel != null && followerModel.IsChkGroupBlackList ||
                        broadcastMessagesModel != null && broadcastMessagesModel.IsChkGroupBlackList ||
                        autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkGroupBlackList ||
                        sendMessageToNewFollowersModel != null && sendMessageToNewFollowersModel.IsChkGroupBlackList ||
                        tryModel != null && tryModel.IsChkGroupBlackList ||
                        commentModel != null && commentModel.IsChkGroupBlackList)
                        lstBlackListUserGlobal.ForEach(x =>
                        {
                            if (list.Any(y => y.Username.Contains(x.UserName)))
                                list.Remove(list.FirstOrDefault(y => y.Username.Contains(x.UserName)));
                        });
                    if (followerModel != null && followerModel.IsChkPrivateBlackList ||
                        broadcastMessagesModel != null && broadcastMessagesModel.IsChkPrivateBlackList ||
                        autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkPrivateBlackList ||
                        sendMessageToNewFollowersModel != null &&
                        sendMessageToNewFollowersModel.IsChkPrivateBlackList ||
                        tryModel != null && tryModel.IsChkPrivateBlackList ||
                        commentModel != null && commentModel.IsChkPrivateBlackList)
                        lstBlackListUserPrivate.ForEach(x =>
                        {
                            if (list.Any(y => y.Username.Contains(x.UserName)))
                                list.Remove(list.FirstOrDefault(y => y.Username.Contains(x.UserName)));
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        protected List<PinterestUser> FilterBlackListUser(TemplateModel templateModel, List<PinterestUser> list)
        {
            try
            {
                FollowerModel followerModel = null;
                FollowBackModel followBackModel = null;
                AutoReplyToNewMessageModel autoReplyToNewMessageModel = null;
                BroadcastMessagesModel broadcastMessagesModel = null;
                SendMessageToNewFollowersModel sendMessageToNewFollowersModel = null;

                switch (ActivityType)
                {
                    case ActivityType.Follow:
                        followerModel = JsonConvert.DeserializeObject<FollowerModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.FollowBack:
                        followBackModel =
                            JsonConvert.DeserializeObject<FollowBackModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.AutoReplyToNewMessage:
                        autoReplyToNewMessageModel =
                            JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.BroadcastMessages:
                        broadcastMessagesModel =
                            JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.SendMessageToFollower:
                        sendMessageToNewFollowersModel =
                            JsonConvert.DeserializeObject<SendMessageToNewFollowersModel>(
                                templateModel.ActivitySettings);
                        break;
                }

                if (followerModel != null && followerModel.IsChkSkipBlackListedUser ||
                    broadcastMessagesModel != null && broadcastMessagesModel.IsChkSkipBlackListedUser ||
                    autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkSkipBlackListedUser ||
                    sendMessageToNewFollowersModel != null && sendMessageToNewFollowersModel.IsChkSkipBlackListedUser ||
                    followBackModel != null && followBackModel.IsChkSkipBlackListedUser)
                {
                    if (followerModel != null && followerModel.IsChkGroupBlackList ||
                        broadcastMessagesModel != null && broadcastMessagesModel.IsChkGroupBlackList ||
                        autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkGroupBlackList ||
                        sendMessageToNewFollowersModel != null && sendMessageToNewFollowersModel.IsChkGroupBlackList ||
                        followBackModel != null && followBackModel.IsChkGroupBlackList)
                    {
                        var lstBlackListUserGlobal = DbGlobalService.GetBlackListedUser();
                        var usersCount = list.Count;
                        lstBlackListUserGlobal.ForEach(x =>
                        {
                            if (list.Any(y => y.Username == x.UserName))
                                list.Remove(list.FirstOrDefault(y => x.UserName.Contains(y.Username)));
                        });
                        LogSkippedMessage(usersCount,list.Count,ActivityType, "LangKeySkipedGlobalBlacklistedUsers");
                    }

                    if (followerModel != null && followerModel.IsChkPrivateBlackList ||
                        broadcastMessagesModel != null && broadcastMessagesModel.IsChkPrivateBlackList ||
                        autoReplyToNewMessageModel != null && autoReplyToNewMessageModel.IsChkPrivateBlackList ||
                        sendMessageToNewFollowersModel != null &&
                        sendMessageToNewFollowersModel.IsChkPrivateBlackList ||
                        followBackModel != null && followBackModel.IsChkPrivateBlackList)
                    {
                        var lstBlackListUserPrivate = DbAccountService.GetPrivateBlacklist().ToList();

                        var usersCount = list.Count;
                        lstBlackListUserPrivate.ForEach(x =>
                        {
                            if (list.Any(y => y.Username == x.UserName))
                                list.Remove(list.FirstOrDefault(y => x.UserName.Contains(y.Username)));
                        });
                        LogSkippedMessage(usersCount, list.Count, ActivityType, "LangKeySkipedPrivateBlacklistedUsers");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }
        public (bool, List<PinterestPin>) IsFilteredBlackListed(TemplateModel templateModel,ref List<PinterestPin> pins)
        {
            try
            {
                var list = pins.ToList();
                TryModel tryModel = null;
                CommentModel commentModel = null;
                RePinModel repinModel = null;
                switch (ActivityType)
                {
                    case ActivityType.Try:
                        tryModel = JsonConvert.DeserializeObject<TryModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.Comment:
                        commentModel = JsonConvert.DeserializeObject<CommentModel>(templateModel.ActivitySettings);
                        break;

                    case ActivityType.Repin:
                        repinModel = JsonConvert.DeserializeObject<RePinModel>(templateModel.ActivitySettings);
                        break;
                }

                if (tryModel != null && tryModel.IsChkSkipBlackListedUser ||
                    commentModel != null && commentModel.IsChkSkipBlackListedUser ||
                    repinModel != null && repinModel.IsChkSkipBlackListedUser)
                {
                    if (tryModel != null && tryModel.IsChkGroupBlackList ||
                        commentModel != null && commentModel.IsChkGroupBlackList ||
                        repinModel != null && repinModel.IsChkGroupBlackList)
                    {
                        var lstBlackListUserGlobal = DbGlobalService.GetBlackListedUser();
                        var pinsCount = list.Count;
                        lstBlackListUserGlobal.ForEach(x =>
                        {
                            if (list.Any(y => y.User.Username == x.UserName))
                                list.RemoveAll(y => x.UserName.Contains(y.User.Username));
                        });
                        if (pinsCount - list.Count > 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeySkipedPinsOfGlobalBlacklistedUsers".FromResourceDictionary(), pinsCount - list.Count));
                            return (true, list);
                        }
                    }

                    if (tryModel != null && tryModel.IsChkPrivateBlackList ||
                        commentModel != null && commentModel.IsChkPrivateBlackList ||
                        repinModel != null && repinModel.IsChkPrivateBlackList)
                    {
                        var lstBlackListUserPrivate = DbAccountService.GetPrivateBlacklist().ToList();

                        var pinsCount = list.Count;
                        lstBlackListUserPrivate.ForEach(x =>
                        {
                            if (list.Any(y => y.User.Username == x.UserName))
                                list.Remove(list.FirstOrDefault(y => x.UserName.Contains(y.User.Username)));
                        });
                        if (pinsCount - list.Count > 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeySkipedPinsOfPrivateBlacklistedUsers".FromResourceDictionary(), pinsCount - list.Count));
                            return (true, list);
                        }
                    }
                }
                return (false, list);
            }
            catch {return (false,pins);}
        }
        protected List<PinterestPin> FilterBlackListUser(TemplateModel templateModel, List<PinterestPin> list)
        {
            try
            {
               return IsFilteredBlackListed(templateModel,ref list).Item2;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return list;
        }

        protected List<string> FilterWhiteListUser(TemplateModel templateModel, List<string> list)
        {
            try
            {
                UnfollowerModel unfollowerModel = null;

                if (ActivityType == ActivityType.Unfollow)
                    unfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);


                var lstWhitelistUserGlobal = DbGlobalService.GetWhiteListUser().ToList();
                var lstWhitelistUserPrivate = DbAccountService.GetPrivateWhitelist().ToList();

                if (unfollowerModel != null && unfollowerModel.IsChkSkipWhiteListedUser)
                {
                    if (unfollowerModel != null && unfollowerModel.IsChkUseGroupWhiteList)
                        lstWhitelistUserGlobal.ForEach(x =>
                        {
                            if (list.Contains(x.UserName))
                                list.Remove(x.UserName);
                        });
                    if (unfollowerModel != null && unfollowerModel.IsChkUsePrivateWhiteList)
                        lstWhitelistUserPrivate.ForEach(x =>
                        {
                            if (list.Contains(x.UserName))
                                list.Remove(x.UserName);
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        protected List<PinterestUser> FilterWhiteListUser(TemplateModel templateModel, List<PinterestUser> list)
        {
            try
            {
                UnfollowerModel unfollowerModel = null;

                if (ActivityType == ActivityType.Unfollow)
                    unfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);


                var lstWhitelistUserGlobal = DbGlobalService.GetWhiteListUser().ToList();
                var lstWhitelistUserPrivate = DbAccountService.GetPrivateWhitelist().ToList();

                if (unfollowerModel != null && unfollowerModel.IsChkSkipWhiteListedUser)
                {
                    if (unfollowerModel != null && unfollowerModel.IsChkUseGroupWhiteList)
                        lstWhitelistUserGlobal.ForEach(x =>
                        {
                            if (list.Any(y => y.Username.Contains(x.UserName)))
                                list.Remove(list.FirstOrDefault(y => y.Username.Contains(x.UserName)));
                        });
                    if (unfollowerModel != null && unfollowerModel.IsChkUsePrivateWhiteList)
                        lstWhitelistUserPrivate.ForEach(x =>
                        {
                            if (list.Any(y => y.Username.Contains(x.UserName)))
                                list.Remove(list.FirstOrDefault(y => y.Username.Contains(x.UserName)));
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        protected List<InteractedUsers> FilterWhiteListUser(TemplateModel templateModel, List<InteractedUsers> list)
        {
            try
            {
                UnfollowerModel unfollowerModel = null;

                if (ActivityType == ActivityType.Unfollow)
                    unfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);


                var lstWhitelistUserGlobal = DbGlobalService.GetWhiteListUser().ToList();
                var lstWhitelistUserPrivate = DbAccountService.GetPrivateWhitelist().ToList();

                if (unfollowerModel != null && unfollowerModel.IsChkSkipWhiteListedUser)
                {
                    if (unfollowerModel != null && unfollowerModel.IsChkUseGroupWhiteList)
                        lstWhitelistUserGlobal.ForEach(x =>
                        {
                            if (list.Any(y => y.InteractedUsername.Contains(x.UserName)))
                                list.Remove(list.FirstOrDefault(y => y.InteractedUsername.Contains(x.UserName)));
                        });
                    if (unfollowerModel != null && unfollowerModel.IsChkUsePrivateWhiteList)
                        lstWhitelistUserPrivate.ForEach(x =>
                        {
                            if (list.Any(y => y.InteractedUsername.Contains(x.UserName)))
                                list.Remove(list.FirstOrDefault(y => y.InteractedUsername.Contains(x.UserName)));
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        protected List<string> AlreadyInteractedUser(List<string> listOfInteractedUsers)
        {
            var alreadyUsedInteractedUsers = new List<InteractedUsers>();

            alreadyUsedInteractedUsers =
                DbAccountService.GetInteractedUsers(ActivityType).ToList();
            alreadyUsedInteractedUsers.ForEach(x =>
            {
                if (listOfInteractedUsers.Contains(x.InteractedUsername))
                    listOfInteractedUsers.Remove(x.InteractedUsername);
            });
            return listOfInteractedUsers;
        }

        protected List<PinterestUser> AlreadyInteractedUser(List<PinterestUser> listOfInteractedUsers)
        {
            var alreadyUsedInteractedUsers = new List<InteractedUsers>();
            var usersCount = listOfInteractedUsers.Count;
            alreadyUsedInteractedUsers =
                DbAccountService.GetInteractedUsers(ActivityType).ToList();
            alreadyUsedInteractedUsers.ForEach(x =>
            {
                if (listOfInteractedUsers.Any(y => y.Username == x.InteractedUsername))
                    listOfInteractedUsers.Remove(
                        listOfInteractedUsers.FirstOrDefault(y => y.Username == x.InteractedUsername));
            });
            LogSkippedMessage(usersCount, listOfInteractedUsers.Count,ActivityType, "LangKeySkipedAlreadyInteractedUsers");
            return listOfInteractedUsers;
        }
        protected void LogSkippedMessage(int ActualCount,int filteredCount,ActivityType activityType,string key)
        {
            if (ActualCount - filteredCount > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, activityType,string.Format(key?.FromResourceDictionary(),ActualCount-filteredCount));
        }
        protected List<PinterestUser> AlreadyInteractedUserWithSameQuery(List<PinterestUser> listOfUsers,QueryInfo queryInfo)
        {
            var usersCount = listOfUsers.Count;
            var alreadyUsedInteractedUsers = DbAccountService.GetInteractedUsersWithSameQuery(ActivityType, queryInfo);
            alreadyUsedInteractedUsers.ForEach(x =>
            {
                if (listOfUsers.Any(y => y.Username == x.InteractedUsername))
                    listOfUsers.Remove(listOfUsers.FirstOrDefault(y => y.Username == x.InteractedUsername));
            });
            LogSkippedMessage(usersCount, listOfUsers.Count,ActivityType, "LangKeySkipedAlreadyInteractedUsers");
            return listOfUsers;
        }
        protected List<string> AlreadyUnfollowedUser(List<string> listOfInteractedUsers)
        {
            var alreadyUsedInteractedUsers = new List<UnfollowedUsers>();

            alreadyUsedInteractedUsers = DbAccountService.GetUnfollowedUsers().ToList();
            alreadyUsedInteractedUsers.ForEach(x =>
            {
                if (listOfInteractedUsers.Contains(x.Username))
                    listOfInteractedUsers.Remove(x.Username);
            });
            return listOfInteractedUsers;
        }

        protected List<InteractedUsers> AlreadyUnfollowedUser(List<InteractedUsers> listOfInteractedUsers)
        {
            var alreadyUsedInteractedUsers = new List<UnfollowedUsers>();
            var usersCount = listOfInteractedUsers.Count;

            alreadyUsedInteractedUsers = DbAccountService.GetUnfollowedUsers().ToList();
            alreadyUsedInteractedUsers.ForEach(x =>
            {
                if (listOfInteractedUsers.Any(y => y.InteractedUsername.Contains(x.Username)))
                {
                    listOfInteractedUsers.Remove(
                        listOfInteractedUsers.FirstOrDefault(y => y.InteractedUsername.Contains(x.Username)));
                    DbAccountService.Remove<InteractedUsers>(y => y.InteractedUsername.Contains(x.Username));
                }
            });

            //if (usersCount - listOfInteractedUsers.Count > 0)
            //    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
            //        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
            //        String.Format("LangKeySkipedAlreadyUnfollowedUsers".FromResourceDictionary(), usersCount - listOfInteractedUsers.Count));
            return listOfInteractedUsers;
        }

        protected bool AlreadyInteractedBoard(BoardInfo boardInfo)
        {
            var alreadyUsedInteractedBoards = new List<InteractedBoards>();

            var isActivityDoneBefore = false;

            alreadyUsedInteractedBoards = DbAccountService.GetInteractedBoards(ActivityType).ToList();
            alreadyUsedInteractedBoards.ForEach(x =>
            {
                if (boardInfo.BoardName.Equals(x.BoardName))
                    isActivityDoneBefore = true;
            });
            return isActivityDoneBefore;
        }

        protected bool AlreadyInteractedBoard(QueryInfo queryInfo, string boardUrl)
        {
            var alreadyUsedInteractedBoards = new List<InteractedBoards>();

            var isActivityDoneBefore = false;

            if (ActivityType == ActivityType.AcceptBoardInvitation || ActivityType == ActivityType.SendBoardInvitation)
            {
                alreadyUsedInteractedBoards = DbAccountService.GetInteractedBoards(ActivityType).ToList();
                alreadyUsedInteractedBoards.ForEach(x =>
                {
                    if (boardUrl.Equals(x.BoardUrl))
                        isActivityDoneBefore = true;
                });
            }
            else
            {
                alreadyUsedInteractedBoards =
                    DbAccountService.GetInteractedBoards(ActivityType).ToList();
                alreadyUsedInteractedBoards.ForEach(x =>
                {
                    if (boardUrl.Equals(x.BoardUrl))
                        isActivityDoneBefore = true;
                });
            }

            return isActivityDoneBefore;
        }


        protected bool AlreadyInteractedPin(QueryInfo queryInfo, string pinId, string boardUrl = null)
        {
            var alreadyUsed = new List<InteractedPosts>();
            var isActivityDoneBefore = false;

            if (ActivityType == ActivityType.Repin)
            {
                alreadyUsed = DbAccountService.GetInteractedPostsWithSameQueryAndSameBoards(ActivityType,
                    queryInfo, boardUrl).ToList();
            }
            else
            {
                alreadyUsed = DbAccountService.GetInteractedPostsWithSameQuery(ActivityType, queryInfo).ToList();
            }

            alreadyUsed.ForEach(x =>
            {
                if (pinId.Contains(x.PinId))
                    isActivityDoneBefore = true;
            });
            return isActivityDoneBefore;
        }

        protected int NumberOfTimesRepinOnSameBoard(QueryInfo queryInfo, string pinId, string boardUrl = null)
        {
            var alreadyUsed = new List<InteractedPosts>();
            int isActivityDoneBefore = 0;
            alreadyUsed = DbAccountService.GetInteractedPostsWithSameQueryAndSameBoards(ActivityType,
                queryInfo, boardUrl).ToList();


            alreadyUsed.ForEach(x =>
            {
                if (pinId.Contains(x.PinId))
                    isActivityDoneBefore++;
            });
            return isActivityDoneBefore;
        }

        protected List<string> AlreadyInteractedPin(QueryInfo queryInfo, List<string> listOfInteractedPins)
        {
            var alreadyUsedInteractedUsers = new List<InteractedPosts>();

            alreadyUsedInteractedUsers =
                DbAccountService.GetInteractedPostsWithSameQuery(ActivityType, queryInfo).ToList();
            alreadyUsedInteractedUsers.ForEach(x =>
            {
                if (listOfInteractedPins.Contains(x.PinId))
                    listOfInteractedPins.Remove(x.PinId);
            });
            return listOfInteractedPins;
        }

        protected List<PinterestPin> AlreadyInteractedPin(QueryInfo queryInfo, List<PinterestPin> lstPin)
        {
            var lstInteractedPost = new List<InteractedPosts>();
            var lstPinNotInteracted = new List<PinterestPin>();
            lstPinNotInteracted.AddRange(lstPin);
            try
            {
                lstInteractedPost =
                    DbAccountService.GetInteractedPostsWithSameQuery(ActivityType, queryInfo).ToList();
                lstInteractedPost.ForEach(x =>
                {
                    if (lstPinNotInteracted.Any(y => y.PinId.Contains(x.PinId)))
                        lstPinNotInteracted.Remove(lstPinNotInteracted.FirstOrDefault(y => y.PinId.Contains(x.PinId)));
                });

                if (lstPin.Count - lstPinNotInteracted.Count > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Skipped => {lstPin.Count - lstPinNotInteracted.Count} already interacted pins");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstPinNotInteracted;
        }

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, PinterestPin pin)
        {
            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (ModuleSetting.IschkUniquePostForCampaign)
                {
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Pinterest, $"{JobProcess.CampaignId}.post", pin.PinId);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
                }
                if (ModuleSetting.IschkUniqueUserForCampaign)
                {
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Pinterest, JobProcess.CampaignId, pin.User.Username);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
                }
            }

            if (ModuleSetting.IschkUniqueUserForAccount)
            {
                try
                {
                    if ((DbAccountService.GetInteractedPosts(ActivityType).Where(x => x.Username == pin.User.Username)).Any())
                        return false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink, [NotNull] CampaignDetails campaignDetails)
        {
            if (campaignDetails == null)
                return true;
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            campaignDetails = campaignFileManager.GetCampaignById(JobProcess.CampaignId);

            if (campaignDetails != null)
            {
                try
                {
                    JobProcess.AddedToDb = false;
                    #region Action From Random Percentage Of Accounts
                    if (ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var lockObject = _lockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                        lock (lockObject)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Pinterest,
                                   ConstantVariable.GetCampaignDb);
                            try
                            {
                                decimal count = campaignDetails.SelectedAccountList.Count;
                                var randomMaxAccountToPerform = (int)Math.Round(count * ModuleSetting.PerformActionFromRandomPercentage.GetRandom() / 100);

                                var numberOfAccountsAlreadyPerformedAction = CampaignService.GetAllInteractedPosts().Where(x => x.OperationType == ActivityType.ToString() && x.PinId == postPermalink).ToList();

                                if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction.Count)
                                    return false;

                                AddPendingActivityValueToDb(queryInfo, postPermalink, dbOperation);
                                JobProcess.AddedToDb = true;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }

                    }
                    #endregion

                    #region Delay Between action On SamePost
                    if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    {
                        var lockObject = _lockObjects.GetOrAdd("Lock2" + postPermalink, new object());
                        lock (lockObject)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Pinterest,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                List<int> recentlyPerformedActions;
                                recentlyPerformedActions = CampaignService.GetAllInteractedPosts().
                                    Where(x => x.OperationType == ActivityType.ToString() && x.PinId == postPermalink && (x.Status == "Success" || x.Status == "Working")).
                                    OrderByDescending(x => x.InteractionDate).Select(x => x.InteractionDate)
                                    .Take(1).ToList();

                                if (recentlyPerformedActions.Count > 0)
                                {
                                    var recentlyPerformedTime = recentlyPerformedActions[0];
                                    var delay = ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                                    var time = DateTimeUtilities.GetEpochTime();
                                    var time2 = recentlyPerformedTime + delay;
                                    if (time < time2)
                                    {
                                        Thread.Sleep((time2 - time) * 1000);
                                    }
                                }
                                if (!JobProcess.AddedToDb)
                                    AddWorkingActivityValueToDb(queryInfo, postPermalink, dbOperation);
                                else
                                {
                                    var activityType = ActivityType.ToString();
                                    var interactedPost = dbOperation.GetSingle<DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts>(
                                            x => x.PinId == postPermalink && x.OperationType == activityType &&
                                                 x.SinAccUsername == JobProcess.DominatorAccountModel.AccountBaseModel.UserName && (x.Status == "Pending" || x.Status == "Working"));
                                    interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                                    interactedPost.Status = "Working";
                                    dbOperation.Update(interactedPost);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }

                    }
                    #endregion
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException(@"Cancellation Requested !");
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        protected void AddPendingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            var activityType = ActivityType.ToString();
            dbOperation.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts()
            {
                OperationType = activityType,
                QueryType = queryInfo.QueryType,
                Query = queryInfo.QueryValue,
                SinAccUsername = JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                PinId = postPermalink,
                Status = "Pending"
            });
        }

        protected void AddWorkingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            var activityType = ActivityType.ToString();
            dbOperation.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts()
            {
                OperationType = activityType,
                QueryType = queryInfo.QueryType,
                Query = queryInfo.QueryValue,
                SinAccUsername = JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                PinId = postPermalink,
                Status = "Working"
            });
        }
    }
}