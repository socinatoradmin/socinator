using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public abstract class BaseInstagramPostProcessor : BaseInstagramProcessor
    {

        #region Global Variable

        readonly CampaignDetails _campaignDetails;

        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();

        private List<ResultCommentItemUser> LstSpecificComment { get; set; }

        #endregion

        protected BaseInstagramPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
            JobProcess = jobProcess;
            _campaignDetails = processScopeModel.CampaignDetails;
        }

        public void FilterAndStartFinalProcessForOnePost(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, InstagramPost instagramPost)
        {
            try
            {
                if (ActivityType == ActivityType.PostScraper || ActivityType == ActivityType.Like || ActivityType == ActivityType.Comment || ActivityType == ActivityType.Reposter)
                {
                    if (!CheckPostUniqueNess(jobProcessResult, instagramPost))
                        return;

                    if (!ApplyCampaignLevelSettings(queryInfo, instagramPost.Code.GetUrlFromCode(), _campaignDetails))
                        return;

                    if (UserFilterModel(instagramPost, queryInfo))
                    {
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(2));
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $"{instagramPost.Code} User Filter not matched");
                        return;
                    }
                }
                if (!FilterImage(instagramPost))
                {
                    if (ActivityType == ActivityType.Reposter
                        || ActivityType == ActivityType.AddStory)
                    {
                        var locationPost = instagramPost.IsLocationPost;
                        // Get temporary folder path of current system
                        string tempFolderPath = $"{Path.GetTempPath()}{ConstantVariable.ApplicationName}\\{JobProcess.campaignId ?? "AccountMode"}\\{DominatorAccountModel.AccountId}";
                        DirectoryUtilities.CreateDirectory(tempFolderPath);
                        var mediaInfo = 
                            GramStatic.IsBrowser ?
                            GdBrowserManager.MediaInfo(DominatorAccountModel, instagramPost.Code, Token)
                            : InstaFunction.MediaInfo(DominatorAccountModel, AccountModel, instagramPost.Code, Token).Result;
                        instagramPost = mediaInfo.InstagramPost;
                        instagramPost.IsLocationPost = locationPost;
                        DownloadMedia(instagramPost, tempFolderPath);
                    }
                    Token.ThrowIfCancellationRequested();
                    SendToPerformActivity(ref jobProcessResult, instagramPost, queryInfo);
                    if (ActivityType == ActivityType.Reposter || ActivityType == ActivityType.AddStory)
                    {
                        try
                        {
                            instagramPost.RepostMedia.ForEach(media => FileUtilities.DeleteFile(media));
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                else
                {
                    jobProcessResult = new JobProcessResult();
                    if (instagramPost.HasLiked && ActivityType == ActivityType.Like)
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"This {instagramPost.Code} Post is already liked");
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{instagramPost.Code} Post Filter not matched");
                }
                Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex) { ex.DebugLog(); }
        }

        public void FilterAndStartFinalProcessForOneComment(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, InstagramPost instagramPost)
        {
            List<string> specificComment;
            try
            {
                bool isCheckedLikeCommentRepostPerPost = false;
                int likeCommentRepostCountPerPost = 0;

                if (ActivityType == ActivityType.LikeComment)
                {
                    var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                    var modeldata = JsonConvert.DeserializeObject<LikeCommentModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
                    if (modeldata.IsCheckedCommentPerPost)
                    {
                        isCheckedLikeCommentRepostPerPost = true;
                        likeCommentRepostCountPerPost = modeldata.CommentCountPerPost.GetRandom();
                    }
                }
                if (!FilterImage(instagramPost))
                {
                    do
                    {
                        instagramPost.Url = $"{Constants.gdHomeUrl}/p/{instagramPost.Code}";
                        var mediaComments = 
                            GramStatic.IsBrowser ?
                            InstaFunction.GdBrowserManager.GetMediaComments(DominatorAccountModel, instagramPost.Url, Token)
                            : InstaFunction.GetMediaComments(DominatorAccountModel, instagramPost.Code, Token, jobProcessResult.SecondMaxID);
                        if (ActivityType == ActivityType.LikeComment)
                        {
                            var SkippedLiked =  mediaComments.CommentList.RemoveAll(x => x.HasLikedComment);
                            if(SkippedLiked > 0)
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $"Successfully Skipped {SkippedLiked} Already Liked Comments.");
                        }

                        if (mediaComments.CommentList.Count == 0)
                        {
                            if (string.IsNullOrEmpty(mediaComments.MaxId))
                            {
                                NoData(ref jobProcessResult,instagramPost.Url);
                                return;
                            }
                            // Delay of 4 Seconds for hitting to the next post 
                            DelayService.ThreadSleep(TimeSpan.FromSeconds(8));//Thread.Sleep(TimeSpan.FromSeconds(4));
                            continue;
                        }

                        LstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();

                        mediaComments.CommentList.RemoveAll(x => LstInteractedPosts.Any(y => y.CommentId == x.CommentId && y.Comment == x.Text && y.PkOwner == instagramPost.Code));

                        if (ModuleSetting.isChkLikeOnSpecificCommnetOfPost && mediaComments.CommentList.Count != 0)
                        {
                            specificComment = Regex.Split(ModuleSetting.SpecificCommentText, "\r\n").ToList();
                            LstSpecificComment = new List<ResultCommentItemUser>(mediaComments.CommentList);
                            LstSpecificComment = mediaComments.CommentList.FindAll(y => specificComment.Any(x => x.Equals(y.Text) || (!string.IsNullOrEmpty(y.Text) && y.Text.Contains(x))));

                            SetQuantityIfMaxIdEmpty(jobProcessResult, LstSpecificComment.Count);

                            foreach (ResultCommentItemUser eachComment in LstSpecificComment)
                            {
                                if (!FilterUserApply(eachComment.ItemUser, queryInfo))
                                {
                                    if (isCheckedLikeCommentRepostPerPost)
                                    {
                                        try
                                        {
                                            var lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType);

                                            if (lstInteractedPosts.Count(post => post.PkOwner == instagramPost.Code && post.QueryType == queryInfo.QueryType && post.QueryValue == queryInfo.QueryValue) >= likeCommentRepostCountPerPost)
                                                return;
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.DebugLog();
                                        }
                                    }
                                    SendToPerformActivityForLikeComment(ref jobProcessResult, eachComment, instagramPost, queryInfo);
                                }
                                else
                                {
                                    DelayService.ThreadSleep(TimeSpan.FromSeconds(2));//Thread.Sleep(2 * 1000);
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                                    $" {eachComment?.ItemUser?.Username} User Filter not matched");
                                }
                                Token.ThrowIfCancellationRequested();
                            }
                            DelayService.ThreadSleep(TimeSpan.FromSeconds(2));// Thread.Sleep(TimeSpan.FromSeconds(5));

                        }
                        else if (mediaComments.CommentList.Count > 0)
                        {
                            SetQuantityIfMaxIdEmpty(jobProcessResult, mediaComments.CommentList.Count);
                            foreach (ResultCommentItemUser eachComment in mediaComments.CommentList)
                            {
                                if (!FilterUserApply(eachComment.ItemUser, queryInfo))
                                {
                                    if (isCheckedLikeCommentRepostPerPost)
                                    {
                                        try
                                        {
                                            var lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType);

                                            if (lstInteractedPosts.Count(post => post.PkOwner == instagramPost.Code && post.QueryType == queryInfo.QueryType && post.QueryValue == queryInfo.QueryValue) >= likeCommentRepostCountPerPost)
                                                return;
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.DebugLog();
                                        }
                                    }
                                    SendToPerformActivityForLikeComment(ref jobProcessResult, eachComment, instagramPost, queryInfo);
                                }
                                else
                                {
                                    DelayService.ThreadSleep(TimeSpan.FromSeconds(2));//Thread.Sleep(2 * 1000);
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                                    $" {eachComment?.ItemUser?.Username} User Filter not matched");
                                }
                                Token.ThrowIfCancellationRequested();
                            }
                            DelayService.ThreadSleep(TimeSpan.FromSeconds(5));// Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                        jobProcessResult.SecondMaxID = mediaComments.MaxId;


                    } while (!string.IsNullOrEmpty(jobProcessResult.SecondMaxID));
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $"Filter Not Matched For Post {instagramPost?.Code}");
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

        public JobProcessResult StartProcessWithUsersFeeds(QueryInfo queryInfo, List<InstagramUser> lstInstaUsers, string postCode = "")
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            #region Specific post count range of one user for Like/Comment module

            bool isCheckedLikeCommentRepostPerUser = false;
            int likeCommentRepostCountPerUser = 0;

            try
            {
                if (ActivityType == ActivityType.Like || ActivityType == ActivityType.LikeComment || ActivityType == ActivityType.Comment || ActivityType == ActivityType.Reposter)
                {
                    if (ActivityType == ActivityType.Like)
                    {
                        var modeldata = JsonConvert.DeserializeObject<LikeModel>(TemplateModel.ActivitySettings);

                        if (modeldata.IsCheckedLikeCountPerUser)
                        {
                            isCheckedLikeCommentRepostPerUser = true;
                            likeCommentRepostCountPerUser = modeldata.LikeCountPerUser.GetRandom();
                        }
                    }
                    else if (ActivityType == ActivityType.Comment)
                    {
                        var modeldata = JsonConvert.DeserializeObject<CommentModel>(TemplateModel.ActivitySettings);
                        if (modeldata.IsCheckedCommentPerUser)
                        {
                            isCheckedLikeCommentRepostPerUser = true;
                            likeCommentRepostCountPerUser = modeldata.CommentCountPerUser.GetRandom();
                        }
                    }
                    else if (ActivityType == ActivityType.Reposter)
                    {
                        var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                        var modeldata = JsonConvert.DeserializeObject<RePosterModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                        if (modeldata.IsCheckedRepostCountPerUser)
                        {
                            isCheckedLikeCommentRepostPerUser = true;
                            likeCommentRepostCountPerUser = modeldata.RepostCountPerUser.GetRandom();
                        }
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

            #endregion


            try
            {
                foreach (var instagramUser in lstInstaUsers)
                {
                    if (instagramUser.Username == null)//|| instagramUser.IsPrivate
                        continue;
                    Token.ThrowIfCancellationRequested();
                    jobProcessResult = new JobProcessResult();
                    bool isNewUserForBrowser = true;
                    if (FilterUserApply(instagramUser, queryInfo))
                    {
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(2));//Thread.Sleep(2 * 1000);
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $" {instagramUser.Username} {postCode} User Filter not matched");
                        continue;
                    }

                    #region Check Max Like/Comment count reached already
                    if ((ActivityType == ActivityType.Like || ActivityType == ActivityType.LikeComment || ActivityType == ActivityType.Comment || ActivityType == ActivityType.Reposter) && isCheckedLikeCommentRepostPerUser)
                    {
                        try
                        {
                            var lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType);

                            if (ActivityType == ActivityType.Reposter && (lstInteractedPosts.Count(post => post.OriginalMediaOwner == instagramUser.Username) >= likeCommentRepostCountPerUser))
                                continue;
                            else if (ActivityType != ActivityType.LikeComment && lstInteractedPosts.Count(post => post.UsernameOwner == instagramUser.Username && post.QueryType == queryInfo.QueryType && post.QueryValue == queryInfo.QueryValue) >= likeCommentRepostCountPerUser)
                                continue;
                            else
                            {
                                lstInteractedPosts = lstInteractedPosts?.Where(post => post.UsernameOwner == instagramUser.Username && post.QueryType == queryInfo.QueryType && post.QueryValue == queryInfo.QueryValue).ToList();
                                List<InteractedPosts> list = new List<InteractedPosts>();
                                lstInteractedPosts?.ForEach(x =>
                                {
                                    if (list.All(y => y.PkOwner != x.PkOwner))
                                        list.Add(x);
                                });
                                if (list.Count() >= likeCommentRepostCountPerUser)
                                    continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    #endregion
                    // int countPost = 0;
                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        Token.ThrowIfCancellationRequested();

                        int processedLikeCommentRepostCountPerUser = 0;
                        bool shouldWhilebreak = false;
                        var browser = GramStatic.IsBrowser;
                        var userFeedDetails = 
                            browser ?
                            InstaFunction.GdBrowserManager.GetUserFeed(DominatorAccountModel, instagramUser.Username, Token)
                            : InstaFunction.GetUserFeed(DominatorAccountModel, AccountModel,instagramUser.Username, Token, jobProcessResult.maxId, isNewUserBrowser: isNewUserForBrowser);
                        if (userFeedDetails.Issue != null)
                        {
                            if (userFeedDetails.Issue.Error == GDEnums.InstagramError.NotAuthorized)
                                break;
                        }
                        #region Check Feeds are present or not
                        try
                        {
                            if (DominatorAccountModel.IsRunProcessThroughBrowser)
                            {
                                if (instagramUser!=null && instagramUser.IsPrivate || instagramUser.UserDetails!=null && instagramUser.UserDetails.IsPrivate)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                        $" {instagramUser.Username} is private account.");
                                    break;
                                }
                                if (userFeedDetails.Items.Count == 0)
                                    break;
                            }
                            else
                            {
                                if (Convert.ToInt32(Newtonsoft.Json.Linq.JObject.Parse(userFeedDetails.ToString())["num_results"]) == 0)
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            //ex.DebugLog();
                        }

                        #endregion

                        if (userFeedDetails.Success)
                        {
                            List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(userFeedDetails.Items);
                            filteredFeeds = FilterAllImagesApply(filteredFeeds);
                            CheckInteractedPostsData(new List<InteractedPosts>(), filteredFeeds);
                            if (ModuleSetting.PostFilterModel.FilterPostAge && !ModuleSetting.PostFilterModel.FilterBeforePostAge && filteredFeeds.Count == 0)
                                break;
                            jobProcessResult.maxId = userFeedDetails?.MaxId;
                            foreach (var instagramPost in filteredFeeds)
                            {
                                Token.ThrowIfCancellationRequested();
                                if (ActivityType == ActivityType.LikeComment || ActivityType == ActivityType.CommentScraper || ActivityType == ActivityType.ReplyToComment)
                                {
                                    if (ActivityType == ActivityType.LikeComment && isCheckedLikeCommentRepostPerUser)
                                    {
                                        var lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType);
                                        lstInteractedPosts = lstInteractedPosts?.Where(post => post.UsernameOwner == instagramUser.Username && post.QueryType == queryInfo.QueryType && post.QueryValue == queryInfo.QueryValue).ToList();
                                        List<InteractedPosts> list = new List<InteractedPosts>();
                                        lstInteractedPosts?.ForEach(x =>
                                        {
                                            if (list.All(y => y.PkOwner != x.PkOwner))
                                                list.Add(x);
                                        });
                                        if (list.Count() >= likeCommentRepostCountPerUser)
                                        {
                                            shouldWhilebreak = true;
                                            break;
                                        }
                                    }
                                    FilterAndStartFinalProcessForOneComment(queryInfo, ref jobProcessResult, instagramPost);
                                }
                                else
                                {
                                    if (ActivityType == ActivityType.Reposter)
                                    {
                                        var lstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType);
                                        if (lstInteractedPosts.Any(x => instagramPost.Code == x.OriginalMediaCode))
                                            continue;
                                    }
                                    //  if (CheckingUniqueFunctionality(queryInfo, ref jobProcessResult, instagramPost)) break;//pending to Modified code for UniqueUserfunctionalitty
                                    FilterAndStartFinalProcessForOnePost(queryInfo, ref jobProcessResult, instagramPost);

                                    if (jobProcessResult != null && jobProcessResult.IsProcessSuceessfull)
                                        processedLikeCommentRepostCountPerUser++;

                                    if (isCheckedLikeCommentRepostPerUser && (processedLikeCommentRepostCountPerUser >= likeCommentRepostCountPerUser))
                                        break;
                                }
                            }

                            if ((isCheckedLikeCommentRepostPerUser && (processedLikeCommentRepostCountPerUser >= likeCommentRepostCountPerUser)) || shouldWhilebreak)
                                break;
                        }
                        else
                            jobProcessResult.maxId = null;

                        CheckNoMoreDataForWithQuery(ref jobProcessResult);
                        // DelayForScraperActivity();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex) { ex.DebugLog(); }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }

            return jobProcessResult;
        }


        public List<InstagramPost> FilterAllImages(List<InstagramPost> instagramPost)
        {
            return FilterAllImagesApply(instagramPost);
        }

        public bool FilterImage(InstagramPost instagramPost, bool isRefreshData = true)
        {
            if(instagramPost !=null && instagramPost.Pk == null)
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var mediaInfo = 
                        GramStatic.IsBrowser ?
                        InstaFunction.GdBrowserManager.MediaInfo(DominatorAccountModel, instagramPost.Code, DominatorAccountModel.Token)
                        : InstaFunction.MediaInfo(DominatorAccountModel, AccountModel, instagramPost.Code, DominatorAccountModel.Token).Result;
                    if (mediaInfo != null)
                        instagramPost = mediaInfo.InstagramPost;
                }
            }
            switch (ActivityType)
            {
                case ActivityType.Like:
                    if (instagramPost.HasLiked)
                        return true;
                    break;
                case ActivityType.Unlike:
                    if (!instagramPost.HasLiked)
                        return true;
                    break;
                case ActivityType.Comment:
                case ActivityType.PostScraper:

                    try
                    {
                        // Allow multiple comments on same post
                        #region Allow multiple comments for same post
                        if (ActivityType == ActivityType.Comment)
                        {

                            CommentModel = CommentModel ?? GetCommentModel();

                            if ((CommentModel != null) && CommentModel.IsChkMultipleCommentsOnSamePost)
                                break;

                            if (CommentModel != null && (!string.IsNullOrEmpty(CommentModel.MentionUsers) && CommentModel.IsChkMultipleMentionOnSamePost))
                                // ReSharper disable once RedundantJumpStatement
                                break;

                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    break;
                case ActivityType.Reposter:
                    break;
            }

            return FilterImageApply(ref instagramPost);
        }

        public void DownloadMedia(InstagramPost instagramPost, string tempFolderPath)
        {
            var tempMediaPath = string.Empty;

            int downloadPhotoTryCount = 0;
            while (downloadPhotoTryCount <= 2)
            {
                try
                {
                    downloadPhotoTryCount++;

                    WebClient webclient = new WebClient();
                    if (instagramPost.MediaType == MediaType.Image)
                    {
                        tempMediaPath =
                            $"{tempFolderPath}\\{instagramPost.Code}.{GetMediaExtension(instagramPost.Images.First().Url)}";
                        var Url = instagramPost.Images?.FirstOrDefault()?.Url;
                        webclient.DownloadFile(Url, tempMediaPath);
                        instagramPost.RepostMedia.Add(tempMediaPath);
                    }

                    // TODO : When video uploading request will get implemented, we can uncomment below region

                    #region (Commented) Download Video file

                    else if (instagramPost.MediaType == MediaType.Video)
                    {
                        tempMediaPath = $"{tempFolderPath}\\{instagramPost.Code}.{GetMediaExtension(instagramPost.Video.Url)}";
                        var Url = instagramPost?.Video?.Url;
                        webclient.DownloadFile(Url, tempMediaPath);
                        instagramPost.RepostMedia.Add(tempMediaPath);
                    }

                    #endregion

                    else if (instagramPost.MediaType == MediaType.Album)
                    {
                        for (var iterCount = 0; iterCount < instagramPost.Album.Count; iterCount++)
                        {
                            var carouselMedia = instagramPost.Album[iterCount];
                            if ((MediaType)carouselMedia.MediaType == MediaType.Image)
                            {
                                tempMediaPath = $"{tempFolderPath}\\{iterCount + 1}-{instagramPost.Code}.{GetMediaExtension(carouselMedia?.Images?.FirstOrDefault()?.Url)}";
                                var Url = carouselMedia?.Images?.FirstOrDefault()?.Url;
                                webclient.DownloadFile(Url, tempMediaPath);
                                instagramPost.RepostMedia.Add(tempMediaPath);
                            }
                            else if((MediaType)carouselMedia.MediaType == MediaType.Video)
                            {
                                var url = instagramPost?.Video?.Url ?? carouselMedia?.Video?.Url;
                                tempMediaPath = $"{tempFolderPath}\\{iterCount + 1}-{instagramPost.Code}.{GetMediaExtension(url)}";
                                webclient.DownloadFile(url, tempMediaPath);
                                instagramPost.RepostMedia.Add(tempMediaPath);
                            }
                        }
                    }


                    if (instagramPost?.RepostMedia?.Count > 0)
                        break;

                    DelayService.ThreadSleep(TimeSpan.FromSeconds(2)); //Thread.Sleep(2000);
                }
                catch (Exception)
                {
                    Console.WriteLine();
                }
            }
        }

        public string GetMediaExtension(string mediaUrl)
        {
            if (mediaUrl.Contains("stp=dst-"))
                return Utilities.GetBetween(mediaUrl, "stp=dst-", "_")?.Replace("r","");
            if (mediaUrl.Contains("?"))
                mediaUrl = Utilities.GetBetween(mediaUrl, "", "?");
            return mediaUrl.Split('.').Last();
        }

        public void SendToPerformActivity([NotNull] ref JobProcessResult jobProcessResult, InstagramPost instagramPost, QueryInfo queryInfo)
        {
            if (jobProcessResult == null) throw new ArgumentNullException(nameof(jobProcessResult));
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultPost = instagramPost,
                QueryInfo = queryInfo
            });
        }

        public void SendToPerformActivityForLikeComment([NotNull] ref JobProcessResult jobProcessResult, ResultCommentItemUser postComment, InstagramPost instagramPost, QueryInfo queryInfo)
        {
            if (jobProcessResult == null) throw new ArgumentNullException(nameof(jobProcessResult));
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultPostComment = postComment,
                QueryInfo = queryInfo,
                ResultPost = instagramPost
            });
        }

        protected void StartProcessForOwnFollowings(QueryInfo queryInfo)
        {
            try
            {
                if (queryInfo.QueryValue == null)
                    throw new NullReferenceException();

                QueryType = queryInfo.QueryType;

                if ((JobProcess.IsStop())) return;

                var allFollowers = DbAccountService.GetFollowers().Where(x => x.Followers == 1).ToList();
                allFollowers.Shuffle();
                foreach (var follower in allFollowers)
                {
                    if (JobProcess.IsStop())
                        return;
                    DelayService.ThreadSleep(TimeSpan.FromSeconds(1));
                    var followerInfo = InstaFunction.SearchUsername(DominatorAccountModel, follower.Username, Token);

                    #region Process for "SomeonesFollowersPost" query parameter
                    if (QueryType == "SomeonesFollowersPost")
                        StartProcessWithUsersFeeds(queryInfo, new List<InstagramUser>() { followerInfo });
                    #endregion
                    Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected void StartProcessForOwnFollowers(QueryInfo queryInfo)
        {
            try
            {
                if (queryInfo.QueryValue == null)
                    throw new NullReferenceException();

                QueryType = queryInfo.QueryType;

                if (JobProcess.IsStop()) return;

                var allFollowers = DbAccountService.GetFollowers().Where(x => x.Followers == 1).ToList();
                allFollowers.Shuffle();

                foreach (var follower in allFollowers)
                {
                    if (JobProcess.IsStop()) return;

                    DelayService.ThreadSleep(TimeSpan.FromSeconds(1));//Thread.Sleep(1000);
                    var followerInfo = InstaFunction.SearchUsername(DominatorAccountModel, follower.Username, Token);

                    if (QueryType == "Someone's Followers Post(S)")
                        StartProcessWithUsersFeeds(queryInfo, new List<InstagramUser>() { followerInfo });

                    Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static string CalculateMd5Hash(string input)
        {
            string newImage = String.Empty;
            try
            {
                MD5 md5 = MD5.Create();
                byte[] inputBytes = File.ReadAllBytes(input);
                GetMd5HashCode(input, md5, inputBytes);

                byte[] bArray = new byte[inputBytes.Length + 1];
                inputBytes.CopyTo(bArray, 0);
                bArray[bArray.Length - 1] = Convert.ToByte('\0');
                newImage = input.Split('.')[0] + "_hash.jpg";
                File.WriteAllBytes(newImage, bArray);

                inputBytes = File.ReadAllBytes(newImage);
                GetMd5HashCode(newImage, md5, inputBytes);
                if (File.Exists(input))
                    File.Delete(input);
            }
            catch (Exception ex) { ex.DebugLog(); }

            return newImage;
        }

        public static string GetMd5HashCode(string input, MD5 md5, byte[] inputBytes)
        {
            StringBuilder sb = new StringBuilder();
            // byte[] inputBytess = File.ReadAllBytes(input);
            byte[] hash1 = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            foreach (var t in hash1)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
