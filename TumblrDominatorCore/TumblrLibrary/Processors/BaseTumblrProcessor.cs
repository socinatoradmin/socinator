using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrCampaign = DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign;

namespace TumblrDominatorCore.TumblrLibrary.Processors
{
    public abstract class BaseTumblrProcessor
    {
        protected readonly ITumblrBrowserManager _browser;
        protected readonly IDbCampaignService _campaignService;
        protected readonly IDbAccountService _dbAccountService;

        protected readonly IProcessScopeModel _processScopeModel;
        protected readonly ITumblrJobProcess JobProcess;
        protected readonly ITumblrFunct TumblrFunct;
        protected readonly IDbGlobalService DbGlobalService;


        protected BaseTumblrProcessor(IProcessScopeModel processScopeModel, ITumblrJobProcess jobProcess,
            IDbAccountService dbAccountService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct, IDbGlobalService globalService)
        {
            JobProcess = jobProcess;
            _dbAccountService = dbAccountService;
            _campaignService = campaignService;
            TumblrFunct = tumblrFunct;
            ModuleSetting = JobProcess.ModuleSetting;
            _processScopeModel = processScopeModel;
            DbGlobalService = globalService;
            _browser = JobProcess.browserManager;
            PostScraperModel = processScopeModel.GetActivitySettingsAs<PostScraperModel>();
            CampaignDetails = processScopeModel.CampaignDetails;
        }

        public ModuleSetting ModuleSetting { get; set; }
        public SearchFilterModel _searchFilterModel { get; set; }
        public PostFilterModel _postFilterModel { get; set; }
        public UserFilterModel _userFilterModel { get; set; }
        public PostScraperModel PostScraperModel { get; set; }

        public FollowerModel followerModel { get; set; }
        public UnfollowerModel unFollowerModel { get; set; }

        public LikeModel likeModel { get; set; }

        public UnLikeModel UnLikeModel { get; set; }
        public ReblogModel reblogModel { get; set; }
        public CommentModel comentModel { get; set; }

        public PostScraperModel postScrapperModel { get; set; }

        public BroadcastMessagesModel broadcastMessagesModel { get; set; }

        public CampaignDetails CampaignId { get; set; }

        public static ConcurrentDictionary<string, object> LockObjects { get; } =
            new ConcurrentDictionary<string, object>();

        public ActivityType ActivityType => JobProcess.ActivityType;
        public CampaignDetails CampaignDetails { get; set; } = new CampaignDetails();

        public void Start(QueryInfo queryInfo)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                _searchFilterModel = InstanceProvider.GetInstance<SearchFilterModel>();
                switch (JobProcess.ActivityType)
                {
                    case ActivityType.Like:
                        likeModel = _processScopeModel.GetActivitySettingsAs<LikeModel>();
                        _searchFilterModel = likeModel?.SearchFilter;
                        break;
                    case ActivityType.Unlike:
                        UnLikeModel = _processScopeModel.GetActivitySettingsAs<UnLikeModel>();
                        _searchFilterModel = UnLikeModel?.SearchFilter;
                        break;
                    case ActivityType.Reblog:
                        reblogModel = _processScopeModel.GetActivitySettingsAs<ReblogModel>();
                        _searchFilterModel = reblogModel?.SearchFilter;
                        break;
                    case ActivityType.PostScraper:
                        postScrapperModel = _processScopeModel.GetActivitySettingsAs<PostScraperModel>();
                        _searchFilterModel = postScrapperModel?.SearchFilter;
                        break;
                    case ActivityType.Follow:
                        followerModel = _processScopeModel.GetActivitySettingsAs<FollowerModel>();
                        break;
                    case ActivityType.Unfollow:
                        unFollowerModel = _processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
                        break;
                    case ActivityType.BroadcastMessages:
                        broadcastMessagesModel = _processScopeModel.GetActivitySettingsAs<BroadcastMessagesModel>();
                        break;
                    case ActivityType.Comment:
                        comentModel = _processScopeModel.GetActivitySettingsAs<CommentModel>();
                        _searchFilterModel = comentModel.SearchFilter;
                        break;
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
            var jobProcessResult = new JobProcessResult();
            if (queryInfo.QueryType != null && queryInfo.QueryValue != null)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                Process(queryInfo, ref jobProcessResult);
            }
        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);


