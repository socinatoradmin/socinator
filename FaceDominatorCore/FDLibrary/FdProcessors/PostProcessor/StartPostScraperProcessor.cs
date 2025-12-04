using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.PostProcessor
{

    public class StartPostScraperProcessor : BaseFbPostProcessor
    {
        // RedditPostResponseHandler _redditPostResponseHandler;
        public StartPostScraperProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)

        {
        }


        // ReSharper disable once RedundantAssignment
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = new JobProcessResult();
            try
            {

                #region Properties

                var postLikeCommentorModel = JobProcess.ModuleSetting.PostLikeCommentorModel;

                List<PostOptions> listCompletedOption = new List<PostOptions>();

                Dictionary<PostOptions, List<string>> dictInputUrl = new Dictionary<PostOptions, List<string>>()
                {
                    { PostOptions.OwnWall, new List<string>() {PostOptions.OwnWall.ToString() } },
                    { PostOptions.NewsFeed, new List<string>() {PostOptions.NewsFeed.ToString() } },
                    { PostOptions.FriendWall, postLikeCommentorModel.ListFriendProfileUrl },
                    { PostOptions.Pages, postLikeCommentorModel.ListPageUrl },
                    { PostOptions.Group, postLikeCommentorModel.ListGroupUrl },
                    { PostOptions.CustomPostList, postLikeCommentorModel.ListCustomPostList },
                    { PostOptions.Campaign, postLikeCommentorModel.ListFaceDominatorCampaign },
                    { PostOptions.Albums, postLikeCommentorModel.ListAlbums },
                    { PostOptions.Keyword, postLikeCommentorModel.ListKeywords },
                    { PostOptions.ProfileScraper, postLikeCommentorModel.ListCampaign },
                    { PostOptions.Hashtag , postLikeCommentorModel.ListHashtags},
                    { PostOptions.PostSharer , postLikeCommentorModel.ListPostSharer},
                    { PostOptions.PostScraperCampaign,postLikeCommentorModel.ListPostScraperCampaign }
                };

                List<PostOptions> lstPostOptions = dictInputUrl.Keys.ToList();

                #endregion

                while (listCompletedOption.Count < 7)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    try
                    {
                        lstPostOptions.Shuffle();

                        foreach (var option in lstPostOptions)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {
                                jobProcessResult = new JobProcessResult();

                                if (listCompletedOption.Contains(option))
                                    continue;

                                if (option == PostOptions.CustomPostList &&
                                        postLikeCommentorModel.LstPostOptions[option])
                                {
                                    FilterAndStartFinalProcessForCustom(ref jobProcessResult, PostOptions.CustomPostList.ToString());
                                }
                                else if (option == PostOptions.ProfileScraper &&
                                         postLikeCommentorModel.LstPostOptions[option])
                                {
                                    FilterAndStartFinalProcessForProfileScraperCampaign(ref jobProcessResult, PostOptions.Campaign.ToString(),
                                        dictInputUrl[option]);
                                }
                                else if (option == PostOptions.Campaign &&
                                        postLikeCommentorModel.LstPostOptions[option])
                                {
                                    FilterAndStartFinalProcessForCampaign(ref jobProcessResult, PostOptions.Campaign.ToString());
                                }
                                else if (option == PostOptions.PostScraperCampaign &&
                                          postLikeCommentorModel.LstPostOptions[option])
                                {
                                    FilterAndStartFinalProcessForPostScraperCampaign(ref jobProcessResult, PostOptions.PostScraperCampaign.ToString(),
                                        dictInputUrl[option]);
                                }
                                else if (postLikeCommentorModel.LstPostOptions[option])
                                {
                                    FilterAndStartFinalProcessForPost(ref jobProcessResult, option,
                                            dictInputUrl[option], string.Empty);
                                }
                                else
                                {
                                    listCompletedOption.Add(option);
                                }

                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                if (jobProcessResult.HasNoResult && !listCompletedOption.Contains(option))
                                    listCompletedOption.Add(option);

                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException("Requested Cancelled !");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

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


                jobProcessResult.HasNoResult = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            jobProcessResult.HasNoResult = true;
        }

        public void FilterAndStartFinalProcessForProfileScraperCampaign
            (ref JobProcessResult jobProcessResult, string queryType, List<string> selectedOptions)
        {
            try
            {
                foreach (string campaignId in selectedOptions)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var ObjDbCampaignService = new DbCampaignService(campaignId);

                    List<string> lstProfileUrl = ObjDbCampaignService.GetInteractedUsers(ActivityType.ProfileScraper)
                        .Select(x => x.UserId).ToList();

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, lstProfileUrl.Count, "Campaign",
                        campaignId, _ActivityType);

                    FilterAndStartFinalProcessForPost(ref jobProcessResult, PostOptions.ProfileScraper,
                        selectedOptions, campaignId, lstProfileUrl);

                    jobProcessResult.HasNoResult = true;
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

        public void FilterAndStartFinalProcessForCampaign(ref JobProcessResult jobProcessResult, string queryType)
        {
            try
            {

                foreach (string campaignId in JobProcess.ModuleSetting.PostLikeCommentorModel
                    .ListFaceDominatorCampaign)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    List<PublishedPostDetailsModel> lstShuffleInputData =
                        DominatorHouseCore.Diagnostics.PublisherInitialize.GetNetworksPublishedPost
                                (campaignId, SocialNetworks.Facebook);

                    List<FacebookPostDetails> listPost = new List<FacebookPostDetails>();

                    lstShuffleInputData.ForEach(x =>
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        FacebookPostDetails objModel = new FacebookPostDetails { PostUrl = x.Link };
                        if (listPost.FirstOrDefault(y => y.PostUrl == x.Link) == null)
                        {
                            listPost.Add(objModel);
                        }
                    });

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, listPost.Count, "Campaign",
                            campaignId, _ActivityType);

                    ProcessDataOfPosts(ref jobProcessResult,
                          listPost, queryType, campaignId);

                    jobProcessResult.HasNoResult = true;
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FilterAndStartFinalProcessForCustom(ref JobProcessResult jobProcessResult,
            string queryType)
        {
            try
            {

                List<string> lstShuffleInputData = FdFunctions.FdFunctions.RandomShuffle
                    (JobProcess.ModuleSetting.PostLikeCommentorModel.ListCustomPostList).ToList();

                List<FacebookPostDetails> listPost = new List<FacebookPostDetails>();

                lstShuffleInputData.ForEach(x =>
                {
                    FacebookPostDetails objModel = FdConstants.getFaceBookPOstFromUrlOrId(x);
                    if (listPost.FirstOrDefault(y => y.PostUrl == x) == null)
                    {
                        listPost.Add(objModel);
                    }
                });

                ProcessDataOfPosts(ref jobProcessResult,
                          listPost, queryType, string.Empty,ShowLog:true);
                jobProcessResult.HasNoResult = true;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FilterAndStartFinalProcessForPostScraperCampaign(ref JobProcessResult jobProcessResult, string queryType, List<string> selectedOptions)
        {
            try
            {
                foreach (string campaignId in selectedOptions)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var ObjDbCampaignService = new DbCampaignService(campaignId);

                    var lstProfileUrl = ObjDbCampaignService.GetInteractedPosts(ActivityType.PostScraper).ToList();

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, lstProfileUrl.Count, "Campaign",
                        campaignId, _ActivityType);
                    FilterAndStartFinalProcessForPost_PostScraperCampaign(ref jobProcessResult, PostOptions.PostScraperCampaign,
                        selectedOptions, campaignId, lstProfileUrl);

                    jobProcessResult.HasNoResult = true;
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

        public void FilterAndStartFinalProcessForPost(ref JobProcessResult jobProcessResult, PostOptions queryType,
            List<string> lstItems, string campaignId, List<string> lstFriendUrl = null)
        {
            IResponseHandler responseHandler = null;

            List<string> lstShuffleInputData = lstFriendUrl != null
                ? FdFunctions.FdFunctions.RandomShuffle(lstFriendUrl).ToList()
                : FdFunctions.FdFunctions.RandomShuffle(lstItems).ToList();

            foreach (string entityUrl in lstShuffleInputData)
            {
                SuccessCount = 0;
                responseHandler = null;

                jobProcessResult.HasNoResult = false;

                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    switch (queryType)
                    {
                        case PostOptions.Pages:
                            Browsermanager.SearchPostsByPageUrl(AccountModel, FbEntityType.Fanpage, entityUrl);
                            break;

                        case PostOptions.Group:
                            Browsermanager.SearchPostsByGroupUrl(AccountModel, FbEntityType.Groups, entityUrl);
                            break;

                        case PostOptions.FriendWall:
                            Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.Post, entityUrl);
                            break;

                        case PostOptions.OwnWall:
                            Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.Post, entityUrl);
                            break;

                        case PostOptions.Keyword:
                            Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.Posts, entityUrl);
                            break;

                        case PostOptions.Albums:
                            Browsermanager.SearchByAlbum(AccountModel, SearchKeywordType.Posts, entityUrl);
                            break;

                        case PostOptions.Hashtag:
                            Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.Posts, entityUrl);
                            break;

                        case PostOptions.ProfileScraper:
                            Browsermanager.LoadPageSource(AccountModel, entityUrl.StartsWith(FdConstants.FbHomeUrl) ? entityUrl : $"{FdConstants.FbHomeUrl}{entityUrl}", clearandNeedResource: true);
                            break;
                        case PostOptions.PostSharer:
                            {
                                Browsermanager.LoadPageSource(AccountModel, entityUrl, clearandNeedResource: true);
                                Browsermanager.SearchByPostSharer();
                            }
                            break;
                        case PostOptions.NewsFeed:
                            Browsermanager.SearchByfeed(AccountModel, SearchKeywordType.Posts, entityUrl);
                            break;

                    }
                }

                var scrapeValueFrom = queryType == PostOptions.NewsFeed
                 ? FbEntityType.NewFeedPost
                 : queryType == PostOptions.Albums
                 ? FbEntityType.Album
                 : queryType == PostOptions.PostSharer
                 ? FbEntityType.PostSharers
                 : FbEntityType.Friends;
                var searchCount = 0;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (!AccountModel.IsRunProcessThroughBrowser)
                            responseHandler = ScraperFunctionActionTables[$"{queryType.ToString()}"]
                             (AccountModel, responseHandler, entityUrl);
                        else
                            responseHandler = Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, scrapeValueFrom, 7, 0);
                        searchCount++;
                        if (responseHandler != null || responseHandler.Status)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            List<FacebookPostDetails> lstPostIds = responseHandler
                                .ObjFdScraperResponseParameters.ListPostDetails;

                            //          lstPostIds = lstPostIds.Where(x => !x.PostUrl.Contains(entityUrl)).ToList();

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel
                                .AccountNetwork, AccountModel.AccountBaseModel.UserName,
                                    lstPostIds.Count, queryType, entityUrl, _ActivityType);

                            jobProcessResult.maxId = responseHandler.PageletData;

                            jobProcessResult.HasNoResult = !responseHandler.HasMoreResults;

                            if (lstPostIds.Count() == 0)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                                break;
                            }

                            ProcessDataOfPosts(ref jobProcessResult, lstPostIds,
                                lstFriendUrl != null
                                ? PostOptions.ProfileScraper.ToString() : queryType.ToString(),
                                lstFriendUrl != null
                                ? campaignId : entityUrl);

                            jobProcessResult.maxId = responseHandler.PageletData;

                            jobProcessResult.HasNoResult = !jobProcessResult.HasNoResult
                                ? !responseHandler.HasMoreResults
                                : jobProcessResult.HasNoResult;
                            if (searchCount > 3 && !jobProcessResult.IsProcessSuceessfull) jobProcessResult.HasNoResult = true;

                        }
                        else
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (Exception ex)
                    {
                        jobProcessResult.HasNoResult = true;
                        ex.DebugLog();
                    }
                }

                if (jobProcessResult.IsProcessCompleted)
                    break;
            }
        }


        public void FilterAndStartFinalProcessForPost_PostScraperCampaign(ref JobProcessResult jobProcessResult, PostOptions queryType,
           List<string> lstItems, string campaignId, List<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts> lstPostUrl = null)
        {

            var lstShuffleInputData = lstPostUrl != null
                ? FdFunctions.FdFunctions.RandomShuffle(lstPostUrl).ToList()
                : FdFunctions.FdFunctions.RandomShuffle(lstPostUrl).ToList();

            try
            {
                var posts = new List<FacebookPostDetails>();

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                foreach (var entityUrl in lstShuffleInputData)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var post = new FacebookPostDetails()
                    {
                        PostUrl = !string.IsNullOrEmpty(entityUrl.PostUrl) ? entityUrl.PostUrl : $"{FdConstants.FbHomeUrl}{entityUrl.PostId}",
                        Id = entityUrl.PostId
                    };
                    posts.Add(post);
                }

                ProcessDataOfPosts(ref jobProcessResult, posts,
                                   PostOptions.PostScraperCampaign.ToString(), campaignId);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }
    }
}
