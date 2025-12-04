using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace RedditDominatorCore.Publisher
{
    public class RdPublisherJobProcess : PublisherJobProcess
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IRdHttpHelper _httpHelper;
        private readonly IRedditLogInProcess _redditLogInProcess;
        private readonly IRedditFunction _redditFunct;
        private readonly IRdBrowserManager _browserManager;

        public RdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _httpHelper = _accountScopeFactory[accountId].Resolve<IRdHttpHelper>();
            _redditLogInProcess = _accountScopeFactory[accountId].Resolve<IRedditLogInProcess>();
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            RedditModel = genericFileManager.GetModuleDetails<RedditModel>
                                  (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Reddit))
                              .FirstOrDefault(x => x.CampaignId == campaignId) ?? new RedditModel();
        }

        public RdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _httpHelper = _accountScopeFactory[accountId].Resolve<IRdHttpHelper>();
            _redditLogInProcess = _accountScopeFactory[accountId].Resolve<IRedditLogInProcess>();
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            RedditModel = genericFileManager.GetModuleDetails<RedditModel>
                                  (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Reddit))
                              .FirstOrDefault(x => x.CampaignId == campaignId) ?? new RedditModel();

            _browserManager = _redditLogInProcess._browserManager;
            _redditFunct = InstanceProvider.GetInstance<IRedditFunction>();
        }

        public ModuleSetting ModuleSetting { get; set; }
        public RedditModel RedditModel { get; set; }

        public override bool DeletePost(string postId) => true;

        public override bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails,
            bool isDelayNeeded = true)
        {
            if (!AccountModel.IsRunProcessThroughBrowser)
            {
                PublishPostResponseHandler finalResponse = null;
                var redditFunction =
                    _accountScopeFactory[accountId].Resolve<IRedditFunction>();
                var accountParameters = new PaginationParameter();
                Task.Factory.StartNew(() =>
                {
                    var updatedPostDetails = PerformGeneralSettings(postDetails);
                    updatedPostDetails = PerformRedditPostSetting(updatedPostDetails, RedditModel);
                    //To share post on wall from subreddit/user url
                    if (string.Empty != postDetails.ShareUrl && postDetails.PostSource == PostSource.SharePost)
                    {
                        GetAccountParameters(accountParameters);
                        redditFunction.GetSubmitPage(AccountModel, accountParameters);
                        var redditPostDetails = new PublishPostModel();
                        RedditPostResponseHandler redditPostResponseHandler = null;
                        redditPostResponseHandler = redditFunction.ScrapePostsByUrl(AccountModel,
                        updatedPostDetails.ShareUrl, QueryInfo.NoQuery, redditPostResponseHandler);
                        ApplyPostSettings(updatedPostDetails, redditPostDetails, redditFunction, accountParameters);
                        var groupUserName = GetGroupUserName(groupUrl);
                        if (redditPostResponseHandler.LstRedditPost.Count > 0)
                        {
                            var redditPost = redditPostResponseHandler.LstRedditPost[0];

                            redditPostDetails.CrosspostFullname = redditPost.PostId;
                            redditPostDetails.ApiType = "json";
                            redditPostDetails.Title = redditPost.Title;
                            redditPostDetails.SubmitType = "subreddit";
                            redditPostDetails.ValidateOnSubmit = true;
                            redditPostDetails.Sr = groupUserName;
                            redditPostDetails.Url = postDetails.ShareUrl;

                            finalResponse = redditFunction.PublishPostCrossPost(AccountModel, redditPostDetails,
                                accountParameters);
                            if (finalResponse.Success)
                            {
                                postDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                                {
                                    AccountName = AccountModel.UserName,
                                    DestinationUrl = groupUrl,
                                    Destination = "Group",
                                    Description = postDetails.PostDescription,
                                    IsPublished = finalResponse.Success ? "Yes" : "No",
                                    Successful = finalResponse.Success ? "Yes" : "No",
                                    Link = finalResponse.PostUrl,
                                    PublishedDate = DateTime.Now
                                });
                                UpdatePostWithSuccessful(groupUrl, postDetails, finalResponse.PostUrl);
                                GlobusLogHelper.log.Info(Log.PublishingSuccessfully,
                                    AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, "Group", groupUrl);
                            }
                            else
                            {
                                UpdatePostWithFailed(groupUrl, postDetails, finalResponse.FailureMessage);
                                GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, "Group",
                                    groupUrl + $" {finalResponse.FailureMessage}");
                            }

                        }
                    }
                    else
                    {
                        PostInGroup(groupUrl, postDetails, out finalResponse, redditFunction, accountParameters,
                                                out updatedPostDetails);
                        if (finalResponse.Success)
                        {
                            //var postedId = redditFunction.GetLatestPostUrlWithGateWay(accountParameters, AccountModel);
                            //if (string.IsNullOrEmpty(finalResponse.PostUrl) && !finalResponse.PostUrl.Contains(postedId))
                            //    finalResponse.PostUrl = $"https://www.reddit.com{groupUrl?.TrimEnd('/')}" + "/comments/" +
                            //                            postedId + "/" + updatedPostDetails.PublisherInstagramTitle
                            //                                .ToLower().Replace(" ", "_");

                            updatedPostDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                            {
                                AccountName = AccountModel.UserName,
                                DestinationUrl = groupUrl,
                                Destination = "Group",
                                Description = updatedPostDetails.PostDescription,
                                IsPublished = finalResponse.Success ? "Yes" : "No",
                                Successful = finalResponse.Success ? "Yes" : "No",
                                Link = finalResponse.PostUrl,
                                PublishedDate = DateTime.Now
                            });
                            UpdatePostWithSuccessful(groupUrl, postDetails, finalResponse.PostUrl);
                            GlobusLogHelper.log.Info(Log.PublishingSuccessfully,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, "Group", groupUrl);
                        }
                        else
                        {
                            UpdatePostWithFailed(groupUrl, postDetails, finalResponse.FailureMessage);
                            GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, "Group",
                                groupUrl + $" {finalResponse.FailureMessage}");
                        }
                    }

                }).Wait();
                if (isDelayNeeded) DelayBeforeNextPublish();
                return finalResponse.Success;
            }
            //For browser automation

            var publishSuccess = false;
            var PostUrl = string.Empty;
            var ErrorMessage = string.Empty;
            Task.Factory.StartNew(() =>
            {
                var updatedPostDetails = PerformGeneralSettings(postDetails);
                updatedPostDetails = PerformRedditPostSetting(updatedPostDetails, RedditModel);
                _redditLogInProcess.LoginWithBrowserMethod(AccountModel, CampaignCancellationToken.Token);
                if (!AccountModel.IsUserLoggedIn || !FilterMatched(updatedPostDetails)) return;
                var MediaType = string.Empty;
                if (updatedPostDetails.MediaList.Count > 0)
                    MediaType = MediaUtilites.GetMimeTypeByFilePath(updatedPostDetails.MediaList.FirstOrDefault());
                var groupUserName = GetGroupUserName(groupUrl);
                if (string.IsNullOrEmpty(MediaType) ? false : MediaType.Contains("video") && !string.IsNullOrEmpty(groupUserName))
                {
                    var VideoPublishResponse = PublishPostAndMedia(accountId, updatedPostDetails, groupUserName);
                    publishSuccess = VideoPublishResponse.Item1;
                    PostUrl = VideoPublishResponse.Item2;
                    ErrorMessage = VideoPublishResponse.Item3;
                }
                else
                    publishSuccess = _browserManager.Publisher(AccountModel, updatedPostDetails, groupUserName, OtherConfiguration, out ErrorMessage);
                if (publishSuccess)
                {
                    var accountParameters = new PaginationParameter();
                    var postUrl = string.IsNullOrEmpty(PostUrl) ? _browserManager.BrowserWindow.CurrentUrl() : PostUrl;
                    updatedPostDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                    {
                        AccountName = AccountModel.UserName,
                        DestinationUrl = groupUrl,
                        Destination = "Group",
                        Description = updatedPostDetails.PostDescription,
                        IsPublished = publishSuccess ? "Yes" : "No",
                        Successful = publishSuccess ? "Yes" : "No",
                        Link = postUrl,
                        PublishedDate = DateTime.Now
                    });
                    UpdatePostWithSuccessful(groupUrl, postDetails, postUrl);
                    GlobusLogHelper.log.Info(Log.PublishingSuccessfully, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Group", groupUrl);
                    isDelayNeeded = true;
                }
                else
                {
                    UpdatePostWithFailed(groupUrl, postDetails, "Not Publish");
                    GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Group", $"{groupUrl} ==> {ErrorMessage}");
                }

                _browserManager.CloseBrowser();
            }).Wait();
            if (isDelayNeeded)
                DelayBeforeNextPublish();

            return publishSuccess;
        }

        private PublisherPostlistModel PerformRedditPostSetting(PublisherPostlistModel updatedPostDetails, RedditModel redditModel)
        {
            var updatedRedditPostSettings = updatedPostDetails.DeepClone();
            updatedRedditPostSettings.RedditPostSetting.IsNsfw = redditModel.IsMarkAsNsfw && redditModel.IsNsfwSubOptions;
            updatedRedditPostSettings.RedditPostSetting.IsSpoiler = redditModel.IsMarkAsSpoiler && redditModel.IsSpoilerSubOptions;
            updatedRedditPostSettings.RedditPostSetting.IsOriginalContent = redditModel.IsMarkAsOriginalContent && redditModel.IsOriginalContentSubOptions;
            updatedRedditPostSettings.RedditPostSetting.IsDisableSendingReplies = redditModel.IsDisableSendingSubOptions && redditModel.IsDisplaySendingReplies;
            return updatedRedditPostSettings;
        }

        private bool FilterMatched(PublisherPostlistModel updatedPostDetails)
        {
            if (!IsFilterApplied(AccountModel, updatedPostDetails))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, updatedPostDetails.PostSource.ToString(), "Filter Not Matched!");
                if (_browserManager != null && _browserManager.BrowserWindow != null)
                    _browserManager.CloseBrowser();
                return false;
            }
            return true;
        }

        private void PostInGroup(string groupUrl, PublisherPostlistModel postDetails,
            out PublishPostResponseHandler finalResponse, IRedditFunction redditFunction,
            PaginationParameter accountParameters, out PublisherPostlistModel updatedPostDetails)
        {
            updatedPostDetails = PerformGeneralSettings(postDetails);
            updatedPostDetails = PerformRedditPostSetting(updatedPostDetails, RedditModel);
            GetAccountParameters(accountParameters);
            redditFunction.GetSubmitPage(AccountModel, accountParameters);

            var groupUserName = GetGroupUserName(groupUrl);
            var mediaType = string.Empty;
            if (updatedPostDetails.MediaList.Count > 0)
                mediaType = MediaUtilites.GetMimeTypeByFilePath(updatedPostDetails.MediaList.FirstOrDefault());
            var groupPublishResponse = PublishPostAndMedia(AccountModel.AccountId, updatedPostDetails, groupUserName, mediaType.Contains("video"));
            finalResponse = new PublishPostResponseHandler(new DominatorHouseCore.Request.ResponseParameter()) { FailureMessage = groupPublishResponse.Item3, Success = groupPublishResponse.Item1, PostUrl = groupPublishResponse.Item2 };

            #region Old Code For Video Publish.
            //redditFunction.GetSubRedditSubmitValidation(AccountModel, accountParameters, groupUserName);

            ////GlobusLogHelper.log.Debug($"{AccountModel.UserName}-------{groupUserName}---------{subRedditSubmitValidationResponse.Response}");
            //var redditPostDetails = new PublishPostModel();
            //ApplyPostSettings(updatedPostDetails, redditPostDetails, redditFunction, accountParameters);

            //redditPostDetails.ApiType = "json";
            //redditPostDetails.Title = updatedPostDetails.PublisherInstagramTitle;

            //// If there is no title and postdescription is given in that case on the basis of url we fetch title of that Url and publish it
            //if (string.IsNullOrEmpty(updatedPostDetails.PublisherInstagramTitle) &&
            //    string.IsNullOrEmpty(redditPostDetails.Text))
            //    redditPostDetails.Title = redditFunction.FetchTitleOfUrl(AccountModel, redditPostDetails);

            //redditPostDetails.SubmitType = "subreddit";
            //redditPostDetails.ValidateOnSubmit = true;
            //redditPostDetails.Sr = groupUserName;

            //// Here we are passing OtherConfiguration for checking Approve post funcitonality whether it is checked or not in SocioPublisher PostConfiguration - OtherConfiguration
            //finalResponse =
            //    redditFunction.PublishPost(AccountModel, redditPostDetails, accountParameters, OtherConfiguration);
            #endregion
        }

        private static string GetGroupUserName(string groupUrl)
        {
            return groupUrl.Contains("https://www.reddit.com")
                ? Regex.Replace(groupUrl, "https://www.reddit.com/r/", "").Replace("/", "")
                : groupUrl.Contains("https://www.reddit.com") ? Regex.Replace(groupUrl, "https://www.reddit.com/r/", "").Replace("/", "")
                : Regex.Replace(groupUrl, "r/", "").Replace("/", "");
        }

        private void ApplyPostSettings(PublisherPostlistModel postDetails, PublishPostModel redditPostDetails,
            IRedditFunction redditFunction, PaginationParameter accountParameters)
        {
            ApplyGeneralSetting(postDetails, redditPostDetails);
            if (postDetails.MediaList.Count > 0)
            {
                foreach (var media in postDetails.MediaList)
                {
                    var mediaFilePath = media?.Replace("_SOCINATORIMAGE.jpg", string.Empty);
                    var postDetailsClone = new PublisherPostlistModel();

                    //For scraped post to publish
                    if (postDetails.PostSource == PostSource.ScrapedPost)
                        try
                        {
                            var downloadPath =
                                $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Socinator\\RdScrapeFiles";
                            if (!Directory.Exists(downloadPath))
                                Directory.CreateDirectory(downloadPath);

                            var webClient = new WebClient();

                            if (postDetails.RedditScrapedMediaType.Equals("image"))
                            {
                                var downloadDataByte = webClient.DownloadData(postDetails.RedditScrapedVideoUrl);
                                File.WriteAllBytes($"{downloadPath}\\{postDetails.FetchedPostIdOrUrl}.mp4",
                                    downloadDataByte);
                                mediaFilePath = $"{downloadPath}\\{postDetails.FetchedPostIdOrUrl}.jpg";
                            }
                            else
                            {
                                var downloadDataByte = webClient.DownloadData(media);
                                File.WriteAllBytes($"{downloadPath}\\{postDetails.FetchedPostIdOrUrl}.jpg",
                                    downloadDataByte);
                                mediaFilePath = $"{downloadPath}\\{postDetails.FetchedPostIdOrUrl}.jpg";
                            }

                            postDetailsClone = postDetails.DeepCloneObject();
                            postDetailsClone.MediaList = new ObservableCollection<string>
                            {
                                mediaFilePath
                            };
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    else
                        postDetailsClone = postDetails.DeepCloneObject();
                    //To generate new Hash code for media
                    if (postDetails.IsChangeHashOfMedia)
                        mediaFilePath = MediaUtilites.CalculateMD5Hash(mediaFilePath);
                    redditPostDetails.Kind = MediaUtilites.GetMimeTypeByFilePath(mediaFilePath);
                    // For creating video thumbnail
                    if (redditPostDetails.Kind == "video/mp4")
                    {
                        var objMediaUtilites = new MediaUtilites();
                        var thumbnailFilePath = objMediaUtilites.GetThumbnailPng(mediaFilePath);
                        redditPostDetails.ThumbnailKind = MediaUtilites.GetMimeTypeByFilePath(thumbnailFilePath);
                        var responseThumbnail = redditFunction.GetUploadedMediaId(AccountModel, accountParameters,
                            thumbnailFilePath, redditPostDetails.ThumbnailKind);
                        redditFunction.UploadImageAndGetMediaId(responseThumbnail, postDetails, AccountModel,
                            redditPostDetails.ThumbnailKind, thumbnailFilePath);
                        redditPostDetails.ThumbnailUrl = responseThumbnail.UploadedImageUrl;
                    }
                    else
                    {
                        var response = redditFunction.GetUploadedMediaId(AccountModel, accountParameters, mediaFilePath,
                        redditPostDetails.Kind);
                        redditFunction.UploadImageAndGetMediaId(response, postDetailsClone, AccountModel,
                            redditPostDetails.Kind);
                        redditPostDetails.Url = response.UploadedImageUrl;
                    }

                    if (redditPostDetails.Url != null) break;
                }
            }
        }

        private void ApplyGeneralSetting(PublisherPostlistModel postDetails, PublishPostModel redditPostDetails)
        {
            var settings = postDetails.RedditPostSetting;
            redditPostDetails.Sendreplies = settings.IsDisableSendingReplies || RedditModel.IsDisableSendingSubOptions && RedditModel.IsDisplaySendingReplies;

            redditPostDetails.Nsfw = settings.IsNsfw || RedditModel.IsNsfwSubOptions && RedditModel.IsMarkAsNsfw;

            redditPostDetails.OriginalContent = settings.IsOriginalContent || RedditModel.IsOriginalContentSubOptions && RedditModel.IsMarkAsOriginalContent;

            redditPostDetails.Spoiler = settings.IsSpoiler || RedditModel.IsSpoilerSubOptions && RedditModel.IsMarkAsSpoiler;

            redditPostDetails.Text = postDetails.PostDescription;

            //redditPostDetails.Kind = postDetails.MediaList.Count > 0 ? "video" : string.IsNullOrEmpty(postDetails.PdSourceUrl) ? "self" : "link";

            redditPostDetails.Url = string.IsNullOrEmpty(postDetails.PdSourceUrl) ? string.Empty : postDetails.PdSourceUrl;

            redditPostDetails.Title = postDetails.PublisherInstagramTitle;
        }

        private void GetAccountParameters(PaginationParameter accountParameters)
        {
            _redditLogInProcess.CheckLogin(AccountModel, CampaignCancellationToken.Token);
            if (!AccountModel.IsUserLoggedIn) return;

            // Initialize Header Parameters
            new RequestParameter(_httpHelper);
            var url = "https://www.reddit.com";
            var response = _httpHelper.GetRequest(url);
            var json2 = RdConstants.GetJsonPageResponse(response.Response);
            try
            {
                if (string.IsNullOrEmpty(json2))
                {
                    var url1 = $"https://www.reddit.com/user/{AccountModel.UserName}";
                    var responseNew = _httpHelper.GetRequest(url1);

                    var jsonNew = Utilities.GetBetween(responseNew.Response, "window.___r =",
                        ",\"savedScrollPositions\":{}}}</script>");
                    jsonNew = jsonNew + ",\"savedScrollPositions\":{}}}";
                    var jsonobject1 = JObject.Parse(jsonNew);
                    accountParameters.SessionTracker = jsonobject1["user"]["sessionTracker"].ToString();
                    accountParameters.Reddaid = jsonobject1["user"]["reddaid"]?.ToString();
                    var loidLoid = jsonobject1["user"]["loid"]["loid"].ToString();
                    var loidBlob = jsonobject1["user"]["loid"]["blob"].ToString();
                    var loidCreated = jsonobject1["user"]["loid"]["loidCreated"].ToString();
                    var loidVersion = jsonobject1["user"]["loid"]["version"].ToString();
                    accountParameters.Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
                    accountParameters.AccessToken = jsonobject1["user"]["session"]["accessToken"].ToString();
                }
                else
                {
                    var jsonobject1 = JObject.Parse(json2);
                    accountParameters.SessionTracker = jsonobject1["user"]["sessionTracker"].ToString();
                    accountParameters.Reddaid = jsonobject1["user"]["reddaid"]?.ToString();
                    var loidLoid = jsonobject1["user"]["loid"]["loid"].ToString();
                    var loidBlob = jsonobject1["user"]["loid"]["blob"].ToString();
                    var loidCreated = jsonobject1["user"]["loid"]["loidCreated"].ToString();
                    var loidVersion = jsonobject1["user"]["loid"]["version"].ToString();
                    accountParameters.Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
                    accountParameters.AccessToken = jsonobject1["user"]["session"]["accessToken"].ToString();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails,
            bool isDelayNeeded = true)
        {
            var MediaType = string.Empty;
            var publishSuccess = false;
            try
            {
                var updatedPostDetails = PerformGeneralSettings(postDetails);
                updatedPostDetails = PerformRedditPostSetting(updatedPostDetails, RedditModel);
                if (updatedPostDetails.MediaList.Count > 0)
                    MediaType = MediaUtilites.GetMimeTypeByFilePath(updatedPostDetails.MediaList.FirstOrDefault());
                ReplaceFileNameAsDescription(updatedPostDetails, postDetails);
                if (!AccountModel.IsRunProcessThroughBrowser && !MediaType.Contains("video"))
                {
                    PublishPostResponseHandler finalResponse = null;
                    var redditFunction =
                        _accountScopeFactory[accountId].Resolve<IRedditFunction>();
                    var accountParameters = new PaginationParameter();
                    RedditPostResponseHandler redditPostResponseHandler = null;
                    Task.Factory.StartNew(() =>
                    {

                        if (!FilterMatched(updatedPostDetails))
                            return;
                        var mediaPath = string.Empty;
                        var addMissingExtension = new ObservableCollection<string>();

                        foreach (var media in updatedPostDetails.MediaList)
                        {
                            if (!string.IsNullOrEmpty(media))
                            {
                                FileInfo fi = new FileInfo(media);
                                var extension = fi.Extension;
                                if (string.IsNullOrWhiteSpace(extension))
                                {
                                    mediaPath = Path.ChangeExtension(media, ".jpeg");
                                    File.Move(media, mediaPath);
                                    addMissingExtension.Add(mediaPath);
                                }
                                else
                                    addMissingExtension.Add(media);
                            }
                        }

                        if (addMissingExtension != null)
                            updatedPostDetails.MediaList = addMissingExtension;
                        GetAccountParameters(accountParameters);
                        redditFunction.GetSubmitPage(AccountModel, accountParameters);
                        var redditPostDetails = new PublishPostModel();

                        //To share post on wall from subreddit/user url
                        if (string.Empty != updatedPostDetails.ShareUrl && postDetails.PostSource == PostSource.SharePost)
                        {
                            redditPostResponseHandler = redditFunction.ScrapePostsByUrl(AccountModel,
                                updatedPostDetails.ShareUrl, QueryInfo.NoQuery, redditPostResponseHandler);
                            ApplyPostSettings(updatedPostDetails, redditPostDetails, redditFunction, accountParameters);
                            if (redditPostResponseHandler.LstRedditPost.Count > 0)
                            {
                                foreach (var redditPost in redditPostResponseHandler.LstRedditPost)
                                {
                                    redditPostDetails.CrosspostFullname = redditPost.PostId;
                                    redditPostDetails.ApiType = "json";
                                    redditPostDetails.Title = redditPost.Title;
                                    redditPostDetails.SubmitType = "profile";
                                    redditPostDetails.ValidateOnSubmit = true;
                                    redditPostDetails.Sr = $"u_{AccountModel.UserName}";
                                    redditPostDetails.Url = updatedPostDetails.ShareUrl;

                                    finalResponse = redditFunction.PublishPostCrossPost(AccountModel, redditPostDetails,
                                        accountParameters);
                                    if (finalResponse.Response.Contains("INVALID_CROSSPOST_THING"))
                                        continue;
                                    SaveToDbAfterPublishPost(finalResponse, accountParameters, updatedPostDetails,
                                        postDetails);
                                }
                            }
                            //To share post on wall from posturl
                            else
                            {
                                var jsonResponse = RdConstants.GetJsonPageResponse(redditPostResponseHandler.Response);
                                var jsonObject = JObject.Parse(jsonResponse);
                                updatedPostDetails.MediaList.Add(
                                    jsonObject["posts"]["models"].First().First()["media"]["content"].ToString());
                                redditPostDetails.CrosspostFullname =
                                    jsonObject["posts"]["models"].First().First()["id"].ToString();
                                redditPostDetails.ApiType = "json";
                                redditPostDetails.Title = jsonObject["posts"]["models"].FirstOrDefault().First()["title"]
                                    .ToString();
                                redditPostDetails.SubmitType = "profile";
                                redditPostDetails.ValidateOnSubmit = true;
                                redditPostDetails.Sr = $"u_{AccountModel.UserName}";
                                redditPostDetails.Url = updatedPostDetails.ShareUrl;

                                finalResponse = redditFunction.PublishPostCrossPost(AccountModel, redditPostDetails,
                                    accountParameters);
                                SaveToDbAfterPublishPost(finalResponse, accountParameters, updatedPostDetails, postDetails);
                            }
                        }
                        else
                        {
                            //var MediaUploadResponse = PublishMedia(AccountModel.AccountId, updatedPostDetails, string.Empty, false);
                            var MediaUploadResponse = PublishPostAndMedia(AccountModel.AccountId, updatedPostDetails, string.Empty, false, accountParameters);
                            finalResponse = new PublishPostResponseHandler(new DominatorHouseCore.Request.ResponseParameter()) { PostUrl = MediaUploadResponse.Item2, Success = MediaUploadResponse.Item1, FailureMessage = MediaUploadResponse.Item3 };
                            SaveToDbAfterPublishPost(finalResponse, accountParameters, updatedPostDetails, postDetails);
                            //For deleting published scraped post
                            if (postDetails.PostSource == PostSource.ScrapedPost)
                            {
                                var downloadedScrapedPostPath =
                                    $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Socinator\\RdScrapeFiles\\{postDetails.FetchedPostIdOrUrl}";

                                if (postDetails.RedditScrapedMediaType.Equals("image"))
                                    downloadedScrapedPostPath = downloadedScrapedPostPath + ".mp4";
                                else
                                    downloadedScrapedPostPath = downloadedScrapedPostPath + ".jpg";

                                if (File.Exists(downloadedScrapedPostPath))
                                    File.Delete(downloadedScrapedPostPath);
                            }
                        }
                    }).Wait();
                    return finalResponse != null && finalResponse.Success;
                }
                //For browser automation
                var PostUrl = string.Empty;
                Task.Factory.StartNew(() =>
                {
                    var mediaPath = string.Empty;
                    var ErrorMessage = string.Empty;
                    if (postDetails.PostSource == PostSource.ScrapedPost)
                        updatedPostDetails.PostDescription = postDetails.PostDescription;
                    var addMissingExtension = new ObservableCollection<string>();

                    foreach (var media in updatedPostDetails.MediaList)
                    {
                        if (!media.Contains("https://") && !media.Contains("http://"))
                        {
                            if (!string.IsNullOrEmpty(media))
                            {
                                FileInfo fi = new FileInfo(media);
                                var extension = fi.Extension;
                                if (string.IsNullOrWhiteSpace(extension))
                                {
                                    mediaPath = Path.ChangeExtension(media, ".jpeg");
                                    File.Move(media, mediaPath);
                                    addMissingExtension.Add(mediaPath);
                                }
                                else
                                    addMissingExtension.Add(media);
                            }
                        }
                    }

                    if (addMissingExtension != null && addMissingExtension.Count != 0)
                        updatedPostDetails.MediaList = addMissingExtension;
                    var settings = postDetails.RedditPostSetting;

                    updatedPostDetails.RedditPostSetting.IsDisableSendingReplies = settings.IsDisableSendingReplies ||
                                                    RedditModel.IsDisableSendingSubOptions &&
                                                    RedditModel.IsDisplaySendingReplies;

                    updatedPostDetails.RedditPostSetting.IsNsfw = settings.IsNsfw ||
                                             RedditModel.IsNsfwSubOptions && RedditModel.IsMarkAsNsfw;

                    updatedPostDetails.RedditPostSetting.IsOriginalContent = settings.IsOriginalContent ||
                                                        RedditModel.IsOriginalContentSubOptions &&
                                                        RedditModel.IsMarkAsOriginalContent;

                    updatedPostDetails.RedditPostSetting.IsSpoiler = settings.IsSpoiler ||
                                                RedditModel.IsSpoilerSubOptions && RedditModel.IsMarkAsSpoiler;
                    _redditLogInProcess.LoginWithBrowserMethod(AccountModel, CampaignCancellationToken.Token);
                    if (!AccountModel.IsUserLoggedIn || !FilterMatched(updatedPostDetails)) return;
                    if (string.IsNullOrEmpty(MediaType) ? false : MediaType.Contains("video"))
                    {
                        var VideoPublishResponse = PublishPostAndMedia(accountId, updatedPostDetails);
                        publishSuccess = VideoPublishResponse.Item1;
                        PostUrl = VideoPublishResponse.Item2.Contains("reddit.com") ? VideoPublishResponse.Item2 : RdConstants.NewRedditHomePageAPI + VideoPublishResponse.Item2;
                        ErrorMessage = VideoPublishResponse.Item3;
                    }
                    else
                        publishSuccess = _browserManager.Publisher(AccountModel, updatedPostDetails, AccountModel.UserName, OtherConfiguration, out ErrorMessage);
                    if (publishSuccess)
                    {
                        var postUrl = string.IsNullOrEmpty(PostUrl) ? _browserManager.BrowserWindow.CurrentUrl() : PostUrl;
                        updatedPostDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                        {
                            AccountName = AccountModel.UserName,
                            DestinationUrl = AccountModel.UserName,
                            Destination = "OwnWall",
                            Description = updatedPostDetails.PostDescription,
                            IsPublished = publishSuccess ? "Yes" : "No",
                            Successful = publishSuccess ? "Yes" : "No",
                            Link = postUrl,
                            PublishedDate = DateTime.Now,
                            SocialNetworks = SocialNetworks.Reddit
                        });
                        UpdatePostWithSuccessful(AccountModel.UserName, postDetails, postUrl);
                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Ownwall", AccountModel.AccountBaseModel.UserName);
                    }
                    else
                    {
                        UpdatePostWithFailed(AccountModel.UserName, postDetails, "Not Publish");
                        GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Ownwall", AccountModel.AccountBaseModel.UserName + $" {ErrorMessage}");
                    }
                    _browserManager.CloseBrowser();
                }).Wait();
                if (isDelayNeeded)
                    DelayBeforeNextPublish();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return publishSuccess;
        }

        private Tuple<bool, string, string> PublishPostAndMedia(string accountId, PublisherPostlistModel postDetails, string GroupName = "", bool IsVideo = true, PaginationParameter accountParameters = null)
        {
            var IsSuccess = false;
            var PostUrl = string.Empty;
            var ErrorMessage = string.Empty;
            var IsGroupPublish = !string.IsNullOrEmpty(GroupName);
            try
            {
                var redditFunction = _accountScopeFactory[accountId].Resolve<IRedditFunction>();
                var redditPostDetails = new PublishPostModel();
                var File = postDetails.MediaList.Count > 0 ? postDetails.MediaList.FirstOrDefault() : string.Empty;
                ApplyGeneralSetting(postDetails, redditPostDetails);
                var MediaUploadResponse = string.Empty;
                var NotAMedia = false;
                FileInfo fileInfo = null;
                if (!string.IsNullOrEmpty(File))
                {
                    fileInfo = new FileInfo(File);
                    var UploadParameter = redditFunction.GetUploadedMediaId(AccountModel, accountParameters, IsVideo ? File : fileInfo != null ? fileInfo.Name : string.Empty, IsVideo ? string.Empty : "image%2Fjpeg");
                    var MediaUploadPostData = Utils.GeneratePostDataForMediaUpload(postDetails.MediaList, _httpHelper.GetRequestParameter(), UploadParameter);
                    //Uploading Media.
                    MediaUploadResponse = UploadAndGetResponse(MediaUploadPostData, IsVideo);
                }
                else
                    NotAMedia = true;
                var postSetting = postDetails.RedditPostSetting;
                var FinalUploadResponse = string.Empty;
                if (IsGroupPublish)
                {
                    Dictionary<string, bool> allowedPostTypes = new Dictionary<string, bool>();
                    var ListFlairs = new List<RedditPostFlairInfo>();
                    var IsAllowedToPost = IsAllowedPostingOnGroup(GroupName, out allowedPostTypes, ListFlairs);
                    if (IsAllowedToPost)
                    {
                        var flair = ListFlairs.Count > 0 ? IsVideo ? ListFlairs.Any(y => y.FlairTitle.Contains("Video")) ? ListFlairs.FirstOrDefault(x => x.FlairTitle.Contains("Video")) : ListFlairs.GetRandomItem() : ListFlairs.GetRandomItem() : new RedditPostFlairInfo();
                        var MediaId = Utils.GetBetween(MediaUploadResponse, "<Key>", "</Key>");
                        var token = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                        if (NotAMedia && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) && string.IsNullOrEmpty(postDetails.PdSourceUrl))
                        {
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetGroupPostData1($"{token}",MediaId,postSetting,GroupName,Title:postDetails.PublisherInstagramTitle,description:postDetails.PostDescription,flairId:flair.Id,flairText:flair.FlairTitle,NotAMedia:NotAMedia,IsVideo:IsVideo))).Response;
                        }
                        else if(IsVideo && allowedPostTypes["videos"])
                        {
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetGroupPostData1($"{token}", MediaId, postSetting, GroupName, Title: postDetails.PublisherInstagramTitle, description: postDetails.PostDescription, flairId: flair.Id, flairText: flair.FlairTitle, NotAMedia: NotAMedia, IsVideo: IsVideo))).Response;
                        }
                        else if(!string.IsNullOrEmpty(postDetails.PdSourceUrl) && allowedPostTypes["links"])
                        {
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetGroupPostData1($"{token}", MediaId, postSetting, GroupName, Title: postDetails.PublisherInstagramTitle, description: postDetails.PostDescription,Link:postDetails.PdSourceUrl, flairId: flair.Id, flairText: flair.FlairTitle, NotAMedia: NotAMedia, IsVideo: IsVideo))).Response;
                        }
                        else if(!IsVideo && allowedPostTypes["images"])
                        {
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetGroupPostData1($"{token}", MediaId, postSetting, GroupName, Title: postDetails.PublisherInstagramTitle, description: postDetails.PostDescription, flairId: flair.Id, flairText: flair.FlairTitle, NotAMedia: NotAMedia, IsVideo: IsVideo))).Response;
                        }
                        else
                        {
                            IsSuccess = false;
                            PostUrl = string.Empty;
                            ErrorMessage = $"Given Post Is Not Allowed In {GroupName}";
                        }
                        PostUrl = Utils.GetBetween(FinalUploadResponse, "\"permalink\":\"", "\"");
                        PostUrl = PostUrl.Contains("reddit.com") ? PostUrl : RdConstants.NewRedditHomePageAPI + PostUrl;
                        var PostID = Utils.GetBetween(FinalUploadResponse, "\"id\":\"", "\"");
                        IsSuccess = !string.IsNullOrEmpty(PostUrl) && !string.IsNullOrEmpty(PostID);
                    }
                    else
                    {
                        IsSuccess = false;
                        PostUrl = string.Empty;
                        ErrorMessage = $"Given Post Is Not Allowed In {GroupName}";
                    }
                }
                else
                {
                    var MediaId = Utils.GetBetween(MediaUploadResponse, "<Key>", "</Key>");
                    var MediaUrl = Utils.GetBetween(MediaUploadResponse, "<Location>", "</Location>");
                    var token = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                    FinalUploadResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetFinalSubmitPostData1($"{token}", MediaId, string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) ? fileInfo != null ? fileInfo.Name : string.Empty : postDetails.PublisherInstagramTitle, postDetails.PostDescription, postSetting.IsSpoiler, postSetting.IsNsfw, false, !postSetting.IsDisableSendingReplies, "self", "profile", IsVideo, string.Empty, string.Empty, NotAMedia, postDetails.PdSourceUrl, ImageUrl: MediaUrl))).Response;
                    PostUrl = Utils.GetBetween(FinalUploadResponse, "\"permalink\":\"", "\"");
                    PostUrl = PostUrl.Contains("reddit.com") ? PostUrl : RdConstants.NewRedditHomePageAPI + PostUrl;
                    var PostID = Utils.GetBetween(FinalUploadResponse, "\"id\":\"", "\"");
                    if (postSetting.IsOriginalContent && !string.IsNullOrEmpty(PostID))
                    {
                        _httpHelper.GetRequestParameter().ContentType = RdConstants.ContentType;
                        var PostResponse = _httpHelper.PostRequest(RdConstants.MarkAsOriginalContentAPI, RdConstants.MarkAsOriginalContentPostData(PostID)).Response;
                    }
                    //IsSuccess = !string.IsNullOrEmpty(PostUrl) && ApprovedMediaPost(PostID);
                    IsSuccess = FinalUploadResponse != null ? FinalUploadResponse.Contains("ok\":true"):false;
                }
                if (!IsSuccess)
                {
                    var jsonHandler = JsonJArrayHandler.GetInstance;
                    ErrorMessage = jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(FinalUploadResponse), "data", "createProfilePost", "fieldErrors",0,"message");
                    ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(FinalUploadResponse), "data", "createPost", "fieldErrors", 0, "message") : ErrorMessage;
                }
            }
            catch (Exception)
            {
            }
            return new Tuple<bool, string, string>(IsSuccess, PostUrl, ErrorMessage);
        }

        private bool IsAllowedPostingOnGroup(string groupName, out Dictionary<string, bool> allowedPostTypes, List<RedditPostFlairInfo> listFlairs)
        {
            allowedPostTypes = new Dictionary<string, bool>();
            var htmlDoc = new HtmlDocument();
            try
            {
                var response1 = _httpHelper.GetRequest(RdConstants.GetGroupDetailsApi(groupName)).Response;
                var response = RdConstants.GetJsonPageResponse(response1);
                if (string.IsNullOrEmpty(response))
                {
                    htmlDoc.LoadHtml(response1);
                    var postTypeSelectNode = htmlDoc.DocumentNode.SelectSingleNode("//r-post-type-select");
                    var postFlairNode= htmlDoc.DocumentNode.SelectSingleNode("//r-post-flairs-modal");
                    if (postFlairNode != null)
                    {
                        foreach (var dataNode in postFlairNode.SelectNodes(".//data"))
                        {
                            var flairId = dataNode.GetAttributeValue("data-id", "N/A");
                            var flairName=dataNode.GetAttributeValue("data-text", "N/A");
                            listFlairs.Add(new RedditPostFlairInfo()
                            {
                                Id = flairId,
                                FlairTitle = flairName
                            });
                        }
                    }
                    if (postTypeSelectNode != null)
                    {
                        foreach (var dataNode in postTypeSelectNode.SelectNodes(".//data"))
                        {
                            var value = dataNode.GetAttributeValue("value", "N/A");
                            bool.TryParse(dataNode.GetAttributeValue("data-disabled", "false"), out bool disabled);
                            var text = dataNode.InnerText.Trim().ToLower();
                            if (text.Contains("text"))
                                allowedPostTypes.Add("text", !disabled);
                            if (text.Contains("image"))
                                allowedPostTypes.Add("images", !disabled);
                            if (text.Contains("video"))
                                allowedPostTypes.Add("videos", !disabled);
                            if (text.Contains("link"))
                                allowedPostTypes.Add("links", !disabled);
                        }
                        return allowedPostTypes.Count > 0 ? true : false;
                    }
                    else
                        return false;
                }
                else
                {
                    var jsonHandler = JsonJArrayHandler.GetInstance;
                    var jobject = jsonHandler.ParseJsonToJObject(response);
                    var allowedPostTypeData = jsonHandler.GetJTokenValue(jobject, "subreddits", "about", 0, "allowedPostTypes");
                    if (!string.IsNullOrEmpty(allowedPostTypeData))
                    {
                        bool.TryParse(jsonHandler.GetJTokenValue(allowedPostTypeData, "images"), out bool isAllowImage);
                        bool.TryParse(jsonHandler.GetJTokenValue(allowedPostTypeData, "videos"), out bool isAllowVideo);
                        bool.TryParse(jsonHandler.GetJTokenValue(allowedPostTypeData, "text"), out bool isAllowText);
                        bool.TryParse(jsonHandler.GetJTokenValue(allowedPostTypeData, "links"), out bool isAllowlinks);
                        allowedPostTypes.Add("images", isAllowImage);
                        allowedPostTypes.Add("videos", isAllowVideo);
                        allowedPostTypes.Add("text", isAllowText);
                        allowedPostTypes.Add("links", isAllowlinks);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        private void ReplaceFileNameAsDescription(PublisherPostlistModel updatedPostDetails, PublisherPostlistModel postDetails)
        {
            if (updatedPostDetails.MediaList.Count > 0 && ((updatedPostDetails.scrapePostModel.IsScrapeGoogleImgaes && updatedPostDetails.scrapePostModel.IsUseFileNameAsDescription) || (updatedPostDetails.PostDetailModel.IsUploadMultipleImage && updatedPostDetails.PostDetailModel.IsUseFileNameAsDescription)))
                updatedPostDetails.PostDescription = new FileInfo(updatedPostDetails.MediaList.FirstOrDefault()).Name;
            else
                updatedPostDetails.PostDescription = postDetails.PostDescription;
            //Assign FileName As Post Title If Title Is Empty.
            if (string.IsNullOrEmpty(updatedPostDetails.PublisherInstagramTitle) && updatedPostDetails.MediaList.Count > 0)
                updatedPostDetails.PublisherInstagramTitle = new FileInfo(updatedPostDetails.MediaList.FirstOrDefault()).Name;
        }

        private Tuple<bool, string, string> PublishMedia(string accountId, PublisherPostlistModel postDetails, string GroupName = "", bool IsVideo = true)
        {
            var IsSuccess = false;
            var PostUrl = string.Empty;
            var ErrorMessage = string.Empty;
            var IsGroupPublish = !string.IsNullOrEmpty(GroupName);
            try
            {
                var redditFunction = _accountScopeFactory[accountId].Resolve<IRedditFunction>();
                var accountParameters = new PaginationParameter();
                GetAccountParameters(accountParameters);
                redditFunction.GetSubmitPage(AccountModel, accountParameters);
                var redditPostDetails = new PublishPostModel();
                var File = postDetails.MediaList.Count > 0 ? postDetails.MediaList.FirstOrDefault() : string.Empty;
                ApplyGeneralSetting(postDetails, redditPostDetails);
                var MediaUploadResponse = string.Empty;
                var NotAMedia = false;
                FileInfo fileInfo = null;
                if (!string.IsNullOrEmpty(File))
                {
                    fileInfo = new FileInfo(File);
                    var UploadParameter = redditFunction.GetUploadedMediaId(AccountModel, accountParameters, IsVideo ? File : fileInfo != null ? fileInfo.Name : string.Empty, IsVideo ? string.Empty : "image%2Fjpeg");
                    var MediaUploadPostData = Utils.GeneratePostDataForMediaUpload(postDetails.MediaList, _httpHelper.GetRequestParameter(), UploadParameter);
                    //Uploading Media.
                    MediaUploadResponse = UploadAndGetResponse(MediaUploadPostData, IsVideo);
                }
                else
                    NotAMedia = true;
                var postSetting = postDetails.RedditPostSetting;
                if (IsGroupPublish)
                {
                    var AllowedPostType = string.Empty;
                    var ListFlairs = new List<RedditPostFlairInfo>();
                    var IsAllowedToPost = IsAllowedPosting(GroupName, out AllowedPostType, postDetails, ListFlairs);
                    var FinalUploadResponse = string.Empty;
                    if (IsAllowedToPost)
                    {
                        var flair = ListFlairs.Count > 0 ? IsVideo ? ListFlairs.Any(y => y.FlairTitle.Contains("Video")) ? ListFlairs.FirstOrDefault(x => x.FlairTitle.Contains("Video")) : ListFlairs.GetRandomItem() : ListFlairs.GetRandomItem() : new RedditPostFlairInfo();
                        if (flair != null && !string.IsNullOrEmpty(flair.FlairTitle) && !string.IsNullOrEmpty(flair.Id))
                            ApplyFlairOnPost(flair, GroupName);
                        if (AllowedPostType.Contains("posts"))
                        {
                            var MediaId = Utils.GetBetween(MediaUploadResponse, "<Key>", "</Key>");
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.CrossPostPublishUrl, RdConstants.GetFinalSubmitPostData($"{GroupName.ToLower()}", MediaId, string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) ? fileInfo != null ? fileInfo.Name : string.Empty : postDetails.PublisherInstagramTitle, postDetails.PostDescription, postSetting.IsSpoiler, postSetting.IsNsfw, postSetting.IsOriginalContent, !postSetting.IsDisableSendingReplies, "self", "subreddit", IsVideo, flair.Id, flair.FlairTitle, NotAMedia, postDetails.PdSourceUrl)).Response;
                            PostUrl = Utils.GetBetween(FinalUploadResponse, "\"url\": \"", "\", \"");
                        }
                        else if (AllowedPostType.Contains("images") && !IsVideo)
                        {
                            var PostUploadedUrl = Utils.GetBetween(MediaUploadResponse, "<Location>", "</Location>");
                            var post = RdConstants.GetGroupPostData(GroupName, postSetting, postDetails.PublisherInstagramTitle, false, string.Empty, PostUploadedUrl, "image", string.Empty, true, flair.Id, flair.FlairTitle);
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.CrossPostPublishUrl, Encoding.UTF8.GetBytes(post)).Response;
                            var postId = redditFunction.GetLatestPostUrlWithGateWay(accountParameters, AccountModel);
                            PostUrl = $"https://www.reddit.com/r/{GroupName}/comments/{postId}/{postDetails.PublisherInstagramTitle.ToLower()?.Replace(" ", "_")?.Replace("?", "")}/";
                        }
                        else if (AllowedPostType.Contains("videos") && IsVideo)
                        {
                            var VideoUploadedUrl = Utils.GetBetween(MediaUploadResponse, "<Location>", "</Location>");
                            var ThumbnailFilePath = new MediaUtilites().GetThumbnailPng(postDetails.MediaList.FirstOrDefault());
                            fileInfo = new FileInfo(ThumbnailFilePath);
                            var UploadParameter = redditFunction.GetUploadedMediaId(AccountModel, accountParameters, fileInfo != null ? fileInfo.Name : string.Empty, "image%2Fjpeg");
                            var MediaUploadPostData = Utils.GeneratePostDataForMediaUpload(postDetails.MediaList, _httpHelper.GetRequestParameter(), UploadParameter);
                            var ThumbNailUploadResponse = UploadAndGetResponse(MediaUploadPostData, false);
                            var ThumbNailUrl = Utils.GetBetween(ThumbNailUploadResponse, "<Location>", "</Location>");
                            var post = RdConstants.GetGroupPostData(GroupName, postSetting, postDetails.PublisherInstagramTitle, IsVideo, VideoUploadedUrl, ThumbNailUrl, "video", string.Empty, false, flair.Id, flair.FlairTitle);
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.CrossPostPublishUrl, Encoding.UTF8.GetBytes(post)).Response;
                            var postId = redditFunction.GetLatestPostUrlWithGateWay(accountParameters, AccountModel);
                            PostUrl = $"https://www.reddit.com/r/{GroupName}/comments/{postId}/{postDetails.PublisherInstagramTitle.ToLower()?.Replace(" ", "_")?.Replace("?", "")}/";
                        }
                        else if (AllowedPostType.Contains("links"))
                        {
                            var post = RdConstants.GetGroupPostData(GroupName, postSetting, postDetails.PublisherInstagramTitle, false, string.Empty, string.Empty, "link", postDetails.PdSourceUrl, false, flair.Id, flair.FlairTitle);
                            FinalUploadResponse = _httpHelper.PostRequest(RdConstants.CrossPostPublishUrl, Encoding.UTF8.GetBytes(post)).Response;
                            PostUrl = Utils.GetBetween(FinalUploadResponse, "\"url\": \"", "\"");
                        }
                        else
                        {
                            IsSuccess = false;
                            PostUrl = string.Empty;
                            ErrorMessage = $"Given Post Is Not Allowed In {GroupName}";
                        }
                        IsSuccess = !string.IsNullOrEmpty(PostUrl);
                        if (!IsSuccess)
                        {
                            var jsonHandler = JsonJArrayHandler.GetInstance;
                            ErrorMessage = jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(FinalUploadResponse), "json", "errors", 1, 1);
                            ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(FinalUploadResponse), "json", "errors", 0, 1) : ErrorMessage;
                        }
                    }
                    else
                    {
                        IsSuccess = false;
                        PostUrl = string.Empty;
                        ErrorMessage = string.IsNullOrEmpty(AllowedPostType) ? $"Given Post Is Not Allowed In {GroupName}" : $"Posting Is Not Allowed In {GroupName}";
                    }
                }
                else
                {
                    var MediaId = Utils.GetBetween(MediaUploadResponse, "<Key>", "</Key>");
                    var FinalUploadResponse = _httpHelper.PostRequest(RdConstants.CrossPostPublishUrl, RdConstants.GetFinalSubmitPostData($"u_{AccountModel.AccountBaseModel.UserName}", MediaId, string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) ? fileInfo != null ? fileInfo.Name : string.Empty : postDetails.PublisherInstagramTitle, postDetails.PostDescription, postSetting.IsSpoiler, postSetting.IsNsfw, false, !postSetting.IsDisableSendingReplies, "self", "profile", IsVideo, string.Empty, string.Empty, NotAMedia, postDetails.PdSourceUrl)).Response;
                    PostUrl = Utils.GetBetween(FinalUploadResponse, "\"url\": \"", "\", \"");
                    var PostID = Utils.GetBetween(FinalUploadResponse, "\"name\": \"", "\"");
                    if (postSetting.IsOriginalContent)
                    {
                        _httpHelper.GetRequestParameter().ContentType = RdConstants.ContentType;
                        var PostResponse = _httpHelper.PostRequest(RdConstants.MarkAsOriginalContentAPI, RdConstants.MarkAsOriginalContentPostData(PostID)).Response;
                    }
                    IsSuccess = !string.IsNullOrEmpty(PostUrl) && ApprovedMediaPost(PostID);
                    if (!IsSuccess)
                    {
                        var jsonHandler = JsonJArrayHandler.GetInstance;
                        ErrorMessage = jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(FinalUploadResponse), "json", "errors", 1, 1);
                        ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(FinalUploadResponse), "json", "errors", 0, 1) : ErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new Tuple<bool, string, string>(IsSuccess, PostUrl, ErrorMessage);
        }

        private void ApplyFlairOnPost(RedditPostFlairInfo flair, string GroupName)
        {
            var post = $"sr={GroupName}&field=flair&kind=self&title=&url&flair_id={flair.Id}&show_error_list=true";
            var response = _httpHelper.PostRequest("https://oauth.reddit.com/api/validate_submission_field?raw_json=1&gilding_detail=1", Encoding.UTF8.GetBytes(post)).Response;
        }

        private bool IsAllowedPosting(string groupName, out string AllowedPostType, PublisherPostlistModel postDetails, List<RedditPostFlairInfo> listFlairs)
        {
            var IsAllowToPost = false;
            AllowedPostType = string.Empty;
            try
            {
                var SubRedditInfoResponse = _httpHelper.GetRequest(RdConstants.GetSubRedditInfoAPI(groupName)).Response;
                var jsonHandler = JsonJArrayHandler.GetInstance;
                var jsonObject = jsonHandler.ParseJsonToJObject(SubRedditInfoResponse);
                var token = jsonHandler.GetJTokenOfJToken(jsonObject, "subredditAboutInfo")?.First;
                var subRedditInfo = jsonHandler.GetJTokenOfJToken(jsonObject, "subredditAboutInfo", token?.Path?.Split('.')?.LastOrDefault()?.ToString(), "allowedPostTypes");
                bool.TryParse(jsonHandler.GetJTokenValue(subRedditInfo, "links"), out bool links);
                bool.TryParse(jsonHandler.GetJTokenValue(subRedditInfo, "images"), out bool images);
                bool.TryParse(jsonHandler.GetJTokenValue(subRedditInfo, "videos"), out bool videos);
                bool.TryParse(jsonHandler.GetJTokenValue(subRedditInfo, "text"), out bool posts);
                bool.TryParse(jsonHandler.GetJTokenValue(subRedditInfo, "polls"), out bool polls);
                AllowedPostType = posts && (!string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) || !string.IsNullOrEmpty(postDetails.PostDescription) || postDetails.MediaList.Count > 0) ? "posts" : images && videos && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) && postDetails.MediaList.Count > 0 ? "images+videos" : images && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) && postDetails.MediaList.Count > 0 ? "images" : videos && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) && postDetails.MediaList.Count > 0 ? "videos" : links && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) && !string.IsNullOrEmpty(postDetails.PdSourceUrl) ? "links" : polls && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) && !string.IsNullOrEmpty(postDetails.PostDescription) ? "polls" : string.Empty;
                IsAllowToPost = !string.IsNullOrEmpty(AllowedPostType);
                var FlairToken = jsonHandler.GetJTokenOfJToken(jsonObject, "postFlair")?.First;
                var FlairInfo = jsonHandler.GetJTokenOfJToken(jsonObject, "postFlair", FlairToken?.Path?.Split('.')?.LastOrDefault()?.ToString());
                bool.TryParse(jsonHandler.GetJTokenValue(FlairInfo, "displaySettings", "isEnabled"), out bool isFlairEnabled);
                bool.TryParse(jsonHandler.GetJTokenValue(FlairInfo, "permissions", "canAssignOwn"), out bool canAssignOwn);
                jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(FlairInfo, "templateIds"))?.ForEach(ID =>
                {
                    var infoToken = jsonHandler.GetJTokenOfJToken(FlairInfo, "templates", ID.ToString());
                    bool.TryParse(jsonHandler.GetJTokenValue(infoToken, "textEditable"), out bool isTextEditable);
                    bool.TryParse(jsonHandler.GetJTokenValue(infoToken, "modOnly"), out bool isModOnly);
                    int.TryParse(jsonHandler.GetJTokenValue(infoToken, "maxEmojis"), out int maxEmojis);
                    listFlairs.Add(new RedditPostFlairInfo
                    {
                        Id = ID.ToString(),
                        FlairTitle = jsonHandler.GetJTokenValue(infoToken, "text"),
                        IsTextEditable = isTextEditable,
                        Type = jsonHandler.GetJTokenValue(infoToken, "type"),
                        IsModOnly = isModOnly,
                        AllowableContent = jsonHandler.GetJTokenValue(infoToken, "allowableContent"),
                        IsFlairEnabled = isFlairEnabled,
                        CanAssignOwn = canAssignOwn,
                        MaxEmojis = maxEmojis
                    });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return IsAllowToPost;
        }

        private string UploadAndGetResponse(byte[] videoUploadPostData, bool IsVideo = true)
        {
            var AuthorizationToken = _httpHelper.GetRequestParameter().Headers["Authorization"];
            if (_httpHelper.GetRequestParameter().Headers.ToString().Contains("Authorization"))
                _httpHelper.GetRequestParameter().Headers.Remove("Authorization");
            var VideoUploadResponse = _httpHelper.PostRequest(RdConstants.GetMediaUploadAPI(IsVideo), videoUploadPostData).Response;
            var requestParameter = _httpHelper.GetRequestParameter();
            requestParameter.Headers["Authorization"] = AuthorizationToken;
            requestParameter.ContentType = "application/json";
            return VideoUploadResponse;
        }

        private bool ApprovedMediaPost(string postID)
        {
            _httpHelper.GetRequestParameter().ContentType = RdConstants.AcceptTypeAds;
            var ApprovedPostResponse = _httpHelper.PostRequest(RdConstants.ApprovePostAPI, RdConstants.ApprovePostData(postID)).Response;
            return !string.IsNullOrEmpty(ApprovedPostResponse) && ApprovedPostResponse.Contains("\"modApprove\":{\"ok\":true");
        }

        private bool IsFilterApplied(DominatorAccountModel accountModel, PublisherPostlistModel updatedPostDetails)
        {
            if (accountModel != null && updatedPostDetails != null)
            {
                if (!string.IsNullOrEmpty(updatedPostDetails.ShareUrl))
                {
                    updatedPostDetails.ShareUrl = Utils.UpdateDomain(updatedPostDetails.ShareUrl);
                    var requestParameter = _redditFunct.SetRequestParametersAndProxy(accountModel);
                    requestParameter.Cookies = accountModel.Cookies;
                    _httpHelper.SetRequestParameter(requestParameter);
                    var PageResponse = _httpHelper.GetRequest(updatedPostDetails.ShareUrl);
                    if (updatedPostDetails.sharePostModel.IsMinimumDays)
                    {
                        long.TryParse(Utilities.GetBetween(PageResponse.Response, "created\":", ","), out long CreatedTimeStamp);
                        var dateTime = CreatedTimeStamp.EpochToDateTimeLocal();
                        var currentDate = DateTime.Now;
                        var ts = currentDate - dateTime;
                        var days = (int)Math.Ceiling(ts.TotalDays);
                        if (days > updatedPostDetails.sharePostModel.MinimumDays)
                            return false;
                    }
                    if (updatedPostDetails.sharePostModel.IsPostBetween)
                    {
                        int.TryParse(Utilities.GetBetween(PageResponse.Response, "score\":", ","), out int VoteCount);
                        if (!updatedPostDetails.sharePostModel.PostBetween.InRange(VoteCount))
                            return false;
                    }
                }
            }
            return true;
        }

        public override bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails,
            bool isDelayed = true)
        {
            return false;
        }

        public override bool PublishOnCustomDestination(string accountId, PublisherCustomDestinationModel customList,
            PublisherPostlistModel postDetails, bool isDelayNeeded = true)
        {
            if (!AccountModel.IsRunProcessThroughBrowser)
            {
                if (string.Compare("Community", customList.DestinationType,
                        StringComparison.CurrentCultureIgnoreCase) != 0) return false;
                PublishPostResponseHandler finalResponse = null;
                var redditFunction =
                    _accountScopeFactory[accountId].Resolve<IRedditFunction>();
                var accountParameters = new PaginationParameter();
                PublisherPostlistModel updatedPostDetails;
                Task.Factory.StartNew(() =>
                {
                    PostInGroup(customList.DestinationValue, postDetails, out finalResponse, redditFunction,
                        accountParameters, out updatedPostDetails);
                    if (finalResponse.Success)
                    {
                        var postedId = redditFunction.GetLatestPostUrlWithGateWay(accountParameters, AccountModel);
                        if (!finalResponse.PostUrl.Contains(postedId))
                            finalResponse.PostUrl = finalResponse.PostUrl + "/comments/" + postedId + "/" +
                                                    updatedPostDetails.PublisherInstagramTitle.ToLower()
                                                        .Replace(" ", "_");

                        updatedPostDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                        {
                            AccountName = AccountModel.UserName,
                            DestinationUrl = customList.DestinationValue,
                            Destination = "Community",
                            Description = updatedPostDetails.PostDescription,
                            IsPublished = finalResponse.Success ? "Yes" : "No",
                            Successful = finalResponse.Success ? "Yes" : "No",
                            Link = finalResponse.PostUrl,
                            PublishedDate = DateTime.Now
                        });
                        UpdatePostWithSuccessful(customList.DestinationValue, postDetails, finalResponse.PostUrl);
                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Community", customList.DestinationValue);
                    }
                    else
                    {
                        UpdatePostWithFailed(customList.DestinationValue, postDetails, finalResponse.FailureMessage);
                        GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Community", customList.DestinationValue);
                    }
                }).Wait();
                if (isDelayNeeded)
                    DelayBeforeNextPublish();

                return finalResponse.Success;
            }
            //For browser automation

            var publishSuccess = false;
            Task.Factory.StartNew(() =>
            {
                _redditLogInProcess.LoginWithBrowserMethod(AccountModel, CampaignCancellationToken.Token);
                if (!AccountModel.IsUserLoggedIn) return;

                var updatedPostDetails = PerformGeneralSettings(postDetails);
                updatedPostDetails = PerformRedditPostSetting(updatedPostDetails, RedditModel);
                var groupUserName = GetGroupUserName(customList.DestinationValue);
                publishSuccess = _browserManager.Publisher(AccountModel, updatedPostDetails, groupUserName, OtherConfiguration, out _);
                if (publishSuccess)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    var afterPublishPageSource = _browserManager.SearchByCustomUrl(AccountModel,
                        $"https://www.reddit.com/user/{AccountModel.UserName}/posts/");
                    var json = RdConstants.GetJsonPageResponse(afterPublishPageSource.Response);
                    var jsonobject = JObject.Parse(json);
                    var post = jsonobject["posts"]["models"]
                        .LastOrDefault(x => x.ToString().Contains(updatedPostDetails.PublisherInstagramTitle))
                        .ToString();
                    var postId = Regex.Split(post, "\":")[0].Replace("\"", "");
                    postId = Utils.GetRedditId(postId);
                    var postUrl = "https://www.reddit.com/user/" + $"{AccountModel.UserName}" + "/comments/" + postId +
                                  "/" + updatedPostDetails.PublisherInstagramTitle.ToLower().Replace(" ", "_");

                    updatedPostDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                    {
                        AccountName = AccountModel.UserName,
                        DestinationUrl = customList.DestinationValue,
                        Destination = "Group",
                        Description = updatedPostDetails.PostDescription,
                        IsPublished = publishSuccess ? "Yes" : "No",
                        Successful = publishSuccess ? "Yes" : "No",
                        Link = postUrl,
                        PublishedDate = DateTime.Now
                    });
                    UpdatePostWithSuccessful(customList.DestinationValue, postDetails, postUrl);
                    GlobusLogHelper.log.Info(Log.PublishingSuccessfully, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Group", customList.DestinationValue);
                    isDelayNeeded = true;
                }
                else
                {
                    UpdatePostWithFailed(customList.DestinationValue, postDetails, "Not Publish");
                    GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Group", customList.DestinationValue);
                }

                _browserManager.CloseBrowser();
            }).Wait();
            if (isDelayNeeded)
                DelayBeforeNextPublish();

            return publishSuccess;
        }
        private void SaveToDbAfterPublishPost(PublishPostResponseHandler finalResponse,
            PaginationParameter accountParameters, PublisherPostlistModel updatedPostDetails,
            PublisherPostlistModel postDetails)
        {
            try
            {
                if (finalResponse.Success)
                {
                    var redditFunction =
                        _accountScopeFactory[AccountModel.AccountId].Resolve<IRedditFunction>();

                    //var postedId = redditFunction.GetLatestPostUrlWithGateWay(accountParameters, AccountModel);
                    //if (!string.IsNullOrEmpty(finalResponse.PostUrl) && !finalResponse.PostUrl.Contains(postedId))
                    //    finalResponse.PostUrl = finalResponse.PostUrl + "/comments/" + postedId + "/" +
                    //                            updatedPostDetails.PublisherInstagramTitle.ToLower().Replace(" ", "_");
                    finalResponse.PostUrl = finalResponse.PostUrl.Contains("reddit.com") ? finalResponse.PostUrl : finalResponse.HomePage + finalResponse.PostUrl;
                    updatedPostDetails.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                    {
                        AccountName = AccountModel.UserName,
                        DestinationUrl = AccountModel.UserName,
                        Destination = "OwnWall",
                        Description = updatedPostDetails.PostDescription,
                        IsPublished = finalResponse.Success ? "Yes" : "No",
                        Successful = finalResponse.Success ? "Yes" : "No",
                        Link = finalResponse.PostUrl,
                        PublishedDate = DateTime.Now,
                        SocialNetworks = SocialNetworks.Reddit
                    });
                    UpdatePostWithSuccessful(AccountModel.UserName, postDetails, finalResponse.PostUrl);
                    GlobusLogHelper.log.Info(Log.PublishingSuccessfully, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Ownwall", AccountModel.AccountBaseModel.UserName);
                }
                else
                {
                    UpdatePostWithFailed(AccountModel.UserName, postDetails, finalResponse.FailureMessage);
                    GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Ownwall",
                        AccountModel.AccountBaseModel.UserName + $" {finalResponse.FailureMessage}");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}