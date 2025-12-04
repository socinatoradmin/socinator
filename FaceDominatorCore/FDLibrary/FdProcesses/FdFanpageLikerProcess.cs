using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThreadUtils;
using AccountInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages;
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts;
using CampaignInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPages;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;
using OwnPages = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.OwnPages;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdFanpageLikerProcess : FdJobProcessInteracted<AccountInteractedPages>
    {
        public FanpageLikerModel FanpageLikerModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public Dictionary<string, string> DictPageUrl = new Dictionary<string, string>();

        public readonly IFdRequestLibrary FdRequestLibrary;
        private readonly IDelayService _delayService;
        FanpageDetails ownPageDetails { get; set; }
        public FdFanpageLikerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)

        {

            FdRequestLibrary = fdRequestLibrary;
            FanpageLikerModel = processScopeModel.GetActivitySettingsAs<FanpageLikerModel>();
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;

            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (FanpageLikerModel.IsActionasPageChecked)
                {
                    ownPageDetails = FdConstants.getFanPageFromUrlOrIdOrUserName(FanpageLikerModel.ListOwnPageUrl.FirstOrDefault());
                }
                var response = AccountModel.IsRunProcessThroughBrowser ?
                     FdLogInProcess._browserManager.LikeFanpage(AccountModel, objFanpageDetails, ownPageDetails)
                   : FdRequestLibrary.LikeFanpage(AccountModel, objFanpageDetails.FanPageID, JobCancellationTokenSource.Token);

                if (response.Status)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID);
                    IncrementCounters();
                    AddProfileScraperDataToDatabase(scrapeResult);
                    StartAfterAction(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID, response.Error);
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

        private void StartAfterAction(ScrapeResultNew scrapeResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;

                if (FanpageLikerModel.IsChkLikeFanPageLatestPost || FanpageLikerModel.ChkCommentOnFanPageLatestPostsChecked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Started After Action For Fanpage {FdConstants.FbHomeUrl}{objFanpageDetails.FanPageID}");
                    LikerCommentOnPost(scrapeResult);
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

        private void LikerCommentOnPost(ScrapeResultNew scrapeResult)
        {
            FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;

            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                int delayBetweenComments = FanpageLikerModel.DelayBetweenCommentsForAfterActivity.GetRandom();

                int delayBetweenLikes = FanpageLikerModel.DelayBetweenLikesForAfterActivity.GetRandom();

                int noOfLikes = FanpageLikerModel.LikeBetweenJobs.GetRandom();

                int noOfComments = FanpageLikerModel.CommentsPerFanPage.GetRandom();

                int totalLikes = 0;

                int totalCommments = 0;

                string pageId = string.Empty;
                FanpageDetails ownPageDetails = null;
                IResponseHandler scrapPostListFromFanpageResponseHandler = null;
                if (FanpageLikerModel.IsActionasPageChecked)
                {
                    List<string> urlList = GetAllRelatedPages();
                    ownPageDetails = FdConstants.getFanPageFromUrlOrIdOrUserName(urlList.FirstOrDefault());
                }
                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    FdLogInProcess._browserManager.SearchPostsByPageUrl(AccountModel, FbEntityType.Fanpage, objFanpageDetails.FanPageUrl);
                }
                while (scrapPostListFromFanpageResponseHandler == null ||
                   scrapPostListFromFanpageResponseHandler.HasMoreResults)
                {

                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        scrapPostListFromFanpageResponseHandler = FdLogInProcess._browserManager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.NewFeedPost, 2, 0);
                    }

                    else
                        scrapPostListFromFanpageResponseHandler =
                            FdRequestLibrary.GetPostListFromFanpagesNew(AccountModel,
                                scrapPostListFromFanpageResponseHandler, objFanpageDetails.FanPageID);

                    if (scrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count == 0)
                    {

                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Posts not available for After action");
                        break;
                    }

                    foreach (var post in scrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters
                        .ListPostDetails)
                    {
                        //FdRequestLibrary.GetPostDetailNew(AccountModel, post);

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (FanpageLikerModel.IsActionasPageChecked && string.IsNullOrEmpty(post.LikePostAsPageId))
                            post.LikePostAsPageId = ownPageDetails.FanPageID;

                        if (post.OwnerName == objFanpageDetails.FanPageName || objFanpageDetails.FanPageUrl.Contains(post.OwnerId))
                        {
                            if (FanpageLikerModel.IsChkLikeFanPageLatestPost && totalLikes < noOfLikes)
                            {

                                var isSuccess = AccountModel.IsRunProcessThroughBrowser
                                             ? FdLogInProcess._browserManager.LikePost(Account, post, ReactionType.Like, ownPageDetails)
                                           : FdRequestLibrary.LikeUnlikePost(AccountModel, post.Id, ReactionType.Like);

                                if (isSuccess)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Successfully Liked posts with url {FdConstants.FbHomeUrl}{post.Id}");
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Added {delayBetweenComments} sec delay between Like post");
                                    AddProfileScraperDataToDatabase(post, string.Empty,
                                        ActivityType.Like);
                                    _delayService.ThreadSleep(delayBetweenLikes * 1000);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Failed to Like posts with url {FdConstants.FbHomeUrl}{post.Id}");
                                }

                                totalLikes++;
                            }

                            if (FanpageLikerModel.ChkCommentOnFanPageLatestPostsChecked &&
                                totalCommments < noOfComments)
                            {
                                var comment = GetComments();
                                List<string> CommentList = new List<string>();
                                CommentList.Add(comment);

                                var isSuccess = AccountModel.IsRunProcessThroughBrowser ? FdLogInProcess._browserManager.CommentOnSinglePost(AccountModel, post, CommentList, postAsPageId: post.LikePostAsPageId, fanpageDetails: ownPageDetails)
                                       : FdRequestLibrary.CommentOnPost(AccountModel, post.Id, comment).ObjFdScraperResponseParameters.IsCommentedOnPost;

                                if (isSuccess)
                                {
                                    AddProfileScraperDataToDatabase(post, comment, ActivityType.Comment);
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Successfully Commented on posts with url {FdConstants.FbHomeUrl}{post.Id}");
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
                                        $"Failed to comment posts with url {FdConstants.FbHomeUrl}{post.Id}");

                                totalCommments++;

                            }
                        }

                        if (totalCommments >= noOfComments && totalLikes >= noOfLikes)
                            break;
                    }

                    if ((noOfLikes <= totalLikes && noOfComments <= totalCommments)
                        || (noOfLikes == 0 && noOfComments <= totalCommments)
                        || (noOfLikes <= totalLikes && noOfComments == 0))
                        break;


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


        public List<string> GetAllRelatedPages()
        {
            List<string> urlList = new List<string>();
            foreach (string url in FanpageLikerModel.ListOwnPageUrl)
            {
                try
                {
                    var pageid = DictPageUrl.ContainsKey(url) ? DictPageUrl[url] : string.Empty;
                    if (!DictPageUrl.ContainsKey(url))
                    {
                        pageid = FdRequestLibrary.GetPageIdFromUrl(AccountModel, url);
                        DictPageUrl.Add(url, pageid);
                        if (ObjDbAccountService.GetSingle<OwnPages>(x =>
                                x.PageId == pageid) != null)
                        {
                            urlList.Add(pageid);
                        }
                        else if (ObjDbAccountService.GetSingle<OwnPages>(x =>
                                 x.PageUrl == url) != null)
                        {
                            urlList.Add(pageid);
                        }
                    }
                    else if (ObjDbAccountService.GetSingle<OwnPages>(x => x.PageId == pageid) != null)
                    {
                        urlList.Add(pageid);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return urlList;
        }


        private string GetComments()
        {
            var commentList = Regex.Split(FanpageLikerModel.UploadComment, "\r\n");
            commentList.Shuffle();
            return commentList.FirstOrDefault();
        }


        private void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FanpageDetails page = (FanpageDetails)scrapeResult.ResultPage;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedPages
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PageId = page.FanPageID,
                        PageUrl = $"{FdConstants.FbHomeUrl}{page.FanPageID}",
                        PageName = page.FanPageName,
                        PageType = page.FanPageCategory,
                        TotalLikers = page.FanPageLikerCount,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now


                    });
                }

                DbAccountService.Add(new AccountInteractedPages
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    PageId = page.FanPageID,
                    PageUrl = $"{FdConstants.FbHomeUrl}{page.FanPageID}",
                    PageName = page.FanPageName,
                    PageType = page.FanPageCategory,
                    TotalLikers = page.FanPageLikerCount,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }

        void AddProfileScraperDataToDatabase(FacebookPostDetails postDetails, string comment,
    ActivityType activityType, string mentions = "")
        {
            try
            {
                var likeType = ReactionType.Like.ToString();


                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    ObjDbCampaignService.Add(new CampaignInteractedPosts
                    {
                        ActivityType = activityType.ToString(),
                        QueryType = postDetails.QueryType,
                        QueryValue = postDetails.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostId = postDetails.Id,
                        LikeType = likeType,
                        Comment = comment,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        PostDescription = JsonConvert.SerializeObject(postDetails),
                        Mentions = mentions
                    });
                }

                ObjDbAccountService.Add(new AccountInteractedPosts
                {
                    ActivityType = activityType.ToString(),
                    QueryType = postDetails.QueryType,
                    QueryValue = postDetails.QueryValue,
                    PostId = postDetails.Id,
                    LikeType = likeType,
                    Comment = comment,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    PostDescription = JsonConvert.SerializeObject(postDetails),
                    Mentions = mentions

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }
    }
}
