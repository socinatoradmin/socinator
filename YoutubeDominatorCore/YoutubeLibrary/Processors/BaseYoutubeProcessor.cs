using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.DatabaseHandler.YdTables.Campaign;
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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.EngageModel;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;
using YoutubeDominatorCore.YoutubeModels.WatchVideoModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors
{
    public abstract class BaseYoutubeProcessor : IQueryProcessor
    {
        private static readonly ConcurrentDictionary<string, object> LockObjects =
            new ConcurrentDictionary<string, object>();

        private readonly IBlackWhiteListHandler _blackWhiteListHandler;
        protected readonly IYdBrowserManager BrowserManager;
        private readonly CampaignDetails _campaignModel;
        protected readonly IDbCampaignService CampaignService;
        private readonly int _continueousFailedCount;
        protected readonly IDbAccountService DbAccountService;
        protected readonly IYdJobProcess JobProcess;
        protected readonly TemplateModel TemplateModel;
        protected readonly IYoutubeFunctionality YoutubeFunction;

        private string _lastPostChannelId;
        protected List<string> UniqueChannelsList;
        protected List<string> UniquePostsList;

        protected BaseYoutubeProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, IYoutubeFunctionality youtubeFunctionality)
        {
            try
            {
                JobProcess = jobProcess;

                DbAccountService = ((YdJobProcess)jobProcess).DbAccountService;
                CampaignService = campaignService;

                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    BrowserManager = jobProcess.BrowserManager;
                else
                    YoutubeFunction = youtubeFunctionality;

                jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleModeSetting =
                    jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActType];
                if (moduleModeSetting == null)
                    return;

                if (BlackListHandlerShouldBeInitialized)
                    _blackWhiteListHandler = blackWhiteListHandler;

                if (!string.IsNullOrEmpty(JobProcess?.TemplateId) &&
                    (JobProcess.ActivityType == ActivityType.ViewIncreaser ||
                     JobProcess.ActivityType == ActivityType.UnSubscribe))
                    TemplateModel = InstanceProvider.GetInstance<ITemplatesFileManager>()
                        .GetTemplateById(JobProcess.TemplateId);

                if (ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                      ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    _campaignModel = InstanceProvider.GetInstance<ICampaignsFileManager>()
                        .GetCampaignById(JobProcess.CampaignId);

                var ytSetting =
                    InstanceProvider.GetInstance<IGenericFileManager>()
                        .GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile()) ?? new YoutubeModel();

                if (ytSetting.IsCheckActivitiesOnNPost)
                    LimitCountForThisQuery = ytSetting.ActivitiesOnNPost.GetRandom();
                _continueousFailedCount = ytSetting.StopRunQueryIfNActivityFailed;

                SetActivatedChannelFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected List<Func<YoutubePost, bool>> FilterPostActionList { get; set; }
        protected List<Func<YoutubeChannel, bool>> FilterChannelActionList { get; set; }

        protected int FilterOrFailed { get; set; }
        protected int? LimitCountForThisQuery { get; set; }
        protected string SearchFilterUrlParam { get; set; }

        protected ActivityType ActType => JobProcess.ActivityType;
        protected YdModuleSetting ModuleSetting => JobProcess.ModuleSetting;

        private bool BlackListHandlerShouldBeInitialized =>
            JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers &&
            (JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList ||
             JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUsePrivateWhiteList)
            || JobProcess.ModuleSetting.SkipBlacklist.IsSkipBlackListUsers &&
            (JobProcess.ModuleSetting.SkipBlacklist.IsSkipGroupBlackListUsers ||
             JobProcess.ModuleSetting.SkipBlacklist.IsSkipPrivateBlackListUser);

        public void Start(QueryInfo queryInfo)
        {
            try
            {
#if DEBUG
                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Thread.CurrentThread.Name =
                        JobProcess.DominatorAccountModel.UserName + DateTime.Now.GetCurrentEpochTime();
#endif

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (LimitCountForThisQuery != null)
                    CustomLog(
                        $"Got random activity limit count as {LimitCountForThisQuery} for this running query [{queryInfo.QueryType} {queryInfo.QueryValue}]");

                if (!string.IsNullOrEmpty(queryInfo.QueryType))
                    CustomLog($"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");

                SwitchAccountChannel();

                FilterOrFailed = 0;
                Process(queryInfo);
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
                CloseSubBrowser();
            }
        }

        protected abstract void Process(QueryInfo quexryInfo);

        protected void StartFinalProcess(ref JobProcessResult jobProcessResult, QueryInfo queryInfo,
            YoutubeChannel channel = null, YoutubePost post = null)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultChannel = channel,
                ResultPost = post,
                QueryInfo = queryInfo
            });

            if (jobProcessResult.IsProcessSuceessfull)
            {
                if (LimitCountForThisQuery != null && ActType != ActivityType.LikeComment)
                    --LimitCountForThisQuery;
                FilterOrFailed = 0;
            }
            else
            {
                FilterOrFailed++;
            }
        }

        protected void CloseSubBrowser(bool closeMainBrowser = false)
        {
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser &&
                (closeMainBrowser || BrowserManager.BrowserWindow != null))
            {
                BrowserManager.SetVideoQuality = false;
                BrowserManager.CloseBrowser();
            }
        }

        private void SwitchAccountChannel()
        {
            try
            {
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var pageNameAndId = "";

                switch (ActType)
                {
                    case ActivityType.Like:
                        var likeModel = JsonConvert.DeserializeObject<LikeModel>(templatesFileManager.Get()
                            .FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = likeModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        SearchFilterUrlParam =
                            YdStatic.SearchUrlParamFromFilter(likeModel.VideoFilterModel.SearchVideoFilterModel);
                        break;
                    case ActivityType.Dislike:
                        var disLikeModel = JsonConvert.DeserializeObject<DislikeModel>(templatesFileManager.Get()
                            .FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = disLikeModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        SearchFilterUrlParam =
                            YdStatic.SearchUrlParamFromFilter(disLikeModel.VideoFilterModel.SearchVideoFilterModel);
                        break;
                    case ActivityType.LikeComment:
                        var likeCommentModel = JsonConvert.DeserializeObject<LikeCommentModel>(templatesFileManager
                            .Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = likeCommentModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        SearchFilterUrlParam =
                            YdStatic.SearchUrlParamFromFilter(likeCommentModel.VideoFilterModel.SearchVideoFilterModel);
                        break;

                    case ActivityType.Comment:
                        var commentModel = JsonConvert.DeserializeObject<CommentModel>(templatesFileManager.Get()
                            .FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = commentModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        SearchFilterUrlParam =
                            YdStatic.SearchUrlParamFromFilter(commentModel.VideoFilterModel.SearchVideoFilterModel);
                        break;

                    case ActivityType.Subscribe:
                        var subscribeModel = JsonConvert.DeserializeObject<SubscribeModel>(templatesFileManager.Get()
                            .FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = subscribeModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        break;

                    case ActivityType.UnSubscribe:
                        var unSubscribeModel = JsonConvert.DeserializeObject<UnsubscribeModel>(templatesFileManager
                            .Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = unSubscribeModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        break;

                    case ActivityType.ViewIncreaser:
                        var viewIncreaserModel = JsonConvert.DeserializeObject<ViewIncreaserModel>(templatesFileManager
                            .Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                        //if (viewIncreaserModel.MozillaSelected)
                        //    (YoutubeFunction ?? new YoutubeFunctionality(null, null)).DownloadGeckoDriver(JobProcess.DominatorAccountModel);

                        pageNameAndId = viewIncreaserModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;

                        SearchFilterUrlParam =
                            YdStatic.SearchUrlParamFromFilter(
                                viewIncreaserModel.VideoFilterModel.SearchVideoFilterModel);
                        break;

                    case ActivityType.ChannelScraper:
                        var channelScraperModel = JsonConvert.DeserializeObject<ChannelScraperModel>(
                            templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)
                                ?.ActivitySettings);
                        pageNameAndId = channelScraperModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        break;

                    case ActivityType.PostScraper:
                        var postScraperModel = JsonConvert.DeserializeObject<PostScraperModel>(templatesFileManager
                            .Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = postScraperModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        SearchFilterUrlParam =
                            YdStatic.SearchUrlParamFromFilter(postScraperModel.VideoFilterModel.SearchVideoFilterModel);
                        break;
                    case ActivityType.CommentScraper:
                        var commentScraperModel = JsonConvert.DeserializeObject<CommentScraperModel>(templatesFileManager
                            .Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                        pageNameAndId = commentScraperModel.ListSelectDestination
                            .FirstOrDefault(x =>
                                x.AccountId == JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                            ?.SelectedChannel;
                        break;
                }

                if (pageNameAndId == YdStatic.DefaultChannel)
                    pageNameAndId = YdStatic.GetDefaultChannel;

                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    BrowserManager.SwitchChannel(pageNameAndId, 1);
                else
                    YoutubeFunction.SwitchChannel(JobProcess.DominatorAccountModel, pageNameAndId, 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     taking ActivityType:QueryType:QueryValue combination as a key
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="paginationId"></param>
        /// <param name="isToAddPagination"></param>
        /// <param name="currentUser"></param>
        protected void AddOrUpdatePaginationId(QueryInfo queryInfo, string paginationId)
        {
            try
            {
                if (string.IsNullOrEmpty(paginationId))
                    return;

                var queryCombination =
                    $"{JobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{JobProcess.DominatorAccountModel.UserName}:{queryInfo.Id}";
                SocinatorAccountBuilder.Instance(JobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdatePaginationId(queryCombination, paginationId).SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected string GetPaginationId(QueryInfo queryInfo)
        {
            var paginationId = string.Empty;
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var dominatorAccountModel =
                    accountsFileManager.GetAccountById(JobProcess.DominatorAccountModel.AccountId);

                var queryCombination =
                    $"{JobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{JobProcess.DominatorAccountModel.UserName}:{queryInfo.Id}";

                dominatorAccountModel.PaginationId.TryGetValue(queryCombination, out paginationId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return paginationId;
        }

        protected void SetActivatedChannelFilter()
        {
            try
            {
                var objYoutubeChannel = new ScrapeFilter.ChannelFilterModel(ModuleSetting);

                if (ModuleSetting.ChannelFilterModel.IsViewsCountChecked)
                    (FilterChannelActionList ?? (FilterChannelActionList = new List<Func<YoutubeChannel, bool>>()))
                        .Add(objYoutubeChannel.FilterViewCount);

                if (ModuleSetting.ChannelFilterModel.IsSubscribersCountChecked)
                    (FilterChannelActionList ?? (FilterChannelActionList = new List<Func<YoutubeChannel, bool>>()))
                        .Add(objYoutubeChannel.FilterSubscribeCount);

                if (ModuleSetting.ChannelFilterModel.IsTitleShouldContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.ChannelFilterModel.TitleShouldContainsWordPhrase))
                    (FilterChannelActionList ?? (FilterChannelActionList = new List<Func<YoutubeChannel, bool>>()))
                        .Add(objYoutubeChannel.FilterTitleShouldContains);

                if (ModuleSetting.ChannelFilterModel.IsTitleShouldNotContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.ChannelFilterModel.TitleShouldNotContainsWordPhrase))
                    (FilterChannelActionList ?? (FilterChannelActionList = new List<Func<YoutubeChannel, bool>>()))
                        .Add(objYoutubeChannel.FilterTitleShouldNotContains);

                if (ModuleSetting.ChannelFilterModel.IsDescriptionShouldContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.ChannelFilterModel.DescriptionShouldContainsWordPhrase))
                    (FilterChannelActionList ?? (FilterChannelActionList = new List<Func<YoutubeChannel, bool>>()))
                        .Add(objYoutubeChannel.FilterDescriptionShouldContains);

                if (ModuleSetting.ChannelFilterModel.IsDescriptionShouldNotContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.ChannelFilterModel.DescriptionShouldNotContainsWordPhrase))
                    (FilterChannelActionList ?? (FilterChannelActionList = new List<Func<YoutubeChannel, bool>>()))
                        .Add(objYoutubeChannel.FilterDescriptionShouldNotContains);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool FilterChannelApply(YoutubeChannel youtubeChannel, int numberOfScrapedResults)
        {
            var filtered = false;

            if (youtubeChannel == null)
                return true;

            if (FilterChannelActionList?.Count > 0)
                //CustomLog("Filtering channel => " + youtubeChannel.ChannelId);

                foreach (var filterMethod in FilterChannelActionList)
                    try
                    {
                        if (filterMethod(youtubeChannel))
                        {
                            FilterOrFailed++;
                            filtered = true;
                            //GlobusLogHelper.log.Info(Log.FilterApplied,
                            //    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            //    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActType,
                            //    numberOfScrapedResults);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

            return filtered;
        }

        protected bool ReturnIfFilterFailedOrLimitReached(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (FilterOrFailed >= _continueousFailedCount)
            {
                jobProcessResult.IsProcessCompleted = true;
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActType,
                    $"Successfully Continuous \nFilter/Failed count {_continueousFailedCount}.Also per account limit reached on Query [{queryInfo.QueryType} {queryInfo.QueryValue}] Searching For Next Query If Any...");
                return true;
            }

            if (LimitCountForThisQuery == 0)
            {
                jobProcessResult.IsProcessCompleted = true;

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActType,
                    $"\nActivity on posts per account limit reached on Query [{queryInfo.QueryType} {queryInfo.QueryValue}] " +
                    "\nSearching For Next Query If Any...\n");
                return true;
            }

            return false;
        }

        protected List<T> SkipBlackListOrWhiteList<T>(List<T> lstTagDetails)
        {
            if (_blackWhiteListHandler != null)
            {
                if (JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers &&
                    (JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList ||
                     JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUsePrivateWhiteList))
                    lstTagDetails = _blackWhiteListHandler.SkipWhiteListUsers(lstTagDetails);

                if (JobProcess.ModuleSetting.SkipBlacklist.IsSkipBlackListUsers &&
                    (JobProcess.ModuleSetting.SkipBlacklist.IsSkipGroupBlackListUsers ||
                     JobProcess.ModuleSetting.SkipBlacklist.IsSkipPrivateBlackListUser))
                    lstTagDetails = _blackWhiteListHandler.SkipBlackListUsers(lstTagDetails);
            }

            return lstTagDetails;
        }

        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink)
        {
            var actType = ActType.ToString();

            if (_campaignModel != null)
                try
                {
                    JobProcess.AddedToDb = false;

                    #region Action From Random Percentage Of Accounts

                    if (ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                        lock (lockObject)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(_campaignModel.CampaignId, SocialNetworks.YouTube,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                decimal count = _campaignModel.SelectedAccountList.Count;
                                var randomMaxAccountToPerform = (int)Math.Round(
                                    count * ModuleSetting.PerformActionFromRandomPercentage.GetRandom() / 100);

                                var numberOfAccountsAlreadyPerformedAction = CampaignService.GetAllInteractedPosts()
                                    .Where(x => x.ActivityType == actType && x.VideoUrl == postPermalink).ToList();

                                if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction.Count)
                                    return false;

                                AddActivityValueToDb(queryInfo, postPermalink, dbOperation, "Pending");
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
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(_campaignModel.CampaignId, SocialNetworks.YouTube,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                List<int> recentlyPerformedActions;
                                recentlyPerformedActions = CampaignService.GetAllInteractedPosts()
                                    .Where(x => x.ActivityType == actType && x.VideoUrl == postPermalink &&
                                                (x.Status == "Success" || x.Status == "Working"))
                                    .OrderByDescending(x => x.InteractionTimeStamp).Select(x => x.InteractionTimeStamp)
                                    .Take(1).ToList();

                                if (recentlyPerformedActions.Count > 0)
                                {
                                    var recentlyPerformedTime = recentlyPerformedActions[0];
                                    var delay = ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                                    var time = DateTimeUtilities.GetEpochTime();
                                    var time2 = recentlyPerformedTime + delay;
                                    if (time < time2)
                                    {
                                        var sleepTime = time2 - time;
                                        CustomLog(
                                            $"waiting for {sleepTime} seconds as last activity on the post[{postPermalink}] was on {recentlyPerformedTime.EpochToDateTimeLocal()} in the campaign [{_campaignModel.CampaignName}]");
                                        Task.Delay(TimeSpan.FromSeconds(sleepTime))
                                            .Wait(JobProcess.JobCancellationTokenSource.Token);
                                    }
                                }

                                if (!JobProcess.AddedToDb)
                                {
                                    AddActivityValueToDb(queryInfo, postPermalink, dbOperation, "Working");
                                }
                                else
                                {
                                    var interactedPost = dbOperation.GetSingle<InteractedPosts>(
                                        x => x.VideoUrl == postPermalink && x.ActivityType == actType &&
                                             x.AccountUsername == JobProcess.DominatorAccountModel.AccountBaseModel
                                                 .UserName && (x.Status == "Pending" || x.Status == "Working"));
                                    interactedPost.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
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

            return true;
        }

        protected void AddActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation,
            string status)
        {
            var existing = dbOperation.GetSingle<InteractedPosts>(x =>
                x.AccountUsername == JobProcess.DominatorAccountModel.AccountBaseModel.UserName &&
                x.VideoUrl == postPermalink);
            var existed = existing != null;
            if (!existed)
                existing = new InteractedPosts();

            existing.ActivityType = ActType.ToString();
            existing.QueryType = queryInfo.QueryType;
            existing.QueryValue = queryInfo.QueryValue;
            existing.AccountUsername = JobProcess.DominatorAccountModel.AccountBaseModel.UserName;
            existing.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
            existing.VideoUrl = postPermalink;
            existing.Status = status;

            if (existed)
                dbOperation.Update(existing);
            else
                dbOperation.Add(existing);
        }

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, YoutubePost post, QueryInfo queryInfo)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (ModuleSetting.IschkUniquePostForCampaign)
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.YouTube, $"{JobProcess.CampaignId}.post", post.Code);
                    }
                    catch (Exception)
                    {
                        if (UniquePostsList == null)
                            UniquePostsList = new List<string>();
                        UniquePostsList.Add(post.Code);
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

                if (ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.YouTube, JobProcess.CampaignId, post.ChannelId);
                    }
                    catch (Exception)
                    {
                        if (UniqueChannelsList == null)
                            UniqueChannelsList = new List<string>();
                        UniqueChannelsList.Add(post.ChannelId);

                        if (_lastPostChannelId != post.ChannelId)
                            CustomLog(
                                $"Skipping posts from the channel [{post.ChannelId}], as Unique setting is on. QueryInfo: [{queryInfo.QueryType} : {queryInfo.QueryValue}]");
                        _lastPostChannelId = post.ChannelId;
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }

            if (ModuleSetting.IschkUniqueUserForAccount)
                try
                {
                    if (DbAccountService.GetInteractedPosts(ActType).Where(x => x.ChannelId == post.ChannelId).Any())
                    {
                        if (UniqueChannelsList == null)
                            UniqueChannelsList = new List<string>();
                        UniqueChannelsList.Add(post.ChannelId);

                        if (_lastPostChannelId != post.ChannelId)
                            CustomLog(
                                $"Skipping posts from the channel [{post.ChannelId}], as Unique setting is on. QueryInfo: [{queryInfo.QueryType} : {queryInfo.QueryValue}]");
                        _lastPostChannelId = post.ChannelId;

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return true;
        }

        protected bool SkipTheUniqueOne(QueryInfo queryInfo, string videoId = null, string channelId = null)
        {
            var returnValue = false;
            if (!string.IsNullOrEmpty(channelId) && UniqueChannelsList != null &&
                UniqueChannelsList.Contains(channelId))
                returnValue = true;

            if (!string.IsNullOrEmpty(videoId) && UniquePostsList != null && UniquePostsList.Contains(videoId))
            {
                CustomLog(
                    $"Skipping the post [{videoId}], as Unique setting is on. QueryInfo: [{queryInfo.QueryType} : {queryInfo.QueryValue}]");
                returnValue = true;
            }

            return returnValue;
        }

        protected string PostUrlWithCommentId(string urlBefore)
        {
            if (urlBefore.Contains("//youtu.be/"))
                urlBefore = urlBefore.StringMatches(@"(youtu.be/)(.*)")[0].ToString().Replace("youtu.be/", "");

            var url = urlBefore.Contains("outube.com") ? urlBefore : $"https://www.youtube.com/watch?v={urlBefore}";

            //if (!url.ToLower().Contains("&lc="))
            //    url = $"{url}&lc=Nothing";
            return url;
        }

        protected bool CommnetsRequiredAndCommentDisabled(YoutubePost post, bool isCustom)
        {
            if ((ActType == ActivityType.Comment || ActType == ActivityType.LikeComment ||
                 ActType == ActivityType.CommentScraper || ActType == ActivityType.Subscribe) && !post.CommentEnabled)
            {
                var extraMsg = ActType == ActivityType.LikeComment ? " Can't like any comment." :
                    ActType == ActivityType.Subscribe ? " Can't subscribe any commenter." : "";
                CustomLog($"Skipping Url ({post.PostUrl}) : Comments are disabled for this video.{extraMsg}");
                if (isCustom)
                    JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                return true;
            }

            return false;
        }


        /// <summary>
        ///     Got no data after search
        /// </summary>
        /// <param name="jobProcessResult"></param>
        protected void NoData(ref JobProcessResult jobProcessResult)
        {
            NoMoreDataLog();
            jobProcessResult.HasNoResult = true;
            jobProcessResult.maxId = null;
            jobProcessResult.IsProcessCompleted = false;
            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        protected void CustomLog(string message)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, message);
        }

        protected void NoMoreDataLog()
        {
            GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType);
        }

        protected void FoundXResultLog(QueryInfo queryInfo, int xCount)
        {
            GlobusLogHelper.log.Info(Log.FoundXResults,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, xCount, queryInfo.QueryType,
                queryInfo.QueryValue ?? string.Empty, ActType);
        }
    }
}