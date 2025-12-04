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

namespace PinDominatorCore.PDLibrary.Process
{
    internal class TryProcess : PdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDelayService _delayService;
        public TryModel TryModel { get; set; }
        private int UserCountToComment { get; set; }
        private readonly IDbCampaignService _dbCampaignService;
        private int _activityFailedCount = 1;

        public TryProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager,
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess,
            IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _delayService = delayService;
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            TryModel = JsonConvert.DeserializeObject<TryModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);

            InitializePerDayCount();
            _dbCampaignService = dbCampaignService;
        }
        
        private void InitializePerDayCount()
        {
            UserCountToComment = TryModel.Tries.GetRandom();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var pin = (PinterestPin) scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);
            
            var jobProcessResult = new JobProcessResult();

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var msgList = new List<string>();
                foreach (var item in TryModel.LstDisplayManageNoteModel)
                    if (item.SelectedQuery.Any(
                        x => x.Content.QueryValue.ToString() == scrapeResult.QueryInfo.QueryValue))
                        msgList.Add(item.TryText);
                if (msgList.Count == 0)
                {
                    jobProcessResult.IsProcessCompleted = true;
                    return jobProcessResult;
                }

                var noteModel = TryModel.LstDisplayManageNoteModel.GetRandomItem();

                var pinterestPin = (PinterestPin) scrapeResult.ResultPost;

                if (TryModel.IsMakeNoteAsSpinText)
                    noteModel.TryText = SpinTexHelper.GetSpinText(noteModel.TryText);

                 var response = PinFunct.TryPin(noteModel.MediaPath, noteModel.TryText, pinterestPin.PinId, DominatorAccountModel);
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);

                    IncrementCounters();

                    AfterTryAddDataToDataBase(scrapeResult);
                    if (TryModel.IsChkFollowUserAfterTry || TryModel.ChkTryUsersLatestPinsChecked)
                        PostTryProcess(pinterestPin);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (TryModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == TryModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {TryModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(TryModel.FailedActivityReschedule);
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
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

            return jobProcessResult;
        }

        public void PostTryProcess(PinterestPin pinterestPin)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyStartedAfterSomeAction".FromResourceDictionary(),"LangKeyTry".FromResourceDictionary()));

                #region Follow user after Try action

                if (TryModel.IsChkFollowUserAfterTry)
                    FollowUser(pinterestPin);

                #endregion

                #region Try User's Latest Pins after Try process

                if (TryModel.ChkTryUsersLatestPinsChecked)
                    TryUsersLatestPins(pinterestPin);

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
                    DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyFollow".FromResourceDictionary(),"LangKeyTry".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(),20, "LangkeyFollowProcess".FromResourceDictionary()));
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

        private void TryUsersLatestPins(PinterestPin pinterestPin)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(),"LangKeyTry".FromResourceDictionary(), "LangKeyTry".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(),"LangKeyTryProcess".FromResourceDictionary()));
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

                var currentNoteCount = 0;
                var pinNoteCount = TryModel.TriesPerUser.GetRandom();

                if (IsPostTryProcessCompleted())
                    return;
                
                var hasNoMorePosts = false;
                var bookMark = (string)null;
                TryModel.LstNotes = Regex.Split(TryModel.Message, @"\r\n").ToList();

                var isScroll = false;
                var scroll = 0;
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
                        var userPinDetails = PinFunct.GetPinsFromSpecificUser(pinOwner, DominatorAccountModel, bookMark,NeedCommentCount: TryModel.PostFilterModel.FilterComments);
                        lstPins = userPinDetails.LstUserPin;
                        bookMark = userPinDetails.BookMark;
                    }

                    foreach (var pin in lstPins)
                    {
                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var moduleSetting =
                            jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                        
                        var note = TryModel.LstNotes.GetRandomItem<string>();

                        TryResponse tryResponse = null;
                        if (DominatorAccountModel.IsRunProcessThroughBrowser)
                            tryResponse = PdLogInProcess.BrowserManager.Try(DominatorAccountModel, pinterestPin.PinId,
                               note, TryModel.MediaPath, JobCancellationTokenSource);
                        else
                            tryResponse = PinFunct.TryPin(TryModel.MediaPath, note, pinterestPin.PinId, DominatorAccountModel);

                        if (tryResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                string.Format("LangKeySuccessfullyTriedPin".FromResourceDictionary(),pinterestPin.PinId));

                            if (moduleSetting.IsTemplateMadeByCampaignMode)
                            {
                                _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts()
                                {
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    MediaType = MediaType.Image,
                                    OperationType = "Post" + ActivityType.Try,
                                    PinId = pin.PinId,
                                    Username = pinOwner,
                                    PinDescription = pin.Description,
                                    SourceBoard = pin.BoardUrl,
                                    SourceBoardName = pin.BoardName,
                                    PinWebUrl = pin.PinWebUrl,
                                    CommentCount = pin.CommentCount,
                                    SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                    SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                });
                            }
                            DbAccountService.Add(new InteractedPosts()
                            {
                                InteractionDate = DateTimeUtilities.GetEpochTime(),
                                MediaType = MediaType.Image,
                                OperationType = "Post" + ActivityType.Try,
                                PinId = pin.PinId,
                                Username = pinOwner,
                                PinDescription = pin.Description,
                                SourceBoard = pin.BoardUrl,
                                SourceBoardName = pin.BoardName,
                                PinWebUrl = pin.PinWebUrl,
                                CommentCount = pin.CommentCount
                            });
                            
                            currentNoteCount++;
                        }

                        if (IsPostTryProcessCompleted() ||
                            lstPins.Count < pinNoteCount || pinNoteCount <= currentNoteCount)
                        {
                            hasNoMorePosts = true;
                            return;
                        }

                        var delay = TryModel.DelayBetweenTriesForAfterActivity.GetRandom();
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsPostTryProcessCompleted()
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (moduleSetting.IsTemplateMadeByCampaignMode)
            {
                var lstInteractedPosts = _dbCampaignService.GetInteractedPosts("Post" + ActivityType.Try).ToList();
                if (UserCountToComment <= lstInteractedPosts.Count)
                    return true;
            }
            else
            {
                var lstInteractedPosts = DbAccountService.GetInteractedPosts("Post" + ActivityType.Try).ToList();
                if (UserCountToComment <= lstInteractedPosts.Count)
                    return true;
            }

            return false;
        }

        private void AfterTryAddDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var pin = (PinterestPin) scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                    {
                        OperationType = ActivityType.ToString(),
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        SourceBoard = pin.BoardUrl,
                        SourceBoardName = pin.BoardName,
                        CommentCount = pin.CommentCount,
                        MediaString = pin.MediaString,
                        MediaType = pin.MediaType,
                        PinDescription = pin.Description,
                        PinId = pin.PinId,
                        PinWebUrl = pin.PinWebUrl,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        TryCount = pin.NoOfTried,
                        UserId = pin.User.UserId,
                        Username = pin.User.Username
                    });
                dbAccountService.Add(new InteractedPosts
                {
                    OperationType = ActivityType.ToString(),
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    SourceBoard = pin.BoardUrl,
                    SourceBoardName = pin.BoardName,
                    CommentCount = pin.CommentCount,
                    MediaString = pin.MediaString,
                    MediaType = pin.MediaType,
                    PinDescription = pin.Description,
                    PinId = pin.PinId,
                    PinWebUrl = pin.PinWebUrl,
                    TryCount = pin.NoOfTried,
                    UserId = DominatorAccountModel.AccountBaseModel.UserId,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}