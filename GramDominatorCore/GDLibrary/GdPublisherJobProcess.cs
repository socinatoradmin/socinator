using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ThreadUtils;
using Unity;
using InstagramModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel;

namespace GramDominatorCore.GDLibrary
{
    public class GdPublisherJobProcess : PublisherJobProcess
    {
        private IGdHttpHelper httpHelper;
        private readonly IGdLogInProcess _loginProcess;
        private readonly IDateProvider _dateProvider;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private CancellationTokenSource _campaignCancellationToken;
        public InstagramModel InstagramModel { get; set; }
        public IDelayService DelayServices { get; set; }
        public IGdBrowserManager GdBrowserManager { get; set; }
        public GdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            httpHelper = _accountScopeFactory[accountId].Resolve<IGdHttpHelper>();
            _loginProcess = _accountScopeFactory[accountId].Resolve<IGdLogInProcess>();
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InstagramModel = genericFileManager.GetModuleDetails<InstagramModel>
                                     (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram))
                                 .FirstOrDefault(x => x.CampaignId == campaignId) ?? new InstagramModel();
            DelayServices = InstanceProvider.GetInstance<IDelayService>();
            GdBrowserManager = InstanceProvider.GetInstance<IGdBrowserManager>();
            _dateProvider = InstanceProvider.GetInstance<IDateProvider>();
        }


        public GdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            httpHelper = _accountScopeFactory[accountId].Resolve<IGdHttpHelper>();
            _loginProcess = _accountScopeFactory[accountId].Resolve<IGdLogInProcess>();
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InstagramModel = genericFileManager.GetModuleDetails<InstagramModel>
                                  (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram))
                              .FirstOrDefault(x => x.CampaignId == campaignId) ?? new InstagramModel();
            DelayServices = InstanceProvider.GetInstance<IDelayService>();
            GdBrowserManager = InstanceProvider.GetInstance<IGdBrowserManager>();
            _campaignCancellationToken = campaignCancellationToken;
        }


        public override bool DeletePost(string postId)
        {

            try
            {
                // Get Media Id from full Url
                //string mediaId = postId.GetCodeFromUrl().GetIdFromCode();
                AccountModel accountModel = new AccountModel(AccountModel);

                #region Login process

                if (!AccountModel.IsUserLoggedIn)
                {
                    //loginProcess = CommonServiceLocator.InstanceProvider.GetInstance<IGdLogInProcess>();
                    //LogInProcess logInProcess = new LogInProcess(httpHelper);
                    _loginProcess.LoginWithAlternativeMethod(AccountModel, AccountModel.Token);

                    if (!AccountModel.IsUserLoggedIn)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                            "Publisher", "User not able to logged in");
                        return false;
                    }
                }

                #endregion

                #region Delete Media process and display on Logger

                InstaFunct instaFunct = new InstaFunct(AccountModel, httpHelper, GdBrowserManager, DelayServices, _dateProvider);
                var browser = GramStatic.IsBrowser;
                if (browser)
                    GdBrowserManager.BrowserLogin(AccountModel, CurrentJobCancellationToken.Token);
                var deleteMediaResponse = 
                    browser ?
                    GdBrowserManager.DeleteMedia(AccountModel, new InstagramPost { Code = postId }, CurrentJobCancellationToken.Token)
                    : instaFunct.DeleteMedia(AccountModel, accountModel, CurrentJobCancellationToken.Token, postId).Result;

                if (deleteMediaResponse != null && deleteMediaResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                        "Delete Posted Media",
                        deleteMediaResponse.DidDelete
                            ? $"Successfully deleted media : [{postId}]"
                            : $"Seems the media : [{postId}] has been already deleted");

                    return true;
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                    "Delete Posted Media", $"Failed to delete media : [{postId}]");

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if(AccountModel.IsRunProcessThroughBrowser)
                    GdBrowserManager.CloseBrowser();
            }
            return false;
        }
        IInstaFunction instaFunct => _loginProcess.InstagramFunctFactory.InstaFunctions;
        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            try
            {
                var updatesPostListModel = PerformGeneralSettings(postDetails);
                if (!ValidateInstagramPosts(updatesPostListModel))
                    return false;
                if (!CheckLogin())
                    return false;
                //InstaFunct instaFunct = _loginProcess.InstagramFunctFactory.InstaFunctions; //new InstaFunct(AccountModel, httpHelper, GdBrowserManager, DelayServices, _dateProvider);

                AccountModel account = new AccountModel(AccountModel);
                IgPublisherModel publisherModel = new IgPublisherModel();
                InstagramModel instagramModel = null;
                UploadMediaResponse uploadMediaResponse = null;
                List<ImagePosition> imagePostion = new List<ImagePosition>();
                string postCaption = updatesPostListModel.PostDescription;
                if (!string.IsNullOrEmpty(postCaption))
                {
                    postCaption = postCaption.Replace("\r\n", "\n");
                }
                string fullCaption = postCaption;
                string newMediaPathforVideo = string.Empty;
                if (!updatesPostListModel.IsChangeHashOfMedia && updatesPostListModel.MediaList.Count != 0)
                {
                    publisherModel.mediaPath =
                        updatesPostListModel.MediaList[new Random().Next(0, updatesPostListModel.MediaList.Count)];
                }
                else
                {
                    publisherModel.mediaPath = updatesPostListModel.MediaList.Count > 0 ?
                       updatesPostListModel.MediaList[new Random().Next(0, updatesPostListModel.MediaList.Count)]:string.Empty;
                    publisherModel.mediaPath = !string.IsNullOrEmpty(publisherModel.mediaPath) ? MediaUtilites.CalculateMD5Hash(publisherModel.mediaPath):postDetails.ShareUrl;
                }
                GdPostSettings gdPostSettings = updatesPostListModel.GdPostSettings;
                bool isLocationName = false;

                if (updatesPostListModel.MediaList.Count > 1)
                {
                    //multiple image and video post
                    foreach (string media in updatesPostListModel.MediaList)
                    {
                        //video
                        if (media.Contains(".mp4") || publisherModel.mediaPath.EndsWith("mp4") || publisherModel.mediaPath.EndsWith("MP4"))
                        {
                            if (!SetThumbnailAndVideoFormats(media, publisherModel, updatesPostListModel, ref newMediaPathforVideo))
                                return false;
                            if (!string.IsNullOrEmpty(newMediaPathforVideo))
                            {
                                publisherModel.mediaPath = newMediaPathforVideo;
                            }
                            if (updatesPostListModel.GdPostSettings.IsPostAsStoryPost || OtherConfiguration.IsPostAsStoryChecked)
                            {
                                publisherModel.checkedVideoPath.Add(media);
                                publisherModel.isVideo = true;
                            }
                            publisherModel.lstThumbnailVideo.Add(publisherModel.thumbnailFilePath);
                        }
                        else
                        {
                            //image
                            if (!string.IsNullOrEmpty(publisherModel.mediaPath) &&
                                !publisherModel.mediaPath.Contains("wp-content"))
                            {
                                publisherModel.mediaPath = new ImageHelper().ConvertPicIntoActualSize(media, AccountModel, gdPostSettings, ref imagePostion, OtherConfiguration);
                                if (publisherModel.mediaPath.Contains("image size"))
                                    return false;
                                publisherModel.CheckedImagePath.Add(publisherModel.mediaPath);
                            }
                        }
                    }
                    //setting configuration
                    instagramModel = ManageAdvancedSettings(postCaption, ref fullCaption,
                        updatesPostListModel.MediaList, ref isLocationName, publisherModel);

                    publisherModel.isPostAsStory = gdPostSettings.IsPostAsStoryPost
                        ? gdPostSettings.IsPostAsStoryPost
                        : OtherConfiguration.IsPostAsStoryChecked;
                    publisherModel.lstTagUserIds = ManageInstaIndividualSettings(gdPostSettings);
                    publisherModel.isLocationName = isLocationName;
                    if (instagramModel != null && instagramModel.IsPostusingGeoLocation)
                        publisherModel.geoLocation = instagramModel.GeoLocation;
                    if(gdPostSettings != null && !string.IsNullOrEmpty(gdPostSettings.GeoLocationList))
                        publisherModel.geoLocation = gdPostSettings.GeoLocationList;
                    publisherModel.tagLocationDetails = CombineFilteration(AccountModel, gdPostSettings,
                        publisherModel.geoLocation, null, publisherModel.isLocationName);
                }
                else
                {
                    //Single image and video post
                    if (publisherModel.mediaPath.Contains(".mp4") || publisherModel.mediaPath.EndsWith("mp4") || publisherModel.mediaPath.EndsWith("MP4"))
                    {
                        //video
                        publisherModel.isVideo = true;
                        if (gdPostSettings.IsReelPost)
                        {
                            if (!SetThumbnail(publisherModel.mediaPath, publisherModel, updatesPostListModel, ref newMediaPathforVideo))
                                return false;
                        }
                        else
                        {
                            if (!SetThumbnailAndVideoFormats(publisherModel.mediaPath, publisherModel, updatesPostListModel, ref newMediaPathforVideo))
                                return false;
                        }
                        if (!string.IsNullOrEmpty(newMediaPathforVideo))
                        {
                            publisherModel.mediaPath = newMediaPathforVideo;
                        }
                        if (updatesPostListModel.GdPostSettings.IsPostAsStoryPost || OtherConfiguration.IsPostAsStoryChecked)
                        {
                            publisherModel.checkedVideoPath.Add(publisherModel.mediaPath);
                            publisherModel.isPostAsStory = gdPostSettings.IsPostAsStoryPost
                            ? gdPostSettings.IsPostAsStoryPost
                            : OtherConfiguration.IsPostAsStoryChecked;
                        }
                        if (updatesPostListModel.GdPostSettings.IsReelPost)
                        {
                            publisherModel.checkedVideoPath.Add(publisherModel.mediaPath);
                            publisherModel.isPostAsReels = gdPostSettings.IsReelPost;
                        }
                        publisherModel.lstThumbnailVideo.Add(publisherModel.thumbnailFilePath);
                        instagramModel = ManageAdvancedSettings(postCaption, ref fullCaption,
                            updatesPostListModel.MediaList, ref isLocationName, publisherModel);
                        publisherModel.isLocationName = isLocationName;
                        if (instagramModel != null && instagramModel.IsPostusingGeoLocation)
                            publisherModel.geoLocation = instagramModel.GeoLocation;
                        publisherModel.tagLocationDetails = CombineFilteration(AccountModel, gdPostSettings, publisherModel.geoLocation, null, publisherModel.isLocationName);

                        publisherModel.lstTagUserIds = ManageInstaIndividualSettings(gdPostSettings);

                    }
                    else
                    {
                        //Rss Feed
                        if (postDetails.PostSource == PostSource.RssFeedPost)
                        {
                            publisherModel.mediaPath = GetRssFeedMedia(ref postCaption, postDetails);
                            fullCaption = postCaption;
                            if (string.IsNullOrEmpty(publisherModel.mediaPath))
                                return false;
                        }
                        if (!string.IsNullOrEmpty(publisherModel.mediaPath) &&
                            !publisherModel.mediaPath.Contains("wp-content"))
                        {
                            publisherModel.mediaPath = new ImageHelper().ConvertPicIntoActualSize(publisherModel.mediaPath, AccountModel, gdPostSettings, ref imagePostion, OtherConfiguration);
                            if (publisherModel.mediaPath.Contains("image size"))
                                return false;
                            if (updatesPostListModel.GdPostSettings.IsPostAsStoryPost || OtherConfiguration.IsPostAsStoryChecked)
                                publisherModel.CheckedImagePath.Add(publisherModel.mediaPath);
                        }

                        publisherModel.lstTagUserIds = ManageInstaIndividualSettings(gdPostSettings);

                        instagramModel = ManageAdvancedSettings(postCaption, ref fullCaption,
                            updatesPostListModel.MediaList, ref isLocationName, publisherModel);

                        publisherModel.isPostAsStory = gdPostSettings.IsPostAsStoryPost
                            ? gdPostSettings.IsPostAsStoryPost
                            : OtherConfiguration.IsPostAsStoryChecked;
                        publisherModel.isLocationName = isLocationName;
                        if (gdPostSettings.IsGeoLocation)
                            publisherModel.geoLocation = gdPostSettings.GeoLocationList;
                        publisherModel.tagLocationDetails = CombineFilteration(AccountModel, gdPostSettings, publisherModel.geoLocation, null, publisherModel.isLocationName);
                    }
                }

                // Media Upload Response

                #region Media Upload Response
                if (gdPostSettings.IsReelPost && !publisherModel.mediaPath.Contains(".mp4"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                        "Publisher", "Instagram does not allow to post Photo As Part of Reels");
                    return true;
                }
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, SocialNetworks.Instagram, AccountModel.UserName,
                            CampaignName);
                        if (publisherModel.isAlbum)
                        {
                            if (!AccountModel.IsRunProcessThroughBrowser)
                            {
                                uploadMediaResponse = publisherModel.IsImageAlbum
                                                        ? instaFunct.UploadPhotoAlbum(AccountModel, account, CurrentJobCancellationToken.Token, publisherModel.ImageAlbumList, postCaption,
                                                            tagLocation: publisherModel.tagLocationDetails,
                                                            lstTagUserIds: publisherModel.lstTagUserIds.Select(x=>x.Username).ToList())

                                                        : UploadVideoAlbum(publisherModel.lstThumbnailVideo, publisherModel.videoAlbumList,
                                                            fullCaption, tagLocation: publisherModel.tagLocationDetails,
                                                            isStoryVideo: updatesPostListModel.GdPostSettings.IsPostAsStoryPost,
                                                            lstTagUserIds: publisherModel.lstTagUserIds.Select(x => x.Username).ToList());
                            }
                            else
                            {
                                if (publisherModel.IsImageAlbum)
                                {
                                    InstagramPost instagrampost = new InstagramPost();
                                    instagrampost.UserTags = publisherModel.lstTagUserIds;
                                    instagrampost.CommentsDisabled = instagramModel != null ?instagramModel.IsDisableCommentsForNewPost : false;
                                    instagrampost.IsCheckedCropMedia = gdPostSettings != null ? gdPostSettings.IsCheckedCropMedias : false;
                                    instagrampost.CropRatio = gdPostSettings != null ? gdPostSettings.SelectedResolution :string.Empty;
                                    instagrampost.Caption = string.IsNullOrEmpty(fullCaption) ? postCaption : fullCaption;
                                    instagrampost.HasLocation = gdPostSettings.IsGeoLocation || publisherModel.isLocationName || (instagramModel != null && (instagramModel.IsGeoLocationId || instagramModel.IsGeoLocationName));
                                    if (instagrampost.HasLocation)
                                    {
                                        instagrampost.Location = new Location();
                                        instagrampost.Location.Name = publisherModel.geoLocation;
                                        AssignLocationDetails(instagrampost, publisherModel.tagLocationDetails);
                                    }
                                    uploadMediaResponse =
                                    GramStatic.IsBrowser ?
                                    instaFunct.GdBrowserManager.UploadMedia(AccountModel, instagrampost, publisherModel.ImageAlbumList, CurrentJobCancellationToken.Token)
                                    : instaFunct.UploadMedia(AccountModel, instagrampost, publisherModel.ImageAlbumList).Result;
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                                                            "Publisher", "Video album cannot be posted.");
                                }

                            }
                        }
                        else if (!publisherModel.isVideo)
                        {

                            if (publisherModel.isPostAsStory && !AccountModel.IsRunProcessThroughBrowser)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                                    "Publisher",publisherModel.mediaPath.StartsWith("http")? $"Please wait... started to share {publisherModel.mediaPath}"
                                    : "Please wait... started to publish photo story");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                                 "Publisher", publisherModel.mediaPath.StartsWith("http") ? $"Please wait... started to share {publisherModel.mediaPath}":
                                 "Please wait... started to upload photo");
                            }
                            if (!AccountModel.IsRunProcessThroughBrowser)
                            {
                                uploadMediaResponse = publisherModel.isPostAsStory
                                                        ? UploadStoryPhotos(publisherModel.CheckedImagePath, postCaption,
                                                            publisherModel.lstTagUserIds.Select(x => x.Username).ToList(), imagePostion, tagLocation: publisherModel.tagLocationDetails)
                                                        : instaFunct.UploadTimeLinePhoto(AccountModel, account, CurrentJobCancellationToken.Token, publisherModel.mediaPath, publisherModel.lstTagUserIds.Select(x => x.Username).ToList(),
                                                            null, false, fullCaption,
                                                            tagLocation: publisherModel.tagLocationDetails);
                            }
                            else
                            {
                                if (publisherModel.isPostAsStory)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                                    "Publisher", "Sorry! Story process is disabled at the moment");
                                }
                                else
                                {
                                    if (publisherModel.ImageAlbumList.Count == 0)
                                        publisherModel.ImageAlbumList.Add(publisherModel.mediaPath);
                                    InstagramPost instagrampost = new InstagramPost();
                                    instagrampost.UserTags = publisherModel.lstTagUserIds;
                                    instagrampost.Caption = string.IsNullOrEmpty(fullCaption) ? postCaption:fullCaption;
                                    instagrampost.HasLocation = gdPostSettings.IsGeoLocation ||publisherModel.isLocationName || (instagramModel != null && (instagramModel.IsGeoLocationId || instagramModel.IsGeoLocationName));
                                    if (instagrampost.HasLocation)
                                    {
                                        instagrampost.Location = new Location();
                                        instagrampost.Location.Name = publisherModel.geoLocation;
                                        AssignLocationDetails(instagrampost, publisherModel.tagLocationDetails);
                                    }
                                    instagrampost.CommentsDisabled =  instagramModel != null ? instagramModel.IsDisableCommentsForNewPost : false;
                                    instagrampost.IsCheckedCropMedia = gdPostSettings != null ? gdPostSettings.IsCheckedCropMedias : false;
                                    instagrampost.CropRatio = gdPostSettings != null ? gdPostSettings.SelectedResolution : string.Empty;
                                    //     instagrampost.Location.Name = instagrampost.HasLocation ? gdPostSettings.GeoLocationList : string.Empty;
                                    uploadMediaResponse = 
                                    GramStatic.IsBrowser ?
                                    instaFunct.GdBrowserManager.UploadMedia(AccountModel, instagrampost, publisherModel.ImageAlbumList, CurrentJobCancellationToken.Token)
                                    : instaFunct.UploadMedia(AccountModel, instagrampost, publisherModel.ImageAlbumList).Result;
                                }
                            }
                        }
                        else
                        {
                            if (!AccountModel.IsRunProcessThroughBrowser)
                            {
                                uploadMediaResponse = publisherModel.isPostAsReels ? UploadReelsVideos(publisherModel.checkedVideoPath, publisherModel.lstThumbnailVideo,
                                        postCaption,
                                        publisherModel.lstTagUserIds.Select(x => x.Username).ToList(), tagLocation: publisherModel.tagLocationDetails) : publisherModel.isPostAsStory
                                    ? UploadStoryVideos(publisherModel.checkedVideoPath, publisherModel.lstThumbnailVideo,
                                        postCaption,
                                        publisherModel.lstTagUserIds.Select(x => x.Username).ToList(), tagLocation: publisherModel.tagLocationDetails)
                                    : instaFunct.UploadVideo(AccountModel, account, CurrentJobCancellationToken.Token,
                                        !File.Exists(publisherModel.convertedMediaFilePath)
                                            ? publisherModel.mediaPath
                                            : publisherModel.convertedMediaFilePath, publisherModel.thumbnailFilePath,
                                        fullCaption, tagLocation: publisherModel.tagLocationDetails,
                                        lstTagUserIds: publisherModel.lstTagUserIds.Select(x => x.Username).ToList());
                            }
                            else
                            {
                                var mediaList = new List<string>();
                                //if (!File.Exists(PuppeteerBrowserActivity.GetExecutablePath()))
                                //{
                                //    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                                //    "Publisher", "Google Chrome must be intalled in your system. Post cannot be published since it is video.");
                                //    ToasterNotification.ShowError("Google Chrome must be intalled in your system. Post cannot be published since it is video.");
                                //    return;
                                //}
                                InstagramPost instagrampost = new InstagramPost();
                                instagrampost.UserTags = publisherModel.lstTagUserIds;
                                instagrampost.Caption = string.IsNullOrEmpty(fullCaption) ? postCaption : fullCaption;
                                mediaList.Add(publisherModel.mediaPath);
                                instagrampost.CommentsDisabled = instagramModel != null ? instagramModel.IsDisableCommentsForNewPost : false;
                                instagrampost.IsCheckedCropMedia = gdPostSettings.IsCheckedCropMedias;
                                instagrampost.CropRatio = gdPostSettings.SelectedResolution;
                                instagrampost.HasLocation = gdPostSettings.IsGeoLocation || publisherModel.isLocationName || (instagramModel != null && (instagramModel.IsGeoLocationId || instagramModel.IsGeoLocationName));
                                if (instagrampost.HasLocation)
                                {
                                    instagrampost.Location = new Location();
                                    instagrampost.Location.Name = publisherModel.geoLocation;
                                    AssignLocationDetails(instagrampost, publisherModel.tagLocationDetails);
                                }
                                if(instagrampost.Images is null || instagrampost.Images.Count == 0)
                                {
                                    instagrampost.Images = new List<InstaGramImage> { new InstaGramImage {Url = publisherModel?.thumbnailFilePath } };
                                }
                                //PupBrowserRequest pup = new PupBrowserRequest(AccountModel, AccountModel.CancellationSource ?? new CancellationTokenSource());
                                //uploadMediaResponse = pup.UploadVideo(instagrampost, mediaList);
                                uploadMediaResponse = 
                                GramStatic.IsBrowser ?
                                instaFunct.UploadMediaHttp(AccountModel, instagrampost, mediaList, AccountModel.Token).Result
                                : instaFunct.UploadMedia(AccountModel, instagrampost, mediaList).Result;
                            }
                        }
                        
                        if (AccountModel.IsRunProcessThroughBrowser)
                            GdBrowserManager.CloseBrowser();
                        if ((uploadMediaResponse != null) && uploadMediaResponse.Success)
                        {
                            var PostUrl = postDetails.PostSource == PostSource.SharePost ?
                            $"https://www.instagram.com/direct/t/{uploadMediaResponse.Code}/"
                            :publisherModel.isPostAsReels || !publisherModel.isPostAsStory ?
                            $"https://www.instagram.com/p/{uploadMediaResponse.Code}" :
                            $"https://www.instagram.com/stories/{AccountModel.UserName}";
                            if (publisherModel.isPostAsReels)
                            {
                                GlobusLogHelper.log.Info(Log.PublishingSuccessfully, SocialNetworks.Instagram,
                                    AccountModel.UserName, publisherModel.isPostAsStory ? "Reels" : "Own Wall",
                                    AccountModel.UserName);
                                // updatesPostListModel.PostDescription = fullCaption;
                                UpdatePostWithSuccessful(AccountModel.UserName, updatesPostListModel,PostUrl);
                            }
                            else if (!publisherModel.isPostAsStory)
                            {
                                GlobusLogHelper.log.Info(Log.PublishingSuccessfully, SocialNetworks.Instagram,
                                    AccountModel.UserName, publisherModel.isPostAsStory ? "Story" : "Own Wall",
                                    AccountModel.UserName);
                                // updatesPostListModel.PostDescription = fullCaption;
                                UpdatePostWithSuccessful(AccountModel.UserName, updatesPostListModel,PostUrl);


                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.PublishingSuccessfully, SocialNetworks.Instagram,
                                      AccountModel.UserName, "Story", AccountModel.UserName);
                                UpdatePostWithSuccessful(AccountModel.UserName, updatesPostListModel,PostUrl);
                            }

                            // This method is calling for performing post publish activities
                            DoPostPublisherActivities(uploadMediaResponse, instagramModel, gdPostSettings,
                                updatesPostListModel.PostId);
                        }
                        else
                        {
                            if (uploadMediaResponse.Issue != null)
                            {
                                if (uploadMediaResponse.Issue.Error == InstagramError.AspectRatio)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName, "Publisher",
                                    $"Uploaded image isn't in an allowed aspect ratio");
                                }
                            }

                            GlobusLogHelper.log.Info(Log.PublishingFailed, SocialNetworks.Instagram,
                                AccountModel.UserName, publisherModel.isPostAsStory ? "Story" : "Own Wall",
                                AccountModel.UserName);
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Cancellation Requested!");
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.InnerExceptions)
                        {
                            if (e is TaskCanceledException || e is OperationCanceledException)
                                e.DebugLog("Cancellation requested before task completion!");
                            else
                                e.DebugLog(e.StackTrace + e.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }).Wait();

                #endregion

                if (isDelayNeed)
                    DelayBeforeNextPublish();
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private void AssignLocationDetails(InstagramPost instagrampost, string tagLocationDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(tagLocationDetails))
                    return;
                var handler = JsonJArrayHandler.GetInstance;
                var obj = handler.ParseJsonToJObject(tagLocationDetails);
                double.TryParse(handler.GetJTokenValue(obj, "lat"), out double lat);
                double.TryParse(handler.GetJTokenValue(obj, "lng"), out double lng);
                var name = string.IsNullOrEmpty(instagrampost?.Location?.Name) ? handler.GetJTokenValue(obj, "name") : instagrampost?.Location?.Name;
                instagrampost.Location = new Location
                {
                    Name = name,
                    Address = handler.GetJTokenValue(obj,"address"),
                    Id = handler.GetJTokenValue(obj, "facebook_places_id"),
                    Lat = lat,
                    Lng = lng
                };
            }
            catch { }
        }

        public UploadMediaResponse UploadStoryPhotos(ObservableCollection<string> MediaList, string postCaption, List<string> lstTagUserIds, List<ImagePosition> imagePostion, string tagLocation = null)
        {
            InstaFunct instaFunct = new InstaFunct(AccountModel, httpHelper, GdBrowserManager, DelayServices, _dateProvider);
            UploadMediaResponse uploadMediaResponse = null;
            #region when Required
            //instaFunct.SegmentationModels();
            //instaFunct.FaceModels();
            //instaFunct.FaceEffects();
            //instaFunct.CameraModels();
            //instaFunct.CreativeAssest();
            //instaFunct.CreativeAssest();
            #endregion

            foreach (var mediaPath in MediaList)
            {
                string uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                uploadMediaResponse = instaFunct.Story_Upload_IG_Photo(mediaPath, uploadId, lstTagUserIds, imagePostion, tagLocation, MediaList.Count);
            }
            return uploadMediaResponse;
        }

        public UploadMediaResponse UploadStoryVideos(List<string> MediaList, List<string> lstThumnail, string PostCaption,
            List<string> lstTagUserIds, string tagLocation = null)
        {
            InstaFunct instaFunct = new InstaFunct(AccountModel, httpHelper, GdBrowserManager, DelayServices, _dateProvider);
            UploadMediaResponse uploadMediaResponse = null;
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                  "Publisher", "Please wait... started to publish video story");
            #region when required
            //instaFunct.SegmentationModels();
            //instaFunct.FaceModels();
            //instaFunct.FaceEffects();
            //instaFunct.CameraModels();
            //instaFunct.CreativeAssest();
            //instaFunct.CreativeAssest(); 
            #endregion
            for (int mediaPath = 0; mediaPath < MediaList.Count; mediaPath++)
            {
                string uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                string thumbnail = lstThumnail[mediaPath];
                uploadMediaResponse = instaFunct.VideoStory(MediaList[mediaPath], thumbnail, uploadId, lstTagUserIds, tagLocation, MediaList.Count);
            }
            return uploadMediaResponse;
        }

        public UploadMediaResponse UploadReelsVideos(List<string> MediaList, List<string> lstThumnail, string PostCaption,
            List<string> lstTagUserIds, string tagLocation = null)
        {
            InstaFunct instaFunct = new InstaFunct(AccountModel, httpHelper, GdBrowserManager, DelayServices, _dateProvider);
            UploadMediaResponse uploadMediaResponse = null;
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                  "Publisher", "Please wait... started to publish video Reels");
            #region when required
            //instaFunct.SegmentationModels();
            //instaFunct.FaceModels();
            //instaFunct.FaceEffects();
            //instaFunct.CameraModels();
            //instaFunct.CreativeAssest();
            //instaFunct.CreativeAssest(); 
            #endregion
            AccountModel account = new AccountModel(AccountModel);
            IgPublisherModel publisherModel = new IgPublisherModel();
            for (int mediaPath = 0; mediaPath < MediaList.Count; mediaPath++)
            {
                string uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                string thumbnail = lstThumnail[mediaPath];
                var mediaInfo = GramStatic.GetMediaInfo(MediaList[mediaPath]);
                var hashCode = uploadId.GetHashCode();

                uploadMediaResponse = instaFunct.IgVideo_ReelsClips_Assets(AccountModel, account,
         uploadId, mediaInfo, CurrentJobCancellationToken.Token, MediaList[mediaPath],
         thumbnail, hashCode, PostCaption, tagLocation = null,
        lstTagUserIds = null);
                if (uploadMediaResponse != null && uploadMediaResponse.Success)
                    uploadMediaResponse = instaFunct.IgUploading_Reels_Video(AccountModel, account, CurrentJobCancellationToken.Token, MediaList[mediaPath],
                        thumbnail, uploadId, mediaInfo, false, "");

                if (uploadMediaResponse != null && uploadMediaResponse.Success)
                    uploadMediaResponse = instaFunct.igphoto_uploadingThumnail(AccountModel, account, CurrentJobCancellationToken.Token,
                        thumbnail, uploadId, mediaInfo, MediaList[mediaPath], false, PostCaption, tagLocation,
                        lstTagUserIds);

                if (uploadMediaResponse != null && uploadMediaResponse.Success)
                    uploadMediaResponse = instaFunct.Configure_Reels_Video(AccountModel, account, CurrentJobCancellationToken.Token, MediaList[mediaPath],
                        thumbnail, uploadId, PostCaption, tagLocation, lstTagUserIds);
            }
            return uploadMediaResponse;
        }

        private List<InstagramUser> ManageInstaIndividualSettings(GdPostSettings gdPostSettings,bool IsBrowser=true)
        {
            #region Tag Users

            var lstTagUserIds = new List<InstagramUser>();
            var lstTagusers = new List<string>();
            if (gdPostSettings.IsTagUser || gdPostSettings.IsMentionUser)
            {
                try
                {
                    if (gdPostSettings.IsTagUser)
                    {
                        lstTagusers = Regex.Split(gdPostSettings.TagUserList?.Replace(",","\n"), "\n")
                        .Where(user => !string.IsNullOrEmpty(user)).Select(user => user.Trim()).ToList();
                    }
                    else
                    {
                        lstTagusers = Regex.Split(gdPostSettings.MentionUserList, "\n")
                        .Where(user => !string.IsNullOrEmpty(user)).Select(user => user.Trim()).ToList();
                    }


                    foreach (var tagUser in lstTagusers)
                    {
                        //UsernameInfoIgResponseHandler userInfo = null;
                        //if (!AccountModel.IsRunProcessThroughBrowser)
                        //{
                        //    userInfo = instaFunct.SearchUsername(AccountModel, tagUser, CurrentJobCancellationToken.Token); 
                        //}
                        //else
                        //{
                        //    userInfo = instaFunct.GdBrowserManager.GetUserInfo(AccountModel, tagUser, CurrentJobCancellationToken.Token);
                        //}
                        var userInfo = instaFunct.SearchUsername(AccountModel, tagUser, CurrentJobCancellationToken.Token);
                        if (userInfo != null && userInfo.ToString().Contains("User not found"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                                "Publisher",
                                $"This user name {tagUser} is not available so it will not add in your tag feature");
                            continue;
                        }
                        if(userInfo !=null && !string.IsNullOrEmpty(userInfo.Username) && !lstTagUserIds.Any(x=>x?.Username == userInfo?.Username))
                            lstTagUserIds.Add(userInfo);
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return lstTagUserIds;

            #endregion
        }

        private string CombineFilteration(DominatorAccountModel AccountModel, GdPostSettings gdPostSettings,
            string geoLocation, string tagLocationDetails, bool IsLocationName = false)
        {
            try
            {
                #region Geo location finding              
                List<string> lst_Location;
                if (gdPostSettings.IsGeoLocation && !string.IsNullOrEmpty(gdPostSettings.GeoLocationList))
                {
                    geoLocation = gdPostSettings.GeoLocationList;
                    if (gdPostSettings.IsGeoLocationName)
                    {
                        if (geoLocation.Contains("\n"))
                        {
                            lst_Location = geoLocation.Split('\n').ToList();
                            geoLocation = lst_Location.GetRandomItem();
                        }

                        IsLocationName = true;
                    }
                    else
                    {
                        if (geoLocation.Contains("\n"))
                        {
                            lst_Location = geoLocation.Split('\n').ToList();
                            geoLocation = lst_Location.GetRandomItem();
                        }

                        IsLocationName = false;
                    }
                }

                if (!string.IsNullOrEmpty(geoLocation))
                {
                    if (IsLocationName)
                    {
                        LocationIgReponseHandler locationSearchResponse =
                            instaFunct.SearchForLocation(AccountModel, geoLocation.Trim(), true).Result;
                        if (locationSearchResponse.ToString().Contains("\"venues\": []"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                        "Publisher",
                        $"This Location name {geoLocation} is not available so it will not post with location");
                        }
                        else
                        {
                            JObject jObjectLocation = new JObject
                            {
                                ["name"] = geoLocation,
                                ["address"] = string.Empty,
                                ["lat"] = locationSearchResponse.Latitude,
                                ["lng"] = locationSearchResponse.Longitude,
                                ["external_source"] = locationSearchResponse.ExternalSource,
                                ["facebook_places_id"] = locationSearchResponse.FacebookPlaceId
                            };
                            tagLocationDetails = JsonConvert.SerializeObject(jObjectLocation);
                        }
                    }
                    else
                    {
                        LocationIdIgReponseHandler locationIdIgReponseHandler =
                            instaFunct.SearchLocationId(AccountModel, geoLocation.Trim());
                        if (locationIdIgReponseHandler.ToString().Contains("\"items\": [], \"status\": \"ok\""))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                        "Publisher",
                        $"This Location id {geoLocation} is not available so it will not post with location");
                        }
                        else
                        {
                            JObject jObjectLocation = new JObject
                            {
                                ["name"] = locationIdIgReponseHandler.Name,
                                ["address"] = locationIdIgReponseHandler.Address ?? string.Empty,
                                ["lat"] = locationIdIgReponseHandler.Latitude,
                                ["lng"] = locationIdIgReponseHandler.Longitude,
                                ["external_source"] = locationIdIgReponseHandler.ExternalSource,
                                ["facebook_places_id"] = locationIdIgReponseHandler.FacebookPlaceId
                            };
                            tagLocationDetails = JsonConvert.SerializeObject(jObjectLocation);
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return tagLocationDetails;
        }

        private InstagramModel ManageAdvancedSettings(string postCaption, ref string fullCaption,
            ObservableCollection<string> MediaList, ref bool isLocationName, IgPublisherModel publisherModel)
        {
            InstagramModel instagramModel = new InstagramModel();

            try
            {
                instagramModel = GenericFileManager.GetModuleDetails<InstagramModel>
                        (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram))
                    .FirstOrDefault(x => x.CampaignId == CampaignId);

                if (instagramModel != null)
                {
                    // Post using Geo Location

                    #region Post using Geo Location

                    try
                    {
                        if (instagramModel.IsPostusingGeoLocation &&
                            !string.IsNullOrEmpty(instagramModel.GeoLocation))
                        {
                            publisherModel.geoLocation = instagramModel.GeoLocation;
                            var LocationList = instagramModel.GeoLocation.Split('\n');
                            string Location = LocationList.GetRandomItem();
                            instagramModel.GeoLocation = Location.Trim('\r');
                        }
                        isLocationName = instagramModel.IsGeoLocationName;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion


                    // Enable Automatic Hashtags

                    #region Enable Automatic Hashtags

                    if (instagramModel.IsEnableAutomaticHashTags)
                    {
                        try
                        {
                            List<string> lstHashKeywords = new List<string>();

                            if (!string.IsNullOrEmpty(instagramModel.HashWords))
                            {
                                lstHashKeywords = Regex.Split(instagramModel?.HashWords?.Replace("\r\n","\n"), "\n")
                                    .Where(x => !string.IsNullOrEmpty(x)).ToList();
                            }

                            lstHashKeywords.Shuffle();

                            for (int hashtagAddCount = 0; hashtagAddCount < lstHashKeywords.Count; hashtagAddCount++)
                            {
                                if (hashtagAddCount >= instagramModel.MaxHashtagsPerPost)
                                    break;
                                var cap = lstHashKeywords[hashtagAddCount].Trim();
                                if (!string.IsNullOrEmpty(cap))
                                {
                                    fullCaption += cap.StartsWith("#")?cap: $"#{cap}";
                                }
                            }


                            if (lstHashKeywords.Count < instagramModel.MaxHashtagsPerPost)
                            {
                                string postCaptionSpecialCharactersFree =
                                    GdUtilities.RemoveSpecialCharacters(postCaption);
                                var lstHashFromCaptionByWordLength = Regex
                                    .Split(postCaptionSpecialCharactersFree.Replace("\r\n", " "), " ")
                                    .Where(x => IsAlphaNumeric(x) && x.Length >= instagramModel.MinimumWordLength)
                                    .ToList();

                                lstHashFromCaptionByWordLength.Shuffle();

                                int hashtagCountToAdd =
                                    instagramModel.MaxHashtagsPerPost - lstHashKeywords.Count;

                                for (int hashtagAddCount = 0; hashtagAddCount < hashtagCountToAdd; hashtagAddCount++)
                                {
                                    fullCaption += $"#{lstHashFromCaptionByWordLength[hashtagAddCount].Trim()}";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    #endregion


                    // Enable Dynamic Hashtag

                    #region Enable Dynamic Hashtags

                    if (instagramModel.IsEnableDynamicHashTags &&
                        instagramModel.IsAddHashTagEvenIfAlreadyHastags)
                    {
                        try
                        {
                            int maxHashtagRangePerPost = instagramModel.MaxHashtagsPerPostRange.GetRandom();

                            var hashTagsPresentInCaption = Regex.Split(fullCaption, "#")
                                .Where(x => !string.IsNullOrEmpty(Utilities.GetBetween($"{x}##", "", "##")
                                    .Trim()))
                                .ToList();

                            if (!string.IsNullOrEmpty(fullCaption) && !fullCaption.StartsWith("#"))
                            {
                                hashTagsPresentInCaption.RemoveAt(0);
                            }

                            int presentHashtagCount = hashTagsPresentInCaption.Count;

                            int hashtagCountToAdd = (presentHashtagCount < maxHashtagRangePerPost)
                                ? maxHashtagRangePerPost - presentHashtagCount
                                : 0;

                            if (hashtagCountToAdd != 0)
                            {
                                #region Hashtags from List1

                                int percentCountFromHashtag =
                                    hashtagCountToAdd * instagramModel.PickPercentHashTag / 100;
                                List<string> lstHashTagFromList1 = Regex
                                    .Split(instagramModel.HashtagsFromList1, "\n")
                                    .Where(x => !string.IsNullOrEmpty(x)).ToList();

                                for (int hashtagcount = 0;
                                    hashtagcount < percentCountFromHashtag;
                                    hashtagcount++)
                                {
                                    fullCaption += $"#{lstHashTagFromList1[hashtagcount].Trim()}";
                                }

                                #endregion

                                #region Hashtags from List2

                                int percentCountFromList = hashtagCountToAdd - percentCountFromHashtag;
                                List<string> lstHashTagFromList2 = Regex
                                    .Split(instagramModel.HashtagsFromList2, "\n")
                                    .Where(x => !string.IsNullOrEmpty(x)).ToList();

                                for (int hashtagCount = 0; hashtagCount < percentCountFromList; hashtagCount++)
                                {
                                    fullCaption += $"#{lstHashTagFromList2[hashtagCount].Trim()}";
                                }

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    #endregion


                    // Enable Dynamic mentions

                    #region Enable Dynamic mentions

                    if (instagramModel.IsEnableDynamicMentions)
                    {
                        try
                        {
                            var mentionedUsers = Regex.Split(instagramModel.UserNamesSeparatedByComma, ",")
                                .Where(x => !string.IsNullOrEmpty(x)).Select(y => y.Replace("\r\n", "")).ToList();
                            List<string> lstMentionedusers = new List<string>();

                            for (int mentionUserCount = 0;
                                mentionUserCount < instagramModel.NumberOfUsersToMention;
                                mentionUserCount++)
                            {
                                lstMentionedusers.Add(mentionedUsers[mentionUserCount].Trim());
                                // " "(space) is very important here for mentioning users. 
                                // It only differentiates between a user and normal text.
                                fullCaption += $" @{mentionedUsers[mentionUserCount].Trim()}";
                            }

                            // Delete users from mentioned List

                            #region Delete users from mentioned List

                            if (instagramModel.IsDeleteUsersFromList)
                            {
                                lstMentionedusers.ForEach(x =>
                                {
                                    instagramModel.UserNamesSeparatedByComma =
                                        instagramModel.UserNamesSeparatedByComma.Replace(x, "");
                                });

                                instagramModel.UserNamesSeparatedByComma =
                                    instagramModel.UserNamesSeparatedByComma.Trim(',');

                                var lstInstagramModel = GenericFileManager.GetModuleDetails<InstagramModel>(
                                    ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram));

                                int campaignIndex = lstInstagramModel.IndexOf(
                                    lstInstagramModel.FirstOrDefault(x => x.CampaignId == CampaignId));
                                lstInstagramModel[campaignIndex] = instagramModel;

                                GenericFileManager.UpdateModuleDetails(lstInstagramModel,
                                    ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram));
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    #endregion


                    // Retry Video Posts
                    // TODO : Retry Video Posts

                    #region Retry Video Posts

                    try
                    {
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion


                    // Post as Album
                    // TODO : Post as Album

                    #region Post as Album

                    try
                    {
                        if (instagramModel.IsPostMultipleImagesVideoPostsAsAlbum)
                        {
                            if (MediaList.Count > 1 && MediaList.Count <= 10)
                            {
                                publisherModel.isAlbum = true;
                                foreach (string image in MediaList)
                                {
                                    if (image.Contains(".jpg") || image.Contains(".png") || image.Contains(".jpeg"))
                                    {
                                        publisherModel.ImageAlbumList.Add(image);
                                        publisherModel.IsImageAlbum = true;
                                    }
                                    else
                                    {
                                        publisherModel.videoAlbumList.Add(image);
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return instagramModel;
        }

        private bool SetThumbnailAndVideoFormats(string mediaPath, IgPublisherModel publisherModel, PublisherPostlistModel updatesPostListModel, ref string newMediaPath)
        {

            try
            {

                publisherModel.thumbnailFilePath = String.Empty;
                publisherModel.thumbnailFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)}.jpg";
                var convertedMediaFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)} Compressed.mp4";

                //FFProbe ffProbe = new FFProbe();
                var mediaInfos = Utility.GramStatic.GetMediaInfo(mediaPath);
                FFMpegConverter ffMpegConverter = new FFMpegConverter();
                ffMpegConverter.FFMpegToolPath = ConstantVariable.GetOtherDir();
                //ConvertSettings convertSetting = new ConvertSettings();
                //try
                //{
                //    //var mediaInfos = ffProbe.GetMediaInfo(mediaPath);
                //    int width = mediaInfos.Streams[0].Width;
                //    int height = mediaInfos.Streams[0].Height;
                //    float videoRatio = (float)width / (float)height;
                //    if (videoRatio < 1)
                //    {
                //        string NewPath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)} CompressedConvert.mp4";
                //        //convertSetting.SetVideoFrameSize(width, width);
                //        ffMpegConverter.ConvertMedia(mediaPath, null, NewPath, "mp4", convertSetting);
                //        convertedMediaFilePath = NewPath;
                //        publisherModel.thumbnailFilePath = $@"{Path.GetDirectoryName(NewPath)}\{Path.GetFileNameWithoutExtension(NewPath)}.jpg";

                //    }
                //}
                //catch (Exception ex)
                //{
                //    ex.DebugLog();
                //}

                // var mediaInfo = ffProbe.GetMediaInfo(mediaPath);
                //if (!updatesPostListModel.GdPostSettings.IsReelPost && mediaInfos.Duration.TotalSeconds > 31)
                //{
                //    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                //        "Publisher",
                //        $"Instagram only allows to upload video size max: 30 secs. Got size {mediaInfos.Duration.TotalSeconds} secs");
                //    return false;
                //}
                //if (!updatesPostListModel.GdPostSettings.IsPostAsStoryPost && mediaInfos.Duration.TotalSeconds > 61)
                //{
                //    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                //        "Publisher",
                //        $"Instagram only allows to upload video size max: 60 secs. Got size {mediaInfos.Duration.TotalSeconds} secs");
                //    return false;
                //}
                //if (updatesPostListModel.GdPostSettings.IsPostAsStoryPost && mediaInfos.Duration.TotalSeconds > 15)
                //{
                //    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                //        "Publisher",
                //        $"Instagram only allows to upload video size max: 15 secs. Got size {mediaInfos.Duration.TotalSeconds} secs");
                //    return false;
                //}
                ffMpegConverter.GetVideoThumbnail(
                    (!File.Exists(convertedMediaFilePath) ? mediaPath : convertedMediaFilePath),
                    publisherModel.thumbnailFilePath, 3);
                if (!File.Exists(convertedMediaFilePath))
                    newMediaPath = mediaPath;
                else
                    newMediaPath = convertedMediaFilePath;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }


        private bool SetThumbnail(string mediaPath, IgPublisherModel publisherModel, PublisherPostlistModel updatesPostListModel, ref string newMediaPath)
        {

            try
            {
                publisherModel.thumbnailFilePath = String.Empty;
                publisherModel.thumbnailFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)}.jpg";
                ////var convertedMediaFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)} Compressed.mp4";

                ////FFProbe ffProbe = new FFProbe();
                //var mediaInfos = Utility.GramStatic.GetMediaInfo(mediaPath);
                FFMpegConverter ffMpegConverter = new FFMpegConverter();
                ffMpegConverter.FFMpegToolPath = ConstantVariable.GetOtherDir();
                //ConvertSettings convertSetting = new ConvertSettings();
                //try
                //{
                //    //var mediaInfos = ffProbe.GetMediaInfo(mediaPath);
                //    int width = mediaInfos.Streams[0].Width;
                //    int height = mediaInfos.Streams[0].Height;
                //    float videoRatio = (float)width / (float)height;
                //    if (videoRatio < 0.5)
                //    {
                //        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                //        "Publisher",
                //        $"Asspect Ratio Of Video Is Not Valid ");
                //        return false;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    ex.DebugLog();
                //}
                //if (!updatesPostListModel.GdPostSettings.IsReelPost && mediaInfos.Duration.TotalSeconds > 31)
                //{
                //    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                //        "Publisher",
                //        $"Instagram only allows to upload video size max: 30 secs. Got size {mediaInfos.Duration.TotalSeconds} secs");
                //    return false;
                //}

                ffMpegConverter.GetVideoThumbnail(
                    (!File.Exists(mediaPath) ? mediaPath : mediaPath),
                    publisherModel.thumbnailFilePath, 3);
                if (!File.Exists(mediaPath))
                    newMediaPath = mediaPath;
                else
                    newMediaPath = mediaPath;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
        private void DoPostPublisherActivities(UploadMediaResponse uploadMediaResponse, InstagramModel instagramModel,
            GdPostSettings gdPostSettings, string currentPostId)
        {
            // Set deletion job after posting
            //  InstaFunct instaFunct = new InstaFunct(AccountModel, httpHelper, GdBrowserManager, DelayServices, _dateProvider);
            #region Set deletion job after posting

            if (gdPostSettings.IsDeletePostAfterHours)
            {
                try
                {
                    int deletionIntervalTimeInHour = gdPostSettings.DeletePostAfterHours;
                    DateTime deletionTime = DateTime.Now.AddHours(deletionIntervalTimeInHour);

                    PostDeletionModel postDeletionModel = new PostDeletionModel
                    {
                        AccountId = AccountModel.AccountId,
                        CampaignId = CampaignId,
                        DeletionTime = deletionTime,
                        DestinationType = "OwnWall",
                        DestinationUrl = AccountModel.UserName,
                        Networks = SocialNetworks.Instagram,
                        PostId = currentPostId,
                        PublishedIdOrUrl = $"https://www.instagram.com/p/{uploadMediaResponse.Code}"
                    };

                    PublishScheduler.EnableDeletePost(postDeletionModel);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            #endregion
        }

        private bool IsAlphaNumeric(string word)
        {
            return new Regex("^[a-zA-Z0-9]*$").IsMatch(word);
        }

        private string GetRssFeedMedia(ref string postCaption, PublisherPostlistModel postDetails)
        {
            string Description = string.Empty;
            Description = postCaption;
            string path = string.Empty;
            string mediaUrl = string.Empty;
            var image = string.Empty;
            string title = string.Empty;
            title = postDetails.PublisherInstagramTitle;
            for (int i = 0; i < postDetails.MediaList.Count; i++)
            {
                image = postDetails.MediaList[i];

            }
            postCaption = postDetails.PostDescription + " src=" + image;
            mediaUrl = image;
            if (!string.IsNullOrEmpty(postCaption))
            {
                if (postCaption.Contains("wp-content") || postCaption.Contains("wp-post"))
                {
                    mediaUrl = Utilities.GetBetween(postCaption, "src=\"", "\" class");
                    path = mediaUrl;
                    title = Utilities.GetBetween(postCaption, "", "<p");
                }
                else if ((postCaption.Contains(".cms") && postCaption.Contains("photo")) ||
                         postCaption.Contains(".png") || postCaption.Contains(".jpg"))
                {
                    //mediaUrl = Utilities.GetBetween(postCaption, "src=\"", "\">");
                    WebClient webclient = new WebClient();
                    string tempFolderPath =
                        $"{Path.GetTempPath()}{ConstantVariable.ApplicationName}\\{postDetails.PostId}";
                    DirectoryUtilities.CreateDirectory(tempFolderPath);
                    string tempMediaPath = $"{tempFolderPath}\\link1.jpg";
                    webclient.DownloadFile(mediaUrl, tempMediaPath);
                    path = tempMediaPath;
                    //title = Utilities.GetBetween(postCaption, "", "<a");
                }
            }

            postCaption = title + ". " + Description;//+ postDetails.ShareUrl;
            if (string.IsNullOrEmpty(path))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                    "Publisher",
                    $"Image is not available in this feeds description");
            }

            return path;
        }

        #region Upload Video Album Process

        public UploadMediaResponse UploadVideoAlbum(List<string> lstThumbnail, List<string> videoFilePath, string caption = "", string tagLocation = null, bool isStoryVideo = false, List<string> lstTagUserIds = null)
        {
            AccountModel accountModel = new AccountModel(AccountModel);
            UploadMediaResponse uploadMediaResponse = null;
            try
            {
                bool isVideoAlbum = true;
                List<ImageDetails> lstimageDetails = new List<ImageDetails>();

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                    "Publisher", "Please wait... started to publish video alubm");
                for (int iterationCount = 0; iterationCount < videoFilePath.Count; iterationCount++)
                {
                    ImageDetails imageDetails = new ImageDetails();
                    string thumbnail = lstThumbnail[iterationCount];
                    string VideoPath = videoFilePath[iterationCount];
                    var mediaInfo = Utility.GramStatic.GetMediaInfo(VideoPath);
                    Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(2, 4)));
                    string uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds)
                        .ToString();

                    if (!instaFunct.IgVideo_uploadingVideo(AccountModel, accountModel,
                            CurrentJobCancellationToken.Token, VideoPath, thumbnail, uploadId, mediaInfo, isVideoAlbum,
                            "")
                        .Success)
                        return null;

                    if (!instaFunct.igphoto_uploadingThumnail(AccountModel, accountModel,
                        CurrentJobCancellationToken.Token, thumbnail, uploadId, mediaInfo, VideoPath, isVideoAlbum,
                        caption, tagLocation, lstTagUserIds).Success)
                        return null;

                    imageDetails.imageUploadId = uploadId;
                    imageDetails.imageLength = mediaInfo.Duration.ToString();
                    imageDetails.imageHeight = mediaInfo.Streams[0].Height.ToString();
                    imageDetails.imageWidh = mediaInfo.Streams[0].Width.ToString();
                    lstimageDetails.Add(imageDetails);
                }

                uploadMediaResponse =
                    instaFunct.ConfigurevideoAlbum(lstimageDetails, caption, tagLocation, lstTagUserIds);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return uploadMediaResponse;
        }

        public bool CheckLogin()
        {
            if (AccountModel.IsRunProcessThroughBrowser)
                AccountModel.IsUserLoggedIn = false;
            if (!AccountModel.IsUserLoggedIn || AccountModel.Cookies.Count < 10)
            {
                Task.Factory.StartNew(() =>
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                        "Publisher", "Attempt to login");

                    //LogInProcess logInProcess = new LogInProcess(httpHelper);
                    if (!AccountModel.IsRunProcessThroughBrowser)
                    {
                        _loginProcess.LoginWithAlternativeMethod(AccountModel, AccountModel.Token);

                    }
                    else
                    {
                        _loginProcess.CheckLoginAsync(AccountModel, AccountModel.Token).Wait();
                        GdBrowserManager = _loginProcess.GdBrowserManager;
                    }
                }).Wait();
                while (!AccountModel.IsUserLoggedIn)
                    Thread.Sleep(TimeSpan.FromSeconds(20));
            }

            if (!AccountModel.IsUserLoggedIn)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                    "Publisher", "User not able to logged in");
                return false;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, AccountModel.UserName,
                    "Publisher", "Logged In Successfully");
                if (httpHelper.GetRequestParameter().Cookies == null)
                {
                    httpHelper = _accountScopeFactory[AccountModel.AccountId].Resolve<IGdHttpHelper>();
                }
                return true;
            }
        }


        #endregion
    }
    public class ImageDetails
    {
        public string imageUploadId { get; set; }
        public string imageWidh { get; set; }
        public string imageHeight { get; set; }
        public string imageLength { get; set; }
    }
}
