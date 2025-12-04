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
using System.Linq;
using System.Text.RegularExpressions;
using ThreadUtils;
using Unity;
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class PostLikerProcess : FdJobProcessInteracted<AccountInteractedPosts>
    {
        public PostLikerModel PostLikerModel { get; set; }
        private readonly IAccountScopeFactory _accountScopeFactory;
        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary _fdRequestLibrary;
        private readonly IDelayService _delayService;

        IFdBrowserManager BrowserManager = null;

        public PostLikerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _fdRequestLibrary = fdRequestLibrary;
            _delayService = delayService;
            PostLikerModel = processScopeModel.GetActivitySettingsAs<PostLikerModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }
        FanpageDetails FanpageDetails = null;

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {

            JobProcessResult jobProcessResult = new JobProcessResult();
            var comment = string.Empty;
            bool processSuccess = false;

            FacebookPostDetails objFacebookPostDetails = (FacebookPostDetails)scrapeResult.ResultPost;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                ReactionType objReactionType = ReactionType.Like;

                bool isSuccess;
                GetReactionDetails(ref objReactionType);
                if (!string.IsNullOrEmpty(objFacebookPostDetails.LikePostAsPageId) &&
                    AccountModel.IsRunProcessThroughBrowser && !FdLogInProcess._browserManager.isActorChangedtoFanPage)
                {
                    var pageSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{objFacebookPostDetails.LikePostAsPageId}"]
                        .Resolve<IFdBrowserManager>();

                    FanpageDetails = pageSpecificWindow.GetFullPageDetails(AccountModel, new FanpageDetails() { FanPageUrl = $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.LikePostAsPageId}" }).ObjFdScraperResponseParameters.FanpageDetails;
                    pageSpecificWindow.CloseBrowser(AccountModel);
                }

                if (AccountModel.IsRunProcessThroughBrowser)
                    isSuccess = FdLogInProcess._browserManager.LikePost(AccountModel, objFacebookPostDetails, objReactionType, FanpageDetails);
                else
                    isSuccess = _fdRequestLibrary.LikeUnlikePost(AccountModel,
                       string.IsNullOrEmpty(objFacebookPostDetails.Id) ? objFacebookPostDetails.PostUrl : objFacebookPostDetails.Id, objReactionType);

                if (isSuccess)
                {
                    processSuccess = true;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.Id);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.Id, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }


                if (processSuccess)
                {
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                    AddProfileScraperDataToDatabase(scrapeResult, objReactionType, comment
                        , PostLikerModel.LikerCommentorConfigModel.IsLikeTypeFilterChkd);
                    StartAfterAction(scrapeResult);
                }

                if (PostLikerModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(PostLikerModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else DelayBeforeNextActivity();
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

        private void StartAfterAction(ScrapeResultNew scrapeResult)
        {

            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FacebookPostDetails facebookPostDetails = (FacebookPostDetails)scrapeResult.ResultPost;

                if (PostLikerModel.CommentOnPostChecked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Started After Action For PostLike {FdConstants.FbHomeUrl}{facebookPostDetails.Id}");

                    int delayBetweenComments = PostLikerModel.DelayBetweenCommentsForAfterActivity.GetRandom();

                    var comment = GetComments();
                    var commentList = new System.Collections.Generic.List<string>();
                    commentList.Add(comment);

                    FanpageDetails fanpageDetails = null;
                    if (!string.IsNullOrEmpty(facebookPostDetails.LikePostAsPageId))
                        fanpageDetails = FdConstants.getFanPageFromUrlOrIdOrUserName(PostLikerModel.ListOwnPageUrl.FirstOrDefault());

                    var isSuccess = AccountModel.IsRunProcessThroughBrowser
                      ? FdLogInProcess._browserManager.CommentOnSinglePost(AccountModel, facebookPostDetails, commentList, facebookPostDetails.LikePostAsPageId, new System.Collections.Generic.Dictionary<string, string>(), fanpageDetails)
                      : _fdRequestLibrary.CommentOnPost(AccountModel, facebookPostDetails.Id, comment).ObjFdScraperResponseParameters.IsCommentedOnPost;

                    if (isSuccess)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Successfully Commented on posts with url {FdConstants.FbHomeUrl}{facebookPostDetails.Id}");
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Added {delayBetweenComments} sec delay between Comment on post");

                        _delayService.ThreadSleep(delayBetweenComments * 1000);
                    }
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Failed to comment posts with url {FdConstants.FbHomeUrl}{facebookPostDetails.Id}");

                }

                if (PostLikerModel.IsSendFriendRequestChked && facebookPostDetails.EntityType.ToString() == "Friend")
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
                            $"Successfully Sent FriendRequest {FdConstants.FbHomeUrl}{facebookPostDetails.OwnerId}");
                    else
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "Send Request",
                            $"{isSentFriendRequest} {facebookPostDetails.OwnerId}");

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

        private string GetComments()
        {
            var commentList = Regex.Split(PostLikerModel.UploadComment, "\r\n");
            commentList.Shuffle();
            if (PostLikerModel.IsSpintaxChecked)
            {
                var spinMessageList = SpinTexHelper.GetSpinMessageCollection(commentList.FirstOrDefault());
                spinMessageList.Shuffle();
                return spinMessageList.FirstOrDefault();
            }
            else
                return PostLikerModel.UploadComment;
        }

        private void GetReactionDetails(ref ReactionType objReactionType)
        {

            try
            {
                var listReactionType = PostLikerModel.LikerCommentorConfigModel.ListReactionType;

                if (listReactionType.Count > 0)
                {
                    var random = new Random();

                    int index = random.Next(PostLikerModel.LikerCommentorConfigModel.ListReactionType.Count);

                    objReactionType = PostLikerModel.LikerCommentorConfigModel.ListReactionType[index];
                }

                else
                {
                    objReactionType = PostLikerModel.LikerCommentorConfigModel.ListReactionType[0];
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult, ReactionType objReactionType, string comment, bool isLiked)
        {
            try
            {

                var likeType = objReactionType.ToString();

                if (!isLiked)
                {
                    likeType = " ";
                }

                FacebookPostDetails group = (FacebookPostDetails)scrapeResult.ResultPost;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedPosts
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = group.QueryType,
                        QueryValue = group.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostId = group.Id,
                        LikeType = likeType,
                        Comment = comment,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        PostDescription = JsonConvert.SerializeObject(group),
                        OwnerId = group.OwnerId?.ToString(),
                        Status = "Success"

                    });
                }

                DbAccountService.Add(new AccountInteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = group.QueryType,
                    QueryValue = group.QueryValue,
                    PostId = group.Id,
                    LikeType = likeType,
                    Comment = comment,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    PostDescription = JsonConvert.SerializeObject(group),
                    OwnerId = group.OwnerId?.ToString(),
                    Status = "Success"

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
