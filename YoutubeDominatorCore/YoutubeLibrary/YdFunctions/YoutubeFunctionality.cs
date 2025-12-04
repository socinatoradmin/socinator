using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ThreadUtils;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.YdFunctions
{
    public interface IYoutubeFunctionality
    {
        LikeDislikeResponseHandler LikeDislikeVideo(DominatorAccountModel dominatorAccount,
            ActivityType activityType, ref YoutubePost youtubePost, double delayBeforeHit = 0);

        LikeCommentResponseHandler LikeComments(DominatorAccountModel dominatorAccount, ref YoutubePost youtubePost,
            string messageText, double delayBeforeHit = 0);

        CommentResponseHandler CommentOnVideo(DominatorAccountModel dominatorAccount, ref YoutubePost youtubePost,
            string messageText, bool isReply, double delayBeforeHit = 0);

        ReportVideoResponseHandler ReportToVideo(DominatorAccountModel dominatorAccount, YoutubePost youtubePost,
            int option, int subOption, string text, int mins = 0, int sec = 0, double delayBeforeHit = 0);

        Task<bool> ViewIncreaserVideo(DominatorAccountModel dominatorAccount, YoutubePost youtubePost, int delay,
            string channelPageId, CancellationTokenSource cancellationTokenSource, bool mozillaSelected,
            bool browserHidden, bool skipAd, double delayBeforeHit = 0);

        SubscribeResponseHandler SubscribeChannel(DominatorAccountModel dominatorAccount, YoutubeChannel youtubeChannel,
            double delayBeforeHit = 0);

        UnsubscribeResponseHandler UnsubscribeChannel(DominatorAccountModel dominatorAccount,
            ref YoutubeChannel youtubeChannel, double delayBeforeHit = 0);

        string PublishVideoAndGetVideoId(DominatorAccountModel dominatorAccount, PublisherPostlistModel postDetails,
            string channelName, double delayBeforeHit = 0);

        SearchPostsResponseHandler ScrapPostsFromKeyword(DominatorAccountModel dominatorAccount, string keyword,
            SearchPostsResponseHandler searchPostsResponseHandler = null, string searchFilterUrlParam = "EgIQAQ%3D%3D",
            double delayBeforeHit = 0);

        ScrapPostFromChannelResponseHandler ScrapPostsFromChannel(DominatorAccountModel dominatorAccount,
            string keyword, PostDataElements postDataElements, HeadersElements headersElements,
            double delayBeforeHit = 0);

        PostInfoYdResponseHandler GetPostDetails(DominatorAccountModel dominatorAccount, string youtubeVideoUrl,
            bool hasVideoDuration = false, double delayBeforeHit = 0);

        PostCommentScraperResponseHandler ScrapePostCommentsDetails(DominatorAccountModel dominatorAccount,
            YoutubePost youtubePost, bool getCommentsList = true, bool getNecessaryElements = true,
            double delayBeforeHit = 0);

        SearchChannelsResponseHandler ScrapChannelsFromKeyword(DominatorAccountModel dominatorAccount, string keyword,
            PostDataElements postDataElements, HeadersElements headersElements, double delayBeforeHit = 0);

        ChannelInfoResponseHandler GetChannelDetails(DominatorAccountModel dominatorAccount, string subscribeUrl,
            bool getChannelDetails = true, bool getNecessaryElements = true, double delayBeforeHit = 0);

        SubscribedChannelScraperResponseHandler GetSubscribedChannels(DominatorAccountModel dominatorAccount,
            PostDataElements postDataElements, HeadersElements headersElements, double delayBeforeHit = 0);

        Task<OwnChannelScraperResponseHandler> ScrapOwnChannels(DominatorAccountModel dominatorAccount,
            double delayBeforeHit = 0);

        void SwitchChannel(DominatorAccountModel dominatorAccount, string pageId, double delayBeforeHit = 0);

        void DownloadGeckoDriver(DominatorAccountModel dominatorAccount);
    }

    [Localizable(false)]
    public class YoutubeFunctionality : IYoutubeFunctionality
    {
        private static readonly object DownloadGecko = new object();
        private readonly IDelayService _delayService;
        private readonly YoutubeModel _youtubeModel;
        public readonly IYdHttpHelper HttpHelper;

        public YoutubeFunctionality(IYdHttpHelper httpHelper, IDelayService delayService)
        {
            HttpHelper = httpHelper;
            _delayService = delayService;
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                _youtubeModel =
                    genericFileManager.GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile());
            }
            catch
            {
                _youtubeModel = new YoutubeModel();
            }
        }

        public LikeDislikeResponseHandler LikeDislikeVideo(DominatorAccountModel dominatorAccount,
            ActivityType activityType, ref YoutubePost youtubePost, double delayBeforeHit = 0)
        {
            try
            {
                GetPostDetailsIfNull(dominatorAccount, ref youtubePost);

                if (activityType == ActivityType.Like && youtubePost.LikeEnabled &&
                    youtubePost.Reaction != InteractedPosts.LikeStatus.Like || activityType == ActivityType.Dislike &&
                    youtubePost.DislikeEnabled && youtubePost.Reaction != InteractedPosts.LikeStatus.Dislike)
                {
                    var sej = Uri.EscapeDataString("{\"clickTrackingParams\":\"" +
                                                   youtubePost.PostDataElements.TrackingParams +
                                                   "\",\"commandMetadata\":{\"webCommandMetadata\":{\"url\":\"/service_ajax\",\"sendPost\":true,\"apiUrl\":\"/youtubei/v1/like/like\"}},\"likeEndpoint\":{\"status\":\"" +
                                                   (activityType == ActivityType.Like ? "LIKE" : "DISLIKE") +
                                                   "\",\"target\":{\"videoId\":\"" + youtubePost.Code + "\"}," +
                                                   (activityType == ActivityType.Like
                                                       ? "\"likeParams\":\"" + youtubePost.PostDataElements.LikeParams +
                                                         "\"}}"
                                                       : "\"dislikeParams\":\"" +
                                                         youtubePost.PostDataElements.DislikeParams + "\"}}"));
                    var postData =
                        $"sej={sej}&csn={youtubePost.PostDataElements.Csn}&session_token={youtubePost.PostDataElements.XsrfToken}";

                    const string url = "https://www.youtube.com/service_ajax?name=likeEndpoint";

                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.Like, youtubePost.HeadersElements);
                    _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                    var response = HttpHelper.PostRequest(url, postData);

                    return new LikeDislikeResponseHandler(response);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new LikeDislikeResponseHandler();
        }

        public LikeCommentResponseHandler LikeComments(DominatorAccountModel dominatorAccount,
            ref YoutubePost youtubePost, string messageText, double delayBeforeHit = 0)
        {
            try
            {
                if (youtubePost.Reaction != InteractedPosts.LikeStatus.Like)
                {
                    GetPostDetailsIfNull(dominatorAccount, ref youtubePost);

                    var objRequestParameters = new YdRequestParameters
                    {
                        JsonElements =
                            new YoutubeUtilities().CreateJsonPostDataForLikeComment(dominatorAccount, youtubePost)
                    };
                    var postDataByte = objRequestParameters.GetPostDataFromJson();

                    var postUrl = "https://www.youtube.com/service_ajax?name=performCommentActionEndpoint";

                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.LikeComment,
                        youtubePost.HeadersElements);
                    _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                    var response = HttpHelper.PostRequest(postUrl, postDataByte);

                    return new LikeCommentResponseHandler(response);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new LikeCommentResponseHandler();
        }

        public CommentResponseHandler CommentOnVideo(DominatorAccountModel dominatorAccount,
            ref YoutubePost youtubePost, string messageText, bool isReply, double delayBeforeHit = 0)
        {
            try
            {
                GetPostDetailsIfNull(dominatorAccount, ref youtubePost);

                var postUrl = $"https://www.youtube.com/youtubei/v1/comment/create_comment{(isReply ? "_reply" : "")}?key={youtubePost.PostDataElements.InnertubeApiKey}";

                var postData = "{\"context\":" + youtubePost.PostDataElements.InnertubeContext + ",\"internalExperimentFlags\":[],\"consistencyTokenJars\":[]},\"user\":{},\"clientScreenNonce\":\"MC4wOTA2MjAxMjg1NzcwMDQ3NA..\",\"clickTracking\":{\"clickTrackingParams\":\""
                                   + youtubePost.PostDataElements.TrackingParams +
                                   (!isReply ? "\"}},\"createCommentParams\":\"" + youtubePost.PostDataElements.CreateCommentParams : "\"}},\"createReplyParams\":\"" + youtubePost.PostDataElements.CreateReplyParams)
                                                   + "\",\"commentText\":\"" + Uri.EscapeDataString(messageText) + "\"}";

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper,
                    isReply ? YoutubeAct.CommentAsReply : YoutubeAct.Comment, youtubePost.HeadersElements);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                var response = HttpHelper.PostRequest(postUrl, postData);

                return new CommentResponseHandler(response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new CommentResponseHandler();
        }

        public ReportVideoResponseHandler ReportToVideo(DominatorAccountModel dominatorAccount, YoutubePost youtubePost,
            int option, int subOption, string text, int mins = 0, int sec = 0, double delayBeforeHit = 0)
        {
            try
            {
                GetPostDetailsIfNull(dominatorAccount, ref youtubePost);

                var clickedReport = GetReportForm(youtubePost, option, subOption, delayBeforeHit);

                youtubePost.PostDataElements.ClickTrackingParams = clickedReport.PostDataElements.ClickTrackingParams;
                youtubePost.PostDataElements.FlagAction = clickedReport.PostDataElements.FlagAction;
                youtubePost.PostDataElements.FlagRequestType = clickedReport.PostDataElements.FlagRequestType;
                var selectedOptionToVideoReport = clickedReport.SelectedOptionToVideoReport;

                var selectedOptions = SelectReportOptions(youtubePost, 2);

                youtubePost.PostDataElements.ClickTrackingParams = selectedOptions.PostDataElements.ClickTrackingParams;

                var submitted = SubmitAdditionalDetailsAndReport(youtubePost, text, mins, sec, 4);
                submitted.SelectedOptionToVideoReport = selectedOptionToVideoReport;
                return submitted;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new ReportVideoResponseHandler();
            }
        }

        public async Task<bool> ViewIncreaserVideo(DominatorAccountModel dominatorAccount, YoutubePost youtubePost,
            int delay,
            string channelPageId, CancellationTokenSource cancellationTokenSource, bool mozillaSelected,
            bool browserHidden, bool skipAd, double delayBeforeHit = 0)
        {
            if (string.IsNullOrEmpty(youtubePost?.PostUrl))
                return false;
            var viewed = false;
            try
            {
                lock (YdStatic.LockViewIncreaser)
                {
                    if (YdStatic.BrowserOpeningViewIncreaser++ > _youtubeModel.LimitNumberOfSimultaneousWatchVideos - 1)
                        Monitor.Wait(YdStatic.LockViewIncreaser);
                }

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (channelPageId == YdStatic.DefaultChannel)
                    channelPageId = $" [{YdStatic.DefaultChannel}]";
                ExtractChannelPageId(ref channelPageId);
                var urlWithSelectedChannel =
                    $"https://www.youtube.com/signin?authuser=0{channelPageId}&skip_identity_prompt=True&action_handle_signin=true&feature=masthead_switcher&next={Uri.EscapeDataString(youtubePost.PostUrl)}";

                GlobusLogHelper.log.Info(Log.StartedActivity, dominatorAccount.AccountBaseModel.AccountNetwork,
                    dominatorAccount.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                    $"{youtubePost.PostUrl} (Viewing times -> {youtubePost.TotalWatchingCount + 1})");

                await Task.Factory.StartNew(() => viewed = ViewIncreaseWithBrowser(dominatorAccount,
                    urlWithSelectedChannel, delay, cancellationTokenSource, mozillaSelected, browserHidden,
                    youtubePost.Title, skipAd));

                if (cancellationTokenSource.Token.IsCancellationRequested)
                    return false;
            }
            catch (OperationCanceledException)
            {
                lock (YdStatic.LockViewIncreaser)
                {
                    YdStatic.BrowserOpeningViewIncreaser--;
                    Monitor.Pulse(YdStatic.LockViewIncreaser);
                }

                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

            return viewed;
        }

        public SubscribeResponseHandler SubscribeChannel(DominatorAccountModel dominatorAccount,
            YoutubeChannel youtubeChannel, double delayBeforeHit = 0)
        {
            try
            {
                GetChannelDetailsIfNull(dominatorAccount, ref youtubeChannel);

                if (!youtubeChannel.IsSubscribed)
                {
                    var sej = Uri.EscapeDataString("{\"clickTrackingParams\":\"" +
                                                   youtubeChannel.PostDataElements.TrackingParams +
                                                   "\",\"commandMetadata\":{\"webCommandMetadata\":{\"url\":\"/service_ajax\",\"sendPost\":true,\"apiUrl\":\"/youtubei/v1/subscription/subscribe\"}},\"subscribeEndpoint\":{\"channelIds\":[\"" +
                                                   youtubeChannel.ChannelId + "\"],\"params\":\"" +
                                                   youtubeChannel.PostDataElements.Params + "\"}}");

                    var postData =
                        $"sej={sej}&csn={youtubeChannel.PostDataElements.Csn}&session_token={Uri.EscapeDataString(youtubeChannel.PostDataElements.XsrfToken)}";

                    const string postUrl = "https://www.youtube.com/service_ajax?name=subscribeEndpoint";

                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.Subscribe,
                        youtubeChannel.HeadersElements);

                    _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                    return new SubscribeResponseHandler(HttpHelper.PostRequest(postUrl, postData));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new SubscribeResponseHandler(false);
        }

        public UnsubscribeResponseHandler UnsubscribeChannel(DominatorAccountModel dominatorAccount,
            ref YoutubeChannel youtubeChannel, double delayBeforeHit = 0)
        {
            try
            {
                GetChannelDetailsIfNull(dominatorAccount, ref youtubeChannel);

                if (youtubeChannel.IsSubscribed)
                {
                    var sej = Uri.EscapeDataString("{\"clickTrackingParams\":\"" +
                                                   youtubeChannel.PostDataElements.TrackingParams +
                                                   "\",\"commandMetadata\":{\"webCommandMetadata\":{\"url\":\"/service_ajax\",\"sendPost\":true,\"apiUrl\":\"/youtubei/v1/subscription/unsubscribe\"}},\"unsubscribeEndpoint\":{\"channelIds\":[\"" +
                                                   youtubeChannel.ChannelId + "\"],\"params\":\"" +
                                                   youtubeChannel.PostDataElements.Params + "\"}}");

                    var postData =
                        $"sej={sej}&csn={youtubeChannel.PostDataElements.Csn}&session_token={youtubeChannel.PostDataElements.XsrfToken}";

                    const string postUrl = "https://www.youtube.com/service_ajax?name=unsubscribeEndpoint";

                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.Subscribe,
                        youtubeChannel.HeadersElements);

                    _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                    var objectReturn = new UnsubscribeResponseHandler(HttpHelper.PostRequest(postUrl, postData));

                    return objectReturn;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new UnsubscribeResponseHandler(false);
        }


        public string PublishVideoAndGetVideoId(DominatorAccountModel dominatorAccount,
            PublisherPostlistModel postDetails, string channelPageId, double delayBeforeHit = 0)
        {
            try
            {
                if (channelPageId == YdStatic.DefaultChannel)
                    channelPageId = $" [{YdStatic.DefaultChannel}]";

                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));

                // Switch channel if current active channel is different
                SwitchChannel(dominatorAccount, channelPageId);

                // Load Upload Video Page
                var clickedOnUploadPage = LoadUploadPage();

                var filePath = postDetails.IsChangeHashOfMedia
                    ? MediaUtilites.CalculateMD5Hash(postDetails.MediaList.GetRandomItem())
                    : postDetails.MediaList.GetRandomItem();
                // Get Uploading File Infos
                var fileInfo = new UploadingFileInfos(filePath);
                //var fileInfo = new UploadingFileInfos(postDetails.MediaList.GetRandomItem()); 

                #region Select file from uploading box (Sending selected file info)

                var pageId = channelPageId.Trim().Equals($"[{YdStatic.DefaultChannel}]") ? "" : channelPageId;

                var postData = !string.IsNullOrEmpty(pageId)
                    ? YdStatic.UploadingVideoPostDataWithPageId(pageId, fileInfo.FileNameWithoutExtension,
                        fileInfo.FileExtension, fileInfo.FileLength, clickedOnUploadPage)
                    : YdStatic.UploadingVideoPostData(fileInfo.FileNameWithoutExtension, fileInfo.FileExtension,
                        fileInfo.FileLength, clickedOnUploadPage);

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.UploadStep2,
                    new HeadersElements { ChannelPageId = pageId });

                HttpHelper.PostRequest("https://upload.youtube.com/upload/rupio?authuser=0", postData);

                #endregion

                #region Now, upload selected file in youtube server

                var locationHeader = HttpHelper.Response.GetResponseHeader("Location");

                HttpHelper.PostRequest(locationHeader, fileInfo.FileBytes);

                #endregion

                _delayService.ThreadSleep(TimeSpan.FromSeconds(3));

                // get videoId while uploading is on process
                var videoId = UploadingIsInProcess(dominatorAccount.AccountBaseModel.UserName, clickedOnUploadPage);
                if (string.IsNullOrWhiteSpace(videoId))
                    return "";

                #region Finally, Publish the uploaded video

                var finalPostData = YdStatic.PublishVideoPostData(videoId, postDetails.PublisherInstagramTitle,
                    postDetails.PostDescription,
                    fileInfo.FileNameWithoutExtension, clickedOnUploadPage.SessionToken);

                var postUrl = "https://www.youtube.com/metadata_ajax?action_edit_video=1";

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.UploadStep5,
                    clickedOnUploadPage.HeadersElements);

                var responseFinal = HttpHelper.PostRequest(postUrl, finalPostData);

                return responseFinal.Response.Contains("\"metadata_errors\":[]") ? videoId : "";

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public SearchPostsResponseHandler ScrapPostsFromKeyword(DominatorAccountModel dominatorAccount, string keyword,
            SearchPostsResponseHandler searchPostsResponseHandler = null, string searchFilterUrlParam = "EgIQAQ%3D%3D",
            double delayBeforeHit = 0)
        {
            try
            {
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                if (searchPostsResponseHandler == null)
                {
                    var url =
                        $"https://www.youtube.com/results?search_query={WebUtility.UrlEncode(keyword)}&sp={searchFilterUrlParam}"; // sp=EgIQAQ%253D%253D
                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.SearchAnything);
                    searchPostsResponseHandler = new SearchPostsResponseHandler(HttpHelper.GetRequest(url));

                    return searchPostsResponseHandler;
                }

                var urlPagi =
                    $"https://www.youtube.com/results?search_query={WebUtility.UrlEncode(keyword)}&sp={searchFilterUrlParam}"; //"https://www.youtube.com" + searchPostsResponseHandler.PostDataElements.AddInUrl;
                var nextPageUrl = urlPagi + "&pbj=1&ctoken=" +
                                  searchPostsResponseHandler.PostDataElements.ContinuationToken + "&continuation=" +
                                  searchPostsResponseHandler.PostDataElements.ContinuationToken + "&itct=" +
                                  searchPostsResponseHandler.PostDataElements.Itct;
                var nextPagePostData = "session_token=" + searchPostsResponseHandler.PostDataElements.XsrfToken;
                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.SearchAnythingInPagination,
                    searchPostsResponseHandler.HeadersElements);
                searchPostsResponseHandler =
                    new SearchPostsResponseHandler(HttpHelper.PostRequest(nextPageUrl, nextPagePostData),
                        searchPostsResponseHandler);
                if (!searchPostsResponseHandler.HasMoreResults)
                {
                    var url =
                        $"https://www.youtube.com/results?search_query={WebUtility.UrlEncode(keyword)}&sp={searchFilterUrlParam}"; // sp=EgIQAQ%253D%253D
                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.SearchAnything);
                    searchPostsResponseHandler = new SearchPostsResponseHandler(HttpHelper.GetRequest(url));

                    return searchPostsResponseHandler;
                }

                return searchPostsResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new SearchPostsResponseHandler();
        }

        public ScrapPostFromChannelResponseHandler ScrapPostsFromChannel(DominatorAccountModel dominatorAccount,
            string keyword, PostDataElements postDataElements, HeadersElements headersElements,
            double delayBeforeHit = 0)
        {
            try
            {
                string url;

                if (string.IsNullOrEmpty(postDataElements?.ContinuationToken))
                {
                    url = keyword.Contains("/videos") ? keyword : keyword + "/videos";
                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.ScrapePostsFromAChannel);
                }
                else
                {
                    var continuationToken = Uri.EscapeDataString(postDataElements.ContinuationToken);

                    url = "https://www.youtube.com/browse_ajax?ctoken=" + continuationToken + "&continuation="
                          + continuationToken + "&itct=" + postDataElements.Itct;

                    headersElements.RefererUrl = keyword + "/videos";
                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.ScrapePostsFromAChannelInPagination,
                        headersElements);
                }

                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                var objScrapPostFromChannelResponseHandler =
                    new ScrapPostFromChannelResponseHandler(HttpHelper.GetRequest(url), headersElements == null);

                if (headersElements != null)
                    objScrapPostFromChannelResponseHandler.HeadersElements = headersElements;

                return objScrapPostFromChannelResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new ScrapPostFromChannelResponseHandler();
            }
        }

        public PostInfoYdResponseHandler GetPostDetails(DominatorAccountModel dominatorAccount, string youtubeVideoUrl,
            bool hasVideoDuration = false, double delayBeforeHit = 0)
        {
            try
            {
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));

                if (youtubeVideoUrl.Contains("//youtu.be/"))
                    youtubeVideoUrl = youtubeVideoUrl.StringMatches(@"(youtu.be/)(.*?)")[0].Value
                        .Replace("youtu.be/", "");
                var url = youtubeVideoUrl.Contains("outube.com")
                    ? youtubeVideoUrl
                    : $"https://www.youtube.com/watch?v={youtubeVideoUrl}";

                var hasSpecificCommentId = url.ToLower().Contains("&lc=") && !url.Contains("&lc=Nothing");
                if (!hasSpecificCommentId)
                    url = $"{url}&lc=Nothing";

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.ScrapePostsFromAChannel);
                var objResponseParameter = HttpHelper.GetRequest(url);


                var postInfoHandler =
                    new PostInfoYdResponseHandler(objResponseParameter, hasSpecificCommentId: hasSpecificCommentId);

                Utilities.UpdateTestResponseDataFile(objResponseParameter.Response,
                    YdStatic.MyCoreLocation() + @".UnitTests\TestData\PostInfoYdResponse.html");
                return postInfoHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PostInfoYdResponseHandler();
            }
        }

        public PostCommentScraperResponseHandler ScrapePostCommentsDetails(DominatorAccountModel dominatorAccount,
            YoutubePost youtubePost, bool getCommentsList = true, bool getNecessaryElements = true,
            double delayBeforeHit = 0)
        {
            try
            {
                var commentServiceUrl =
                    "https://www.youtube.com/comment_service_ajax?action_get_comments=1&pbj=1&ctoken="
                    + Uri.EscapeDataString(youtubePost.PostDataElements.ContinuationToken) + "&continuation=" +
                    Uri.EscapeDataString(youtubePost.PostDataElements.ContinuationToken) + "&itct=" +
                    Uri.EscapeDataString(youtubePost.PostDataElements.ClickTrackingParams);

                var postData = "session_token=" + Uri.EscapeDataString(youtubePost.PostDataElements.XsrfToken);

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.ScrapeComments, youtubePost.HeadersElements);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                return new PostCommentScraperResponseHandler(HttpHelper.PostRequest(commentServiceUrl, postData),
                    getCommentsList, getNecessaryElements);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PostCommentScraperResponseHandler();
            }
        }

        public SearchChannelsResponseHandler ScrapChannelsFromKeyword(DominatorAccountModel dominatorAccount,
            string keyword, PostDataElements postDataElements, HeadersElements headersElements,
            double delayBeforeHit = 0)
        {
            try
            {
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                if (string.IsNullOrEmpty(postDataElements?.ContinuationToken))
                {
                    SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.SearchAnything);
                    return new SearchChannelsResponseHandler(
                        HttpHelper.GetRequest("https://www.youtube.com/results?sp=EgIQAg%253D%253D&search_query=" +
                                              keyword), headersElements == null);
                }

                var nextPageUrl = "https://www.youtube.com/results?sp=EgIQAg%253D%253D&search_query=" + keyword +
                                  "&pbj=1&ctoken=" + postDataElements.ContinuationToken + "&continuation=" +
                                  postDataElements.ContinuationToken + "&itct=" + postDataElements.Itct;
                var nextPagePostData = "session_token=" + postDataElements.XsrfToken;

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.SearchAnythingInPagination, headersElements);
                var response = HttpHelper.PostRequest(nextPageUrl, nextPagePostData);
                var handlerObj = new SearchChannelsResponseHandler(response, headersElements == null)
                {
                    HeadersElements = headersElements,
                    PostDataElements = { XsrfToken = postDataElements.XsrfToken, Itct = postDataElements.Itct }
                };

                return handlerObj;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new SearchChannelsResponseHandler();
        }

        public ChannelInfoResponseHandler GetChannelDetails(DominatorAccountModel dominatorAccount, string subscribeUrl,
            bool getChannelDetails = true, bool getNecessaryElements = true, double delayBeforeHit = 0)
        {
            try
            {
                subscribeUrl = new YoutubeUtilities().GeneratePostUrlFromYoutubeChannelObject(subscribeUrl);

                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));

                var response = HttpHelper.GetRequest(subscribeUrl);
                var responseHandler = new ChannelInfoResponseHandler(response, getChannelDetails, getNecessaryElements);

                Utilities.UpdateTestResponseDataFile(response.Response,
                    YdStatic.MyCoreLocation() + @".UnitTests\TestData\ChannelInfoPage.html");

                return responseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new ChannelInfoResponseHandler();
            }
        }

        public SubscribedChannelScraperResponseHandler GetSubscribedChannels(DominatorAccountModel dominatorAccount,
            PostDataElements postDataElements, HeadersElements headersElements, double delayBeforeHit = 0)
        {
            string feedChannelsUrl;

            if (string.IsNullOrEmpty(postDataElements?.ContinuationToken ?? ""))
            {
                feedChannelsUrl = "https://www.youtube.com/feed/channels";
                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.GetSubscribedChannels, headersElements);
            }
            else
            {
                feedChannelsUrl = "https://www.youtube.com/browse_ajax?ctoken=" + postDataElements?.ContinuationToken +
                                  "&continuation=" + postDataElements?.ContinuationToken
                                  + "&itct=" + postDataElements?.ClickTrackingParams;
                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.GetSubscribedChannelsAtPagination,
                    headersElements);
            }

            _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
            var channelsScraperResponse =
                new SubscribedChannelScraperResponseHandler(HttpHelper.GetRequest(feedChannelsUrl),
                    headersElements == null);

            if (headersElements != null)
                channelsScraperResponse.HeadersElements = headersElements;

            return channelsScraperResponse;
        }

        public async Task<OwnChannelScraperResponseHandler> ScrapOwnChannels(DominatorAccountModel dominatorAccount,
            double delayBeforeHit = 0)
        {
            _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
            var listChannelPage = await HttpHelper.GetRequestAsync(
                "https://www.youtube.com/channel_switcher?next=%2Faccount&feature=settings", dominatorAccount.Token);
            return new OwnChannelScraperResponseHandler(listChannelPage);
        }

        public void SwitchChannel(DominatorAccountModel dominatorAccount, string channelPageId,
            double delayBeforeHit = 0)
        {
            try
            {
                // Return if already set with the same channel
                if (channelPageId?.Contains($"[{dominatorAccount.AccountBaseModel.ProfileId}]") ?? true)
                    return;

                ExtractChannelPageId(ref channelPageId);

                var url =
                    $"https://www.youtube.com/signin?authuser=0{channelPageId}&skip_identity_prompt=True&action_handle_signin=true&feature=masthead_switcher"; //&next=https%3A%2F%2Fwww.youtube.com%2F

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.HitHomePage);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                var homeResponse = new YoutubeHomePageHandler(HttpHelper.GetRequest(url).Response);

                dominatorAccount.AccountBaseModel.Status = AccountStatus.Success;
                dominatorAccount.AccountBaseModel.UserId = homeResponse.ChannelId;
                dominatorAccount.AccountBaseModel.ProfileId = homeResponse.ChannelUsername;
                dominatorAccount.Cookies = HttpHelper.GetRequestParameter().Cookies;
                SocinatorAccountBuilder.Instance(dominatorAccount.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccount.AccountBaseModel)
                    .AddOrUpdateCookies(dominatorAccount.Cookies)
                    .SaveToBinFile();

                dominatorAccount.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(TimeSpan.FromSeconds(1.5));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Download GeckoDriver (if it doesn't exist in the specific folder) to folder "Socinator" from our server to run
        ///     mozilla firefox selenium Browser.
        /// </summary>
        public void DownloadGeckoDriver(DominatorAccountModel dominatorAccount)
        {
            try
            {
                if (File.Exists($"{ConstantVariable.GetPlatformBaseDirectory()}/geckodriver.exe"))
                    return;

                lock (DownloadGecko)
                {
                    if (File.Exists($"{ConstantVariable.GetPlatformBaseDirectory()}/geckodriver.exe"))
                        return;

                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                        dominatorAccount.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                        "Downloading driver to run mozilla browser (Downloading only for the first time). Please wait..");

                    var imgBytes = new WebClient().DownloadData("http://18.133.153.168/GeckoDriver/geckodriver.exe");
                    File.WriteAllBytes($"{ConstantVariable.GetPlatformBaseDirectory()}/geckodriver.exe", imgBytes);

                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                        dominatorAccount.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                        "Completed downloading driver process.");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private ReportVideoResponseHandler GetReportForm(YoutubePost youtubePost, int setOption, int setSubOption,
            double delayBeforeHit = 0)
        {
            try
            {
                var url = "https://www.youtube.com/service_ajax?name=getReportFormEndpoint";

                var sej = Uri.EscapeDataString("{\"clickTrackingParams\":\"" +
                                               youtubePost.PostDataElements.TrackingParams +
                                               "\",\"commandMetadata\":{\"webCommandMetadata\":{\"url\":\"/service_ajax\",\"sendPost\":true,\"apiUrl\":\"/youtubei/v1/flag/get_form\"}},\"getReportFormEndpoint\":{\"params\":\"" +
                                               youtubePost.PostDataElements.ReportFromEndPointParams + "\"}}");

                var postData =
                    $"sej={sej}&csn={youtubePost.PostDataElements.Csn}&session_token={Uri.EscapeDataString(youtubePost.PostDataElements.XsrfToken)}";

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.Report, youtubePost.HeadersElements);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                var response = HttpHelper.PostRequest(url, postData);
                //{"code":"ERROR","error":"This functionality is not available right now. Please try again later."}
                return new ReportVideoResponseHandler(response.Response, 1, setOption, setSubOption);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ReportVideoResponseHandler();
        }

        private ReportVideoResponseHandler SelectReportOptions(YoutubePost youtubePost, double delayBeforeHit = 0)
        {
            try
            {
                var url = "https://www.youtube.com/service_ajax?name=flagEndpoint";

                var sej = Uri.EscapeDataString("{\"clickTrackingParams\":\"" +
                                               youtubePost.PostDataElements.ClickTrackingParams +
                                               "\",\"commandMetadata\":{\"webCommandMetadata\":{\"url\":\"/service_ajax\",\"sendPost\":true,\"apiUrl\":\"/youtubei/v1/flag/flag\"}},\"flagEndpoint\":{\"flagAction\":\"" +
                                               youtubePost.PostDataElements.FlagAction + "\",\"flagRequestType\":\"" +
                                               youtubePost.PostDataElements.FlagRequestType + "\"}}");

                var postData =
                    $"sej={sej}&csn={youtubePost.PostDataElements.Csn}&session_token={Uri.EscapeDataString(youtubePost.PostDataElements.XsrfToken)}";

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.Report, youtubePost.HeadersElements);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                var response = HttpHelper.PostRequest(url, postData);

                return new ReportVideoResponseHandler(response.Response, 2);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ReportVideoResponseHandler();
        }

        private ReportVideoResponseHandler SubmitAdditionalDetailsAndReport(YoutubePost youtubePost, string text,
            int min, int sec, double delayBeforeHit = 0)
        {
            try
            {
                var url = "https://www.youtube.com/service_ajax?name=flagEndpoint";

                var sej = Uri.EscapeDataString("{\"clickTrackingParams\":\"" +
                                               youtubePost.PostDataElements.ClickTrackingParams +
                                               "\",\"commandMetadata\":{\"webCommandMetadata\":{\"url\":\"/service_ajax\",\"sendPost\":true,\"apiUrl\":\"/youtubei/v1/flag/flag\"}},\"flagEndpoint\":{\"flagAction\":\"" +
                                               youtubePost.PostDataElements.FlagAction + "\"}}");

                var postData =
                    $"text={Uri.EscapeDataString(text)}&minutes={min}&seconds={sec}&sej={sej}&csn={youtubePost.PostDataElements.Csn}&session_token={Uri.EscapeDataString(youtubePost.PostDataElements.XsrfToken)}";

                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.Report, youtubePost.HeadersElements);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                var response = HttpHelper.PostRequest(url, postData);

                return new ReportVideoResponseHandler(response.Response, 3);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new ReportVideoResponseHandler();
        }

        private bool ViewIncreaseWithBrowser(DominatorAccountModel dominatorAccount, string youtubeVideoUrl, int delay,
            CancellationTokenSource cancellationTokenSource, bool mozillaSelected, bool hiddenBrowser,
            string videoTitle, bool skipAd)
        {
            BrowserWindow browserWindow = null;
            var goWithMozilla = false;
            try
            {
                var isPrivateProxy =
                    !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy?.ProxyIp?.Trim() ?? "") &&
                    !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy?.ProxyUsername?.Trim() ?? "");
                goWithMozilla = mozillaSelected
                                && !isPrivateProxy
                                && File.Exists($"{ConstantVariable.GetPlatformBaseDirectory()}/geckodriver.exe");
                if (goWithMozilla)
                    return new MozillaBrowser(dominatorAccount, cancellationTokenSource.Token, hiddenBrowser)
                        .StartWatching(youtubeVideoUrl, delay, videoTitle, skipAd);
                if (mozillaSelected && !goWithMozilla)
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                        dominatorAccount.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                        "Driver to run mozilla browser is Not  SuccessFull So Trying to Play in Embedded Browser");


                var openCef = new Task(() =>
                {
                    try
                    {
                        var visibility = hiddenBrowser ? Visibility.Hidden : Visibility.Visible;
                        Application.Current.Dispatcher.Invoke(async () =>
                        {
                            if (cancellationTokenSource.Token.IsCancellationRequested) return;
                            browserWindow = new BrowserWindow(dominatorAccount, cancellationTokenSource.Token,
                                    youtubeVideoUrl, true, skipAd)
                            { Visibility = visibility };
                            await browserWindow.SetCookie();
                            browserWindow.Show();
                            browserWindow.Visibility = visibility;
                        });

                        var increaseTime = 0;
                        while (delay > ++increaseTime)
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            while (browserWindow.FoundAd)
                            {
                                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _delayService.ThreadSleep(1000);
                            }

                            _delayService.ThreadSleep(1000);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        /*Just Catch It*/
                    }
                });
                openCef.Start();
                openCef.Wait(cancellationTokenSource.Token);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                try
                {
                    if (!goWithMozilla && browserWindow != null)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            browserWindow.Close();
                            browserWindow.Dispose();
                        });

                    lock (YdStatic.LockViewIncreaser)
                    {
                        YdStatic.BrowserOpeningViewIncreaser--;
                        Monitor.Pulse(YdStatic.LockViewIncreaser);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private UploadPageResponseHandler LoadUploadPage(double delayBeforeHit = 0)
        {
            try
            {
                SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.UploadStep1);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                return new UploadPageResponseHandler(HttpHelper
                    .GetRequest(
                        $"https://www.youtube.com/upload?redirect_to_creator=true&fr=4&ar={DateTime.Now.GetCurrentEpochTimeMilliSeconds()}&nv=1" /*"https://www.youtube.com/upload"*/)
                    .Response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new UploadPageResponseHandler();
            }
        }

        private string UploadingIsInProcess(string accountUsername, UploadPageResponseHandler uploadPageResponseHand,
            double delayBeforeHit = 0)
        {
            var feedbackListPostUrl = "https://www.googleapis.com/videofeedback/v1/feedback/list";

            var splitUploadId = uploadPageResponseHand.UploadId.Split(':');

            var feedbackListPostData = "{\"client\":{\"name\":\"youtube_web_uploads\",\"version\":\"" +
                                       splitUploadId[0] + "\",\"token\":\"" + splitUploadId[1] +
                                       "\"},\"queries\":[{\"id\":{\"frontendKey\":\"web_upload:" +
                                       uploadPageResponseHand.FrontEndUploadIdBase +
                                       ":0\"},\"choice\":{\"editorSuggestions\":true,\"videoId\":true,\"videoInfo2\":true,\"videoIssues\":true,\"processingProgress\":true,\"youtubeStatus\":true,\"thumbnailsDone\":true}}]}";

            IResponseParameter response;
            var waitTime = 0;
            var videoId = "";
            SetHeaders.SetWebHeadersBeforeClick(HttpHelper, YoutubeAct.UploadStep4);
            do
            {
                _delayService.ThreadSleep(TimeSpan.FromSeconds(delayBeforeHit));
                response = HttpHelper.PostRequest(feedbackListPostUrl, feedbackListPostData);

                if (response.Response.Contains("\"videoStatus\": \"STATUS_REJECTED\""))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                        accountUsername, ActivityType.Post,
                        "failed with error " +
                        Utilities.GetBetween(response.Response, "\"rejectionReason\": \"", "\""));
                    return "";
                }

                if (string.IsNullOrEmpty(videoId))
                    videoId = Utilities.GetBetween(response.Response, "\"youtubeId\": \"", "\"");

                _delayService.ThreadSleep(TimeSpan.FromSeconds(3));
                waitTime += 3;
            } while (!response.Response.Contains("\"videoStatus\": \"STATUS_SUCCESS\"") && !(waitTime >= 30));

            return videoId;
        }

        public void GetPostDetailsIfNull(DominatorAccountModel dominatorAccount, ref YoutubePost youtubePost,
            double delayBeforeHit = 0)
        {
            if (youtubePost != null) return;
            youtubePost = GetPostDetails(dominatorAccount, youtubePost?.PostUrl, delayBeforeHit: delayBeforeHit)
                .YoutubePost;
            _delayService.ThreadSleep(1000);
        }

        public void GetChannelDetailsIfNull(DominatorAccountModel dominatorAccount, ref YoutubeChannel youtubeChannel,
            double delayBeforeHit = 0)
        {
            if (youtubeChannel != null) return;
            youtubeChannel = GetChannelDetails(dominatorAccount, youtubeChannel?.ChannelUrl).YoutubeChannel;
            _delayService.ThreadSleep(1500);
        }

        public void ExtractChannelPageId(ref string pageIdContent)
        {
            pageIdContent =
                string.IsNullOrEmpty(pageIdContent) || pageIdContent.Contains($" [{YdStatic.DefaultChannel}]")
                    ? ""
                    : pageIdContent;

            if (string.IsNullOrEmpty(pageIdContent)) return;

            var reg = Regex
                .Matches(pageIdContent, @"\d{19,21}");
            if (reg.Count > 0)
                pageIdContent = $"&pageid={reg[0]}";
        }
    }
}