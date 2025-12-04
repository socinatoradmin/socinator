using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using TwtDominatorCore.Interface;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using Unity;

namespace TwtDominatorCore.TDViewModel.Publisher
{
    public class TdPublisherJobProcess : PublisherJobProcess
    {
        private static readonly string defaultPath =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Socinator\Twitter";


        public static List<string> CurrentAccounts = new List<string>();

        // public static List<string> CurrentRunningAccounts = new List<string>();
        public static readonly object LockSameAccountRunning = new object();
        private IAccountScopeFactory _accountScopeFactory;
        private IAccountsFileManager _accountsFileManager;
        private IDelayService _delayService;
        private ITwtLogInProcess _logInProcess;

        private ITwitterFunctionFactory _twitterFunctionFactory;
        private ITwitterAccountSessionManager twitterAccountSession;
        private DominatorAccountModel domAccModel;

        public TdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            InitializeValues(accountId);
        }


        public TdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            InitializeValues(accountId);
        }

        public BrowserWindow _browserWindow { get; set; }
        private ITwitterFunctions _twitterFunction => _twitterFunctionFactory.TwitterFunctions;

        private void InitializeValues(string accountId)
        {
            try
            {
                _delayService = InstanceProvider.GetInstance<IDelayService>();
                twitterAccountSession = InstanceProvider.GetInstance<ITwitterAccountSessionManager>();
                _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                domAccModel = _accountsFileManager.GetAccountById(accountId);
                _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                var twitterFunctionFactory =
                    _accountScopeFactory[domAccModel.AccountId].Resolve<ITwitterFunctionFactory>();
                if (domAccModel.IsRunProcessThroughBrowser)
                    //twitterFunctionFactory.AssignTwitterFunctions(AccountModel);
                    twitterFunctionFactory.AssignHttpTwitterFunctions();
                _twitterFunctionFactory = twitterFunctionFactory;
                _logInProcess = _accountScopeFactory[domAccModel.AccountId].Resolve<ITwtLogInProcess>();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public override bool DeletePost(string postId)
        {
            // if we added this account thread then we make it true and remove from list, release the pulse
            // else don't need to add, release, pulse

            //InitializeValues(AccountModel.AccountId);
            var isAdded = false;
            try
            {
                // sometimes same account come here more than once at a same time
                // to avoid this we using lock

                #region lock region

                try
                {
                    if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                        Thread.CurrentThread.Name = AccountModel.UserName;

                    var postResponse = _twitterFunction.GetSingleTweetDetails(AccountModel,
                        postId);
                    if (!postResponse.Success)
                        return false;

                    lock (LockSameAccountRunning)
                    {
                        isAdded = true;
                        if (domAccModel == null)
                            domAccModel = _accountsFileManager.GetAccountById(AccountModel.AccountId);
                        if (CurrentAccounts.Contains(AccountModel.UserName))
                        {
                            CurrentAccounts.Add(AccountModel.UserName);
                            Monitor.Wait(LockSameAccountRunning);
                        }
                        else
                        {
                            CurrentAccounts.Add(AccountModel.UserName);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }

                #endregion

                StartDeletePost(postId);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
            finally
            {
                try
                {
                    if (isAdded)
                    {
                        CurrentAccounts.Remove(domAccModel.UserName);
                        Monitor.Pulse(LockSameAccountRunning);
                    }
                }
                catch (Exception)
                {
                    //
                }
            }

            return false;
        }

        public bool StartDeletePost(string postId)
        {
            var deletePostId = postId;
            if (!_logInProcess.CheckLogin(domAccModel, domAccModel.Token))
                return false;
            GlobusLogHelper.log.Info(Log.SuccessfulLogin, domAccModel.AccountBaseModel.AccountNetwork,
                domAccModel.AccountBaseModel.UserName);

            if (postId.Contains("/status/"))
                deletePostId = postId
                    .Substring(postId.IndexOf("/status/", StringComparison.Ordinal) + "/status/".Length)
                    .Replace("/", "");

            var deleteRespHandler = _twitterFunction.Delete(domAccModel, deletePostId);

            if (deleteRespHandler.Success)
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, domAccModel.AccountBaseModel.AccountNetwork,
                    domAccModel.UserName, ActivityType.Delete, deletePostId);
            else
                GlobusLogHelper.log.Info(Log.ActivityFailed, domAccModel.AccountBaseModel.AccountNetwork,
                    domAccModel.UserName, ActivityType.Tweet, deletePostId + " " + deleteRespHandler.Issue.Message);


            return deleteRespHandler.Success;
        }

        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails, bool IsDelay)
        {
            var isBrowser =
                domAccModel .IsRunProcessThroughBrowser;

            try
            {
                if (isBrowser)
                {
                    twitterAccountSession.AddOrUpdateSession(ref domAccModel);
                    TdAccountsBrowserDetails.GetInstance().CreateBrowser(domAccModel, domAccModel.Token, twitterAccountSession);

                    if (domAccModel.CookieHelperList.Count == 0)
                        _delayService.ThreadSleep(15000);
                    else
                        _delayService.ThreadSleep(10000);
                    var name = TdAccountsBrowserDetails.GetBrowserName(domAccModel);
                    _browserWindow = TdAccountsBrowserDetails.GetInstance().AccountBrowserCollections[name];

                    _twitterFunction.SetBrowser(domAccModel, new CancellationToken());
                }

                try
                {
                    // return if accountId, postdetail is null, 
                    // if post is empty have no description and media
                    if (string.IsNullOrEmpty(accountId) || postDetails == null ||
                        string.IsNullOrEmpty(postDetails.PostDescription?.Trim()) &&
                        postDetails.MediaList.Where(x => x.Trim().Length > 0).ToList().Count == 0&& postDetails.PostSource!=DominatorHouseCore.Enums.SocioPublisher.PostSource.SharePost)
                        return false;
                    Thread.CurrentThread.Name = domAccModel.UserName;
                }
                catch (Exception ex)
                {
                    ex.DebugLog("Null userId or postDetails");
                }

                domAccModel = _accountsFileManager.GetAccountById(accountId);

                if (!_logInProcess.CheckLogin(domAccModel, domAccModel.Token))
                    return false;


                //perform general settings
                var updatedpostlist = new PublisherPostlistModel();
                try
                {
                    if (postDetails.PostDescription == null)
                        postDetails.PostDescription = string.Empty;
                    updatedpostlist = PerformGeneralSettings(postDetails);
                    if (string.IsNullOrEmpty(postDetails.PostDescription))
                        updatedpostlist.PostDescription = postDetails.PostDescription;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                int deleteHours;

                var advanceUpdatedList = AdvanceSetting(updatedpostlist, out deleteHours);

                var accountModel = new AccountModel(domAccModel);

                if (string.IsNullOrEmpty(accountModel.CsrfToken))
                    accountModel.CsrfToken = domAccModel.Cookies.OfType<Cookie>()
                        .FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com")
                        ?.Value ?? domAccModel.Cookies.OfType<Cookie>()
                        .FirstOrDefault(x => x.Name == "ct0")
                        ?.Value;

                #region  posting

                try
                {
                    var IsContainVideo = false;
                    var IsContainVideoUrl = false;
                    var mediaList = new List<string>();
                    var TagUsers = new List<string>();

                    // here it check image contain in rssfeed or not and assign to mediafile.
                    if (advanceUpdatedList.TdPostSettings.RssImageList != null &&
                        advanceUpdatedList.TdPostSettings.RssImageList?.Count() != 0)
                        mediaList = advanceUpdatedList.TdPostSettings.RssImageList;


                    if (advanceUpdatedList.MediaList != null &&
                        advanceUpdatedList.MediaList.Where(x => x.Trim().Length > 0).ToList().Count != 0)
                    {
                        var videoPath = advanceUpdatedList.MediaList.FirstOrDefault();
                        if (videoPath.Contains("/status/"))
                        {
                            var currentTweetTag = new TagDetails();
                            currentTweetTag.IsTweetContainedVideo = true;
                            currentTweetTag.Username =
                                Utilities.GetBetween(videoPath, $"https://{TdConstants.Domain}/", "/status/");
                            currentTweetTag.Id = Utilities.GetBetween(videoPath + "$$", "/status/", "$$");
                            IsContainVideo = IsContainVideoUrl = GettingTweetMedia(currentTweetTag, ref mediaList);
                        }
                        else
                        {
                            IsContainVideo = advanceUpdatedList.MediaList.Any(media => media.Contains(".mp4") || media.Contains(".MP4"));
                            mediaList = advanceUpdatedList.MediaList.Select(x => x.Replace("_SOCINATORIMAGE.jpg", ""))
                                .ToList();
                            if (isBrowser && mediaList[0].Contains("https://video.twimg.com/"))
                            {
                                mediaList = new List<string>();
                                var currentTweetTag = new TagDetails();
                                currentTweetTag.IsTweetContainedVideo = true;
                                currentTweetTag.Id = advanceUpdatedList.FetchedPostIdOrUrl;
                                currentTweetTag.Username =
                                    Utilities.GetBetween($"{advanceUpdatedList.PdSourceUrl}/", "]", "/");
                                GettingTweetMedia(currentTweetTag, ref mediaList);
                            }
                        }
                    }

                    if (advanceUpdatedList.TdPostSettings.MentionUserList != null &&
                        advanceUpdatedList.TdPostSettings.IsMentionUser &&
                        !string.IsNullOrEmpty(advanceUpdatedList.TdPostSettings.MentionUserList))
                    {
                        if (advanceUpdatedList.TdPostSettings.MentionUserList.Contains("\r\n"))
                            TagUsers = postDetails.TdPostSettings.MentionUserList.Split('\n').Select(x => x.Trim()).ToList();
                        else
                            TagUsers = postDetails.TdPostSettings.MentionUserList.Split(',').Select(x => x.Trim()).ToList();
                        TagUsers = TagUsers.OrderBy(x => Guid.NewGuid()).ToList();
                    }
                    if (updatedpostlist.TdPostSettings.IsDeletePostAfterHours)
                        deleteHours = postDetails.TdPostSettings.DeletePostAfterHours;

                    // sometime not posting tweet in single attempt
                    // therefore we do two  more attempt to post an tweet 
                    dynamic TwtResponse = null;
                    var TweetId=string.Empty;
                    for (var index = 0; index < 3 && (TwtResponse is null || !TwtResponse.Success); index++)
                    {
                        if (index != 0)
                            _delayService.ThreadSleep(10000);
                        if(advanceUpdatedList.PostSource==DominatorHouseCore.Enums.SocioPublisher.PostSource.SharePost && !string.IsNullOrEmpty(advanceUpdatedList.ShareUrl))
                        {
                            var tweetDetails=_twitterFunction.GetSingleTweetDetails(domAccModel,advanceUpdatedList.ShareUrl)?.TweetDetails;
                            if (IsFiltered(tweetDetails,advanceUpdatedList))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    domAccModel.AccountBaseModel.AccountNetwork,
                                    domAccModel.AccountBaseModel.UserName,"Publish",
                                    $"Filter Not Matched For {TdUtility.GetTweetUrl(tweetDetails.Username,tweetDetails.Id)}");
                                return false;
                            }
                            TwtResponse = _twitterFunction.Retweet(domAccModel,tweetDetails.Id, tweetDetails.Username);
                            TweetId=tweetDetails.Id;
                        }
                        else
                        {
                            // here we passing query type so that secondary browser not get opened in publisher
                            TwtResponse = _twitterFunction.Tweet(domAccModel, advanceUpdatedList.PostDescription,
                                CombinedCancellationToken.Token, "", "", "Publish", ActivityType.Tweet, IsContainVideo,
                                mediaList, TagUsers);
                            TweetId=TwtResponse.TweetId;
                        }
                    }

                    #region after succesfully published

                    try
                    {
                        if (TwtResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                domAccModel.AccountBaseModel.AccountNetwork, domAccModel.UserName, ActivityType.Post,
                                $"at => {TdUtility.GetTweetUrl(domAccModel.UserName, TweetId)}");
                            if (isBrowser)
                                TdAccountsBrowserDetails.CloseAllBrowser(domAccModel);
                            UpdateAfterSuccessfullyPost(TweetId,ref advanceUpdatedList, deleteHours);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed, domAccModel.AccountBaseModel.AccountNetwork,
                                domAccModel.UserName, ActivityType.Post,$"{TweetId}==>{TwtResponse.Issue.Message}" );
                            UpdatePostWithFailed(AccountModel.AccountBaseModel.UserName, advanceUpdatedList,
                                TwtResponse.Issue.Message);
                        }

                        // deleting media after successfully posted
                        if (IsContainVideoUrl)
                        {
                            var videoPath = mediaList.FirstOrDefault();
                            if (File.Exists(videoPath))
                                File.Delete(videoPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog();
                    }

                    #endregion

                    if (IsDelay)
                        DelayBeforeNextPublish();

                    return TwtResponse.Success;
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (IsDelay)
                    DelayBeforeNextPublish();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }

            #endregion

            return false;
        }

        private bool IsFiltered(TagDetails tagDetails, PublisherPostlistModel publisherPostlist)
        {
            try
            {
                if (tagDetails != null && publisherPostlist != null && publisherPostlist.sharePostModel.IsPostBetween && !publisherPostlist.sharePostModel.PostBetween.InRange(tagDetails.LikeCount))
                    return true;
                if (tagDetails != null && publisherPostlist != null && publisherPostlist.sharePostModel.IsMinimumDays && tagDetails.DaysCount > publisherPostlist.sharePostModel.MinimumDays)
                    return true;
                return false;
            }
            catch { return false; }
        }

        private PublisherPostlistModel AdvanceSetting(PublisherPostlistModel postDetails, out int deleteAfterSomeHours)
        {
            deleteAfterSomeHours = 0;

            if (postDetails == null)
                return null;

            try
            {
                #region initializing values
                var twitterAdvanceModel = GenericFileManager.GetModuleDetails<TwitterModel>
                        (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Twitter))
                    .FirstOrDefault(x => x.CampaignId == postDetails.CampaignId);
                
                if (twitterAdvanceModel == null)
                    return postDetails;
                if (twitterAdvanceModel.IsDeletePostAfter)
                    deleteAfterSomeHours = twitterAdvanceModel.DeletePostAfter.GetRandom();


                var totalHashtagCount = 0;
                var extraHashTags = "";
                var description = postDetails.PostDescription == null ? "" : postDetails.PostDescription;

                #endregion

                #region Automatic hashTag

                try
                {
                    var hashKeywords = new List<string>();
                    var currentHashtagCount = 0;


                    if (twitterAdvanceModel.IsEnableAutomaticHashTags && twitterAdvanceModel.MaxHashtagsPerPost > 0 &&
                        twitterAdvanceModel.HashWords != null &&
                        !string.IsNullOrEmpty(twitterAdvanceModel.HashWords.Trim()))
                    {
                        hashKeywords = twitterAdvanceModel.HashWords.Split(',').Select(x => x.Trim()).ToList()
                            .OrderBy(order => Guid.NewGuid()).ToList();
                        hashKeywords.ForEach(keyword =>
                        {
                            // adding only those hashtah having word length greater than minimum word length
                            if (twitterAdvanceModel.MaxHashtagsPerPost > currentHashtagCount &&
                                twitterAdvanceModel.MinimumWordLength <= keyword.Length)
                            {
                                description += $" #{keyword}";
                                ++currentHashtagCount;
                            }
                        });
                    }

                    totalHashtagCount += currentHashtagCount;
                    description += extraHashTags;
                    postDetails.PostDescription = description;
                }
                catch (Exception ex)
                {
                    ex.DebugLog("Error in Automatic HashTag");
                }

                #endregion

                #region Dynamic Hashtag

                try
                {
                    description = postDetails.PostDescription == null ? "" : postDetails.PostDescription;
                    extraHashTags = "";

                    var ListHastag1 = new List<string>();
                    var ListHastag2 = new List<string>();
                    var maxDynamicHashTag = twitterAdvanceModel.MaxHashtagsPerPostRange.GetRandom();

                    if (twitterAdvanceModel.HashtagsFromList1 != null)
                        ListHastag1 = twitterAdvanceModel.HashtagsFromList1.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim())
                            .OrderBy(x => Guid.NewGuid()).ToList();

                    if (twitterAdvanceModel.HashtagsFromList2 != null)
                        ListHastag2 = twitterAdvanceModel.HashtagsFromList2.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim())
                            .OrderBy(x => Guid.NewGuid()).ToList();


                    var IsDynamicHashTag =
                        twitterAdvanceModel.IsEnableDynamicHashTags && twitterAdvanceModel
                                                                        .IsAddHashTagEvenIfAlreadyHastags
                                                                    && maxDynamicHashTag > totalHashtagCount &&
                                                                    (ListHastag1.Count > 0 || ListHastag2.Count > 0);
                    if (IsDynamicHashTag)
                    {
                        // picking hashtags from totals 
                        var pickFromList1 = twitterAdvanceModel.PickPercentHashTag *
                                            (ListHastag1.Count + ListHastag2.Count) / 100;
                        var pickFromList2 = twitterAdvanceModel.PickPercentFromList *
                                            (ListHastag2.Count + ListHastag1.Count) / 100;

                        // picking hashtags from first list as per %age
                        for (var i = 0; i < pickFromList1; i++)
                            if (maxDynamicHashTag > totalHashtagCount && ListHastag1.Count > i)
                            {
                                ++totalHashtagCount;
                                extraHashTags += $" #{ListHastag1[i]}";
                            }
                            else
                            {
                                break;
                            }

                        // picking hashtags from second list as per %age
                        for (var i = 0; i < pickFromList2; i++)
                            if (maxDynamicHashTag > totalHashtagCount && ListHastag2.Count > i)
                            {
                                ++totalHashtagCount;
                                extraHashTags += $" #{ListHastag2[i]}";
                            }
                            else
                            {
                                break;
                            }
                    }

                    description += extraHashTags;
                    postDetails.PostDescription = description;
                }
                catch (Exception ex)
                {
                    ex.DebugLog("Error in Dynamic HashTag");
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postDetails;
        }

        public bool GettingTweetMedia(TagDetails TagForProcess, ref List<string> FilePath)
        {
            var IsSuccess = true;

            try
            {
                if (!Directory.Exists(defaultPath))
                    Directory.CreateDirectory(defaultPath);

                if (TagForProcess.IsTweetContainedVideo)
                {
                    FilePath.AddRange(_twitterFunction.DownloadVideoUsingThirdParty(TagForProcess.Id,
                        TagForProcess.Username, defaultPath, TagForProcess.Id));
                    if (TdUtility.CheckDuration(new Uri(FilePath[0])))
                    {
                        GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason,
                            domAccModel.AccountBaseModel.AccountNetwork, domAccModel.UserName,
                            "couldn't repost with video size more than 140secs");
                        IsSuccess = false;
                    }
                }
                else
                {
                    FilePath = string.IsNullOrEmpty(TagForProcess.Code)
                        ? null
                        : new List<string>(Regex.Split(TagForProcess.Code, "\n"));
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                if (TagForProcess != null && TagForProcess.Username != null && TagForProcess.Id != null)
                    ex.DebugLog($"TagId {TagForProcess.Id} UserName {TagForProcess.Username}");
                //   //ex.DebugLog();(ex.ToString());
            }

            return IsSuccess;
        }

        private void UpdateAfterSuccessfullyPost(string TweetId,
            ref PublisherPostlistModel advanceUpdatedList, int deleteHours)
        {
            try
            {
                var DestinationUrl = AccountModel.AccountBaseModel.ProfileId;
                //$"https://{Domain}/{domAccModel.AccountBaseModel.ProfileId}";
                var Link = $"https://{TdConstants.Domain}/{domAccModel.AccountBaseModel.ProfileId}/status/{TweetId}";
                #region Setting Delete after some hour
                if(advanceUpdatedList.TdPostSettings.IsDeletePostAfterHours)
                {
                    try
                    {
                        var objDeletionModel = new PostDeletionModel();
                        var account = _accountsFileManager.GetAccountById(AccountModel.AccountBaseModel.AccountId);
                        if (advanceUpdatedList.TdPostSettings.DeletePostAfterHours != 0)
                                objDeletionModel.DeletionTime =
                                    DateTime.Now.AddHours(advanceUpdatedList.TdPostSettings.DeletePostAfterHours);
                        else if(deleteHours != 0)
                            objDeletionModel.DeletionTime =
                                    DateTime.Now.AddHours(deleteHours);
                        else
                            objDeletionModel.DeletionTime = DateTime.Now.AddYears(1);
                        #region assigning values to delete model
                        objDeletionModel.AccountId = account.AccountId;
                        objDeletionModel.CampaignId = CampaignId;
                        objDeletionModel.DestinationType = "OwnWall";
                        objDeletionModel.DestinationUrl = DestinationUrl;
                        objDeletionModel.PostId = advanceUpdatedList.PostId;
                        objDeletionModel.PublishedIdOrUrl = Link;
                        objDeletionModel.Networks = SocialNetworks.Twitter;
                        #endregion
                        PublishScheduler.EnableDeletePost(objDeletionModel);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                #endregion

                UpdatePostWithSuccessful(AccountModel.AccountBaseModel.UserName, advanceUpdatedList, Link);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}