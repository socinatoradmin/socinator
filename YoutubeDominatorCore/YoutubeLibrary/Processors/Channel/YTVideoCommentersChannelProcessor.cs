using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Channel
{
    internal class YTVideoCommentersChannelProcessor : BaseYoutubeChannelProcessor
    {
        private readonly ChannelScraperModel _channelScraperModel;
        private readonly SubscribeModel _subscribeModel;
        private string videoId = "";

        public YTVideoCommentersChannelProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality) : base(jobProcess, blackWhiteListHandler, campaignService,
            youtubeFunctionality)
        {
            if (ActType == ActivityType.Subscribe)
                _subscribeModel = JsonConvert.DeserializeObject<SubscribeModel>(InstanceProvider
                    .GetInstance<ITemplatesFileManager>().Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)
                    ?.ActivitySettings);
            else
                _channelScraperModel = JsonConvert.DeserializeObject<ChannelScraperModel>(InstanceProvider
                    .GetInstance<ITemplatesFileManager>().Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)
                    ?.ActivitySettings);
        }

        protected override void Process(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            JobProcess.IsCustom = true;

            var url = PostUrlWithCommentId(queryInfo.QueryValue);
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url, clearandNeedResource: true);
            var postInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                ? BrowserManager.PostInfo(ActType, url, sortCommentsByNew: ActType == ActivityType.Subscribe || ActType == ActivityType.ChannelScraper, needChannelDetails: true)
                : YoutubeFunction.GetPostDetails(JobProcess.DominatorAccountModel, url, delayBeforeHit: 2.5);

            if (postInfo == null || !postInfo.Success || postInfo.IsLiveVideo ||
                SkipBlackListOrWhiteList(new List<YoutubePost> { postInfo.YoutubePost }).Count == 0)
            {
                CustomLog($"Failed to fetch info from custom post url ( {queryInfo.QueryValue} ).");
            }
            else
            {
                videoId = postInfo.YoutubePost.Code;
                StartCustomPostProcess(queryInfo, ref jobProcessResult, postInfo.YoutubePost, true);
            }
        }

        public void StartCustomPostProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            YoutubePost postInfo, bool isCustom = false)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (CommnetsRequiredAndCommentDisabled(postInfo, isCustom))
                    return;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var lstCommentIds = GetPostCommentInfo(postInfo);

                StartProcessForCommenters(postInfo, ref jobProcessResult, queryInfo, lstCommentIds);
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

        private PostCommentScraperResponseHandler GetPostCommentInfo(YoutubePost newYoutubePost)
        {
            var lstCommentIds = new PostCommentScraperResponseHandler();
            lstCommentIds.ListOfCommentsInfo = new List<YoutubePostCommentModel>();
            lstCommentIds.ListOfCommentsInfo.AddRange(newYoutubePost.ListOfCommentsInfo ??
                                                      new List<YoutubePostCommentModel>());

            return lstCommentIds;
        }

        private void StartProcessForCommenters(YoutubePost post, ref JobProcessResult jobProcessResult,
            QueryInfo queryInfo, PostCommentScraperResponseHandler lstCommentIds)
        {
            jobProcessResult.maxId = string.Empty;
            var lessThan10 = lstCommentIds.ListOfCommentsInfo.Count < 10;

            int? commentersCountToInteract = null;
            var act = ActType == ActivityType.Subscribe ? "subscribing" : "scraping";
            if ((_subscribeModel?.IsCheckSubscribeNCommenters ?? false) ||
                (_channelScraperModel?.IsCheckScrapeNCommenters ?? false))
            {
                commentersCountToInteract = (_subscribeModel != null
                    ? _subscribeModel.SubscribeNCommenters
                    : _channelScraperModel.ScrapeNCommenters).GetRandom();

                CustomLog(
                    $"Got random count [{commentersCountToInteract}] for {act} commenters in the video [{post.Code}] for this running query [{queryInfo.QueryType} {queryInfo.QueryValue}]");
            }

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (lstCommentIds.ListOfCommentsInfo == null || lstCommentIds.ListOfCommentsInfo.Count == 0 ||
                    lstCommentIds.ListOfCommentsInfo.Count == 0)
                {
                    NoMoreDataLog();
                    jobProcessResult.maxId = null;
                    return;
                }

                if (!string.IsNullOrEmpty(lstCommentIds.PostDataElements.ContinuationToken))
                    post.PostDataElements.ContinuationToken = lstCommentIds.PostDataElements.ContinuationToken;
                jobProcessResult.maxId = post.PostDataElements.ContinuationToken;
                StartProcessForListOfComments(queryInfo, ref jobProcessResult, post, lstCommentIds.ListOfCommentsInfo,
                    ref commentersCountToInteract);

                if (commentersCountToInteract == 0)
                {
                    CustomLog(
                        $"{act} commenters count limit reached in the video [{post.Code}] for this running query [{queryInfo.QueryType} {queryInfo.QueryValue}]\nGoing for next query if any..");
                    break;
                }

                if (lessThan10)
                    break;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {

                        var lastCommentId = lstCommentIds.ListOfCommentsInfo?.Last()?.CommentId ?? "";
                        lstCommentIds = new PostCommentScraperResponseHandler();
                        lstCommentIds.ListOfCommentsInfo = GetVideoCommentInfoByBrowser(post, lastCommentId);
                        lstCommentIds.HasMoreResults = lstCommentIds.ListOfCommentsInfo.Count > 0;
                    }
                    else
                    {
                        lstCommentIds =
                            YoutubeFunction.ScrapePostCommentsDetails(JobProcess.DominatorAccountModel, post);
                    }
                }
            }
        }

        private List<YoutubePostCommentModel> GetVideoCommentInfoByBrowser(YoutubePost youtubePost,
            string lastCommentId)
        {
            var index = 5 + youtubePost.CommentLikeIndex;

            var lstCommentIds = BrowserManager.ScrapePostCommentsDetails(ActType, lastCommentId, ref index, 3);

            return lstCommentIds ?? new List<YoutubePostCommentModel>();
        }

        private void StartProcessForListOfComments(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            YoutubePost newYoutubePost, List<YoutubePostCommentModel> comments, ref int? commentersCountToSubscribe)
        {
            var setIndex = newYoutubePost.CommentLikeIndex;

            var ytChannelsList = new List<YoutubeChannel>();
            foreach (var post in comments)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var alreadyDonefromDb = AlreadyDoneInDB(queryInfo, post.CommenterChannelId, "");
                var skipped = alreadyDonefromDb ? false : SkipCommentersHaveLessLikes(post.CommentLikesCount);

                if (alreadyDonefromDb || skipped || post.CommenterChannelId ==
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserId)
                {
                    if (skipped)
                    {
                        FilterOrFailed++;
                        if (ReturnIfFilterFailedOrLimitReached(queryInfo, ref jobProcessResult))
                            return;
                    }

                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        ++setIndex;
                        newYoutubePost.CommentLikeIndex = setIndex;
                        BrowserManager.ScrollScreen(100);
                    }

                    continue;
                }

                if (!ytChannelsList.Any(x => x.ChannelId == post.CommenterChannelId))
                    ytChannelsList.Add(new YoutubeChannel
                    {
                        ChannelId = post.CommenterChannelId,
                        InteractedCommentUrl = YdStatic.VideoUrl(videoId, post.CommentId)
                    });
            }

            var blacklistedFiltred = SkipBlackListOrWhiteList(ytChannelsList);
            foreach (var channel in blacklistedFiltred)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    if (ReturnIfFilterFailedOrLimitReached(queryInfo, ref jobProcessResult))
                        return;

                    var url = YdStatic.ChannelUrl(channel.ChannelId, channel.ChannelUsername);
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url, clearandNeedResource: true);

                    var channelInfoResponseHandler = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                        ? BrowserManager.GetChannelDetails(url, 2)
                        : YoutubeFunction.GetChannelDetails(JobProcess.DominatorAccountModel, url, delayBeforeHit: 2);

                    if (channelInfoResponseHandler.Success)
                    {
                        StartCustomChannelProcess(queryInfo, ref jobProcessResult,
                            channelInfoResponseHandler.YoutubeChannel);
                        if (commentersCountToSubscribe != null)
                        {
                            --commentersCountToSubscribe;
                            if (commentersCountToSubscribe == 0)
                                return;
                        }
                    }

                    ++setIndex;
                    newYoutubePost.CommentLikeIndex = setIndex;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private bool SkipCommentersHaveLessLikes(string likesCountOnComment)
        {
            try
            {
                if (_subscribeModel?.IsCheckSkipTheCommentersCommentHaveLessThanNLikes ?? false)
                    return Convert.ToInt32(YoutubeUtilities.YoutubeElementsCountInNumber(likesCountOnComment)) <
                           _subscribeModel.SkipTheCommentersCommentHaveLessThanNLikes;

                if (_channelScraperModel?.IsCheckSkipTheCommentersCommentHaveLessThanNLikes ?? false)
                    return Convert.ToInt32(YoutubeUtilities.YoutubeElementsCountInNumber(likesCountOnComment)) <
                           _channelScraperModel.SkipTheCommentersCommentHaveLessThanNLikes;
            }
            catch
            {
            }

            return false;
        }
    }
}