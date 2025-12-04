using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using PinDominatorCore.Request;
using PinDominatorCore.Utility;
using Unity;
using PinDominatorCore.Response;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using System.IO;
using DominatorHouseCore.Enums.SocioPublisher;
using System.Drawing;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.PDLibrary
{
    public class PdPublisherJobProcess : PublisherJobProcess
    {
        public static Dictionary<string, int> CurrentRunning = new Dictionary<string, int>();
        private readonly IPdHttpHelper _httpHelper = InstanceProvider.GetInstance<IPdHttpHelper>();
        public static readonly object Lock = new object();
        private PinterestModel AdvanceModel { get; }

        public PdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            var advanceModel = GenericFileManager.GetModuleDetails<PinterestModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            AdvanceModel = advanceModel;

        }

        public PdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            var advanceModel = GenericFileManager.GetModuleDetails<PinterestModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest))
                .FirstOrDefault(x => x.CampaignId == campaignId);

            AdvanceModel = advanceModel;
        }

        public override bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            return postDetails != null && base.PublishOnGroups(accountId, groupUrl, postDetails, isDelayNeed);
        }

        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            return postDetails != null && base.PublishOnOwnWall(accountId, postDetails, isDelayNeed);
        }

        public override bool PublishOnPages(string accountId, string boardUrl, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            IAccountScopeFactory accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            IPdBrowserManager browserManager = accountScopeFactory[$"{accountId}_{postDetails.CampaignId}_Publisher"].Resolve<IPdBrowserManager>();

            try
            {
                DominatorAccountModel objDominatorAccountModel = null;
                try
                {
                    var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                    objDominatorAccountModel = accountsFileManager.GetAccountById(accountId);

                    Thread.CurrentThread.Name = objDominatorAccountModel.AccountBaseModel.UserName + Guid.NewGuid();
                }
                catch
                {
                    // ignored
                }

                if (string.IsNullOrEmpty(boardUrl))
                    return false;

                var updatedPostList = PerformGeneralSettings(postDetails);
                ReplaceFileNameAsDescription(updatedPostList, postDetails);
                if (!string.IsNullOrEmpty(updatedPostList.PublisherInstagramTitle) &&
                    updatedPostList.PublisherInstagramTitle.Length > 99)
                    updatedPostList.PublisherInstagramTitle = updatedPostList.PublisherInstagramTitle.Substring(0, 99);

                if (AdvanceModel?.IsEnableCampaignSourceURL ?? false)
                {
                    var lstPdSourceUrl = Regex.Split(AdvanceModel.SourceURL, "\r\n").ToList();
                    lstPdSourceUrl.Shuffle();
                    if (AdvanceModel.IsOverwritePostSourceURL || string.IsNullOrEmpty(updatedPostList.PdSourceUrl))
                        updatedPostList.PdSourceUrl = lstPdSourceUrl.GetRandomItem();
                }

                updatedPostList.PostDescription = updatedPostList.PostDescription ?? string.Empty;
                updatedPostList.PostDescription =
                    GetPostDescriptionWithHashTag(updatedPostList.PostDescription, AdvanceModel);
;
                lock (Lock)
                {
                    if (postDetails.PostSource == PostSource.RssFeedPost)
                    {
                        //changes image file type as cms to jpg
                        ChangeRSSFeedMediaExtension(updatedPostList);
                        //pinterest not allow rss feed url
                        RemoveUrlFromDescription(updatedPostList);
                    }
                    var pinFunct = accountScopeFactory[$"{accountId}_Publisher"].Resolve<IPinFunction>();
                    var objLoginProcess = accountScopeFactory[$"{accountId}_Publisher"].Resolve<IPdLogInProcess>();

                    if (objDominatorAccountModel != null && objDominatorAccountModel.IsRunProcessThroughBrowser)
                        objLoginProcess.BrowserManager = accountScopeFactory[$"{accountId}_{postDetails.CampaignId}_Publisher"].Resolve<IPdBrowserManager>();

                    bool isLoggedIn = objLoginProcess.CheckLoginSync(objDominatorAccountModel, objDominatorAccountModel.Token);
                    if (!isLoggedIn) return false;
                    var PostSuccess = false;
                    if (updatedPostList.ListOfSections?.Count > 0)
                        if(updatedPostList.ListOfSections.Any(x=>x.BoardUrl.Contains(boardUrl)))
                            updatedPostList.ListOfSections.ForEach(sectionDetails =>
                        {
                            PostSuccess=PublishOnBoard(updatedPostList, postDetails,boardUrl, objDominatorAccountModel,accountId,accountScopeFactory,pinFunct,browserManager, sectionDetails);
                            if (sectionDetails.SectionTitle != updatedPostList.ListOfSections.LastOrDefault().SectionTitle)
                            {
                                var delayAfterPublish = RandomUtilties.GetRandomNumber(30,20);
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post,$"Next Publish On Section Will Start After {delayAfterPublish} Seconds");
                                Thread.Sleep(delayAfterPublish*1000);
                            }
                        });
                        else
                            PostSuccess = PublishOnBoard(updatedPostList, postDetails, boardUrl, objDominatorAccountModel, accountId, accountScopeFactory, pinFunct, browserManager);
                    else
                        PostSuccess=PublishOnBoard(updatedPostList,postDetails,boardUrl,objDominatorAccountModel,accountId, accountScopeFactory, pinFunct, browserManager);
                    if(isDelayNeed)
                        DelayBeforeNextPublish();
                    return PostSuccess;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                browserManager.CloseLast();
            }
        }
        private void ReplaceFileNameAsDescription(PublisherPostlistModel updatedPostDetails, PublisherPostlistModel postDetails)
        {
            if (updatedPostDetails.MediaList.Count > 0 && ((updatedPostDetails.scrapePostModel.IsScrapeGoogleImgaes && updatedPostDetails.scrapePostModel.IsUseFileNameAsDescription) || (updatedPostDetails.PostDetailModel.IsUploadMultipleImage && updatedPostDetails.PostDetailModel.IsUseFileNameAsDescription)))
                updatedPostDetails.PostDescription = new FileInfo(updatedPostDetails.MediaList.FirstOrDefault()).Name;
            else
                updatedPostDetails.PostDescription = postDetails.PostDescription;
        }
        private bool PublishOnBoard(PublisherPostlistModel updatedPostList, PublisherPostlistModel postDetails, string boardUrl, DominatorAccountModel objDominatorAccountModel, string accountId, IAccountScopeFactory accountScopeFactory,IPinFunction pinFunct, IPdBrowserManager browserManager, SectionDetails sectionDetails = null)
        {
            var TryMessage = string.Empty;
            var SuccessMessage = string.Empty;
            if (sectionDetails != null)
            {
                TryMessage = string.Format("LangKeyTryingToPostOnBoardSection".FromResourceDictionary(), boardUrl, sectionDetails.SectionTitle);
                SuccessMessage = sectionDetails.SectionTitle;
            }
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post,string.IsNullOrEmpty(TryMessage)?string.Format("LangKeyTryingToPostOnBoard".FromResourceDictionary(), boardUrl):TryMessage);

            RepostPinResponseHandler objRepostPinResponseHandler;
            var mediaType = string.Empty;
            var NotAMedia = updatedPostList.MediaList.Count == 0 && !string.IsNullOrEmpty(updatedPostList.ShareUrl);
            if (NotAMedia)
            {
                mediaType = "custom Pin";
            }
            else
            {
                mediaType = MediaUtilites.GetMimeTypeByFilePath(updatedPostList.MediaList.GetRandomItem());
            }
            if (objDominatorAccountModel.IsRunProcessThroughBrowser && !mediaType.Contains("video"))
            {
                browserManager = accountScopeFactory[$"{accountId}_{postDetails.CampaignId}_Publisher"].Resolve<IPdBrowserManager>();
                objRepostPinResponseHandler = browserManager.Post(objDominatorAccountModel, boardUrl, updatedPostList,
                    objDominatorAccountModel.CancellationSource,sectionDetails);
            }
            else
            {
                if (_httpHelper.GetRequestParameter().Cookies == null || _httpHelper.GetRequestParameter().Cookies.Count == 0)
                    pinFunct.HttpHelper.GetRequestParameter().Cookies = objDominatorAccountModel.Cookies;

                if (string.IsNullOrEmpty(pinFunct.Domain))
                {
                    pinFunct.Domain = pinFunct.HttpHelper.GetRequestParameter()?.Cookies["csrftoken"]?.Domain;
                    pinFunct.Domain = pinFunct.Domain != null && pinFunct.Domain[0].Equals('.') ? pinFunct.Domain.Remove(0, 1) : pinFunct.Domain;
                }
                if (NotAMedia)
                    objRepostPinResponseHandler = pinFunct.RepostPin(updatedPostList.ShareUrl, boardUrl, objDominatorAccountModel, updatedPostList.LstPublishedPostDetailsModels.First().DestinationUrl, updatedPostList.PostSource.ToString(), objDominatorAccountModel.CancellationSource,sectionDetails);
                else
                    objRepostPinResponseHandler = pinFunct.PostPin(objDominatorAccountModel, boardUrl, updatedPostList,sectionDetails);
            }
            if (postDetails.PostSource == PostSource.ScrapedPost || postDetails.PostSource == PostSource.SharePost)
            {
                try
                {
                    var downloadedScrapePostPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}" +
                        $"\\Socinator\\ScrapePins\\{postDetails.FetchedPostIdOrUrl}{objDominatorAccountModel.AccountBaseModel.AccountId}";

                    downloadedScrapePostPath = downloadedScrapePostPath + ".jpg";
                    if (File.Exists(downloadedScrapePostPath))
                        File.Delete(downloadedScrapePostPath);
                }
                catch
                {
                    // ignored
                }
            }
            var ErrorMessage = objRepostPinResponseHandler?.Issue?.Message;
            ErrorMessage = !string.IsNullOrEmpty(ErrorMessage) && ErrorMessage.Contains("Something went wrong") ? PdConstants.TooSmallImage1 : ErrorMessage;
            var IsPostSuccess = objRepostPinResponseHandler != null && objRepostPinResponseHandler.Success;
            if (IsPostSuccess)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post,string.IsNullOrEmpty(SuccessMessage)?
                    string.Format("LangKeySuccessfullyPostedPinIdOnBoard".FromResourceDictionary(),
                    objRepostPinResponseHandler.PinId, boardUrl):string.Format("LangKeySuccessfullyPostedPinIdOnBoardSection".FromResourceDictionary(),objRepostPinResponseHandler.PinId,boardUrl,SuccessMessage));
                var postUrl = $"https://{pinFunct.Domain}/pin/" + objRepostPinResponseHandler.PinId + "/";
                UpdatePostWithSuccessful(boardUrl, updatedPostList, postUrl);
                goto CheckSuccess;
            }
            if (objRepostPinResponseHandler == null || objRepostPinResponseHandler?.Issue == null)
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post,
                   String.Format("LangKeyFailedToPostPinOnBoard".FromResourceDictionary(), boardUrl));
            else
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post,
                    String.Format("LangKeyfailedToPostPinOnBoardWithError".FromResourceDictionary(), boardUrl,
                    ErrorMessage + $"==>{{{updatedPostList.MediaList.First()}}}"));
            CheckSuccess:
            if (IsPostSuccess || !browserManager.IsValidImage(ErrorMessage).status)
                return true;
            else
                return false;
        }

        private void RemoveUrlFromDescription(PublisherPostlistModel updatedPostList)
        {
            try
            {
                if (!string.IsNullOrEmpty(updatedPostList.PostDescription)&& updatedPostList.PostDescription.Contains("http"))
                {
                    updatedPostList.PostDescription = updatedPostList.PostDescription ?? string.Empty;
                    updatedPostList.PostDescription = Regex.Replace(updatedPostList.PostDescription, @"http[^\s]+", ""); 
                }
            }
            catch (Exception)
            {
            }
        }

        private void ChangeRSSFeedMediaExtension(PublisherPostlistModel updatedposturldetails)
        {
            try
            {
                Image img;
                string changeExtensionFileName = string.Empty;
                var folderPath = $@"{ConstantVariable.MediaTempFolder}\[{DateTime.Now:MM-dd-yyyy}]";
                DirectoryUtilities.CreateDirectory(folderPath);
                var mediaCount = updatedposturldetails.MediaList.Count;
                foreach (var media in updatedposturldetails.MediaList.DeepCloneObject())
                {
                    var fileName = $"{folderPath}\\{updatedposturldetails.CampaignId}_{updatedposturldetails.PostId}_{mediaCount--}" +
                                   $"{MediaUtilites.GetFileExtension(media)}";

                    if (DirectoryUtilities.CheckExistingFie(fileName))
                    {
                        img = Image.FromFile(fileName);
                        changeExtensionFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                        changeExtensionFileName = folderPath + "\\" + changeExtensionFileName + ".jpg";
                        img.Save(changeExtensionFileName);
                        updatedposturldetails.MediaList[updatedposturldetails.MediaList.IndexOf(media)] = changeExtensionFileName;                        
                    }
                }              
            }
            catch (Exception)
            {
            }

        }

        private string GetPostDescriptionWithHashTag(string postDesc, PinterestModel objAdvanceSetting)
        {
            var fullCaption = postDesc;
            if (objAdvanceSetting?.IsEnableAutomaticHashTags ?? false)
                try
                {
                    var lstHashKeywords = new List<string>();

                    if (!string.IsNullOrEmpty(objAdvanceSetting.HashWords))
                        lstHashKeywords = Regex.Split(objAdvanceSetting.HashWords, ",")
                            .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    lstHashKeywords.Shuffle();

                    for (var hashtagAddCount = 0; hashtagAddCount < lstHashKeywords.Count; hashtagAddCount++)
                    {
                        if (hashtagAddCount >= objAdvanceSetting.MaxHashtagsPerPost)
                            break;
                        fullCaption += $" #{lstHashKeywords[hashtagAddCount]?.Trim()?.Replace("#","")}";
                    }
                    fullCaption = fullCaption.Trim();

                    if (lstHashKeywords.Count < objAdvanceSetting.MaxHashtagsPerPost)
                    {
                        var postCaptionSpecialCharactersFree =
                            PdUtility.RemoveSpecialCharacters(postDesc);
                        var lstHashFromCaptionByWordLength = Regex
                            .Split(postCaptionSpecialCharactersFree.Replace("\r\n", " "), " ")
                            .Where(x => IsAlphaNumeric(x) &&
                                        x.Length >= objAdvanceSetting.MinimumWordLength)
                            .ToList();

                        lstHashFromCaptionByWordLength.Shuffle();

                        var hashtagCountToAdd =
                            objAdvanceSetting.MaxHashtagsPerPost - lstHashKeywords.Count;

                        for (var hashtagAddCount = 0;
                            hashtagAddCount < hashtagCountToAdd &&
                            lstHashFromCaptionByWordLength.Count > hashtagAddCount;
                            hashtagAddCount++)
                            fullCaption += $" #{lstHashFromCaptionByWordLength[hashtagAddCount]?.Trim()?.Replace("#","")}";
                        fullCaption = fullCaption.Trim();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return string.Empty;
                }

            if (objAdvanceSetting != null && objAdvanceSetting.IsEnableDynamicHashTags &&
                objAdvanceSetting.IsAddHashTagEvenIfAlreadyHastags)
                try
                {
                    var maxHashtagRangePerPost = objAdvanceSetting.MaxHashtagsPerPostRange.GetRandom();

                    var hashTagsPresentInCaption = Regex.Split(fullCaption, "#")
                        .Where(x => !string.IsNullOrEmpty(Utilities.GetBetween($"{x}##", "", "##")
                            .Trim()))
                        .ToList();

                    if (!fullCaption.StartsWith("#")) hashTagsPresentInCaption.RemoveAt(0);

                    var presentHashtagCount = hashTagsPresentInCaption.Count;

                    var hashtagCountToAdd = presentHashtagCount < maxHashtagRangePerPost
                        ? maxHashtagRangePerPost - presentHashtagCount
                        : 0;

                    if (hashtagCountToAdd != 0)
                    {
                        #region Hashtags from List1

                        var percentCountFromHashtag =
                            hashtagCountToAdd * objAdvanceSetting.PickPercentHashTag / 100;
                        var lstHashTagFromList1 = Regex
                            .Split(objAdvanceSetting.HashtagsFromList1, ",")
                            .Where(x => !string.IsNullOrEmpty(x)).ToList();

                        for (var hashtagcount = 0;
                            hashtagcount < percentCountFromHashtag;
                            hashtagcount++)
                            fullCaption += $" #{lstHashTagFromList1[hashtagcount].Trim()}";
                        fullCaption = fullCaption.Trim();

                        #endregion

                        #region Hashtags from List2

                        var percentCountFromList = hashtagCountToAdd - percentCountFromHashtag;
                        var lstHashTagFromList2 = Regex
                            .Split(objAdvanceSetting.HashtagsFromList2, ",")
                            .Where(x => !string.IsNullOrEmpty(x)).ToList();

                        for (var hashtagCount = 0; hashtagCount < percentCountFromList; hashtagCount++)
                            fullCaption += $" #{lstHashTagFromList2[hashtagCount].Trim()}";
                        fullCaption = fullCaption.Trim();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return fullCaption;
        }

        private bool IsAlphaNumeric(string word)
        {
            return new Regex("^[a-zA-Z0-9]*$").IsMatch(word);
        }

        public override bool PublishOnCustomDestination(string accountId,
            PublisherCustomDestinationModel customDestinationModel, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            try
            {
                if (string.IsNullOrEmpty(customDestinationModel.DestinationValue))
                    return false;

                var updatedpostlist = PerformGeneralSettings(postDetails);

                AdvanceSetting objAdvanceSetting = null;
                Application.Current.Dispatcher.Invoke(() => { objAdvanceSetting = new AdvanceSetting(); });

                if (AdvanceModel.IsEnableCampaignSourceURL)
                {
                    var lstPdSourceUrl = Regex.Split(AdvanceModel.SourceURL, "\r\n").ToList();
                    lstPdSourceUrl.Shuffle();
                    if (AdvanceModel.IsOverwritePostSourceURL || string.IsNullOrEmpty(updatedpostlist.PdSourceUrl))
                        updatedpostlist.PdSourceUrl = lstPdSourceUrl.GetRandomItem();
                }

                updatedpostlist.PostDescription = updatedpostlist.PostDescription ?? string.Empty;
                var discriptionArray = updatedpostlist.PostDescription.Split(' ');

                if (AdvanceModel.IsEnableAutomaticHashTags)
                {
                    var hashTags = AdvanceModel.HashWords.Split(',');
                    var hashCount = 0;
                    for (var hash = 0; hash < discriptionArray.Length; hash++)
                    {
                        if (hashTags.Any(x => x == discriptionArray[hash]))
                        {
                            discriptionArray[hash] = "#" + discriptionArray[hash];
                            hashCount++;
                        }

                        if (hashCount >= AdvanceModel.MaxHashtagsPerPost)
                            break;
                    }
                }

                var descriptionAfterHash = string.Empty;
                foreach (var item in discriptionArray) descriptionAfterHash += item + " ";
                updatedpostlist.PostDescription = descriptionAfterHash;

                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var objDominatorAccountModel = accountsFileManager.GetAccountById(accountId);
                var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                var pinFunct = accountScopeFactory[$"{accountId}_Publisher"].Resolve<IPinFunction>();
                var objLoginProcess = accountScopeFactory[$"{accountId}_Publisher"].Resolve<IPdLogInProcess>();
                var isLoggedIn = objLoginProcess
                    .CheckLoginAsync(objDominatorAccountModel, objDominatorAccountModel.Token, true).Result;

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post, "LangKeyTryingToPost".FromResourceDictionary());

                var objRepostPinResponseHandler = pinFunct.PostPin(objDominatorAccountModel,
                    customDestinationModel.DestinationValue,
                    updatedpostlist);
                if (objRepostPinResponseHandler.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                        objDominatorAccountModel.AccountBaseModel.UserName, ActivityType.Post,
                        String.Format("LangKeySuccessfullyPostedPinId".FromResourceDictionary(), objRepostPinResponseHandler.PinId));

                    var domain = "www.pinterest.com";
                    if (objDominatorAccountModel.Cookies != null && objDominatorAccountModel.Cookies.Count > 0
                           && objDominatorAccountModel.Cookies["csrftoken"] != null)
                        domain = objDominatorAccountModel.Cookies["csrftoken"].Domain;
                    var postUrl = $"https://{domain}/pin/" + objRepostPinResponseHandler.PinId + "/";
                    UpdatePostWithSuccessful(customDestinationModel.DestinationValue, updatedpostlist, postUrl);
                    DelayBeforeNextPublish();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
    }
}