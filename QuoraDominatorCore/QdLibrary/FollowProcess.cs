using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedPosts;
using InteractedUsers = DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedUsers;
using InteractedUsersAccount = DominatorHouseCore.DatabaseHandler.QdTables.Accounts.InteractedUsers;

namespace QuoraDominatorCore.QdLibrary
{
    public class FollowProcess : QdJobProcessInteracted<InteractedUsersAccount>
    {
        private readonly IQdHttpHelper _httpHelper;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly SocialNetworks _networks;
        public IQuoraFunctions quoraFunct;
        public IQDBrowserManagerFactory Factory;
        public FollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess,
            IQDBrowserManagerFactory factory)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _httpHelper = qdHttpHelper;
            quoraFunct = qdFunc;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            FollowerModel = processScopeModel.GetActivitySettingsAs<FollowerModel>();
            _networks = SocialNetworks.Quora;
            Factory = qdLogInProcess.managerFactory;
        }

        public FollowerModel FollowerModel { get; set; }


        private IQuoraBrowserManager _browser { get; set; }

        private int _stopFollowToolWhenReachedCount = 1 ;

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var quoraUser = (QuoraUser)scrapeResult.ResultUser;
            _browser = Factory.QdBrowserManager();
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var moduleConfiguration =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var quoraConfig = genericFileManager.GetModel<QuoraModel>(ConstantVariable.GetOtherQuoraSettingsFile());

                if (quoraConfig != null && quoraConfig.IsEnableFollowDifferentUserChecked)
                    GlobalInteractionDetails.AddInteractedData(SocialNetworks.Quora, ActivityType, quoraUser.Username);

                #region Check for campaignwise Unique Follow user

                else if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode &&
                         FollowerModel.IsFollowUniqueUsersInCampaignChecked)
                    CampaignInteractionDetails.AddInteractedData(SocialNetworks.Quora, CampaignId, quoraUser.Username);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                jobProcessResult.IsProcessSuceessfull = false;
                ex.DebugLog();
                return jobProcessResult;
            }

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
            try
            {
                var userResponse = "";
                FollowResponseHandler response = null;

                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var request = _httpHelper.GetRequestParameter();
                    request.UserAgent =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36";
                    request.Accept =
                        "text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8,application/signed-exchange;v=b3";
                    request.Cookies = DominatorAccountModel.Cookies;
                    _httpHelper.SetRequestParameter(request);

                    var url = $"{QdConstants.HomePageUrl}/profile/" + scrapeResult.ResultUser.Username;
                    userResponse = _httpHelper.GetRequest(url).Response;

                    response = quoraFunct.Follow(DominatorAccountModel, userResponse, url).Result;
                    if (response.Response.Response.Contains("FollowUserSuccessful") || response.Success)
                    {
                        if (response.Response.Response.Contains("You have exceeded the limit for following new people"))
                        {
                            jobProcessResult.IsProcessSuceessfull = false;
                            jobProcessResult.IsProcessCompleted = true;
                            FollowerModel.IsChkFollowToolGetsTemporaryBlockedChecked = true;
                        }

                        AddFollowedDataToDataBase(scrapeResult);

                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Username);
                        IncrementCounters();

                        PostFollowProcess(quoraUser, scrapeResult.QueryInfo);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response.Issue?.Error);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                else
                {
                    var url = $"{QdConstants.HomePageUrl}/profile/" + scrapeResult.ResultUser.Username;
                    var FollowResponse = _browser.Follow(url)["Follow"];
                    if (string.IsNullOrEmpty(FollowResponse))
                    {
                        AddFollowedDataToDataBase(scrapeResult);

                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Username);
                        IncrementCounters();

                        PostFollowProcess(quoraUser, scrapeResult.QueryInfo);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, FollowResponse);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            DelayBeforeNextActivity();
            return jobProcessResult;
        }


        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var quorauser = (QuoraUser)scrapeResult.ResultUser;

                #region Save to AccountDb

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new InteractedUsersAccount
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = quorauser.UserId,
                    FollowedBack = quorauser.FollowedBack,
                    Time = DateTimeUtilities.GetEpochTime()
                });

                #endregion

                #region Save to CampaignDb

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        InteractedUserId = quorauser.UserId,
                        FollowBackStatus = quorauser.FollowedBack,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    });
                }

                #endregion

                #region Save to PrivateBlacklistDb

                if (FollowerModel.IsChkPrivateblacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quorauser.Username,
                            UserId = quorauser.UserId,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                        });

                #endregion

                #region Add to GroupBlacklist DB

                if (FollowerModel.IsChkGroupblacklist)
                {
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quorauser.Username,
                        UserId = quorauser.UserId,
                        AddedDateTime = DateTime.Now
                    });
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.OtherConfigurationStarted,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

                var accountUserName = quoraFunct.GetProfileUrl(DominatorAccountModel)
                    .Replace($"{QdConstants.HomePageUrl}/profile/", "");
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var currentUserInfo = quoraFunct.UserInfo(DominatorAccountModel,
                    $"{QdConstants.HomePageUrl}/profile/" + accountUserName);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDisplayColumn2(currentUserInfo.FollowingCount)
                    .SaveToBinFile();
                var currentAccount = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Quora)
                    .DominatorAccountViewModel
                    .LstDominatorAccountModel.FirstOrDefault(x => x.AccountId == DominatorAccountModel.AccountId);
                if (currentAccount != null) currentAccount.DisplayColumnValue2 = currentUserInfo.FollowingCount;

                var lstTotalFollowedUsers = DbAccountService.GetInteractedUsers(ActivityType).ToList();
                // Process for auto Follow and Unfollow

                #region Process for auto Follow and Unfollow

                //if (FollowerModel.IsChkStopFollow)
                //{                    
                //if ((FollowerModel.IsChkStopFollowToolWhenReachChecked && followingCount > FollowerModel.StopFollowToolWhenReach.GetRandom())
                //    || FollowerModel.IsChkFollowToolGetsTemporaryBlockedChecked || (FollowerModel.IsChkWhenFollowerFollowingsIsSmallerThanChecked && FollowerModel.FollowerFollowingsMaxValue < currentUserInfo.FollowRatio) || FollowerModel.IsChkEnableAutoFollowUnfollowChecked)
                //    StopFollow();
                //}
                if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked) //FollowerModel.IsChkStartUnFollow
                {
                    var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                    if (FollowerModel.IsChkStopFollowToolWhenReachChecked &&
                        currentUserInfo.FollowingCount > FollowerModel.StopFollowToolWhenReach.GetRandom()
                        || FollowerModel.IsChkFollowToolGetsTemporaryBlockedChecked)
                        dominatorScheduler.ChangeAccountsRunningStatus(false, DominatorAccountModel.AccountId,
                            ActivityType.Follow);
                }

                //if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked)//FollowerModel.IsChkStartUnFollow
                //{
                //    if ((FollowerModel.IsChkStartUnFollowWhenReached && followingCount > FollowerModel.StartUnFollowToolWhenReach.GetRandom())
                //        || (FollowerModel.IsChkFollowToolGetsTemporaryBlockedChecked || scrapeResult.IsAccountLocked) || FollowerModel.IsChkEnableAutoFollowUnfollowChecked)//&&
                //    {
                //        DominatorScheduler.EnableDisableModules(ActivityType.Follow, ActivityType.Unfollow, DominatorAccountModel.AccountId);
                //    }
                //}

                #endregion

                // Process for Unfollow previously followed user

                #region Process for Unfollow previously followed user

                if (FollowerModel.IsChkUnfollowUsersChecked)
                {
                    var unfollowedprevious = FollowerModel.UnfollowPrevious.GetRandom();
                    if (unfollowedprevious > 0)
                    {
                        if (FollowerModel.IsChkUnfollowfollowedbackChecked)
                            lstTotalFollowedUsers.ForEach(x =>
                            {
                                try
                                {
                                    if ((DateTime.Now - x.Date.EpochToDateTimeUtc()).TotalHours >
                                        unfollowedprevious && x.FollowedBack == 1)
                                        UnFollow(x.InteractedUsername);
                                }
                                catch (Exception e)
                                {
                                    e.DebugLog();
                                }
                            });
                        if (FollowerModel.IsChkUnfollownotfollowedbackChecked)
                            lstTotalFollowedUsers.ForEach(x =>
                            {
                                try
                                {
                                    if (x.FollowedBack == 0 && (DateTime.Now - x.Date.EpochToDateTimeUtc()).TotalHours >
                                        unfollowedprevious)
                                        UnFollow(x.InteractedUsername);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            });
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UnFollow(string username)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var ProfileUrl = $"{QdConstants.HomePageUrl}/profile/{username}";
            var linkresp = _httpHelper.GetRequest(ProfileUrl).Response;
            var response = quoraFunct.UnFollow(DominatorAccountModel, linkresp, ProfileUrl).Result;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (response.Success)
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Unfollow, username);
        }

        private void PostFollowProcess(QuoraUser quoraUser, QueryInfo queryinfo)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);

            #region PostUpvote

            if (FollowerModel.IsChkLikeUsersLatestPost)
                try
                {
                    if (quoraUser.NumberOfPost == 0)
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Number of Post is 0");
                    }
                    else
                    {
                        var upvotePerDayLimit = FollowerModel.UpvotePerDay.GetRandom();
                        var numberofpostToday = dbCampaignService.GetAllInteractedPosts()
                            .Count(x => x.InteractionDate >= DateTime.Today && x.LikeCount == 1);

                        if (FollowerModel.IsChkUpvotePerDay && numberofpostToday >= upvotePerDayLimit &&
                            FollowerModel.UpvotePerDay.EndValue != 0)
                        {
                            GlobusLogHelper.log.Info(Log.DailyLimitReached, SocialNetworks.Quora,
                                DominatorAccountModel.UserName, ActivityType, "Upvote Posts");
                        }
                        else
                        {
                            int numberofUpvote = 0, followBetweenGetRandom = FollowerModel.UpvoteMax.GetRandom();

                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var postResponseHanler = quoraFunct.Post(DominatorAccountModel, quoraUser.Username);
                            var responseFollowerFirstPage = postResponseHanler.Response.Response;


                            //string biid = Utilities.GetBetween(responseFollowerFirstPage, "\"biid\": ", "},");
                            //string biid2 = Utilities.GetBetween(responseFollowerFirstPage, "\"object_id\": ", "},");
                            foreach (var eachPostBiid in postResponseHanler.PostUrl)
                            {
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                var response = quoraFunct.PostUpvote(DominatorAccountModel, responseFollowerFirstPage,
                                    eachPostBiid.Key);

                                if (response.Success)
                                {
                                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        DominatorAccountModel.AccountBaseModel.UserName);

                                    dbCampaignService.Add(new InteractedPosts
                                    {
                                        Username = quoraUser.Username,
                                        LikeCount = 1,
                                        InteractionDate = DateTime.Now
                                    });
                                    if (FollowerModel.IsChkMaxUpvote && ++numberofUpvote >= followBetweenGetRandom)
                                        break;
                                    if (FollowerModel.IsChkUpvotePerDay &&
                                        ++numberofUpvote + numberofpostToday >= upvotePerDayLimit &&
                                        FollowerModel.UpvotePerDay.EndValue != 0)
                                    {
                                        GlobusLogHelper.log.Info(Log.DailyLimitReached, SocialNetworks.Quora,
                                            DominatorAccountModel.UserName, ActivityType, " Upvote Posts");
                                        break;
                                    }

                                    Thread.Sleep(3000);
                                }
                            }

                            //if (FollowerModel.IsChkMaxUpvote && ++numberofUpvote >= FollowerModel.UpvoteMax.GetRandom())
                            //    break;

                            #region Pagination

                            var forSecondPage =
                                Utilities.GetBetween(responseFollowerFirstPage, "hashes", "has_more").Split('[')[1]
                                    .Split(']')[0].Trim();
                            forSecondPage = forSecondPage.TrimStart('"').TrimEnd('"');
                            var arrayForSecondPostData = Regex.Split(forSecondPage, "\", \"");

                            var formKey = Utilities.GetBetween(responseFollowerFirstPage, "formkey\": \"", "\"");
                            var windowId = Utilities.GetBetween(responseFollowerFirstPage, "windowId\": \"", "\"");
                            var postKey = Utilities.GetBetween(responseFollowerFirstPage, "postkey\": \"", "\"");
                            var serializeComponent =
                                Utilities.GetBetween(responseFollowerFirstPage, "serialized_component", "auto_paged")
                                    .Split('"')[2].Replace("+", "%2B").Replace("\\+", "%2B").Replace("+", "%2B")
                                    .Replace("/", "%2F").Replace("=\\n", "").Replace("\\", "%5C");
                            var everyPageHash = "";
                            var answerPagination = responseFollowerFirstPage;
                            while (!FollowerModel.IsChkMaxUpvote || FollowerModel.IsChkMaxUpvote &&
                                   numberofUpvote <= followBetweenGetRandom)
                            {
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                try
                                {
                                    if (FollowerModel.IsChkUpvotePerDay &&
                                        numberofUpvote + numberofpostToday >= upvotePerDayLimit &&
                                        FollowerModel.UpvotePerDay.EndValue != 0)
                                        break;
                                    var numberofPost = int.Parse(Utilities
                                        .GetBetween(Regex.Split(responseFollowerFirstPage, "list_count")[5], ">", "<")
                                        .Replace(",", ""));
                                    if (FollowerModel.IsChkMaxUpvote && numberofPost < followBetweenGetRandom)
                                        break;
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                var forthird =
                                    "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22paged_list_parent_cid%22%3A%22XmvJnrQ%22%2C%22client_hashes%22%3A%5B%22";
                                forthird += string.Join("%22%2C%22", arrayForSecondPostData);
                                try
                                {
                                    var forNextPage =
                                        Utilities.GetBetween(answerPagination, "hashes", "has_more").Split('[')[1]
                                            .Split(']')[0];
                                    forNextPage = forNextPage.TrimEnd('"').TrimStart('"');
                                    var arrayForPostData = Regex.Split(forNextPage, "\", \"");
                                    everyPageHash += string.Join("%22%2C%22", arrayForPostData);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                forthird = forthird + everyPageHash +
                                           "%22%5D%2C%22force_cid%22%3A%22yIqDpEj%22%2C%22domids_to_remove%22%3A%5B%5D%7D%7D&revision=d3267db8daf4846602de61b0609529ee3e393d85&referring_controller=user&referring_action=all_posts&__vcon_json=%5B%22QG2IoEDL3Ygx4X%22%5D&__vcon_method=get_next_page&__e2e_action_id=eyq2rp90il&js_init=%7B%22hashes%22%3A%5B%22";
                                forthird += string.Join("%22%2C%22", arrayForSecondPostData);
                                forthird += "%22%5D%2C%22has_more%22%3Atrue%2C%22serialized_component%22%3A%22" +
                                            serializeComponent +
                                            "%3D%3D%5Cn%22%2C%22auto_paged%22%3Atrue%2C%22retarget_links%22%3Atrue%2C%22enable_mobile_hide_content%22%3Atrue%7D&__metadata=%7B%7D";

                                try
                                {
                                    postResponseHanler = quoraFunct.Post(DominatorAccountModel, quoraUser.Username,
                                        formKey, postKey, windowId, forthird);
                                    foreach (var eachPostBiid in postResponseHanler.PostUrl)
                                    {
                                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        var response = quoraFunct.PostUpvote(DominatorAccountModel,
                                            responseFollowerFirstPage, eachPostBiid.Key);
                                        if (response.Success)
                                        {
                                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                                DominatorAccountModel.AccountBaseModel.UserName);

                                            dbCampaignService.Add(new InteractedPosts
                                            {
                                                Username = quoraUser.Username,
                                                LikeCount = 1,
                                                InteractionDate = DateTime.Now
                                            });
                                            if (FollowerModel.IsChkMaxUpvote &&
                                                ++numberofUpvote >= followBetweenGetRandom)
                                                break;
                                            if (FollowerModel.IsChkUpvotePerDay &&
                                                ++numberofUpvote + numberofpostToday >= upvotePerDayLimit &&
                                                FollowerModel.UpvotePerDay.EndValue != 0)
                                            {
                                                GlobusLogHelper.log.Info(Log.DailyLimitReached, SocialNetworks.Quora,
                                                    DominatorAccountModel.UserName, ActivityType, " Upvote Posts");
                                                break;
                                            }

                                            Thread.Sleep(3000);
                                        }
                                    }

                                    if (FollowerModel.IsChkMaxUpvote && ++numberofUpvote >= followBetweenGetRandom)
                                        break;
                                    answerPagination = postResponseHanler.Response.Response;
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            }

                            #endregion
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            #region Commented for leter use

            //else if (FollowerModel.ChkLikeRandomPostsChecked)
            //{
            //    try
            //    {
            //        if (quoraUser.NumberOfPost == 0)
            //            GlobusLogHelper.log.Info($"{SocialNetworks.Quora}\t {DominatorAccountModel.UserName}\t Could not Upvote Random Post As Number of Post is 0");
            //        else
            //        {
            //            int NumberofpostToday = IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);dbCampaignService.Get<InteractedPosts>(x => x.InteractionDate.Day == DateTime.Now.Day && x.LikeCount == 1).Count;
            //            if (NumberofpostToday > FollowerModel.IncreaseEachDayLike.GetRandom())
            //                GlobusLogHelper.log.Info($"{SocialNetworks.Quora}\t {DominatorAccountModel.UserName}\t has reached per day limit to Upvote Posts");
            //            else
            //            {
            //                int NumberofPostSoFar = IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);dbCampaignService.Get<InteractedPosts>(x => x.LikeCount == 1).Count;
            //                if (NumberofPostSoFar > FollowerModel.LikeMaxBetween.GetRandom())
            //                    GlobusLogHelper.log.Info($"{SocialNetworks.Quora}\t {DominatorAccountModel.UserName}\t Maximum upvote limit has reached");
            //                else
            //                {
            //                    QuoraFunct ObjQuoraFunct = new QuoraFunct(DominatorAccountModel);
            //                    PostResponseHandler PostResponseHanler = ObjQuoraFunct.Post(quoraUser.Username);
            //                    string responseFollowerFirstPage = PostResponseHanler.response.Response;
            //                    QuoraFunct quorafunct = new QuoraFunct(DominatorAccountModel);
            //                    string formkey1 = Utilities.GetBetween(responseFollowerFirstPage, "\"formkey\": \"", "\", \"");
            //                    string postkey1 = Utilities.GetBetween(responseFollowerFirstPage, "\"postkey\": \"", "\"");
            //                    string windowId1 = Utilities.GetBetween(responseFollowerFirstPage, "\"windowId\": \"", "\"");
            //                    PostUpvoteResponseHandler response = quorafunct.PostUpvote(formkey1, postkey1, windowId1, PostResponseHanler.PostUrl.ElementAt(new Random().Next(0, PostResponseHanler.PostUrl.Count)).Key);
            //                    if (response.Success)
            //                    {
            //                        GlobusLogHelper.log.Info("{0}\t {1}\t Upvoted " + PostResponseHanler.PostUrl[PostResponseHanler.PostUrl.ElementAt(new Random().Next(0, PostResponseHanler.PostUrl.Count)).Key], DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName);
            //                        IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);dbCampaignService.Add<InteractedPosts>(new InteractedPosts()
            //                        {
            //                            Username = quoraUser.Username,
            //                            LikeCount = 1,
            //                            InteractionDate = DateTime.Now
            //                        });
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        GlobusLogHelper.log.Info("Failed Upvote Random post");
            //    }

            //} 

            #endregion

            #endregion

            #region StopFollowToolWhenReached
            //if (FollowerModel.IsChkStopFollowToolWhenReachChecked && _stopFollowToolWhenReachedCount++ >= FollowerModel.StopFollowToolWhenReach.GetRandom())
            //{

            //    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
            //        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
            //        $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_stopFollowToolWhenReachedCount - 1} " + "Followings");

            //    StopAndRescheduleJob();

            //}
            #endregion


            if (FollowerModel.ChkCommentOnUserLatestPostsChecked && FollowerModel.UploadComment != null)
                try
                {
                    if (quoraUser.NumberOfPost == 0)
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, SocialNetworks.Quora,
                            DominatorAccountModel.UserName, ActivityType,
                            ":Could not Comment on Latest Post As Number of Post is 0");
                    }
                    else
                    {
                        var numberofCommentToday = dbCampaignService.GetAllInteractedPosts().Count(x =>
                            x.SinAccUsername == DominatorAccountModel.UserName && x.InteractionDate >= DateTime.Today &&
                            x.CommentCount == 1);
                        var numberofCommentSoFar =
                            dbCampaignService.GetAllInteractedPosts().Count(x => x.CommentCount == 1);
                        if (FollowerModel.IsChkPerDayComment &&
                            numberofCommentToday >= FollowerModel.Comments.GetRandom())
                        {
                            GlobusLogHelper.log.Info(Log.DailyLimitReached, SocialNetworks.Quora,
                                DominatorAccountModel.UserName, ActivityType, "Comment on Posts");
                        }
                        else if (FollowerModel.IsChkCommentPercentage && numberofCommentSoFar >=
                                 Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                                     FollowerModel.CommentPercentage))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora,
                                DominatorAccountModel.UserName, ActivityType, "comment percentage limit has reached");
                        }
                        else
                        {
                            var postResponseHanler = quoraFunct.Post(DominatorAccountModel, quoraUser.Username);
                            var responseFollowerFirstPage = postResponseHanler.Response.Response;


                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var comment = quoraFunct.CommentOnPost(DominatorAccountModel, responseFollowerFirstPage,
                                FollowerModel.UploadComment, postResponseHanler.PostUrl.Keys.First());
                            if (comment.Success)
                            {
                                GlobusLogHelper.log.Info(
                                    "{0}\t {1}\t Commented on " +
                                    postResponseHanler.PostUrl[postResponseHanler.PostUrl.Keys.First()],
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName);
                                dbCampaignService.Add(new InteractedPosts
                                {
                                    Username = quoraUser.Username,
                                    CommentCount = 1,
                                    InteractionDate = DateTime.Now
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    GlobusLogHelper.log.Info("Failed to Comment on Latest post");
                }

            if (FollowerModel.ChkRemovePoorQualitySourcesChecked)
                try
                {
                    var accountUserName = quoraFunct.GetProfileUrl(DominatorAccountModel)
                        .Replace($"{QdConstants.HomePageUrl}/profile/", "");
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var currentUserInfo = quoraFunct.UserInfo(DominatorAccountModel,
                        $"{QdConstants.HomePageUrl}/profile/" + accountUserName);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (currentUserInfo.FollowerCount > FollowerModel.FollowBackRatio.EndValue)
                        if (currentUserInfo.FollowerCount / currentUserInfo.FollowingCount <
                            FollowerModel.FollowBackRatio.StartValue)
                        {
                            int i;
                            for (i = 0; i < FollowerModel.SavedQueries.Count; i++)
                                if (FollowerModel.SavedQueries[i].QueryValue == queryinfo.QueryValue)
                                    break;
                            FollowerModel.SavedQueries.RemoveAt(i);
                            var templateFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                            var templatemodel = templateFileManager.Get().FirstOrDefault(x => x.Id == TemplateId);
                            templatemodel.ActivitySettings = JsonConvert.SerializeObject(FollowerModel);
                            //Need to check
                            templateFileManager.UpdateActivitySettings(TemplateId, templatemodel.ActivitySettings);
                            GlobusLogHelper.log.Info("{0}\t {1}\t Removed Query " + queryinfo.QueryValue,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName);
                        }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            #region Message

            if (FollowerModel.ChkSendDirectMessageAfterFollowChecked && FollowerModel.Message != null)
                try
                {
                    var numberofMessageToday = dbCampaignService.GetAllInteractedPosts().Count(x =>
                        x.SinAccUsername == DominatorAccountModel.UserName && x.InteractionDate >= DateTime.Today &&
                        x.Message != null);
                    var numberofMessageSoFar = dbCampaignService.GetAllInteractedPosts().Count(x =>
                        x.SinAccUsername == DominatorAccountModel.UserName && x.Message != null);
                    if (FollowerModel.IsChkMaxPerDayMessage &&
                        numberofMessageToday >= FollowerModel.MessageBetween.GetRandom() &&
                        FollowerModel.MessageBetween.EndValue != 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora,
                            DominatorAccountModel.UserName, ActivityType, "has reached per day limit to send message");
                    }
                    else if (FollowerModel.IsChkDirectMessagePercentage && numberofMessageSoFar >=
                             Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                                 FollowerModel.DirectMessagePercentage))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora,
                            DominatorAccountModel.UserName, ActivityType, "comment percentage limit has reached");
                    }
                    else
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var postdata =
                            "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%7D%7D&revision=2632e70624cb9a1d5d661d281246b09c3c449417&formkey=" +
                            quoraUser.Formkey + "&postkey=" + quoraUser.PostKey + "&window_id=" + quoraUser.WindowId +
                            "&referring_controller=user&referring_action=profile&parent_cid=hlBjci2&parent_domid=uSeGXb&domids_to_remove=%5B%5D&__vcon_json=%5B%22YlYPeMg3Y9i9%2Bv%22%5D&__vcon_method=load_menu&__e2e_action_id=f0623d22ak&js_init=%7B%22oid%22%3A" +
                            quoraUser.UserId +
                            "%2C%22is_sticky%22%3Afalse%2C%22sticky_offsets%22%3A%7B%22top%22%3A300%7D%2C%22lazy_loaded%22%3Atrue%2C%22sdvars%22%3A%22AAEAAFAlbvv%2BNdVzDYzQZVXcS4rWiTPj%2F6fQjy%2BIyMr463BXpRe1ABNmRPNfhq3kpJoEUrZY2Hak%5CnjR1Z3s3aD%2Bm48PvCg0fIRKXKsGQUvVqHU76cKqRIkWuJBvpJ05KXlYqmuLLiXVjw7o2bkCsAKHnV%5Cn61x7dDdOpAyk4GHAWx9IRJzG%2F1mlOwU5BZw2TqPBhaeLFQ%3D%3D%5Cn%22%7D&__metadata=%7B%7D";

                        var checkCanMessage = _httpHelper
                            .PostRequest(
                                $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_v=YlYPeMg3Y9i9%2Bv&_m=load_menu",
                                postdata).Response;

                        if (checkCanMessage.Contains(">Message<") || checkCanMessage.Contains(">Messages<"))
                        {
                            if (FollowerModel.ChkAddMessageChecked)
                            {
                                var sendMessage = quoraFunct.SendMessage(DominatorAccountModel, quoraUser.Formkey,
                                    quoraUser.PostKey, quoraUser.WindowId, quoraUser.UserId, FollowerModel.Message);

                                if (sendMessage.Success)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora,
                                        DominatorAccountModel.UserName, ActivityType,
                                        $"Sent Message to {quoraUser.Username}");
                                    dbCampaignService.Add(new InteractedPosts
                                    {
                                        Username = quoraUser.Username,
                                        Message = FollowerModel.Message,
                                        InteractionDate = DateTime.Now
                                    });
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "Message option is not there");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            #endregion
        }
    }
}