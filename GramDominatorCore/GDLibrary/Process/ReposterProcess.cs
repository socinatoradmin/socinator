using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDLibrary.InstagramBrowser.PuppeteerBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary
{
    public class ReposterProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public RePosterModel RePosterModel { get; set; }
        private int ActionBlockedCount;
        private List<string> LstTagUsers { get; set; } = new List<string>();
        public ReposterProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            RePosterModel = JsonConvert.DeserializeObject<RePosterModel>(templateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var Message = RePosterModel.RepostAsStory ? $"{scrapeResult.ResultPost.Code} as Story" : scrapeResult.ResultPost.Code;

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, Message);

            JobProcessResult jobProcessResult = new JobProcessResult();
            string thumbnailFilePath = string.Empty;
            string convertedMediaFilePath = string.Empty;
            List<string> lstOfUserTags = new List<string>();
            string serilizeUser = null;
            try
            {
                InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
                instagramPost.IsCheckedCropMedia = RePosterModel.IsCheckedCropMedias;
                instagramPost.CropRatio = RePosterModel.SelectedResolution;
                instagramPost.Caption = ModuleSetting.IsChkPostCaption ? MakeCaption(scrapeResult) : !string.IsNullOrEmpty(instagramPost.Caption) ? RePosterModel.CommentWithPostCaptionAndUsername ? $"{instagramPost.Caption}\n @{instagramPost?.User?.Username} ": instagramPost.Caption:"";
                if (instagramPost.Caption.Contains("\r\n"))
                {
                    instagramPost.Caption = instagramPost.Caption.Replace("\r\n", "\n");
                    char[] charcterCount = instagramPost.Caption.ToCharArray();
                    if (charcterCount.Count() > 2199)
                        instagramPost.Caption = instagramPost.Caption.Remove(2199);
                    
                }
                var browser = GramStatic.IsBrowser;
                UploadMediaResponse uploadResponse = null;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    try
                    {
                        if (ModuleSetting.IsChkUserTag)
                            LstTagUsers = GetTagUser(ref lstOfUserTags);
                        AccountModel.WwwClaim = AccountModel.WwwClaim ?? "0";
                        AccountModel.WwwClaim = instaFunct.GetGdHttpHelper().Response.Headers["x-ig-set-www-claim"] ?? AccountModel.WwwClaim;
                        if (instagramPost.MediaType == MediaType.Image)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName,
                             ActivityType, "Please wait... started to upload photo");
                            uploadResponse = instaFunct.UploadTimeLinePhoto(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, scrapeResult.QueryInfo.QueryTypeDisplayName, LstTagUsers, null, false, instagramPost.Caption);

                        }
                        else if (instagramPost.MediaType == MediaType.Video)
                        {
                            if (SetThumbnailAndVideoFormat(scrapeResult.QueryInfo.QueryTypeDisplayName, ref thumbnailFilePath, ref convertedMediaFilePath, instagramPost))
                                uploadResponse = instaFunct.UploadVideo(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, scrapeResult.QueryInfo.QueryTypeDisplayName, thumbnailFilePath, instagramPost.Caption, null, LstTagUsers);
                        }
                        else if (instagramPost.MediaType == MediaType.Album)
                        {
                            string fileName = Path.GetFileName(scrapeResult.QueryInfo.QueryTypeDisplayName);
                            int imageCount = Convert.ToInt32(Utilities.GetBetween(fileName, "", "-"));
                            List<string> mediaList = new List<string>();

                            for (int mediaCount = 0; mediaCount < imageCount; mediaCount++)
                            {
                                mediaList.Add(scrapeResult.QueryInfo.QueryTypeDisplayName.Replace(fileName, $"{mediaCount + 1}-{instagramPost.Code}.jpg"));
                            }

                            uploadResponse = instaFunct.UploadPhotoAlbum(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, mediaList, instagramPost.Caption, null, LstTagUsers);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    } 
                }
                else
                {
                    if (RePosterModel.RepostAsStory)
                    {
                        foreach(var media in instagramPost.RepostMedia)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var StoryUploadResponse = instaFunct.StoryUpload(DominatorAccountModel, instagramPost, media).Result;
                            if (StoryUploadResponse != null && StoryUploadResponse.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{StoryUploadResponse?.StoryUrl} as Story");
                                AddRepostedDataToDataBase(scrapeResult, StoryUploadResponse, serilizeUser);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{media} as Story failed to upload.");
                            }
                            if(instagramPost.RepostMedia.Count > 1)
                            {
                                var delay = RandomUtilties.GetRandomNumber(20, 5);
                                DelayBeforeNextActivity(delay); // Delay between each story upload
                            }
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        IncrementCounters();
                        jobProcessResult.IsProcessSuceessfull = true;
                        DelayBeforeNextActivity();
                        return jobProcessResult;
                    }
                    else
                    {
                        if (ModuleSetting.IsChkUserTag && !RePosterModel.RepostAsStory)
                        {
                            LstTagUsers = GetTagUser(ref lstOfUserTags);
                            instagramPost?.UserTags?.Clear();
                            foreach (var user in LstTagUsers)
                            {
                                var userInfo =
                                    browser ?
                                    instaFunct.GdBrowserManager.GetUserInfo(DominatorAccountModel, user, JobCancellationTokenSource.Token)
                                    : instaFunct.SearchUsername(DominatorAccountModel, user, JobCancellationTokenSource.Token);
                                instagramPost.UserTags.Add(userInfo);
                            }
                        }
                        if (instagramPost.MediaType == MediaType.Album)
                        {
                            uploadResponse =
                                browser ?
                                instaFunct.GdBrowserManager.UploadMedia(DominatorAccountModel, instagramPost, instagramPost.RepostMedia, JobCancellationTokenSource.Token)
                                : instaFunct.UploadMedia(DominatorAccountModel, instagramPost, instagramPost.RepostMedia).Result;
                        }
                        else if (instagramPost.MediaType == MediaType.Image)
                        {
                            uploadResponse =
                                browser ?
                                instaFunct.GdBrowserManager.UploadMedia(DominatorAccountModel, instagramPost, instagramPost.RepostMedia, JobCancellationTokenSource.Token)
                                : instaFunct.UploadMedia(DominatorAccountModel, instagramPost, instagramPost.RepostMedia).Result;
                        }
                        else
                        {
                            if (!File.Exists(PuppeteerBrowserActivity.GetExecutablePath()))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{scrapeResult.ResultPost.Code} -> Google Chrome must be intalled in system. post cannot be reposted since it is video.");
                                ToasterNotification.ShowError("Google Chrome must be intalled in system. post cannot be reposted since it is video.");
                                return jobProcessResult;
                            }
                            //PupBrowserRequest pup = new PupBrowserRequest(DominatorAccountModel,DominatorAccountModel.CancellationSource??new System.Threading.CancellationTokenSource());
                            //uploadResponse = pup.UploadVideo(instagramPost, mediaList);
                            uploadResponse = browser ?
                                instaFunct.UploadMediaHttp(DominatorAccountModel, instagramPost, instagramPost.RepostMedia, JobCancellationTokenSource.Token).Result
                                : instaFunct.UploadMedia(DominatorAccountModel, instagramPost, instagramPost.RepostMedia).Result;
                        }
                    }
                }

                if (uploadResponse != null && uploadResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, Message);
                    if (RePosterModel.IsCommentAfterRepost && !RePosterModel.RepostAsStory)
                    {
                        var instaPost = (InstagramPost)scrapeResult.ResultPost;
                        var commentText = RePosterModel.CommentWithPostCaptionAndUsername ? MakeComment(RePosterModel.OriginalCommentAfterRepostInputText, instaPost, RePosterModel.CommentWithPostCaptionAndUsername) : RePosterModel.OriginalCommentAfterRepostInputText;
                        commentText = SpinTexHelper.GetSpinText(commentText);
                        var commentResponse = 
                            browser ?
                            instaFunct.GdBrowserManager.Comment(DominatorAccountModel, AccountModel, instaPost.Code, commentText?.Replace("\r\n", "\n"), DominatorAccountModel.Token)
                            : instaFunct.Comment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instaPost.Code, commentText).Result;
                        if (commentResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment.ToString(), commentText);
                            instaPost.CommentText = commentText;
                            scrapeResult.ResultPost = instaPost;
                        }
                    }
                    if (ModuleSetting.NoOfUniqueUserTag && lstOfUserTags.Count != 0)
                        serilizeUser = JsonConvert.SerializeObject(lstOfUserTags);
                    IncrementCounters();

                    AddRepostedDataToDataBase(scrapeResult, uploadResponse, serilizeUser);

                    jobProcessResult.IsProcessSuceessfull = true;
                    
                }
                else
                {
                    RemoveFailedRepostedDataFromDataBase(scrapeResult);
                    if (!CheckResponse.CheckProcessResponse(uploadResponse, DominatorAccountModel, ActivityType, scrapeResult, ref ActionBlockedCount))
                    {
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                // Delay between each activity
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private string MakeComment(string commentText, InstagramPost instaPost,bool MentionPostOwner=false)
        {
            var OriginalComment = string.Empty;
            try
            {
                if(!string.IsNullOrEmpty(commentText) && commentText.Contains(Macros.OriginalPostCaption) && commentText.Contains(Macros.UserName))
                {
                    if(commentText.Contains(instaPost.User.Username) || (!string.IsNullOrEmpty(instaPost.Caption) && instaPost.Caption.Contains($"@{instaPost?.User?.Username}")))
                        OriginalComment = instaPost.Caption;
                    else
                        OriginalComment = commentText.Replace(Macros.OriginalPostCaption, instaPost.Caption)
                            .Replace(Macros.UserName, "\n @" + instaPost?.User?.Username + " ");

                }
                else
                {
                    OriginalComment = MentionPostOwner ?$"{commentText}\n @{instaPost?.User?.Username} " : commentText;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return OriginalComment;
        }

        private void AddRepostedDataToDataBase(ScrapeResultNew scrapResult, UploadMediaResponse uploadPhotoResponse, string UserLst = null)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramPost instagramPost = (InstagramPost)scrapResult.ResultPost;

            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                var dboperationCampaign =
                    new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);

                if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost || ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                {
                    string permalink = instagramPost.Code.GetUrlFromCode();

                    var interactedPost =
                        dboperationCampaign.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                            x => x.Permalink == permalink && x.ActivityType == ActivityType &&
                                 x.Username == DominatorAccountModel.AccountBaseModel.UserName &&
                                 (x.Status == "Pending" || x.Status == "Working"));

                    if (interactedPost != null)
                    {
                        interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                        interactedPost.MediaType = uploadPhotoResponse.MediaType;
                        interactedPost.ActivityType = ActivityType;
                        interactedPost.OriginalMediaCode = instagramPost.Code;
                        interactedPost.OriginalMediaOwner = instagramPost.User.Username;
                        interactedPost.PkOwner = uploadPhotoResponse.Code;
                        interactedPost.UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName;
                        interactedPost.Username = DominatorAccountModel.AccountBaseModel.UserName;
                        interactedPost.QueryType = scrapResult.QueryInfo.QueryType;
                        interactedPost.QueryValue = scrapResult.QueryInfo.QueryValue;
                        interactedPost.Status = "Success";
                        interactedPost.Comment = instagramPost.CommentText;
                        dboperationCampaign.Update(interactedPost);
                    }
                }
                else
                {
                    CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = uploadPhotoResponse.MediaType,
                        ActivityType = ActivityType,
                        OriginalMediaCode = instagramPost.Code,
                        OriginalMediaOwner = instagramPost.User.Username,
                        PkOwner = uploadPhotoResponse.Code,
                        UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        QueryType = scrapResult.QueryInfo.QueryType,
                        QueryValue = scrapResult.QueryInfo.QueryValue,
                        UsersTag = UserLst,
                        Comment = instagramPost.CommentText,
                        Status = "Success"

                    });
                }
            }


            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
                new InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = uploadPhotoResponse.MediaType,
                    ActivityType = ActivityType,
                    OriginalMediaCode = instagramPost.Code,
                    OriginalMediaOwner = instagramPost.User.Username,
                    PkOwner = uploadPhotoResponse.Code,
                    UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapResult.QueryInfo.QueryType,
                    QueryValue = scrapResult.QueryInfo.QueryValue,
                    Comment = instagramPost.CommentText
                });

            // Add data to respected Account's FeedInfoes table
            AccountDbOperation.Add(
                new DominatorHouseCore.DatabaseHandler.GdTables.Accounts.FeedInfoes()
                {
                    Caption = instagramPost.Caption,
                    TakenAt = uploadPhotoResponse.TakenAt,
                    MediaId = uploadPhotoResponse.MediaId,
                    MediaCode = uploadPhotoResponse.Code,
                    MediaType = uploadPhotoResponse.MediaType,
                    PostedBy = "Software"
                });
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public string MakeCaption(ScrapeResultNew scrapeResult)
        {
            string CustomCaption = RePosterModel.OriginalPostCaptionInputText;
            //CustomCaption = CustomCaption.Replace('\r',' ');
            string OriginalCaption = string.Empty;
            InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
            try
            {
                if (!string.IsNullOrEmpty(CustomCaption))
                {
                    OriginalCaption = CustomCaption.Replace(Macros.OriginalPostCaption, scrapeResult.ResultPost.Caption).Replace(Macros.AccountUserName, " @" + DominatorAccountModel.AccountBaseModel.UserName)
                    .Replace(Macros.PostUrl, scrapeResult.ResultPost.Code.GetUrlFromCode()).Replace(Macros.UserName, " @" + instagramPost.User.Username);
                    if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                    {
                        string spinText = " " + SpinTexHelper.GetSpinText(OriginalCaption) + " ";
                        OriginalCaption = spinText;
                    }
                }
                else
                {
                    OriginalCaption = RePosterModel.CommentWithPostCaptionAndUsername ? $"{instagramPost?.Caption} \n @{instagramPost.User.Username} ":instagramPost?.Caption ;
                }
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return OriginalCaption;
        }

        public void RemoveFailedRepostedDataFromDataBase(ScrapeResultNew scrapeResult)
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

        public bool SetThumbnailAndVideoFormat(string mediaPath, ref string thumbnailFilePath, ref string convertedMediaFilePath, InstagramPost instagramPost)
        {
            try
            {
                thumbnailFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)}.jpg";
                convertedMediaFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)} Compressed.mp4";

                //FFMpegConverter ffMpegConverter = new FFMpegConverter();
                //ffMpegConverter.FFMpegToolPath= ConstantVariable.GetOtherDir();
                //FFProbe ffProbe = new FFProbe();
                //ffProbe.ToolPath = ConstantVariable.GetOtherDir();
                //var mediaInfo = ffProbe.GetMediaInfo(mediaPath);
                var mediaInfo = Utility.GramStatic.GetMediaInfo(mediaPath);

                if (mediaInfo.Duration.TotalSeconds > 70 &&string.IsNullOrEmpty(instagramPost.ProductType))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, instagramPost.User.Username, "Reposter",
                        $"Instagram only allows to upload video size max: 60 secs. Got size {mediaInfo.Duration.TotalSeconds} secs");
                    return false;
                }
                if (mediaInfo.Duration.TotalSeconds > 70 && instagramPost.ProductType.Contains("igtv"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, instagramPost.User.Username, "Reposter",
                        $"{instagramPost.ProductType} video,Instagram only allows to upload video size max: 60 to 69 secs. Got size {mediaInfo.Duration.TotalSeconds} secs");
                    return false;
                }
                //ffMpegConverter.GetVideoThumbnail(
                //    (!File.Exists(convertedMediaFilePath) ? mediaPath : convertedMediaFilePath), thumbnailFilePath, 3);
                thumbnailFilePath = Utility.GramStatic.GetVideoThumb(mediaPath, convertedMediaFilePath);
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public List<string> GetTagUser(ref List<string> lstOfuserTags)
        {
            var ListOfTagUserIds = new List<string>();
            var ListTagUsers = RePosterModel.UserTagInputText.Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim('\r')).ToList();
            if (ModuleSetting.NoOfUniqueUserTag)
            {
                try
                {
                    var DbTaggedUsers = new List<string>();
                    var dbLstofUsers = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x => x.UsersTag != null).Select(y => y.UsersTag);
                    foreach (var users in dbLstofUsers)
                    {
                        if (string.IsNullOrEmpty(users))
                            continue;
                        var dbData = JsonConvert.DeserializeObject<List<string>>(users);
                        DbTaggedUsers.AddRange(dbData);
                    }
                    ListTagUsers.RemoveAll(x => DbTaggedUsers.Any(y => x == y));
                    int userCount = ListTagUsers.Count < ModuleSetting.NoOfUserTagPerPost.EndValue ? ListTagUsers.Count : ModuleSetting.NoOfUserTagPerPost.EndValue;
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        lstOfuserTags = ListTagUsers.GetRange(0, userCount);
                        return lstOfuserTags;
                    }
                    ListOfTagUserIds = GetUserId(userCount, ListTagUsers, ref lstOfuserTags);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    return ListTagUsers;
                ListOfTagUserIds = GetUserId(ListTagUsers.Count, ListTagUsers, ref lstOfuserTags);
            }

            return ListOfTagUserIds;
        }

        public List<string> GetUserId(int userCount, List<string> lstTagUsers, ref List<string> lstOfuserTags)
        {
            List<string> lstOfTagUserIds = new List<string>();
            for (int i = 0; i < userCount; i++)
            {
                string tagUsers = lstTagUsers[i];
                var TagUserInfo = instaFunct.SearchUsername(DominatorAccountModel, tagUsers, JobCancellationTokenSource.Token);
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    TagUserInfo = instaFunct.SearchUsername(DominatorAccountModel, tagUsers, JobCancellationTokenSource.Token);
                //}
                //else
                //    TagUserInfo = instaFunct.GdBrowserManager.GetUserInfo(DominatorAccountModel, tagUsers, JobCancellationTokenSource.Token);
                if (string.IsNullOrEmpty(TagUserInfo.ToString()))
                {
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(5));//Thread.Sleep(TimeSpan.FromSeconds(5));
                    TagUserInfo = instaFunct.SearchUsername(DominatorAccountModel, tagUsers, JobCancellationTokenSource.Token);
                }
                if (TagUserInfo.ToString().Contains("User not found"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName,
                "Reposter",
                $"This user name {tagUsers} is not available so it will not add in your tag feature");
                    continue;
                }
                lstOfTagUserIds.Add(TagUserInfo.Pk);
                if (ModuleSetting.NoOfUniqueUserTag)
                    lstOfuserTags.Add(TagUserInfo.Username);
            }
            return lstOfTagUserIds;
        }
    }
}
