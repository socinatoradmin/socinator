using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor
{
    public abstract class BaseInstagramProcessor : IQueryProcessor
    {
        protected IGdJobProcess JobProcess { get; set; }
        protected IDbAccountService DbAccountService;
        protected IDbCampaignService CampaignService;
        protected IInstaFunction InstaFunction;
        static readonly ConcurrentDictionary<string, object> LockObjects = new ConcurrentDictionary<string, object>();
        public ActivityType ActivityType { get; set; }
        protected CancellationToken Token { get; set; }
        private BlackListWhitelistHandler BlackListWhitelistHandler { get; set; }
        protected DominatorAccountModel DominatorAccountModel { get; }
        protected ModuleSetting ModuleSetting { get; set; }
        protected int VisitedQueryCount { get; set; } = 0;
        protected string QueryType { get; set; }
        protected AccountModel AccountModel { get; set; }
        protected string CampaignId { get; set; }
        protected TemplateModel TemplateModel { get; set; }
        protected CommentModel CommentModel { get; set; }
        protected IGdBrowserManager GdBrowserManager { get; set; }
        protected IDelayService DelayService;
        public IGlobalInteractionDetails GlobalInteractionDetails { get; }
        protected BaseInstagramProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
           IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService,
           IGdBrowserManager gdBrowserManager)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            TemplateModel model = templatesFileManager.GetTemplateById(processScopeModel.TemplateId);
            TemplateModel = model;
            JobProcess = jobProcess;
            DbAccountService = dbAccountService;
            this.CampaignService = campaignService;
            GlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
            DominatorAccountModel = jobProcess.DominatorAccountModel;
            ModuleSetting = jobProcess.ModuleSetting;
            Token = jobProcess.JobCancellationTokenSource.Token;
            ActivityType = jobProcess.ActivityType;
            InstaFunction = jobProcess.loginProcess.InstagramFunctFactory.InstaFunctions ?? jobProcess.instaFunct;//jobProcess.instaFunct;
            GdBrowserManager = gdBrowserManager;
            AccountModel = new AccountModel(jobProcess.DominatorAccountModel);
            CampaignId = jobProcess.campaignId;
            DelayService = _delayService;
        }

        public void Start(QueryInfo queryInfo)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var jobProcessResult = new JobProcessResult();
                if (queryInfo.QueryType != null)
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");

                else
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Searching for { JobProcess.ActivityType}");

                //This delay is for account safety purpose because in same time we are hitting request in powerAdspy also
         //       DelayService.ThreadSleep(TimeSpan.FromSeconds(20));

                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    //var browserManagerFactory = InstanceProvider.GetInstance<IBrowserManagerFactory>();
                    //browserManagerFactory.CheckStatusAsync(DominatorAccountModel, Token);
                    //GdBrowserManager = browserManagerFactory.GdBrowserManager(DominatorAccountModel,Token);
                    GdBrowserManager = InstaFunction.GdBrowserManager;
                }

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    Process(queryInfo, ref jobProcessResult);
                }

                //if (DominatorAccountModel.IsRunProcessThroughBrowser)
                //    GdBrowserManager.CloseBrowser();
            }
            catch (OperationCanceledException)
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    InstaFunction.CloseBrowser();
                throw new OperationCanceledException();
            }
        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        protected List<InstagramPost> FilterWhitelistBlacklistUsersFromFeeds(List<InstagramPost> allFeedDetails)
        {
            try
            {
                var allUsers = allFeedDetails.Select(x => x.User).ToList();

                var usersList = FilterWhitelistBlacklistUsers(allUsers);
                if (usersList != null)
                    return allFeedDetails.Where(x => usersList.Contains(x.User)).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return allFeedDetails;
        }

        protected List<InstagramUser> FilterWhitelistBlacklistUsers(List<InstagramUser> instagramUsers, InstagramUser singleInstaUser = null)
        {
            // _i++;
            try
            {
                if (BlackListWhitelistHandler == null)
                    BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);

                if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.FollowBack ||
                    ActivityType == ActivityType.BroadcastMessages ||
                    ActivityType == ActivityType.AutoReplyToNewMessage ||
                    ActivityType == ActivityType.SendMessageToFollower || ActivityType == ActivityType.Reposter ||
                    ActivityType == ActivityType.Like ||
                    ActivityType == ActivityType.Comment || ActivityType == ActivityType.LikeComment)
                {
                    #region Skip Blacklisted users

                    if (ModuleSetting.SkipBlacklist.IsSkipBlackListUsers &&
                        (ModuleSetting.SkipBlacklist.IsSkipGroupBlackListUsers ||
                         ModuleSetting.SkipBlacklist.IsSkipPrivateBlackListUser))
                        return BlackListWhitelistHandler.SkipBlackListUser(instagramUsers, singleInstaUser);
                    #endregion
                }
                else if (ActivityType == ActivityType.Unfollow)
                {
                    #region Skip Whitelisted users

                    if (ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers && (ModuleSetting.ManageBlackWhiteListModel.IsSkipGroupWhiteList || ModuleSetting.ManageBlackWhiteListModel.IsSkipPrivateWhiteList))
                        return BlackListWhitelistHandler.SkipWhiteListUser(instagramUsers);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return instagramUsers;
        }
        protected bool SkippedAsBlackListWhitelisted(InstagramUser instagramUser)
        {
            try
            {
                if (BlackListWhitelistHandler == null)
                    BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);

                if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.FollowBack ||
                    ActivityType == ActivityType.BroadcastMessages ||
                    ActivityType == ActivityType.AutoReplyToNewMessage ||
                    ActivityType == ActivityType.SendMessageToFollower || ActivityType == ActivityType.Reposter ||
                    ActivityType == ActivityType.Like ||
                    ActivityType == ActivityType.Comment || ActivityType == ActivityType.LikeComment)
                {
                    #region Skip Blacklisted users

                    if (ModuleSetting.SkipBlacklist.IsSkipBlackListUsers &&
                        (ModuleSetting.SkipBlacklist.IsSkipGroupBlackListUsers ||
                         ModuleSetting.SkipBlacklist.IsSkipPrivateBlackListUser))
                    {
                        var users = BlackListWhitelistHandler.GetBlackListUsers();
                        return users != null && users.Contains(instagramUser?.Username);
                    }
                    #endregion
                }
                else if (ActivityType == ActivityType.Unfollow)
                {
                    #region Skip Whitelisted users

                    if (ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers && (ModuleSetting.ManageBlackWhiteListModel.IsSkipGroupWhiteList || ModuleSetting.ManageBlackWhiteListModel.IsSkipPrivateWhiteList))
                    {
                        var users = new List<string>();
                        if (ModuleSetting.ManageBlackWhiteListModel.IsSkipGroupWhiteList)
                            users.AddRange(BlackListWhitelistHandler.GetWhiteListUsers(GDEnums.Enums.WhitelistblacklistType.Group));
                        if (ModuleSetting.ManageBlackWhiteListModel.IsSkipPrivateWhiteList)
                            users.AddRange(BlackListWhitelistHandler.GetWhiteListUsers(GDEnums.Enums.WhitelistblacklistType.Private));
                        return users.Count > 0 && users.Contains(instagramUser?.Username);
                    }

                    #endregion
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink, [NotNull] CampaignDetails campaignDetails)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            campaignDetails = campaignFileManager.GetCampaignById(JobProcess.campaignId);

            if (campaignDetails != null)
            {
                try
                {
                    JobProcess.AddedToDb = false;
                    #region Action From Random Percentage Of Accounts
                    if (ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                        lock (lockObject)
                        {
                            Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Instagram,
                                   ConstantVariable.GetCampaignDb);
                            try
                            {
                                decimal count = campaignDetails.SelectedAccountList.Count;
                                var randomMaxAccountToPerform = (int)Math.Round(count * ModuleSetting.PerformActionFromRandomPercentage.GetRandom() / 100);

                                var numberOfAccountsAlreadyPerformedAction = CampaignService.GetAllInteractedPosts().Where(x => x.ActivityType == ActivityType && x.Permalink == postPermalink).ToList();

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
                        var lockObject = LockObjects.GetOrAdd("Lock2" + postPermalink, new object());
                        lock (lockObject)
                        {
                            Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Instagram,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                List<int> recentlyPerformedActions;
                                recentlyPerformedActions = CampaignService.GetAllInteractedPosts().
                                    Where(x => x.ActivityType == ActivityType && x.Permalink == postPermalink && (x.Status == "Success" || x.Status == "Working")).
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
                                        DelayService.ThreadSleep((time2 - time) * 1000);// Thread.Sleep((time2 - time) * 1000);
                                    }
                                }
                                if (!JobProcess.AddedToDb)
                                    AddWorkingActivityValueToDb(queryInfo, postPermalink, dbOperation);
                                else
                                {
                                    var interactedPost = dbOperation.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                                            x => x.Permalink == postPermalink && x.ActivityType == ActivityType &&
                                                 x.Username == DominatorAccountModel.AccountBaseModel.UserName && (x.Status == "Pending" || x.Status == "Working"));
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
            dbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
            {
                ActivityType = ActivityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                Username = DominatorAccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                Permalink = postPermalink,
                Status = "Pending"
            });
        }

        protected void AddWorkingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            dbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
            {
                ActivityType = ActivityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                Username = DominatorAccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                Permalink = postPermalink,
                Status = "Working"
            });
        }

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, InstagramPost post)
        {
            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                if (ModuleSetting.IschkUniquePostForCampaign)
                {
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Instagram, $"{CampaignId}.post", post.Code);
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
                        instance.AddInteractedData(SocialNetworks.Instagram, CampaignId, post.User.Username);
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
                    if (ActivityType == ActivityType.Reposter)
                    {
                        if ((DbAccountService.GetInteractedPosts(ActivityType).Where(x => x.OriginalMediaOwner == post.User.Username)).Any())
                            return false;
                    }
                    else
                    {
                        if ((DbAccountService.GetInteractedPosts(ActivityType).Where(x => x.UsernameOwner == post.User.Username)).Any())
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        protected void CheckNoMoreDataForWithQuery(ref JobProcessResult jobProcessResult)
        {
            if (!jobProcessResult.IsProcessCompleted && string.IsNullOrEmpty(jobProcessResult.maxId))
                jobProcessResult.HasNoResult = true;
        }

        protected void DelayForScraperActivity()
        {
            if (ActivityType == ActivityType.UserScraper || ActivityType == ActivityType.HashtagsScraper ||
                ActivityType == ActivityType.PostScraper)
                JobProcess.DelayBeforeNextActivity();

        }

        protected void CheckForNoMoreDataAndStopProcess(JobProcessResult jobProcessResult)
        {
            if (VisitedQueryCount >= ModuleSetting.SavedQueries.Count)
            {
                CheckForNoMoreData(jobProcessResult);
                JobProcess.Stop();
                Token.ThrowIfCancellationRequested();
            }
        }

        protected void CheckForNoMoreData(JobProcessResult jobProcessResult)
        {
            if (!jobProcessResult.IsProcessCompleted && string.IsNullOrEmpty(jobProcessResult.maxId))
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                jobProcessResult.HasNoResult = true;
            }
        }

        protected string CheckPostId(QueryInfo queryInfo)
        {
            string tempCode = queryInfo.QueryValue.Length > 100 ? queryInfo.QueryValue.Remove(11, queryInfo.QueryValue.Length - 11) : queryInfo.QueryValue;
            tempCode = tempCode.Trim();
            if (tempCode.Contains("www.instagram.com"))
                tempCode = tempCode.Split('/')[4];
            if (tempCode.Length > 11)
                tempCode = tempCode.Substring(0, 11).Trim();

            // string cehck= (tempCode.GetCodeFromUrl() ?? "1000000000143").GetCodeFromId();
            return (tempCode.GetCodeFromUrl() ?? tempCode).GetIdFromCode();//BV1pPeghFdn//BV1pPeghFd
        }

        protected string GetPostCode(QueryInfo queryInfo)
        {
            string tempCode = queryInfo.QueryValue.Length > 100 ? queryInfo.QueryValue.Remove(11, queryInfo.QueryValue.Length - 11) : queryInfo.QueryValue;
            tempCode = tempCode.Trim();
            if (tempCode.Contains("www.instagram.com"))
                tempCode = tempCode.Split('/')[4];
            if (tempCode.Length > 11)
                tempCode = tempCode.Substring(0, 11).Trim();

            return tempCode;
        }

        protected string CheckCustomUsername(QueryInfo queryInfo)
        {
            string tempCode = queryInfo.QueryValue.Length > 100 ? queryInfo.QueryValue.Remove(11, queryInfo.QueryValue.Length - 11) : queryInfo.QueryValue;
            if (tempCode.Contains("www.instagram.com"))
                tempCode = tempCode.Split('/')[3];
            return tempCode.Trim();
        }

        protected void SetQuantityIfMaxIdEmpty(JobProcessResult jobProcessResult, int lastQuanitity)
        {
            if (!(string.IsNullOrEmpty(jobProcessResult.maxId) && lastQuanitity > 0)) return;
            JobProcess.maxId = $"GDHasOnly{lastQuanitity}Last";
        }

        public bool FilterImageApply(ref InstagramPost postDetails)
        {
            ScrapeFilter.Image image = new ScrapeFilter.Image(ModuleSetting);
            try
            {
                if (FilterAlreadyLikePost(postDetails)) return true;
                if (image.FilterPostAge(postDetails)) return true;
                if (image.FilterComments(postDetails)) return true;
                if (image.FilterLikes(postDetails)) return true;
                if (image.FilterPostType(postDetails)) return true;
                if (image.FilterCaptionLength(postDetails)) return true;
                if (image.FilterByPostCaption(postDetails)) return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }
        public List<InstagramPost> FilterAllImagesApply(List<InstagramPost> postDetails)
        {
            ScrapeFilter.Image image = new ScrapeFilter.Image(ModuleSetting);
            var filteredMedias = new List<InstagramPost>();
            try
            {
                foreach(var media in postDetails)
                {
                    var data = media;
                    if (!FilterImageApply(ref data))
                        filteredMedias.Add(data);
                }
                //postDetails = FilterAlreadyLikePost(postDetails);

                //postDetails = postDetails.Count > 0 ? FilterPostAge(postDetails) : postDetails;

                //postDetails = postDetails.Count > 0 ? image.FilterComments(postDetails) : postDetails;

                //postDetails = postDetails.Count > 0 ? image.FilterLikes(postDetails) : postDetails;

                //postDetails = postDetails.Count > 0 ? image.FilterPostType(postDetails) : postDetails;

                //postDetails = postDetails.Count > 0 ? image.FilterCaptionLength(postDetails) : postDetails;

                //postDetails = postDetails.Count > 0 ? image.FilterByPostCaption(postDetails) : postDetails;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                filteredMedias = postDetails;
            }
            return filteredMedias;
        }
        public bool FilterUserApply(InstagramUser instaUser, QueryInfo queryInfo)
        {
            ScrapeFilter.User user;

            if (queryInfo.IsCustomFilterSelected)
            {
                UserFilterModel userFilterModel = JsonConvert.DeserializeObject<UserFilterModel>(queryInfo.CustomFilters);

                ModuleSetting moduleSettingsFromSearchControl = new ModuleSetting() { UserFilterModel = userFilterModel };

                user = new ScrapeFilter.User(moduleSettingsFromSearchControl);
            }
            else
            {
                user = new ScrapeFilter.User(ModuleSetting);
            }

            try
            {
                //if (user.FilterPrivateUsers(instaUser)) return true;
                if (user.FilterBusinessUsers(instaUser)) return true;
                if (user.FilterIsVerified(instaUser)) return true;
                if (user.FilterProfileUsers(instaUser)) return true;
                if (user.FilterBlacklistedUsers(instaUser, ModuleSetting.UserBlacklist, ModuleSetting.UserBlacklist)) return true;
                if (user.FilterGender(instaUser)) return true;
                if (ActivityType == ActivityType.Unfollow && ModuleSetting.IsUnfollowFollowings && !ModuleSetting.IsChkCancelPrivateRequest && !IsAlreadyFollowingUsers(instaUser)) return true;
                if (!user.ShouldGetDetailedInfo()) return false;

                UsernameInfoIgResponseHandler detailedInfo = GetDetailedInfoUser(instaUser.Username);
                DelayService.ThreadSleep(TimeSpan.FromSeconds(new Random().Next(1, 3)));// Thread.Sleep(new Random().Next(1000, 3000));
                if (user.FilterPrivateUsers(detailedInfo)) return true;
                if (user.FilterProfileUsers(detailedInfo)) return true;
                if (user.FilterFollowers(detailedInfo)) return true;
                if (user.FilterFollowings(detailedInfo)) return true;
                if (user.FilterMinFollowRatio(detailedInfo)) return true;
                if (user.FilterMaxFollowRatio(detailedInfo)) return true;
                if (user.FilterFollowRatioRange(detailedInfo)) return true;
                if (user.FilterPosts(detailedInfo)) return true;
                if (user.FilterBioRestrictedWords(detailedInfo)) return true;
                if (user.FilterBioNotRestrictedWords(detailedInfo)) return true;
                if (user.FilterBioRestrictedWordsLength(detailedInfo)) return true;
                if (user.FilterNonEnglishUsers(detailedInfo)) return true;
                #region Filter by last posted date

                if (ModuleSetting.UserFilterModel.FilterPostedInRecentDays || ModuleSetting.UserFilterModel.FilterNotPostedInRecentdDays || ModuleSetting.UserFilterModel.FilterInvaildWord)
                {
                    UserInfoWithFeed userInfoWithFeed = UserInfoWithFeeds(instaUser);
                    if (ModuleSetting.UserFilterModel.FilterPostedInRecentDays && user.FilterMaxDaysSinceLastPost(userInfoWithFeed)) return true;

                    if (ModuleSetting.UserFilterModel.FilterNotPostedInRecentdDays && user.FilterNotPostedWithinSomeDays(userInfoWithFeed)) return true;

                    if (ModuleSetting.UserFilterModel.FilterInvaildWord && user.FilterPostCaptionWithSpecificWords(userInfoWithFeed)) return true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                // ignored
            }
            return false;
        }


        public bool FilterUserApplyForFollow(InstagramUser instaUser, QueryInfo queryInfo)
        {
            ScrapeFilter.User user;

            if (queryInfo.IsCustomFilterSelected)
            {
                UserFilterModel userFilterModel = JsonConvert.DeserializeObject<UserFilterModel>(queryInfo.CustomFilters);

                ModuleSetting moduleSettingsFromSearchControl = new ModuleSetting() { UserFilterModel = userFilterModel };

                user = new ScrapeFilter.User(moduleSettingsFromSearchControl);
            }
            else
                user = new ScrapeFilter.User(ModuleSetting);


            try
            {
                //if (user.FilterPrivateUsers(instaUser)) return true;
                if (user.FilterBusinessUsers(instaUser)) return true;
                if (user.FilterIsVerified(instaUser)) return true;
                if (user.FilterProfileUsers(instaUser)) return true;
                if (user.FilterBlacklistedUsers(instaUser, ModuleSetting.UserBlacklist, ModuleSetting.UserBlacklist)) return true;
                if (user.FilterGender(instaUser)) return true;
                if (ActivityType == ActivityType.Follow && IsAlreadyFollowingUsers(instaUser)) return true;
                if (ActivityType == ActivityType.Unfollow && !ModuleSetting.IsChkCancelPrivateRequest && !IsAlreadyFollowingUsers(instaUser))
                {
                    return true;
                }
                if (!user.ShouldGetDetailedInfo()) return false;
                var IGUser = instaUser.UserDetails.Username != null ? instaUser : GetDetailedInfoUser(instaUser.Username);
                var detailedInfo = IGUser.UserDetails;
                if (user.FilterPrivateUsers(IGUser)) return true;
                if (user.FilterProfileUsers(detailedInfo)) return true;
                if (user.FilterFollowers(detailedInfo)) return true;
                if (user.FilterFollowings(detailedInfo)) return true;
                if (user.FilterMinFollowRatio(detailedInfo)) return true;
                if (user.FilterMaxFollowRatio(detailedInfo)) return true;
                if (user.FilterFollowRatioRange(detailedInfo)) return true;
                if (user.FilterPosts(detailedInfo)) return true;
                if (user.FilterBioRestrictedWords(detailedInfo)) return true;
                //if (user.FilterBioNotRestrictedWords(detailedInfo)) return true;
                if (user.FilterBioRestrictedWordsLength(detailedInfo)) return true;
                if (user.FilterNonEnglishUsers(detailedInfo)) return true;
                #region Filter by last posted date

                if (ModuleSetting.UserFilterModel.FilterPostedInRecentDays || ModuleSetting.UserFilterModel.FilterNotPostedInRecentdDays || ModuleSetting.UserFilterModel.FilterInvaildWord)
                {
                    UserInfoWithFeed userInfoWithFeed = UserInfoWithFeeds(instaUser);
                    if (ModuleSetting.UserFilterModel.FilterPostedInRecentDays && user.FilterMaxDaysSinceLastPost(userInfoWithFeed)) return true;

                    if (ModuleSetting.UserFilterModel.FilterNotPostedInRecentdDays && user.FilterNotPostedWithinSomeDays(userInfoWithFeed)) return true;

                    if (ModuleSetting.UserFilterModel.FilterInvaildWord && user.FilterPostCaptionWithSpecificWords(userInfoWithFeed)) return true;
                }
                #endregion
            }
            catch (Exception)
            {
                // ignored
            }
            return false;


        }

        public List<InstagramUser> FilterAllUserApply(List<InstagramUser> instaUser, QueryInfo queryInfo)
        {
            ScrapeFilter.User user;

            if (queryInfo.IsCustomFilterSelected)
            {
                UserFilterModel userFilterModel = JsonConvert.DeserializeObject<UserFilterModel>(queryInfo.CustomFilters);

                ModuleSetting moduleSettingsFromSearchControl = new ModuleSetting() { UserFilterModel = userFilterModel };

                user = new ScrapeFilter.User(moduleSettingsFromSearchControl);
            }
            else
                user = new ScrapeFilter.User(ModuleSetting);

            try
            {

                instaUser = user.FilterAllPrivateUser(instaUser);
                instaUser = user.FilterAllBusinessUsers(instaUser);
                instaUser = user.FilterIsVerifiedAll(instaUser);
                instaUser = user.FilterAllProfileUsers(instaUser);
                instaUser = user.FilterAllUserGender(instaUser);
                instaUser = user.FilterAllBlackListedUser(instaUser, ModuleSetting.UserBlacklist, ModuleSetting.UserBlacklist);
                IsAlreadyAllFollowingUsers(instaUser);
                if (!user.ShouldGetDetailedInfo())
                    return instaUser;

                instaUser = instaUser.Count > 0 ? user.FilterAllFollowers(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllFollowings(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllMinFollowRatio(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllMaxFollowRatio(instaUser) : instaUser;    //Pending
                instaUser = instaUser.Count > 0 ? user.FilterAllFollowRatioRange(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterPosts(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllBioRestrictedWords(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllBioNotRestrictedWords(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllBioRestrictedWordsLength(instaUser) : instaUser;
                instaUser = instaUser.Count > 0 ? user.FilterAllNonEnglishUsers(instaUser) : instaUser;
            }
            catch (Exception)
            {
                // ignored
            }
            return instaUser;
        }

        public List<InstagramPost> FilterAllPostUserApply(List<InstagramPost> instaPostUser, QueryInfo queryInfo)
        {
            ScrapeFilter.User user;

            if (queryInfo.IsCustomFilterSelected)
            {
                UserFilterModel userFilterModel = JsonConvert.DeserializeObject<UserFilterModel>(queryInfo.CustomFilters);

                ModuleSetting moduleSettingsFromSearchControl = new ModuleSetting() { UserFilterModel = userFilterModel };

                user = new ScrapeFilter.User(moduleSettingsFromSearchControl);
            }
            else
                user = new ScrapeFilter.User(ModuleSetting);

            try
            {
                instaPostUser = user.FilterAllPostUserPrivate(instaPostUser);
                instaPostUser = user.FilterAllPostUserBusiness(instaPostUser);
                instaPostUser = user.FilterPostUserIsVerifiedAll(instaPostUser);
                instaPostUser = user.FilterAllPostuserProfile(instaPostUser);
                instaPostUser = user.FilterAllPostUserGender(instaPostUser);
                instaPostUser = user.FilterAllPostUserBlackListed(instaPostUser, ModuleSetting.UserBlacklist, ModuleSetting.UserBlacklist);

                IsAlreadyAllFollowingPostUsers(instaPostUser);

                if (!user.ShouldGetDetailedInfo())
                    return instaPostUser;

                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllPostFollowers(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllPostFollowings(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllMinPostFollowRatio(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllMaxPostFollowRatio(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllPostFollowRatioRange(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterPostUserPosts(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllPostBioRestrictedWords(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllPostBioNotRestrictedWords(instaPostUser) : instaPostUser;
                instaPostUser = instaPostUser.Count > 0 ? user.FilterAllPostBioRestrictedWordsLength(instaPostUser) : instaPostUser;
            }
            catch (Exception)
            {
                // ignored
            }
            return instaPostUser;
        }

        protected bool IsAlreadyFollowingUsers(InstagramUser instaUser)
        {
            return instaUser != null && (instaUser.IsFollowing || (instaUser.UserDetails != null && instaUser.UserDetails.IsFollowing));
            //if (instaUser.OutgoingRequest)
            //    return true;
            //if (DominatorAccountModel.IsRunProcessThroughBrowser)
            //{
            //    if (InstaFunction.IsAlreadyFollowedByBrowser(instaUser.Username, DominatorAccountModel, ActivityType, Token))
            //        return true;
            //    else
            //        return false;
            //}
            //if (AccountModel.LstFollowings == null || AccountModel.LstFollowings.Count == 0 || !AccountModel.LstFollowings.Any())
            //    AccountModel.LstFollowings = GetFollowingUsers();

            //if (AccountModel.LstFollowings != null && !string.IsNullOrEmpty(instaUser.Pk))
            //{
            //    if (AccountModel.LstFollowings.Any(y => y.Pk == instaUser.Pk))
            //    {
            //        return true;
            //    }
                    
            //    return false;
            //}
            //if (AccountModel.LstFollowings != null)
            //    return AccountModel.LstFollowings.Any(y => y.Username == instaUser.Username);

            //return false;
        }


        protected List<InstagramPost> IsAlreadyAllFollowingPostUsers(List<InstagramPost> instaPostUser)
        {
            //if (AccountModel.LstFollowings == null || !AccountModel.LstFollowings.Any())
            //    AccountModel.LstFollowings = GetFollowingUsers();

            //if (AccountModel.LstFollowings != null)
            //    instaPostUser.RemoveAll(x => AccountModel.LstFollowings.Any(y => y.Username == x.User.Username));
            instaPostUser.RemoveAll(x => x != null && x?.User != null && x.User.IsFollowing);
            return instaPostUser;
        }
        protected List<InstagramUser> IsAlreadyAllFollowingUsers(List<InstagramUser> instaUser)
        {
            //if (AccountModel.LstFollowings == null || !AccountModel.LstFollowings.Any())
            //    AccountModel.LstFollowings = GetFollowingUsers();

            //if (AccountModel.LstFollowings != null)
            //    instaUser.RemoveAll(x => AccountModel.LstFollowings.Any(y => y.Username.Equals(x.Username)));
            instaUser.RemoveAll(x => x != null && x.IsFollowing);
            return instaUser;
        }

        public List<InstagramUser> GetFollowingUsers()
        {
            if (!DominatorAccountModel.IsUserLoggedIn)
                return null;
            var accountUserName = string.Empty;
            var lstInstagramUser = new List<InstagramUser>();
            string maxid = null;
            var browser = GramStatic.IsBrowser;
            if (DominatorAccountModel.AccountBaseModel.UserName.Contains("@"))
            {
                accountUserName = DominatorAccountModel.AccountBaseModel.UserId ?? AccountModel.DsUserId;
                //accountUserName = DominatorAccountModel.Cookies["ds_user_id"].Value;
                //if (string.IsNullOrEmpty(accountUserName))
                //accountUserName = DominatorAccountModel.Cookies["ds_user"].Value;

            }
            else
                accountUserName = DominatorAccountModel.AccountBaseModel.UserName;
            if (string.IsNullOrEmpty(accountUserName) && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.ProfileId))
                accountUserName = DominatorAccountModel.AccountBaseModel.ProfileId;
            if (!browser)
            {
                var visited = InstaFunction.GdBrowserManager.VisitPage(DominatorAccountModel, $"https://www.instagram.com/{accountUserName}/").Result;
                var usernameInfo = InstaFunction.SearchUsername(DominatorAccountModel, accountUserName, Token);
                DominatorAccountModel.AccountBaseModel.UserId = usernameInfo.Pk;

                do
                {
                    var userFollowings = InstaFunction.GetUserFollowings(DominatorAccountModel, AccountModel, DominatorAccountModel.AccountBaseModel.UserId, Token,string.Empty, maxid, DominatorAccountModel.AccountBaseModel.ProfileId, IsWeb: true).Result;
                    if (!userFollowings.Success)
                        break;
                    lstInstagramUser.AddRange(userFollowings.UsersList);
                    maxid = userFollowings.MaxId;
                    if (lstInstagramUser.Count > 50)
                        break;
                } while (!string.IsNullOrEmpty(maxid)); 
            }
            else
            {
                var users = InstaFunction.GdBrowserManager.GetUserFollowings(DominatorAccountModel, DominatorAccountModel.UserName, Token);
                lstInstagramUser.AddRange(users.UsersList);
            }

            return lstInstagramUser;
        }

        public List<InstagramUser> GetAccountFollowers()
        {
            if (!DominatorAccountModel.IsUserLoggedIn) return null;
            var lstFollowers = new List<InstagramUser>();
            var browser = GramStatic.IsBrowser;
            string maxid = null;
            if (!browser)
            {
                var username = !string.IsNullOrEmpty(DominatorAccountModel?.AccountBaseModel?.UserName)
                    && DominatorAccountModel.AccountBaseModel.UserName.Contains("@") ? DominatorAccountModel.AccountBaseModel.ProfileId
                    : DominatorAccountModel.AccountBaseModel.UserName;
                var usernameInfo = InstaFunction.SearchUsername(DominatorAccountModel,username, Token);
                DominatorAccountModel.AccountBaseModel.UserId = usernameInfo.Pk;

                do
                {
                    FollowerAndFollowingIgResponseHandler userFollowers = InstaFunction.GetUserFollowers(DominatorAccountModel, DominatorAccountModel.AccountBaseModel.UserId, Token, maxid, DominatorAccountModel.AccountBaseModel.ProfileId, IsWeb: true).Result;
                    if (!userFollowers.Success) break;
                    lstFollowers.AddRange(userFollowers.UsersList);
                    maxid = userFollowers.MaxId;
                    if (lstFollowers.Count >= usernameInfo.FollowerCount)
                        break;
                } while (!string.IsNullOrEmpty(maxid));
            }
            else
            {
                var userFollowers = GdBrowserManager.GetUserFollowers(DominatorAccountModel, DominatorAccountModel.AccountBaseModel.UserName, Token);
                if (userFollowers.Success)
                {
                    lstFollowers.AddRange(userFollowers.UsersList);
                }
            }

            return lstFollowers;
        }

        protected UsernameInfoIgResponseHandler GetDetailedInfoUser(string user)
        {
            try
            {
                return GramStatic.IsBrowser ?
                    InstaFunction.GdBrowserManager.GetUserInfo(DominatorAccountModel, user, Token)
                    : InstaFunction.SearchUsername(DominatorAccountModel, user, Token);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        protected void CustomLog(string message) =>
            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, message);

        protected void NoMoreDataLog() => GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType);

        protected UserInfoWithFeed UserInfoWithFeeds(InstagramUser user, bool isFollowing = false)
        {
            if (!isFollowing && user.IsPrivate)
                return null;
            var browser = GramStatic.IsBrowser;
            var userFeed = 
                browser ?
                InstaFunction.GdBrowserManager.GetUserFeed(DominatorAccountModel, user.Username, Token)
                : InstaFunction.GetUserFeed(DominatorAccountModel, AccountModel, user.Username, Token);
            if (userFeed.Success)
                return new UserInfoWithFeed(user, userFeed.Items);
            return null;
        }

        protected CommentModel GetCommentModel()
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            return JsonConvert.DeserializeObject<CommentModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
        }
        public bool FilterByEnableDisableOwnPostComment(InstagramPost instagramPost)
        {
            if ((ActivityType == ActivityType.Comment) && (instagramPost.User.Username == DominatorAccountModel.UserName) && CommentModel.IsChkCommentsOnOwnPost)
            {
                if (!(CommentModel ?? GetCommentModel()).IsChkCommentsOnOwnPost)
                    return true;
            }
            return false;
        }
        public List<InstagramPost> FilterByEnableDisableOwnPostComment(List<InstagramPost> instagramPost)
        {
            List<InstagramPost> posts = new List<InstagramPost>();
            if (ActivityType == ActivityType.Comment && CommentModel.IsChkCommentsOnOwnPost)
            {
                instagramPost.ForEach(x =>
                {
                    if (x.User.Username == DominatorAccountModel.UserName)
                    {
                        if ((CommentModel ?? GetCommentModel()).IsChkCommentsOnOwnPost)
                            posts.Add(x);
                    }
                });
                instagramPost.RemoveAll(x => posts.Any(y => y.Code == x.Code));
            }

            return instagramPost;
        }
        protected bool UserFilterModel(InstagramPost instagramPost, QueryInfo queryInfo)
        {
            try
            {
                var userInfo = InstaFunction.SearchUsername(DominatorAccountModel, instagramPost?.User?.Username, Token);
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //{

                //    userInfo = InstaFunction.SearchUserInfoById(DominatorAccountModel, AccountModel, instagramPost.User.Pk, Token);
                //}
                //else
                //    userInfo = InstaFunction.GdBrowserManager.GetUserInfo(DominatorAccountModel, instagramPost.User.Username, Token);
                if (!DominatorAccountModel.IsRunProcessThroughBrowser && userInfo == null)
                    userInfo = InstaFunction.SearchUserInfoById(DominatorAccountModel, AccountModel, instagramPost.User.Pk, Token).Result;
                if (FilterUserApply(userInfo, queryInfo))
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        protected List<InstagramPost> GetAllHashTagFeeds(HashTagFeedIgResponseHandler feedDetails)
        {
            return new List<InstagramPost>(feedDetails.RankedItems.Concat(feedDetails.Items));
        }

        public bool CheckQueryValueOnMessageList(BroadcastMessagesModel BroadcastMessagesModel, QueryInfo queryInfo)
        {
            if (ActivityType == ActivityType.BroadcastMessages)
            {
                var getManageMessagesModels = BroadcastMessagesModel.LstDisplayManageMessageModel.Where(x =>
                               x.SelectedQuery.FirstOrDefault(y =>
                                   (y.Content.QueryType == queryInfo.QueryType) &&
                                   (y.Content.QueryValue == queryInfo.QueryValue)) != null).ToList();
                if (getManageMessagesModels.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"this query value is not availabl in message list");
                    return true;
                }
            }
            return false;
        }
        public bool CheckingLoginRequiredResponse(string response, string responseMessage, QueryInfo queryInfo)
        {
            try
            {

                if (!string.IsNullOrEmpty(response) && (response.Contains("login_required") || response.Contains("challenge_required")))
                {
                    try
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                       $"Log in required please update your account");
                        DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                        JobProcess.Stop();
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if (!string.IsNullOrEmpty(response) && response.Contains("content that may not meet Instagram's community guidelines") && !response.Contains("\"status\": \"ok\""))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"{responseMessage}");
                    return false;
                }

                if (!string.IsNullOrEmpty(response) && response.Contains("will show up here."))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"This {queryInfo.QueryValue} is not available in instagram");
                    return false;
                }
                if (!string.IsNullOrEmpty(response) && (response.Contains("Media not found or unavailable") || response.Contains("Media is unavailable")))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"{queryInfo.QueryValue} Media is unavailable");
                    return false;
                }
                if (!string.IsNullOrEmpty(response) && response.Contains("User not found"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                    $" {queryInfo.QueryValue} User not found, please check your input");
                    return false;
                }
                if (!string.IsNullOrEmpty(response) && response.Contains("Please wait a few minutes before you try again."))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                   $"Please run this campaign after sometimes.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        public List<InstagramUser> GetUserInfoDetails(List<InstagramUser> lstUser)
        {
            UsernameInfoIgResponseHandler usernameInfoIgResponseHandler = null;
            bool isUserName = false;
            var instagramUser = new List<InstagramUser>();
            try
            {
                var users = new ScrapeFilter.User(ModuleSetting);
                if (!users.ShouldGetDetailedInfo())
                    return lstUser;

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.UserName,ActivityType,
                    $"Updating Users Detailed Information for {lstUser.Count} users");

                foreach (var user in lstUser)
                {
                    Token.ThrowIfCancellationRequested();
                    if (!isUserName)
                    {
                        usernameInfoIgResponseHandler =
                                                InstaFunction.SearchUsername(DominatorAccountModel, user.Username, Token);
                        //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                        //{
                        //    usernameInfoIgResponseHandler =
                        //                        InstaFunction.SearchUsername(DominatorAccountModel, user.Username, Token); 
                        //}
                        //else
                        //{
                        //    usernameInfoIgResponseHandler =
                        //                        GdBrowserManager.GetUserInfo(DominatorAccountModel, user.Username, Token);
                        //}
                        
                    }
                    if (usernameInfoIgResponseHandler != null && usernameInfoIgResponseHandler.ToString()
                        .Contains("Please wait a few minutes before you try again"))
                    {
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(5));
                        isUserName = true;
                    }
                    if (isUserName)
                    {
                        usernameInfoIgResponseHandler =
                            InstaFunction.SearchUserInfoById(DominatorAccountModel, AccountModel,user.Pk, Token).Result;
                        DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                        if (usernameInfoIgResponseHandler != null && usernameInfoIgResponseHandler.Success)
                            user.UserDetails = usernameInfoIgResponseHandler.instaUserDetails;
                    }
                    DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                    if (usernameInfoIgResponseHandler != null && usernameInfoIgResponseHandler.Success)
                        user.UserDetails = usernameInfoIgResponseHandler.instaUserDetails;
                    instagramUser.Add(user);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return instagramUser.Count > 0 ? instagramUser: lstUser;
        }

        public List<InstagramPost> GetPostOfUserInfoDetails(List<InstagramPost> lstFeed)
        {
            UsernameInfoIgResponseHandler usernameInfoIgResponseHandler = null;
            bool isUserName = false;
            try
            {
                var users = new ScrapeFilter.User(ModuleSetting);
                if (!users.ShouldGetDetailedInfo())
                    return lstFeed;

                for (int user = 0; user < lstFeed.Count; user++)
                {
                    Token.ThrowIfCancellationRequested();
                    if (!isUserName)
                    {
                        usernameInfoIgResponseHandler = InstaFunction.SearchUsername(DominatorAccountModel, lstFeed[user].User.Username, Token);
                        DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                        if (usernameInfoIgResponseHandler != null && usernameInfoIgResponseHandler.Success)
                            lstFeed[user].User.UserDetails = usernameInfoIgResponseHandler.instaUserDetails;
                    }
                    if (usernameInfoIgResponseHandler != null && usernameInfoIgResponseHandler.ToString().Contains("Please wait a few minutes before you try again"))
                    {
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(5));
                        isUserName = true;
                    }
                    if (isUserName)
                    {
                        usernameInfoIgResponseHandler = InstaFunction.SearchUserInfoById(DominatorAccountModel, AccountModel, lstFeed[user].User.Pk, Token).Result;
                        DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                        if (usernameInfoIgResponseHandler != null && usernameInfoIgResponseHandler.Success)
                            lstFeed[user].User.UserDetails = usernameInfoIgResponseHandler.instaUserDetails;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return lstFeed;
        }
        public HashTagFeedIgResponseHandler GetResponseFromHashTagPost(QueryInfo queryInfo, JobProcessResult jobProcessResult, string hashtag, int nextPageCount,
             ref List<InstagramPost> lstInstagramPost, ref string topMaxId, ref string topNextMediaId, ref string recentMaxId, ref string recentNextMediaId)
        {
            HashTagFeedIgResponseHandler feedDetails = null;
            lstInstagramPost = new List<InstagramPost>();
            try
            {
                {
                    if (!string.IsNullOrEmpty(jobProcessResult?.maxId))
                        topMaxId = jobProcessResult.maxId;
                    if (!string.IsNullOrEmpty(jobProcessResult?.RankToken))
                        topNextMediaId = jobProcessResult.RankToken;
                    feedDetails = 
                        GramStatic.IsBrowser ?
                        InstaFunction.GdBrowserManager.GetHashtagFeedForUserScraper(DominatorAccountModel, queryInfo, Token)
                        : InstaFunction.GetHashtagFeedForUserScraper(DominatorAccountModel, AccountModel, queryInfo.QueryValue, Token, nextPageCount, ModuleSetting.IsRecentHashTagPost, topMaxId, topNextMediaId).Result;
                    topMaxId = jobProcessResult.maxId = feedDetails?.MaxId;
                    topNextMediaId = jobProcessResult.RankToken = feedDetails?.NextMediaId;
                    lstInstagramPost = GetAllHashTagFeeds(feedDetails);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return feedDetails;
        }

        protected void NoData(ref JobProcessResult jobProcessResult,string PostUrl="")
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $"No Comments Found For Post {PostUrl}");
            jobProcessResult.HasNoResult = true;
            jobProcessResult.maxId = null;
            jobProcessResult.IsProcessCompleted = false;
        }

        public bool FilterPostAge(InstagramPost post)
        {
            //if (ModuleSetting.PostFilterModel.FilterPostAge)
            //{
            //    if (!ModuleSetting.PostFilterModel.FilterBeforePostAge)
            //        filteredFeeds.RemoveAll(x => (DateTime.UtcNow - x.TakenAt.EpochToDateTimeUtc()).TotalDays > ModuleSetting.PostFilterModel.MaxLastPostAge);
            //    else
            //        filteredFeeds.RemoveAll(x => (DateTime.UtcNow - x.TakenAt.EpochToDateTimeUtc()).TotalDays < ModuleSetting.PostFilterModel.MaxPostAge);
            //}
            //return filteredFeeds;
            return false;
        }

        public bool FilterAlreadyLikePost(InstagramPost post)
        {
            return ActivityType == ActivityType.Like && post != null && post.HasLiked;
        }


        //This method is only for User scrapper , To skip already scraped user which is scapped by across all accounts
        public List<InstagramUser> GetInteractedUserAccrossAllFor(List<InstagramUser> instaUser)
        {
            var Skipped = 0;
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            var instaConfig = genericFileManager.GetModel<InstagramUserModel>(ConstantVariable.GetOtherInstagramSettingsFile());
            if (ActivityType == ActivityType.UserScraper && instaConfig != null && instaConfig.IsEnableScrapeDiffrentUserChecked && !ModuleSetting.IsScrpeUniqueUserForThisCampaign)
            {
                List<string> lstUser = GlobalInteractionDetails.GetInteractedData(SocialNetworks.Instagram, ActivityType);
                Skipped = instaUser.RemoveAll(x => lstUser.Any(y => y == x.Username));
            }
            if (Skipped > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"Successfully Skipped {Skipped} Interacted Users.");
            return instaUser;
        }

        //This method is only for User scrapper , To skip already scraped user which is scrapped by campaign
        public List<InstagramUser> GetInteractedCampaignUser(List<InstagramUser> instaUser)
        {
            var Skipped = 0;
            if (ModuleSetting.IsScrpeUniqueUserForThisCampaign && ActivityType == ActivityType.UserScraper)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                List<string> lstUser = instance.GetCampaignInteractedData(SocialNetworks.Instagram, $"{CampaignId}.UserScraper");
                if (instaUser?.FirstOrDefault()?.Pk != null)
                    Skipped = instaUser.RemoveAll(x => lstUser.Any(y => y == x.Pk));
                else
                    Skipped = instaUser.RemoveAll(x => lstUser.Any(y => y == x.UserId));
            }
            if (Skipped > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"Successfully Skipped {Skipped} Interacted Users.");
            return instaUser;
        }
        public bool CheckInteractedPostDbData(MediaInfoIgResponseHandler mediaInfo, List<InteractedPosts> lstInteractedPosts)
        {
            if (ActivityType == ActivityType.Reposter)
            {
                lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();
                if ((lstInteractedPosts.Any(y => y.OriginalMediaCode == mediaInfo.InstagramPost.Code)))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"This Post {mediaInfo.InstagramPost.Code} is already posted by this account");
                    return true;
                }
            }
            if (ActivityType == ActivityType.Comment && !CommentModel.IsChkMultipleCommentsOnSamePost || ActivityType == ActivityType.PostScraper || ActivityType == ActivityType.Like)
            {
                lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();
                if (lstInteractedPosts.Any(x => x.PkOwner == mediaInfo.InstagramPost.Code))
                    return true;
            }
            return false;
        }
        public bool CheckInteractedUserDbData(List<InteractedUsers> lstInteractedUsers, UsernameInfoIgResponseHandler userInfo,
            List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers> lstCampaignIntractedUsersForUserScraper)
        {
            if(ActivityType == ActivityType.StoryViewer)
            {
                lstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                if(lstInteractedUsers.Any(x => x.InteractedUsername == userInfo.Username && x.Status == "Story Seen"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"This {DominatorAccountModel.UserName} is already seen this {userInfo.Username} user stories");
                    return true;
                }
            }
            if (ActivityType == ActivityType.Follow)
            {
                //lstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                if (userInfo != null && userInfo.instaUserDetails != null && userInfo.instaUserDetails.IsFollowing /*&& lstInteractedUsers.Any(x => x.InteractedUsername == userInfo.Username)*/)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"This {DominatorAccountModel.UserName} is already following this {userInfo.Username} user");
                    return true;
                }
            }

            if (ActivityType == ActivityType.BroadcastMessages && ModuleSetting.IsSkipUserWhoReceivedMessage)
            {
                lstInteractedUsers = DbAccountService.Get<InteractedUsers>(x => x.DirectMessage != null).ToList();
                if (lstInteractedUsers.Any(x => x.InteractedUsername == userInfo.Username))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"This user {userInfo.Username} message has been sent already");
                    return true;
                }
            }
            if (ActivityType != ActivityType.UserScraper)
            {
                var usersList = FilterWhitelistBlacklistUsers(null, userInfo);
                if (usersList != null && usersList.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                    $"This user {userInfo.Username} is in Blacklist");
                    return true;
                }
            }

            if (ActivityType == ActivityType.UserScraper && !ModuleSetting.IsScrpeUniqueUserForThisCampaign)
            {
                lstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType)
                    .Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                if (lstInteractedUsers.Any(x => x.InteractedUsername == userInfo.Username))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType,
                        $"This user {userInfo.Username} already scraped");
                    return true;
                }
            }

            if (ActivityType == ActivityType.UserScraper && ModuleSetting.IsScrpeUniqueUserForThisCampaign)
            {
                lstCampaignIntractedUsersForUserScraper = CampaignService.GetAllInteractedUsers().ToList();
                if (lstCampaignIntractedUsersForUserScraper.Any(x => x.InteractedUsername == userInfo.Username))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"This user {userInfo.Username} already scraped in current campaign");
                    return true;
                }

            }
            return false;
        }

        public List<InstagramPost> CheckInteractedPostsData(List<InteractedPosts> lstInteractedPosts, List<InstagramPost> filteredFeeds)
        {
            var SkippedCount = 0;
            if (ActivityType == ActivityType.Reposter)
            {
                lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();
                SkippedCount = filteredFeeds.RemoveAll(x => lstInteractedPosts.Any(y => y.OriginalMediaCode == x.Code));
            }
            if (ActivityType == ActivityType.Comment || ActivityType == ActivityType.PostScraper || ActivityType == ActivityType.Like)
            {
                lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();
                SkippedCount =  filteredFeeds.RemoveAll(x => lstInteractedPosts.Any(y => y.PkOwner == x.Code));
            }
            if(SkippedCount > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $"Skipped {SkippedCount} Already Interacted Post.");
            return filteredFeeds;
        }

        public List<InstagramUser> FilterOnlyBusinessAccounts(List<InstagramUser> lstInteractedUsers)
        {
            try
            {
                lstInteractedUsers.RemoveAll(x => x.UserDetails.IsBusiness == false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return lstInteractedUsers;
        }
        public List<InstagramPost> FilterOnlyBusinessAccounts(List<InstagramPost> lstInteractedPost)
        {
            if (ModuleSetting.FollowOnlyBusinessAccounts)
                lstInteractedPost.RemoveAll(x => x.User.IsBusiness == false);
            return lstInteractedPost;
        }
    }
}
