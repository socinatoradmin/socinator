using System;
using System.Linq;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using PinDominatorCore.Response;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts;
using System.Collections.Generic;
using PinDominatorCore.PDUtility;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominatorCore.PDLibrary.Process
{
    public class FollowProcess : PdJobProcessInteracted<InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly IDbCampaignService _dbCampaignService;
        private int _activityFailedCount = 1;

        public FollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager,
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess,
            IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _delayService = delayService;
            FollowerModel = JsonConvert.DeserializeObject<FollowerModel>(TemplateModel.ActivitySettings);

            InitializePerDayAndPercentageCount();
            _dbCampaignService = dbCampaignService;
        }

        //  private IPinFunct _pinFunct;
        // Per day Like count for individual campaigns
        private int UserCountToTry { get; set; }

        // Per day Comment count for individual campaigns
        private int UserCountToComment { get; set; }

        public FollowerModel FollowerModel { get; set; }

        /// <summary>
        ///     Initializes per day count actions (like, comment, message) and percentages for individual campaigns
        /// </summary>
        private void InitializePerDayAndPercentageCount()
        {
            UserCountToTry = FollowerModel.Tries.GetRandom();
            UserCountToComment = FollowerModel.Comments.GetRandom();
        }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var campaignInteractionDetails =
                InstanceProvider.GetInstance<ICampaignInteractionDetails>();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
            if (moduleSetting == null)
                return new JobProcessResult();

            CheckAutoFollowUnfollowProcess(scrapeResult);

            // if scrapeResult.ResultPost is null then it is user otherwise it is board
            if (scrapeResult.ResultPost != null)
            {
                var board = (PinterestBoard)scrapeResult.ResultPost;
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);
                try
                {
                    if (moduleSetting.IsTemplateMadeByCampaignMode && FollowerModel.IsChkFollowUnique)
                        try
                        {
                            campaignInteractionDetails.AddInteractedData(SocialNetworks.Pinterest, CampaignId,
                                board.BoardUrl);
                        }
                        catch (Exception)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"{string.Format("LangKeySkipping".FromResourceDictionary())} {board.BoardUrl}" +
                                $" {string.Format("LangKeyAsInteractedforthisCampaignForUniqueUser".FromResourceDictionary())}");

                            JobProcessResult.IsProcessSuceessfull = false;
                            return JobProcessResult;
                        }

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    BoardResponse response = null;
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        response = PdLogInProcess.BrowserManager.FollowBoard(DominatorAccountModel, board.BoardUrl, JobCancellationTokenSource);

                    else
                        response = PinFunct.FollowBoardSingle(board.BoardUrl, DominatorAccountModel);
                    if (response != null && response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);

                        IncrementCounters();

                        AddFollowedDataToDataBase(scrapeResult);

                        if (FollowerModel.ChkTryUserLatestPostsChecked || FollowerModel.ChkCommentOnUserLatestPostsChecked)
                            PostFollowProcess(board, scrapeResult.QueryInfo);

                        JobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);
                        try
                        {
                            campaignInteractionDetails.AddInteractedData(SocialNetworks.Pinterest, CampaignId, board.BoardUrl);
                        }
                        catch
                        {
                            // ignored
                        }

                        JobProcessResult.IsProcessSuceessfull = false;
                    }
                    DelayBeforeNextActivity();
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            else
            {
                try
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

                    var pinterestUser = (PinterestUser)scrapeResult.ResultUser;

                    if (moduleSetting.IsTemplateMadeByCampaignMode && FollowerModel.IsChkFollowUnique)
                        try
                        {
                            campaignInteractionDetails.AddInteractedData(SocialNetworks.Pinterest, CampaignId,
                                pinterestUser.Username);
                        }
                        catch (Exception)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"{string.Format("LangKeySkipping".FromResourceDictionary())} {pinterestUser.Username} " +
                                $"{string.Format("LangKeyAsInteractedforthisCampaignForUniqueUser".FromResourceDictionary())}");

                            JobProcessResult.IsProcessSuceessfull = false;
                            return JobProcessResult;
                        }

                    FriendshipsResponse friendshipsResponse = null;
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        friendshipsResponse = PdLogInProcess.BrowserManager.FollowUser(DominatorAccountModel, pinterestUser.Username, JobCancellationTokenSource);
                    else
                        friendshipsResponse = PinFunct.FollowUserSingle(DominatorAccountModel, pinterestUser.Username);

                    if (friendshipsResponse != null && friendshipsResponse.Success)
                    {
                        var followedBackData = DbAccountService.GetFriendships(FollowType.FollowingBack).ToList();
                        if (followedBackData != null)
                            foreach (var item in followedBackData)
                                if (item.Username == pinterestUser.Username)
                                {
                                    pinterestUser.FollowedBack = 1;
                                    break;
                                }

                        scrapeResult.ResultUser = pinterestUser;

                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

                        IncrementCounters();

                        AddFollowedDataToDataBase(scrapeResult);
                        PostFollowProcess(pinterestUser, scrapeResult.QueryInfo);
                        JobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        if (friendshipsResponse != null && friendshipsResponse.Issue != null &&
                            friendshipsResponse.Issue.Message.Contains("Already followed user"))
                            return JobProcessResult;
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, friendshipsResponse?.Issue?.Message);

                        //Reschedule if action block
                        if (FollowerModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == FollowerModel.ActivityFailedCount)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                                $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {FollowerModel.FailedActivityReschedule} " +
                                $"{"LangKeyHour".FromResourceDictionary()}");

                            StopAndRescheduleJob(FollowerModel.FailedActivityReschedule);
                        }

                        try
                        {
                            campaignInteractionDetails.AddInteractedData(SocialNetworks.Pinterest, CampaignId, pinterestUser.Username);
                        }
                        catch
                        {
                            // ignored
                        }

                        JobProcessResult.IsProcessSuceessfull = false;
                    }
                   CheckAutoFollowUnfollowProcess(scrapeResult);
                    DelayBeforeNextActivity();
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return JobProcessResult;
        }
       
        public void CheckAutoFollowUnfollowProcess(ScrapeResultNew scrapeResult)
        {
            try
            {
                UserNameInfoPtResponseHandler currentUserInfo = null;

                if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked &&
                    (FollowerModel.IsChkStartUnFollowToolStopFollow || FollowerModel.IsChkStopFollowTool))
                {
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        currentUserInfo = PdLogInProcess.BrowserManager.SearchByCustomUser(DominatorAccountModel,
                            DominatorAccountModel.AccountBaseModel.ProfileId,
                               JobCancellationTokenSource);
                    }
                    else
                        currentUserInfo = PinFunct.GetUserDetails(DominatorAccountModel.AccountBaseModel.ProfileId,
                            DominatorAccountModel, FollowerModel.UserFilterModel.FilterPostCounts || FollowerModel.ChkCommentOnUserLatestPostsChecked).Result;
                }
                var lstTotalFollowedusers =
                    DbAccountService.Get<InteractedUsers>(x =>
                    x.Username == DominatorAccountModel.AccountBaseModel.ProfileId);


                #region Process for auto Follow and Unfollow

                if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked &&
                    FollowerModel.IsChkStartUnFollowToolStopFollow)
                {
                    if (FollowerModel.IsChkStopFollowToolWhenReachChecked)
                    {
                        if (currentUserInfo != null)
                        {
                            var followingCount = currentUserInfo.FollowingCount;
                            if (FollowerModel.IsChkStopFollowToolWhenReachChecked &&
                                followingCount >= FollowerModel.StopFollowToolWhenReach.GetRandom()) StartUnFollow();
                        }
                    }

                    if (FollowerModel.IsChkWhenFollowerFollowingsGreater)
                    {
                        if (currentUserInfo != null)
                        {
                            var follwingRatio = currentUserInfo.FollowerCount / currentUserInfo.FollowingCount;
                            if (FollowerModel.IsChkWhenFollowerFollowingsGreater &&
                                FollowerModel.FollowerFollowingsMaxValue < follwingRatio) StartUnFollow();
                        }
                    }
                }

                if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked && FollowerModel.IsChkStopFollowTool)
                {
                    if (FollowerModel.IsChkStopFollowToolWhenReachChecked)
                    {                        
                        if (FollowerModel.IsChkStopFollowToolWhenReachChecked &&
                            currentUserInfo.FollowingCount >= FollowerModel.StopFollowToolWhenReach.GetRandom()) StopFollowTool();
                    }

                    if (FollowerModel.IsChkWhenFollowerFollowingsGreater)
                    {
                        if (currentUserInfo != null)
                        {
                            var followFollwingRatio = currentUserInfo.FollowerCount / currentUserInfo.FollowingCount;
                            if (FollowerModel.IsChkWhenFollowerFollowingsGreater &&
                                FollowerModel.FollowerFollowingsMaxValue > followFollwingRatio) StopFollowTool();
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartUnFollow()
        {
            try
            {
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                dominatorScheduler.EnableDisableModules(ActivityType.Follow, ActivityType.Unfollow,
                    DominatorAccountModel.AccountId);
            }
            catch (InvalidOperationException ex)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    ex.Message.Contains("1001")
                        ? "LangKeyFollowMetAutoEnableConfigForUnfollowMessage".FromResourceDictionary()
                        : "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StopFollowTool()
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                dominatorScheduler.StopActivity(DominatorAccountModel, "LangKeyFollow".FromResourceDictionary(), moduleConfiguration.TemplateId, false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void PostFollowProcess(PinterestUser pinterestUser, QueryInfo queryInfo)
        {
            try
            {
                if (FollowerModel.ChkTryUserLatestPostsChecked || FollowerModel.ChkCommentOnUserLatestPostsChecked)
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType, "LangKeyStartedAfterFollow".FromResourceDictionary());

                #region Try Post process after Follow

                if (FollowerModel.ChkTryUserLatestPostsChecked)
                    TryUsersLatestPosts(pinterestUser, queryInfo);

                #endregion

                #region Comment on post after Follow process

                if (FollowerModel.ChkCommentOnUserLatestPostsChecked)
                    CommentOnUsersLatestPosts(pinterestUser, queryInfo);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void PostFollowProcess(PinterestBoard pinterestBoard, QueryInfo queryInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, "LangKeyStartedAfterFollow".FromResourceDictionary());

                #region Try Post process after Follow

                if (FollowerModel.ChkTryUserLatestPostsChecked)
                    TryBoardsLatestPosts(pinterestBoard, queryInfo);

                #endregion

                #region Comment on post after Follow process

                if (FollowerModel.ChkCommentOnUserLatestPostsChecked)
                    CommentOnBoardsLatestPosts(pinterestBoard, queryInfo);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsPostFollowProcessCompleted(ActivityType activityType,string Username="")
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (moduleSetting.IsTemplateMadeByCampaignMode)
            {
                if (activityType == ActivityType.Try)
                {
                    var lstInteractedPosts = _dbCampaignService.GetInteractedPosts(activityType).ToList();
                    lstInteractedPosts = lstInteractedPosts.Where(x => x.Username == Username &&
                    x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName).ToList();
                    if (UserCountToTry <= lstInteractedPosts.Count)
                        return true;
                }
                else if (activityType == ActivityType.Comment)
                {
                    var lstInteractedPosts = _dbCampaignService.GetInteractedPosts(activityType).ToList();
                    lstInteractedPosts = lstInteractedPosts.Where(x => x.Username == Username &&
                    x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName).ToList();
                    if (UserCountToComment <= lstInteractedPosts.Count)
                        return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (activityType == ActivityType.Try)
                {
                    var lstInteractedPosts = _dbCampaignService.GetInteractedPosts(activityType).ToList();
                    lstInteractedPosts = lstInteractedPosts.Where(x => x.Username == Username &&
                    x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName).ToList();
                    if (UserCountToTry <= lstInteractedPosts.Count)
                        return true;
                }
                else if (activityType == ActivityType.Comment)
                {
                    var lstInteractedPosts = _dbCampaignService.GetInteractedPosts(activityType).ToList();
                    lstInteractedPosts = lstInteractedPosts.Where(x => x.Username == Username &&
                    x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName).ToList();
                    if (UserCountToComment <= lstInteractedPosts.Count)
                        return true;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private void CommentOnUsersLatestPosts(PinterestUser pinterestUser, QueryInfo queryInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary(), "LangKeyFollow".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyCommentProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);
                var currentCommentCount = 0;
                var pinCommentCount = FollowerModel.Comments.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Comment, pinterestUser.Username.ToString()))
                    return;

                var hasNoMorePosts = false;
                string bookMark = null;

                bool isScroll = false;
                int scroll = 0;
                while (!hasNoMorePosts)
                {
                    var lstPins = new List<PinterestPin>();
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        lstPins = PdLogInProcess.BrowserManager.SearchPinsOfUser(DominatorAccountModel, pinterestUser.Username,
                               JobCancellationTokenSource, isScroll, scroll);
                        scroll = 1;
                        isScroll = true;
                        if (lstPins.Count == 0)
                            hasNoMorePosts = true;
                    }
                    else
                    {
                        var userPinDetails = PinFunct.GetPinsFromSpecificUser(pinterestUser.Username, DominatorAccountModel, bookMark,NeedCommentCount:FollowerModel.PostFilterModel.FilterComments);
                        lstPins = userPinDetails.LstUserPin;
                        bookMark = userPinDetails.BookMark;
                    }

                    foreach (var pinterestPin in lstPins)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var comment = FollowerModel.LstComments.GetRandomItem();
                        var commentResponse = PinFunct.CommentOnPin(pinterestPin.PinId, comment, DominatorAccountModel);

                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var moduleSetting =
                            jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                        if (commentResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                string.Format("LangKeySuccessfullyCommentedWithSomeNameAndPin".FromResourceDictionary(), pinterestUser.Username, pinterestPin.PinId));
                            PinInfoPtResponseHandler pinInfo = null;
                            if (DominatorAccountModel.IsRunProcessThroughBrowser)
                            {
                                try
                                {
                                    BrowserManager.AddNew(JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                    pinInfo = BrowserManager.SearchByCustomPin(DominatorAccountModel, pinterestPin.PinId, JobCancellationTokenSource);
                                }
                                catch (OperationCanceledException)
                                {
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                                finally
                                {
                                    if (BrowserManager.BrowserWindows.Count > 2)
                                        BrowserManager.CloseLast();
                                }
                            }
                            else
                                pinInfo = PinFunct.GetPinDetails(pinterestPin.PinId, DominatorAccountModel);

                            if (pinInfo != null && pinInfo.Success)
                            {
                                if (moduleSetting.IsTemplateMadeByCampaignMode)
                                    _dbCampaignService.Add(new InteractedPosts
                                    {
                                        OperationType = ActivityType.Comment.ToString(),
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        Username = pinInfo.UserName,
                                        QueryType = queryInfo.QueryType,
                                        Query = queryInfo.QueryValue,
                                        SourceBoard = pinInfo.BoardId,
                                        SourceBoardName = pinInfo.BoardName,
                                        CommentCount = pinInfo.CommentCount,
                                        MediaType = pinInfo.MediaType,
                                        PinDescription = pinInfo.Description,
                                        PinId = pinInfo.PinId,
                                        PinWebUrl = pinInfo.PinWebUrl,
                                        UserId = pinInfo.UserId,
                                        MediaString = pinInfo.MediaString,
                                        Comment = comment,
                                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                    });

                                DbAccountService.Add(
                                    new DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts
                                    {
                                        OperationType = ActivityType.Comment.ToString(),
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        Username = pinInfo.UserName,
                                        QueryType = queryInfo.QueryType,
                                        Query = queryInfo.QueryValue,
                                        SourceBoard = pinInfo.BoardId,
                                        SourceBoardName = pinInfo.BoardName,
                                        CommentCount = pinInfo.CommentCount,
                                        MediaType = pinInfo.MediaType,
                                        PinDescription = pinInfo.Description,
                                        PinId = pinInfo.PinId,
                                        PinWebUrl = pinInfo.PinWebUrl,
                                        UserId = pinInfo.UserId,
                                        MediaString = pinInfo.MediaString,
                                        Comment = comment
                                    });
                            }

                            currentCommentCount++;
                        }

                        if (IsPostFollowProcessCompleted(ActivityType.Comment,pinterestPin.User.Username.ToString()) ||
                            lstPins.Count < pinCommentCount || pinCommentCount <= currentCommentCount)
                        {
                            hasNoMorePosts = true;
                            return;
                        }

                        var delay = FollowerModel.DelayBetweenCommentsForAfterActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment, delay);
                        _delayService.ThreadSleep(delay * 1000);
                    }

                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (string.IsNullOrEmpty(bookMark) || bookMark.Contains("[]") || lstPins.Count<=0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                  DominatorAccountModel.UserName, ActivityType,
                                  string.Format("LangKeyNoMorePinsAvailable".FromResourceDictionary(), "LangKeyCommentProcess".FromResourceDictionary()));                            
                            break;
                        }
                         
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CommentOnBoardsLatestPosts(PinterestBoard pinterestBoard, QueryInfo queryInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary(), "LangKeyFollow".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyCommentProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);
                var currentCommentCount = 0;
                var pinCommentCount = FollowerModel.Comments.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Comment))
                    return;

                var hasNoMorePosts = false;
                var bookMark = (string)null;
                FollowerModel.LstComments = Regex.Split(FollowerModel.Message, @"\r\n").ToList();

                bool isScroll = false;
                int scroll = 0;
                while (!hasNoMorePosts)
                {
                    var lstPins = new List<PinterestPin>();
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        lstPins = PdLogInProcess.BrowserManager.SearchPinsOfBoard(DominatorAccountModel, pinterestBoard.BoardUrl,
                               JobCancellationTokenSource, isScroll, scroll);
                        scroll = 1;
                        isScroll = true;
                        if (lstPins.Count == 0)
                            hasNoMorePosts = true;
                    }
                    else
                    {
                        var boardPinDetails =
                            PinFunct.GetPinsByBoardUrl(pinterestBoard.BoardUrl, DominatorAccountModel, bookMark);
                        lstPins = boardPinDetails.LstBoardPin;
                        bookMark = boardPinDetails.BookMark;
                    }

                    foreach (var pinterestPin in lstPins)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var comment = FollowerModel.LstComments.GetRandomItem();
                        var commentResponse = PinFunct.CommentOnPin(pinterestPin.PinId, comment, DominatorAccountModel);

                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var moduleSetting =
                            jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                        if (commentResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                string.Format("LangKeySuccessfullyCommentedWithSomeNameAndPin".FromResourceDictionary(), pinterestBoard.BoardUrl, pinterestPin.PinId));

                            PinInfoPtResponseHandler pinInfo = null;
                            if (DominatorAccountModel.IsRunProcessThroughBrowser)
                            {
                                try
                                {
                                    BrowserManager.AddNew(JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                    pinInfo = BrowserManager.SearchByCustomPin(DominatorAccountModel,
                                        pinterestPin.PinId, JobCancellationTokenSource);
                                }
                                catch (OperationCanceledException)
                                {
                                    throw new OperationCanceledException();
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                                finally
                                {
                                    if (BrowserManager.BrowserWindows.Count > 2)
                                        BrowserManager.CloseLast();
                                }
                            }
                            else
                                pinInfo = PinFunct.GetPinDetails(pinterestPin.PinId, DominatorAccountModel);
                            if (pinInfo != null && pinInfo.Success)
                            {
                                if (moduleSetting.IsTemplateMadeByCampaignMode)
                                {
                                    _dbCampaignService.Add(new InteractedPosts()
                                    {
                                        OperationType = ActivityType.Comment.ToString(),
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        Username = pinInfo.UserName,
                                        QueryType = queryInfo.QueryType,
                                        Query = queryInfo.QueryValue,
                                        SourceBoard = pinInfo.BoardId,
                                        SourceBoardName = pinInfo.BoardName,
                                        CommentCount = pinInfo.CommentCount,
                                        MediaType = pinInfo.MediaType,
                                        PinDescription = pinInfo.Description,
                                        PinId = pinInfo.PinId,
                                        PinWebUrl = pinInfo.PinWebUrl,
                                        UserId = pinInfo.UserId,
                                        MediaString = pinInfo.MediaString,
                                        Comment = comment,
                                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                    });
                                }

                                DbAccountService.Add(new InteractedPosts()
                                {
                                    OperationType = ActivityType.Comment.ToString(),
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    Username = pinInfo.UserName,
                                    QueryType = queryInfo.QueryType,
                                    Query = queryInfo.QueryValue,
                                    SourceBoard = pinInfo.BoardId,
                                    SourceBoardName = pinInfo.BoardName,
                                    CommentCount = pinInfo.CommentCount,
                                    MediaType = pinInfo.MediaType,
                                    PinDescription = pinInfo.Description,
                                    PinId = pinInfo.PinId,
                                    PinWebUrl = pinInfo.PinWebUrl,
                                    UserId = pinInfo.UserId,
                                    MediaString = pinInfo.MediaString,
                                    Comment = comment,
                                });
                            }

                            currentCommentCount++;
                        }

                        if (IsPostFollowProcessCompleted(ActivityType.Comment, pinterestPin.User.Username.ToString()) ||
                            lstPins.Count < pinCommentCount || pinCommentCount <= currentCommentCount)
                        {
                            hasNoMorePosts = true;
                            return;
                        }

                        var delay = FollowerModel.DelayBetweenCommentsForAfterActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment, delay);
                        _delayService.ThreadSleep(delay * 1000);
                    }

                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (string.IsNullOrEmpty(bookMark) || bookMark.Contains("[]"))
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void TryUsersLatestPosts(PinterestUser pinterestUser, QueryInfo queryInfo)
        {
            try
            {                
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyTry".FromResourceDictionary(), "LangKeyFollow".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyTryProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);
                var currentTryCount = 0;
                var pinTryCount = FollowerModel.Tries.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Try, pinterestUser.Username.ToString()))
                    return;

                var hasNoMorePosts = false;
                var bookMark = (string)null;
                FollowerModel.LstNotes = Regex.Split(FollowerModel.Note, @"\r\n").ToList();

                bool isScroll = false;
                int scroll = 0;
                while (!hasNoMorePosts)
                {
                    var lstPins = new List<PinterestPin>();
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        lstPins = PdLogInProcess.BrowserManager.SearchPinsOfUser(DominatorAccountModel, pinterestUser.Username,
                               JobCancellationTokenSource, isScroll, scroll);
                        scroll = 1;
                        isScroll = true;
                        if (lstPins.Count == 0)
                            hasNoMorePosts = true;
                    }
                    else
                    {
                        var userPinDetails =
                            PinFunct.GetPinsFromSpecificUser(pinterestUser.Username, DominatorAccountModel, bookMark,NeedCommentCount:FollowerModel.PostFilterModel.FilterComments);
                        lstPins = userPinDetails.LstUserPin;
                        bookMark = userPinDetails.BookMark;
                    }

                    foreach (var pinterestPin in lstPins)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        string note = FollowerModel.LstNotes.GetRandomItem();

                        TryResponse tryResponse = null;
                        if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            tryResponse = PdLogInProcess.BrowserManager.Try(DominatorAccountModel, pinterestPin.PinId,
                               note, FollowerModel.MediaPath, JobCancellationTokenSource);
                        }
                        else
                            tryResponse = PinFunct.TryPin(FollowerModel.MediaPath, note, pinterestPin.PinId, DominatorAccountModel);

                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var moduleSetting =
                            jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                        if (tryResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                string.Format("LangKeySuccessfullyTriedWithSomeNameAndPin".FromResourceDictionary(), pinterestUser.Username, pinterestPin.PinId));
                            var pinInfo = PinFunct.GetPinDetails(pinterestPin.PinId, DominatorAccountModel);
                            if (pinInfo.Success)
                            {
                                if (moduleSetting.IsTemplateMadeByCampaignMode)
                                    _dbCampaignService.Add(new InteractedPosts
                                    {
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        MediaType = MediaType.Image,
                                        OperationType = ActivityType.Try.ToString(),
                                        QueryType = queryInfo.QueryType,
                                        Query = queryInfo.QueryValue,
                                        PinId = pinInfo.PinId,
                                        UserId = pinInfo.UserId,
                                        Username = pinInfo.UserName,
                                        SourceBoard = pinInfo.BoardId,
                                        SourceBoardName = pinInfo.BoardName,
                                        CommentCount = pinInfo.CommentCount,
                                        PinDescription = pinInfo.Description,
                                        PinWebUrl = pinInfo.PinWebUrl,
                                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                    });

                                DbAccountService.Add(
                                    new DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts
                                    {
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        MediaType = MediaType.Image,
                                        QueryType = queryInfo.QueryType,
                                        Query = queryInfo.QueryValue,
                                        OperationType = ActivityType.Try.ToString(),
                                        PinId = pinInfo.PinId,
                                        UserId = pinInfo.UserId,
                                        Username = pinInfo.UserName,
                                        SourceBoard = pinInfo.BoardId,
                                        SourceBoardName = pinInfo.BoardName,
                                        CommentCount = pinInfo.CommentCount,
                                        PinDescription = pinInfo.Description,
                                        PinWebUrl = pinInfo.PinWebUrl
                                    });
                            }

                            currentTryCount++;
                        }

                        if (IsPostFollowProcessCompleted(ActivityType.Try, pinterestPin.User.Username) ||
                            lstPins.Count < pinTryCount || pinTryCount <= currentTryCount)
                        {
                            hasNoMorePosts = true;
                            return;
                        }

                        var delay = FollowerModel.DelayBetweenTriesForAfterActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Try, delay);
                        _delayService.ThreadSleep(delay * 1000);
                    }

                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (string.IsNullOrEmpty(bookMark) || bookMark.Contains("[]"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                  DominatorAccountModel.UserName, ActivityType,
                                  string.Format("LangKeyNoMorePinsAvailable".FromResourceDictionary(), "LangKeyTry".FromResourceDictionary()));                            
                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void TryBoardsLatestPosts(PinterestBoard pinterestBoard, QueryInfo queryInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyStartedSomeActivityPostAfterSomeAction"), "LangKeyTry".FromResourceDictionary(), "LangKeyFollow".FromResourceDictionary());
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyTryProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);
                var currentTryCount = 0;
                var pinTryCount = FollowerModel.TriesPerUser.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Try))
                    return;

                var hasNoMorePosts = false;
                var bookMark = (string)null;
                FollowerModel.LstNotes = Regex.Split(FollowerModel.Note, @"\r\n").ToList();


                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                bool isScroll = false;
                int scroll = 0;
                while (!hasNoMorePosts)
                {
                    var lstPins = new List<PinterestPin>();
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        lstPins = PdLogInProcess.BrowserManager.SearchPinsOfBoard(DominatorAccountModel, pinterestBoard.BoardUrl,
                               JobCancellationTokenSource, isScroll, scroll);
                        scroll = 1;
                        isScroll = true;
                        if (lstPins.Count == 0)
                            hasNoMorePosts = true;
                    }
                    else
                    {
                        var boardPinDetails = PinFunct.GetPinsByBoardUrl(pinterestBoard.BoardUrl, DominatorAccountModel, bookMark);
                        lstPins = boardPinDetails.LstBoardPin;
                        bookMark = boardPinDetails.BookMark;
                    }

                    foreach (var pinterestPin in lstPins)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        string note = FollowerModel.LstNotes.GetRandomItem<string>();
                        TryResponse tryResponse = null;
                        if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            tryResponse = PdLogInProcess.BrowserManager.Try(DominatorAccountModel, pinterestPin.PinId,
                               note, FollowerModel.MediaPath, JobCancellationTokenSource);
                        }
                        else
                            tryResponse = PinFunct.TryPin(FollowerModel.MediaPath, note, pinterestPin.PinId, DominatorAccountModel);

                        if (tryResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                string.Format("LangKeySuccessfullyTriedWithSomeNameAndPin".FromResourceDictionary(), pinterestBoard.BoardUrl, pinterestPin.PinId));
                            if (moduleSetting.IsTemplateMadeByCampaignMode)
                                _dbCampaignService.Add(new InteractedPosts
                                {
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    MediaType = MediaType.Image,
                                    QueryType = queryInfo.QueryType,
                                    Query = queryInfo.QueryValue,
                                    OperationType = ActivityType.Try.ToString(),
                                    PinId = pinterestPin.PinId,
                                    SourceBoard = pinterestBoard.BoardUrl,
                                    Username = pinterestBoard.UserName,
                                    SourceBoardName = pinterestBoard.BoardName,
                                    SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                    SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                });

                            DbAccountService.Add(new InteractedPosts()
                            {
                                InteractionDate = DateTimeUtilities.GetEpochTime(),
                                MediaType = MediaType.Image,
                                QueryType = queryInfo.QueryType,
                                Query = queryInfo.QueryValue,
                                OperationType = ActivityType.Try.ToString(),
                                PinId = pinterestPin.PinId,
                                SourceBoard = pinterestBoard.BoardUrl,
                                Username = pinterestBoard.UserName,
                                SourceBoardName = pinterestBoard.BoardName,
                            });
                            currentTryCount++;
                        }

                        if (IsPostFollowProcessCompleted(ActivityType.Try) ||
                            lstPins.Count < pinTryCount || pinTryCount <= currentTryCount)
                        {
                            hasNoMorePosts = true;
                            return;
                        }

                        var delay = FollowerModel.DelayBetweenTriesForAfterActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Try, delay);
                        _delayService.ThreadSleep(delay * 1000);
                    }
                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                        if (string.IsNullOrEmpty(bookMark) || bookMark.Contains("[]"))
                            break;
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (scrapeResult.ResultPost == null)
                {
                    var user = (PinterestUser)scrapeResult.ResultUser;
                    if (moduleSetting.IsTemplateMadeByCampaignMode)
                        _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers
                        {
                            ActivityType = ActivityType.ToString(),
                            QueryType = scrapeResult.QueryInfo.QueryType,
                            Query = scrapeResult.QueryInfo.QueryValue,
                            Bio = user.UserBio,
                            FollowersCount = user.FollowersCount,
                            FollowingsCount = user.FollowingsCount,
                            FullName = user.FullName,
                            HasAnonymousProfilePicture = user.HasProfilePic,
                            PinsCount = user.PinsCount,
                            ProfilePicUrl = user.ProfilePicUrl,
                            Username=user.Username,
                            InteractedUsername = user.Username,
                            InteractedUserId = user.UserId,
                            InteractionTime = DateTimeUtilities.GetEpochTime(),
                            Website = user.WebsiteUrl,
                            FollowedBack = user.FollowedBack,
                            IsVerified = user.IsVerified,
                            TriesCount = user.TriesCount,
                            Type = PinterestIdentityType.User.ToString(),
                            SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                            SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                        });


                    DbAccountService.Add(new InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        Date = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Bio = user.UserBio,
                        FollowersCount = user.FollowersCount,
                        FollowingsCount = user.FollowingsCount,
                        FullName = user.FullName,
                        HasAnonymousProfilePicture = user.HasProfilePic,
                        PinsCount = user.PinsCount,
                        ProfilePicUrl = user.ProfilePicUrl,
                        Username=user.Username,
                        InteractedUsername = user.Username,
                        InteractedUserId = user.UserId,
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        Website = user.WebsiteUrl,
                        FollowedBack = user.FollowedBack,
                        IsVerified = user.IsVerified,
                        TriesCount = user.TriesCount,
                        Type = PinterestIdentityType.User.ToString()
                    });

                    var existed =
                        DbAccountService.GetSingle<Friendships>(x => x.Username == scrapeResult.ResultUser.Username);
                    if (existed == null)
                    {
                        DbAccountService.Add(new Friendships
                        {
                            Time = DateTimeUtilities.GetEpochTime(),
                            Bio = user.UserBio,
                            FullName = user.FullName,
                            HasAnonymousProfilePicture = user.HasProfilePic,
                            PinsCount = user.PinsCount,
                            ProfilePicUrl = user.ProfilePicUrl,
                            Username = scrapeResult.ResultUser.Username,
                            Website = user.WebsiteUrl,
                            IsVerified = user.IsVerified,
                            FollowType = FollowType.Following,
                            Followers = user.FollowersCount,
                            Followings = user.FollowingsCount,
                            BoardsCount = user.BoardsCount
                        });
                    }
                    else if (existed.FollowType == FollowType.FollowingBack)
                    {
                        existed.FollowType = FollowType.Mutual;
                        DbAccountService.Update(existed);
                    }
                }
                else
                {
                    var board = (PinterestBoard)scrapeResult.ResultPost;
                    if (moduleSetting.IsTemplateMadeByCampaignMode)
                        _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers
                        {
                            ActivityType = ActivityType.ToString(),
                            InteractionTime = DateTimeUtilities.GetEpochTime(),
                            QueryType = scrapeResult.QueryInfo.QueryType,
                            Query = scrapeResult.QueryInfo.QueryValue,
                            BoardDescription = board.BoardDescription,
                            BoardUrl = board.BoardUrl,
                            BoardName = board.BoardName,
                            FollowersCount = board.FollowersCount,
                            PinsCount = board.PinsCount,
                            InteractedUsername = board.UserName,
                            InteractedUserId=board.UserId,
                            HasAnonymousProfilePicture=board.HasProfilePicture,
                            FullName=board.FullName,
                            ProfilePicUrl=board.ProfilePicUrl,
                            SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                            SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,                            
                            Type = "LangKeyBoard".FromResourceDictionary()
                        });

                    DbAccountService.Add(new InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        BoardDescription = board.BoardDescription,
                        BoardUrl = board.BoardUrl,
                        BoardName = board.BoardName,
                        InteractedUsername = board.UserName,
                        InteractedUserId = board.UserId,
                        FollowersCount = board.FollowersCount,
                        HasAnonymousProfilePicture=board.HasProfilePicture,
                        FullName=board.FullName,
                        ProfilePicUrl=board.ProfilePicUrl,
                        PinsCount = board.PinsCount,
                        Type = "LangKeyBoard".FromResourceDictionary()
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}