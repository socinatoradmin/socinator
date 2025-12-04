using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
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
using InteractedUsers = DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers;
using PinDominatorCore.Response;
using PinDominatorCore.PDUtility;
using DominatorHouseCore.Interfaces;

namespace PinDominatorCore.PDLibrary.Process
{
    public class CommentProcess : PdJobProcessInteracted<InteractedPosts>
    {
        private readonly CommentModel _commentModel;
        private readonly IDelayService _delayService;
        private readonly IDbCampaignService _dbCampaignService;
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly PinterestOtherConfigModel _pinterestOtherConfigModel;
        private int _activityFailedCount = 1;

        public CommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager, IJobActivityConfigurationManager jobActivityConfigurationManager,
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess,
            IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _delayService = delayService;
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            _commentModel = JsonConvert.DeserializeObject<CommentModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);
            InitializePerDayAndPercentageCount();
            _dbCampaignService = dbCampaignService;
            _accountServiceScoped = accountServiceScoped;
            _pinterestOtherConfigModel = processScopeModel.GetActivitySettingsAs<PinterestOtherConfigModel>();

            var moduleSettings = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
            if (moduleSettings.IsTemplateMadeByCampaignMode && _commentModel.IsUniqueComment && !string.IsNullOrEmpty(CampaignId))
            {
                try
                {
                    lock (PdStatic.LockInitializingCommentLockDict)
                    {
                        if (!PdStatic.LockUniqueCommentsDict.ContainsKey(CampaignId))
                            PdStatic.LockUniqueCommentsDict[CampaignId] = new object();

                        if (_commentModel.IsPostUniqueCommentFromEachAccount)
                        {
                            if (!PdStatic.UniqueCommentsListWithPinId.ContainsKey(CampaignId))
                                PdStatic.UniqueCommentsListWithPinId[CampaignId] = new Dictionary<string, List<string>>();

                            if (!PdStatic.FirstTimeUniqueCommentListFromDb.ContainsKey(CampaignId))
                                PdStatic.FirstTimeUniqueCommentListFromDb[CampaignId] = new Dictionary<string, bool>();
                        }
                        else
                        {
                            if (!PdStatic.UniqueCommentsListUniqueCmntUniqueAccount.ContainsKey(CampaignId))
                                PdStatic.UniqueCommentsListUniqueCmntUniqueAccount[CampaignId] = new List<string>();

                            if (!PdStatic.FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount.ContainsKey(CampaignId))
                                PdStatic.FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount[CampaignId] = true;
                        }
                    }
                }
                catch (Exception ex)
                { ex.DebugLog(); }
            }
        }

        private int UserCountToComment { get; set; }
        private int LikeCountToComment { get; set; }
        public int CommentCountFromPercentage { get; set; }

        /// <summary>
        ///     Initializes per day count actions (like, comment, message) and percentages for individual campaigns
        /// </summary>
        private void InitializePerDayAndPercentageCount()
        {
            UserCountToComment = _commentModel.Comments.GetRandom();
            LikeCountToComment = _commentModel.Likes.GetRandom();
            if (_commentModel.IsChkCommentPercentage)
                // Calculates comments percentage from max job count
                CommentCountFromPercentage = Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                    _commentModel.CommentPercentage);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var pin = (PinterestPin)scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);

            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var commentList = new List<string>();
                foreach (var item in _commentModel.LstDisplayManageCommentModel)
                    if (item.SelectedQuery.Any(x => x.Content.QueryValue.ToString() == scrapeResult.QueryInfo.QueryValue
                                                    && x.Content.QueryType.ToString() ==
                                                    scrapeResult.QueryInfo.QueryType
                                                    || x.Content.QueryType == "Own Followers" ||
                                                    x.Content.QueryType == "Own Followings"
                                                    || x.Content.QueryType == "Newsfeed"))
                        commentList.Add(item.CommentText);

                if (_commentModel.IsChkAllowMultipleCommentsOnSamePost)
                {
                    var alreadyPostedComment = DbAccountService
                        .GetInteractedPostsWithSameQuery(ActivityType, scrapeResult.QueryInfo)
                        .Where(x => x.PinId == pin.PinId);
                    foreach (var item in alreadyPostedComment)
                        commentList.Remove(item.Comment);
                }

                if (commentList.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType
                        , "LangKeyMentionQueryTypeWithCommentMessage".FromResourceDictionary());
                    jobProcessResult.IsProcessCompleted = true;
                    return jobProcessResult;
                }
                var comment = commentList.GetRandomItem();
                var pinterestPin = (PinterestPin)scrapeResult.ResultPost;
                if (!string.IsNullOrEmpty(CampaignId) && _commentModel.IsUniqueComment)
                {
                    lock (PdStatic.LockUniqueCommentsDict[CampaignId])
                    {
                        if (_commentModel.IsPostUniqueCommentFromEachAccount)
                        {
                            if (!PdStatic.UniqueCommentsListWithPinId[CampaignId].ContainsKey(pinterestPin.PinId))
                                PdStatic.UniqueCommentsListWithPinId[CampaignId][pinterestPin.PinId] = new List<string>();

                            if (!PdStatic.FirstTimeUniqueCommentListFromDb[CampaignId].ContainsKey(pinterestPin.PinId))
                                PdStatic.FirstTimeUniqueCommentListFromDb[CampaignId][pinterestPin.PinId] = true;

                            // To get the list of unique comments on the pin
                            PdStatic.UnusedCommentsForUnique(DominatorAccountModel.AccountBaseModel.AccountId, CampaignId, pinterestPin.PinId, commentList, _dbCampaignService, _accountServiceScoped, _pinterestOtherConfigModel.IsCampainWiseUnique, _pinterestOtherConfigModel.IsAccountWiseUnique);
                            string selectedcomment = "";
                            PdStatic.RemoveUsedUniqueComment(ref selectedcomment, CampaignId, pinterestPin.PinId);
                            comment = selectedcomment;
                        }
                        else
                        {
                            // To get the list of unique comments from the account
                            commentList = PdStatic.UnusedCommentsForUniqueCommentFromUniqueUser(CampaignId, commentList, _dbCampaignService, _accountServiceScoped, _pinterestOtherConfigModel.IsCampainWiseUnique, _commentModel.IsChkAllowMultipleCommentsOnSamePost);
                            string Selectedcomment = "";
                            PdStatic.RemoveUsedUniqueCommentFromUniqueUser(ref Selectedcomment, CampaignId);
                            comment = Selectedcomment;
                        }
                    }
                }
                else
                {
                    string alreadyPostedwithSameComment = (DbAccountService.Get<InteractedPosts>(x =>
                        x.PinId == pin.PinId && x.Comment == comment).Where(y => y.PinId.ToLower().Contains(pin.Code.ToLower())))?.FirstOrDefault()?.Comment;
                    while (!string.IsNullOrEmpty(alreadyPostedwithSameComment))
                    {
                        commentList.Remove(alreadyPostedwithSameComment);
                        if (commentList.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube, DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType, $"Already commented \"{alreadyPostedwithSameComment}\" in same pin => {pin.PinId}");
                            return jobProcessResult;
                        }
                        alreadyPostedwithSameComment = (DbAccountService.Get<InteractedPosts>(x =>
                            x.PinId == pin.PinId && x.Comment == comment).Where(y => y.PinId.ToLower().Contains(pin.Code.ToLower())))?.FirstOrDefault()?.Comment;
                    }
                }
                if (_commentModel.IsMakeCommentAsSpinText)
                    comment = SpinTexHelper.GetSpinText(comment);
                if (string.IsNullOrEmpty(comment))
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, DominatorAccountModel.AccountBaseModel.UserName,
                        ActivityType, string.Format("LangKeySkipedPin".FromResourceDictionary(), pinterestPin.PinId));
                    if (!_commentModel.IsPostUniqueCommentFromEachAccount)
                    {
                        DominatorAccountModel.IsNeedToSchedule = false;
                        JobCancellationTokenSource.Cancel();
                    }
                    return jobProcessResult;
                }
                #region Comment Process.
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var response = PinFunct.CommentOnPin(pinterestPin.PinId, comment, DominatorAccountModel);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);

                    IncrementCounters();

                    AfterCommentAddDataToDataBase(scrapeResult, comment);
                    if (_commentModel.IsChkFollowUserAfterComment || _commentModel.ChkCommentOnUserLatestPostsChecked ||
                        _commentModel.ChkLikeOthersCommentChecked)
                        PostCommentProcess(pinterestPin);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (_commentModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == _commentModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                           DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                           $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                           $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {_commentModel.FailedActivityReschedule} " +
                           $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(_commentModel.FailedActivityReschedule);
                    }

                    jobProcessResult.IsProcessSuceessfull = false;
                }
                #endregion
                if (_commentModel != null && _commentModel.IsEnableAdvancedUserMode && _commentModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(_commentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                //ex.DebugLog();
            }

            return jobProcessResult;
        }
        public void PostCommentProcess(PinterestPin pinterestPin)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyStartedAfterSomeAction".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary()));

                #region Follow user after Comment action

                if (_commentModel.IsChkFollowUserAfterComment)
                    FollowUser(pinterestPin);

                #endregion

                #region Comment on User's Latest Pin after Comment process

                if (_commentModel.ChkCommentOnUserLatestPostsChecked)
                    CommentOnUsersLatestPins(pinterestPin);

                #endregion

                #region Like Other's Comment

                if (_commentModel.ChkLikeOthersCommentChecked)
                    LikeOthersComments(pinterestPin);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FollowUser(PinterestPin pinterestPin)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyFollow".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyFollowProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);

                string pinOwner;
                FriendshipsResponse response;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    pinOwner = PdLogInProcess.BrowserManager.SearchByCustomPin(DominatorAccountModel, pinterestPin.PinId, JobCancellationTokenSource)?.UserName;
                    response = PdLogInProcess.BrowserManager.FollowUser(DominatorAccountModel, pinOwner, JobCancellationTokenSource);
                }
                else
                {
                    pinOwner = PinFunct.PinOwner(pinterestPin.PinId, DominatorAccountModel);
                    response = PinFunct.FollowUserSingle(DominatorAccountModel, pinOwner);
                }

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyFollowedPinOwnerSuccessMessage".FromResourceDictionary(), pinOwner));

                    UserNameInfoPtResponseHandler userInfo;
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        userInfo = PdLogInProcess.BrowserManager.SearchByCustomUser(DominatorAccountModel, pinOwner, JobCancellationTokenSource);
                    else
                        userInfo = PinFunct.GetUserDetails(pinOwner, DominatorAccountModel).Result;

                    if (moduleSetting.IsTemplateMadeByCampaignMode)
                        _dbCampaignService.Add(new InteractedUsers
                        {
                            ActivityType = ActivityType.Follow.ToString(),
                            Bio = userInfo.Biography,
                            FollowersCount = userInfo.FollowerCount,
                            FollowingsCount = userInfo.FollowingCount,
                            FullName = userInfo.FullName,
                            HasAnonymousProfilePicture = userInfo.HasProfilePicture,
                            PinsCount = userInfo.PinsCount,
                            ProfilePicUrl = userInfo.ProfilePicUrl,
                            InteractedUsername = userInfo.Username,
                            InteractedUserId = userInfo.UserId,
                            InteractionTime = DateTimeUtilities.GetEpochTime(),
                            Website = userInfo.WebsiteUrl,
                            IsVerified = userInfo.IsVerified,
                            Type = "User",
                            SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                            SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                        });
                    DbAccountService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedUsers
                    {
                        ActivityType = ActivityType.Follow.ToString(),
                        Bio = userInfo.Biography,
                        FollowersCount = userInfo.FollowerCount,
                        FollowingsCount = userInfo.FollowingCount,
                        FullName = userInfo.FullName,
                        HasAnonymousProfilePicture = userInfo.HasProfilePicture,
                        PinsCount = userInfo.PinsCount,
                        ProfilePicUrl = userInfo.ProfilePicUrl,
                        InteractedUsername = userInfo.Username,
                        InteractedUserId = userInfo.UserId,
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        Website = userInfo.WebsiteUrl,
                        IsVerified = userInfo.IsVerified,
                        Type = "User"
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CommentOnUsersLatestPins(PinterestPin pinterestPin)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyCommentProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                _delayService.ThreadSleep(20 * 1000);

                string pinOwner;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    pinOwner = PdLogInProcess.BrowserManager.SearchByCustomPin(DominatorAccountModel, pinterestPin.PinId, JobCancellationTokenSource)?.UserName;
                    PdLogInProcess.BrowserManager.FollowUser(DominatorAccountModel, pinOwner, JobCancellationTokenSource);
                }
                else
                {
                    pinOwner = PinFunct.PinOwner(pinterestPin.PinId, DominatorAccountModel);
                    PinFunct.FollowUserSingle(DominatorAccountModel, pinOwner);
                }

                var currentCommentCount = 0;
                var pinCommentCount = _commentModel.Comments.GetRandom();

                if (IsPostCommentProcessCompleted("Post" + ActivityType.Comment))
                    return;

                var hasNoMorePosts = false;
                var bookMark = (string)null;
                _commentModel.LstComments = Regex.Split(_commentModel.Message, @"\r\n").ToList();

                bool isScroll = false;
                int scroll = 0;
                while (!hasNoMorePosts)
                {
                    var lstPins = new List<PinterestPin>();
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        lstPins = PdLogInProcess.BrowserManager.SearchPinsOfUser(DominatorAccountModel, pinOwner,
                               JobCancellationTokenSource, isScroll, scroll);
                        scroll = 1;
                        isScroll = true;
                        if (lstPins.Count == 0)
                            hasNoMorePosts = true;
                    }
                    else
                    {
                        var userPinDetails = PinFunct.GetPinsFromSpecificUser(pinOwner, DominatorAccountModel, bookMark,NeedCommentCount: _commentModel.PostFilterModel.FilterComments);
                        lstPins = userPinDetails.LstUserPin;
                        bookMark = userPinDetails.BookMark;
                    }

                    foreach (var pin in lstPins)
                    {
                        var comment = _commentModel.LstComments.GetRandomItem();
                        var commentResponse = PinFunct.CommentOnPin(pin.PinId, comment, DominatorAccountModel);

                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var moduleSetting =
                            jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                        if (moduleSetting == null)
                            return;

                        if (commentResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                string.Format("LangKeySuccessfullyCommentedPin".FromResourceDictionary(), pinterestPin.PinId));

                            if (moduleSetting.IsTemplateMadeByCampaignMode)

                                _dbCampaignService.Add(
                                    new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                                    {
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        MediaType = MediaType.Image,
                                        OperationType = "Post" + ActivityType.Comment,
                                        PinId = pin.PinId,
                                        Username = pinOwner,
                                        PinDescription = pin.Description,
                                        SourceBoard = pin.BoardUrl,
                                        SourceBoardName = pin.BoardName,
                                        PinWebUrl = pin.PinWebUrl,
                                        CommentCount = pin.CommentCount,
                                        Comment = comment,
                                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                    });
                            DbAccountService.Add(new InteractedPosts
                            {
                                InteractionDate = DateTimeUtilities.GetEpochTime(),
                                MediaType = MediaType.Image,
                                OperationType = "Post" + ActivityType.Comment,
                                PinId = pin.PinId,
                                Username = pinOwner,
                                PinDescription = pin.Description,
                                SourceBoard = pin.BoardUrl,
                                SourceBoardName = pin.BoardName,
                                PinWebUrl = pin.PinWebUrl,
                                CommentCount = pin.CommentCount,
                                Comment = comment
                            });

                            currentCommentCount++;
                        }

                        if (IsPostCommentProcessCompleted("Post" + ActivityType.Comment) ||
                            lstPins.Count < pinCommentCount || pinCommentCount <= currentCommentCount)
                        {
                            hasNoMorePosts = true;
                            return;
                        }

                        var delay = _commentModel.DelayBetweenCommentsForAfterActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment, delay);
                        _delayService.ThreadSleep(delay * 1000);
                    }

                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                        if (string.IsNullOrEmpty(bookMark) || bookMark.Contains("[]"))
                            break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void LikeOthersComments(PinterestPin pinterestPin)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyLikeComment".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyLikeCommentProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);
                if (IsPostCommentProcessCompleted(ActivityType.LikeComment.ToString()))
                    return;

                var hasNoMorePosts = false;
                var bookMark = (string)null;
                var currentCommentCount = 0;
                while (!hasNoMorePosts)
                {
                    var commentIds = PinFunct.GetCommentsOfPin(pinterestPin.PinId, DominatorAccountModel, bookMark);
                    var lstComments = commentIds.LstComments;
                    //filter my own comments.
                    var  Skipped = lstComments.RemoveAll(x => x.Commentor.Username == DominatorAccountModel.AccountBaseModel.ProfileId);
                    if(Skipped > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                $"Successfully Skipped {Skipped} Own Liked Posts.");
                    foreach (var comment in lstComments)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.UserName, ActivityType,
                                $"Trying To Like comment {comment.CommentId} of pin {pinterestPin.PinId}");
                        var pinOwner = PinFunct.PinOwner(pinterestPin.PinId, DominatorAccountModel);
                        var response = PinFunct.LikeSomeonesComment(comment.CommentId, pinterestPin.PinId, DominatorAccountModel);                      
                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var moduleSetting =
                            jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                        if (moduleSetting == null)
                            return;

                        if (response.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                $"Successfully Liked comment {comment.CommentId} of pin {pinterestPin.PinId}");

                            if (moduleSetting.IsTemplateMadeByCampaignMode)
                                _dbCampaignService.Add(
                                    new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                                    {
                                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                                        MediaType = MediaType.Image,
                                        OperationType = ActivityType.LikeComment.ToString(),
                                        PinId = pinterestPin.PinId,
                                        Username = pinOwner,
                                        PinDescription = pinterestPin.Description,
                                        SourceBoard = pinterestPin.BoardUrl,
                                        SourceBoardName = pinterestPin.BoardName,
                                        PinWebUrl = pinterestPin.PinWebUrl,
                                        CommentId = comment.CommentId,
                                        CommentCount = pinterestPin.CommentCount,
                                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                    });
                            DbAccountService.Add(new InteractedPosts
                            {
                                InteractionDate = DateTimeUtilities.GetEpochTime(),
                                MediaType = MediaType.Image,
                                OperationType = ActivityType.LikeComment.ToString(),
                                PinId = pinterestPin.PinId,
                                Username = pinOwner,
                                PinDescription = pinterestPin.Description,
                                SourceBoard = pinterestPin.BoardUrl,
                                CommentId = comment.CommentId,
                                SourceBoardName = pinterestPin.BoardName,
                                PinWebUrl = pinterestPin.PinWebUrl,
                                CommentCount = pinterestPin.CommentCount
                            });

                            currentCommentCount++;
                            if (IsPostCommentProcessCompleted(ActivityType.LikeComment.ToString()) &&
                                currentCommentCount >= LikeCountToComment)
                            {
                                hasNoMorePosts = true;
                                return;
                            }
                        }
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var delay = _commentModel.DelayBetweenLikeCommentsForAfterActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType.LikeComment, delay);
                        _delayService.ThreadSleep(delay * 1000);
                    }

                    bookMark = commentIds.BookMark;
                    if (string.IsNullOrEmpty(bookMark) || bookMark.Contains("-end-"))
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsPostCommentProcessCompleted(string activityType, string pinId = "")
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (moduleSetting.IsTemplateMadeByCampaignMode)
            {
                var actType = activityType;
                var actTypeInt = ((int)ActivityType.Try).ToString();
                var lstInteractedPostCommentss
                    = _dbCampaignService.Get<DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts>
                        (x => x.OperationType == actType || x.OperationType == actTypeInt && x.PinId == pinId);
                if (LikeCountToComment <= lstInteractedPostCommentss.Count)
                    return true;
            }
            else
            {
                var lstInteractedPosts =
                    _dbCampaignService.Get<InteractedPosts>
                        (x => x.OperationType == activityType);
                if (UserCountToComment <= lstInteractedPosts.Count)
                    return true;
            }

            return false;
        }

        private void AfterCommentAddDataToDataBase(ScrapeResultNew scrapeResult, string msg)
        {
            try
            {
                var pin = (PinterestPin)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                    {
                        OperationType = ActivityType.ToString(),
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Username = pin.User.Username,
                        SourceBoard = pin.BoardUrl,
                        SourceBoardName = pin.BoardName,
                        CommentCount = pin.CommentCount,
                        MediaType = pin.MediaType,
                        PinDescription = pin.Description,
                        PinId = pin.PinId,
                        PinWebUrl = pin.PinWebUrl,
                        UserId = pin.User.UserId,
                        TryCount = pin.NoOfTried,
                        MediaString = pin.MediaString,
                        Comment = msg,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                DbAccountService.Add(new InteractedPosts
                {
                    OperationType = ActivityType.ToString(),
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = pin.User.Username,
                    SourceBoard = pin.BoardUrl,
                    SourceBoardName = pin.BoardName,
                    CommentCount = pin.CommentCount,
                    MediaType = pin.MediaType,
                    PinDescription = pin.Description,
                    PinId = pin.PinId,
                    PinWebUrl = pin.PinWebUrl,
                    UserId = pin.User.UserId,
                    TryCount = pin.NoOfTried,
                    Comment = msg,
                    MediaString = pin.MediaString
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}