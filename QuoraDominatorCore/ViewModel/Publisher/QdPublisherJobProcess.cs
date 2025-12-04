using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace QuoraDominatorCore.ViewModel.Publisher
{
    public class QdPublisherJobProcess : PublisherJobProcess
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;
        private IQuoraBrowserManager _browser;
        private readonly IQdLogInProcess _logInProcess;
        private readonly IQuoraFunctions _quoraFunction;
        private readonly DominatorAccountModel dominatorAccountModel;
        private IQDBrowserManagerFactory managerFactory;
        private JsonJArrayHandler handler => JsonJArrayHandler.GetInstance;
        public QdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            dominatorAccountModel = _accountsFileManager.GetAccountById(accountId);
            dominatorAccountModel.AccountId = accountId;
            _quoraFunction = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IQuoraFunctions>();
            _logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IQdLogInProcess>();
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            managerFactory = _logInProcess.managerFactory;
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
        }

        public QdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            dominatorAccountModel = _accountsFileManager.GetAccountById(accountId);

            dominatorAccountModel.AccountId = accountId;
            _quoraFunction = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IQuoraFunctions>();
            _logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IQdLogInProcess>();
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
        }
        public override CustomPostDetail GetCustomPostDetails(DominatorAccountModel dominatorAccount, string PostUrl)
        {
            var PostDetails = new CustomPostDetail();
            JObject jsonObject;
            long PostCreationTime = 0;
            int PostUpvoteCount = 0;
            try
            {
                var PostResponse = _quoraFunction.CustomPostResponse(PostUrl).Response.Replace("\\\"", "\"").Replace("\\\"", "\"");
                if (PostResponse.Contains("{\"data\":{\"post\":"))
                {
                    jsonObject = handler.ParseJsonToJObject(QdConstants.GetJsonForAllTypePosts(PostResponse, "post"));
                    long.TryParse(handler.GetJTokenValue(jsonObject, "data", "post", "creationTime"), out PostCreationTime);
                    int.TryParse(handler.GetJTokenValue(jsonObject, "data", "post", "numUpvotes"), out PostUpvoteCount);
                }
                else if (PostResponse.Contains("{\"data\":{\"tribeItem\":"))
                {
                    jsonObject = handler.ParseJsonToJObject(QdConstants.GetJsonForAllTypePosts(PostResponse, "tribeItem"));
                    long.TryParse(handler.GetJTokenValue(jsonObject, "data", "tribeItem", "post", "creationTime"), out PostCreationTime);
                    int.TryParse(handler.GetJTokenValue(jsonObject, "data", "tribeItem", "post", "numUpvotes"), out PostUpvoteCount);
                }
                else if (PostResponse.Contains("{\"data\":{\"answer\":"))
                {
                    jsonObject = handler.ParseJsonToJObject(QdConstants.GetJsonForAllTypePosts(PostResponse, "answer"));
                    long.TryParse(handler.GetJTokenValue(jsonObject, "data", "answer", "creationTime"), out PostCreationTime);
                    int.TryParse(handler.GetJTokenValue(jsonObject, "data", "answer", "numUpvotes"), out PostUpvoteCount);
                }
                PostDetails.PostedDate = (PostCreationTime / 1000).EpochToDateTimeUtc();
                PostDetails.LikesCount = PostUpvoteCount;
            }
            catch (Exception)
            {
                return PostDetails;
            }
            return PostDetails;
        }
        private bool IsFilterApplied(DominatorAccountModel accountModel, PublisherPostlistModel updatedPostDetails)
        {
            if (accountModel != null && updatedPostDetails != null)
                if (!string.IsNullOrEmpty(updatedPostDetails.ShareUrl) && (updatedPostDetails.sharePostModel.IsMinimumDays || updatedPostDetails.sharePostModel.IsPostBetween))
                {
                    var CustomPostDetails = GetCustomPostDetails(accountModel, updatedPostDetails.ShareUrl);
                    var Days = (CustomPostDetails.PostedDate != null) ? QdUtilities.GetDateDifferenceFromTimeStamp(CustomPostDetails.PostedDate) : 0;
                    if (updatedPostDetails.sharePostModel.IsMinimumDays && updatedPostDetails.sharePostModel.MinimumDays < Days)
                        return true;
                    if (updatedPostDetails.sharePostModel.IsPostBetween && !updatedPostDetails.sharePostModel.PostBetween.InRange(CustomPostDetails.LikesCount))
                        return true;
                }
            return false;
        }
        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails, bool isDelayNeed)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var account = accountsFileManager.GetAccountById(accountId);
            var isQuestionPosted = false;
            _browser = managerFactory.QdBrowserManager();
            var updatedPost = PerformGeneralSettings(postDetails);
            try
            {
                if (IsFilterApplied(account, updatedPost))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, updatedPost.PostSource.ToString(), "Filter Not Matched!");
                    return false;
                }
                var accountScope = _accountScopeFactory[dominatorAccountModel.AccountId];
                var httpHelper = accountScope.Resolve<IQdHttpHelper>();
                var FinalUrl = string.Empty;
                var AlreadyAskedQuestion = false;
                ReplaceFileNameAsDescription(postDetails, updatedPost);
                var IsSharePost = updatedPost.PostSource == PostSource.ScrapedPost;
                var Question = IsSharePost ? updatedPost.ShareUrl : string.IsNullOrEmpty(updatedPost.PostDescription) ? updatedPost.PublisherInstagramTitle : updatedPost.PostDescription;
                var IsPost = CheckIsPost(updatedPost);
                if (!account.IsRunProcessThroughBrowser)
                {
                    _logInProcess.LoginWithDataBaseCookies(dominatorAccountModel,true, dominatorAccountModel.Token);
                    PostQuestionResponseHandler PostResponse = null;
                    var pagesource = httpHelper.GetRequest(QdConstants.HomePageUrl).Response;
                    if (IsPost)
                        PostResponse = string.IsNullOrEmpty(updatedPost.ShareUrl) ? _quoraFunction.CreatePost(account, pagesource, updatedPost.MediaList.FirstOrDefault(), Question).Result 
                            :_quoraFunction.SharePost(account,pagesource,updatedPost.ShareUrl).Result;
                    else
                        PostResponse = _quoraFunction.PostQuestion(account, pagesource, Question, updatedPost.PdSourceUrl).Result;
                    isQuestionPosted = PostResponse.QuestionUrl.Contains("unanswered")|| PostResponse.QuestionUrl.Contains("profile/");
                    if (isQuestionPosted)
                        FinalUrl = PostResponse.QuestionUrl;
                    AlreadyAskedQuestion = PostResponse.AlreadyAsked;
                }
                else
                {
                    IResponseParameter response = null;
                    var loggedIn = Task.Run(async () =>
                    {
                        return await _browser.BrowserLoginAsync(account, account.Token);
                    });
                    Question = string.IsNullOrEmpty(Question) ?updatedPost.ShareUrl: Question;
                    if (IsPost)
                        response = _browser.CreatePost(account, Question, updatedPost.MediaList.FirstOrDefault(), out FinalUrl);
                    else
                        response = _browser.PostQuestion(account,Question.TrimEnd('?'), updatedPost.PdSourceUrl, out FinalUrl);
                    if (response.Response.Contains("Was your question already asked"))
                        AlreadyAskedQuestion = true;
                    else if (response.Response.Contains("You recently asked") || FinalUrl.Contains("unanswered")|| FinalUrl.Contains("profile/") || response.Response.Contains("Assistant"))
                        isQuestionPosted = true;
                }
                if (isQuestionPosted && !AlreadyAskedQuestion)
                {
                    var questionUrl = string.IsNullOrEmpty(FinalUrl)? $"{QdConstants.HomePageUrl}/unanswered/" + Question.Replace(" ", "-").Replace("?", "").Replace("(","").Replace(")","").Replace(",",""):FinalUrl;
                    GlobusLogHelper.log.Info(Log.PublishingSuccessfully,dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,"Own Wall",$"\" {Question} \" Successfully posted At ==> {questionUrl}.");
                    UpdatePostWithSuccessful(account.UserName, postDetails, questionUrl);
                }else if(AlreadyAskedQuestion)
                    GlobusLogHelper.log.Info(Log.CustomMessage,dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,"Publish",$"\" {Question} \" Already Has Been "+(IsSharePost ?"Shared!": "Asked!"));
                else
                {
                    GlobusLogHelper.log.Info(Log.PublishingFailed,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,"Own Wall", "Publish Failed With Error.");
                    UpdatePostWithFailed(account.UserName, postDetails, string.Empty);
                }
                if(isDelayNeed)
                    DelayBeforeNextPublish();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally {
                if (_browser!=null && _browser.BrowserWindow!=null)
                    _browser.CloseBrowser();
            }
            return isQuestionPosted;
        }

        private bool CheckIsPost(PublisherPostlistModel updatedPost)
        {
            if (updatedPost.MediaList.Count > 0 || updatedPost.PostSource == PostSource.SharePost || updatedPost.PostSource==PostSource.ScrapedPost || updatedPost.PostDescription.Length>250 ||updatedPost.PostDescription.Contains("\n"))
                return true;
            return false;
        }

        private void ReplaceFileNameAsDescription(PublisherPostlistModel postDetails, PublisherPostlistModel updatedPost)
        {
            if (postDetails.MediaList.Count > 0 && ((postDetails.scrapePostModel.IsScrapeGoogleImgaes && postDetails.scrapePostModel.IsUseFileNameAsDescription) || (postDetails.PostDetailModel.IsUploadMultipleImage && postDetails.PostDetailModel.IsUseFileNameAsDescription)))
                updatedPost.PostDescription = new FileInfo(postDetails.MediaList.FirstOrDefault()).Name;
            else
                updatedPost.PostDescription = postDetails.PostDescription;
        }
    }
}