        protected bool CheckUserUniqueNess(JobProcessResult jobProcessResult, string username,
            ActivityType activityType)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActivityType];

            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var campaignInteractionDetails =
                    InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        campaignInteractionDetails.AddInteractedData(SocialNetworks.Tumblr,
                            $"{CampaignDetails.CampaignId}", username);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }
            return true;
        }
        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink)
        {
            if (CampaignDetails == null) return true;
            try
            {
                // JobProcess.AddedToDb = false;
                // bool addedToDb = false;

                #region like From Random Percentage Of Accounts

                if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                {
                    var lockObject = LockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                    lock (lockObject)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        try
                        {
                            decimal count = CampaignDetails.SelectedAccountList.Count;
                            var randomMaxAccountToPerform = (int)Math.Round(
                                count * JobProcess.ModuleSetting.PerformActionFromRandomPercentage.GetRandom() / 100);
                            var numberOfAccountsAlreadyPerformedAction =
                                _campaignService.GetCountOfInteractionForSpecificPost(ActivityType, postPermalink);

                            if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction) return false;
                            AddPendingActivityValueToDb(queryInfo, postPermalink);
                            //  JobProcess.AddedToDb = true;
                        }
                        catch (AggregateException ae)
                        {
                            foreach (var e in ae.InnerExceptions)
                                if (e is TaskCanceledException || e is OperationCanceledException)
                                    throw new OperationCanceledException(@"Cancellation Requested !");
                                else
                                    e.DebugLog(e.StackTrace + e.Message);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }

                #endregion

                #region Delay Between actions On SamePost

                if (JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                {
                    var lockObject = LockObjects.GetOrAdd("Lock2" + postPermalink, new object());
                    lock (lockObject)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        try
                        {
                            var recentlyPerformedActions = _campaignService
                                .GetInteractedPosts(postPermalink, ActivityType, "")
                                .OrderByDescending(x => x.InteractionTimeStamp).Select(x => x.InteractionTimeStamp)
                                .Take(1).ToList();


                            if (recentlyPerformedActions.Count > 0)
                            {
                                var recentlyPerformedTime = recentlyPerformedActions[0];
                                var delay = JobProcess.ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                                var time = DateTimeUtilities.GetEpochTime();
                                var time2 = recentlyPerformedTime + delay;
                                if (time < time2) Thread.Sleep((time2 - time) * 1000);
                            }


                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException(@"Cancellation Requested !");
                        }
                        catch (AggregateException ae)
                        {
                            foreach (var e in ae.InnerExceptions)
                                if (e is TaskCanceledException || e is OperationCanceledException)
                                    throw new OperationCanceledException(@"Cancellation Requested !");
                                else
                                    e.DebugLog(e.StackTrace + e.Message);
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
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        throw new OperationCanceledException(@"Cancellation Requested !");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }


        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, TumblrPost post,
            ActivityType activityType)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var campaignInteractionDetails =
                    InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (JobProcess.ModuleSetting.IschkUniquePostForCampaign)
                    try
                    {
                        campaignInteractionDetails.AddInteractedData(SocialNetworks.Tumblr,
                            $"{CampaignDetails.CampaignId}.post", post.PostUrl);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        campaignInteractionDetails.AddInteractedData(SocialNetworks.Tumblr, CampaignDetails.CampaignId,
                            post.OwnerUsername);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }

            if (JobProcess.ModuleSetting.IschkUniqueUserForAccount)
                try
                {
                    if (_dbAccountService.GetPostfrmUniqueUser(post.OwnerUsername, activityType) > 0) return false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return true;
        }

        private void AddPendingActivityValueToDb(QueryInfo queryInfo, string postPermalink)
        {
            _campaignService.Add(new TumblrCampaign.InteractedPosts
            {
                ActivityType = ActivityType.ToString(),
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                AccountEmail = JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                ContentId = postPermalink,
                Status = "Pending"
            });
        }

        #region Posts Filter

        public List<TumblrPost> FilterdPost(List<TumblrPost> postList)
        {
            var pins = postList;
            try
            {
                if (PostScraperModel.BlogFilterModel.FilterComments)
                    pins.RemoveAll(x => !string.IsNullOrEmpty(x.NotesCount) && TumblrUtility.IsIntegerOnly(x.NotesCount) && !PostScraperModel.BlogFilterModel.CommentsCountRange.InRange(int.Parse(x.NotesCount)));

                if (PostScraperModel.BlogFilterModel.IscheckPostReply)
                    pins.RemoveAll(x => !x.PostComment);
                if (PostScraperModel.BlogFilterModel.FilterRestrictedPostCaptionList)
                    pins.RemoveAll(x => !string.IsNullOrEmpty(x.Caption) && PostScraperModel.BlogFilterModel.CaptionBlacklists.Contains(x.Caption));
                if (PostScraperModel.BlogFilterModel.FilterAcceptedPostCaptionList)
                    pins.RemoveAll(x => !string.IsNullOrEmpty(x.Caption) && !PostScraperModel.BlogFilterModel.CaptionWhitelist.Contains(x.Caption));
                return pins;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return pins;
            }
        }


        #endregion
        #region Filter Blacklisted or WhiteListed Users
        protected List<string> FilterBlacklistedOrWhiteListedUsers(List<string> list)
        {

            try
            {
                var lstBlackListUserGlobal = DbGlobalService.GetAllBlackListUsers();
                var lstWhiteListUserGlobal = DbGlobalService.GetAllWhiteListUsers();
                var lstWhiteListUserPrivate = _dbAccountService.GetPrivateWhitelist()?.ToList();
                var lstBlackListUserPrivate = _dbAccountService.GetPrivateBlacklist()?.ToList();

                if (followerModel != null && followerModel.IsGroupBlacklists ||
                    likeModel != null && likeModel.IsGroupBlacklists ||
                    reblogModel != null && reblogModel.IsGroupBlacklists ||
                    broadcastMessagesModel != null && broadcastMessagesModel.IsGroupBlacklists)
                    list.RemoveAll(x => lstBlackListUserGlobal.Any(y => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y.UserName) && y.UserName == x));
                if (followerModel != null && followerModel.IsPrivateBlacklists ||
                    likeModel != null && likeModel.IsPrivateBlacklists ||
                    reblogModel != null && reblogModel.IsPrivateBlacklists ||
                    broadcastMessagesModel != null && broadcastMessagesModel.IsPrivateBlacklists)
                    list.RemoveAll(x => lstBlackListUserPrivate.Any(y => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y.UserName) && y.UserName == x));
                if (unFollowerModel != null && unFollowerModel.IsChkToGroupWhitelist)
                    list.RemoveAll(x => lstWhiteListUserGlobal.Any(y => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y.UserName) && y.UserName == x));
                if (unFollowerModel != null && unFollowerModel.IsChkPrivateWhiteListed)
                    list.RemoveAll(x => lstWhiteListUserPrivate.Any(y => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y.UserName) && y.UserName == x));
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }
        #endregion

    }
}