using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThreadUtils;
using Unity;
using AccountFriends = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.Friends;
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class PostCommentorProcess : FdJobProcessInteracted<AccountInteractedPosts>
    {
        public PostCommentorModel PostCommentorModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        private static Dictionary<string, string> CommentPostPair { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> AccountCommentPair { get; set; } = new Dictionary<string, string>();

        public static Dictionary<string, string> UniqueAccountCommentPair { get; set; } = new Dictionary<string, string>();

        private static Dictionary<string, bool> UniqueCommentAvailability { get; } = new Dictionary<string, bool>();

        private static readonly object CommentLock = new object();
        private readonly IAccountScopeFactory _accountScopeFactory;

        private readonly IFdRequestLibrary _fdRequestLibrary;
        private FanpageDetails FanpageDetails;

        private readonly IDelayService _delayService;

        public PostCommentorProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

            _fdRequestLibrary = fdRequestLibrary;
            _delayService = delayService;
            PostCommentorModel = processScopeModel.GetActivitySettingsAs<PostCommentorModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();

            if (!UniqueCommentAvailability.ContainsKey(AccountModel.AccountId))
                UniqueCommentAvailability.Add(AccountModel.AccountId, false);

        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {

            JobProcessResult jobProcessResult = new JobProcessResult();
            bool processSuccess = false;

            var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();


            var campaignDetails = binFileHelper.GetCampaignDetail().FirstOrDefault(x => x.CampaignId == CampaignId);
            var noOfAccount = 1;

            if (campaignDetails != null)
                noOfAccount = campaignDetails.SelectedAccountList.Count;

            FacebookPostDetails objFacebookPostDetails = (FacebookPostDetails)scrapeResult.ResultPost;


            if ((CommentPostPair.Count == 0 || UniqueAccountCommentPair.Count == 0) &&
                   (PostCommentorModel.IschkUniqueCommentOptionChecked || PostCommentorModel.IschkAllowMultipleComment))
            {
                LoadSentCommentFromDb();
            }

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                List<string> commentList = new List<string>();

                string comment = string.Empty;

                var mentionDictionary = new Dictionary<string, string>();

                int commentCount = 1;
                if (PostCommentorModel.IschkAllowMultipleComment)
                    commentCount = PostCommentorModel.MaximumCommentPerPost.GetRandom();

                for (int i = 0; i < commentCount; i++)
                {
                    comment = PostCommentorModel.IschkUniqueCommentOptionChecked ? GetUniqueCommentDetails(objFacebookPostDetails)
                        : GetCommentDetailsNew(objFacebookPostDetails);
                    if (!string.IsNullOrEmpty(comment))
                    {
                        comment = comment.Replace("\r", string.Empty);
                        commentList.Add(comment);
                    }

                }

                string usedMention = string.Empty;
                if (PostCommentorModel.IsMentionUsersChecked)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        mentionDictionary = CommentWithMentionsBrowser(ref usedMention);
                    else
                        comment = CommentWithMentions(comment, ref usedMention);
                }


                if (commentList.Count > 0)
                {

                    if (!string.IsNullOrEmpty(objFacebookPostDetails.LikePostAsPageId) &&
                        AccountModel.IsRunProcessThroughBrowser && !FdLogInProcess._browserManager.isActorChangedtoFanPage)
                    {
                        var pageSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{objFacebookPostDetails.LikePostAsPageId}"]
                            .Resolve<IFdBrowserManager>();

                        FanpageDetails = pageSpecificWindow.GetFullPageDetails(AccountModel, new FanpageDetails()
                        { FanPageUrl = $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.LikePostAsPageId}" }).ObjFdScraperResponseParameters.FanpageDetails;

                        pageSpecificWindow.CloseBrowser(AccountModel);

                    }
                    bool isCommented = AccountModel.IsRunProcessThroughBrowser
                            ? FdLogInProcess._browserManager.CommentOnSinglePost(AccountModel, objFacebookPostDetails, commentList, objFacebookPostDetails.LikePostAsPageId, mentionDictionary, FanpageDetails)
                            : _fdRequestLibrary.CommentOnPost(AccountModel, objFacebookPostDetails.Id, comment, objFacebookPostDetails.LikePostAsPageId).ObjFdScraperResponseParameters.IsCommentedOnPost;

                    var urlforLog = objFacebookPostDetails.ScapedUrl.Contains("/reel/") && !objFacebookPostDetails.ScapedUrl.Contains(objFacebookPostDetails.Id)
                        ? objFacebookPostDetails.ScapedUrl : $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.Id}";
                    if (isCommented)
                    {
                        processSuccess = true;
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, urlforLog);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, urlforLog, "");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }

                    if (processSuccess)
                    {
                        IncrementCounters();
                        jobProcessResult.IsProcessSuceessfull = true;
                        commentList.ForEach(x => AddProfileScraperDataToDatabase(scrapeResult, x, usedMention));
                        StartAfterAction(scrapeResult);
                    }
                    if (PostCommentorModel.IschkUniqueCommentChecked || PostCommentorModel.IschkUniqueCommentForEachPostChecked)
                    {
                        if (UniqueCommentAvailability.ContainsKey(AccountModel.AccountId))
                            UniqueCommentAvailability[AccountModel.AccountId] = true;

                        if (UniqueCommentAvailability.Count >= noOfAccount &&
                            UniqueCommentAvailability.All(y => y.Value))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "No more unique comment available for this campaign!");
                            Stop();
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                        else
                        {
                            //GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "No more unique comment available!");
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                    }

                    if (PostCommentorModel.EnableDelayBetweenPerformingActionOnSamePost)
                        DelayBeforeNextActivity(PostCommentorModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                    else DelayBeforeNextActivity();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "No more unique comment available for this campaign!");
                    Stop();
                    jobProcessResult.IsProcessSuceessfull = false;
                }
            }
            catch (OperationCanceledException)
            {
                FdLogInProcess._browserManager.CloseBrowser(AccountModel);
                throw new OperationCanceledException();
            }
            catch (AggregateException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            return jobProcessResult;
        }

        private void StartAfterAction(ScrapeResultNew scrapeResult)
        {

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FacebookPostDetails facebookPostDetails = (FacebookPostDetails)scrapeResult.ResultPost;
                var url = facebookPostDetails.ScapedUrl.Contains("/reel/") && !facebookPostDetails.ScapedUrl.Contains(facebookPostDetails.Id)
                       ? facebookPostDetails.ScapedUrl : $"{FdConstants.FbHomeUrl}{facebookPostDetails.Id}";
                if (PostCommentorModel.IsLikePostChecked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Started After Action For PostCommenter {url}");

                    int delayBetweenComments = PostCommentorModel.DelayBetweenCommentsForAfterActivity.GetRandom();

                    bool isSuccess = false;
                    isSuccess = AccountModel.IsRunProcessThroughBrowser
                        ? FdLogInProcess._browserManager.LikePost(AccountModel, facebookPostDetails, ReactionType.Like, FanpageDetails)
                        : _fdRequestLibrary.LikeUnlikePost(AccountModel, facebookPostDetails.Id, ReactionType.Like);

                    if (isSuccess)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Successfully Like post with url {url}");

                        _delayService.ThreadSleep(delayBetweenComments * 1000);
                    }
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Failed to Like post with url {url}");

                }

                if (PostCommentorModel.IsSendFriendRequestChked && facebookPostDetails.EntityType.ToString() == "Friend")
                {
                    string ownerUrl = !string.IsNullOrEmpty(facebookPostDetails.OwnerId) ? $"{FdConstants.FbHomeUrl}{facebookPostDetails.OwnerId}"
                                   : Utilities.GetBetween($"/strt{facebookPostDetails.PostUrl}", "/strt", "/post/");
                    string isSentFriendRequest = string.Empty;
                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        FdLogInProcess._browserManager.LoadPageSource(AccountModel, ownerUrl);
                        isSentFriendRequest = FdLogInProcess._browserManager.SendFriendRequestSingleUser(AccountModel, facebookPostDetails.OwnerId);
                    }
                    else
                        isSentFriendRequest = _fdRequestLibrary.SendFriendRequest(AccountModel, facebookPostDetails.OwnerId);

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (isSentFriendRequest == "success")
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Successfully Sent FriendRequest {url}");
                    else
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "Send Request",
                            $"{url}");

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




        private void LoadSentCommentFromDb()
        {
            List<CampaignInteractedPosts> data = ObjDbCampaignService.GetAllInteractedData<CampaignInteractedPosts>();

            data.ForEach(x =>
            {
                try
                {
                    if (!UniqueAccountCommentPair.ContainsKey($"{CampaignId}_{x.Comment}"))
                    {
                        UniqueAccountCommentPair.Add($"{CampaignId}_{x.Comment}", $"{CampaignId}_{x.Comment}");
                    }
                    if (!CommentPostPair.ContainsKey($"{CampaignId}_{x.Comment}_{x.PostId}"))
                    {
                        CommentPostPair.Add($"{CampaignId}_{x.Comment}_{x.PostId}", $"{CampaignId}_{x.Comment}_{x.PostId}");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private string CommentWithMentions(string comment, ref string usedMentions)
        {
            var mention = string.Empty;

            string mentionsUsed = string.Empty;

            try
            {
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(AccountModel);

                var randomFriends = objFdFunctions.MentionFriends(AccountModel, PostCommentorModel.SelectOptionModel);

                randomFriends.RemoveAll(x => x == "");

                int noOfMentions = PostCommentorModel.NoOfUserMention.GetRandom();

                if (noOfMentions == 0)
                {
                    noOfMentions = PostCommentorModel.NoOfUserMention.GetRandom();
                }

                if (randomFriends.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Getting friend details for mention!");
                }

                randomFriends.ForEach(x =>
                {
                    var friendId = _fdRequestLibrary.GetFriendUserId(AccountModel, x);
                    PostCommentorModel.SelectOptionModel.ListCustomDetailsUrl.Remove(x);
                    PostCommentorModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(AccountModel.AccountId, $"{FdConstants.FbHomeUrl}{friendId}"));
                    _delayService.ThreadSleep(1000);
                });

                randomFriends = PostCommentorModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Where(x => x.Key == AccountModel.AccountId).Select(x => x.Value).ToList();

                Random random = new Random();

                randomFriends = randomFriends.OrderBy(x => random.Next()).Take(noOfMentions).ToList();

                randomFriends.ForEach(x =>
                {
                    var friendId = FdFunctions.FdFunctions.GetIntegerOnlyString(x);
                    var friendDetails = ObjDbAccountService.GetSingle<AccountFriends>(y => y.FriendId == friendId);
                    mention += $"@[{friendId}:{friendDetails.FullName}] ";

                    mentionsUsed = mention + ",";
                });

                if (!string.IsNullOrEmpty(mentionsUsed))
                    mentionsUsed = mentionsUsed.Remove(mentionsUsed.Length - 1);

                if (!string.IsNullOrEmpty(mentionsUsed))
                {
                    comment = mention + comment;
                    usedMentions = mentionsUsed;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return comment;
        }

        private Dictionary<string, string> CommentWithMentionsBrowser(ref string usedMentions)
        {
            var mention = string.Empty;

            string mentionsUsed = string.Empty;

            Dictionary<string, string> mentionUserList = new Dictionary<string, string>();

            try
            {
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(AccountModel);

                var randomFriends = objFdFunctions.MentionFriends(AccountModel, PostCommentorModel.SelectOptionModel);

                randomFriends.RemoveAll(x => x == "");

                int noOfMentions = PostCommentorModel.NoOfUserMention.GetRandom();

                if (noOfMentions == 0)
                {
                    noOfMentions = PostCommentorModel.NoOfUserMention.GetRandom();
                }

                if (randomFriends.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Getting friend details for mention!");
                }

                randomFriends.ForEach(x =>
                {
                    var friendId = _fdRequestLibrary.GetFriendUserId(AccountModel, x);
                    PostCommentorModel.SelectOptionModel.ListCustomDetailsUrl.Remove(x);
                    PostCommentorModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(AccountModel.AccountId, $"{FdConstants.FbHomeUrl}{friendId}"));
                    Thread.Sleep(1000);
                });

                randomFriends = PostCommentorModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Where(x => x.Key == AccountModel.AccountId).Select(x => x.Value).ToList();

                Random random = new Random();

                randomFriends = randomFriends.OrderBy(x => random.Next()).Take(noOfMentions).ToList();

                randomFriends.ForEach(x =>
                {
                    //var friendId = FdFunctions.FdFunctions.GetIntegerOnlyString(x);
                    var friendId = Utilities.GetBetween(x, FdConstants.FbHomeUrl, "\"");
                    friendId = string.IsNullOrEmpty(friendId) ? x.Split('/').LastOrDefault() : friendId;
                    var friendDetails = ObjDbAccountService.GetSingle<AccountFriends>(y => y.FriendId == friendId);

                    if (!mentionUserList.ContainsKey(friendId))
                    {
                        mentionUserList.Add(friendId, friendDetails.FullName);
                        mention = friendDetails.FullName;
                    }
                    mentionsUsed += mention + ",";
                });


                usedMentions = mentionsUsed.Remove(mentionsUsed.Length - 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return mentionUserList;
        }

        /*
                private void GetComments([NotNull] ref string comments, ScrapeResultNew scrapeResult)
                {
                    if (comments == null) throw new ArgumentNullException(nameof(comments));
                    comments = string.Empty;

                    FacebookPostDetails post = (FacebookPostDetails)scrapeResult.ResultPost;

                    List<ManageCommentModel> listCommentsWithoutFilter = new List<ManageCommentModel>();

                    var commentList = PostCommentorModel.LikerCommentorConfigModel.LstManageCommentModel;

                    foreach (var commentInfo in commentList)
                    {
                        try
                        {
                            if (post.Caption.Contains(commentInfo.FilterText) || post.SubDescription.Contains(commentInfo.FilterText) ||
                                    post.Title.Contains(commentInfo.FilterText))
                            {
                                comments = commentInfo.CommentText;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (string.IsNullOrEmpty(commentInfo.FilterText))
                            listCommentsWithoutFilter.Add(commentInfo);
                    }

                    var random = new Random();

                    int index = random.Next(listCommentsWithoutFilter.Count);

                    comments = listCommentsWithoutFilter[index].CommentText;

                }
        */



        private string GetCommentDetailsNew(FacebookPostDetails objFacebookPostDetails)
        {
            string commentDetails = string.Empty;

            QueryInfo queryInfo = new QueryInfo
            {
                QueryType = objFacebookPostDetails.QueryType,
                QueryValue = objFacebookPostDetails.QueryValue
            };



            List<string> lstComment = new List<string>();

            bool isCommentAdded = false;

            try
            {
                PostCommentorModel.LikerCommentorConfigModel.LstManageCommentModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryType == queryInfo.QueryType && y.Content.QueryValue == queryInfo.QueryValue)
                        {
                            if (lstComment.All(z => z != x.CommentText))
                            {
                                lstComment.Add(x.CommentText);
                            }
                        }
                    });
                });

                foreach (var currentMessage in lstComment)
                {
                    commentDetails = currentMessage;

                    var messageCollection = new List<string>();

                    var messages = new List<string>();

                    var commentText = string.Empty;

                    if (PostCommentorModel.LikerCommentorConfigModel.IsSpintaxChecked)
                    {
                        messages = SpinTexHelper.GetSpinMessageCollection(commentDetails);

                        foreach (var message in messages)
                        {
                            commentText = message.Trim();
                            messageCollection.Add(commentText);
                        }

                        if (AccountCommentPair.Count >= messageCollection.Count)
                            AccountCommentPair.Clear();
                    }
                    else
                    {
                        messageCollection.Add(currentMessage);
                        if (AccountCommentPair.Count >= lstComment.Count)
                            AccountCommentPair.Clear();
                    }

                    messageCollection.Shuffle();
                    messageCollection.Shuffle();

                    foreach (var comment in messageCollection)
                    {
                        try
                        {
                            if (AccountCommentPair.Any(z =>
                                z.Key == comment))
                            {
                            }
                            else
                            {
                                commentDetails = comment;

                                AccountCommentPair.Add($"{comment}", AccountModel.AccountId);

                                isCommentAdded = true;

                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    if (isCommentAdded)
                    {
                        break;
                    }
                }

                return commentDetails;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commentDetails;
        }


        private string GetUniqueCommentDetails(FacebookPostDetails objFacebookPostDetails)
        {
            string commentDetail;

            QueryInfo queryInfo = new QueryInfo
            {
                QueryType = objFacebookPostDetails.QueryType,
                QueryValue = objFacebookPostDetails.QueryValue
            };



            List<string> lstComment = new List<string>();

            try
            {
                PostCommentorModel.LikerCommentorConfigModel.LstManageCommentModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryType == queryInfo.QueryType && y.Content.QueryValue == queryInfo.QueryValue)
                        {
                            if (lstComment.All(z => z != x.CommentText))
                            {
                                string newComment = x.CommentText;
                                lstComment.Add(newComment);
                            }
                        }
                    });
                });

                commentDetail = GetUniqueComment(lstComment, objFacebookPostDetails);

                return commentDetail;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                commentDetail = string.Empty;
            }


            return commentDetail;
        }


        public string GetUniqueComment(List<string> lstComment, FacebookPostDetails objFacebookPostDetails)
        {
            string commentDetail = string.Empty;

            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            foreach (var currentComment in lstComment)
            {
                var textComment = currentComment;

                var commentCollection = new List<string>();

                var comment = string.Empty;

                if (PostCommentorModel.LikerCommentorConfigModel.IsSpintaxChecked)
                {
                    commentCollection = SpinTexHelper.GetSpinMessageCollection(textComment);
                }
                else
                {
                    commentCollection.Add(currentComment);
                }



                foreach (var commentText in commentCollection)
                {
                    try
                    {
                        comment = commentText.Trim();
                        if (modulesetting.IsTemplateMadeByCampaignMode && PostCommentorModel.IschkUniqueCommentChecked)
                        {
                            lock (CommentLock)
                            {
                                if (!UniqueAccountCommentPair.Keys.Contains($"{CampaignId}_{comment}"))
                                {
                                    UniqueAccountCommentPair.Add($"{CampaignId}_{comment}", $"{AccountModel.AccountBaseModel.UserName}");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        if ((modulesetting.IsTemplateMadeByCampaignMode && PostCommentorModel.IschkUniqueCommentForEachPostChecked)
                             || PostCommentorModel.IschkAllowMultipleComment)
                        {
                            lock (CommentLock)
                            {
                                var fullCommentForUser = $"{CampaignId}_{comment}_{objFacebookPostDetails.Id}";


                                if (!CommentPostPair.Keys.Contains($"{fullCommentForUser}"))
                                {
                                    CommentPostPair.Add($"{fullCommentForUser}", $"{fullCommentForUser}");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        AccountCommentPair.Add($"{comment}", AccountModel.AccountId);

                        commentDetail = comment;

                        break;


                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (!string.IsNullOrEmpty(commentDetail))
                    break;
            }

            return commentDetail;
        }


        void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult, string comment, string mentions)
        {
            try
            {
                var likeType = string.Empty;

                bool isProcessCompleted = true;

                FacebookPostDetails postDetails = (FacebookPostDetails)scrapeResult.ResultPost;

                if (PostCommentorModel.IschkAllowMultipleComment)
                    isProcessCompleted = CheckIfProcessCompletedForPost(postDetails);

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedPosts
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = postDetails.QueryType,
                        QueryValue = postDetails.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostId = postDetails.Id,
                        PostUrl = postDetails.PostUrl,
                        LikeType = likeType,
                        Comment = comment,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        PostDescription = JsonConvert.SerializeObject(postDetails),
                        Mentions = mentions,
                        IsMoreCommentsNeeded = !isProcessCompleted
                    });
                }

                DbAccountService.Add(new AccountInteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = postDetails.QueryType,
                    QueryValue = postDetails.QueryValue,
                    PostId = postDetails.Id,
                    PostUrl = postDetails.PostUrl,
                    LikeType = likeType,
                    Comment = comment,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    PostDescription = JsonConvert.SerializeObject(postDetails),
                    Mentions = mentions,
                    IsMoreCommentsNeeded = !isProcessCompleted

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }

        private bool CheckIfProcessCompletedForPost(FacebookPostDetails postDetails)
        {
            var count = DbAccountService.GetInteractedPosts(ActivityType, postId: postDetails.Id).Count();

            if (count < postDetails.MaxCommentsOnEachPost - 1)
                return false;

            return true;
        }
    }
}
