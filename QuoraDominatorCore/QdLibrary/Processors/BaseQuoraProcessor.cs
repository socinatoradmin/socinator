using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuoraDominatorCore.QdLibrary.Processors
{
    public abstract class BaseQuoraProcessor : IQueryProcessor
    {
        protected IQuoraBrowserManager _browser;
        protected readonly IDbCampaignService CampaignService;
        protected readonly IDbAccountService DbAccountService;
        protected readonly IDbGlobalService DbGlobalService;
        protected readonly IQdJobProcess JobProcess;
        protected List<string> Blacklistuser;
        protected List<string> PrivateBlacklistedUser;
        public IQuoraFunctions quoraFunct;
        protected List<string> Whitelistuser;
        public readonly UnfollowerModel _unFollowModel;
        public readonly FollowerModel followermodel;
        public readonly UpvotePostsModel upvotePostModel;
        public readonly DownvotePostModel downvotePostModel;
        public readonly QuestionFilterModel questionFilterModel;
        protected IProcessScopeModel ProcessScope;
        public IQdHttpHelper httpHelper;
        public readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public BaseQuoraProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions quoraFunctions, IProcessScopeModel processScopeModel)
        {
            quoraFunct = quoraFunctions;
            JobProcess = jobProcess;
            DbAccountService = dbAccountService;
            CampaignService = campaignService;
            DbGlobalService = globalService;
            _browser = browser;
            Blacklistuser = DbGlobalService.GetAllBlackListUsers()?.Select(x => x.UserName).ToList();
            Whitelistuser = DbGlobalService.GetAllWhiteListUsers()?.Select(x => x.UserName).ToList();
            PrivateBlacklistedUser = DbAccountService.GetPrivateBlacklist();
            CampaignDetails = processScopeModel.CampaignDetails;
            _unFollowModel = processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
            followermodel = processScopeModel.GetActivitySettingsAs<FollowerModel>();
            questionFilterModel = processScopeModel.GetActivitySettingsAs<QuestionFilterModel>();
            upvotePostModel = processScopeModel.GetActivitySettingsAs<UpvotePostsModel>();
            downvotePostModel = processScopeModel.GetActivitySettingsAs<DownvotePostModel>();
            ProcessScope = processScopeModel;
        }
        
        public static ConcurrentDictionary<string, object> LockObjects { get; } =
            new ConcurrentDictionary<string, object>();

        protected ActivityType ActivityType => JobProcess.ActivityType;
        public CampaignDetails CampaignDetails { get; set; } = new CampaignDetails();

        public void Start(QueryInfo queryInfo)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var jobProcessResult = new JobProcessResult();

                if (JobProcess.ActivityType != ActivityType.Unfollow)

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                Process(queryInfo, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();
            }
        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        protected bool AlreadyInteractedUser(QueryInfo queryInfo)
        {
            var isActivityDoneBefore = false;

            if (ActivityType == ActivityType.Unfollow)
            {
                var alreadyUsedUnfollowedUsers = DbAccountService.GetUnfollowedUsers();
                alreadyUsedUnfollowedUsers.ForEach(x =>
                {
                    if (queryInfo.QueryValue.Contains(x))
                        isActivityDoneBefore = true;
                });
            }
            else
            {
                var alreadyUsedInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).ToList();
                alreadyUsedInteractedUsers.ForEach(x =>
                {
                    if (queryInfo.QueryValue.Contains(x.InteractedUsername))
                        isActivityDoneBefore = true;
                });
            }

            return isActivityDoneBefore;
        }

        internal void StartAnswerForEachQuestion(QueryInfo queryInfo, List<string> questionList,
            ref JobProcessResult jobProcessResult)
        {
            var InteractedAnswes=DbAccountService.GetInteractedAnswers(ActivityType).ToList();
            var SkippedAnswers = 0;
            if(InteractedAnswes != null && (SkippedAnswers=questionList.RemoveAll(x=>InteractedAnswes.Any(y=>y.Accountusername==JobProcess.DominatorAccountModel.AccountBaseModel.UserName && y.AnswersUrl==x)))> 0 )
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"Skipped {SkippedAnswers} Interacted Answers");
            foreach (var questionUrl in questionList)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var objQuestionDetailsResponseHandler =
                        quoraFunct.QuestionDetails(JobProcess.DominatorAccountModel, questionUrl);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var quoraUser = new QuoraUser
                    {
                        Url = questionUrl
                    };
                    jobProcessResult =
                        FilterAndStartFinalProcessForAnswerOnQuestion(queryInfo, quoraUser,
                            objQuestionDetailsResponseHandler);
                    if (jobProcessResult.IsProcessCompleted)
                        break;
                }
                catch (OperationCanceledException)
                {
                    if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        internal JobProcessResult FilterAndStartFinalProcessForAnswerOnQuestion(QueryInfo queryInfo,
            QuoraUser quoraUser, QuestionDetailsResponseHandler questionresponsehandler)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!QuestionFilterApply(questionresponsehandler))
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                JobProcessResult jobProcessResult;
                FinalProcess(queryInfo, out jobProcessResult, quoraUser);
                return jobProcessResult;
            }
            else
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"filter not matched to {queryInfo.QueryValue}");

            return new JobProcessResult();
        }

        internal void FinalProcess(QueryInfo queryInfo, out JobProcessResult jobProcessResult, QuoraUser quoraUser)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultUser = quoraUser,
                QueryInfo = queryInfo
            });
        }


        protected static string GenerateTimeStamp()
        {
            var strGenerateTimeStamp = string.Empty;
            try
            {
                var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                strGenerateTimeStamp = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return strGenerateTimeStamp;
        }

        internal JobProcessResult FilterAndStartFinalProcess(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            List<string> lstUserNames)
        {
            List<string> interactedUser = null;
            var SkippedUserCount = 0;
            lstUserNames.Reverse();
            var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
            try
            {
                try
                {
                    if (ActivityType == ActivityType.Unfollow)
                        interactedUser = DbAccountService.GetUnfollowedUsers();
                    if (ActivityType == ActivityType.Unfollow && _unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked && !_unFollowModel.IsChkPeopleFollowedBySoftwareChecked)
                        interactedUser.AddRange(DbAccountService.GetInteractedUsers(ActivityType.Follow).Select(x => x.InteractedUsername).ToList());
                    else if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.UserScraper || ActivityType == ActivityType.ReportUsers)
                        interactedUser = DbAccountService.GetInteractedUsers(ActivityType).Select(x => x.InteractedUsername).ToList();
                    else if (ActivityType == ActivityType.BroadcastMessages)
                        interactedUser = DbAccountService.GetInteractedMessage().Select(z => z.Username).ToList();
                    if (interactedUser != null && interactedUser.Count > 0)
                        if ((SkippedUserCount = lstUserNames.RemoveAll(z => interactedUser.Any(y => y == z.Replace($"{QdConstants.HomePageUrl}/profile/", "")))) > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successfully Skipped {SkippedUserCount} Interacted User.");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                lstUserNames = FilterBlackListedUser(lstUserNames);
                lstUserNames = FilterWhiteListedUser(lstUserNames);
                foreach (var eachUsers in lstUserNames)
                {
                    var eachUser = eachUsers.Replace($"{QdConstants.HomePageUrl}/profile/", "");
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    try
                    {
                        try
                        {
                            var profileUrl = $"{QdConstants.HomePageUrl}/profile/{JobProcess.DominatorAccountModel.AccountBaseModel.UserFullName}";
                            if (ActivityType == ActivityType.Unfollow)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var currentUserInfo = !IsBrowser ? quoraFunct.UserInfo(JobProcess.DominatorAccountModel,profileUrl):
                                    _browser.GetUserInfo(JobProcess.DominatorAccountModel,profileUrl);
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                if (_unFollowModel.IsChkEnableAutoFollowUnfollowChecked)
                                    if (_unFollowModel.IsChkStopUnFollowToolWhenReachChecked && currentUserInfo.FollowingCount <= _unFollowModel.StopFollowToolWhenReachValue.GetRandom() ||
                                        _unFollowModel.IsChkWhenNoUsersToUnfollow && currentUserInfo.FollowingCount == 0)
                                        ChangeAccountRunningStatus(false, JobProcess.DominatorAccountModel.AccountId, ActivityType.Unfollow);
                                if (_unFollowModel.IsChkStartFollowWithoutStoppingUnfollow)
                                    ChangeAccountRunningStatus(true, JobProcess.DominatorAccountModel.AccountId, ActivityType.Follow);
                            }
                            else if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.SendMessageToFollower)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var currentUserInfo = !IsBrowser ?  quoraFunct.UserInfo(JobProcess.DominatorAccountModel,profileUrl) 
                                    :_browser.GetUserInfo(JobProcess.DominatorAccountModel,profileUrl,IsBrowser);
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                if (followermodel.IsChkEnableAutoFollowUnfollowChecked)
                                {
                                    if (followermodel.IsChkStopFollowToolWhenReachChecked &&
                                        currentUserInfo.FollowingCount >= followermodel.StopFollowToolWhenReach.GetRandom()
                                        || followermodel.IsChkFollowToolGetsTemporaryBlockedChecked)
                                        ChangeAccountRunningStatus(false, JobProcess.DominatorAccountModel.AccountId, ActivityType.Follow);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                       var userInfoResponseHandler = !IsBrowser ? quoraFunct.UserInfo(JobProcess.DominatorAccountModel, eachUser)
                                : _browser.GetUserInfo(JobProcess.DominatorAccountModel, $"{QdConstants.HomePageUrl}/profile/{eachUser}");
                        if (!userInfoResponseHandler.IsFollowing && ActivityType == ActivityType.Unfollow)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Already UnFollowed User {eachUser}");
                            continue;
                        }
                        if (userInfoResponseHandler.IsFollowing && ActivityType == ActivityType.Follow)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Already Followed User {eachUser}");
                            continue;
                        }
                        if (!CheckUserUniqueNess(jobProcessResult, eachUsers, ActivityType)) continue;
                        if (!ApplyCampaignLevelSettings(queryInfo, eachUsers)) continue;
                        userInfoResponseHandler.Username = string.IsNullOrEmpty(userInfoResponseHandler.Username) ? eachUser : userInfoResponseHandler.Username;
                        FilterAndStartFinalProcessForEachUser(queryInfo, out jobProcessResult, userInfoResponseHandler,
                                userInfoResponseHandler);
                    }
                    catch (OperationCanceledException)
                    {
                        if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser();
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    if (jobProcessResult.IsProcessCompleted)
                        break;
                }
            }
            finally { if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser(); jobProcessResult.IsProcessSuceessfull = true; }

            return jobProcessResult;
        }

        private void ChangeAccountRunningStatus(bool IsStart, string accountId, ActivityType activityType)
        {
            try
            {
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                dominatorScheduler.ChangeAccountsRunningStatus(IsStart,accountId,activityType);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch(Exception ex) { ex.DebugLog(); }
        }

        private List<string> FilterWhiteListedUser(List<string> lstUserNames)
        {
            try
            {
                if (Whitelistuser != null && Whitelistuser.Count > 0)
                    if(lstUserNames.RemoveAll(x => Whitelistuser.Any(y => y == x)) > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successfully Skipped WhiteListed User.");
            }
            catch(Exception ex) { ex.DebugLog(); }
            return lstUserNames;
        }

        public List<string> FilterBlackListedUser(List<string> lstUserNames)
        {
            var NewListOfUsers = new List<string>();
            try
            {
                var Status = GetBlackListedCheckedStatus();
                if (Status != null)
                {
                    lstUserNames.ForEach(x => NewListOfUsers.Add(x.Contains("/") ? x.Split('/').LastOrDefault(y => y != string.Empty) : x));
                    if (Status.Item1 || Status.Item2)
                    {
                        var GroupRemoved = NewListOfUsers.RemoveAll(x => Blacklistuser.Any(y => y == x));
                        var PrivateRemoved = PrivateBlacklistedUser != null ? NewListOfUsers.RemoveAll(z => PrivateBlacklistedUser.Any(t => t == z)) : GroupRemoved;
                        GroupRemoved = PrivateRemoved > 0 ? PrivateRemoved : GroupRemoved;
                        if (GroupRemoved > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successfully Skipped BlackListed User.");
                    }
                    lstUserNames.Clear();
                }
            }catch(Exception ex) { ex.DebugLog();}
            return NewListOfUsers.Count > 0 ?NewListOfUsers:lstUserNames;
        }
        private Tuple<bool,bool> GetBlackListedCheckedStatus()
        {
            switch (ActivityType)
            {
                case ActivityType.ReportUsers:
                    var ReportModel= ProcessScope.GetActivitySettingsAs<ReportUserModel>();
                    return new Tuple<bool, bool>(ReportModel.IsChkReportUserSkipPrivateBlacklist, ReportModel.IsChkReportUserSkipGroupBlacklist);
                case ActivityType.Unfollow:
                    var UnFollowModel= ProcessScope.GetActivitySettingsAs<UnfollowerModel>();
                    return new Tuple<bool, bool>(UnFollowModel.IsChkUnFollowerSkipPrivateBlacklist, UnFollowModel.IsChkUnFollowerSkipGroupBlacklist);
                case ActivityType.Follow:
                    var FollowModel= ProcessScope.GetActivitySettingsAs<FollowerModel>();
                    return new Tuple<bool, bool>(FollowModel.IsChkFollowerSkipPrivateBlacklist, FollowModel.IsChkFollowerSkipGroupBlacklist);
                case ActivityType.BroadcastMessages:
                    var BroadCastMessageModel = ProcessScope.GetActivitySettingsAs<BroadcastMessagesModel>();
                    return new Tuple<bool, bool>(BroadCastMessageModel.IsChkBroadCastSkipPrivateBlacklist, BroadCastMessageModel.IsChkBroadCastSkipGroupBlacklist);
                case ActivityType.SendMessageToFollower:
                    var SendMessageToFollowerModel = ProcessScope.GetActivitySettingsAs<SendMessageToFollowerModel>();
                    return new Tuple<bool, bool>(SendMessageToFollowerModel.IsChkSendMessageSkipPrivateBlacklist, SendMessageToFollowerModel.IsChkSendMessageSkipGroupBlacklist);
                case ActivityType.AutoReplyToNewMessage:
                    var AutoReplyToNewMessageModel = ProcessScope.GetActivitySettingsAs<AutoReplyToNewMessageModel>();
                    return new Tuple<bool, bool>(AutoReplyToNewMessageModel.IsChkAutoReplySkipPrivateBlacklist, AutoReplyToNewMessageModel.IsChkAutoReplySkipGroupBlacklist);
                case ActivityType.AnswerOnQuestions:
                    var AnswerOnQuestionsModel = ProcessScope.GetActivitySettingsAs<AnswerQuestionModel>();
                    return new Tuple<bool, bool>(AnswerOnQuestionsModel.IsChkAnswerOnQuestionSkipPrivateBlacklist, AnswerOnQuestionsModel.IsChkAnswerOnQuestionSkipGroupBlacklist);
                case ActivityType.DownvoteQuestions:
                    var DownvoteQuestionsModel = ProcessScope.GetActivitySettingsAs<DownvoteQuestionsModel>();
                    return new Tuple<bool, bool>(DownvoteQuestionsModel.IsChkDownvoteQuestionSkipPrivateBlacklist, DownvoteQuestionsModel.IsChkDownvoteQuestionSkipGroupBlacklist);
                case ActivityType.UserScraper:
                    var UserScraperModel = ProcessScope.GetActivitySettingsAs<UserScraperModel>();
                    return new Tuple<bool, bool>(UserScraperModel.IsChkUserScraperSkipPrivateBlacklist, UserScraperModel.IsChkUserScraperSkipGroupBlacklist);
                case ActivityType.AnswersScraper:
                    var AnswersScraperModel = ProcessScope.GetActivitySettingsAs<AnswersScraperModel>();
                    return new Tuple<bool, bool>(AnswersScraperModel.IsChkAnswerScraperSkipPrivateBlacklist, AnswersScraperModel.IsChkAnswerScraperSkipGroupBlacklist);
                case ActivityType.QuestionsScraper:
                    var QuestionsScraperModel = ProcessScope.GetActivitySettingsAs<QuestionsScraperModel>();
                    return new Tuple<bool, bool>(QuestionsScraperModel.IsChkQuestionScraperSkipPrivateBlacklist, QuestionsScraperModel.IsChkQuestionScraperSkipGroupBlacklist);
                case ActivityType.UpvoteAnswers:
                    var UpvoteAnswersModel = ProcessScope.GetActivitySettingsAs<UpvoteAnswersModel>();
                    return new Tuple<bool, bool>(UpvoteAnswersModel.IsChkUpvoteAnswerSkipPrivateBlacklist, UpvoteAnswersModel.IsChkUpvoteAnswerSkipGroupBlacklist);
                case ActivityType.DownvoteAnswers:
                    var DownvoteAnswersModel = ProcessScope.GetActivitySettingsAs<DownvoteAnswersModel>();
                    return new Tuple<bool, bool>(DownvoteAnswersModel.IsChkDownvoteAnswerSkipPrivateBlacklist, DownvoteAnswersModel.IsChkDownvoteAnswerSkipGroupBlacklist);
                case ActivityType.UpvotePost:
                    return new Tuple<bool, bool>(upvotePostModel.IsChkUpvotePostSkipPrivateBlacklist, upvotePostModel.IsChkUpvotePostSkipGroupBlacklist);
                case ActivityType.DownVotePost:
                    return new Tuple<bool, bool>(downvotePostModel.IsChkDownvotePostSkipPrivateBlacklist, downvotePostModel.IsChkDownvotePostSkipGroupBlacklist);
                default:
                    return new Tuple<bool, bool>(false,false);
            }
        }

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

                if (JobProcess.ModuleSetting.IschkUniqueUserForAccount)
                    try
                    {
                        campaignInteractionDetails.AddInteractedData(SocialNetworks.Quora,
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

        internal void FilterAndStartFinalProcessForEachUser(QueryInfo queryInfo, out JobProcessResult jobProcessResult,
            QuoraUser quoraUser, UserInfoResponseHandler user = null)
        {
            if (UserFilterApply(user))
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"filter not matched to {quoraUser.Username}");
            else
                FinalProcess(queryInfo, out jobProcessResult, quoraUser);
            jobProcessResult = new JobProcessResult();
        }

        internal JobProcessResult FilterAndStartFinalProcessForAnswers(QueryInfo queryInfo,
            JobProcessResult jobProcessResult, List<AnswerDetails> urlList)
        {
            AnswerDetailsResponseHandler answerDetailsResponseHandler = null;
            httpHelper=InstanceProvider.GetInstance<IQdHttpHelper>();
            var InteractedAnswer = DbAccountService?.GetInteractedAnswers();
            var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
            var InteractedAnswerCount = InteractedAnswer != null ?InteractedAnswer.Count(x=>urlList.Any(y=>y.AnswerUrl==x.AnswersUrl||x.AnswersUrl.Contains(y.AnswerUrl)|| y.AnswerUrl == x.QueryValue || x.QueryValue.Contains(y.AnswerUrl))): 0;
            if(InteractedAnswerCount > 0)
            {
                urlList.RemoveAll(x => InteractedAnswer.Any(y => y.AnswersUrl == x.AnswerUrl || y.QueryValue == x.AnswerUrl));
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"Successfully Skipped {InteractedAnswerCount} Already Interacted Answer.");
            }
            foreach (var url in urlList)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    if (IsBrowser)
                    {
                        var response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url.AnswerUrl);
                        var currentUrl = _browser.BrowserWindow.CurrentUrl();
                        if ((!(currentUrl.Contains(".quora.com/")) || response.Response.Contains("Page Not Found")) || response.Response.Contains("{\\\"data\\\":{\\\"question\\\"") && queryInfo.QueryType == "Custom URLs")
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"This Answer Url ==> {currentUrl} Does Not Exists.Please Check The Url.");
                            continue;
                        }
                        answerDetailsResponseHandler = new AnswerDetailsResponseHandler(response, IsBrowser);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(url.AnswerID))
                            url.AnswerID = quoraFunct.GetAnswerId(url.AnswerUrl,out _);
                        var AnsweResponse = httpHelper.GetRequest(url.AnswerUrl).Response;
                        if(AnsweResponse.Contains("{\\\"data\\\":{\\\"question\\\"") && queryInfo.QueryType=="Custom URLs")
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                               $"This Answer Url ==> {url.AnswerUrl} Does Not Exists.Please Check The Url.");
                            continue;
                        }
                        var currenturl=Utilities.GetBetween(AnsweResponse, "\\\"permaUrlOnOriginalQuestion\\\":\\\"", "\\\",").Replace("\\\\u00e", "%C3%A").Replace("\\\\u2019", "%E2%80%99");
                        currenturl = QdConstants.DecodeUnicodeEscapes(currenturl);
                        answerDetailsResponseHandler = new AnswerDetailsResponseHandler(new ResponseParameter() { Response = AnsweResponse }, IsBrowser);
                        int.TryParse(Utilities.GetBetween(AnsweResponse, "\\\"numViews\\\":", ","), out int viewCount);
                        int.TryParse(Utilities.GetBetween(AnsweResponse, "\\\"numUpvotes\\\":", ","), out int upvoteCount);
                        answerDetailsResponseHandler.UpvoteCount = upvoteCount;
                        answerDetailsResponseHandler.AnswerView=viewCount;
                        answerDetailsResponseHandler.AnsweredUserUrl = string.IsNullOrEmpty(answerDetailsResponseHandler.AnsweredUserUrl) ? currenturl.Contains("https//:www.quora.com")?currenturl:QdConstants.HomePageUrl+currenturl : answerDetailsResponseHandler.AnsweredUserUrl;
                    }
                    var ansUrl = answerDetailsResponseHandler.AnsweredUserUrl;
                    UserInfoResponseHandler objUserInfo = null;
                    var userName = string.Empty;
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        try
                        {
                            if (!ansUrl.Contains(".quora.com"))
                                objUserInfo = quoraFunct.UserInfo(JobProcess.DominatorAccountModel, answerDetailsResponseHandler.ProfileUrl);
                            else
                            {
                                userName = string.IsNullOrEmpty(answerDetailsResponseHandler.Username)? Utilities.GetBetween(answerDetailsResponseHandler.AnsweredUserUrl + "/", "answer/", "\\"):answerDetailsResponseHandler.Username;
                                if (!string.IsNullOrEmpty(userName))
                                    objUserInfo = quoraFunct.UserInfo(JobProcess.DominatorAccountModel, userName);
                                else if(string.IsNullOrEmpty(userName))
                                {
                                    var SplitResult = answerDetailsResponseHandler.AnsweredUserUrl?.Split('/');
                                    userName = SplitResult.Length > 3 ? SplitResult[5] : "";
                                    if (!string.IsNullOrEmpty(userName))
                                        objUserInfo = quoraFunct.UserInfo(JobProcess.DominatorAccountModel, userName);
                                    else continue;
                                }    
                                else continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else
                    {
                        if (!ansUrl.Contains(".quora.com/"))
                            objUserInfo = quoraFunct.UserInfo(JobProcess.DominatorAccountModel, answerDetailsResponseHandler.ProfileUrl);
                        else
                        {
                            userName = Utilities.GetBetween(answerDetailsResponseHandler.AnsweredUserUrl, "answer/", "\\");
                            userName = string.IsNullOrEmpty(userName) ?!string.IsNullOrEmpty(answerDetailsResponseHandler.AnsweredUserUrl)?answerDetailsResponseHandler.AnsweredUserUrl.Split('/').LastOrDefault(x=>x!=string.Empty):string.Empty:userName;
                            userName = Regex.Replace(userName, "\\?.*", "");
                            if (!string.IsNullOrEmpty(userName))
                                objUserInfo = quoraFunct.UserInfo(JobProcess.DominatorAccountModel, userName);
                            else continue;
                        }

                    }
                    var usernameList = new List<string>() { userName };
                    usernameList=FilterBlackListedUser(usernameList);
                    if (usernameList.Count==0)
                    {
                        jobProcessResult.IsProcessCompleted = true;
                        break;
                    }
                    if (ActivityType == ActivityType.ReportAnswers || ActivityType == ActivityType.DownvoteAnswers)
                        if (Whitelistuser.Contains(answerDetailsResponseHandler.AnsweredUserUrl?.Replace($"{QdConstants.HomePageUrl}/profile/", "")))
                            continue;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (!CheckUserUniqueNess(jobProcessResult, queryInfo.QueryValue, ActivityType)) continue;

                    if (!CheckPostUniqueNess(jobProcessResult, ansUrl, ActivityType)) continue;
                    if (!ApplyCampaignLevelSettings(queryInfo, ansUrl)) continue;

                    FilterAndStartFinalProcessForEachAnswer(queryInfo, out jobProcessResult,
                        new QuoraUser { Url = ansUrl, Username = objUserInfo.Username }, answerDetailsResponseHandler,
                        objUserInfo);
                }
                catch (OperationCanceledException)
                {
                    if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser();
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    //if (_browser!=null && _browser.BrowserWindow != null) _browser.CloseBrowser();
                }
                if (jobProcessResult.IsProcessCompleted)
                    break;
            }
            return jobProcessResult;
        }

        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink)
        {
            if (CampaignDetails == null) return true;
            try
            {
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
                                CampaignService.GetCountOfInteractionForSpecificPost(ActivityType, postPermalink);

                            if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction) return false;
                            AddPendingActivityValueToDb(queryInfo, postPermalink);
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
                            List<DateTime> recentlyPerformedActions = null;

                            if (ActivityType == ActivityType.UpvoteAnswers || ActivityType == ActivityType.DownvoteAnswers || ActivityType == ActivityType.ReportAnswers)
                                recentlyPerformedActions = CampaignService.GetInteracteractedAnswers().Where(x => x.ActivityType == ActivityType.ToString() && x.AnswersUrl == postPermalink)
                                .OrderByDescending(x => x.InteractionDateTime).Select(x => x.InteractionDateTime)
                                .Take(1).ToList();
                            else if (ActivityType == ActivityType.ReportUsers)
                                recentlyPerformedActions = CampaignService.GetAllInteractedUsers().Where(x => x.ActivityType == ActivityType.ToString() && x.InteractedUsername == postPermalink)
                                .OrderByDescending(x => x.InteractionDateTime).Select(x => x.InteractionDateTime)
                                .Take(1).ToList();

                            if (recentlyPerformedActions.Count > 0)
                            {
                                var recentlyPerformedTime = DateTimeUtilities.ConvertToEpoch(recentlyPerformedActions[0]);
                                var delay = JobProcess.ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                                var time = DateTimeUtilities.GetEpochTime();
                                var time2 = recentlyPerformedTime + delay;
                                if (time < time2)
                                {
                                    var sleepTime = time2 - time;
                                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            $"waiting for {sleepTime} seconds as last activity on the post[{postPermalink}] was on {recentlyPerformedTime.EpochToDateTimeLocal()} in the campaign [{CampaignDetails.CampaignName}]");
                                    Task.Delay(TimeSpan.FromSeconds(sleepTime))
                                        .Wait(JobProcess.JobCancellationTokenSource.Token);
                                }
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

        private void AddPendingActivityValueToDb(QueryInfo queryInfo, string postPermalink)
        {
            CampaignService.Add(new InteractedPosts
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

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, string url, ActivityType activityType)
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
                        campaignInteractionDetails.AddInteractedData(SocialNetworks.Quora,
                            $"{CampaignDetails.CampaignId}.post", url);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        campaignInteractionDetails.AddInteractedData(SocialNetworks.Quora, CampaignDetails.CampaignId,
                            url);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }

            return true;
        }


        private void FilterAndStartFinalProcessForEachAnswer(QueryInfo queryInfo, out JobProcessResult jobProcessResult,
            QuoraUser quoraUser, AnswerDetailsResponseHandler objAnswerDetailsResponseHandler,
            UserInfoResponseHandler objUserInfo)
        {
            if (!AnswerFilterApply(objAnswerDetailsResponseHandler, objUserInfo) && !UserFilterApply(objUserInfo))
                FinalProcess(queryInfo, out jobProcessResult, quoraUser);
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"Filtered Not Matched For {objAnswerDetailsResponseHandler.AnsweredUserUrl} To Perform {JobProcess.ActivityType}");
                jobProcessResult = new JobProcessResult();
            }
        }

        internal JobProcessResult FilterAndStartFinalProcessWithQuestionUrl(QueryInfo queryInfo,
            JobProcessResult jobProcessResult, List<string> lstUserNames)
        {
            var InterectedQuestions=DbAccountService.GetInteractedQuestion();
            var skippedCount = 0;
            foreach (var eachurl in lstUserNames)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    QuestionDetailsResponseHandler objQuestionDetailsResponseHandler = null;
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        objQuestionDetailsResponseHandler =
                            quoraFunct.QuestionDetails(JobProcess.DominatorAccountModel, eachurl);
                    }
                    else
                    {
                        var resp = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, eachurl);
                        objQuestionDetailsResponseHandler = new QuestionDetailsResponseHandler(resp);
                    }
                    var InteractedAnswerCount = InterectedQuestions != null ? InterectedQuestions.Count(x => objQuestionDetailsResponseHandler.QuestionUrl==x.QuestionUrl):0;
                    if (InteractedAnswerCount > 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                           $"Successfully Skipped Already Interacted QuestionUrl==>{eachurl}.");continue;
                    }  
                    var objQuoraUser = new QuoraUser { Url = objQuestionDetailsResponseHandler.QuestionUrl };
                    if(string.IsNullOrEmpty(objQuestionDetailsResponseHandler.QuestionUrl) || !objQuestionDetailsResponseHandler.Success)
                       {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Wrong url with Query Type");
                            jobProcessResult.IsProcessCompleted = true;
                            break;
                        }
                    if(CampaignService.GetInteracteractedAnswers().Any(answer=>answer.AnswersUrl==objQuoraUser.Url))
                    {
                        skippedCount++;
                        continue;
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FilterAndStartFinalProcessForEachQuestionUrl(queryInfo, objQuoraUser,
                        objQuestionDetailsResponseHandler, out jobProcessResult);
                }
                catch (OperationCanceledException)
                {
                    if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (jobProcessResult.IsProcessCompleted)
                    break;
            }
            if(skippedCount > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Skipped {skippedCount} Interacted Answers");
            return jobProcessResult;
        }

        internal void FilterAndStartFinalProcessForEachQuestionUrl(QueryInfo queryInfo,
            QuoraUser quoraUser, QuestionDetailsResponseHandler questionresponsehandler,
            out JobProcessResult jobProcessResult)
        {
            if(!QuestionFilterApply(questionresponsehandler))
                FinalProcess(queryInfo, out jobProcessResult, quoraUser);
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Filter Not Matched For {questionresponsehandler.QuestionUrl}");
                jobProcessResult = new JobProcessResult();
            }
        }

        public void FilterAndStartProcessForPosts(QueryInfo queryInfo, JobProcessResult objJobProcessResult, List<PostDetails> postDetailsList)
        {
            try
            {
                List<string> interactedPosts = null;
                if(ActivityType==ActivityType.UpvotePost)
                   interactedPosts = DbAccountService.GetInteractedPosts(ActivityType.UpvotePost, JobProcess.DominatorAccountModel.AccountBaseModel.UserName).Select(x=>x.PostUrl).ToList();
                if(ActivityType == ActivityType.DownVotePost)
                    interactedPosts = DbAccountService.GetInteractedPosts(ActivityType.DownVotePost, JobProcess.DominatorAccountModel.AccountBaseModel.UserName).Select(x => x.PostUrl).ToList();

                var InteractedPostCount = interactedPosts != null ? interactedPosts.Count(x => postDetailsList.Any(y => y.PostUrl == x || x.Contains(y.PostUrl))) : 0;
                if (InteractedPostCount > 0)
                {
                    postDetailsList.RemoveAll(x => interactedPosts.Any(y => y == x.PostUrl));
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"Successfully Skipped {InteractedPostCount} Already Interacted Post.");
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                var RemainingList=FilterBlackListedUser(postDetailsList.Select(post => post.PostAuthorProfileUrl).ToList());
                foreach (var postDetail in postDetailsList)
                {
                    if (!RemainingList.Contains(postDetail.PostAuthorProfileUrl)) continue;
                    if(postDetail.ViewerVoteType.Contains("upvote") && ActivityType == ActivityType.UpvotePost)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"Successfully Skipped Already Upvoted Post {postDetail.PostUrl}");
                        continue;
                    }
                    if (postDetail.ViewerVoteType.Contains("downvote") && ActivityType == ActivityType.DownVotePost)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"Successfully Skipped Already Downvoted Post {postDetail.PostUrl}");
                        continue;
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.StartedActivity,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, postDetail.PostUrl);
                    FilterAndStartProcessForEachPost(queryInfo, out JobProcessResult jobProcessResult, postDetail);
                    Task.Delay(TimeSpan.FromSeconds(5)).Wait(JobProcess.JobCancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartProcessForEachPost(QueryInfo queryInfo, out JobProcessResult jobProcessResult, PostDetails postDetail)
        {
            if (!PostFilterApply(postDetail))
            {
                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultUser = postDetail,
                    QueryInfo = queryInfo
                });
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Filter Not Matched For {postDetail.PostUrl}");
                jobProcessResult = new JobProcessResult();
            }
        }


        #region Filter

        private bool PostFilterApply(PostDetails postDetail)
        {
            if (ActivityType == ActivityType.UpvotePost)
                return (upvotePostModel.IsCheckViewsCount && !upvotePostModel.ViewsCountRange.InRange(postDetail.ViewsCount)) ||
                       (upvotePostModel.IsCheckShareCount && !upvotePostModel.ShareCountRange.InRange(postDetail.ShareCount)) ||
                       (upvotePostModel.IsCheckPostOwnerFollowerCount && !upvotePostModel.PostOwnerFollowerCount.InRange(postDetail.PostAuthorFollowerCount)) ||
                       (upvotePostModel.PostFilterModel.FilterLikes && !upvotePostModel.PostFilterModel.LikesCountRange.InRange(postDetail.UpvoteCount)) ||
                       (upvotePostModel.PostFilterModel.FilterComments && !upvotePostModel.PostFilterModel.CommentsCountRange.InRange(postDetail.CommentCount));
            else if (ActivityType == ActivityType.DownVotePost)
                return (downvotePostModel.IsCheckViewsCount && !downvotePostModel.ViewsCountRange.InRange(postDetail.ViewsCount)) ||
                       (downvotePostModel.IsCheckShareCount && !downvotePostModel.ShareCountRange.InRange(postDetail.ShareCount)) ||
                       (downvotePostModel.IsCheckPostOwnerFollowerCount && !downvotePostModel.PostOwnerFollowerCount.InRange(postDetail.PostAuthorFollowerCount)) ||
                       (downvotePostModel.PostFilterModel.FilterLikes && !downvotePostModel.PostFilterModel.LikesCountRange.InRange(postDetail.UpvoteCount)) ||
                       (downvotePostModel.PostFilterModel.FilterComments && !downvotePostModel.PostFilterModel.CommentsCountRange.InRange(postDetail.CommentCount));
            else
                return false;
        }
        public bool UserFilterApply(UserInfoResponseHandler quorauser)
        {
            try
            {
                var objUserFilter = new ScrapeFilter.User(JobProcess.ModuleSetting, JobProcess.DominatorAccountModel, ProcessScope);
                if (objUserFilter.IsAnonymousProfilePicture(quorauser))
                    return true;
                if (objUserFilter.FilterFollowers(quorauser))
                    return true;
                if (objUserFilter.FilterFollowings(quorauser))
                    return true;
                if (objUserFilter.IsAnswerCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsQuestionCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsPostCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsBlogCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsTopicCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsEditCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsAnswerViewCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.FilterBioRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterUserNameRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterLocationRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterStudyPlaceRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterWorkPlaceRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.CheckForRangeFollowRatio(quorauser))
                    return true;
                if (objUserFilter.FilterIgnoreFollower(quorauser))
                    return true;
                if (objUserFilter.FilterLastWriteAnswer(quorauser))
                    return true;
                if (objUserFilter.FilterMinimumCharacterInBio(quorauser))
                    return true;
                if (ActivityType == ActivityType.Unfollow)
                {
                    //if (objUserFilter.FilterFollowedOutsideSoftware(JobProcess.DominatorAccountModel.AccountId, quorauser))
                    //  return true;
                    if (objUserFilter.FilterFollowedBackOrNot(quorauser))
                        return true;
                    if (objUserFilter.FilterLastAnswer(quorauser, JobProcess.DominatorAccountModel))
                        return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        protected bool QuestionFilterApply(QuestionDetailsResponseHandler questionresponsehandler)
        {
            var objUserFilter = new ScrapeFilter.User(JobProcess.ModuleSetting, ProcessScope);
            if (objUserFilter.FilterQuestionView(questionresponsehandler))
                return true;
            if (objUserFilter.FilterQuestionFollower(questionresponsehandler))
                return true;
            if (objUserFilter.FilterQuestionComment(questionresponsehandler))
                return true;
            if (objUserFilter.FilterQuestionAnswer(questionresponsehandler))
                return true;
            //if (objUserFilter.FilterQuestionAskDay(questionresponsehandler))
            //    return true;
            if (objUserFilter.FilterQuestionAskSpaces(questionresponsehandler))
                return true;
            if (objUserFilter.FilterQuestionLocked(questionresponsehandler))
                return true;
            return false;
        }

        protected bool AnswerFilterApply(AnswerDetailsResponseHandler answerResponseHandler,
            UserInfoResponseHandler quorauser = null)
        {
            var objUserFilter = new ScrapeFilter.User(JobProcess.ModuleSetting,ProcessScope);
            if (objUserFilter.FilterAnswerView(answerResponseHandler))
                return true;
            if (objUserFilter.FilterAnswerDay(answerResponseHandler))
                return true;
            if (objUserFilter.FilterAnswerCount(answerResponseHandler))
                return true;
            if (ActivityType == ActivityType.ReportAnswers)
            {
                if (objUserFilter.IsAnonymousProfilePicture(quorauser))
                    return true;
                if (objUserFilter.FilterFollowers(quorauser))
                    return true;
                if (objUserFilter.FilterFollowings(quorauser))
                    return true;
                if (objUserFilter.IsAnswerCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsQuestionCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsPostCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsBlogCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsTopicCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsEditCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.IsAnswerViewCountIsInRange(quorauser))
                    return true;
                if (objUserFilter.FilterBioRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterUserNameRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterLocationRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterStudyPlaceRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.FilterWorkPlaceRestrictedWords(quorauser))
                    return true;
                if (objUserFilter.CheckForRangeFollowRatio(quorauser))
                    return true;
                if (objUserFilter.FilterIgnoreFollower(quorauser))
                    return true;
                if (objUserFilter.FilterLastWriteAnswer(quorauser))
                    return true;
            }

            return false;
        }

        #endregion
    }
}