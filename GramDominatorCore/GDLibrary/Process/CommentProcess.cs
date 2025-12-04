using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class CommentProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public CommentModel CommentModel { get; set; }

        private readonly Queue<ManageCommentModel> _queManageCommentModel = new Queue<ManageCommentModel>();

        private QueryInfo _lastUsedQueryInfo;

        private string _lastUsedMentionUsers = string.Empty;

        private List<string> _lstMentionUsers = new List<string>();

        public readonly object ObjectUniqueMention = new object();
        public static readonly object MultipleCommentLockFirstOption = new object();
        public static readonly object MultipleCommentLockSecondOption = new object();
        public static readonly object UniqueMentionAndComment = new object();
        private static Dictionary<string, List<string>> UniqueCommentPerPost { get; set; } = new Dictionary<string, List<string>>();

        private static Dictionary<string, List<string>> AllowMultiplecommentOnSamePostFirstOption { get; set; } = new Dictionary<string, List<string>>();
        private static List<string> AllowMultiplecommentOnSamePostSecondOption { get; } = new List<string>();

        public static readonly Object UniqueLock = new object();
        private int ActionBlockedCount;

        private readonly Dictionary<string, int> _commentList = new Dictionary<string, int>();

        private List<InteractedPosts> LstInteractedPostsForCampaign { get; set; } = new List<InteractedPosts>();

        public CommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdLogInProcess logInProcess, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            CommentModel = JsonConvert.DeserializeObject<CommentModel>(templateModel.ActivitySettings);
            loginProcess = logInProcess;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);
                instaFunct = loginProcess.InstagramFunctFactory.InstaFunctions;
                string commentTexts = string.Empty;
            UniqueComment:
                int delay = ModuleSetting.DelayBetweenEachActionBlock.GetRandom();
                InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
                string lastComments = string.Empty;
                if (CommentModel.LstDisplayManageCommentModel.Count != 0 && ModuleSetting.IsUniqueComment)
                    lastComments = CommentModel.LstDisplayManageCommentModel.Last().CommentText;

                //Allow Multiple comment and mention in one post with Unique Features
                if (ModuleSetting.IsUniqueCommentAndMention && CommentModel.IsChkMultipleCommentsOnSamePost || CommentModel.IsChkMultipleMentionOnSamePost)
                    commentTexts = AllowMultpleCommentAndMentionWithUniqueFeatures(scrapeResult, jobProcessResult);

                //Only allow Multiple Comment and mention in one post
                if (!ModuleSetting.IsUniqueCommentAndMention && CommentModel.IsChkMultipleCommentsOnSamePost || CommentModel.IsChkMultipleMentionOnSamePost)
                    commentTexts = AllowMultipleCommentAndMentionOnOnePost(scrapeResult, jobProcessResult);

                if (commentTexts == "NoMoreData")
                    return jobProcessResult;

                string commentText = commentTexts;
                if (string.IsNullOrEmpty(commentText))
                    commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;

                if (ModuleSetting.IsUniqueMention && string.IsNullOrEmpty(lastComments))
                {
                    if (_lstMentionUsers.Count != 0 && !string.IsNullOrEmpty(commentText))
                        lastComments = _lstMentionUsers.Last();
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                           "No Unique Mention is available for comment");
                        Stop();
                    }
                }
                lock (UniqueMentionAndComment)
                {
                #region Unique Comment
                checkComment:
                    if (!CommentModel.IsChkMultipleCommentsOnSamePost && !CommentModel.IsChkMultipleMentionOnSamePost)
                    {
                        if (!CheckUniqueCommentFromEachAccount(jobProcessResult, scrapeResult, ref commentText))
                        {
                            if (ModuleSetting.IsUniqueComment || ModuleSetting.IsUniqueMention)
                            {
                                if (ModuleSetting.IsUniqueComment)
                                    commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;
                                else
                                {
                                    var GetPostedCommented = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();

                                    var checkUniqueMention = _lstMentionUsers.Where(x => GetPostedCommented.All(y => !x.Contains(y.Comment.Split('@')[1]))).ToList();
                                    if (checkUniqueMention.Count != 0)
                                        commentText = "@" + checkUniqueMention.FirstOrDefault();
                                    else
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                           "No Unique Mention is available for comment");
                                        Stop();
                                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    }
                                }

                                if (lastComments.Equals(commentText))
                                {
                                    if (ModuleSetting.IsPostUniqueCommentFromEachAccount)
                                    {
                                        var accountWiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.Username == DominatorAccountModel.AccountBaseModel.UserName);
                                        if (accountWiseComment.Count == CommentModel.LstDisplayManageCommentModel.Count())
                                        {
                                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                           "No Unique comment is available for comment");
                                            Stop();
                                        }
                                    }
                                    else
                                    {
                                        var checkUniqueComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.Comment == commentText);
                                        int isCommnetAvailable = checkUniqueComment.Where(x => x.Comment == commentText).Count();
                                        if (isCommnetAvailable == 0)
                                            goto checkComment;

                                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                             "No Unique comment is available for comment");
                                        Stop();
                                    }
                                }
                                else
                                    goto checkComment;
                            }
                            else
                                return jobProcessResult;
                        }
                    }
                }
                #endregion

                if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                    commentText = " " + SpinTexHelper.GetSpinText(commentText) + " ";

                try
                {
                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        CheckOffensiveCommentResponseHandler checkOffensiveComment = instaFunct.CheckOffensiveComment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.Pk, commentText);
                        if (checkOffensiveComment.is_offensive)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Your Comment is Offensive, please change your Comment");
                            return jobProcessResult;
                        }
                    }
                    var browser = GramStatic.IsBrowser;
                    if (scrapeResult.ResultUser == null)
                    {
                        var mediaInfo =
                            browser ?
                            instaFunct.GdBrowserManager.MediaInfo(DominatorAccountModel, scrapeResult.ResultPost.Code, JobCancellationTokenSource.Token)
                            : instaFunct.MediaInfo(DominatorAccountModel, AccountModel, scrapeResult.ResultPost.Code, JobCancellationTokenSource.Token).Result;
                        scrapeResult.ResultPost = mediaInfo.InstagramPost;
                    }
                    var response = 
                        browser ?
                        instaFunct.GdBrowserManager.Comment(DominatorAccountModel, AccountModel, instagramPost.Code, commentText, JobCancellationTokenSource.Token)
                        : instaFunct.Comment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.Code : instagramPost.Pk, commentText).Result;
                    
                    if (response != null && response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);

                        IncrementCounters();

                        AddCommentedDataToDataBase(scrapeResult, commentText);

                        // Do activity after comment action
                        DoAfterCommentAction(instagramPost, scrapeResult);

                        jobProcessResult.IsProcessSuceessfull = true;

                    }
                    else if (response != null && (response.ToString().Contains("Comments on this post have been limited") || response.Issue.Message == "CommentLimited"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                 $"{scrapeResult.ResultPost.Code} Comments on this post have been limited by instagram, Please comment on diffrent post");
                        return jobProcessResult;
                    }
                    else if (!response.Success && response.Issue != null && response.Issue.Message == "You must write ContentLength bytes to the request stream before calling [Begin]GetResponse.")
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        if (response.ToString().Contains("This block will expire on"))
                        {
                            string expireDate = Utilities.GetBetween(response.ToString(), "This block will expire on", ".");
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" action has been blocked.This block will expire on {expireDate}");
                            Stop();
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        else if (response.ToString().Contains("Action Blocked") && response.ToString().Contains("\"feedback_required\""))
                        {
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(30));//Thread.Sleep(TimeSpan.FromSeconds(30));
                            bool LoginStatus = false;
                            var BackupCookie = DominatorAccountModel.Cookies;
                            var logOutStatus = instaFunct.Logout(DominatorAccountModel, AccountModel);
                            if (logOutStatus.Success)
                            {
                                ResetCookies(BackupCookie);
                                LoginStatus = loginProcess.LoginWithAlternativeMethodForBlocking(DominatorAccountModel);
                            }
                            if (LoginStatus)
                            {

                                CheckOffensiveCommentResponseHandler checkOffensiveComment = instaFunct.CheckOffensiveComment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.Pk, commentText);
                                if (checkOffensiveComment.is_offensive)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Your Comment is Offensive, please change your Comment");
                                    return jobProcessResult;
                                }
                                response = instaFunct.Comment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.Code : instagramPost.Pk, commentText).Result;
                                if (response != null && response.Success)
                                {
                                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                               DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);

                                    IncrementCounters();

                                    AddCommentedDataToDataBase(scrapeResult, commentText);

                                    // Do activity after comment action
                                    DoAfterCommentAction(instagramPost, scrapeResult);

                                    jobProcessResult.IsProcessSuceessfull = true;
                                }
                                else if (response != null && (response.ToString().Contains("Comments on this post have been limited") || response.Issue.Message == "CommentLimited"))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                             $"{scrapeResult.ResultPost.Code} Comments on this post have been limited by instagram, Please comment on diffrent post");
                                    return jobProcessResult;
                                }
                                else
                                {
                                    RemoveFailedCommentedDataFromDataBase(scrapeResult);
                                    if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref ActionBlockedCount, delay))
                                    {
                                        Stop();
                                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    }
                                    jobProcessResult.IsProcessSuceessfull = false;
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                      $"please check your account manually once");
                                Stop();
                            }
                        }
                        else
                        {
                            if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref ActionBlockedCount, delay))
                            {
                                Stop();
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                    }

                    // Delay between each activity
                    if (CommentModel != null && CommentModel.EnableDelayBetweenPerformingActionOnSamePost)
                        DelayBeforeNextActivity(CommentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                    else
                        DelayBeforeNextActivity();
                    if (CommentModel.IsChkMultipleCommentsOnSamePost || CommentModel.IsChkMultipleMentionOnSamePost)
                        goto UniqueComment;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            catch (Exception)
            {
                //ignored
            }
            return jobProcessResult;
        }
        public void ResetCookies(CookieCollection Cookies)
        {
            DominatorAccountModel.Cookies = new CookieCollection();
            foreach (Cookie cookie in Cookies)
            {
                var cookieHelper = new CookieHelper();
                cookieHelper.Name = cookie.Name;
                cookieHelper.Value = cookie.Value;
                cookieHelper.Domain = cookie.Domain;
                cookieHelper.Expires = cookie.Expires;
                cookieHelper.HttpOnly = cookie.HttpOnly;
                cookieHelper.Secure = cookie.Secure;

                if (cookie.Name.Contains("mid") || cookie.Name.Contains("csrftoken") || cookie.Name.Contains("sessionid") || cookie.Name.Contains("ds_user_id")
                    || cookie.Name.Contains("rur") || cookie.Name.Contains("ds_user") || cookie.Name.Contains("igfl"))
                {
                    DominatorAccountModel.CookieHelperList.Add(cookieHelper);
                    DominatorAccountModel.Cookies.Add(cookie);
                }

            }
        }
        private string GetCommentModel(QueryInfo queryInfo)
        {
            string comments;
            if (_lastUsedQueryInfo == null)
                _lastUsedQueryInfo = queryInfo;

            else
            {
                if (queryInfo.QueryType != _lastUsedQueryInfo.QueryType ||
                    queryInfo.QueryValue != _lastUsedQueryInfo.QueryValue)
                {
                    _queManageCommentModel.Clear();
                    _lastUsedQueryInfo = queryInfo;
                }
            }

            if (_queManageCommentModel.Count == 0)
            {
                var getManageCommentModels = CommentModel.LstDisplayManageCommentModel.Where(x =>
                    x.SelectedQuery.FirstOrDefault(y =>
                        (y.Content.QueryType == queryInfo.QueryType) &&
                        (y.Content.QueryValue.Contains(queryInfo.QueryValue))) != null).ToList();
                if (ModuleSetting.IsCommentOnceFromEachAccount)
                {
                    getManageCommentModels.Distinct();
                    if (!_commentList.ContainsKey(queryInfo.QueryType))
                        _commentList.Add(queryInfo.QueryType, getManageCommentModels.Count);
                }
                getManageCommentModels.ForEach(x => _queManageCommentModel.Enqueue(x));
            }
            // It makes "_queManageMessagesModel" Queue into a Circular Queue.

            try
            {
                var manageMessagesModel = _queManageCommentModel.Dequeue();
                _queManageCommentModel.Enqueue(manageMessagesModel);
                comments = manageMessagesModel.CommentText;
            }
            catch (Exception)
            {
                comments = "";
            }

            // Mention users in comment feature
            #region Mention users in comment feature
            if (CommentModel.IsChkMentionRandomUsers)
            {
                if (_lstMentionUsers.Count == 0)
                    _lstMentionUsers = Regex.Split(CommentModel.MentionUsers, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();


                //Chekcking purpose comment 
                //  _lstMentionUsers.Shuffle();
                _lastUsedMentionUsers = string.Empty;

                if (ModuleSetting.IschkUniqueMentionPerPostFromEachAccount)
                {
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(10));//Thread.Sleep(10 * 1000);
                    List<string> mentionUserList = new List<string>();
                    var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
                    LstInteractedPostsForCampaign = dboperationCampaign.Get<InteractedPosts>();
                    foreach (var t in LstInteractedPostsForCampaign)
                    {
                        string mUsers = t.Comment;
                        List<string> splitUsers = mUsers.Split(' ').ToList().Select(x => x.Replace("@", "")).ToList();
                        foreach (string splitsUsers in splitUsers)
                        {
                            mentionUserList.Add(splitsUsers);
                        }
                    }
                    _lstMentionUsers.RemoveAll(x => mentionUserList.Contains(x));

                    if (_lstMentionUsers.Count == 0)
                    {
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    int mentionUserCount = CommentModel.RandomlyGeneratedUsers.GetRandom();

                    for (int mentionUserIterCount = 0; mentionUserIterCount < _lstMentionUsers.Count; mentionUserIterCount++)
                    {
                        _lastUsedMentionUsers += $" @{_lstMentionUsers[mentionUserIterCount]}";

                        if ((mentionUserIterCount + 1) >= mentionUserCount)
                            break;
                    }
                }
                else
                {
                    int mentionUserCount = CommentModel.RandomlyGeneratedUsers.GetRandom();
                    if (ModuleSetting.IsUniqueMention && ModuleSetting.IsCommentOnceFromEachAccount)
                    {
                        List<string> lstOfMentionedComment = new List<string>();
                        var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
                        LstInteractedPostsForCampaign = dboperationCampaign.Get<InteractedPosts>();
                        foreach (InteractedPosts interactedPosts in LstInteractedPostsForCampaign)
                        {
                            var interactedPost = interactedPosts.Comment.TrimStart().Split(' ');
                            lstOfMentionedComment.AddRange(interactedPost);
                        }
                        _lstMentionUsers.RemoveAll(x => lstOfMentionedComment.Any(y => y.Trim('@') == x));
                        for (int mentionUserIterCount = 0; mentionUserIterCount < _lstMentionUsers.Count; mentionUserIterCount++)
                        {
                            _lastUsedMentionUsers += $" @{_lstMentionUsers[mentionUserIterCount]}";
                            if ((mentionUserIterCount + 1) >= mentionUserCount)
                                break;
                        }
                    }
                    else
                    {
                        //List<string> _lstCommentMention = new List<string>();

                        var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
                        LstInteractedPostsForCampaign = dboperationCampaign.Get<InteractedPosts>().Where(x => x.Username == DominatorAccountModel.UserName).ToList(); ;
                        List<string> leftComments = new List<string>();
                        foreach (InteractedPosts interactedPosts in LstInteractedPostsForCampaign)
                        {
                            var interactedPost = interactedPosts.Comment.TrimStart().Split(' ');
                            leftComments.AddRange(interactedPost);
                        }
                        // _lstCommentMention = _lstMentionUsers;
                        var removedComment = _lstMentionUsers.Where(x => leftComments.Any(y => y.Trim('@') == x)).ToList();
                        _lstMentionUsers.RemoveAll(x => leftComments.Any(y => y.Trim('@') == x));
                        for (int mentionUserIterCount = 0; mentionUserIterCount < _lstMentionUsers.Count; mentionUserIterCount++)
                        {
                            _lastUsedMentionUsers += $" @{_lstMentionUsers[mentionUserIterCount]}";

                            if ((mentionUserIterCount + 1) >= mentionUserCount)
                                break;
                        }
                        _lstMentionUsers.AddRange(removedComment);
                    }

                }
            }
            return comments;

            #endregion
        }

        private void AddCommentedDataToDataBase(ScrapeResultNew scrapeResult, string comment)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;

            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost || ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                {
                    string permalink = instagramPost.Code.GetUrlFromCode();

                    var interactedPost =
                        CampaignDbOperation.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                            x => x.Permalink == permalink && x.ActivityType == ActivityType &&
                                 x.Username == DominatorAccountModel.AccountBaseModel.UserName &&
                                 (x.Status == "Pending" || x.Status == "Working"));

                    if (interactedPost != null)
                    {
                        interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                        interactedPost.MediaType = instagramPost.MediaType;
                        interactedPost.ActivityType = ActivityType;
                        interactedPost.PkOwner = instagramPost.Code;
                        interactedPost.UsernameOwner = instagramPost.User.Username;
                        interactedPost.Username = DominatorAccountModel.AccountBaseModel.UserName;
                        interactedPost.Comment = comment;
                        interactedPost.QueryType = scrapeResult.QueryInfo.QueryType;
                        interactedPost.QueryValue = scrapeResult.QueryInfo.QueryValue;
                        interactedPost.Status = "Success";
                        CampaignDbOperation.Update(interactedPost);
                    }
                }
                else
                {
                    CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = instagramPost.MediaType,
                        ActivityType = ActivityType,
                        PkOwner = instagramPost.Code,
                        UsernameOwner = instagramPost.User?.Username,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        Comment = comment,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        Status = "Success"
                    });
                }
            }

            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
       new InteractedPosts()
       {
           InteractionDate = DateTimeUtilities.GetEpochTime(),
           MediaType = instagramPost.MediaType,
           ActivityType = ActivityType,
           PkOwner = instagramPost.Code,
           UsernameOwner = instagramPost.User?.Username,
           Username = DominatorAccountModel.AccountBaseModel.UserName,
           Comment = comment,
           QueryType = scrapeResult.QueryInfo.QueryType,
           QueryValue = scrapeResult.QueryInfo.QueryValue
       });
        }

        private void DoAfterCommentAction(InstagramPost instagramPost, ScrapeResultNew scrapeResult)
        {
            int delay = ModuleSetting.DelayBetweenEachActionBlock.GetRandom();
            if (CommentModel.IsChkAfterCommentAction)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, "Started After Comment Action functionality");

                #region Like post after comment
                if (CommentModel.IsChkLikePostAfterComment)
                {
                    DelatBetweenAfterCommentProcesses(ActivityType.Like);
                    if (instagramPost.HasLiked)
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Post : {instagramPost.Code} is already liked by the user");

                    else
                    {
                        var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                        var browser = GramStatic.IsBrowser;
                        var likeResponse = 
                            browser ?
                            instaFunct.GdBrowserManager.Like(DominatorAccountModel, AccountModel, instagramPost.Code, JobCancellationTokenSource.Token)
                            : instaFunct.Like(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.Code , instagramPost.User.Username, instagramPost.User.Pk, scrapeResult.QueryInfo).Result;
                        if (likeResponse.Success)
                        {
                            LikeAfterCommentAction(likeResponse, instagramPost, scrapeResult);
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram,
                                DominatorAccountModel.UserName, ActivityType,
                                likeResponse.Success
                                    ? $"Successfully liked the post having code : {instagramPost.Code}"
                                    : $"Failed to like the post having code : {instagramPost.Code}");
                        }
                        else
                        {
                            if (likeResponse.ToString().Contains("Please wait a few minutes before you try again.") || likeResponse.ToString().Contains("\"feedback_required\""))
                            {
                                if (likeResponse.ToString().Contains("This block will expire on"))
                                {
                                    string expireDate = Utilities.GetBetween(likeResponse.ToString(), "This block will expire on", ".");
                                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Like,
                                  instagramPost.Code, $": action has been blocked");
                                    CommentModel.IsChkLikePostAfterComment = false;
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, "Like",
                                           $"Your account has been blocked for {ActivityType.Like} operation.This block will expire on {expireDate}");
                                    return;
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Like,
                                   instagramPost.Code, ": action has been blocked.");
                                    CommentModel.IsChkLikePostAfterComment = false;

                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, "Like",
                                            $"Your account has been blocked for {ActivityType.Like} operation. Hence  {ActivityType.Like} activity will be enable after a few minutes to keep your account secure.");
                                }

                            }
                        }

                    }
                }
                #endregion

                #region Follow user after comment
                if (CommentModel.IsChkFollowUserAfterComment)
                {
                    DelatBetweenAfterCommentProcesses(ActivityType.Follow);
                    UserFriendshipResponse friendshipResponse = null;
                    var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                    if (IsBrowser)
                        friendshipResponse = instaFunct.GdBrowserManager.UserFriendship(DominatorAccountModel, AccountModel, instagramPost.User.UserId, JobCancellationTokenSource.Token);
                    else
                        friendshipResponse = instaFunct.UserFriendship(DominatorAccountModel, AccountModel, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.User.Username : instagramPost.User.Pk);

                    if (!friendshipResponse.Following)
                    {
                        FriendshipsResponse followResponse = null;
                        if (GramStatic.IsBrowser)
                            followResponse = instaFunct.GdBrowserManager.Follow(DominatorAccountModel, JobCancellationTokenSource.Token, instagramPost.User);
                        else
                            followResponse = instaFunct.Follow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.User.Pk);
                        if (followResponse.Success)
                        {
                            FollowAfterCommentAction(followResponse, instagramPost.User, scrapeResult);
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram,
                                DominatorAccountModel.UserName, ActivityType,
                                followResponse.Success
                                    ? $"Successfully followed post owner : {instagramPost.User.Username}"
                                    : $"Failed to follow post owner : {instagramPost.User.Username}");
                        }
                        else if (followResponse.Success && !followResponse.Following)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                               $"Sorry, You are unable to follow this user {instagramPost.User.Username} as your instagram account blocked , please check once manually");
                            Stop();

                        }
                        else if (followResponse.ToString().Contains("Sorry, you're following the max limit of accounts"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Sorry, you're following the max limit of accounts. You'll need to unfollow some accounts to start following more.");
                            Stop();
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            if (followResponse.ToString().Contains("Please wait a few minutes before you try again.") || followResponse.ToString().Contains("\"feedback_required\""))
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    scrapeResult.ResultUser.Username, ": action has been blocked.");

                                ActionBlockedCount++;

                                if (ActionBlockedCount >= 4)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Your account has been blocked for {ActivityType} operation for last 4 times. Hence {ActivityType} activity will automatically disabled for current Job and will start on next Job");

                                    Stop();
                                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Your account has been blocked for {ActivityType} operation. Hence {ActivityType} activity will be enable after a {delay} minutes to keep your account secure.");
                                    delayservice.ThreadSleep(TimeSpan.FromSeconds(delay));//Thread.Sleep(TimeSpan.FromMinutes(delay));
                                }
                            }
                            else
                                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser?.Username, followResponse.Issue?.Message);

                        }
                    }
                    #endregion
                }
            }
        }

        private void DelatBetweenAfterCommentProcesses(ActivityType activityTypeAfterFollow)
        {
            int delay;

            switch (activityTypeAfterFollow)
            {
                case ActivityType.Like:
                    delay = CommentModel.DelayBetweenLikesForAfterActivity.GetRandom();
                    break;
                case ActivityType.Follow:
                    delay = CommentModel.DelayBetweenFollowForAfterActivity.GetRandom();
                    break;
                default:
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"No such activity found for AfterActivity : {activityTypeAfterFollow} process");
                    return;
            }

            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Delaying process for {delay} second{(delay > 1 ? "s" : "")} for after activity : {activityTypeAfterFollow} process");

            delayservice.ThreadSleep(TimeSpan.FromSeconds(delay));//Thread.Sleep(delay * 1000);
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public void RemoveFailedCommentedDataFromDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
                InstagramPost post = (InstagramPost)scrapeResult.ResultPost;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost || ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        string permalink = post.Code.GetUrlFromCode();

                        var interactedPost = dboperationCampaign.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                                x => x.Permalink == permalink && x.Username == DominatorAccountModel.AccountBaseModel.UserName && (x.Status == "Pending" || x.Status == "Working"));
                        if (interactedPost != null)
                            dboperationCampaign.Remove(interactedPost);

                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private bool CheckUniqueCommentFromEachAccount(JobProcessResult jobProcessResult, ScrapeResultNew scrapeResult, ref string commentText)
        {
            try
            {
                string UniqueComment = commentText;
                List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts> postwiseComment = new List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();
                List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts> accountwiseComment = new List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();
                List<string> leftMention = new List<string>();
                if ((ModuleSetting.IsCommentOnceFromEachAccount && ModuleSetting.IsUniqueCommentAndMention && ModuleSetting.IsUniqueComment) ||
                   (ModuleSetting.IsCommentOnceFromEachAccount && ModuleSetting.IsUniqueCommentAndMention && ModuleSetting.IsUniqueMention))
                {
                    int mentionAndCommentCount = UniqueComment.Split('@').Count();
                    if (mentionAndCommentCount > 2)
                    {
                        var mentionAndComment = UniqueComment.TrimStart().Split('@');
                        foreach (string mention in mentionAndComment)
                        {
                            if (string.IsNullOrEmpty(mention))
                                continue;
                            var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                            instance.AddInteractedData(SocialNetworks, $"{CampaignId}.comment", mention);
                        }
                    }
                    else
                    {
                        var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                        instance.AddInteractedData(SocialNetworks, $"{CampaignId}.comment", UniqueComment);
                    }
                }
                if ((ModuleSetting.IsPostUniqueCommentFromEachAccount && ModuleSetting.IsUniqueCommentAndMention && ModuleSetting.IsUniqueComment) ||
                    (ModuleSetting.IsPostUniqueCommentFromEachAccount && ModuleSetting.IsUniqueCommentAndMention && ModuleSetting.IsUniqueMention))
                {
                    lock (UniqueLock)
                    {
                    mentionComment:
                        var lastData = new List<string>();
                        bool postCommentCheck = false;
                        if (!UniqueCommentPerPost.ContainsKey(scrapeResult.ResultPost.Code))
                            UniqueCommentPerPost.Add(scrapeResult.ResultPost.Code, lastData);
                        else
                            lastData = UniqueCommentPerPost[scrapeResult.ResultPost.Code];

                        bool postWiseCommentCheck = lastData.Contains(UniqueComment);
                        accountwiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.Username == DominatorAccountModel.AccountBaseModel.UserName);
                        bool accountCommentCheck = accountwiseComment.Any(x => x.Comment == UniqueComment);
                        if (!postWiseCommentCheck)
                        {
                            postwiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.PkOwner == scrapeResult.ResultPost.Code);
                            postCommentCheck = postwiseComment.Any(x => x.Comment == UniqueComment);
                        }
                        if (postWiseCommentCheck || accountCommentCheck || postCommentCheck)
                        {
                            if (ModuleSetting.IsUniqueComment)
                                return false;
                            else
                            {
                                if (postWiseCommentCheck || postCommentCheck)
                                {
                                    leftMention = _lstMentionUsers.Where(x => postwiseComment.All(y => !x.Contains(y.Comment.Split('@')[1]))).ToList();
                                    if (postwiseComment.Count == 0)
                                    {
                                        var postComments = UniqueCommentPerPost[scrapeResult.ResultPost.Code];
                                        leftMention = leftMention.Where(x => postComments.All(y => !x.Contains(y.Split('@')[1]))).ToList();
                                    }
                                }
                                else
                                    leftMention = _lstMentionUsers.Where(x => accountwiseComment.All(y => !x.Contains(y.Comment.Split('@')[1]))).ToList();

                                if (leftMention.Count() != 0)
                                {
                                    UniqueComment = " @" + leftMention.FirstOrDefault();
                                    goto mentionComment;
                                }
                            }
                        }

                        UniqueCommentPerPost[scrapeResult.ResultPost.Code].Add(UniqueComment);
                        commentText = UniqueComment;
                    }
                }
            }
            catch (Exception)
            {
                jobProcessResult.IsProcessSuceessfull = false;
                UniqueCommentDbCheck(scrapeResult.QueryInfo);

                return false;
            }

            return true;
        }

        public string UniqueMentionGetCommentModel(ScrapeResultNew scrapeResult)
        {
            string commentText;
            lock (ObjectUniqueMention)
            {
                commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;
            }
            return commentText;
        }

        public void UniqueCommentDbCheck(QueryInfo queryInfo)
        {
            var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
            LstInteractedPostsForCampaign = dboperationCampaign.Get<InteractedPosts>(x => x.QueryType == queryInfo.QueryType);
            int totalCommentCount = _commentList[queryInfo.QueryType];

            if (LstInteractedPostsForCampaign.Count == totalCommentCount)
                Stop();
        }

        public void LikeAfterCommentAction(LikeResponse likeResponse, InstagramPost instagramPost, ScrapeResultNew scrapeResult)
        {
            if (likeResponse.Success)
            {
                // Add data to respected campaign InteractedPosts table
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = instagramPost.MediaType,
                        ActivityType = ActivityType.Comment,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        PkOwner = instagramPost.Code,
                        UsernameOwner = instagramPost.User.Username,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        Status = "Liked"
                    });
                }

                // Add data to respected Account InteractedPosts table
                AccountDbOperation.Add(
                    new InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = instagramPost.MediaType,
                        ActivityType = ActivityType.Comment,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        PkOwner = instagramPost.Code,
                        UsernameOwner = instagramPost.User.Username,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        Status = "Liked"
                    });
            }
        }
        public void FollowAfterCommentAction(FriendshipsResponse followResponse, InstagramUser instagramUser, ScrapeResultNew scrapeResult)
        {
            try
            {
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation?.Add(
                            new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                            {
                                ActivityType = ActivityType.Comment.ToString(),
                                Date = DateTimeUtilities.GetEpochTime(),
                                QueryType = scrapeResult.QueryInfo.QueryType,
                                Query = scrapeResult.QueryInfo.QueryValue,
                                Username = DominatorAccountModel.AccountBaseModel.UserName,
                                InteractedUsername = instagramUser.Username,
                                InteractedUserId = instagramUser.Pk,
                                FollowedBack = followResponse.FollowedBack ? 1 : 0,
                                IsPrivate = followResponse.IsPrivate,
                                Time = DateTimeUtilities.GetEpochTime(),
                                Status = "Followed"
                            });
                }

                // Add data to respected Account InteractedUsers table;
                AccountDbOperation.Add(new InteractedUsers()
                {
                    ActivityType = ActivityType.Comment.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = instagramUser.Username,
                    InteractedUserId = instagramUser.Pk,
                    FollowedBack = followResponse.FollowedBack ? 1 : 0,
                    IsPrivate = followResponse.IsPrivate,
                    Time = DateTimeUtilities.GetEpochTime(),
                    Status = "Followed"

                });

                if (followResponse.IsPrivate)
                {
                    AccountDbOperation.Add(new Friendships()
                    {
                        Username = instagramUser.Username,
                        IsPrivate = instagramUser.IsPrivate,
                        IsVerified = instagramUser.IsVerified,
                        UserId = instagramUser.Pk,
                        FullName = instagramUser.FullName,
                        HasAnonymousProfilePicture = (instagramUser.HasAnonymousProfilePicture == true),
                        ProfilePicUrl = instagramUser.ProfilePicUrl,
                        FollowType = FollowType.Requested,
                        IsFollowBySoftware = true,
                        Time = DateTimeUtilities.GetEpochTime()
                    });
                }
                else
                {
                    AccountDbOperation.Add(new Friendships()
                    {
                        Username = instagramUser.Username,
                        IsPrivate = instagramUser.IsPrivate,
                        IsVerified = instagramUser.IsVerified,
                        UserId = instagramUser.Pk,
                        FullName = instagramUser.FullName,
                        HasAnonymousProfilePicture = (instagramUser.HasAnonymousProfilePicture == true),
                        ProfilePicUrl = instagramUser.ProfilePicUrl,
                        Followings = 1,
                        FollowType = FollowType.Following,
                        IsFollowBySoftware = true,
                        Time = DateTimeUtilities.GetEpochTime()
                    });

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string AllowMultpleCommentAndMentionWithUniqueFeatures(ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult)
        {
            string commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;
            if ((CommentModel.IsChkMultipleCommentsOnSamePost && CommentModel.IsPostUniqueCommentFromEachAccount && ModuleSetting.IsUniqueComment))
            {
                string Comment = AllowMultipleCommentAndMentionUniqueFilterFirstOption(scrapeResult, jobProcessResult);
                return Comment;
            }
            if (CommentModel.IsChkMultipleCommentsOnSamePost && ModuleSetting.IsCommentOnceFromEachAccount && ModuleSetting.IsUniqueComment)
            {
                string Comment = AllowMultipleCommentAndMentionUniqueFilterSecondOption(scrapeResult, jobProcessResult);
                return Comment;
            }

            if ((CommentModel.IsChkMultipleMentionOnSamePost && CommentModel.IsPostUniqueCommentFromEachAccount && ModuleSetting.IsUniqueMention))
            {
                string Mention = AllowMultipleCommentAndMentionUniqueFilterFirstOption(scrapeResult, jobProcessResult);
                return Mention;
            }
            if (CommentModel.IsChkMultipleMentionOnSamePost && ModuleSetting.IsCommentOnceFromEachAccount && ModuleSetting.IsUniqueMention)
            {
                string Comment = AllowMultipleCommentAndMentionUniqueFilterSecondOption(scrapeResult, jobProcessResult);
                return Comment;
            }
            return commentText;
        }

        public string AllowMultipleCommentAndMentionUniqueFilterFirstOption(ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult)
        {
        UniqueComment:
            string commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;
            lock (MultipleCommentLockFirstOption)
            {
                var lastData = new List<string>();
                if (!AllowMultiplecommentOnSamePostFirstOption.ContainsKey(DominatorAccountModel.UserName))
                    AllowMultiplecommentOnSamePostFirstOption.Add(DominatorAccountModel.UserName, lastData);

                bool commentCheck = AllowMultiplecommentOnSamePostFirstOption[DominatorAccountModel.UserName].Contains(commentText);
                var accountWiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.Username == DominatorAccountModel.AccountBaseModel.UserName);
                bool accountCommentCheck = accountWiseComment.Any(x => x.Comment == commentText);
                if (!commentCheck && accountCommentCheck)
                    AllowMultiplecommentOnSamePostFirstOption[DominatorAccountModel.UserName].Add(commentText);

                if (commentCheck || accountCommentCheck)
                {
                    int noOfComments = 0;
                    if (CommentModel.IsChkMultipleMentionOnSamePost)
                        noOfComments = _lstMentionUsers.Count;
                    else
                        noOfComments = CommentModel.LstDisplayManageCommentModel.Count;

                    if (noOfComments == AllowMultiplecommentOnSamePostFirstOption[DominatorAccountModel.UserName].Count)
                    {
                        // Stop();
                        AllowMultiplecommentOnSamePostFirstOption.Remove(DominatorAccountModel.AccountBaseModel.UserName);
                        return "NoMoreData";
                    }
                    goto UniqueComment;
                }
                else
                {
                    AllowMultiplecommentOnSamePostFirstOption[DominatorAccountModel.UserName].Add(commentText);
                    return commentText;
                }
            }
        }

        public string AllowMultipleCommentAndMentionUniqueFilterSecondOption(ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult)
        {
        UniqueComment:
            string commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;
            lock (MultipleCommentLockSecondOption)
            {
                bool uniqueComment = AllowMultiplecommentOnSamePostSecondOption.Contains(commentText);
                var accountWiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>();
                bool accountCommentCheck = accountWiseComment.Any(x => x.Comment == commentText);
                if (!uniqueComment && accountCommentCheck)
                    AllowMultiplecommentOnSamePostSecondOption.Add(commentText);

                if (uniqueComment || accountCommentCheck)
                {
                    int noOfComments = 0;
                    if (CommentModel.IsChkMultipleMentionOnSamePost)
                        noOfComments = _lstMentionUsers.Count;
                    else
                        noOfComments = CommentModel.LstDisplayManageCommentModel.Count;

                    if (noOfComments == AllowMultiplecommentOnSamePostSecondOption.Count)
                    {
                        AllowMultiplecommentOnSamePostSecondOption.Remove(DominatorAccountModel.AccountBaseModel.UserName);
                        return "NoMoreData";
                    }
                    goto UniqueComment;
                }
                else
                {
                    AllowMultiplecommentOnSamePostSecondOption.Add(commentText);
                    return commentText;
                }
            }
        }

        public string AllowMultipleCommentAndMentionOnOnePost(ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult)
        {
            string commentText = GetCommentModel(scrapeResult.QueryInfo) + _lastUsedMentionUsers;
            int noOfComments = 0;
            if (CommentModel.IsChkMultipleCommentsOnSamePost)
                noOfComments = CommentModel.LstDisplayManageCommentModel.Count;
            else
                noOfComments = _lstMentionUsers.Count;

            var noOfPostedComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.Username == DominatorAccountModel.UserName);
            if (noOfPostedComment.Count >= noOfComments)
            {
                var leftComments = CommentModel.LstDisplayManageCommentModel.Where(x => noOfPostedComment.All(y => y.Comment != x.CommentText));
                if (leftComments.Count() != 0)
                    commentText = leftComments.FirstOrDefault().CommentText;
                else
                    return "NoMoreData";
            }


            return commentText;
        }

        public bool PostUniqueCommentOnASpecificPostFromEachAccount(string comments, ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult)
        {
            try
            {
                lock (UniqueLock)
                {
                    var lastData = new List<string>();
                    bool postCommentCheck = false;
                    if (!UniqueCommentPerPost.ContainsKey(scrapeResult.ResultPost.Code))
                        UniqueCommentPerPost.Add(scrapeResult.ResultPost.Code, lastData);
                    else
                        lastData = UniqueCommentPerPost[scrapeResult.ResultPost.Code];

                    bool postWiseCommentCheck = lastData.Contains(comments);
                    var accountwiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.Username == DominatorAccountModel.AccountBaseModel.UserName);
                    bool accountCommentCheck = accountwiseComment.Any(x => x.Comment == comments);
                    if (!postWiseCommentCheck)
                    {
                        var postwiseComment = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.PkOwner == scrapeResult.ResultPost.Code);
                        postCommentCheck = postwiseComment.Any(x => x.Comment == comments);
                    }
                    if (postWiseCommentCheck || accountCommentCheck || postCommentCheck)
                        return false;
                    UniqueCommentPerPost[scrapeResult.ResultPost.Code].Add(comments);
                }
            }
            catch (Exception)
            {

            }
            return true;
        }
        public bool UseSpecificCommentOnlyOnceFromAAccount(string comments, ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult)
        {
            try
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                instance.AddInteractedData(SocialNetworks, $"{CampaignId}.comment", comments);
            }
            catch (Exception)
            {
                jobProcessResult.IsProcessSuceessfull = false;
                UniqueCommentDbCheck(scrapeResult.QueryInfo);
                return false;
            }
            return true;
        }

    }
}
