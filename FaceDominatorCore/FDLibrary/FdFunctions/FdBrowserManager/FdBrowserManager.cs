using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.FanpageResponseHandler;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.InviterResponseHandler;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.MessageResponseHandler;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.PostResponseHandler;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.Publisher;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.UpdateAccountResponseHandler;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.UserResponseHandler;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.FDResponse.EventsResponse;
using FaceDominatorCore.FDResponse.FriendsResponse;
using FaceDominatorCore.FDResponse.GroupsResponse;
using FaceDominatorCore.FDResponse.InviterResponse;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager
{
    public interface IFdBrowserManager : IFdBaseBrowserManger
    {
        bool isActorChangedtoFanPage { get; set; }
        void AssignCancelationToken(CancellationToken cancellationTokenSource);

        EventCreaterResponseHandler EventCreater(DominatorAccountModel accountModel, EventCreaterManagerModel fdEvents);

        IResponseHandler ScrollWindowAndGetData(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "", List<string> listSavedIds = null);

        bool ScrollWindowAndGetCurrentPage(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, FanpageDetails details);

        MakeAdminResponseHandler MakeGroupAdmin(DominatorAccountModel accountModel, string groupId, FacebookUser userId);

        IResponseHandler ScrollWindowAndGetFriends(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "",
            string paginationClass = "");

        IResponseHandler ScrollWindowAndGetDataForPost(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0);

      
        bool InviteAsPersonalMessage(DominatorAccountModel accountModel, string eventId, FacebookUser objFacebookUser, string note);

        void SearchByKeywordOrHashTag(DominatorAccountModel account, SearchKeywordType entity, string keyWordorHashtag, bool isHashTag = false);
        void SearchByfeed(DominatorAccountModel account, SearchKeywordType entity, string keyWord);
        void SearchByAlbum(DominatorAccountModel account, SearchKeywordType entity, string url);
        void SearchByBirthDayEvent(DominatorAccountModel account, string url);

        bool SendEventInvittationTofriends(DominatorAccountModel accountModel, string eventId, FacebookUser objFacebookUser, string note);
        void CheckGotReactions(DominatorAccountModel accountModel, ref JobProcessResult jobProcessResult);
        void SearchByGraphSearchUrl(DominatorAccountModel account, FbEntityType entity, string url);

        void SearchFanpageLikedByFriends(DominatorAccountModel account, FbEntityType entity, string url);

        bool SearchByFriendUrl(DominatorAccountModel account, FbEntityType entity, string friendUrl);

        void SearchByPostSharer();
        FdUserInfoResponseHandlerMobile GetFullUserDetails(DominatorAccountModel account,
            FacebookUser facebookUser, bool closeBrowser = true);


        FdSendTextMessageResponseHandler SendMessage(DominatorAccountModel account,
            string userId, string message, bool isLinkPreviewActive = false, string queryTypeEnum = ""
            , string mediaPath = "", List<string> medias = null, bool openWindow = false);
        void SearchMembersData(DominatorAccountModel account, FbEntityType entity, GroupMemberCategory groupCategory,
            string keyWord);


        void ApplyPageFilters(DominatorAccountModel account, FanpageCategory objFanpageCategory, bool isVerifiedPage);

        void ApplyPlaceFilters(DominatorAccountModel account, FdPlaceFilterModel fdPlaceFilterModel);

        void ApplyGroupFilters(DominatorAccountModel account, GroupType objGroupType, GroupMemberShip objGroupMemberShip);

        IResponseHandler GetFullPostDetails(DominatorAccountModel account,
           FacebookPostDetails facebookPostDetails, bool isCustomPost = false);

       

        bool Unfriend(DominatorAccountModel accountModel);
        bool UnfollowFriend(DominatorAccountModel accountModel);
        FanpageScraperResponseHandler GetFullPageDetails(DominatorAccountModel account,
           FanpageDetails fanpageDetails, bool isNewWindow = true, bool isCloseBrowser = true);
        FdSeachGroupResponseHandler GetGroupDetails(DominatorAccountModel account,
          GroupDetails facebookGroup, bool isNewWindow = true, bool isCloseBrowser = false);
        string GetFullGroupDetails(DominatorAccountModel account,
          string groupUrl, bool isNewWindow = true);

        string GetGroupCountDetails(DominatorAccountModel account, GroupDetails groupDetails, bool isMemberGroup = false);

        void GetIndividualGroupDetails(DominatorAccountModel account, GroupDetails groupDetails, bool isMemberGroup = false);

        void SearchPostReactions(DominatorAccountModel account, BrowserReactionType entity, string postUrlbool);

        void SearchByEventUrl(DominatorAccountModel account, FbEntityType entity, string postUrl,
            EventGuestType eventGuestType);

        void SearchByFriendRequests(DominatorAccountModel account, FbEntityType entity);

        void SearchPostsByGroupUrl(DominatorAccountModel account, FbEntityType entity, string groupUrl, bool isFdBuySell = false);
        void SearchPostsByPageUrl(DominatorAccountModel account, FbEntityType entity, string pageeUrl, bool isVisitPostPage = false);

        LikeFanpageResponseHandler LikeFanpage(DominatorAccountModel account,
           FanpageDetails fanpageDetails, FanpageDetails ownPageDetails = null);

        bool JoinGroups(DominatorAccountModel account, GroupDetails groupDetails, GroupJoinerModel GroupJoinerModel);
        string LoadPageSource(DominatorAccountModel account, string url, bool clearandNeedResource = false, bool isOpenNewWindow = false, int timeSec = 15);
        List<FacebookUser> GetMutualFriends(DominatorAccountModel accountModel, string userId);

        bool CommentOnSinglePost(DominatorAccountModel accountModel, FacebookPostDetails post, List<string> comments, string postAsPageId = "",
            Dictionary<string, string> mentionDictionary = null, FanpageDetails fanpageDetails = null, bool IsCustomPost = false);

        bool LikePost(DominatorAccountModel account, FacebookPostDetails facebookPostDetails
            , ReactionType reactionType, FanpageDetails fanpageDetails = null);
        IResponseHandler LikeComments(DominatorAccountModel account, FdPostCommentDetails commentDetails,
                        ReactionType reactionType, FanpageDetails fanpageDetails = null);

        IResponseHandler ReplyToComments(DominatorAccountModel account, FdPostCommentDetails facebookPostDetails,
            string comment, string postAsPageId = "", FanpageDetails fanpageDetails = null, Dictionary<string, string> mentionDictionary = null, bool isMentionCommentedUser = false);

        bool AcceptCancelFriendRequest(DominatorAccountModel account, FacebookUser facebookUser,
            string activityClassName, string queryType);

        bool CancelSentRequest(DominatorAccountModel account, FacebookUser facebookUser);

        void OpenMessengerWindow(DominatorAccountModel account);

        IResponseHandler ScrollWindowAndGetUnRepliedMessages(DominatorAccountModel account, MessageType messageType
            , int noOfPageToScroll, int lastPageNo = 0);

        IResponseHandler ScrollWindowAndGetMessages(DominatorAccountModel account,
            int noOfPageToScroll, string otherUserId, int lastPageNo = 0);

        string SendFriendRequestSingleUser(DominatorAccountModel accountModel, string userId);

        bool OpenFriendLinkAndSendMessage(DominatorAccountModel account, FacebookUser facebookUser
            , bool closeDailog = true, bool isCheckMessage = false);

        bool UnjoinGroups(DominatorAccountModel account, GroupDetails groupDetails);

        Task<IResponseHandler> UpdateGroupsAsync(DominatorAccountModel account, CancellationToken token);

        bool InvitePages(DominatorAccountModel account, FacebookUser user, InviterOptions inviterOptionsModel);

        IResponseHandler InviteGroups(DominatorAccountModel account, FacebookUser user, string note);

        IResponseHandler PostOnOwnWall(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel);

        IResponseHandler PostOnPages(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel, string pageUrl);

        IResponseHandler PostOnGroups(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel);

        IResponseHandler ShareToFriendProfiles(DominatorAccountModel _accountModel, string friendUrl, PublisherPostlistModel postDetails, CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel, FacebookModel advanceModel);

        FdSendTextMessageResponseHandler SelectUserAndSendMessage(DominatorAccountModel account,
            SenderDetails facebookUser, string message, bool isLinkPreviewActive = false, string queryTypeEnum = ""
            , string mediaPath = "");
        IResponseHandler ScrollWindowAndGetDataForCommentsForSinglePost(DominatorAccountModel accountModel,
            FacebookPostDetails post, FbEntityType postCommentor, int noOfPageToScroll, List<FdPostCommentDetails> lstCommentDetails, int lastPageNo = 0);
        void GetCommentersFromPost(DominatorAccountModel account, BrowserReactionType entity, string postUrl,
          bool isLoadPost = true);
        IResponseHandler UpdateFriendsDetailsFromPages(DominatorAccountModel account, List<string> lstPageId);
        IResponseHandler UpdateOwnPagesDetails(DominatorAccountModel account, FacebookUser facebookUser);
        Task CloseAllMessageDialogues(string ariaLabel = "Close chat");
        void ChangeLanguage(DominatorAccountModel accountModel, string Language);
        IResponseHandler ScrollWindowGetRepliesForComment(DominatorAccountModel account, FdPostCommentDetails commentDetails,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "");

        void GetFriendsProfileId(DominatorAccountModel account, FacebookUser facebookUser);

        bool GoToNextPagination(DominatorAccountModel accountModel);

       

        int GetfriendsCount(DominatorAccountModel account);
        int GetGroupsCount(DominatorAccountModel account);

        bool DeletePost(DominatorAccountModel account, string postId, PostDeletionModel postDeletionModel);

        bool DisablePostComment(DominatorAccountModel account, FacebookPostDetails postDetails, PostOptions objPostOption);
        bool IsFriendsInGroup(GroupDetails objGroupDetails);
        GroupDetails GroupDetails(GroupDetails objGroupDetails);
        void SwitchToOwnPage(DominatorAccountModel account, string url, FanpageDetails fanpageDetails = null);
        void OpenNewWindowAndGoToUrl(DominatorAccountModel account, string url);
        Task<IResponseHandler> UpdateProfilePic(DominatorAccountModel accountModel, EditProfileModel editProfileModel);
        UpdateUserNameAndFullNameResponseHandler UpdateUserNameAndName(DominatorAccountModel accountModel, EditProfileModel editProfileModel, bool UpdateUserName = false, bool UpdateFullName = false, string ProfileId = "");
        AdvancedProfileUpdateResponseHandler UpdateAdvancedProfileDetails(DominatorAccountModel accountModel, EditProfileModel editProfileModel, bool UpdateWebsite = false, bool UpdateBio = false, bool UpdateEmail = false, bool UpdateContact = false, bool UpdateGender = false, string ProfileId = "");
        bool TurnOffCommentsOrNotificationsForPost(DominatorAccountModel accountModel, string postUrl, CancellationTokenSource campaignCancellationToken, bool turnOffComment = false, bool turnOffNotification = false);

        void SwitchToPendingMessageRequest(DominatorAccountModel account);
        Task<FdGetAllMessagedUserResponseHandler> GetPendingUserChat(DominatorAccountModel dominatorAccount);
    }
    public class FdBrowserManager : FdBaseBrowserManger, IFdBrowserManager
    {
        public static SemaphoreSlim _copyPasteLock = new SemaphoreSlim(1, 1);

        public bool isActorChangedtoFanPage { get; set; }
        public KeyValuePair<int, int> ScreenResolution { get; set; } = FdFunctions.GetScreenResolution();
        public void AssignCancelationToken(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public void SearchByKeywordOrHashTag(DominatorAccountModel account, SearchKeywordType entity, string keyWordorHashtag, bool isHashTag = false)
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var textBoxPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.SearchFacebookButtonScript}[0].getBoundingClientRect().x"
                                           , customScriptY: $"{FdConstants.SearchFacebookButtonScript}[0].getBoundingClientRect().y");
                    if (textBoxPosition.Key != 0 && textBoxPosition.Value != 0)
                        BrowserWindow.MouseClick(textBoxPosition.Key + 25, textBoxPosition.Value + 6, delayAfter: 2);
                    else
                        BrowserWindow.ExecuteScript($"{FdConstants.SearchFacebookButtonScript}[0].click()", delayInSec: 2);
                    await BrowserWindow.EnterCharsAsync($" {keyWordorHashtag}", typingDelay: 0.3, delayAtLast: 5);
                    cancellationToken.ThrowIfCancellationRequested();
                    List<string> searchOptionValue = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "option", valueType: ValueTypes.InnerText);
                    if (searchOptionValue.Count == 0)
                        searchOptionValue = await BrowserWindow.GetListInnerHtmlChildElement(ActType.GetValue, AttributeType.Role, "listbox", AttributeType.AriaSelected, "false", valueType: ValueTypes.InnerText);
                    searchOptionValue.Reverse();
                    int searchIndex = searchOptionValue.FindIndex(x => x.Equals(keyWordorHashtag));
                    if (searchIndex > -1)
                    {
                        KeyValuePair<int, int> xandYSearch = await BrowserWindow.GetXAndYAsync(customScriptX: FdConstants.GetlocFromRoleOptionXCoordinate(searchIndex)
                            , customScriptY: FdConstants.GetlocFromRoleOptionYCoordinate(searchIndex));
                        if (xandYSearch.Key == 0 && xandYSearch.Value == 0)
                            xandYSearch = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-selected=\"false\"]')[{searchIndex}].getBoundingClientRect().x",
                            customScriptY: $"document.querySelectorAll('[aria-selected=\"false\"]')[{searchIndex}].getBoundingClientRect().y");
                        if (xandYSearch.Key != 0 && xandYSearch.Value != 0)
                            await BrowserWindow.MouseClickAsync(xandYSearch.Key + 20, xandYSearch.Value + 5, delayAfter: 6);
                    }
                    else
                    {
                        if (isHashTag)
                            await BrowserWindow.GoToCustomUrl(keyWordorHashtag.Contains(FdConstants.FbHomeUrl) ? keyWordorHashtag : $"{FdConstants.FbHomeUrl}hashtag/{keyWordorHashtag}/", delayAfter: 6);
                        else
                            await BrowserWindow.GoToCustomUrl(FdConstants.GetKeyWordSearchUrl(keyWordorHashtag), delayAfter: 6);
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    await LoadSource();
                    if (!isHashTag)
                    {
                        BrowserWindow.ClearResources();
                        BrowserWindow.SetResourceLoadInstance();
                        if(entity==SearchKeywordType.Posts)
                        {
                            if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.PostFromInputBoxButton}.length"))?.Result?.ToString(), out var postFrom) && postFrom > 0)
                            {
                                var isDone = await ClickOrScrollToViewAndClick($"{FdConstants.PostFromInputBoxButton}[0]", timetoWait: 5, addX: RandomUtilties.GetRandomNumber(25, 20), addY: RandomUtilties.GetRandomNumber(15, 10));
                                if (isDone)
                                    await ClickOrScrollToViewAndClick($"{FdConstants.PublicPostOptionsButton}[0]", timetoWait: 5, isLoadSource: true, addX: RandomUtilties.GetRandomNumber(20, 15), addY: RandomUtilties.GetRandomNumber(13, 8));
                            }
                            else
                                await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "href", $"search/{entity.ToString().ToLower()}")}[0]", timetoWait: 7, isLoadSource: true, addX: RandomUtilties.GetRandomNumber(30, 20), addY: RandomUtilties.GetRandomNumber(15, 10));

                        }
                        else
                          await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "href", $"search/{entity.ToString().ToLower()}")}[0]", timetoWait: 7, isLoadSource: true, addX: RandomUtilties.GetRandomNumber(30, 20), addY: RandomUtilties.GetRandomNumber(15, 10));

                    }
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();
        }

        public void SearchByGraphSearchUrl(DominatorAccountModel account, FbEntityType entity, string url)
        {
            LoadPageSource(account, url, true);
        }

        public void SearchFanpageLikedByFriends(DominatorAccountModel account, FbEntityType entity, string url)
        {
            bool isRunning = true;
            LoadPageSource(account, url);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance();
                    if (entity == FbEntityType.Fanpage)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "href", "pages/?category=liked")}[0].click()", delayInSec: 5);
                    else if (entity == FbEntityType.FanpageLikedByFriends)
                    {
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>div", "tab", "ariaHasPopup", "menu")}[0].click()", delayInSec: 5);
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "menuitemradio", "href", "&sk=likes")}[0].click()", delayInSec: 5);
                    }
                    await LoadSource();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

        }

        public bool SearchByFriendUrl(DominatorAccountModel account, FbEntityType entity, string friendUrl)
        {
            bool isRunning = true;
            var isSearched = false;
            if (friendUrl == "OwnWall" && !friendUrl.Contains(FdConstants.FbHomeUrl))
                friendUrl = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}";
            friendUrl = !FdFunctions.IsIntegerOnly(friendUrl) ? friendUrl : FdConstants.FbHomeUrl + friendUrl;
            var pageSource = LoadPageSource(account, friendUrl, true);
            cancellationToken.ThrowIfCancellationRequested();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (entity != FbEntityType.Post)
                    {
                        BrowserWindow.ClearResources();
                        if (int.TryParse((await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div>a[role=\"tab\"],div>a[class*=\"x16tdsg8 x1hl2dhg x1vjfegm x3nfvp2 xrbpyxo x1itg65n x16dsc37\"]')].filter(x=>x.href?.includes(\"&sk=friends\")||x.textContent===\"Friends\").length"))?.Result?.ToString(), out int count) && count > 0)
                            await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div>a[role=\"tab\"],div>a[class*=\"x16tdsg8 x1hl2dhg x1vjfegm x3nfvp2 xrbpyxo x1itg65n x16dsc37\"]')].filter(x=>x.href?.includes(\"&sk=friends\")||x.textContent===\"Friends\")[0]", timetoWait: 5);
                        else
                        {
                            await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"tab\"],div[class*=\"x1vjfegm x1itg65n x10l6tqk x17qophe x13vifvy\"]')].filter(x=>x.id!=\"\"&&(x.ariaHasPopup?.includes(\"menu\")||x.textContent===\"More\"))[0]", timetoWait: 4);
                            await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div>a[role=\"menuitemradio\"],div>a[class*=\"x1344otq x1de53dj x1n2onr6 x16tdsg8 x1ja2u2z\"]')].filter(x=>x.href?.includes(\"&sk=friends\")||x.textContent===\"Friends\")[0]", timetoWait: 5);
                        }
                        if (entity == FbEntityType.Followers)
                        {
                            BrowserWindow.ClearResources();
                            if (int.TryParse((await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div>a[role=\"tab\"],div>a[class*=\"x16tdsg8 x1hl2dhg x1vjfegm x3nfvp2 xrbpyxo x1itg65n x16dsc37\"]')].filter(x=>x.href?.includes(\"&sk=followers\")||x.textContent===\"Followers\").length"))?.Result?.ToString(), out int followercount) && followercount > 0)
                                isSearched = await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div>a[role=\"tab\"],div>a[class*=\"x16tdsg8 x1hl2dhg x1vjfegm x3nfvp2 xrbpyxo x1itg65n x16dsc37\"]')].filter(x=>x.href?.includes(\"&sk=followers\")||x.textContent===\"Followers\")[0]", timetoWait: 5);
                        }
                    }
                    else
                        isSearched = true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            return isSearched;
        }
        public void SearchByPostSharer()
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    BrowserWindow.ClearResources();
                    await ClickOrScrollToViewAndClick($"{FdConstants.SHareButtonScript}[0]", timetoWait: 7, isLoadSource: true, addX: RandomUtilties.GetRandomNumber(20, 15), addY: RandomUtilties.GetRandomNumber(10, 5));
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();
        }
        public bool IsFriendsInGroup(GroupDetails objGroupDetails)
        {
            bool isRunning = true;
            var pageSource = String.Empty;
            var popupResponse = String.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                List<string> lisItems = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,
                        FdConstants.KeywordClassElement3);

                lisItems.Reverse();

                int index = lisItems.IndexOf(lisItems.FirstOrDefault(x => x.Contains(objGroupDetails.GroupId)));
                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, FdConstants.KeywordClassElement3, index: index, delayAfter: 5);

                await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                                , 0, 150, delayAfter: 5, scrollCount: 1);
                KeyValuePair<int, int> userLocation = await BrowserWindow.GetXAndYAsync(customScriptY: $"document.getElementsByClassName('{FdConstants.KeywordClassElement3}')[{index}].getElementsByClassName('xggy1nq x1a2a7pz xt0b8zv xzsf02u x1s688f')[0].getBoundingClientRect().y",
                    customScriptX: $"document.getElementsByClassName('{FdConstants.KeywordClassElement3}')[{index}].getElementsByClassName('xggy1nq x1a2a7pz xt0b8zv xzsf02u x1s688f')[0].getBoundingClientRect().x");
                if (userLocation.Key != 0 && userLocation.Value != 0)
                {
                    await BrowserWindow.MouseHoverAsync(userLocation.Key + 5, userLocation.Value + 5, delayAfter: 10);

                    using (FdHtmlParseUtility utility = new FdHtmlParseUtility())
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var popupList = utility.GetListInnerHtmlFromPartialTagName(pageSource, "span", "class", "x193iq5w xeuugli x13faqbe x1vvkbs x1xmvt09 x6prxxf xvq8zen xo1l8bm xzsf02u x1yc453h");
                        popupResponse = popupList.FirstOrDefault(x => x.Contains("friend")) ?? string.Empty;
                    }
                }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return popupResponse.Contains("friend");
        }

        public GroupDetails GroupDetails(GroupDetails objGroupDetails)
        {
            bool isRunning = true;
            var pageSource = String.Empty;
            var groupDetailsResponse = new GroupDetails();
            var popupResponse = String.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await BrowserWindow.GoToCustomUrl(objGroupDetails.GroupUrl, delayAfter: 8);
                pageSource = await BrowserWindow.GetPageSourceAsync();
                using (FdHtmlParseUtility utility = new FdHtmlParseUtility())
                {
                    objGroupDetails.GroupDescription = utility.GetInnerTextFromPartialTagNameContains(pageSource, "span", "class", "jq4qci2q a3bd9o3v b1v8xokw m9osqain");
                    objGroupDetails.GroupType = utility.GetInnerTextFromPartialTagNameContains(pageSource, "div", "class", "rq0escxv l9j0dhe7 du4w35lb j83agx80 cbu4d94t pfnyh3mw d2edcug0 hpfvmrgz n8tt0mok hyh9befq r8blr3vg jwdofwj8");
                    if (string.IsNullOrEmpty(objGroupDetails.GroupType))
                        objGroupDetails.GroupType = utility.GetInnerTextFromPartialTagNameContains(pageSource, "div", "class", "xeuugli xg83lxy x1h0ha7o x1120s5i x1nn3v0j");
                }


                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return objGroupDetails;
        }


        public void SearchMembersData(DominatorAccountModel account, FbEntityType entity, GroupMemberCategory groupCategory,
            string groupUrl)

        {
            bool isRunning = true;

            string pageSource = string.Empty;

            List<string> tabButns = new List<string>();

            DateTime lastXMin = DateTime.Now;

            string groupId = string.Empty;

            string membersUrl = string.Empty;

            List<string> lstRadioButn = new List<string>();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    groupId = FdRegexUtility.FirstMatchExtractor(groupUrl, $"https://www.facebook.com/groups/(.*?)/");

                    groupId = string.IsNullOrEmpty(groupId) ? groupUrl.Split('/').LastOrDefault() : groupId;
                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance();
                    switch (groupCategory)
                    {
                        case GroupMemberCategory.AdminsAndModerators:
                            membersUrl = $"{FdConstants.FbHomeUrl}groups/{groupId}/members/admins";
                            break;

                        case GroupMemberCategory.MembersWithThingsInCommon:
                            membersUrl = $"{FdConstants.FbHomeUrl}groups/{groupId}/members/things_in_common";
                            break;

                        case GroupMemberCategory.Friends:
                            membersUrl = $"{FdConstants.FbHomeUrl}groups/{groupId}/members/friends";
                            break;

                        case GroupMemberCategory.LocalMembers:
                            membersUrl = $"{FdConstants.FbHomeUrl}groups/{groupId}/members/near_you";
                            break;

                        default:
                            membersUrl = $"{FdConstants.FbHomeUrl}groups/{groupId}/members";
                            break;
                    }


                    cancellationToken.ThrowIfCancellationRequested();

                    await BrowserWindow.GoToCustomUrl(membersUrl, delayAfter: 8);

                    await Task.Delay(1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

        }
        public string GetGroupCountDetails(DominatorAccountModel account, GroupDetails groupDetails,
            bool isMemberGroup = false)
        {
            bool isRunning = true;
            string memberCount = string.Empty;
            string groupMemberUrl = groupDetails.GroupUrl.EndsWith("/") ? $"{groupDetails.GroupUrl}members/" : $"{groupDetails.GroupUrl}/members/";
            LoadPageSource(account, groupMemberUrl);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                memberCount = (await BrowserWindow.ExecuteScriptAsync(FdConstants.MemberCountScript))?.Result?.ToString() ?? "0";
                if (string.IsNullOrEmpty(memberCount))
                    memberCount = (await BrowserWindow.ExecuteScriptAsync(FdConstants.MemberCountScript2))?.Result?.ToString() ?? "0";
                if (!string.IsNullOrEmpty(memberCount))
                    memberCount = memberCount.Replace(",", "").Replace("Members", "").Replace("total members", "");
                if (memberCount.Contains('K') || memberCount.Contains('M'))
                    memberCount = FdFunctions.SymboleToCount(memberCount);
                else
                    memberCount = !string.IsNullOrEmpty(FdFunctions.GetIntegerOnlyString(memberCount)) ?
                            FdFunctions.GetIntegerOnlyString(memberCount) : "0";
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return memberCount;
        }

        public void GetIndividualGroupDetails(DominatorAccountModel account, GroupDetails groupDetails,
            bool isMemberGroup = false)
        {
            bool isRunning = true;

            string pageSource = string.Empty;
            var groupInfo = new List<string>();

            List<string> listDetails = new List<string>();
            string groupMemberUrl = groupDetails.GroupUrl;
            if (isMemberGroup)
                groupMemberUrl = $"{groupDetails.GroupUrl}/members/";
            pageSource = LoadPageSource(account, groupMemberUrl);

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    groupInfo = HtmlParseUtility.GetListInnerHtmlFromTagName(pageSource, "div", "class", "x78zum5 xdt5ytf x1wsgfga x9otpla");
                    if (groupInfo.Count() == 0)
                        groupInfo = HtmlParseUtility.GetListInnerHtmlFromTagName(pageSource, "div", "class", "mfn553m3 th51lws0");

                    groupDetails.GroupName = HtmlParseUtility.GetInnerTextFromTagName(groupInfo[0], "a", "role", "link");
                    groupDetails.GroupType = HtmlParseUtility.GetInnerTextFromTagName(groupInfo[0], "div", "class", "x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w xeuugli xg83lxy x1h0ha7o x1120s5i x1nn3v0j");

                    using (FdHtmlParseUtility utility = new FdHtmlParseUtility())
                    {
                        //         groupInfo = utility.GetListOuterHtmlFromPartialTagName(pageSource, "div", "class", "bi6gxh9e aov4n071");
                        groupDetails.GroupDescription = utility.GetInnerTextFromPartialTagNameContains(pageSource, "span", "class", "jq4qci2q a3bd9o3v b1v8xokw m9osqain");
                        groupDetails.GroupType = utility.GetInnerTextFromPartialTagNameContains(groupInfo[0], "div", "class", "xeuugli xg83lxy x1h0ha7o x1120s5i x1nn3v0j");
                        if (string.IsNullOrEmpty(groupDetails.GroupType))
                        {
                            groupInfo = utility.GetListInnerHtmlFromPartialTagName(pageSource, "div", "class",
                                "x78zum5 xdt5ytf xz62fqu x16ldp7u");
                            var mainInfo = groupInfo.FirstOrDefault(x => x.Contains(groupDetails.GroupName));
                            groupDetails.GroupType = utility.GetInnerTextFromPartialTagNameContains(mainInfo,
                                "div", "class", "xeuugli xg83lxy x1h0ha7o x1120s5i x1nn3v0j");
                        }
                        if (string.IsNullOrEmpty(groupDetails.GroupType))
                        {
                            groupInfo = utility.GetListOuterHtmlFromPartialTagNameContains(pageSource, "div", "class",
                                "xamitd3 xsyo7zv x16hj40l x10b6aqq x1yrsyyn");
                            var mainInfo = groupInfo.FirstOrDefault(x => x.Contains(groupDetails.GroupName));
                            groupDetails.GroupType = utility.GetInnerTextFromPartialTagNameContains(mainInfo, "div", "class",
                                "xeuugli xg83lxy x1h0ha7o x1120s5i x1nn3v0j");
                        }

                    }
                    if (pageSource.ToLower().Contains("join group") || pageSource.ToLower().Contains("join community"))
                        groupDetails.GroupJoinStatus = "Not a member";
                    else
                        groupDetails.GroupJoinStatus = "Request Sent / Already a Member";
                }
                catch (Exception) { }

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();


        }


        public IResponseHandler ScrollWindowAndGetData(DominatorAccountModel account, FbEntityType entity, int noOfPageToScroll,
            int lastPageNo = 0, string className = "", List<string> listSavedIds = null)
        {
            int failedCount = 0;
            int currentCount = 0;
            int previousCount = 0;
            string pageSource = string.Empty;
            bool isRunning = true;
            var jsonresponseList = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var tempjsonresponselist = new List<string>();
                    while (lastPageNo < noOfPageToScroll && failedCount < 2)
                    {
                        if (entity == FbEntityType.Friends)
                            await BrowserWindow.MouseScrollAsync(100, ScreenResolution.Value / 2, 0, -500, delayAfter: 5, scrollCount: 5);
                        else
                            await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2 + 50, ScreenResolution.Value / 2, 0, -500, delayAfter: 5, scrollCount: 5);
                        if (entity == FbEntityType.PostLikers)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Feedback\"", true);
                        }
                        else if (entity == FbEntityType.PostSharers)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"reshares\":{\"edges\":[{\"node\"", true);
                        }
                        else if (entity == FbEntityType.Fanpage || entity == FbEntityType.FanpageLikedByFriends)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"role\":\"ENTITY_PAGES\"", true);
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__isEntity\":\"Page\"", true);
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);
                        }
                        else if (entity == FbEntityType.Groups)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"role\":\"ENTITY_GROUPS\"", true);
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Group\"", true);

                        }
                        else if (entity == FbEntityType.GroupMembers)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true, "__isGroupMember\":\"User\"");

                        }
                        else if (entity == FbEntityType.Places)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"role\":\"ENTITY_PLACES\"", true);
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);
                        }
                        else if (entity == FbEntityType.PostLikers)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Feedback\"", true, "\"reactors\":{\"edges\"");
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);
                        }
                        else if (entity == FbEntityType.EventGuests)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"event\":{\"friends\"", true, "\"__typename\":\"User\"");
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);
                        }
                        else if (entity == FbEntityType.User || entity == FbEntityType.Friends)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"role\":\"ENTITY_USER\"", true);
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__isEntity\":\"User\"", true);
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);
                        }

                        tempjsonresponselist.RemoveAll(x => jsonresponseList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        jsonresponseList.AddRange(tempjsonresponselist);
                        lastPageNo++;

                        if (currentCount == previousCount && jsonresponseList.Count == currentCount) failedCount++;
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                        BrowserWindow.ClearResources();
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    pageSource = await BrowserWindow.GetPageSourceAsync();

                }
                catch (Exception)
                {

                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();
            if (entity == FbEntityType.User || entity == FbEntityType.Friends || entity == FbEntityType.GroupMembers
               || entity == FbEntityType.PostLikers || entity == FbEntityType.EventGuests || entity == FbEntityType.SuggestedFriends || entity == FbEntityType.PostSharers || entity == FbEntityType.PostCommentor)
                return new SearchPeopleResponseHandler(jsonresponseList, new ResponseParameter() { Response = pageSource }, listSavedIds ?? new List<string>(), failedCount < 2, entity);
            else if (entity == FbEntityType.Fanpage || entity == FbEntityType.FanpageLikedByFriends || entity == FbEntityType.Places)
                return new FdSearchPageResponseHandler(new ResponseParameter() { Response = pageSource }, jsonresponseList, failedCount < 2, entity);
            else if (entity == FbEntityType.Groups)
                return new GroupScraperResponseHandler(new ResponseParameter() { Response = pageSource }, jsonresponseList, failedCount < 2);
            else
                return null;
        }

        public bool ScrollWindowAndGetCurrentPage(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, FanpageDetails details)
        {
            int lastPageNo = 0;
            int failedCount = 0;
            bool isRunning = true;
            bool isFoundPage = false;
            int currentCount = 0;
            string pageSource = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int previousCount = 0;

                    while (lastPageNo < noOfPageToScroll && failedCount <= 5)
                    {
                        lastPageNo++;

                        currentCount = await BrowserWindow.GetItemCountInnerHtml(ActType.GetValue, AttributeType.ClassName, FdConstants.FriendsFanpageLikes2Element);

                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                                    , 0, -500, delayAfter: 2, scrollCount: 8);

                        if (currentCount == previousCount)
                        {
                            failedCount++;
                            await Task.Delay(1000, cancellationToken);
                        }
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }

                        pageSource = await BrowserWindow.GetPageSourceAsync(1);

                        if (pageSource.Contains(details.FanPageID) || pageSource.Contains(details.FanPageUrl))
                            break;

                        cancellationToken.ThrowIfCancellationRequested();

                    }

                    await Task.Delay(1000, cancellationToken);

                    pageSource = await BrowserWindow.GetPageSourceAsync();

                    if (pageSource.Contains(details.FanPageID) || pageSource.Contains(details.FanPageUrl))
                        isFoundPage = true;
                }
                catch (Exception)
                {
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

            return isFoundPage;
        }

        public IResponseHandler ScrollWindowAndGetFriends(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "",
            string paginationClass = "")
        {
            int failedCount = 0;
            bool isRunning = true;
            List<string> JsonResponseList = new List<string>();
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int currentCount = 0;
                    int previousCount = 0;
                    var tempjsonresponselist = new List<string>();
                    while (lastPageNo < noOfPageToScroll && failedCount < 2)
                    {
                        if (entity == FbEntityType.SentFriendRequests || entity == FbEntityType.Friends || entity == FbEntityType.SuggestedFriends || entity == FbEntityType.IncommingFriendRequests)
                            await BrowserWindow.MouseScrollAsync(100, ScreenResolution.Value / 2, 0, -500, delayAfter: 5, scrollCount: 5);
                        else
                            await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2 + 50, ScreenResolution.Value / 2, 0, -500, delayAfter: 5, scrollCount: 5);

                        if (entity == FbEntityType.SentFriendRequests)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"outgoing_friend_requests\"", true);
                        else if (entity == FbEntityType.IncommingFriendRequests)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"friend_requests\"", true);
                            if(tempjsonresponselist is null || tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"data\":{\"viewer\":{\"pymk_grid\"", true);
                        }
                        else if (entity == FbEntityType.SuggestedFriends)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"people_you_may_know\":{\"", true, "\"__typename\":\"User\"");
                        else if (entity == FbEntityType.UserGreetings)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"", true, "\"all_friends_by_birthday_month\"");
                        else if (entity == FbEntityType.Friends)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"all_friends", true);
                        else if (entity == FbEntityType.AddedFriends)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"", true, "\"name\":\"Recently added\"");
                        else
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);


                        tempjsonresponselist.RemoveAll(x => JsonResponseList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        JsonResponseList.AddRange(tempjsonresponselist);
                        lastPageNo++;
                        BrowserWindow.ClearResources();
                        if (currentCount == previousCount && JsonResponseList.Count == currentCount) failedCount++;
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception) { }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new SearchPeopleResponseHandler(JsonResponseList, new ResponseParameter() { Response = pageSource }, new List<string>(), failedCount < 2, entity);
        }
        public IResponseHandler ScrollWindowAndGetDataForPost(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0)
        {
            int failedcount = 0;
            List<string> jsonresponseList = new List<string>();
            string pageSource = string.Empty;
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int previousCount = 0;
                    int currentCount = 0;

                    var tempjsonresponselist = new List<string>();
                    BrowserWindow.SetResourceLoadInstance();
                    while (lastPageNo < noOfPageToScroll && failedcount < 2)
                    {
                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2 + 100, ScreenResolution.Value / 2
                                    , 0, -500, delayAfter: 5, scrollCount: 5);
                        if (entity == FbEntityType.PostSharers)
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"reshares\":{\"edges\":[{\"node\"", true);
                        }
                        else
                        {
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Story\"", true);
                        }
                        tempjsonresponselist.RemoveAll(x => jsonresponseList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        jsonresponseList.AddRange(tempjsonresponselist);
                        lastPageNo++;
                        if (currentCount == previousCount && jsonresponseList.Count == currentCount) failedcount++;
                        else
                        {
                            failedcount = 0;
                            previousCount = currentCount;
                        }
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    isRunning = false;
                }
                catch (Exception)
                {
                    isRunning = false;
                }
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new BrowserPostResponseHandler(new ResponseParameter() { Response = pageSource }, jsonresponseList, entity, failedcount < 2);
        }

       

        public FdUserInfoResponseHandlerMobile GetFullUserDetails(DominatorAccountModel account,
           FacebookUser facebookUser, bool closeBrowser = true)
        {

            var JsonResponseList = new List<string>();
            bool isRunning = true;
            string pageSource = string.Empty;
            var profileUrl = string.IsNullOrEmpty(facebookUser.ProfileUrl) ? facebookUser.ScrapedProfileUrl : facebookUser.ProfileUrl;
            pageSource = LoadPageSource(account, profileUrl, clearandNeedResource: true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div>a[role=\\\"tab\\\"],div>a[class*=\\\"x16tdsg8 x1hl2dhg x1vjfegm x3nfvp2 xrbpyxo x1itg65n x16dsc37\\\"]')].filter(x=>x.href?.includes(\"/about\")||x.textContent===\"About\")[0]", timetoWait: 4);
                    await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div>a[role=\"link\"],div>a[class*=\"xz9dl7a x1iji9kk xsag5q8 x1sln4lm x1n2onr6\"]')].filter(x=>x.href?.includes(\"/about_contact_and_basic_info\")||x.textContent===\"Contact and basic info\")[0]", timetoWait: 3);
                    var tempList = await BrowserWindow.GetPaginationDataList("\"data\":{\"user\":{\"about_app_sections\":{\"nodes\"", true);
                    var tempList2 = await BrowserWindow.GetPaginationDataList("\"data\":{\"user\":{\"profile_header_renderer\"", true);
                    JsonResponseList.AddRange(tempList2);
                    JsonResponseList.AddRange(tempList);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex) { ex.DebugLog(); }
                if (closeBrowser)
                    CloseBrowser(account);
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            return new FdUserInfoResponseHandlerMobile(new ResponseParameter() { Response = pageSource }, JsonResponseList, facebookUser);
        }



        public void GetFriendsProfileId(DominatorAccountModel account,
           FacebookUser facebookUser)
        {
            DateTime currentTime = DateTime.Now;

            bool isRunning = true;

            string pageSource = null;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    CustomBrowserWindow = new BrowserWindow(account, targetUrl: facebookUser.ProfileUrl, customUse: true)
                    {
                        Visibility = Visibility.Hidden
                    };
#if DEBUG
                    CustomBrowserWindow.Visibility = Visibility.Visible;
#endif
                    CustomBrowserWindow.BrowserSetCookie();
                    CustomBrowserWindow.Show();

                    pageSource = await CustomBrowserWindow.GetPageSourceAsync();

                    if (string.IsNullOrEmpty(facebookUser.UserId) || facebookUser.UserId == "0")
                        facebookUser.UserId = FdRegexUtility.FirstMatchExtractor(pageSource, FdConstants.DataProfileIdRegx);

                    if (facebookUser.UserId.Contains("&"))
                        facebookUser.UserId = FdFunctions.GetIntegerOnlyString(facebookUser.UserId.Split('&')[0]);

                    facebookUser.ProfileUrl = FdConstants.FbHomeUrl + facebookUser.UserId;

                    CustomBrowserWindow.Close();
                    CustomBrowserWindow.Dispose();

                }
                catch (Exception ex)
                {
                    if (ex.Message == "")
                    { }
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }
        public void ApplyPageFilters(DominatorAccountModel account, FanpageCategory fanpageCategory,
            bool isVerifiedPage)
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (isVerifiedPage && (await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel,
                    "Verified", valueType: ValueTypes.OuterHtml)).ToLower().Contains("aria-checked=\"false\""))
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, attributeValue: "Verified",
                           index: 0, delayAfter: 2);
                if (fanpageCategory.ToString() != FanpageCategoryWeb.AnyCategory.ToString())
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.FilterButtonScript, "div>div", "combobox", "filter by category")}[0].click()", delayInSec: 4);
                    await BrowserWindow.ExecuteScriptAsync($"[...document.querySelectorAll('div[role=\"option\"]')][{(int)Enum.Parse(typeof(FanpageCategoryWeb), fanpageCategory.ToString())}].click()", delayInSec: 5);
                    await LoadSource(5);
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }

        public void ApplyPlaceFilters(DominatorAccountModel account, FdPlaceFilterModel fdPlaceFilterModel)
        {
            bool isRunning = true;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (fdPlaceFilterModel.IsVisitedByFriendsChecked || fdPlaceFilterModel.IsOpenNowChecked || fdPlaceFilterModel.IsTakeAwayChecked || fdPlaceFilterModel.IsDeliveryChecked)
                        BrowserWindow.ClearResources();
                    if (fdPlaceFilterModel.IsVisitedByFriendsChecked)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.FilterButtonScript, "div>input", "switch", "visited by friends")}[0].click()", delayInSec: 5);

                    if (fdPlaceFilterModel.IsOpenNowChecked)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.FilterButtonScript, "div>input", "switch", "open now")}[0].click()", delayInSec: 5);

                    if (fdPlaceFilterModel.IsTakeAwayChecked)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.FilterButtonScript, "div>input", "switch", "takeaway")}[0].click()", delayInSec: 5);
                    if (fdPlaceFilterModel.IsDeliveryChecked)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.FilterButtonScript, "div>input", "switch", "delivery")}[0].click()", delayInSec: 5);
                    if (fdPlaceFilterModel.IsPriceRangeChecked)
                    {
                        var objPlacePriceRange = (PlacePriceRange)Enum.Parse(typeof(PlacePriceRange), fdPlaceFilterModel.SelectedPriceRange);
                        if (objPlacePriceRange.ToString() != PlacePriceRange.AnyPrice.ToString())
                        {
                            BrowserWindow.ClearResources();
                            await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.FilterButtonScript, "div>div", "combobox", "filter by price")}[0].click()", delayInSec: 2);
                            await BrowserWindow.ExecuteScriptAsync($"[...document.querySelectorAll('div[role=\"option\"]')][{(int)Enum.Parse(typeof(PlacePriceRange), objPlacePriceRange.ToString())}].click()", delayInSec: 5);
                        }
                    }
                    await LoadSource(8);
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
        }


        public void ApplyGroupFilters(DominatorAccountModel account, GroupType objGroupType, GroupMemberShip objGroupMemberShip)
        {
            bool isRunning = true;

            int selectedGroupType = (int)objGroupType;

            int selectedGroupMemberShip = (int)objGroupMemberShip;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (selectedGroupMemberShip == 3)
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, attributeValue: "_4f3b",
                          index: 4, delayAfter: 1);

                if (selectedGroupMemberShip == 2)
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, attributeValue: "oajrlxb2 rq0escxv f1sip0of hidtqoto nhd2j8a9 datstx6m kvgmc6g5 cxmmr5t8 oygrvhab hcukyx3x b5wmifdl lzcic4wl jb3vyjys rz4wbd8a qt6c0cv9 a8nywdso pmk7jnqg j9ispegn kr520xx4 k4urcfbm",
                        index: 0, delayAfter: 2);

                if (selectedGroupType != 1 && selectedGroupType != 3)
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, attributeValue: "Public groups",
                           delayAfter: 1);

                isRunning = false;

            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }

        public FanpageScraperResponseHandler GetFullPageDetails(DominatorAccountModel account,
           FanpageDetails fanpageDetails, bool isNewWindow = true, bool isCloseBrowser = true)
        {
            string response = string.Empty;
            bool isRunning = true;
            var JsonResponseList = new List<string>();
            LoadPageSource(account, fanpageDetails.FanPageUrl);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await CloseAllMessageDialogues("Close");
                    await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "tab", "textContent", "About")}[0].click()", delayInSec: 3);
                    await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Page transparency")}[0].click()", delayInSec: 3);
                    var tempList2 = await BrowserWindow.GetPaginationDataList("\"data\":{\"user\":{\"profile_header_renderer\"", true);
                    var tempList = await BrowserWindow.GetPaginationDataList("\"data\":{\"user\":{\"about_app_sections\":{\"nodes\"", true);
                    JsonResponseList.AddRange(tempList2);
                    JsonResponseList.AddRange(tempList);
                    response = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            if (isCloseBrowser)
                CloseBrowser(account);
            return new FanpageScraperResponseHandler(new ResponseParameter() { Response = response }, JsonResponseList, fanpageDetails);

        }

        public LikeFanpageResponseHandler LikeFanpage(DominatorAccountModel account,
           FanpageDetails fanpageDetails, FanpageDetails ownPageDetails = null)
        {
            bool isRunning = true;

            string response = null;

            string pageSource = string.Empty;
            if (ownPageDetails != null && !isActorChangedtoFanPage)
                SwitchToOwnPage(account, ownPageDetails?.FanPageUrl, ownPageDetails);

            Application.Current.Dispatcher.Invoke(async () =>
            {
                pageSource = await BrowserWindow.GetPageSourceAsync();
                if (pageSource.Contains("_42ft _4jy0 PageLikeButton _4jy4 _517h _51sy"))
                {
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_42ft _4jy0 PageLikeButton _4jy4 _517h _51sy", delayAfter: 2);
                    List<string> validation = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "_42ft _4jy0 PageLikedButton _52nf PageLikeButton _4jy4 _517h _51sy _5f0v", ValueTypes.InnerText);
                    if (validation.IndexOf("Liked") >= 0)
                        response = "Liked";
                }
                else
                {
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Like", delayAfter: 4);

                    pageSource = await BrowserWindow.GetPageSourceAsync();

                    if (pageSource.Contains("aria-label=\"Like Page\""))
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Like Page", delayAfter: 4);

                    if (pageSource.Contains("aria-label=\"Follow\""))
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Follow", delayAfter: 4);

                    if ((await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Liked")).Contains("Liked"))
                        response = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "qtwd0hg0 qypqp5cg j83agx80 bp9cbjyn");

                    if ((await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Following")).Contains("Following"))
                        response = await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Following", ValueTypes.InnerText);

                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (response.Contains("Liked") || pageSource.Contains("aria-label=\"Liked\"") || response.Contains("Following"))
                        response = "Liked";

                    isRunning = false;
                }

            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return new LikeFanpageResponseHandler(new ResponseParameter(), response);
        }

        public bool JoinGroups(DominatorAccountModel account, GroupDetails groupDetails, GroupJoinerModel GroupJoinerModel)
        {
            bool isRunning = true;
            var groupurl = !string.IsNullOrEmpty(groupDetails.GroupUrl) && groupDetails.GroupUrl.StartsWith(FdConstants.FbHomeUrl)
                ? groupDetails.GroupUrl : FdConstants.FbHomeUrl + groupDetails.GroupId;

            string response = null;
            string pageSource = string.Empty;
            int index = 0;
            if (!BrowserWindow.CurrentUrl().Contains(groupDetails.GroupUrl) && !BrowserWindow.CurrentUrl().Contains(groupDetails.GroupId))
                pageSource = LoadPageSource(account, groupurl, true);
            List<string> responseList = null;


            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(pageSource))
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (pageSource.Contains("aria-label=\"Close\""))
                    {
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Close", delayAfter: 2);
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Exit", delayAfter: 2);
                    }
                    BrowserWindow.SetResourceLoadInstance();
                    var isjoinClicked = await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"join group\")||x.textContent?.toLowerCase().includes(\"join group\")||x.ariaLabel?.toLowerCase().includes(\"follow group\")||x.ariaLabel?.toLowerCase().includes(\"join community\"))[0]", timetoWait: 5, isLoadSource: true);
                    if (!isjoinClicked)
                        await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"join group\")||x.textContent?.toLowerCase().includes(\"join group\")||x.ariaLabel?.toLowerCase().includes(\"follow group\")||x.ariaLabel?.toLowerCase().includes(\"join community\"))[1]", timetoWait: 5, isLoadSource: true);

                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    var isHowGrpJoinPresent = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                        "x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x78zum5 xdt5ytf x1iyjqo2 x1al4vs7");
                    if (!string.IsNullOrEmpty(isHowGrpJoinPresent))
                    {
                        responseList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,
                            "x6s0dn4 x1y1aw1k x1sxyh0 xwib8y2 xurb0ha", valueType: ValueTypes.OuterHtml);
                        responseList.Reverse();
                        index = responseList.IndexOf(responseList.FirstOrDefault(x => x.Contains("Acting profile")));
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "x6s0dn4 x1y1aw1k x1sxyh0 xwib8y2 xurb0ha", index: index, delayAfter: 5);
                        pageSource = BrowserWindow.GetPageSource();
                        index = Regex.Matches(pageSource, "aria-label=\"Join Group\"").Count - 1;
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Join Group", index: index, delayAfter: 5);
                    }

                    responseList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"group_request_to_join", true);
                    responseList = responseList.Count > 0 ? responseList : await BrowserWindow.GetPaginationDataList("{\"data\":{\"join_forum_group\"", true);
                    response = responseList.FirstOrDefault(x => x.Contains("group_request_to_join") || x.Contains("\"data\":{\"join_forum_group\""));

                    // Popup for selecting account
                    if (string.IsNullOrEmpty(response))
                    {
                        KeyValuePair<int, int> userLocation = await BrowserWindow.GetXAndYAsync(customScriptY: $"document.getElementsByClassName('pfnyh3mw d2edcug0 cxgpxx05 sj5x9vvc')[0].getElementsByClassName('{FdConstants.SuggestedFriendsUserElement}')[0].getBoundingClientRect().y",
                            customScriptX: $"document.getElementsByClassName('pfnyh3mw d2edcug0 cxgpxx05 sj5x9vvc')[0].getElementsByClassName('{FdConstants.SuggestedFriendsUserElement}')[0].getBoundingClientRect().x");
                        if (userLocation.Key != 0 && userLocation.Value != 0)
                            await BrowserWindow.MouseClickAsync(userLocation.Key + 50, userLocation.Value + 10, delayAfter: 4);

                        int dialogIndex = int.Parse(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.Role, "dialog")) - 1;
                        await BrowserWindow.BrowserActAsync(ActType.CustomActByQueryType, AttributeType.Role, "dialog", index: dialogIndex, value: $"querySelectorAll('[aria-label=\"Join Group\"]')[0].click()", delayAfter: 2);

                        Task.Delay(TimeSpan.FromSeconds(15), cancellationToken).Wait();
                        responseList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"group_request_to_join", true);

                        response = responseList.FirstOrDefault(x => x.Contains("group_request_to_join"));
                    }

                    // Popup for Questions
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (pageSource.Contains("aria-label=\"Participant questions\"") || pageSource.Contains("aria-label=\"Answer questions\""))
                    {
                        var isNotNowClicked = await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3\"]')].filter(x=>x.ariaLabel?.toLowerCase()===\"not now\"||x.textContent?.toLowerCase()===\"not now\")[0]");

                        if (isNotNowClicked)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "x1wpzbip x14yjl9h xudhj91 x18nykt9 xww2gxu", delayAfter: 3);
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Submit", delayAfter: 4);
                        }
                    }
                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();

                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;

            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return !string.IsNullOrEmpty(response);

        }

        public void SearchPostReactions(DominatorAccountModel account, BrowserReactionType entity, string postUrl
           )
        {
            bool isRunning = true;
            LoadPageSource(account, postUrl, clearandNeedResource: true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    var reactionPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.reactionButtonScript}[0].getBoundingClientRect().x",
                       customScriptY: $"{FdConstants.reactionButtonScript}[0].getBoundingClientRect().y");
                    if (reactionPosition.Value > 80 && reactionPosition.Value < ScreenResolution.Value - 200)
                        await BrowserWindow.MouseClickAsync(reactionPosition.Key + 17, reactionPosition.Value + 9, delayAfter: 6);
                    else
                        await BrowserWindow.ExecuteScriptAsync($"{FdConstants.reactionButtonScript}[0].click()", delayInSec: 6);
                    await LoadSource(8);
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }




        public void SearchByEventUrl(DominatorAccountModel account, FbEntityType entity, string eventUrl,
            EventGuestType eventGuestType)
        {
            bool isRunning = true;
            eventUrl = !FdFunctions.IsIntegerOnly(eventUrl) ? eventUrl : FdConstants.FbHomeUrl + eventUrl;
            LoadPageSource(account, eventUrl, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {

                try
                {
                    BrowserWindow.ClearResources();
                    var resp = await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>div", "button", "textContent", "See All")}[0].click()", delayInSec: 10);
                    if (!resp.Success)
                    {
                        string guestClass = "ihqw7lf3 cbu4d94t j83agx80 bp9cbjyn";
                        if (!string.IsNullOrEmpty(await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, guestClass)))
                        {
                            List<string> lstBtn = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, guestClass);
                            int index = lstBtn.IndexOf(lstBtn.FirstOrDefault(x => x.ToLower().Contains("going")));
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, guestClass, index: index, delayAfter: 10);
                        }
                    }

                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }

        public void SearchByFriendRequests(DominatorAccountModel account, FbEntityType entity)
        {
            bool isRunning = true;
            if (entity == FbEntityType.AddedFriends)
            {
                var userName = account.AccountBaseModel?.UserId;
                if (string.IsNullOrEmpty(account.AccountBaseModel?.UserId))
                {
                    var pageSource = BrowserWindow.GetPageSource();
                    userName = FdRegexUtility.FirstMatchExtractor(pageSource, $"\"username\":\"(.*?)\"");

                    userName = string.IsNullOrEmpty(userName) ? FdRegexUtility.FirstMatchExtractor(pageSource, $"\"USER_ID\":\"(.*?)\"") : userName;

                    if (string.IsNullOrEmpty(userName) || userName.Contains("profile.php"))
                        userName = account.AccountBaseModel?.UserId;
                }
                if (FdFunctions.IsIntegerOnly(userName))
                    LoadPageSource(account, $"https://www.facebook.com/profile.php?id={userName}&sk=friends", true);
                else
                    LoadPageSource(account, $"{FdConstants.FbHomeUrl}{userName}/friends_recent", true);

            }
            else
                LoadPageSource(account, "https://www.facebook.com/friends", true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    if (entity == FbEntityType.AddedFriends)
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "tab", "textContent", "Recently added")}[0]", timetoWait: 7, isLoadSource: true);
                    else if (entity == FbEntityType.SuggestedFriends)
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Suggestions")}[0]", timetoWait: 7, isLoadSource: true);
                    else if (entity == FbEntityType.Friends)
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "All friends")}[0]", timetoWait: 7, isLoadSource: true);

                    else if (entity == FbEntityType.IncommingFriendRequests || entity == FbEntityType.SentFriendRequests)
                    {
                        var Clicked =  await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Friend requests")}[0]", timetoWait: 7, isLoadSource: true);
                        if(!Clicked)
                            await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Friend Requests")}[0]", timetoWait: 7, isLoadSource: true);

                        if (entity == FbEntityType.SentFriendRequests)
                        {
                            BrowserWindow.ClearResources();
                            await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>div", "button", "textContent", "View sent requests")}[0]", timetoWait: 7, isLoadSource: true);
                        }
                    }
                    await LoadSource(4);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
        }



        public void SearchPostsByGroupUrl(DominatorAccountModel account, FbEntityType entity,
            string groupUrl, bool isFdBuySell = false)
        {
            bool isRunning = true;
            LoadPageSource(account, groupUrl, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {

                try
                {
                    if (!isFdBuySell && bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>a", "role", "tab", "textContent", "discussion")}[0].ariaSelected"))?.Result?.ToString(), out bool isdiscussionSelected) && !isdiscussionSelected)
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>a", "role", "tab", "textContent", "discussion")}[0]", isLoadSource: true);

                    if (isFdBuySell && bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>a", "role", "tab", "textContent", "buy and sell")}[0].ariaSelected"))?.Result?.ToString(), out bool isbuysellSelected) && !isbuysellSelected)
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>a", "role", "tab", "textContent", "buy and sell")}[0]", isLoadSource: true);
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }

        public void SearchPostsByPageUrl(DominatorAccountModel account, FbEntityType entity,
            string pageUrl, bool isVisitPostPage = false)
        {
            bool isRunning = true;

            Task.Delay(2000, cancellationToken).Wait();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(pageUrl, delayAfter: 8);

                    string pageSource = await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.Role, "tablist");

                    DateTime lastXMin = DateTime.Now.AddMinutes(1);

                    while (string.IsNullOrEmpty(pageSource) && lastXMin >= DateTime.Now)
                    {
                        await Task.Delay(500, cancellationToken);
                        pageSource = await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.Role, "tablist");

                        if ((await BrowserWindow.GetPageSourceAsync()).Contains("<title>Error</title>"))
                            BrowserWindow.Refresh();

                    }

                    if (isVisitPostPage)
                    {

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Switch",
                           delayAfter: 2);

                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var switchAttribute = string.Empty;
                        int switchIndex = 0;
                        using (var parser = new FdHtmlParseUtility())
                        {
                            var data = parser.GetListOuterHtmlFromPartialTagName(pageSource,
                                                                                "div",
                                                                                "aria-label",
                                                                                "Switch");
                            switchIndex = data.IndexOf(data.FirstOrDefault(x => x.Contains("aria-label=\"Switch Now\"")));
                            switchAttribute = switchIndex > 1 || switchIndex == -1 ? "Switch" : "Switch Now";
                        }
                        if (switchAttribute.Contains("Switch Now"))
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery,
                                                                AttributeType.AriaLabel,
                                                                switchAttribute,
                                                                index: 0,
                                                                delayAfter: 5);
                            Task.Delay(2000, cancellationToken).Wait();
                        }
                        else
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Switch", index: 1,
                            delayAfter: 5);
                        }
                    }

                    lastXMin = DateTime.Now.AddMinutes(1);

                    do
                    {
                        if ((await BrowserWindow.GetPageSourceAsync()).Contains("<title>Error</title>"))
                        {
                            lastXMin = DateTime.Now.AddMinutes(3);
                            BrowserWindow.Refresh();
                        }
                        pageSource = string.Empty;
                        pageSource = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                            FdConstants.PostElement5Class, ValueTypes.OuterHtml);
                    } while (string.IsNullOrEmpty(pageSource) && lastXMin >= DateTime.Now);

                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();

        }

        public string LoadPageSource(DominatorAccountModel account, string url, bool clearandNeedResource = false, bool isNewWindow = false, int timeSec = 15)
        {
            bool isRunning = true;

            string pageSource = string.Empty;

            DateTime currentTime = DateTime.Now;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (isNewWindow)
                    {
                        CustomBrowserWindow = new BrowserWindow(account, targetUrl: url, isNeedResourceData: clearandNeedResource, customUse: true)
                        {
                            Visibility = Visibility.Hidden
                        };
#if DEBUG
                        CustomBrowserWindow.Visibility = Visibility.Visible;
#endif
                        CustomBrowserWindow.BrowserSetCookie();
                        CustomBrowserWindow.Show();
                        await Task.Delay(8000);
                        do
                        {
                            await Task.Delay(1500);
                            pageSource = await BrowserWindow.GetPageSourceAsync();
                        } while ((!CustomBrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timeSec + 15) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < timeSec));
                    }
                    else
                    {
                        if (BrowserWindow == null)
                        {
                            BrowserWindow = new BrowserWindow(account, targetUrl: url, isNeedResourceData: clearandNeedResource, customUse: true)
                            {
                                Visibility = Visibility.Hidden
                            };
#if DEBUG
                            BrowserWindow.Visibility = Visibility.Visible;
#endif
                            BrowserWindow.BrowserSetCookie();
                            BrowserWindow.Show();
                            await Task.Delay(8000);
                        }
                        else if (BrowserWindow != null && (!BrowserWindow.CurrentUrl().Contains(url) || clearandNeedResource))
                        {
                            if (clearandNeedResource)
                            {
                                BrowserWindow.ClearResources();
                                BrowserWindow.SetResourceLoadInstance();
                            }
                            await BrowserWindow.GoToCustomUrl(url, 6);
                        }
                        do
                        {
                            await Task.Delay(1500);
                            pageSource = await BrowserWindow.GetPageSourceAsync();
                        } while ((!BrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timeSec + 15) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < timeSec));
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();
            return pageSource;
        }
        public FdSendTextMessageResponseHandler SelectUserAndSendMessage(DominatorAccountModel account,
            SenderDetails facebookUser, string message, bool isLinkPreviewActive = false, string queryTypeEnum = ""
            , string mediaPath = "")
        {
            bool isRunning = true;

            DateTime currentTime = DateTime.Now;

            FdSendTextMessageResponseHandler messageDetails = null;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    List<string> listButtons = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.DataTestId
                        , "mwthreadlist-item", valueType: ValueTypes.OuterHtml);

                    listButtons.Reverse();

                    List<string> chklist = facebookUser.SenderImage.Split('?').ToList();

                    string chkParameter = chklist.LastOrDefault();

                    int messageButtonIndex = listButtons.IndexOf(listButtons.FirstOrDefault(x => x.Contains(chkParameter)));

                    await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.DataTestId, "mwthreadlist-item", index: messageButtonIndex);

                    await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                                        , 0, -100, delayAfter: 2, scrollCount: 1);

                    KeyValuePair<int, int> xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[data-testid=\"mwthreadlist-item\"]')[{messageButtonIndex}].getBoundingClientRect().x"
                        , customScriptY: $"document.querySelectorAll('[data-testid=\"mwthreadlist-item\"]')[{messageButtonIndex}].getBoundingClientRect().y");

                    await BrowserWindow.MouseClickAsync(xAndY.Key + 20, xAndY.Value + 20, delayAfter: 5);

                    await CopyPasteTextAsync($"{message} ", activityType: "Chat", parentIndex: 0);

                    KeyValuePair<int, int> sendBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.SendMessageButtonScript}[0].getBoundingClientRect().x"
                                            , customScriptY: $"{FdConstants.SendMessageButtonScript}[0].getBoundingClientRect().y");
                    if (sendBtnLoc.Key != 0 && sendBtnLoc.Value != 0)
                        BrowserWindow.MouseClick(sendBtnLoc.Key + 11, sendBtnLoc.Value + 7, delayAfter: 5);
                    else
                        BrowserWindow.ExecuteScript($"{FdConstants.SendMessageButtonScript}[0].click()", delayInSec: 5);

                }
                catch (ArgumentException) { }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;

            });

            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();

            return messageDetails;
        }

        public FdSendTextMessageResponseHandler SendMessage(DominatorAccountModel account,
            string userId, string message, bool isLinkPreviewActive = false, string queryTypeEnum = ""
            , string mediaPath = "", List<string> medias = null, bool openWindow = false)
        {
            bool isRunning = true;
            var MessageWindowBtnLoc = new KeyValuePair<int, int>();
            var resp = new JavascriptResponse();
            FdSendTextMessageResponseHandler messageDetails = null;
            var url = userId.StartsWith(FdConstants.FbHomeUrl) ? userId : $"{FdConstants.FbHomeUrl}{userId}";
            LoadPageSource(account, url);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (openWindow)
                    {

                        if (!string.IsNullOrEmpty(await BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.DataTestId,
                            "page-message-button", valueType: ValueTypes.OuterHtml, delayBefore: 1)))
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.DataTestId, "page-message-button", delayAfter: 3);
                            //some time pages are asking get started to be sent automatically
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_2xh6 _2xh7", index: 0);
                        }
                        MessageWindowBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.MessageWindowButtonScript}[0].getBoundingClientRect().x"
                                               , customScriptY: $"{FdConstants.MessageWindowButtonScript}[0].getBoundingClientRect().y");
                        if (MessageWindowBtnLoc.Key != 0 && MessageWindowBtnLoc.Value != 0 && MessageWindowBtnLoc.Value > 60 && MessageWindowBtnLoc.Value < ScreenResolution.Value - 200)
                            BrowserWindow.MouseClick(MessageWindowBtnLoc.Key + 15, MessageWindowBtnLoc.Value + 9, delayAfter: 5);
                        else
                            resp = BrowserWindow.ExecuteScript($"{FdConstants.MessageWindowButtonScript}[0].click()", delayInSec: 5);
                    }
                    if (openWindow && MessageWindowBtnLoc.Key == 0 && MessageWindowBtnLoc.Value == 0 && !resp.Success)
                        messageDetails = new FdSendTextMessageResponseHandler(new ResponseParameter() { Response = "", HasError = true }) { Status = false, IsMessageSent = false, FbErrorDetails = new FdErrorDetails() { Description = "Messaging to User Not Available !" } };
                    else
                        messageDetails = await SendMessageToFriends(account, userId, message, isLinkPreviewActive, mediaPath, medias);
                }
                catch (Exception ex)
                { ex.DebugLog(); }
                finally
                { isRunning = false; }
            });
            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();
            return messageDetails;
        }

        public async Task<FdSendTextMessageResponseHandler> SendMessageToFriends(DominatorAccountModel account, string entityId, string message,
            bool isLinkPreviewActive = false, string mediaPath = "", List<string> medias = null)
        {
            try
            {
                var isSentBtnClicked = false;
                int tempCount = 0;
                if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.GetStartedButtonScript}.length")).Result.ToString(), out tempCount) && tempCount > 0)
                {
                    var xy = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.GetStartedButtonScript}[0].getBoundingClientRect().x",
                        customScriptY: $"{FdConstants.GetStartedButtonScript}[0].getBoundingClientRect().y");
                    if (xy.Key != 0 && xy.Value != 0)
                        await BrowserWindow.MouseClickAsync(xy.Key + 8, xy.Value + 4, delayAfter: 3);
                }
                if (!string.IsNullOrEmpty(mediaPath) || medias?.Count > 0)
                {
                    BrowserWindow.ChooseFileFromDialog(mediaPath, medias);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    KeyValuePair<int, int> imageBtnLoc = new KeyValuePair<int, int>();
                    imageBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: "document.getElementsByClassName('x1lytzrv x1t2pt76 x7ja8zs x1qrby5j x3nfvp2')[0].querySelectorAll('[aria-label=\"Attach a file\"]')[0].getBoundingClientRect().x",
                            customScriptY: "document.getElementsByClassName('x1lytzrv x1t2pt76 x7ja8zs x1qrby5j x3nfvp2')[0].querySelectorAll('[aria-label=\"Attach a file\"]')[0].getBoundingClientRect().y");

                    if (imageBtnLoc.Key == 0 && imageBtnLoc.Value == 0)
                        imageBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label*=\"Attach a file\"]')[0].getBoundingClientRect().x",
                            customScriptY: "document.querySelectorAll('[aria-label*=\"Attach a file\"]')[0].getBoundingClientRect().y");
                    if (imageBtnLoc.Key != 0 && imageBtnLoc.Value != 0)
                        await BrowserWindow.MouseClickAsync(imageBtnLoc.Key + 10, imageBtnLoc.Value + 10, delayAfter: 10);
                }
                if (message != null && !string.IsNullOrEmpty(message))
                {
                    KeyValuePair<int, int> textElement = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.MessageBoxScript}[0].getBoundingClientRect().x",
                           customScriptY: $"{FdConstants.MessageBoxScript}[0].getBoundingClientRect().y");
                    if (textElement.Key == 0 && textElement.Value == 0)
                        textElement = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[role=\"textbox\"]')[[...document.querySelectorAll('[role=\"textbox\"]')].length-1].getBoundingClientRect().x",
                        customScriptY: $"document.querySelectorAll('[role=\"textbox\"]')[[...document.querySelectorAll('[role=\"textbox\"]')].length-1].getBoundingClientRect().y");

                    if (textElement.Key != 0 && textElement.Value != 0)
                        await BrowserWindow.MouseClickAsync(textElement.Key + 13, textElement.Value + 7, delayAfter: 4);
                    await BrowserWindow.CopyPasteContentAsync($"{message} ", winKeyCode: 86, delayAtLast: 5);
                }
                KeyValuePair<int, int> sendBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.SendMessageButtonScript}[0].getBoundingClientRect().x"
                                            , customScriptY: $"{FdConstants.SendMessageButtonScript}[0].getBoundingClientRect().y");
                if (sendBtnLoc.Key != 0 && sendBtnLoc.Value != 0)
                {
                    BrowserWindow.MouseClick(sendBtnLoc.Key + 17, sendBtnLoc.Value + 14, delayAfter: 7);
                    isSentBtnClicked = true;
                }
                else
                    isSentBtnClicked = BrowserWindow.ExecuteScript($"{FdConstants.SendMessageButtonScript}[0].click()", delayInSec: 7).Success;
                if (!string.IsNullOrEmpty(await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "_10 captcha_dialog _3pod uiLayer _4-hy _3qw")))
                {
                    var frame = await BrowserWindow.GetFrame("https://www.google.com/recaptcha/api2/anchor");

                    await BrowserWindow.ExecuteJSAsyncFromFrame(frame, "document.getElementsByClassName('recaptcha-checkbox-border')[0].click();");

                    await BrowserWindow.ExecuteJSAsyncFromFrame(frame, "document.getElementsByClassName('rc-anchor-center-container')[0].click();");
                }
                List<string> sentList = new List<string>();
                sentList = await BrowserWindow.GetListInnerHtml(ActType.GetValue,
                    AttributeType.ClassName, "x78zum5 xdt5ytf x193iq5w x1n2onr6 xuk3077", ValueTypes.InnerText);

                if (sentList.Count() == 0)
                {
                    var emojiList = await BrowserWindow.GetListInnerHtmlChildElement(ActType.GetValue, AttributeType.ClassName,
                        "x78zum5 xdt5ytf x193iq5w x1n2onr6 xuk3077", AttributeType.ClassName, "xxymvpz x10w6t97 x1td3qas", parentIndex: 0);
                    emojiList.Reverse();
                    List<string> emojis = new List<string>();
                    emojiList.ForEach(x =>
                    {
                        emojis.Add(Regex.Match(x, "alt=\"(.*?)\"").Groups[1].ToString());
                    });
                    var sb = new StringBuilder();
                    emojis.ForEach(s => sb.Append(s));
                    sentList.Add(sb.ToString());
                }
                sentList.RemoveAll(x => x == "");
                sentList.Add(sentList.Any(x => x.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Contains(message?.Trim().Replace("\r\n", string.Empty).Replace("\n", string.Empty))) || isSentBtnClicked ? "message_id" : string.Empty);

                await CloseAllMessageDialogues();
                return new FdSendTextMessageResponseHandler(new ResponseParameter() { Response = sentList.FirstOrDefault(x => x.Contains("message_id")) ?? string.Empty });


            }
            catch (Exception) { }

            return null;
        }
        public async Task CloseAllMessageDialogues(string ariaLabel = "Close chat")
        {
            var openedChatWindows = await BrowserWindow.GetElementValueAsync(ActType.Focus, AttributeType.AriaLabel,
                ariaLabel);
            while (!string.IsNullOrEmpty(openedChatWindows))
            {
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, ariaLabel, delayAfter: 3);
                openedChatWindows = await BrowserWindow.GetElementValueAsync(ActType.Focus, AttributeType.AriaLabel,
                ariaLabel);
            }
        }

        public IResponseHandler GetFullPostDetails(DominatorAccountModel account,
           FacebookPostDetails facebookPostDetails, bool isCustomPost = false)
        {
            bool isRunning = true;

            bool isCookiesCleared = false;
            List<string> jsonResponseList = new List<string>();
            if (facebookPostDetails.PostUrl.StartsWith(FdConstants.FbHomeUrlMobile))
            {
                facebookPostDetails.PostUrl = facebookPostDetails.PostUrl?.Replace(FdConstants.FbHomeUrlMobile, FdConstants.FbHomeUrl);
            }
            if (!facebookPostDetails.PostUrl.StartsWith(FdConstants.FbWatchVideoUrl))
            {
                facebookPostDetails.PostUrl = facebookPostDetails.PostUrl.StartsWith(FdConstants.FbHomeUrl)
                            ? facebookPostDetails.PostUrl : FdConstants.FbHomeUrl + facebookPostDetails.PostUrl;
            }
            if (facebookPostDetails.PostUrl.Contains("/videos/"))
                facebookPostDetails.PostUrl = facebookPostDetails.PostUrl?.Replace("/videos/", "/posts/");
            var pageSource = LoadPageSource(account, facebookPostDetails.PostUrl, clearandNeedResource: true, isNewWindow: isCustomPost);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(facebookPostDetails.ScapedUrl))
                        facebookPostDetails.ScapedUrl = facebookPostDetails.PostUrl;
                    if (!isCustomPost)
                    {
                        jsonResponseList = await BrowserWindow.GetPaginationDataList("{\"__typename\":\"Story\"", true);
                        if (jsonResponseList.Count() == 0)
                            jsonResponseList = await BrowserWindow.GetPaginationDataList("{\"__typename\":\"Photo\"", true);
                    }
                    else
                    {
                        jsonResponseList = await CustomBrowserWindow.GetPaginationDataList("{\"__typename\":\"Story\"", true);
                    }
                    facebookPostDetails.IsActorChangeable = pageSource.Contains("aria-label=\"Voice Selector\"") || pageSource.Contains("aria-label=\"Available Voices\"");

                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new BrowserPostDetailsResponseHandler(new ResponseParameter() { Response = pageSource }, jsonResponseList, facebookPostDetails, isCookiesCleared, false);


        }

      
        public string GetFullGroupDetails(DominatorAccountModel account,
           string groupUrl, bool isNewWindow = true)
        {
            bool isRunning = true;

            string groupId = string.Empty;

            string pageSource = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    if (isNewWindow)
                    {
                        BrowserWindow = new BrowserWindow(account, targetUrl: groupUrl, customUse: true)
                        {
                            Visibility = Visibility.Hidden
                        };
#if DEBUG
                        BrowserWindow.Visibility = Visibility.Visible;
#endif
                        BrowserWindow.BrowserSetCookie();
                        BrowserWindow.Show();
                    }
                    else
                        await BrowserWindow.GoToCustomUrl(groupUrl, delayAfter: 5);
                    if (!BrowserWindow.IsLoaded)
                        await Task.Delay(6000, cancellationToken);
                    pageSource = await BrowserWindow.GetPageSourceAsync();

                    groupId = FdRegexUtility.FirstMatchExtractor(pageSource, FdConstants.EntityIdRegex);
                    if (string.IsNullOrEmpty(groupId))
                        groupId = FdRegexUtility.FirstMatchExtractor(pageSource, "\"groupID\":\"(.*?)\"");
                    if (string.IsNullOrEmpty(groupId))
                        groupId = FdRegexUtility.FirstMatchExtractor(groupUrl + "/", $"https://www.facebook.com/groups/(.*?)/");

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    if (isNewWindow)
                        BrowserWindow.Close();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();

            return groupId;
        }
        private string gotoPOstUrl(DominatorAccountModel account, FacebookPostDetails facebookPostDetails)
        {
            var pageSource = "";
            var url = "";
            if (BrowserWindow != null)
            {
                pageSource = BrowserWindow.GetPageSource();
                url = BrowserWindow.CurrentUrl();
            }
            if ((facebookPostDetails.ScapedUrl.Contains("/reel/") || facebookPostDetails.PostUrl.Contains("/reel/")) && !url.Contains(facebookPostDetails.ScapedUrl))
                pageSource = LoadPageSource(account, facebookPostDetails.ScapedUrl);
            else if (!url.Contains(facebookPostDetails.ScapedUrl) && !url.Contains(facebookPostDetails.Id) && (string.IsNullOrEmpty(facebookPostDetails.PostUrl) || !url.Contains(facebookPostDetails.PostUrl)))
            {
                if (!string.IsNullOrEmpty(facebookPostDetails.PostUrl))
                    pageSource = LoadPageSource(account, facebookPostDetails.PostUrl);
                else
                    pageSource = LoadPageSource(account, facebookPostDetails.ScapedUrl);
            }
            return pageSource;
        }

        public bool LikePost(DominatorAccountModel account, FacebookPostDetails facebookPostDetails,
           ReactionType reactionType, FanpageDetails ownpageDetails = null)
        {
            bool isRunning = true;

            string response = string.Empty;
            gotoPOstUrl(account, facebookPostDetails);
            if (ownpageDetails != null && !string.IsNullOrEmpty(facebookPostDetails.LikePostAsPageId) && !isActorChangedtoFanPage)
                SwitchToOwnPage(account, ownpageDetails?.FanPageUrl, ownpageDetails);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    int index = 0;
                    if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.LikeButtonScript}.length"))?.Result?.ToString(), out int lengthLike) && lengthLike > 0)
                    {
                        index = lengthLike > 1 ? lengthLike - 1 : 0;
                        await BrowserWindow.ExecuteScriptAsync($"{FdConstants.LikeButtonScript}[{index}].scrollIntoViewIfNeeded()", delayInSec: 4);
                        if (reactionType.GetDescriptionAttr() == BrowserReactionType.Like.GetDescriptionAttr())
                        {
                            await BrowserWindow.ExecuteScriptAsync($"{FdConstants.LikeButtonScript}[{index}].click()", delayInSec: 5);
                        }
                        else
                        {
                            KeyValuePair<int, int> likeButtonPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.LikeButtonScript}[{index}].getBoundingClientRect().x",
                                customScriptY: $"{FdConstants.LikeButtonScript}[{index}].getBoundingClientRect().y");
                            if (likeButtonPosition.Value < 80 || likeButtonPosition.Value > ScreenResolution.Value - 200)
                            {
                                await BrowserWindow.ExecuteScriptAsync($"{FdConstants.LikeButtonScript}[{index}].scrollIntoViewIfNeeded()", delayInSec: 4);
                                likeButtonPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.LikeButtonScript}[{index}].getBoundingClientRect().x",
                                customScriptY: $"{FdConstants.LikeButtonScript}[{index}].getBoundingClientRect().y");
                            }

                            var moved = BrowserWindow.MoveMouseAtLocation(likeButtonPosition.Key + 20, likeButtonPosition.Value + 15);
                            await BrowserWindow.MouseHoverAsync(likeButtonPosition.Key + 25, likeButtonPosition.Value + 15, delayAfter: 0.8);

                            KeyValuePair<int, int> reactionBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"Reactions\"]')[0].querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[0].getBoundingClientRect().x"
                                , customScriptY: $"document.querySelectorAll('[aria-label=\"Reactions\"]')[0].querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[0].getBoundingClientRect().y");

                            await BrowserWindow.MouseHoverAsync(reactionBtnLoc.Key + 13, reactionBtnLoc.Value + 10);

                            await BrowserWindow.MouseClickAsync(reactionBtnLoc.Key + 13, reactionBtnLoc.Value + 10, delayAfter: 5);
                        }
                    }
                    else if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.OtherReactionButtonScript}.length"))?.Result?.ToString(), out int lengthReaction) && lengthReaction > 0)
                    {
                        KeyValuePair<int, int> likeButtonPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.OtherReactionButtonScript}[0].getBoundingClientRect().x",
                               customScriptY: $"{FdConstants.OtherReactionButtonScript}[0].getBoundingClientRect().y");
                        if (likeButtonPosition.Value < 80 || likeButtonPosition.Value > ScreenResolution.Value - 200)
                        {
                            await BrowserWindow.ExecuteScriptAsync($"{FdConstants.OtherReactionButtonScript}[0].scrollIntoViewIfNeeded()", delayInSec: 4);
                            likeButtonPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.OtherReactionButtonScript}[0].getBoundingClientRect().x",
                               customScriptY: $"{FdConstants.OtherReactionButtonScript}[0].getBoundingClientRect().y");
                        }
                        var moved = BrowserWindow.MoveMouseAtLocation(likeButtonPosition.Key + 20, likeButtonPosition.Value + 15);
                        await BrowserWindow.MouseHoverAsync(likeButtonPosition.Key + 25, likeButtonPosition.Value + 15, delayAfter: 0.8);

                        KeyValuePair<int, int> reactionBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"Reactions\"]')[0].querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[0].getBoundingClientRect().x"
                            , customScriptY: $"document.querySelectorAll('[aria-label=\"Reactions\"]')[0].querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[0].getBoundingClientRect().y");

                        await BrowserWindow.MouseHoverAsync(reactionBtnLoc.Key + 13, reactionBtnLoc.Value + 10);

                        await BrowserWindow.MouseClickAsync(reactionBtnLoc.Key + 13, reactionBtnLoc.Value + 10, delayAfter: 5);
                    }

                    var ariaLabelAttribute = !string.IsNullOrEmpty(reactionType.ToString()) ? $"Remove {reactionType}" : "Remove Like";
                    if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AlreadyLikedButtonScript(ariaLabelAttribute)}.length"))?.Result?.ToString(), out int Likedlength) && Likedlength > 0)
                    {
                        response = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AlreadyLikedButtonScript(ariaLabelAttribute)}[{Likedlength - 1}].outerHTML"))?.Result?.ToString();

                    }
                    if (string.IsNullOrEmpty(response) && (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AlreadyLikedButtonScript("Active like")}.length"))?.Result?.ToString(), out int LikedActivelength) && LikedActivelength > 0))
                    {
                        response = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AlreadyLikedButtonScript("Active like")}[{LikedActivelength - 1}].outerHTML"))?.Result?.ToString();
                    }
                    if (string.IsNullOrEmpty(response) && (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AlreadyLikedButtonScript("Like")}.length"))?.Result?.ToString(), out int ReelLikedActivelength) && ReelLikedActivelength > 0))
                    {
                        response = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AlreadyLikedButtonScript("Like")}[{ReelLikedActivelength - 1}].outerHTML"))?.Result?.ToString();
                    }
                    response = lengthLike > 0 && (response.Contains($"Change {reactionType} reaction") || response.Contains("Active Like") || response.Contains("Active like") || response.Contains(ariaLabelAttribute)) ? "Liked"
                    : lengthLike == 0 && (response.Contains($"Change {reactionType} reaction") || response.Contains("Active Like") || (response.Contains("Like") && response.Contains("aria - pressed = \"true\"")) || response.Contains(ariaLabelAttribute)) ? "Already Liked" :
                    string.Empty;
                    isRunning = false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                }
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return !string.IsNullOrEmpty(response);

        }
        public IResponseHandler LikeComments(DominatorAccountModel account,
              FdPostCommentDetails commentDetails, ReactionType reactionType, FanpageDetails fanpageDetails = null)
        {
            bool isRunning = true;
            string response = null;
            if (!string.IsNullOrEmpty(fanpageDetails?.FanPageID) && !isActorChangedtoFanPage)
                SwitchToOwnPage(account, fanpageDetails.FanPageUrl, fanpageDetails);
            var commenturl = !string.IsNullOrEmpty(commentDetails.CommentUrl) && commentDetails.CommentUrl.StartsWith(FdConstants.FbHomeUrl) ? commentDetails.CommentUrl : $"{FdConstants.FbHomeUrl}{commentDetails.CommentId}";
            LoadPageSource(account, commenturl);
            Application.Current.Dispatcher.Invoke(async () =>
            {

                try
                {
                    response = (await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].InnerHtml"))?.Result?.ToString() ?? "";

                    if (response.Contains($"aria-label=\"Remove {reactionType.GetDescriptionAttr()}\"") || response.Contains($"aria-label=\"Change {reactionType.GetDescriptionAttr()}"))
                    {
                        response = "Already Liked Comment";
                    }
                    else
                    {
                        var likeBtnLoc = new KeyValuePair<int, int>();
                        //for other Than reaction Type if reacted and also Not Like
                        if (response.Contains("aria-label=\"Remove ") || response.Contains("aria-label=\"Change "))
                        {
                            likeBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label*=\"Remove \"]')[0].getBoundingClientRect().x",
                             customScriptY: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label*=\"Remove \"]')[0].getBoundingClientRect().y");
                            if (likeBtnLoc.Value < 80 || likeBtnLoc.Value > ScreenResolution.Value - 200)
                            {
                                await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label*=\"Remove \"]')[0].scrollIntoViewIfNeeded()", delayInSec: 4);
                                likeBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label*=\"Remove \"]')[0].getBoundingClientRect().x",
                                  customScriptY: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label*=\"Remove \"]')[0].getBoundingClientRect().y");
                            }
                        }
                        else
                        {
                            likeBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label=\"Like\"]')[0].getBoundingClientRect().x",
                            customScriptY: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label=\"Like\"]')[0].getBoundingClientRect().y");
                            if (likeBtnLoc.Value < 80 || likeBtnLoc.Value > ScreenResolution.Value - 200)
                            {
                                await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label=\"Like\"]')[0].scrollIntoViewIfNeeded()", delayInSec: 4);
                                likeBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label=\"Like\"]')[0].getBoundingClientRect().x",
                                  customScriptY: $"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[aria-label=\"Like\"]')[0].getBoundingClientRect().y");
                            }
                        }
                        if (likeBtnLoc.Key != 0 && likeBtnLoc.Value != 0)
                        {
                            await BrowserWindow.MouseHoverAsync(likeBtnLoc.Key + 3, likeBtnLoc.Value + 4, delayAfter: 5);

                            KeyValuePair<int, int> reactLocation = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[[...document.querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')].length-1].getBoundingClientRect().x",
                                customScriptY: $"document.querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[[...document.querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')].length-1].getBoundingClientRect().y");
                            if (reactLocation.Key != 0 && reactLocation.Value != 0)
                                await BrowserWindow.MouseClickAsync(reactLocation.Key + 5, reactLocation.Value + 5, delayAfter: 5);
                            else
                                await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')[[...document.querySelectorAll('[aria-label=\"{reactionType.GetDescriptionAttr()}\"]')].length-1].click()", delayInSec: 5);
                            await BrowserWindow.MouseHoverAsync(0, 0);
                        }

                    }
                    response = (await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].InnerHtml"))?.Result?.ToString() ?? "errorSummary";


                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new CommentLikerResponseHandler(new ResponseParameter() { Response = response }, reactionType);

        }


        public IResponseHandler ReplyToComments(DominatorAccountModel account, FdPostCommentDetails commentDetails,
            string comment, string postAsPageId = "", FanpageDetails fanpageDetails = null, Dictionary<string, string> mentionDictionary = null, bool isMentionCommentedUser = false)
        {
            bool isRunning = true;
            string responseData = string.Empty;
            var commentUrl = !string.IsNullOrEmpty(commentDetails.CommentUrl) && commentDetails.CommentUrl.StartsWith(FdConstants.FbHomeUrl) ? commentDetails.CommentUrl : $"{FdConstants.FbHomeUrl}{commentDetails.CommentId}";
            var pageSource = LoadPageSource(account, commentUrl);
            if (pageSource.Contains("This content isn't available at the moment"))
                return null;
            if (fanpageDetails != null && !string.IsNullOrEmpty(postAsPageId) && !isActorChangedtoFanPage)
                SwitchToOwnPage(account, fanpageDetails?.FanPageUrl, fanpageDetails);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await ClickOrScrollToViewAndClick($"{FdConstants.ReplytoCommentButtonScript}[0]", timetoWait: 2);
                    if (isMentionCommentedUser)
                        await ClickOrScrollToViewAndClick($"[...{string.Format(FdConstants.PostCommentScript, commentDetails.CommenterName)}[0].querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent?.includes('Reply'))[0]", timetoWait: 2);
                    if (mentionDictionary.Count > 0)
                    {
                        await AddMentionsForComment(mentionDictionary, activityType: "ReplyToComment");
                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 32, delayAtLast: 0.2);
                    }
                    await BrowserWindow.CopyPasteContentAsync(mentionDictionary.Count > 0 ? $" {comment} " : $"{comment} ", winKeyCode: 86, delayAtLast: 4);
                    BrowserWindow.SetResourceLoadInstance();
                    await BrowserWindow.PressAnyKeyUpdated(13, 2);
                    await Task.Delay(8000, cancellationToken);

                    List<string> paginationList = await BrowserWindow.GetPaginationDataList("{\"data\":{", true);

                    responseData = paginationList != null && paginationList.Count > 0 ?
                      paginationList.FirstOrDefault(x => x.Contains("\"comment_create\":")) : string.Empty;
                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new CommentOnPostResponseHandler(new ResponseParameter() { Response = responseData }, true);
        }

        public bool CommentOnSinglePost(DominatorAccountModel accountModel, FacebookPostDetails post, List<string> comments, string postAsPageId = "",
            Dictionary<string, string> mentionDictionary = null, FanpageDetails fanpageDetails = null, bool IsCustomPost = false)
        {
            bool isRunning = true;
            bool isCommented = false;
            var pageSource = gotoPOstUrl(accountModel, post);
            var outerHtml = HtmlParseUtility.GetOuterHtmlFromTagName(pageSource, "div", "class", "xzsf02u x1a2a7pz x1n2onr6 x14wi4xw notranslate");
           
            if (string.IsNullOrEmpty(outerHtml) ? false : !outerHtml.Contains("contenteditable=\"true\""))
                return false;
            if (fanpageDetails != null && !string.IsNullOrEmpty(post.LikePostAsPageId) && !isActorChangedtoFanPage)
                SwitchToOwnPage(accountModel, fanpageDetails?.FanPageUrl, fanpageDetails);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var index = 0;
                    foreach (var comment in comments)
                    {
                        int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentTextBoxScript}.length"))?.Result?.ToString(), out int lengthCommentBoxes);
                        index = lengthCommentBoxes > 1 ? lengthCommentBoxes - 1 : 0;
                        var resp = await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentTextBoxScript}[{index}].scrollIntoViewIfNeeded()", delayInSec: 4);
                        if (resp.Success)
                        {
                            int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentWriteTextBoxScript}.length"))?.Result?.ToString(), out int lengthWriteCommentBoxes);
                            KeyValuePair<int, int> xAndYCommentWriteBox = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.CommentWriteTextBoxScript}[{lengthWriteCommentBoxes - 1}].getBoundingClientRect().x"
                                               , customScriptY: $"{FdConstants.CommentWriteTextBoxScript}[{lengthWriteCommentBoxes - 1}].getBoundingClientRect().y");
                            if (xAndYCommentWriteBox.Key != 0 && xAndYCommentWriteBox.Value != 0 && xAndYCommentWriteBox.Key > 100 && xAndYCommentWriteBox.Value > 100)
                                await ClickOrScrollToViewAndClick($"{FdConstants.CommentWriteTextBoxScript}[{lengthWriteCommentBoxes - 1}]", timetoWait: 3);
                            else
                            {
                                KeyValuePair<int, int> xAndYCommentButton = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.CommentButtonBoxScript}[{index}].getBoundingClientRect().x"
                                                   , customScriptY: $"{FdConstants.CommentButtonBoxScript}[{index}].getBoundingClientRect().y");
                                if (xAndYCommentButton.Key != 0 && xAndYCommentButton.Value != 0 && xAndYCommentButton.Key > 100 && xAndYCommentButton.Value > 100)
                                    await ClickOrScrollToViewAndClick($"{FdConstants.CommentButtonBoxScript}[{index}]", timetoWait: 4);

                                int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentWriteTextBoxScript}.length"))?.Result?.ToString(), out int lengthWriteCommentBoxes2);
                                xAndYCommentWriteBox = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.CommentWriteTextBoxScript}[{lengthWriteCommentBoxes2 - 1}].getBoundingClientRect().x"
                                              , customScriptY: $"{FdConstants.CommentWriteTextBoxScript}[{lengthWriteCommentBoxes2 - 1}].getBoundingClientRect().y");
                                if (xAndYCommentWriteBox.Key != 0 && xAndYCommentWriteBox.Value != 0)
                                    await ClickOrScrollToViewAndClick($"{FdConstants.CommentWriteTextBoxScript}[{lengthWriteCommentBoxes2 - 1}]", timetoWait: 3);
                            }
                            if (mentionDictionary != null && mentionDictionary.Count > 0)
                                await AddMentionsForComment(mentionDictionary);

                            BrowserWindow.SetResourceLoadInstance();
                            if (!string.IsNullOrEmpty(comment))
                                await BrowserWindow.CopyPasteContentAsync(comment.Length < 20 ? comment + " " : comment, 86, delayAtLast: 3);
                            await BrowserWindow.PressAnyKeyUpdated(13, delayAtLast: 10);
                            pageSource = BrowserWindow.GetPageSource();
                            var responseList = await BrowserWindow.GetPaginationDataList("comment_create", true);
                            BrowserWindow.SetResourceLoadInstance(false);
                            BrowserWindow.ClearResources();
                            isCommented = responseList.Count > 0 && responseList.Any(x => x.Contains("\"comment_create\":"));

                            if (!isCommented)
                            {
                                List<string> commentlist = await BrowserWindow.GetListInnerHtmlChildElement(ActType.GetValue, parentAttributeType: AttributeType.ClassName
                                    , parentAttributeValue: FdConstants.PostElementClass, childAttributeName: AttributeType.ClassName,
                                    childAttributeValue: "ni8dbmo4 stjgntxs hv4rvrfc", valueType: ValueTypes.OuterHtml);
                                commentlist.Reverse();

                                isCommented = commentlist.Count() > 0 ? commentlist.LastOrDefault().Contains(comment)
                                || commentlist.FirstOrDefault().Contains(comment) : false;
                            }
                        }
                    }

                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return isCommented;
        }

        private async Task AddMentionsForComment(Dictionary<string, string> mentionDictionary,
            string activityType = "Comment")
        {

            foreach (KeyValuePair<string, string> mentionDetails in mentionDictionary)
            {
                try
                {
                    int tryCount = 0;

                    bool isFoundUsers = false;

                    string mentionText = mentionDetails.Value;

                    string[] splitMention = mentionText.Split(' ');
                    List<string> listMentionUsers = new List<string>();
                    SearchPeopleResponseHandler mentionDetailsResponseHandler = null;

                    while (tryCount++ < 2 && !isFoundUsers)
                    {

                        BrowserWindow.SetResourceLoadInstance();
                        await BrowserWindow.EnterCharsAsync($"@{mentionText}", delayAtLast: 2);

                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 13, delayAtLast: 2);
                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 32, delayAtLast: 2);
                        listMentionUsers = await BrowserWindow.GetPaginationDataList("\"data\":{\"comet_composer_typeahead_search\"", true, "\"__typename\":\"User\"");
                        if (listMentionUsers.Count == 0)
                            listMentionUsers = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);
                        mentionDetailsResponseHandler = new SearchPeopleResponseHandler(new List<string>() { listMentionUsers.LastOrDefault() }, new ResponseParameter() { Response = "" }, new List<string>(), false);
                        BrowserWindow.SetResourceLoadInstance(false);
                        BrowserWindow.ClearResources();
                        isFoundUsers = true;
                    }
                    if (isFoundUsers && mentionDetailsResponseHandler.ObjFdScraperResponseParameters.ListUser.Count > 0)
                    {
                        string currentUserImage = "";
                        currentUserImage = mentionDetailsResponseHandler.ObjFdScraperResponseParameters.ListUser.
                            FirstOrDefault(x => x.UserId == mentionDetails.Key || x.ProfileId == mentionDetails.Key || x.Username == mentionDetails.Value) != null ? mentionDetailsResponseHandler.ObjFdScraperResponseParameters.ListUser.
                            FirstOrDefault(x => x.UserId == mentionDetails.Key || x.ProfileId == mentionDetails.Key || x.Username == mentionDetails.Value).ScrapedProfileUrl : currentUserImage;

                        if (string.IsNullOrEmpty(currentUserImage))
                            currentUserImage = mentionDetailsResponseHandler.ObjFdScraperResponseParameters.ListUser.FirstOrDefault()?.ScrapedProfileUrl;

                        if (string.IsNullOrEmpty(currentUserImage))
                        {
                            BrowserWindow.SelectAllText();
                            await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                            continue;
                        }
                        List<string> pageMentionList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery,
                            AttributeType.Role, "option");
                        if (pageMentionList.Count > 0)
                        {
                            pageMentionList.Reverse();

                            int indexMentionUser = pageMentionList.IndexOf(pageMentionList.FirstOrDefault(x => x.Contains(currentUserImage)));

                            KeyValuePair<int, int> userLocation = new KeyValuePair<int, int>();
                            if (indexMentionUser == -1)
                            {
                                userLocation = await BrowserWindow.GetXAndYAsync(customScriptX: FdConstants.GetlocFromRoleOptionXCoordinate()
                                    , customScriptY: FdConstants.GetlocFromRoleOptionYCoordinate());
                                if (userLocation.Key != 0 && userLocation.Value != 0)
                                    await BrowserWindow.MouseClickAsync(userLocation.Key, userLocation.Value, delayAfter: 5);
                            }
                            else
                            {
                                userLocation = await BrowserWindow.GetXAndYAsync(customScriptX: FdConstants.GetlocFromRoleOptionXCoordinate(indexMentionUser)
                                    , customScriptY: FdConstants.GetlocFromRoleOptionYCoordinate(indexMentionUser));
                                if (userLocation.Key != 0 && userLocation.Value != 0)
                                    await BrowserWindow.MouseClickAsync(userLocation.Key, userLocation.Value, delayAfter: 5);
                            }
                        }
                    }

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            BrowserWindow.SetResourceLoadInstance(false);
        }
        public bool AcceptCancelFriendRequest(DominatorAccountModel account, FacebookUser facebookUser,
            string activityClassName, string queryType)
        {
            bool isRunning = true;
            string response = null;
            LoadPageSource(account, facebookUser.ProfileUrl, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (queryType.Contains("Accept"))
                    {

                        KeyValuePair<int, int> confirmBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.ConfirmReqButtonScript}[0].getBoundingClientRect().x"
                                           , customScriptY: $"{FdConstants.ConfirmReqButtonScript}[0].getBoundingClientRect().y");
                        if (confirmBtnLoc.Key != 0 && confirmBtnLoc.Value != 0 && confirmBtnLoc.Value > 100 && confirmBtnLoc.Value < ScreenResolution.Value - 200)
                            BrowserWindow.MouseClick(confirmBtnLoc.Key + 15, confirmBtnLoc.Value + 9, delayAfter: 4);
                        else
                            BrowserWindow.ExecuteScript($"{FdConstants.ConfirmReqButtonScript}[0].click()", delayInSec: 4);
                    }
                    else
                    {
                        KeyValuePair<int, int> delBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.DeleteReqButtonScript}[0].getBoundingClientRect().x"
                                           , customScriptY: $"{FdConstants.DeleteReqButtonScript}[0].getBoundingClientRect().y");
                        if (delBtnLoc.Key != 0 && delBtnLoc.Value != 0 && delBtnLoc.Value > 100 && delBtnLoc.Value < ScreenResolution.Value - 200)
                            BrowserWindow.MouseClick(delBtnLoc.Key + 15, delBtnLoc.Value + 9, delayAfter: 4);
                        else
                            BrowserWindow.ExecuteScript($"{FdConstants.DeleteReqButtonScript}[0].click()", delayInSec: 4);
                    }
                    List<string> responseList = await BrowserWindow.GetPaginationDataList("friend_requester", true);
                    response = responseList.LastOrDefault(x => x.Contains(facebookUser.UserId) || x.Contains(facebookUser.ScrapedProfileUrl));
                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance(false);
                }
                catch (Exception) { }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            return !string.IsNullOrEmpty(response);

        }


        public bool CancelSentRequest(DominatorAccountModel account, FacebookUser facebookUser)
        {
            bool isRunning = true;
            var isCanceled = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CancelSendReqButtonScript(facebookUser?.Username, facebookUser?.UserId)}[0].scrollIntoViewIfNeeded()", delayInSec: 2);
                    isCanceled = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CancelSendReqButtonScript(facebookUser?.Username, facebookUser?.UserId)}[0].click()")).Success;
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;

            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return isCanceled;
        }

        public void OpenMessengerWindow(DominatorAccountModel account)
        {
            bool isRunning = true;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Messenger", delayAfter: 3);

                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;

            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

        }
        private async Task<string> GetMessageResponse()
        {
            var Response = string.Empty;
            try
            {
                var Page = await BrowserWindow.GetPageSourceAsync();
                Response = Regex.Match(Page, "{\"data\":{\"viewer\":{\"lightspeed_web_request\":{\"payload\":\"(.*?)\",\"dependencies\":").Groups[1].Value;
                if (string.IsNullOrEmpty(Response))
                {
                    var dataList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"viewer\":{\"lightspeed_web_request\":{\"payload\":", true);
                    dataList.RemoveAll(x => !x.Contains("truncateTablesForSyncGroup"));
                    Response = Regex.Match(dataList[0].ToString(), "{\"data\":{\"viewer\":{\"lightspeed_web_request\":{\"payload\":\"(.*?)\",\"dependencies\":").Groups[1].Value;
                }
                if (Response.Length < 300 || !Response.Contains("truncateTablesForSyncGroup"))
                {
                    var Split = Regex.Split(Page, "<script type=\"application/json\"");
                    if (Split.Length > 0)
                    {
                        var Res = Split.FirstOrDefault(x => x.Contains("truncateTablesForSyncGroup"));
                        Response = !string.IsNullOrEmpty(Res) ? Regex.Match(Res, "{\"data\":{\"viewer\":{\"lightspeed_web_request\":{\"payload\":\"(.*?)\",\"dependencies\":").Groups[1].Value : Response;
                    }
                }
            }
            catch (Exception) { }
            Response = Response.Replace(@"\", "");
            return Response;
        }
        public void SwitchToPendingMessageRequest(DominatorAccountModel account)
        {
            string pageSource = LoadPageSource(account, $"{FdConstants.FbHomeUrl}messages/", true);
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var OptionXY = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"Settings, help and more\"]')[0].getBoundingClientRect().x",
                             customScriptY: $"document.querySelectorAll('[aria-label=\"Settings, help and more\"]')[0].getBoundingClientRect().y");
                    if (OptionXY.Key != 0 && OptionXY.Value != 0)
                        await BrowserWindow.MouseClickAsync(OptionXY.Key + 10, OptionXY.Value + 5, delayAfter: 5);
                    var menuList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", ValueTypes.InnerText);
                    menuList.Reverse();
                    var requestindex = menuList.FindIndex(x => x.Contains("Message requests"));
                    var xandYmenu = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[role=\"menuitem\"]')[{requestindex}].getBoundingClientRect().x",
                        customScriptY: $"document.querySelectorAll('[role=\"menuitem\"]')[{requestindex}].getBoundingClientRect().y");
                    if (xandYmenu.Key != 0 && xandYmenu.Value != 0)
                        await BrowserWindow.MouseClickAsync(xandYmenu.Key + 20, xandYmenu.Value + 5, delayAfter: 5);
                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1500, cancellationToken).Wait();

        }
        public IResponseHandler ScrollWindowAndGetUnRepliedMessages(DominatorAccountModel account, MessageType messageType
        , int noOfPageToScroll, int lastPageNo = 0)
        {

            int failedCount = 0;
            bool isRunning = true;
            var messageDetailsList = new List<FdMessageDetails>();
            var listChatDetails = new List<ChatDetails>();
            var messageSenderDetailsList = new List<SenderDetails>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.Refresh();
                    Task.Delay(5000, cancellationToken).Wait();
                    var JsonResp = await GetMessageResponse();
                    var messageRespHandler = new FdGetAllMessagedUserResponseHandler(new ResponseParameter() { Response = JsonResp }, account, messageType, true);
                    messageDetailsList.AddRange(messageRespHandler.ObjFdScraperResponseParameters.MessageDetailsList);
                    listChatDetails.AddRange(messageRespHandler.ObjFdScraperResponseParameters.ListChatDetails);
                    messageSenderDetailsList.AddRange(messageRespHandler.ObjFdScraperResponseParameters.MessageSenderDetailsList);
                    lastPageNo++;
                    while (lastPageNo < noOfPageToScroll && failedCount < 2)
                    {
                        BrowserWindow.ClearResources();
                        BrowserWindow.Refresh();
                        Task.Delay(5000, cancellationToken).Wait();
                        JsonResp = await GetMessageResponse();
                        await BrowserWindow.ExecuteScriptAsync($"{FdConstants.MessageChatsScript}.scrollIntoView()", 5);
                        if (!string.IsNullOrEmpty(JsonResp))
                        {
                            messageRespHandler = new FdGetAllMessagedUserResponseHandler(new ResponseParameter() { Response = JsonResp }, account, messageType);
                            messageRespHandler.ObjFdScraperResponseParameters.MessageDetailsList.RemoveAll(x => messageDetailsList.Any(y => y.MessageSenderId == x.MessageSenderId));
                            messageRespHandler.ObjFdScraperResponseParameters.ListChatDetails.RemoveAll(x => listChatDetails.Any(y => y.SenderId == x.SenderId));
                            messageRespHandler.ObjFdScraperResponseParameters.MessageSenderDetailsList.RemoveAll(x => messageSenderDetailsList.Any(y => y.SenderId == x.SenderId));
                            messageDetailsList.AddRange(messageRespHandler.ObjFdScraperResponseParameters.MessageDetailsList);
                            listChatDetails.AddRange(messageRespHandler.ObjFdScraperResponseParameters.ListChatDetails);
                            messageSenderDetailsList.AddRange(messageRespHandler.ObjFdScraperResponseParameters.MessageSenderDetailsList);
                        }
                        lastPageNo++;
                    }
                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();
            return new FdGetAllMessagedUserResponseHandler(new ResponseParameter() { Response = "" }, failedCount < 2, failedCount < 2)
            {
                ObjFdScraperResponseParameters = new FdScraperResponseParameters()
                {
                    MessageDetailsList = messageDetailsList,
                    MessageSenderDetailsList = messageSenderDetailsList,
                    ListSenderDetails = listChatDetails
                }
            };

        }

        public IResponseHandler ScrollWindowAndGetMessages(DominatorAccountModel account
          , int noOfPageToScroll, string otherUserId, int lastPageNo = 0)
        {
            MessageType messageType = MessageType.Pending;
            int failedCount = 0;

            bool isRunning = true;

            List<string> itemList = new List<string>();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int previousCount = 0;

                    await Task.Delay(5000, cancellationToken);

                    if (isRunning)
                    {

                        while (lastPageNo < noOfPageToScroll && failedCount <= 5)
                        {
                            lastPageNo++;

                            int currentCount = await BrowserWindow.GetItemCountInnerHtml(ActType.GetValue,
                                AttributeType.ClassName, FdConstants.SentFriendElement);

                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName,
                                FdConstants.SentFriendElement, index: currentCount - 1, delayAfter: 4);

                            await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                                , 0, -500, delayAfter: 4, scrollCount: 8);

                            if (currentCount == previousCount)
                            {
                                await Task.Delay(2000, cancellationToken);
                                failedCount++;
                            }
                            else
                            {
                                failedCount = 0;
                                previousCount = currentCount;
                            }
                        }

                        await Task.Delay(1000, cancellationToken);

                        itemList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, FdConstants.SentFriendElement, ValueTypes.OuterHtml);
                    }

                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();


            return new FdGetAllMessagedUserResponseHandler(new ResponseParameter(), itemList, account.AccountBaseModel.UserId
                , account.AccountBaseModel.UserFullName, messageType, account.AccountId, failedCount > 5);

        }



        public bool OpenFriendLinkAndSendMessage(DominatorAccountModel account, FacebookUser facebookUser,
            bool closeDialog = true, bool isCheckMessage = false)
        {
            bool isRunning = true;

            bool isSentMessage = false;

            string pageSource = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int scrollCount = 10;

                    if (BrowserWindow == null)
                        isRunning = false;

                    List<string> userThreadList = await BrowserWindow.GetListInnerHtml(ActType.GetValue,
                           AttributeType.ClassName, facebookUser.ClassName, valueType: ValueTypes.OuterHtml);
                    string className = facebookUser.ClassName;

                    userThreadList.Reverse();

                    string userdetails = string.IsNullOrEmpty(facebookUser.UserId) || facebookUser.UserId == "0"
                    ? facebookUser.ScrapedProfileUrl
                    : facebookUser.UserId;

                    int currentThreadIndex = userThreadList.IndexOf(userThreadList.FirstOrDefault(x =>
                                x.Contains(userdetails)));

                    if (!string.IsNullOrEmpty(facebookUser.ScrapedProfileUrl) && !string.IsNullOrEmpty(facebookUser.UserId))
                        currentThreadIndex = userThreadList.IndexOf(userThreadList.FirstOrDefault(x =>
                                x.Contains(facebookUser.ScrapedProfileUrl) || x.Contains(facebookUser.UserId)));

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, attributeValue: "Close chat", delayAfter: 2);
                    await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, className,
                        index: currentThreadIndex, delayAfter: 0.5);

                    await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                        , 0, 150, delayAfter: 1, scrollCount: 1);

                    KeyValuePair<int, int> ownprofilePosition = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, className, index: 0);

                    KeyValuePair<int, int> elementPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.getElementsByClassName('{FdConstants.FriendUser2Element}')[{currentThreadIndex}].getElementsByClassName('a5q79mjw g1cxx5fr lrazzd5p oo9gr5id')[0].getBoundingClientRect().x"
                                    , customScriptY: $"document.getElementsByClassName('{FdConstants.FriendUser2Element}')[{currentThreadIndex}].getElementsByClassName('a5q79mjw g1cxx5fr lrazzd5p oo9gr5id')[0].getBoundingClientRect().y");

                    if (elementPosition.Key == 0 || elementPosition.Value == 0)
                        elementPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.getElementsByClassName('{FdConstants.FriendUser2Element}')[{currentThreadIndex}].getElementsByClassName('rn8ck1ys s3jn8y49 icdlwmnq jxuftiz4 cxfqmxzd')[0].getBoundingClientRect().x"
                                    , customScriptY: $"document.getElementsByClassName('{FdConstants.FriendUser2Element}')[{currentThreadIndex}].getElementsByClassName('rn8ck1ys s3jn8y49 icdlwmnq jxuftiz4 cxfqmxzd')[0].getBoundingClientRect().y");

                    if (ownprofilePosition.Value == elementPosition.Value)
                        elementPosition = new KeyValuePair<int, int>(elementPosition.Key, elementPosition.Value + 20);
                    if (elementPosition.Key != 0 && elementPosition.Value != 0)
                        await BrowserWindow.MouseHoverAsync(elementPosition.Key + 10, elementPosition.Value + 2, delayAfter: 5);//+30
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (pageSource.Contains("aria-label=\"Message\""))
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Message", delayAfter: 2);
                    else
                    {
                        List<string> listButtons = await BrowserWindow.GetListInnerHtmlChildElement(ActType.GetValue, AttributeType.ClassName, "pedkr2u6 ijkhr0an art1omkt s13u9afw", AttributeType.ClassName, "abiwlrkh p8dawk7l cbu4d94t taijpn5t k4urcfbm", ValueTypes.InnerText);
                        listButtons.Reverse();
                        int index = listButtons.IndexOf(listButtons.FirstOrDefault(x => x == "Message"));
                        if (index > -1)
                            await BrowserWindow.BrowserActAsync(ActType.CustomActType, AttributeType.ClassName, "pedkr2u6 ijkhr0an art1omkt s13u9afw", value: $"getElementsByClassName('abiwlrkh p8dawk7l cbu4d94t taijpn5t k4urcfbm')[{index}].click()", delayAfter: 5);
                    }


                    //Sometimes Clicking On Own Profile So Validating
                    List<string> UserName = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "ellipsis", ValueTypes.InnerText);

                    if (UserName.Any(x => x == account.AccountBaseModel.UserFullName))
                    {
                        await BrowserWindow.MouseClickAsync(30, 30, delayAfter: 0.5, mouseClickType: MouseClickType.Left);
                        //position Click
                        elementPosition = new KeyValuePair<int, int>(elementPosition.Key + 10, elementPosition.Value + 20);
                        await BrowserWindow.MouseClickAsync(elementPosition.Key, elementPosition.Value, delayAfter: 0.5, mouseClickType: MouseClickType.Right);
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_42ft _4jy0 HovercardMessagesButton _4jy3 _517h", delayAfter: 1);
                    }

                    List<string> messageList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName
                           , "_5wd9 direction_ltr clearfix", valueType: ValueTypes.OuterHtml);

                    if (messageList.Count == 0)
                    {
                        await Task.Delay(2000, cancellationToken);
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_42ft _4jy0 HovercardMessagesButton _4jy3 _517h", delayAfter: 2);
                    }

                    if (isCheckMessage)
                    {
                        do
                        {
                            messageList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName
                               , "_5wd9 direction_ltr clearfix", valueType: ValueTypes.OuterHtml);

                            await Task.Delay(400);

                        } while (scrollCount-- > 0 && messageList.FirstOrDefault
                                (x => x.Contains("data-tooltip-position=\"right\"")) == null);

                        isSentMessage = messageList.Count != 0 && messageList.FirstOrDefault(x => x.Contains("data-tooltip-position=\"right\"")) != null;

                        if (closeDialog && isSentMessage)
                            await CloseAllMessageDialogues();

                        await BrowserWindow.MouseClickAsync(0, 0, delayAfter: 0.5, mouseClickType: MouseClickType.Right);

                    }
                    else
                        isSentMessage = true;

                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return isSentMessage;
        }

        public bool UnjoinGroups(DominatorAccountModel account, GroupDetails groupDetails)
        {
            bool isRunning = true;

            string pageSource = string.Empty;
            bool isUnjoined = false;
            List<string> buttonLst = new List<string>();
            var leaveGroupText = string.Empty;
            //int moreBtnIndex = 0;
            int index = 0;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Joined", index: 0, delayBefore: 4, delayAfter: 5);

                    pageSource = await BrowserWindow.GetPageSourceAsync();

                    if (!pageSource.Contains("Leave group"))
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Following", delayAfter: 5);

                    var JoinOrFollow = await BrowserWindow.GetChildElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                        "x9f619 x1jx94hy x8ii3r7 x1qpq9i9 xdney7k xu5ydu1 xt3gfkd x6ikm8r x10wlt62", AttributeType.ClassName, "x168biu4",
                        parentIndex: 1, childIndex: 0, valueType: ValueTypes.InnerText);
                    if (string.IsNullOrEmpty(JoinOrFollow))
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Following", delayAfter: 5);

                    var JoinBtnClass = pageSource.Contains("l9j0dhe7 swg4t2nn") ? "l9j0dhe7 swg4t2nn" : "x1t2pt76 x7ja8zs x1n2onr6 x1qrby5j x1jfb8zj";

                    buttonLst = await BrowserWindow.GetListInnerHtmlChildElement(ActType.CustomActByQueryType, parentAttributeType: AttributeType.ClassName
                        , parentAttributeValue: JoinBtnClass, childAttributeName: AttributeType.Role, childAttributeValue: "menuitem", parentIndex: 2);

                    if (buttonLst.Count() == 0)
                        buttonLst = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", ValueTypes.OuterHtml);

                    if (buttonLst.Count() == 0)
                    {
                       

                        pageSource = await BrowserWindow.GetPageSourceAsync();

                        var JoinBtnClass2 = pageSource.Contains("l9j0dhe7 swg4t2nn") ? "l9j0dhe7 swg4t2nn" : "x6s0dn4 x1y1aw1k x1sxyh0 xwib8y2 xurb0ha";

                        JoinBtnClass = JoinBtnClass2;

                        buttonLst = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, JoinBtnClass);
                    }
                    var buttonLstOne = new List<string>();
                    if (buttonLst.Count() == 0)
                    {
                      

                        pageSource = await BrowserWindow.GetPageSourceAsync();

                        buttonLstOne = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "x1t2pt76 x7ja8zs x1n2onr6 x1qrby5j x1jfb8zj");
                        buttonLstOne.Reverse();
                        index = buttonLstOne.IndexOf(buttonLstOne.FirstOrDefault(x => x.Contains("Leave group")));
                        if (index < 0)
                            index = buttonLstOne.IndexOf(buttonLstOne.FirstOrDefault(x => x.Contains("Leave Community")));
                        buttonLst = await BrowserWindow.GetListInnerHtmlChildElement(ActType.CustomActByQueryType, parentAttributeType: AttributeType.ClassName
                            , parentAttributeValue: "x1t2pt76 x7ja8zs x1n2onr6 x1qrby5j x1jfb8zj", childAttributeName: AttributeType.Role, childAttributeValue: "menuitem", parentIndex: index);
                    }
                    var parentindex = index;
                    buttonLst.Reverse();

                    var text = buttonLst.FirstOrDefault(x => x.ToLower().Contains("leave group"));
                    if (string.IsNullOrEmpty(text))
                        text = buttonLst.FirstOrDefault(x => x.ToLower().Contains("unfollow group"));
                    if (buttonLst.Count() == 0)
                        text = buttonLstOne.FirstOrDefault(x => x.ToLower().Contains("leave group"));

                    if (text == null)
                        text = string.Empty;
                    leaveGroupText = text.ToLower().Contains("leave group") ? "Leave group"
                                : text.ToLower().Contains("unfollow group") ? "Unfollow group"
                                : "Leave community";
                    index = buttonLst.IndexOf(buttonLst.FirstOrDefault(x => x.ToLower().Contains(leaveGroupText.ToLower())));
                    if (index < 0)
                        index = buttonLstOne.IndexOf(buttonLstOne.FirstOrDefault(x => x.ToLower().Contains(leaveGroupText.ToLower())));

                    BrowserWindow.SetResourceLoadInstance();

                    if (parentindex == 0)
                        parentindex = 2;

                    await BrowserWindow.BrowserActAsync(ActType.CustomActType, AttributeType.ClassName, JoinBtnClass, value: $"querySelectorAll('[role=\"menuitem\"]')[{index}].click()", delayAfter: 6, index: parentindex);
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, JoinBtnClass, index: index, delayAfter: 5);
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: index, delayAfter: 5);
                    leaveGroupText = leaveGroupText.ToLower().Contains("leave group") ? "Leave Group"
                                : leaveGroupText.ToLower().Contains("unfollow group") ? "Unfollow"
                                : "Leave community";
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel,
                        leaveGroupText, delayAfter: 6);

                    var resourceList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"leave_forum_group\"", true);

                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();

                    isUnjoined = resourceList.Count() > 0 ? true : false;

                    var currentPageUrl = BrowserWindow.Browser.Address;

                    if (!string.IsNullOrEmpty(currentPageUrl) && currentPageUrl.StartsWith("https://www.facebook.com/groups/your"))
                        isUnjoined = true;

                    if (!isUnjoined)
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        isUnjoined = FdFunctions.GetNewPrtialDecodedResponse(pageSource).Contains("aria-label=\"Join ") ? true : false;
                    }

                }
                catch (Exception ex) { ex.DebugLog(); }
                finally
                {
                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return isUnjoined;
        }

        public async Task<IResponseHandler> UpdateGroupsAsync(DominatorAccountModel account, CancellationToken token)
        {
            int failedCount = 0;
            bool isRunning = true;
            string pageSource = string.Empty;
            List<string> JsonResponseList = new List<string>();
            BrowserWindow.ClearResources();
            LoadPageSource(account, FdConstants.GroupUpdateUrl, true);
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int previousCount = 0;
                    int currentCount = 0;
                    var tempjsonresponselist = new List<string>();
                    await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "href", "groups/joins")}[0].click()", delayInSec: 4);
                    await LoadSource();
                    while (failedCount < 2)
                    {
                        tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"all_joined_groups\":", true);
                        
                        if (tempjsonresponselist.Count == 0)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Group\"", true);
                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                            , 0, -400, delayAfter: 4, scrollCount: 4);
                        tempjsonresponselist.RemoveAll(x => JsonResponseList.Any(y => y.Contains(x)));
                        
                        currentCount += tempjsonresponselist.Count;
                        
                        JsonResponseList.AddRange(tempjsonresponselist);
                        

                        if (currentCount == previousCount && JsonResponseList.Count == currentCount) failedCount++;
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new GroupScraperResponseHandler(new ResponseParameter() { Response = pageSource }, JsonResponseList, false);
        }

        public bool InvitePages(DominatorAccountModel account, FacebookUser user, InviterOptions inviterOptionsModel)
        {
            bool isRunning = true;
            var status = false;
            List<string> responseList = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {

                try
                {
                    int invitePageIndex;
                    List<string> inviteeList = new List<string>();
                    List<string> invitedList = new List<string>();
                    List<string> menuItemList;
                    DateTime CurrentTime = DateTime.Now;
                    string pageSource = string.Empty;
                    BrowserWindow.SetResourceLoadInstance();
                    int count = 0;

                    bool isCompleted = false;
                    while (count < 5 && !isCompleted)
                    {

                        pageSource = await BrowserWindow.GetPageSourceAsync();

                        if (pageSource.Contains("aria-label=\"Close chat\""))
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Close chat", delayAfter: 4);

                        string ariaLabelClass = pageSource.Contains("See Options") ? "See Options" : "More actions";

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, ariaLabelClass, delayAfter: 4);

                        menuItemList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                            "menuitem");

                        menuItemList.Reverse();

                        invitePageIndex = menuItemList.IndexOf(menuItemList.FirstOrDefault(x => x.ToLower().Contains("invite friends")));

                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                            , 0, -50, delayAfter: 1, scrollCount: 1);

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: invitePageIndex);

                        await Task.Delay(TimeSpan.FromSeconds(15));

                        inviteeList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "checkbox");

                        count++;
                        if (inviteeList.Count > 0)
                            isCompleted = true;

                        if (!isCompleted)
                        {
                            BrowserWindow.Refresh();
                            await Task.Delay(TimeSpan.FromSeconds(60));
                        }
                    }

                    FdFunctions objFdFunctions = new FdFunctions(account);
                    bool canInvite = objFdFunctions.IsFriend(user);

                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();

                    menuItemList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,
                                    "xm7lytj xsyo7zv xdvlbce x16hj40l xc9qbxq", ValueTypes.OuterHtml);

                    menuItemList.Reverse();

                    invitePageIndex = menuItemList.IndexOf(menuItemList.LastOrDefault(x => x.ToLower().Contains("search in all friends")));

                    await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                        , 0, 500, delayAfter: 2, scrollCount: 1);

                    KeyValuePair<int, int> searchTextAreaPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"Search in All Friends\"]')[0].getBoundingClientRect().x"
                        , customScriptY: $"document.querySelectorAll('[aria-label=\"Search in All Friends\"]')[0].getBoundingClientRect().y");
                    await BrowserWindow.MouseClickAsync(searchTextAreaPosition.Key + 2, searchTextAreaPosition.Value + 2, delayAfter: 1);

                    BrowserWindow.SetResourceLoadInstance();

                    await BrowserWindow.EnterCharsAsync(user.Familyname, typingDelay: 0.2, delayAtLast: 5);

                    await BrowserWindow.BrowserActAsync(ActType.CustomActType, AttributeType.ClassName, "x1iorvi4 xykv574 xbmpl8g x4cne27 xifccgj"
                                        , value: "querySelectorAll('[aria-label=\"Search\"]')[0].click()", delayAfter: 5);
                    inviteeList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "checkbox");
                    if (inviteeList.Count > 0)
                    {
                        inviteeList.Reverse();
                        var inviterDetails = new InviterDetailsResponseHandler(new ResponseParameter() { Response = inviteeList.LastOrDefault() });

                        BrowserWindow.SetResourceLoadInstance(false);
                        BrowserWindow.ClearResources();

                        string currentUserImageUrl = Regex.Replace(Regex.Replace(user.ProfilePicUrl, "_nc_ohc=(.*?)&", string.Empty), "oh=(.*?)&", string.Empty);

                        string chkProfilePic = string.Empty;

                        if (!string.IsNullOrEmpty(currentUserImageUrl))
                        {
                            chkProfilePic = Regex.Split(FdRegexUtility.FirstMatchExtractor(currentUserImageUrl, "(.*?)[?]"), "/")?.LastOrDefault();
                        }

                        List<string> listContacts = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                            "checkbox");

                        if (listContacts.Count > 0)
                        {

                            listContacts.Reverse();

                            int currentContactIndex = listContacts.IndexOf(listContacts.FirstOrDefault(x => x.Contains(user.Familyname)));

                            if (currentContactIndex >= 0)
                            {
                                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "checkbox", index: currentContactIndex, delayAfter: 2);

                                if (inviterOptionsModel.IsSendInvitationWithNote || inviterOptionsModel.IsSendInvitationInMessanger)
                                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Name, "checkbox", delayAfter: 2);

                                pageSource = await BrowserWindow.GetPageSourceAsync();
                                string inviteClass = pageSource.Contains("Send Invitations") ? "Send Invitations" : "Send Invites";

                                BrowserWindow.SetResourceLoadInstance();

                                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, inviteClass, delayAfter: 10);

                                responseList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"profile_plus_send_friend_follower_invite", true);
                                if (responseList.Count == 0)
                                    responseList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"page_send_friend_invite", true);
                                BrowserWindow.SetResourceLoadInstance(false);
                                BrowserWindow.ClearResources();

                            }
                        }

                        await Task.Delay(5);
                    }
                    else
                    {
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div", "tab", "textContent", "Invited")}[0]", timetoWait: 5, isLoadSource: true);
                        await ClickOrScrollToViewAndClick($"[...[...document.querySelectorAll('input[placeholder=\"Search in All Friends\"]')],...[...document.querySelectorAll('input[aria-label=\"Search in All Friends\"]')]][0]");
                        BrowserWindow.SelectAllText();
                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                        BrowserWindow.ClearResources();
                        await BrowserWindow.EnterCharsAsync(" " + user.Familyname, typingDelay: 0.2, delayAtLast: 5);
                        if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"[...document.querySelectorAll('div>div[role=\"button\"]')].filter(x=>x.textContent?.includes(\"{user.Familyname}\")).length"))?.Result?.ToString(), out int usercount) && usercount > 0)
                            invitedList.Add(user.Username);
                    }
                    status = (responseList != null && responseList.Count > 0 &&
                (responseList.FirstOrDefault().Contains("{\"data\":{\"profile_plus_send_friend_follower_invite") ||
                responseList.FirstOrDefault().Contains("{\"data\":{\"page_send_friend_invite"))) || invitedList.Count > 0;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    status = false;
                }
                BrowserWindow.ClearResources();
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            if (status && inviterOptionsModel.IsSendInvitationWithNote)
            {
                SendMessage(account, user.UserId, inviterOptionsModel.Note, openWindow: true);
                CloseAllMessageDialogues().Wait(cancellationToken);
            }

            return status;

        }

        public IResponseHandler InviteGroups(DominatorAccountModel account, FacebookUser user, string note)
        {
            bool isRunning = true;

            string pageSource = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    DateTime currentTime = DateTime.Now;

                    await Task.Delay(5000, cancellationToken);

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Invite", delayAfter: 8);

                    List<string> menuList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", valueType: ValueTypes.InnerHtml);

                    if (menuList.Count > 0)
                    {
                        menuList.Reverse();

                        int inviteFriendsIndex = menuList.IndexOf(menuList.FirstOrDefault(x => x.ToLower().Contains("invite facebook friends")));

                        KeyValuePair<int, int> inviteFriendPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[role=\"menuitem\"]')[{inviteFriendsIndex}].getBoundingClientRect().x"
                            , customScriptY: $"document.querySelectorAll('[role=\"menuitem\"]')[{inviteFriendsIndex}].getBoundingClientRect().y");

                        if (inviteFriendPosition.Key != 0 && inviteFriendPosition.Value != 0)
                            await BrowserWindow.MouseClickAsync(inviteFriendPosition.Key + 5, inviteFriendPosition.Value + 5, delayAfter: 10);
                    }

                    int searchBoxIndex = await BrowserWindow.GetItemCountInnerHtml(ActType.GetValue, AttributeType.ClassName, "rq0escxv a8c37x1j a5nuqjux l9j0dhe7 k4urcfbm") - 1;

                    KeyValuePair<int, int> searchTextAreaPosition = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "rq0escxv a8c37x1j a5nuqjux l9j0dhe7 k4urcfbm", index: searchBoxIndex);

                    if (searchTextAreaPosition.Key == 0 && searchTextAreaPosition.Value == 0)
                        searchTextAreaPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"Search for friends by name\"]')[0].getBoundingClientRect().x"
                            , customScriptY: $"document.querySelectorAll('[aria-label=\"Search for friends by name\"]')[0].getBoundingClientRect().y");

                    await BrowserWindow.MouseClickAsync(searchTextAreaPosition.Key + 4, searchTextAreaPosition.Value + 4, delayAfter: 2);

                    await BrowserWindow.EnterCharsAsync($" {user.Familyname} ", typingDelay: 0.2);

                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

                    List<string> listContacts = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                        "option", ValueTypes.OuterHtml);

                    if (listContacts.Count > 0)
                    {
                        listContacts.Reverse();

                        int currentContactIndex = listContacts.IndexOf(listContacts.FirstOrDefault(x =>
                                 x.Contains(user.UserId) || x.Contains(user.Familyname)));
                        if (currentContactIndex >= 0)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.Role,
                                "option", index: currentContactIndex, delayAfter: 1);

                            searchTextAreaPosition = await BrowserWindow.GetXAndYAsync(customScriptX: FdConstants.GetlocFromRoleOptionXCoordinate(currentContactIndex)
                                , customScriptY: FdConstants.GetlocFromRoleOptionYCoordinate(currentContactIndex));

                            await BrowserWindow.MouseClickAsync(searchTextAreaPosition.Key, searchTextAreaPosition.Value, delayAfter: 2);

                            BrowserWindow.SetResourceLoadInstance();

                            int inviteINDEX = await BrowserWindow.GetItemCountInnerHtml(ActType.ActByQuery, AttributeType.AriaLabel, "Invite", valueType: ValueTypes.InnerHtml) - 1;

                            KeyValuePair<int, int> inviteBtnLoc = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[aria-label=\"Invite\"]')[{inviteINDEX}].getBoundingClientRect().x",
                                customScriptY: $"document.querySelectorAll('[aria-label=\"Invite\"]')[{inviteINDEX}].getBoundingClientRect().y");

                            await BrowserWindow.MouseClickAsync(inviteBtnLoc.Key + 10, inviteBtnLoc.Value + 10, delayAfter: 2);

                            await Task.Delay(4000, cancellationToken);

                            List<string> listValue = await BrowserWindow.GetPaginationDataList("{\"data\":{\"group_add_member\"", true);

                            pageSource = await BrowserWindow.GetPageSourceAsync();

                            if (pageSource.Contains("exception_dialog"))
                            {
                                List<string> details = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.DataTestId,
                                "exception_dialog", ValueTypes.InnerText);
                                pageSource = details.FirstOrDefault();
                            }
                            else
                                pageSource = listValue.LastOrDefault();

                        }
                    }
                    else
                    {
                        await BrowserWindow.MouseClickAsync(searchTextAreaPosition.Key + 110, searchTextAreaPosition.Value + 60, delayAfter: 2);
                        BrowserWindow.SetResourceLoadInstance();

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel,
                            "Invite", delayAfter: 2, index: 1);

                        await Task.Delay(2000, cancellationToken);

                        List<string> listValue = await BrowserWindow.GetPaginationDataList("group_add_member", true);

                        if (listValue.Count <= 0)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel,
                                "Send Invitations", delayAfter: 2);

                            await Task.Delay(2000, cancellationToken);

                            listValue = await BrowserWindow.GetPaginationDataList("group_add_member", true);
                        }

                        pageSource = await BrowserWindow.GetPageSourceAsync();

                        if (pageSource.Contains("exception_dialog"))
                        {
                            List<string> details = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.DataTestId,
                            "exception_dialog", ValueTypes.InnerText);
                            pageSource = details.FirstOrDefault();
                        }
                        else
                            pageSource = listValue.LastOrDefault();

                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                BrowserWindow.ClearResources();
                BrowserWindow.SetResourceLoadInstance(false);

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return new BrowserGroupInviterResponseHandler(new ResponseParameter() { Response = pageSource });

        }
        private void ClickAndUploadMediaForStory()
        {
            KeyValuePair<int, int> imageButtonPosition = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.UploadPhotoButtonScript}[0].getBoundingClientRect().x",
                        customScriptY: $"{FdConstants.UploadPhotoButtonScript}[0].getBoundingClientRect().y").Result;

            if (imageButtonPosition.Key != 0 && imageButtonPosition.Value != 0)
                BrowserWindow.MouseClick(imageButtonPosition.Key + 35, imageButtonPosition.Value + 23, delayAfter: 35);

            KeyValuePair<int, int> textBtnLoc = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.WriteTextButtonForImageScript}[0].getBoundingClientRect().x",
                customScriptY: $"{FdConstants.WriteTextButtonForImageScript}[0].getBoundingClientRect().y").Result;
            if (textBtnLoc.Key != 0 && textBtnLoc.Value != 0)
                BrowserWindow.MouseClick(textBtnLoc.Key + 30, textBtnLoc.Value + 17, delayAfter: 5);
        }

        public IResponseHandler PostOnOwnWall(DominatorAccountModel account, PublisherPostlistModel postDetails,
           CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
           FacebookModel advanceSettingsModel)
        {
            string pageSource = string.Empty;
            if (postDetails.FdPostSettings.IsPostAsStoryPost ||
                       advanceSettingsModel.IsPostAsStoryPost)
            {

                int count = 0;
            Post_Story:
                BrowserWindow.SetResourceLoadInstance();
                ClickOrScrollToViewAndClick($"{FdConstants.StoryCreateButtonScript}[0]", $"{FdConstants.FbHomeUrl}stories/create", isLoadSource: true).Wait();
                if (postDetails.MediaList.ToList().Count() > 0)
                {
                    BrowserWindow.ChooseFileFromDialog(pathList: postDetails.MediaList.ToList());
                    ClickAndUploadMediaForStory();
                    if (!string.IsNullOrEmpty(postDetails?.PostDescription))
                    {
                        KeyValuePair<int, int> startTypingBtnLoc = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.StartTypingButtonForImageScript}[0].getBoundingClientRect().x",
                        customScriptY: $"{FdConstants.StartTypingButtonForImageScript}[0].getBoundingClientRect().y").Result;
                        if (startTypingBtnLoc.Key != 0 && startTypingBtnLoc.Value != 0)
                            BrowserWindow.MouseClick(startTypingBtnLoc.Key + 35, startTypingBtnLoc.Value + 15, delayAfter: 3);
                        else if (startTypingBtnLoc.Key == 0 && startTypingBtnLoc.Value == 0 && BrowserWindow.PageText().Result.Contains("Oops, something went wrong. Try again later."))
                        {
                            ClickAndUploadMediaForStory();
                            startTypingBtnLoc = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.StartTypingButtonForImageScript}[0].getBoundingClientRect().x",
                            customScriptY: $"{FdConstants.StartTypingButtonForImageScript}[0].getBoundingClientRect().y").Result;
                            BrowserWindow.MouseClick(startTypingBtnLoc.Key + 55, startTypingBtnLoc.Value + 17, delayAfter: 3);
                        }
                        if (!string.IsNullOrEmpty(postDetails?.PostDescription))
                            CopyPasteText(postDetails.PostDescription, activityType: "STORY");
                    }
                }
                else
                {
                    KeyValuePair<int, int> textButtonPosition = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.CreateTextStoryButtonScript}[0].getBoundingClientRect().x",
                       customScriptY: $"{FdConstants.CreateTextStoryButtonScript}[0].getBoundingClientRect().y").Result;

                    if (textButtonPosition.Key != 0 && textButtonPosition.Value != 0)
                        BrowserWindow.MouseClick(textButtonPosition.Key + 15, textButtonPosition.Value + 17, delayAfter: 4);

                    KeyValuePair<int, int> startTypingBtnLoc = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.StartTypingButtonForTextScript}[0].getBoundingClientRect().x",
                        customScriptY: $"{FdConstants.StartTypingButtonForTextScript}[0].getBoundingClientRect().y").Result;
                    if (startTypingBtnLoc.Key != 0 && startTypingBtnLoc.Value != 0)
                        BrowserWindow.MouseClick(startTypingBtnLoc.Key + 15, startTypingBtnLoc.Value + 20, delayAfter: 2);
                    CopyPasteText(postDetails.PostDescription, activityType: "STORY");

                }

                BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Share to Story", clickIndex: 0, delayAfter: 10);

                Task.Delay(TimeSpan.FromSeconds(60)).Wait(campaignCancellationToken.Token);

                List<string> paginations = BrowserWindow.GetPaginationDataList("{\"data\":{\"story_create\"", true).Result;

                if (paginations.Count <= 0 && count <= 0)
                {
                    count++;
                    goto Post_Story;
                }

                BrowserWindow.ClearResources();
                BrowserWindow.SetResourceLoadInstance(false);

                return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = string.Empty }, paginations.FirstOrDefault(), true);
            }
            else
            {
                List<Friends> listMentions = GetFriendsForMention(advanceSettingsModel, account);
                List<Friends> lisTagFriends = GetFriendsForTag(advanceSettingsModel, account);
                try
                {

                    BrowserWindow.GoToCustomUrl($"{FdConstants.FbHomeUrl}/me/", delayAfter: 5).Wait(campaignCancellationToken.Token);
                    LoadSource().Wait(campaignCancellationToken.Token);
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ScriptforCreatePost, "what\\'s on your mind?")}[0]", isLoadSource: true).Wait(campaignCancellationToken.Token);
                    var _pageText = BrowserWindow.PageText().Result;
                    if (_pageText.Contains("Default audience") && _pageText.Contains("set a default audience"))
                    {
                        if (advanceSettingsModel != null && advanceSettingsModel.IsHidePostInFacebook)
                        {
                            var selectOnlyButtonScript = "[...document.querySelectorAll('div[data-visualcompletion=\"ignore-dynamic\"]>div')].filter(x=>x.textContent.trim()===\"Only me\")[0].{0}();";
                            BrowserWindow.ExecuteScript(string.Format(selectOnlyButtonScript, "scrollIntoView"), delayInSec: 3);
                            BrowserWindow.ExecuteScript(string.Format(selectOnlyButtonScript, "click"), delayInSec: 3);
                        }
                        else
                        {
                            var selectPublicButtonScript = "[...document.querySelectorAll('div[data-visualcompletion=\"ignore-dynamic\"]>div')].filter(x=>x.textContent.trim()===\"Public\")[0].{0}();";
                            BrowserWindow.ExecuteScript(string.Format(selectPublicButtonScript, "scrollIntoView"), delayInSec: 3);
                            BrowserWindow.ExecuteScript(string.Format(selectPublicButtonScript, "click"), delayInSec: 3);
                        }
                        BrowserWindow.ExecuteScript("document.querySelector('div[aria-label=\"Done\"]').click();", delayInSec: 5);
                        if (BrowserWindow.GetPageSource().Contains("Select audience"))
                            BrowserWindow.ExecuteScript("document.querySelector('div[aria-label=\"Save\"]').click();", delayInSec: 5);
                    }
                    if (postDetails.PostSource == PostSource.RssFeedPost)
                    {
                        string removAttachmentClass = "Remove Post Attachment";
                        int removeAttachmentCount = 0;
                        if (int.TryParse(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass).Result, out removeAttachmentCount) && removeAttachmentCount > 0)
                        {

                            BrowserWindow.BrowserAct(ActType.ScrollIntoViewQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass, delayAfter: 1);
                            BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass, delayAfter: 1);
                        }
                    }
                    var MediapathList = postDetails.MediaList.ToList();
                    if (MediapathList.Count > 0) MediapathList.RemoveAll(x => x == "");
                    if (MediapathList.Count > 0 && postDetails.PostSource !=
                        PostSource.SharePost)
                    {
                        pageSource = BrowserWindow.GetPageSource();
                        if (postDetails.PostSource == PostSource.RssFeedPost)
                            GetConvertedRssFeedMedia(postDetails);
                        BrowserWindow.ChooseFileFromDialog(pathList: MediapathList);
                        ClickOrScrollToViewAndClick($"{FdConstants.UploadPhotoScriptWithIndex}", timetoWait: 7).Wait(campaignCancellationToken.Token);
                        if (int.TryParse(BrowserWindow.ExecuteScript($"{FdConstants.AddPhotoorVedioScript}.length")?.Result?.ToString(), out int imageCount2) && imageCount2 > 0)
                            ClickOrScrollToViewAndClick($"{FdConstants.AddPhotoorVedioScript}[0]", timetoWait: 12).Wait(campaignCancellationToken.Token);
                        else
                            Task.Delay(5000).Wait(campaignCancellationToken.Token);
                        if (MediapathList.Count > 1)
                            Task.Delay(MediapathList.Count * 5000).Wait(campaignCancellationToken.Token);
                        pageSource = BrowserWindow.GetPageSource();

                    }
                    if (!string.IsNullOrEmpty(postDetails?.PostDescription) || (postDetails.PostSource == PostSource.SharePost && !string.IsNullOrEmpty(postDetails.ShareUrl)))
                    {
                        var xAndYPostTextLoc = BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.customScriptforWriteTextPost, "what\\'s on your mind?")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(FdConstants.customScriptforWriteTextPost, "what\\'s on your mind?")}[0].getBoundingClientRect().y").Result;

                        if (xAndYPostTextLoc.Key != 0 && xAndYPostTextLoc.Value != 0)
                            BrowserWindow.MouseClick(xAndYPostTextLoc.Key + 20, xAndYPostTextLoc.Value + 20, delayAfter: 2);



                        if (postDetails.PostSource != PostSource.RssFeedPost)
                        {
                            if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                            {
                                BrowserWindow.EnterChars(postDetails.ShareUrl, delayAtLast: 7);

                                // this is to remove the link url as for sharing its not required
                                BrowserWindow.SelectAllText();
                                BrowserWindow.PressAnyKey(winKeyCode: 8, delayAtLast: 5);
                            }
                            if (listMentions.Count != 0)
                            {
                                MentionUsers(listMentions);
                                BrowserWindow.PressAnyKey(1, winKeyCode: 13, delay: 0.1, delayAtLast: 2);
                            }
                            if (postDetails.PostSource != PostSource.SharePost && !string.IsNullOrEmpty(postDetails?.PostDescription))
                            {
                                BrowserWindow.CopyPasteContent(postDetails?.PostDescription, winKeyCode: 86, delayAtLast: 7);
                                if (advanceSettingsModel.IsRemoveLinkPreview && int.TryParse(BrowserWindow.ExecuteScript("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\").length")?.Result?.ToString(), out int count) && count > 0)
                                {
                                    ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\")[0]").Wait(campaignCancellationToken.Token);
                                }
                            }

                        }
                        else
                        {
                            BrowserWindow.CopyPasteContent(postDetails?.PostDescription, winKeyCode: 86, delayAtLast: 7);
                            if (advanceSettingsModel.IsRemoveLinkPreview && int.TryParse(BrowserWindow.ExecuteScript("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\").length")?.Result?.ToString(), out int count) && count > 0)
                            {
                                ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\")[0]").Wait(campaignCancellationToken.Token);
                            }
                        }

                    }
                    if (advanceSettingsModel != null && advanceSettingsModel.IsHidePostInFacebook && BrowserWindow.ExecuteScript($"{FdConstants.AudianceButtonScript}[0].innerText")?.Result?.ToString() != "Only me")
                    {
                        BrowserWindow.ExecuteScript($"{FdConstants.AudianceButtonScript}[0].click();", delayInSec: 5);
                        var selectButtonScript = "[...document.querySelectorAll('div[data-visualcompletion=\"ignore-dynamic\"]>div')].filter(x=>x.textContent.trim()===\"Only me\")[0].{0}();";
                        BrowserWindow.ExecuteScript(string.Format(selectButtonScript, "scrollIntoView"), delayInSec: 3);
                        BrowserWindow.ExecuteScript(string.Format(selectButtonScript, "click"), delayInSec: 3);
                        BrowserWindow.ExecuteScript("document.querySelector('div[aria-label=\"Done\"]').click();", delayInSec: 5);
                        if (BrowserWindow.GetPageSource().Contains("Select audience"))
                            BrowserWindow.ExecuteScript("document.querySelector('div[aria-label=\"Save\"]').click();", delayInSec: 5);
                    }
                    TagFriendsSync(lisTagFriends);
                    BrowserWindow.SetResourceLoadInstance();
                    BrowserWindow.ClearResources();
                    if (int.TryParse(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Next")?.Result, out int nextButtonCount) && nextButtonCount > 0)
                        BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Next", delayAfter: 6);

                    BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Post", delayBefore: 2, delayAfter: 15);
                    if (MediapathList.Count > 0)
                        Task.Delay(MediapathList.Count * 5000).Wait(campaignCancellationToken.Token);

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            List<string> responseList = BrowserWindow.GetPaginationDataList("story_create", isContains: true).Result;

            BrowserWindow.SetResourceLoadInstance(false);
            BrowserWindow.ClearResources();

            if (postDetails.FdPostSettings.IsPostAsStoryPost ||
                      advanceSettingsModel.IsPostAsStoryPost)
                return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = string.Empty }, responseList.FirstOrDefault(), true);
            else
                return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = string.Empty }, responseList.FirstOrDefault(), false);

        }


        public void GetConvertedRssFeedMedia(PublisherPostlistModel postDetails)
        {
            try
            {
                string folderPath = $@"{ConstantVariable.MediaTempFolder}\[{DateTime.Now:MM-dd-yyyy}]";

                if (!System.IO.Directory.Exists(folderPath))
                    DirectoryUtilities.CreateDirectory(folderPath);

                foreach (var media in postDetails.MediaList.ToList())
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(media);
                    string changedExtensionFile = $@"{folderPath}\{fileName}.jpg";

                    if (!DirectoryUtilities.CheckExistingFie(changedExtensionFile))
                    {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(media));
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        img.Save(changedExtensionFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    postDetails.MediaList[postDetails.MediaList.IndexOf(media)] = changedExtensionFile;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool TagFriendsSync(List<Friends> lisTagFriends)
        {
            if (lisTagFriends != null && lisTagFriends.Count > 0)
            {
                BrowserWindow.ExecuteScript($"{FdConstants.TagButtonScript}[0].click()", delayInSec: 9);
                foreach (var taggedUser in lisTagFriends)
                {

                    try
                    {
                        ClickOrScrollToViewAndClick($"{FdConstants.SearchTagPeopleButtonScript}[0]", timetoWait: 5).Wait(cancellationToken);
                        BrowserWindow.SetResourceLoadInstance();
                        BrowserWindow.ClearResources();
                        BrowserWindow.EnterChars($" {taggedUser.FullName} ", typingDelay: 0, delayAtLast: 3);

                        BrowserWindow.ExecuteScript("document.querySelectorAll('li[role=\"option\" i]>div>div')[0].click();", delayInSec: 3);
                        List<string> inviteeList = BrowserWindow.GetPaginationDataList("data\":{\"user\":{\"friends\":{\"edges\"", true).Result;
                        inviteeList.RemoveRange(0, inviteeList.Count - 1);
                        var inviterDetails = new SearchPeopleResponseHandler(inviteeList, new ResponseParameter() { Response = "" }, new List<string>(), false);

                        BrowserWindow.SetResourceLoadInstance(false);
                        FacebookUser curentUser = inviterDetails.ObjFdScraperResponseParameters.ListUser
                                    .FirstOrDefault(x => x.UserId == taggedUser.FriendId || x.Familyname == taggedUser.FullName);

                        string currentUserImageUrl = curentUser != null ? Regex.Replace(Regex.Replace(curentUser.ProfilePicUrl, "_nc_ohc=(.*?)&", string.Empty), "oh=(.*?)&", string.Empty) : string.Empty;
                        string chkProfilePic = Regex.Split(FdRegexUtility.FirstMatchExtractor(currentUserImageUrl, "(.*?)[?]"), "/").LastOrDefault();

                        List<string> listContacts = BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                            "option").Result;

                        if (listContacts.Count > 0)
                        {
                            listContacts.Reverse();

                            int currentContactIndex = listContacts.IndexOf(listContacts.FirstOrDefault(x => x.Contains(chkProfilePic)));

                            if (currentContactIndex >= 0)
                            {
                                KeyValuePair<int, int> friendLoc = BrowserWindow.GetXAndYAsync(customScriptX: FdConstants.GetlocFromRoleOptionXCoordinate(currentContactIndex),
                                    customScriptY: FdConstants.GetlocFromRoleOptionYCoordinate(currentContactIndex)).Result;

                                if (friendLoc.Key != 0 && friendLoc.Value != 0)
                                    BrowserWindow.MouseClick(friendLoc.Key + 10, friendLoc.Value + 10, delayAfter: 5);
                            }
                            else
                                BrowserWindow.PressAnyKey(taggedUser.FullName.Length, winKeyCode: 8, delay: 0.1, delayAtLast: 5);

                        }
                        else
                            BrowserWindow.PressAnyKey(taggedUser.FullName.Length, winKeyCode: 8, delay: 0.1, delayAtLast: 5);
                    }
                    catch
                    {
                    }

                }
                ClickOrScrollToViewAndClick($"{FdConstants.DoneTagPeopleButtonScript}[0]").Wait(cancellationToken);
            }
            return true;
        }

        public bool MentionUsers(List<Friends> lisTagFriends)
        {

            if (lisTagFriends != null && lisTagFriends.Count > 0)
            {
                for (int i = 0; i < lisTagFriends.Count; i++)
                {

                    try
                    {

                        string userName = lisTagFriends[i].FullName;

                        if (Regex.Split(userName, " ").Length > 3)
                        {
                            string[] mentionArray = Regex.Split(userName, " ");

                            userName = userName.Replace(mentionArray.LastOrDefault(), "");

                            userName.Trim();
                        }

                        if (i > 0)
                            BrowserWindow.EnterChars($"  ", typingDelay: 0.1, delayAtLast: 1);

                        BrowserWindow.SetResourceLoadInstance();

                        BrowserWindow.EnterChars($"@{userName} ", typingDelay: 0.2, delayAtLast: 1);
                        BrowserWindow.PressAnyKey(1, winKeyCode: 8, delay: 0.1, delayAtLast: 1);
                        Task.Delay(1000, cancellationToken).Wait();
                        List<string> inviteeList = BrowserWindow.GetPaginationDataList("{\"data\":{\"comet_composer_typeahead_search\"", true).Result;

                        var inviterDetails = new InviterDetailsResponseHandler(new ResponseParameter() { Response = inviteeList.LastOrDefault() }, isMentionFriend: true);

                        DateTime currentTime = DateTime.Now;


                        while (inviterDetails.ObjFdScraperResponseParameters.ListUser.Count == 0
                            && (DateTime.Now - currentTime).TotalSeconds < 4)
                        {
                            Task.Delay(1000, cancellationToken).Wait();

                            inviteeList = BrowserWindow.GetPaginationDataList("{\"data\":{\"comet_composer_typeahead_search\"", true).Result;

                            inviterDetails = new InviterDetailsResponseHandler(new ResponseParameter() { Response = inviteeList.LastOrDefault() });
                        }

                        BrowserWindow.SetResourceLoadInstance(false);
                        BrowserWindow.ClearResources();

                        FacebookUser curentUser = inviterDetails.ObjFdScraperResponseParameters.ListUser
                                        .FirstOrDefault(x => x.UserId == lisTagFriends[i].FriendId);

                        string currentUserImageUrl = curentUser != null ? curentUser.ProfilePicUrl : string.Empty;

                        List<string> listContacts = BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,
                            "_599m").Result;

                        listContacts = listContacts.Count == 0 ? BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,
                            "rbrg37tu bl38xub0 j83agx80 bp9cbjyn").Result : listContacts;

                        if (listContacts.Count > 0)
                        {
                            listContacts.Reverse();

                            int currentContactIndex = listContacts.IndexOf(listContacts.FirstOrDefault(x =>
                                     (FdFunctions.GetDecodedResponse(Regex.Replace(Regex.Replace
                                     (x, "_nc_eui2=(.*?)&amp;", string.Empty), "oh=(.*?)&amp;", string.Empty)).Contains(currentUserImageUrl))));

                            if (currentContactIndex >= 0)
                            {

                                BrowserWindow.PressAnyKey(currentContactIndex, winKeyCode: 40, delay: 100, delayAtLast: 1);

                                BrowserWindow.PressAnyKey(winKeyCode: 13, delay: 0.1, delayAtLast: 1);

                            }
                            else
                                BrowserWindow.PressAnyKey(userName.Length + 2, winKeyCode: 8, delay: 0.1, delayAtLast: 1);

                        }
                        else
                        {
                            BrowserWindow.PressAnyKey(winKeyCode: 9, delay: 0.1, delayAtLast: 0);
                           
                        }


                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }

            return !string.IsNullOrEmpty(BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "_247o").Result);

        }

        private List<Friends> GetFriendsForTag(FacebookModel advanceSettingsModel,
          DominatorAccountModel account)
        {
            try
            {
                int noOfTags = 0;

                FdFunctions objFdFunctions = new FdFunctions(account);

                List<string> randomFriends = new List<string>();

                List<Friends> tagFriendList = new List<Friends>();

                if (advanceSettingsModel.IsTagOptionChecked)
                {
                    if (advanceSettingsModel.IsTagSpecificFriends)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyStartTagFriend".FromResourceDictionary());

                        noOfTags = advanceSettingsModel.UsersForEachPost.GetRandom();

                        randomFriends = objFdFunctions.TagSpecificFriend(account, noOfTags, advanceSettingsModel);

                        randomFriends.RemoveAll(string.IsNullOrEmpty);

                        if (randomFriends.Count > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyGetFriendsCustomList".FromResourceDictionary());

                        randomFriends.ForEach(x =>
                        {
                            FacebookUser facebookUser = new FacebookUser() { ProfileUrl = x };

                            GetFriendsProfileId(account, facebookUser);

                            advanceSettingsModel.ListCustomTaggedUser.Remove(x);

                            advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{facebookUser.UserId}"));

                            Task.Delay(1000, cancellationToken).Wait();
                        });

                        randomFriends = advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                        if (randomFriends.Count == 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyNoFriendsToTag".FromResourceDictionary());

                        Random random = new Random();

                        randomFriends = randomFriends.OrderBy(x => random.Next()).Take(noOfTags).ToList();

                    }

                    else if (advanceSettingsModel.IsAutoTagFriends)
                    {
                        noOfTags = advanceSettingsModel.UsersForEachPost.GetRandom();

                        randomFriends = objFdFunctions.GetRandomFriends(account, noOfTags, advanceSettingsModel);

                    }

                    tagFriendList = objFdFunctions.GetFriendDetails(randomFriends).Take(noOfTags).ToList();

                    return tagFriendList;

                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<Friends>();
        }

        private List<Friends> GetFriendsForMention(
            FacebookModel advanceSettingsModel, DominatorAccountModel account)
        {
            try
            {
                FdFunctions objFdFunctions = new FdFunctions(account);

                if (advanceSettingsModel.IsMentionSpecificFriends)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyStartTagFriend".FromResourceDictionary());

                    int noOfMentions = advanceSettingsModel.MentionUsersForEachPost.GetRandom();

                    List<string> randomFriends = objFdFunctions.MentionSpecificFriendFroPost(account, noOfMentions, advanceSettingsModel);

                    randomFriends.RemoveAll(string.IsNullOrEmpty);

                    if (randomFriends.Count > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyGetFriendsCustomList".FromResourceDictionary());

                    randomFriends.ForEach(x =>
                    {
                        var friendDetails = GetFullUserDetails(account, new FacebookUser() { Username = x });
                        advanceSettingsModel.ListCustomMentionUser.Remove(x);
                        advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{friendDetails.ObjFdScraperResponseParameters.FacebookUser.UserId}"));
                        Task.Delay(1000, cancellationToken).Wait();
                    });

                    randomFriends = advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                    if (randomFriends.Count == 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyNoFriendsToTag".FromResourceDictionary());

                    Random random = new Random();

                    randomFriends = randomFriends.OrderBy(x => random.Next()).Take(noOfMentions).ToList();

                    return objFdFunctions.GetFriendDetails(randomFriends).Take(noOfMentions).ToList();
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<Friends>();
        }


        public IResponseHandler PostOnPages(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel, string pageUrl)
        {

            string pageDetails = string.Empty;

            List<Friends> listMentions = GetFriendsForMention(advanceSettingsModel, account);

            List<Friends> lisTagFriends = GetFriendsForTag(advanceSettingsModel, account);

            string pageActor = GetCurrentPagePostActor(advanceSettingsModel, account);

            DateTime chkForX_Mins = DateTime.Now;

            List<string> responseList = new List<string>();
            try
            {
                BrowserWindow.ClearResources();
                pageDetails = BrowserWindow.GetPageSource();
                string pageId = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.EntityIdRegex);
                if (string.IsNullOrEmpty(pageId) || pageId.Equals("null"))
                    pageId = FdRegexUtility.FirstMatchExtractor(pageDetails, "page_id=(.*?)\"");
                if (string.IsNullOrEmpty(pageId))
                    pageId = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.PageIDRegx);

                string pageName = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.PageNameRegx);

                if (string.IsNullOrEmpty(pageName))
                    pageName = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.FanpageNameRegx);

                if (string.IsNullOrEmpty(pageName))
                    pageName = HtmlParseUtility.GetInnerTextFromTagName(pageDetails, "div", "class", FdConstants.PageNameElement);

                // Change from page to user as Actor
                if (!advanceSettingsModel.IsPostAsPage && !advanceSettingsModel.IsPostAsSamePage && pageDetails.Contains("aria-label=\"Actor selector\""))
                {
                    BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Actor selector", delayAfter: 5);
                    object actors = BrowserWindow.ExecuteScript($"document.querySelectorAll('[role=\"menuitemradio\"]').length").Result;
                    if ((int)actors > 0)
                        BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.Role, attributeValue: "menuitemradio", clickIndex: (int)actors - 1, delayAfter: 8);
                }
                if (!advanceSettingsModel.IsPostAsPage && !advanceSettingsModel.IsPostAsSamePage)
                {
                    if (Convert.ToInt32(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Create Post").Result) > 0)
                        BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Create Post", delayAfter: 5);
                    else
                    {
                        LoadPageSource(account, pageUrl.Contains("profile.php?id") ? $"{pageUrl}&sk=mentions" : $"{pageUrl}/mentions");
                        ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0]", timetoWait: 6).Wait(campaignCancellationToken.Token);
                    }
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.customScriptforWriteTextPost, "write something to")}[0]").Wait(campaignCancellationToken.Token);
                }
                else
                {
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ScriptforCreatePost, "what\\'s on your mind?")}[0]", timetoWait: 6).Wait(campaignCancellationToken.Token);
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.customScriptforWriteTextPost, "what\\'s on your mind?")}[0]").Wait(campaignCancellationToken.Token);
                }
                string currentActorId = FdRegexUtility.FirstMatchExtractor(pageDetails,
                    FdConstants.CurrentActorIdRegex);

                if (string.IsNullOrEmpty(currentActorId))
                    currentActorId = FdRegexUtility.FirstMatchExtractor(pageDetails, "{contextConfig:{actorID:\"(.*?)\"");

                if (string.IsNullOrEmpty(currentActorId))
                    currentActorId = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.ActorIdRegex);

                if (advanceSettingsModel.IsPostAsOwnAccount && !pageDetails.Contains($"data-test-actorid=\"{pageActor}\"")
                    && account.AccountBaseModel.UserId != FdFunctions.GetIntegerOnlyString(currentActorId))
                {
                    ChangePostActorOfPage(account, pageUrl, pageActor);
                    ExpandPostWindow(FdConstants.PostingTextLocClass);
                    ExpandPostWindow(FdConstants.PostingTextLocClass);
                }

                pageDetails = BrowserWindow.GetPageSource();

                currentActorId = FdRegexUtility.FirstMatchExtractor(pageDetails,
                   FdConstants.CurrentActorIdRegex);

                if (string.IsNullOrEmpty(currentActorId))
                    currentActorId = FdRegexUtility.FirstMatchExtractor(pageDetails, "{contextConfig:{actorID:\"(.*?)\"");

                if (string.IsNullOrEmpty(currentActorId))
                    currentActorId = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.ActorIdRegex);


                if (AssignPageId(advanceSettingsModel, account, string.IsNullOrEmpty(pageActor) ? pageName : pageActor,
                          currentActorId, pageId, pageName).Result)
                {
                    if (postDetails.PostSource != PostSource.RssFeedPost)
                    {
                        if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                        {
                            BrowserWindow.CopyPasteContent(postDetails?.ShareUrl, winKeyCode: 86, delayAtLast: 7);
                            // this is to remove the link url as for sharing its not required
                            BrowserWindow.SelectAllText();
                            BrowserWindow.PressAnyKey(winKeyCode: 8, delayAtLast: 5);
                        }
                        if (listMentions.Count > 0)
                        {
                            MentionUsers(listMentions);
                            BrowserWindow.PressAnyKey(winKeyCode: 13, delay: 0.1, delayAtLast: 2);
                        }
                        if (postDetails.PostSource != PostSource.SharePost && !string.IsNullOrEmpty(postDetails?.PostDescription))
                        {
                            BrowserWindow.CopyPasteContent(postDetails?.PostDescription, winKeyCode: 86, delayAtLast: 4);
                            if (advanceSettingsModel.IsRemoveLinkPreview && int.TryParse(BrowserWindow.ExecuteScript("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\").length")?.Result?.ToString(), out int count) && count > 0)
                            {
                                ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\")[0]").Wait(campaignCancellationToken.Token);
                            }
                        }

                    }
                    else if (!string.IsNullOrEmpty(postDetails?.PostDescription))
                    {

                        BrowserWindow.CopyPasteContent(postDetails?.PostDescription, winKeyCode: 86, delayAtLast: 4);
                        if (advanceSettingsModel.IsRemoveLinkPreview && int.TryParse(BrowserWindow.ExecuteScript("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\").length")?.Result?.ToString(), out int count) && count > 0)
                        {
                            ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\")[0]").Wait(campaignCancellationToken.Token);
                        }
                    }

                    KeyValuePair<int, int> imageButtonPosition = new KeyValuePair<int, int>();

                    if (postDetails.MediaList.Count > 0 && postDetails.PostSource != PostSource.SharePost)
                    {
                        if (postDetails.PostSource == PostSource.RssFeedPost)
                        {
                            string removAttachmentClass = "Remove Post Attachment";
                            int removeAttachmentCount = 0;
                            if (int.TryParse(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass).Result, out removeAttachmentCount) && removeAttachmentCount > 0)
                            {

                                BrowserWindow.BrowserAct(ActType.ScrollIntoViewQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass, delayAfter: 1);
                                BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass, delayAfter: 1);
                            }
                        }
                        if (postDetails.PostSource == PostSource.RssFeedPost)
                            GetConvertedRssFeedMedia(postDetails);

                        BrowserWindow.ChooseFileFromDialog(pathList: postDetails.MediaList.ToList());
                        imageButtonPosition = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.UploadPhotoScriptWithIndex}.getBoundingClientRect().x",
                              customScriptY: $"{FdConstants.UploadPhotoScriptWithIndex}.getBoundingClientRect().y").Result;

                        if (imageButtonPosition.Key != 0 && imageButtonPosition.Value != 0)
                            BrowserWindow.MouseClick(imageButtonPosition.Key + 5, imageButtonPosition.Value + 5, delayAfter: 7);
                        if (int.TryParse(BrowserWindow.ExecuteScript($"{FdConstants.AddPhotoorVedioScript}.length")?.Result?.ToString(), out int addPhotoButtonCount) && addPhotoButtonCount > 0)
                            ClickOrScrollToViewAndClick($"{FdConstants.AddPhotoorVedioScript}[0]", timetoWait: 12).Wait(campaignCancellationToken.Token);
                        else if (int.TryParse(BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.ClassName, "x14yjl9h xudhj91 x18nykt9 xww2gxu x6s0dn4 x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x3nfvp2 xl56j7k x1n2onr6 x1qhmfi1 x1vqgdyp x100vrsf")?.Result?.ToString(), out addPhotoButtonCount) && addPhotoButtonCount > 0)
                            ClickOrScrollToViewAndClick("document.getElementsByClassName('x14yjl9h xudhj91 x18nykt9 xww2gxu x6s0dn4 x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x3nfvp2 xl56j7k x1n2onr6 x1qhmfi1 x1vqgdyp x100vrsf')[0]", timetoWait: 12).Wait(campaignCancellationToken.Token);
                        else
                            Task.Delay(TimeSpan.FromSeconds(5)).Wait(campaignCancellationToken.Token);
                        if (postDetails.MediaList.Count > 1)
                            Task.Delay(postDetails.MediaList.Count * 5000, cancellationToken).Wait(campaignCancellationToken.Token);

                    }
                    TagFriendsSync(lisTagFriends);
                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance();
                    if (Convert.ToInt32(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Next").Result) > 0)
                        BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Next", delayAfter: 4);
                    chkForX_Mins = DateTime.Now.AddMinutes(3);
                    while (chkForX_Mins > DateTime.Now)
                    {
                        if (!BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Post", valueType: ValueTypes.OuterHtml).Result.Contains("aria-disabled=\"true\""))
                        {
                            BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Post", delayAfter: 10);
                            break;
                        }

                    }
                    if (Convert.ToInt32(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Not Now").Result) > 0)
                        BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Not Now", delayAfter: 10);
                    if (Convert.ToInt32(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Publish Original Post").Result) > 0)
                        BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Publish Original Post", delayAfter: 10);
                    Task.Delay(5000).Wait(campaignCancellationToken.Token);
                    if (postDetails.MediaList.Count > 0)
                        Task.Delay(postDetails.MediaList.Count * 5000).Wait(campaignCancellationToken.Token);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            responseList = BrowserWindow.GetPaginationDataList("story_create", isContains: true).Result;

            BrowserWindow.SetResourceLoadInstance(false);
            BrowserWindow.ClearResources();
            return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = string.Empty }, responseList.FirstOrDefault(), false);
        }

        public void ChangePostActorOfPage(DominatorAccountModel account, string pageUrl, string pageActor)
        {

            try
            {
                LoadPageSource(account, $"{pageUrl}/settings/?tab=post_attribution&ref=page_edit");
                BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.Value, attributeValue: pageActor, delayAfter: 2);
                BrowserWindow.ExecuteScript($"document.querySelectorAll('[value=\"{pageActor}\"]')[0].click()");
                SearchPostsByPageUrl(account, FbEntityType.Fanpage, pageUrl, true);
            }
            catch (Exception)
            {

            }
        }

        public async Task<bool> AssignPageId(FacebookModel advanceSettingsModel, DominatorAccountModel account,
           string actorName, string currentActorId, string pageId, string pageName)
        {
            if (advanceSettingsModel.IsPostAsSamePage && pageId == FdFunctions.GetIntegerOnlyString(currentActorId))
                return true;

            if ((advanceSettingsModel.IsPostAsOwnAccount && account.AccountBaseModel.UserId != FdFunctions.GetIntegerOnlyString(currentActorId))
                || (advanceSettingsModel.IsPostAsSamePage && pageId != FdFunctions.GetIntegerOnlyString(currentActorId))
                || advanceSettingsModel.IsPostAsPage)
            {
                if (!(await BrowserWindow.GetPageSourceAsync()).Contains(pageId))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", $"Cannot post as {account.AccountBaseModel.UserName} on page {pageName} !");
                    return false;
                }

            }
            return true;
        }

        public string GetCurrentPagePostActor(FacebookModel advanceSettingsModel, DominatorAccountModel account, string type = "")
        {
            try
            {
                if (advanceSettingsModel.IsPostAsPage)
                {
                    FdFunctions objFdFunctions = new FdFunctions(account);

                    List<string> randomPages = objFdFunctions.GetRandomPageActor(account, 1, advanceSettingsModel);

                    randomPages.ForEach(x =>
                    {
                        string pageIdNew = GetFullPageDetails(account, new FanpageDetails() { FanPageUrl = x }).ObjFdScraperResponseParameters.FanpageDetails.FanPageID;
                        advanceSettingsModel.ListCustomPageUrl.Remove(x);
                        advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{pageIdNew}"));
                        Task.Delay(1000, cancellationToken).Wait();
                    });

                    randomPages = advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                    Random random = new Random();

                    if (type == "name")
                        return objFdFunctions.GetPageName(randomPages.OrderBy(x => random.Next()).Take(1).FirstOrDefault());

                    return objFdFunctions.GetPageId(randomPages.OrderBy(x => random.Next()).Take(1).FirstOrDefault());
                }
                else if (advanceSettingsModel.IsPostAsOwnAccount)
                {
                    if (type == "name")
                        return account.AccountBaseModel.UserFullName;
                    else
                        return account.AccountBaseModel.UserId;
                }
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }

        public IResponseHandler PostOnGroups(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            List<Friends> listMentions = GetFriendsForMention(advanceSettingsModel, account);

            List<Friends> lisTagFriends = GetFriendsForTag(advanceSettingsModel, account);

            string pageActor = GetCurrentPagePostActor(advanceSettingsModel, account, "name");

            string pageSource = string.Empty;

            DateTime chkForMins = DateTime.Now;

            try
            {
                if (advanceSettingsModel.IsPostAsPage)
                {
                    SwitchToOwnPage(account, advanceSettingsModel.ListCustomPageUrl.FirstOrDefault());
                }
                if (bool.TryParse(BrowserWindow.ExecuteScriptAsync(FdConstants.GroupDiscussionScript + ".ariaSelected")?.Result?.Result?.ToString() ?? "", out bool isDiscussionSelected) && !isDiscussionSelected)
                    ClickOrScrollToViewAndClick($"{FdConstants.GroupDiscussionScript}", timetoWait: 4).Wait(campaignCancellationToken.Token);
                string groupDetails = BrowserWindow.ExecuteScriptAsync(FdConstants.GroupDiscussionScript + ".outerHTML")?.Result?.Result?.ToString() ?? "";
                string groupId = FdRegexUtility.FirstMatchExtractor(groupDetails, "href=\"https://www.facebook.com/groups/(.*?)/");
                pageSource = BrowserWindow.GetPageSource();

                if (!postDetails.IsFdSellPost)
                {

                    //for buysell Group
                    string tablist = BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.Role, "tablist").Result;
                    if (tablist.ToLower().Contains("buy and sell") && !pageSource.Contains(FdConstants.PostCreate3Class))
                    {
                        if(string.IsNullOrEmpty(groupId))
                            groupId = FdRegexUtility.FirstMatchExtractor(groupDetails, "href=\"/groups/(.*?)/buy_sell_discussion/");
                        LoadPageSource(account, $"{FdConstants.FbHomeUrl}groups/{groupId}/buy_sell_discussion", timeSec: 6);
                    }

                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0]", timetoWait: 6).Wait(campaignCancellationToken.Token);
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.customScriptforWriteTextPost, "create a public post…")}[0]", timetoWait: 2).Wait(campaignCancellationToken.Token);

                    PublishStatusOnGroups(account, postDetails, generalSettingsModel, advanceSettingsModel, campaignCancellationToken,
                              lisTagFriends, listMentions, FdConstants.PostingTextLoc3Class);

                    string postResponse = BrowserWindow.GetPaginationData("story_create", isContains: true).Result;

                    chkForMins = DateTime.Now.AddMinutes(2);
                    while (string.IsNullOrEmpty(postResponse) && chkForMins > DateTime.Now)
                        postResponse = BrowserWindow.GetPaginationDataList("story_create", isContains: true).Result.FirstOrDefault();

                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();
                    return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = string.Empty }, postResponse, false);
                }
                else
                {
                    return PublishOnBuySellGroup(account, postDetails, campaignCancellationToken);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PublisherBrowserResponseHandler(new ResponseParameter());
            }

        }

        public IResponseHandler PublishOnBuySellGroup(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            try
            {
                ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div", "button", "ariaLabel", "Sell Something")}[0]", timetoWait: 8, isLoadSource: true).Wait(campaignCancellationToken.Token);
                ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div", "button", "textContent", "Item for sale")}[0]", timetoWait: 5, isLoadSource: true).Wait(campaignCancellationToken.Token);

                if (postDetails.MediaList.Count > 0)
                {
                    BrowserWindow.ChooseFileFromDialog(pathList: postDetails.MediaList.ToList());
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div", "button", "textContent", "Add photos")}[0]", timetoWait: 25).Wait(campaignCancellationToken.Token);
                }


                if (!string.IsNullOrEmpty(postDetails?.FdSellProductTitle))
                {
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "label", "aria-label", "Title", "textContent", "title")}[0]").Wait(campaignCancellationToken.Token);
                    BrowserWindow.CopyPasteContent(postDetails?.FdSellProductTitle, 86, delayAtLast: 3);
                }

                // Price
                if (postDetails?.FdSellPrice != null)
                {
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "label", "aria-label", "Price", "textContent", "price")}[0]").Wait(campaignCancellationToken.Token);
                    BrowserWindow.CopyPasteContent(postDetails?.FdSellPrice.ToString(), 86, delayAtLast: 3);
                }

                ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "label", "aria-label", "Condition", "textContent", "condition")}[0]").Wait(campaignCancellationToken.Token);
                ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "option", "textContent", postDetails.FdCondition != null ? postDetails.FdCondition.ToLower() : "new")}[0]").Wait(campaignCancellationToken.Token);

                if (!string.IsNullOrEmpty(postDetails?.PostDescription) || !string.IsNullOrEmpty(postDetails?.FdSellLocation))
                {
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div", "button", "textContent", "More details")}[0]").Wait(campaignCancellationToken.Token);
                    // Description

                    if (!string.IsNullOrEmpty(postDetails?.PostDescription))
                    {
                        ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "label", "aria-label", "Description", "textContent", "description")}[0]").Wait(campaignCancellationToken.Token);
                        BrowserWindow.CopyPasteContent(postDetails?.PostDescription, 86, delayAtLast: 4);
                    }
                    // Location
                    if (!string.IsNullOrEmpty(postDetails?.FdSellLocation))
                    {
                        try
                        {
                            ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "label", "aria-label", "Location", "textContent", "location")}[0]").Wait(campaignCancellationToken.Token);
                            BrowserWindow.SelectAllText();
                            BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3).Wait(campaignCancellationToken.Token);
                            BrowserWindow.CopyPasteContent(postDetails?.FdSellLocation, 86, delayAtLast: 6);
                            ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "option", "textContent", "")}[0]").Wait(campaignCancellationToken.Token);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Next", delayAfter: 8);
                BrowserWindow.SetResourceLoadInstance();

                BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Post", delayAfter: 10);

                string postResponse = BrowserWindow.GetPaginationDataList("{\"story_create\"", isContains: true).Result.FirstOrDefault();
                BrowserWindow.SetResourceLoadInstance(false);
                BrowserWindow.ClearResources();

                return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = string.Empty }, postResponse, false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PublisherBrowserResponseHandler(new ResponseParameter());
            }
        }
        public void PublishStatusOnGroups(DominatorAccountModel account, PublisherPostlistModel postDetails, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel,
            CancellationTokenSource campaignCancellationToken, List<Friends> listTags, List<Friends> listMentions,
            string className)
        {
            try
            {
                if (listMentions.Count > 0)
                {
                    MentionUsers(listMentions);
                    BrowserWindow.PressAnyKey(1, winKeyCode: 13, delay: 0.1, delayAtLast: 2);
                }

                if (postDetails.PostSource != PostSource.RssFeedPost && postDetails.PostSource != PostSource.NormalPost)
                {

                    if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                    {
                        BrowserWindow.CopyPasteContent(postDetails?.ShareUrl, winKeyCode: 86, delayAtLast: 7);
                        // this is to remove the link url as for sharing its not required
                        BrowserWindow.SelectAllText();
                        BrowserWindow.PressAnyKey(winKeyCode: 8, delayAtLast: 5);
                    }
                    if (postDetails.PostSource != PostSource.SharePost && !string.IsNullOrEmpty(postDetails?.PostDescription))
                    {
                        BrowserWindow.CopyPasteContent(postDetails?.PostDescription, winKeyCode: 86, delayAtLast: 4);
                        if (advanceSettingsModel.IsRemoveLinkPreview && int.TryParse(BrowserWindow.ExecuteScript("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\").length")?.Result?.ToString(), out int count) && count > 0)
                        {
                            ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\")[0]").Wait(campaignCancellationToken.Token);
                        }
                    }

                }
                else if (!string.IsNullOrEmpty(postDetails?.PostDescription))
                {
                    BrowserWindow.CopyPasteContent(postDetails?.PostDescription, winKeyCode: 86, delayAtLast: 4);

                    if (advanceSettingsModel.IsRemoveLinkPreview && int.TryParse(BrowserWindow.ExecuteScript("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\").length")?.Result?.ToString(), out int count) && count > 0)
                    {
                        ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=> x.ariaLabel===\"Remove link preview from your post\")[0]").Wait(campaignCancellationToken.Token);
                    }
                    BrowserWindow.PressAnyKey(winKeyCode: 32, delayAtLast: 10);
                }


                if (postDetails.MediaList.Count > 0 && postDetails.PostSource != PostSource.SharePost)
                {
                    if (postDetails.PostSource == PostSource.RssFeedPost)
                    {
                        string removAttachmentClass = "Remove Post Attachment";
                        int removeAttachmentCount = 0;
                        if (int.TryParse(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass).Result, out removeAttachmentCount) && removeAttachmentCount > 0)
                        {

                            BrowserWindow.BrowserAct(ActType.ScrollIntoViewQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass, delayAfter: 1);
                            BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: removAttachmentClass, delayAfter: 1);
                        }
                    }
                    if (postDetails.PostSource == PostSource.RssFeedPost)
                        GetConvertedRssFeedMedia(postDetails);
                    BrowserWindow.ChooseFileFromDialog(pathList: postDetails.MediaList.ToList());
                    Task.Delay(2000).Wait(campaignCancellationToken.Token);
                    ClickOrScrollToViewAndClick($"{FdConstants.UploadPhotoScriptWithIndex}", timetoWait: 7).Wait(campaignCancellationToken.Token);
                    if (int.TryParse(BrowserWindow.ExecuteScript($"{FdConstants.AddPhotoorVedioScript}.length")?.Result?.ToString(), out int imageCount2) && imageCount2 > 0)
                        ClickOrScrollToViewAndClick($"{FdConstants.AddPhotoorVedioScript}[0]", timetoWait: 12).Wait(campaignCancellationToken.Token);
                    else
                        Task.Delay(5000).Wait(account.CancellationSource.Token);
                    if (postDetails.MediaList.Count > 1)
                    {
                        Task.Delay(postDetails.MediaList.Count * 5000).Wait(account.CancellationSource.Token);
                    }
                }
                TagFriendsSync(listTags);
                if (listTags.Count() > 0)
                    Task.Delay(2000).Wait(account.CancellationSource.Token);

                DateTime startTime = DateTime.Now;

                while (BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel
                    , "Post", valueType: ValueTypes.OuterHtml).Result.Contains("aria-disabled=\"true\"") &&
                    (DateTime.Now - startTime).TotalSeconds < 60)
                    Task.Delay(2000).Wait(account.CancellationSource.Token);

                BrowserWindow.ClearResources();
                BrowserWindow.SetResourceLoadInstance();
                if (Convert.ToInt32(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Next").Result) > 0)
                    BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Next", delayAfter: 6);

                if (!BrowserWindow.GetElementValueAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Post", valueType: ValueTypes.OuterHtml).Result.Contains("aria-disabled=\"true\""))
                    BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Post", delayAfter: 15);
                if (postDetails.MediaList.Count > 0)
                    Task.Delay(postDetails.MediaList.Count * 5000).Wait(campaignCancellationToken.Token);
                if (int.TryParse(BrowserWindow.ExecuteScript($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>input", "type", "checkbox", "name", "agree-to-group-rules")}.length")?.Result?.ToString(), out int checkboxCount) && checkboxCount > 0)
                {
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>input", "type", "checkbox", "name", "agree-to-group-rules")}[0]").Wait(campaignCancellationToken.Token);
                    ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "ariaLabel", "submit")}[0]", timetoWait: 15).Wait(campaignCancellationToken.Token);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public EventCreaterResponseHandler EventCreater(DominatorAccountModel account, EventCreaterManagerModel eventCreaterManagerModel)
        {
            EventCreaterResponseHandler eventCreaterResponseHandler = null;
            if (eventCreaterManagerModel.EventStartDate < DateTime.Now)
                return new EventCreaterResponseHandler(new ResponseParameter(), eventCreaterManagerModel)
                {
                    ErrorMsg = "Date Should be more then current Time",
                };

            bool isRunning = true;

            if (eventCreaterManagerModel.EventType == "LangKeyCreatePrivateEvent".FromResourceDictionary())
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {

                    try
                    {
                        eventCreaterResponseHandler = await PrivateEventCreater(account, eventCreaterManagerModel);
                    }
                    catch (Exception) { }

                    isRunning = false;

                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        eventCreaterResponseHandler = await PublicEventCreater(account, eventCreaterManagerModel);
                    }
                    catch (Exception) { }

                    isRunning = false;

                });
            }


            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return eventCreaterResponseHandler;
        }

        public async Task<EventCreaterResponseHandler> PrivateEventCreater(DominatorAccountModel account, EventCreaterManagerModel eventCreaterManagerModel)
        {
            string totalResponse = string.Empty;
            try
            {
                await BrowserWindow.GoToCustomUrl($"{FdConstants.FbHomeUrl}events/", delayAfter: 5);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.DataTestId,
                    "event-create-split-menu", delayBefore: 1, delayAfter: 1);

                await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('_54ni _b19 __MenuItem')[0].getElementsByTagName('a')[0].click()");

                //var TempFileDialogHandler = new TempFileDialogHandler(BrowserWindow, eventCreaterManagerModel.MediaPath);
                //BrowserWindow.Browser.DialogHandler = TempFileDialogHandler;
                BrowserWindow.ChooseFileFromDialog(eventCreaterManagerModel.MediaPath);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                KeyValuePair<int, int> xAndY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_3-8_ img sp_00DF0Ouw8cv sx_1840c3");
                await BrowserWindow.MouseClickAsync(xAndY.Key, xAndY.Value);

                await Task.Delay(TimeSpan.FromSeconds(6), cancellationToken);

                KeyValuePair<int, int> xyValue = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_58al", 2);
                await BrowserWindow.MouseClickAsync(xyValue.Key, xyValue.Value);
                await BrowserWindow.EnterCharsAsync(eventCreaterManagerModel.EventName, delayBefore: 0, delayAtLast: 1);


                await BrowserWindow.BrowserActAsync(ActType.EnterByQuery, AttributeType.DataTestId,
                    "event-create-dialog-where-field", eventCreaterManagerModel.EventLocation, delayBefore: 2, delayAfter: 2);

                KeyValuePair<int, int> xAndYValue = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, FdConstants.TextBoxElement);
                await BrowserWindow.MouseClickAsync(xAndYValue.Key, xAndYValue.Value);
                await BrowserWindow.EnterCharsAsync(eventCreaterManagerModel.EventDescription, delayBefore: 1);

                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, "_3smp");

                string[] time = eventCreaterManagerModel.EventStartDate.ToString("HH: mm").Split(':');

                xAndYValue = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_3smp");
                await BrowserWindow.MouseClickAsync(xAndYValue.Key, xAndYValue.Value, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.SelectTextAsync(xAndYValue.Key, xAndYValue.Value, 68, xAndYValue.Value, delayBefore: 1, delayAfter: 5);
                await BrowserWindow.PressAnyKeyUpdated();
                await BrowserWindow.EnterCharsAsync($"/{eventCreaterManagerModel.EventStartDate.ToString("dd/MM/yyyy")}");
                await BrowserWindow.PressAnyKeyUpdated();

                xAndYValue = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_4nx3 _5pw6", 0);
                await BrowserWindow.MouseClickAsync(xAndYValue.Key + 10, xAndYValue.Value + 5, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.EnterCharsAsync(time[0].Trim().ToString());

                xAndYValue = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_4nx3 _5pw6", 1);
                await BrowserWindow.MouseClickAsync(xAndYValue.Key + 10, xAndYValue.Value + 5, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.EnterCharsAsync(time[1].Trim().ToString());

                time = eventCreaterManagerModel.EventEndDate.ToString("HH: mm").Split(':');

                //open end event value
                await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('_4ixq')[0].click()");

                xAndYValue = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_3smp", 1);
                await BrowserWindow.MouseClickAsync(xAndYValue.Key, xAndYValue.Value, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.SelectTextAsync(xAndYValue.Key, xAndYValue.Value, 68, xAndYValue.Value, delayBefore: 1, delayAfter: 5);
                await BrowserWindow.PressAnyKeyUpdated();
                await BrowserWindow.EnterCharsAsync($"/{eventCreaterManagerModel.EventEndDate.ToString("dd/MM/yyyy")}", 0.2);
                await BrowserWindow.PressAnyKeyUpdated();

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

                if (!eventCreaterManagerModel.IsGuestCanInviteFriends)
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.DataTestId,
                        "event_guests_can_invite__checkbox", delayBefore: 2, delayAfter: 2);

                if (!eventCreaterManagerModel.IsShowGuestList)
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.DataTestId,
                    "event_guestlist_enabled_checkbox", delayBefore: 2, delayAfter: 2);

                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.DataTestId,
                    "event-create-dialog-confirm-button", delayBefore: 2, delayAfter: 15);

                totalResponse = await BrowserWindow.GetPageSourceAsync();
            }
            catch (Exception)
            {

            }

            return new EventCreaterResponseHandler(new ResponseParameter() { Response = totalResponse }, eventCreaterManagerModel);
        }

        public async Task<EventCreaterResponseHandler> PublicEventCreater(DominatorAccountModel account, EventCreaterManagerModel eventCreaterManagerModel)
        {

            string totalResponse = string.Empty;
            try
            {
                await BrowserWindow.GoToCustomUrl($"{FdConstants.FbHomeUrl}events/", delayAfter: 10);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel,
                    "Create New Event", delayBefore: 2, delayAfter: 5);
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "x1lku1pv x1a2a7pz x78zum5 x1us19tq xh8yej3", delayAfter: 5);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                await BrowserWindow.EnterCharsAsync(eventCreaterManagerModel.EventName, 0.2, delayBefore: 0, delayAtLast: 1);
                string[] time = eventCreaterManagerModel.EventStartDate.ToString("HH:mm").Split(':');
                var startDate = $" {eventCreaterManagerModel.EventStartDate.Day} " +
                    $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(eventCreaterManagerModel.EventStartDate.Month)} {eventCreaterManagerModel.EventStartDate.Year}";
                var startTime = $" {time[0]}:{time[1]}";
                var endDate = $" {eventCreaterManagerModel.EventEndDate.Day} " +
                    $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(eventCreaterManagerModel.EventEndDate.Month)} {eventCreaterManagerModel.EventEndDate.Year}";
                time = eventCreaterManagerModel.EventEndDate.ToString("HH:mm").Split(':');
                var endTime = $" {time[0]}:{time[1]}";
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "x1hl2dhg xggy1nq x1a2a7pz xt0b8zv x1qq9wsj", delayAfter: 10);
                KeyValuePair<int, int> xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Start date\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().x",
                    customScriptY: "document.querySelectorAll('[aria-label=\"Start date\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().y");
                await BrowserWindow.MouseClickAsync(xAndY.Key, xAndY.Value, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.SelectTextAsync(xAndY.Key, xAndY.Value, 108, xAndY.Value, delayBefore: 1, delayAfter: 2);
                await BrowserWindow.EnterCharsAsync(startDate, 0.2, delayBefore: 0, delayAtLast: 1);
                await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 13, delayAtLast: 3);

                xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Start time\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().x",
                    customScriptY: "document.querySelectorAll('[aria-label=\"Start time\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().y");
                await BrowserWindow.MouseClickAsync(xAndY.Key, xAndY.Value, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.SelectTextAsync(xAndY.Key, xAndY.Value, 108, xAndY.Value, delayBefore: 1, delayAfter: 2);
                await Task.Delay(2000, cancellationToken);
                await BrowserWindow.EnterCharsAsync(startTime, 0.2, delayBefore: 0, delayAtLast: 1);
                await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 13, delayAtLast: 3);

                xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"End date\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().x",
                    customScriptY: "document.querySelectorAll('[aria-label=\"End date\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().y");
                await BrowserWindow.MouseClickAsync(xAndY.Key, xAndY.Value, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.SelectTextAsync(xAndY.Key, xAndY.Value, 108, xAndY.Value, delayBefore: 1, delayAfter: 2);
                await Task.Delay(2000, cancellationToken);
                await BrowserWindow.EnterCharsAsync(endDate, 0.2, delayBefore: 0, delayAtLast: 1);
                await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 13, delayAtLast: 3);

                xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"End time\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().x",
                    customScriptY: "document.querySelectorAll('[aria-label=\"End time\"]')[0].getElementsByClassName('x1a8lsjc x1pi30zi x1swvt13 x9desvi xh8yej3')[0].getBoundingClientRect().y");
                await BrowserWindow.MouseClickAsync(xAndY.Key, xAndY.Value, delayBefore: 0, delayAfter: 0.5);
                await BrowserWindow.SelectTextAsync(xAndY.Key, xAndY.Value, 108, xAndY.Value, delayBefore: 1, delayAfter: 2);
                await Task.Delay(2000, cancellationToken);
                await BrowserWindow.EnterCharsAsync(endTime, 0.3, delayBefore: 0, delayAtLast: 1);
                await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 13, delayAtLast: 3);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                int index = eventCreaterManagerModel.IsPrivatePostingVisibile ? 0 : 1;
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Privacy", delayAfter: 3);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "radio", index: index, delayAfter: 3);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Next", delayAfter: 3);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "radio", index: 6, delayAfter: 3);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Next", delayAfter: 3);
                xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Description\"]')[0].getElementsByTagName('textarea')[0].getBoundingClientRect().x",
                    customScriptY: "document.querySelectorAll('[aria-label=\"Description\"]')[0].getElementsByTagName('textarea')[0].getBoundingClientRect().y");
                await BrowserWindow.MouseClickAsync(xAndY.Key + 5, xAndY.Value + 5, delayAfter: 5);
                await BrowserWindow.EnterCharsAsync(" " + eventCreaterManagerModel.EventDescription, 0.2, delayBefore: 0, delayAtLast: 1);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Next", delayAfter: 3);
                BrowserWindow.ChooseFileFromDialog(eventCreaterManagerModel.MediaPath);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                xAndY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Upload Cover Photo\"]')[0].getBoundingClientRect().x",
                    customScriptY: "document.querySelectorAll('[aria-label=\"Upload Cover Photo\"]')[0].getBoundingClientRect().y");
                await BrowserWindow.MouseClickAsync(xAndY.Key + 2, xAndY.Value + 2, delayAfter: 5);
                //       await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Upload Cover Photo", delayAfter: 20);
                await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Publish Event", delayAfter: 10);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);


                totalResponse = await BrowserWindow.GetPageSourceAsync();
            }
            catch (Exception)
            {

            }

            return new EventCreaterResponseHandler(new ResponseParameter() { Response = totalResponse }, eventCreaterManagerModel);
        }

        public bool SendEventInvittationTofriends(DominatorAccountModel accountModel, string eventId, FacebookUser objFacebookUser, string note)
        {
            bool isRunning = true;
            List<string> inviteData = new List<string>();
            List<FacebookUser> chkFacebookUserList = new List<FacebookUser>();
            List<string> inviteeList = new List<string>();
            LoadPageSource(accountModel, $"{FdConstants.FbHomeUrl}events/{eventId}");

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.ExecuteScriptAsync(FdConstants.InviteEventButtonScript, delayInSec: 6);
                    var xAndYValues = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[placeholder=\"Search for people...\"]')[0].getBoundingClientRect().x",
                       customScriptY: $"document.querySelectorAll('[placeholder=\"Search for people...\"]')[0].getBoundingClientRect().y");
                    if (xAndYValues.Key == 0 && xAndYValues.Value == 0)
                        xAndYValues = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll('[type=\"text\"]')[0].getBoundingClientRect().x",
                        customScriptY: $"document.querySelectorAll('[type=\"text\"]')[0].getBoundingClientRect().y");
                    await BrowserWindow.MouseClickAsync(xAndYValues.Key + 5, xAndYValues.Value + 5, delayAfter: 2);

                    string userName = string.IsNullOrEmpty(objFacebookUser.FullName)
                        ? objFacebookUser.Familyname
                        : objFacebookUser.FullName;

                    BrowserWindow.SetResourceLoadInstance();

                    await BrowserWindow.EnterCharsAsync($"{userName}", 0.15, 0, 4);

                    inviteeList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"event\"", true);

                    BrowserWindow.SetResourceLoadInstance(false);
                    BrowserWindow.ClearResources();

                    chkFacebookUserList = new EventInviterResponseHandler(new ResponseParameter()).GetFacebookUser(inviteeList.LastOrDefault());

                    FacebookUser currentUser = chkFacebookUserList.FirstOrDefault(x => x.Familyname.Contains(objFacebookUser.Familyname) || x.UserId == objFacebookUser.UserId);

                    if (currentUser != null && !string.IsNullOrEmpty(currentUser.ProfilePicUrl))
                        objFacebookUser.ProfilePicUrl = currentUser.ProfilePicUrl;

                    string currentUserImageUrl = Regex.Replace(Regex.Replace(currentUser.ProfilePicUrl, "_nc_ohc=(.*?)&", string.Empty), "oh=(.*?)&", string.Empty);
                    string chkProfilePic = Regex.Split(FdRegexUtility.FirstMatchExtractor(currentUserImageUrl, "(.*?)[?]"), "/").LastOrDefault();

                    List<string> btnList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "x87ps6o x1lku1pv x1a2a7pz xzsf02u x1rg5ohu", ValueTypes.OuterHtml);
                    btnList.Reverse();

                    int userIndex = btnList.IndexOf(btnList.FirstOrDefault(x => x.Contains(chkProfilePic)));
                    if (userIndex < 0)
                    {
                        var userSelectButtonScript = FdConstants.GetScriptforTwoAttributesAndValue(ActType.Click, AttributeType.AriaLabel,
                       currentUser.Familyname, AttributeType.Role, "button", clickIndex: 0);
                        var clickResp = await BrowserWindow.ExecuteScriptAsync(userSelectButtonScript, 4);
                    }
                    else
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "x87ps6o x1lku1pv x1a2a7pz xzsf02u x1rg5ohu", index: userIndex, delayAfter: 4);

                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

                    BrowserWindow.SetResourceLoadInstance();
                    await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.AriaLabel, "Send Invitations", delayAfter: 4);
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel,
                        "Send Invitations", delayBefore: 2, delayAfter: 10);

                    inviteData = await BrowserWindow.GetPaginationDataList("{\"data\":{\"event_unified_invite\"", true);

                    BrowserWindow.SetResourceLoadInstance(false);

                    BrowserWindow.ClearResources();

                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;

            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return inviteData.Count() > 0;

        }

        public bool InviteAsPersonalMessage(DominatorAccountModel accountModel, string eventId, FacebookUser objFacebookUser, string note)
        {
            bool isRunning = true;
            var isSent = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Share", delayAfter: 5);
                    await ClickOrScrollToViewAndClick($"{FdConstants.SendInMessageButtonScript}[0]", timetoWait: 5);
                    await BrowserWindow.EnterCharsAsync($"{note}", 0, 0, 5);
                    string userName = string.IsNullOrEmpty(objFacebookUser.FullName) ? objFacebookUser.Familyname : objFacebookUser.FullName;
                    await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "input", "role", "textbox", "ariaLabel", "search for people and groups")}[0]", timetoWait: 3);
                    await BrowserWindow.EnterCharsAsync(userName, 0, 0, 5);
                    if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>div", "role", "checkbox", "textContent", userName)}.length"))?.Result?.ToString(), out int checkbox) && checkbox > 0)
                    {
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>div", "role", "checkbox", "textContent", userName)}[0].click()", delayInSec: 3);
                        isSent = (await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div>div", "role", "button", "ariaLabel", "send")}[0].click()", delayInSec: 5)).Success;
                    }
                    else
                        isSent = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.UserSendButtonScript(userName)}", delayInSec: 5)).Success;
                    await CloseAllMessageDialogues("Close");
                    await CloseAllMessageDialogues();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;

            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return isSent;
        }

        public IResponseHandler ShareToFriendProfiles(DominatorAccountModel account,
            string friendUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {

            LoadPageSource(account, friendUrl);
            List<string> responseList = new List<string>();
            string postResponse = string.Empty;
            List<Friends> listMentions = GetFriendsForMention(advanceSettingsModel, account);

            List<Friends> lisTagFriends = GetFriendsForTag(advanceSettingsModel, account);

            try
            {
                var createPostLocation = BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0].getBoundingClientRect().y").Result;
                if (createPostLocation.Value < 80 || createPostLocation.Value > ScreenResolution.Value - 200)
                {
                    BrowserWindow.ExecuteScript($"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0].scrollIntoViewIfNeeded()", delayInSec: 4);
                    createPostLocation = BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0].getBoundingClientRect().y").Result;

                }
                if (createPostLocation.Value > 80 && createPostLocation.Value < ScreenResolution.Value - 200)
                    BrowserWindow.MouseClick(createPostLocation.Key + 30, createPostLocation.Value + 10, delayAfter: 6);
                else
                    BrowserWindow.ExecuteScript($"{string.Format(FdConstants.ScriptforCreatePost, "write something")}[0].click()", 6);

                if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                {
                    BrowserWindow.EnterChars($" {postDetails.ShareUrl}     ", typingDelay: 0.05, delayAtLast: 2);

                    BrowserWindow.PressAnyKey(postDetails.ShareUrl.Length + 10, winKeyCode: 8, delay: 0.1, delayAtLast: 1);
                }

                MentionUsers(listMentions);

                if (listMentions.Count == 0)
                    BrowserWindow.EnterChars($" {postDetails.PostDescription} ", typingDelay: 0.1, delayAtLast: 1);
                else
                {
                    BrowserWindow.PressAnyKey(1, winKeyCode: 13, delay: 0.1, delayAtLast: 2);

                    foreach (var text in Regex.Split(postDetails.PostDescription, "\r\n"))
                    {

                        BrowserWindow.EnterChars($" {text}", typingDelay: 0.1);

                        BrowserWindow.EnterChars("\r\n", typingDelay: 0.05);

                        BrowserWindow.PressAnyKey(winKeyCode: 34, delay: 0.1, delayAtLast: 0.5);

                        BrowserWindow.EnterChars(" ", typingDelay: 0.05);

                    }
                }

                if (postDetails.MediaList.Count > 0 && postDetails.PostSource !=
                    PostSource.SharePost)
                {
                    BrowserWindow.ChooseFileFromDialog(pathList: postDetails.MediaList.ToList());
                    KeyValuePair<int, int> imageButtonPosition = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.UploadPhotoScriptWithIndex}.getBoundingClientRect().x",
                       customScriptY: $"{FdConstants.UploadPhotoScriptWithIndex}.getBoundingClientRect().y").Result;
                    if (imageButtonPosition.Value < 80 || imageButtonPosition.Value > ScreenResolution.Value - 200)
                    {
                        BrowserWindow.ExecuteScript($"{FdConstants.UploadPhotoScriptWithIndex}.scrollIntoViewIfNeeded()", delayInSec: 4);
                        imageButtonPosition = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.UploadPhotoScriptWithIndex}.getBoundingClientRect().x",
                       customScriptY: $"{FdConstants.UploadPhotoScriptWithIndex}.getBoundingClientRect().y").Result;
                    }
                    if (imageButtonPosition.Key != 0 && imageButtonPosition.Value != 0)
                        BrowserWindow.MouseClick(imageButtonPosition.Key + 7, imageButtonPosition.Value + 6, delayAfter: 15);
                    else
                        BrowserWindow.ExecuteScript($"{FdConstants.UploadPhotoScriptWithIndex}.click()", delayInSec: 15);
                    if (BrowserWindow.GetPageSource().Contains("Add photos/videos"))
                    {
                        var imageButtonPosition2 = BrowserWindow.GetXAndYAsync(customScriptX: $"{FdConstants.AddPhotoorVedioScript}[0].getBoundingClientRect().x",
                        customScriptY: $"{FdConstants.AddPhotoorVedioScript}[0].getBoundingClientRect().y").Result;
                        if (imageButtonPosition2.Key != 0 && imageButtonPosition2.Value != 0)
                            BrowserWindow.MouseClick(imageButtonPosition2.Key + 25, imageButtonPosition2.Value + 17, delayAfter: 45);
                    }
                    else
                        Task.Delay(25000).Wait(account.CancellationSource.Token);
                }

                TagFriendsSync(lisTagFriends);


                if (postDetails.FdPostSettings.IsPostAsStoryPost ||
                        advanceSettingsModel.IsPostAsStoryPost)
                {
                    BrowserWindow.BrowserAct(ActType.Click, attributeType: AttributeType.ClassName, attributeValue: "_3qpq _3qps  _2iel", delayAfter: 1);

                    BrowserWindow.BrowserAct(ActType.Click, attributeType: AttributeType.ClassName, attributeValue: "_3qpq _3qps  _2iel", clickIndex: 1, delayAfter: 1);

                    if (BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.DataTestId, "react-composer-post-button").Result.Contains("disabled"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", $"Cannot post story with post url {postDetails.ShareUrl}!");
                    }
                }
                Task.Delay(1000, cancellationToken).Wait();

                BrowserWindow.SetResourceLoadInstance();
                if (Convert.ToInt32(BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Next").Result) > 0)
                    BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Next", delayAfter: 4);
                BrowserWindow.BrowserAct(ActType.ActByQuery, attributeType: AttributeType.AriaLabel, attributeValue: "Post", delayBefore: 2, delayAfter: 60);

                responseList = BrowserWindow.GetPaginationDataList("story_create", isContains: true).Result;
                postResponse = responseList.FirstOrDefault(x => x.StartsWith("{\"data\":{\"story_create"));
                BrowserWindow.SetResourceLoadInstance(false);
                BrowserWindow.ClearResources();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            Thread.Sleep(5000);

            string response = BrowserWindow.GetPageSourceAsync().Result;

            return new PublisherBrowserResponseHandler(new ResponseParameter() { Response = response }, postResponse, false);
        }

        public MakeAdminResponseHandler MakeGroupAdmin(DominatorAccountModel accountModel, string groupId, FacebookUser facebookUser)
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl($"{FdConstants.FbHomeUrl}groups/{groupId}/members/", delayAfter: 7);

                    string userName = string.IsNullOrEmpty(facebookUser.Username)
                        ? facebookUser.Familyname
                        : facebookUser.Username;

                    KeyValuePair<int, int> xAndYValues = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_357u _55r1 _58ak _3ct8");
                    await BrowserWindow.MouseClickAsync(xAndYValues.Key, xAndYValues.Value);
                    await BrowserWindow.EnterCharsAsync($"{userName}", 0.15, 0, 1);

                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

                    //At the place of with account id it is coming it is present and from
                    xAndYValues = await BrowserWindow.GetXAndYAsync(AttributeType.DataTestId, $"admin_action_menu_button-search-{facebookUser.UserId}");
                    await BrowserWindow.MouseClickAsync(xAndYValues.Key, xAndYValues.Value, 0, delayAfter: 2);

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.DataTestId, $"admin_action_menu_button-search-{facebookUser.UserId}", delayAfter: 2);

                    var option = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('_54nh')[0].innerText");

                    if (option.Result.ToString() == "Remove as Admin" || option.Result.ToString() == "Cancel admin invitation")
                    {
                        isRunning = false;
                        return new MakeAdminResponseHandler(new ResponseParameter(), "Already Sent Invitation");
                    }
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_54nh", delayAfter: 1);
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName
                        , "_42ft _4jy0 layerConfirm uiOverlayButton _4jy3 _4jy1 selected _51sy", delayAfter: 2);

                    isRunning = false;

                    return new MakeAdminResponseHandler(new ResponseParameter());
                }
                catch (Exception)
                {
                    isRunning = false;
                    return new MakeAdminResponseHandler(new ResponseParameter());
                }

            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return new MakeAdminResponseHandler(new ResponseParameter());
        }

        public string SendFriendRequestSingleUser(DominatorAccountModel accountModel, string userId)
        {
            bool isRunning = true;
            string status = string.Empty;
            string pageSource = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance();
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.AddFriendScript}.length"))?.Result?.ToString(), out int addFriendcount) && addFriendcount > 0)
                    {
                        await ClickOrScrollToViewAndClick($"{FdConstants.AddFriendScript}[0]", timetoWait: 5);
                        List<string> listOfValue = await BrowserWindow.GetPaginationDataList("friend_request_send", true);

                        status = new SendRequestResponseHandler(new ResponseParameter() { Response = (listOfValue.LastOrDefault(x => x.Contains("friend_request_send"))) ?? string.Empty }).RequestStatus;

                        if (string.IsNullOrEmpty(status))
                        {
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "h676nmdw buofh1pr h8xcmbcu", delayAfter: 4);
                            listOfValue = await BrowserWindow.GetPaginationDataList("friend_request_send", true);
                            status = new SendRequestResponseHandler(new ResponseParameter() { Response = (listOfValue.LastOrDefault(x => x.Contains("friend_request_send"))) ?? string.Empty }).RequestStatus;
                        }

                        if (string.IsNullOrEmpty(status))
                        {
                            listOfValue = await BrowserWindow.GetPaginationDataList("for (;;);{\"__ar\":1,\"error\":", true);
                            status = new SendRequestResponseHandler(new ResponseParameter() { Response = listOfValue.LastOrDefault() }).RequestStatus;
                        }

                        if (status == "confirmation required")
                        {
                            await BrowserWindow.ExecuteScriptAsync(
                                "document.getElementsByClassName('_42ft _4jy0 layerConfirm uiOverlayButton _4jy3 _4jy1 selected _51sy')[0].click()");

                            await Task.Delay(2000, cancellationToken);
                            listOfValue = await BrowserWindow.GetPaginationDataList("for (;;);{\"__ar\":1,\"payload\":{\"success\":true}", true);
                            status = new SendRequestResponseHandler(new ResponseParameter() { Response = listOfValue.LastOrDefault() }).RequestStatus;
                        }

                        pageSource = await BrowserWindow.GetPageSourceAsync();
                    }
                    else if (int.TryParse((await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3\"]')].filter(x=>x.ariaLabel===\"Follow\"||x.textContent===\"Follow\").length"))?.Result?.ToString(), out int FollowButtonCount) && FollowButtonCount > 0)
                    {
                        await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x3nfvp2 xdt5ytf xl56j7k x1n2onr6 xh8yej3\"]')].filter(x=>x.ariaLabel===\"Follow\"||x.textContent===\"Follow\")[0]", timetoWait: 5);
                        List<string> listOfValue = await BrowserWindow.GetPaginationDataList("actor_subscribe", true);

                        status = new SendRequestResponseHandler(new ResponseParameter() { Response = (listOfValue.LastOrDefault(x => x.Contains("actor_subscribe"))) ?? string.Empty }).RequestStatus;
                    }

                    status = (string.IsNullOrEmpty(status) &&
                        pageSource.Contains("aria-label=\"Friends\"")) ? "Already Friend" : (string.IsNullOrEmpty(status) &&
                        pageSource.Contains("aria-label=\"Cancel Request\"")) ? "Already Friend Request Sent" :
                        (string.IsNullOrEmpty(status) && addFriendcount == 0) ? "Not Available to sent Request" : status;
                }
                catch (Exception ex) { ex.DebugLog(); }

                BrowserWindow.SetResourceLoadInstance(false);
                BrowserWindow.ClearResources();

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return status;
        }

        public bool Unfriend(DominatorAccountModel accountModel)
        {
            bool isRunning = true;
            bool isSuccess = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    int index = 0;
                    int btnCount = await BrowserWindow.GetItemCountInnerHtml(ActType.ActByQuery, AttributeType.AriaLabel, "Friends");
                    if (btnCount > 0)
                        index = btnCount - 1;

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Friends", index: index);

                    await Task.Delay(3000, cancellationToken);

                    List<string> listValue = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", ValueTypes.InnerText);
                    listValue.Reverse();
                    int indexValue = listValue.IndexOf("Unfriend") == -1 ? 4 : listValue.IndexOf("Unfriend");

                    await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.Role, "menuitem");

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: indexValue);

                    listValue = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.AriaLabel, "Confirm", ValueTypes.InnerText);
                    listValue.Reverse();
                    indexValue = listValue.IndexOf("Confirm") == -1 ? 4 : listValue.IndexOf("Confirm");

                    BrowserWindow.SetResourceLoadInstance();

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Confirm", index: indexValue);

                    await Task.Delay(4000, cancellationToken);

                    List<string> listOfValue = await BrowserWindow.GetPaginationDataList("friend_remove", true);

                    isSuccess = listOfValue.LastOrDefault(x => x.Contains("friend_remove")) != null;

                }
                catch (Exception ex) { ex.DebugLog(); }

                BrowserWindow.SetResourceLoadInstance(false);
                BrowserWindow.ClearResources();
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return isSuccess;
        }

        public bool UnfollowFriend(DominatorAccountModel accountModel)
        {
            bool isRunning = true;
            bool isSuccess = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var friendScriptLength = FdConstants.GetScriptforTwoAttributesAndValue(ActType.GetLength, AttributeType.AriaLabel,
                       "Friends", AttributeType.Role, "button", clickIndex: 0);
                    int friendCount = 0;
                    var friendScriptresp = await BrowserWindow.ExecuteScriptAsync(friendScriptLength);
                    if (friendScriptresp.Success && int.TryParse(friendScriptresp.Result.ToString(), out friendCount) && friendCount > 0)
                    {
                        var friendScript = FdConstants.GetScriptforTwoAttributesAndValue(ActType.Click, AttributeType.AriaLabel,
                       "Friends", AttributeType.Role, "button", clickIndex: 0);
                        await BrowserWindow.ExecuteScriptAsync(friendScript, delayInSec: 5);
                    }
                    else
                    {
                        var followingScript = FdConstants.GetScriptforTwoAttributesAndValue(ActType.Click, AttributeType.AriaLabel,
                       "Following", AttributeType.Role, "button", clickIndex: 0);
                        await BrowserWindow.ExecuteScriptAsync(followingScript, delayInSec: 5);
                    }
                    List<string> listValue = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", ValueTypes.InnerText);
                    listValue.Reverse();
                    int indexValue = -1;
                    if (listValue.Contains("Unfollow"))
                        indexValue = listValue.IndexOf("Unfollow");
                    if (indexValue > -1)
                    {
                        await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.Role, "menuitem");

                        BrowserWindow.SetResourceLoadInstance();

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: indexValue, delayAfter: 5);

                        await Task.Delay(4000, cancellationToken);

                        List<string> listOfValue = await BrowserWindow.GetPaginationDataList("{\"data\":{\"actor_unsubscribe\"", true);

                        isSuccess = listOfValue.Count > 0;
                    }
                }
                catch (Exception ex) { ex.DebugLog(); }

                BrowserWindow.SetResourceLoadInstance(false);
                BrowserWindow.ClearResources();
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return isSuccess;
        }

        public void GetCommentersFromPost(DominatorAccountModel account, BrowserReactionType entity, string postUrl,
          bool isLoadPost = true)
        {
            if (BrowserWindow == null)
                return;
            postUrl = postUrl.Contains(FdConstants.FbHomeUrl)
                ? postUrl
                : $"{FdConstants.FbHomeUrl}{postUrl}";
            if (postUrl.Contains("/videos/"))
                postUrl = postUrl?.Replace("/videos/", "/posts/");
            if (postUrl.Contains("/photos/"))
                postUrl = postUrl?.Replace("/photos/", "/posts/");
            var pageSource = LoadPageSource(account, postUrl);

        }

        public IResponseHandler ScrollWindowAndGetDataForCommentsForSinglePost(DominatorAccountModel accountModel,
           FacebookPostDetails facebookPostDetails, FbEntityType postCommentor, int noOfPageToScroll, List<FdPostCommentDetails> CommentList,
           int lastPageNo = 0)
        {

            bool isRunning = true;
            bool hasMoreRsults = true;
            List<string> itemList = new List<string>();
            string pageSource = gotoPOstUrl(accountModel, facebookPostDetails);
            if (pageSource.Contains("This content isn't available at the moment"))
                return new FdCommentResponseHandler(new ResponseParameter() { Response = pageSource }, facebookPostDetails, itemList, false);

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance();
                    await ClickOrScrollToViewAndClick($"{FdConstants.CommentFilterLocScript}[0]", timetoWait: 3);
                    var indexSelectedFilter = 0;
                    var commentFilterList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                        "menuitem");
                    commentFilterList.Reverse();
                    if (commentFilterList.Any(x => x.Contains("All comments")))
                        indexSelectedFilter = commentFilterList.FindIndex(x => x.Contains("All comments"));
                    else if (commentFilterList.Any(x => x.Contains("Newest")))
                        indexSelectedFilter = commentFilterList.FindIndex(x => x.Contains("Newest"));
                    if (indexSelectedFilter > -1)
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: indexSelectedFilter, delayAfter: 10);
                    itemList = await NormalScrollComments(AttributeType.ClassName, CommentList, lastPageNo, noOfPageToScroll, facebookPostDetails);
                    pageSource = await BrowserWindow.GetPageSourceAsync();

                }
                catch (Exception ex) { ex.DebugLog(); }

                hasMoreRsults = !facebookPostDetails.IsLastPost;
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new FdCommentResponseHandler(new ResponseParameter() { Response = pageSource }, facebookPostDetails, itemList, hasMoreRsults);


        }

        public async Task<List<string>> NormalScrollComments(AttributeType attributeType, List<FdPostCommentDetails> CommentList,
            int lastPageNo, int noOfPageToScroll, FacebookPostDetails postDetails, int parentIndex = 0, FbEntityType entity = FbEntityType.Post)
        {
            var itemList = new List<string>();
            var tempjsonresponselist = new List<string>();
            try
            {
                int failedCount = 0;
                int previousCount = 0;
                int currentCount = 0;
                try
                {
                    while (lastPageNo < noOfPageToScroll && failedCount < 2)
                    {
                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2 + 100, ScreenResolution.Value / 2
                                    , 0, -500, delayAfter: 5, scrollCount: 5);
                        if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "textContent", "view more comments")}.length"))?.Result?.ToString(), out int viewMorecount) && viewMorecount > 0)
                            await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "textContent", "view more comments")}[0]", timetoWait: 5);
                        tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"data\":{\"node\":{\"__typename\":\"Feedback\"", true);
                        if (tempjsonresponselist.Count == 0)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Feedback\"", true);
                        tempjsonresponselist.RemoveAll(x => itemList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        itemList.AddRange(tempjsonresponselist);
                        lastPageNo++;
                        if (currentCount == previousCount && itemList.Count == currentCount)
                        {
                            failedCount++;
                            if (failedCount < 1)
                                await Task.Delay(2000, cancellationToken);
                        }
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

                if (failedCount >= 2)
                    postDetails.IsLastPost = true;
                return itemList;
            }
            catch (Exception e)
            {
                e.DebugLog();
                return itemList;
            }
        }

        public void SearchByAlbum(DominatorAccountModel account, SearchKeywordType entity, string url)
        {
            bool isRunning = true;

            Task.Delay(2000, cancellationToken).Wait();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl(url, delayAfter: 5);
                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(2000, cancellationToken).Wait();

        }



        public IResponseHandler UpdateFriendsDetailsFromPages(DominatorAccountModel account, List<string> lstPageId)
        {
            bool isRunning = true;

            int count = 0;
            bool isCompleted = false;
            List<string> inviteeList = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {

                try
                {

                    while (count < 5 && !isCompleted)
                    {
                        string randomPageId = lstPageId.GetRandomItem();

                        randomPageId = randomPageId.Contains(FdConstants.FbHomeUrl) ? randomPageId : $"{FdConstants.FbHomeUrl}{randomPageId}";

                        await BrowserWindow.GoToCustomUrl($"{randomPageId}", delayAfter: 20);

                        BrowserWindow.SetResourceLoadInstance();

                        KeyValuePair<int, int> keyValuePair = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "hu5pjgll lzf7d6o1 sp_0Y_EE8spOmD sx_37a85b");

                        await BrowserWindow.MouseClickAsync(keyValuePair.Key, keyValuePair.Value, delayAfter: 2);

                        List<string> menuItemList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                            "menuitem");

                        if (menuItemList.Count == 0)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                                         "hu5pjgll lzf7d6o1 sp_0Y_EE8spOmD sx_37a85b", delayAfter: 20);

                            menuItemList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                                "menuitem");
                        }

                        menuItemList.Reverse();

                        int invitePageIndex = menuItemList.IndexOf(menuItemList.FirstOrDefault(x => x.Contains("Invite Friends")));
                        count++;
                        if (!menuItemList.Any(x => x.Contains("Invite Friends")))
                            continue;


                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                            , 0, -50, delayAfter: 1, scrollCount: 1);

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: invitePageIndex);

                        await Task.Delay(TimeSpan.FromSeconds(25));

                        inviteeList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"page\":{\"name\":", true);

                        if (inviteeList.Count <= 0)
                            inviteeList = await BrowserWindow.GetPaginationDataList("\"payload\":{\"entries\":[{\"uid\":", true);

                        if (inviteeList.Count > 0)
                            isCompleted = true;

                        BrowserWindow.ClearResources();

                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return new FriendsUpdateResponseHandler(new ResponseParameter() { Response = inviteeList.LastOrDefault() }, false);

        }

        public IResponseHandler UpdateOwnPagesDetails(DominatorAccountModel account, FacebookUser facebookUser)
        {
            bool isRunning = true;
            int failedCount = 0;
            var pageSource = LoadPageSource(account, FdConstants.FbHomeUrl, clearandNeedResource: true);
            var JsonResponseList = new List<string>();
            bool hasOwnPages = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var tempjsonresponselist = new List<string>();
                    BrowserWindow.ClearResources();
                    await clickSeeMoreButton();
                    await BrowserWindow.ExecuteScriptAsync($"{FdConstants.PagesButtonScript}[0].click()", delayInSec: 5);
                    await LoadSource();
                    int previousCount = 0;
                    int currentCount = 0;
                    while (failedCount < 2)
                    {
                        tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"can_create_page\":", true);
                        if (tempjsonresponselist.Count == 0)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"has_biz_web_access\"", true);
                        if (tempjsonresponselist.FirstOrDefault() != null && tempjsonresponselist.FirstOrDefault().Contains("\"profile_switcher_eligible_profiles\":{\"nodes\":[]}"))
                        {
                            hasOwnPages = false;
                            isRunning = false;
                            return;
                        }
                        if (failedCount < 1)
                            await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                            , 0, -210, delayAfter: 3, scrollCount: 4);
                        tempjsonresponselist.RemoveAll(x => JsonResponseList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        JsonResponseList.AddRange(tempjsonresponselist);

                        if (currentCount == previousCount && JsonResponseList.Count == currentCount) failedCount++;
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();

                }
                catch (Exception ex) { ex.DebugLog(); }

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            if (!hasOwnPages)
                return new FdSearchPageResponseHandler(new ResponseParameter() { Response = pageSource }, new List<string>(), false);
            else
                return new FdSearchPageResponseHandler(new ResponseParameter() { Response = pageSource }, JsonResponseList, false);
        }



        public void ChangeLanguage(DominatorAccountModel account, string language)
        {
            bool isRunning = true;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    string pageSource = await BrowserWindow.GetPageSourceAsync();
                    string languageCode = FdRegexUtility.FirstMatchExtractor(pageSource, FdConstants.LanguageCodeRegex);//FdRegexUtility.FirstMatchExtractor(pageSource, "currentLocale\":\"(.*?)\"");
                    if (string.IsNullOrEmpty(languageCode))
                        languageCode = FdRegexUtility.FirstMatchExtractor(pageSource, FdConstants.LanguageCodeRegex2);
                    if (string.IsNullOrEmpty(languageCode))
                        languageCode = FdRegexUtility.FirstMatchExtractor(pageSource, FdConstants.LanguageCodeUpdatedRegex);
                    if (string.IsNullOrEmpty(languageCode))
                        languageCode = FdRegexUtility.FirstMatchExtractor(pageSource, FdConstants.LanguageCodeUpdatedRegex2);
                    languageCode = !string.IsNullOrEmpty(languageCode) ? languageCode : pageSource.Contains(FdConstants.FdDefaultLanguage)
                    ? FdConstants.FdDefaultLanguage : pageSource.Contains("en_US") ? "en_US" : string.Empty;
                    try
                    {
                        if (!FdConstants.AccountLanguage.ContainsKey(account.AccountBaseModel.UserId)
                                        && !string.IsNullOrEmpty(languageCode))
                            FdConstants.AccountLanguage.Add(account.AccountBaseModel.UserId, languageCode);
                        else if (FdConstants.AccountLanguage[account.AccountBaseModel.UserId] != languageCode
                             && !string.IsNullOrEmpty(languageCode))
                            FdConstants.AccountLanguage[account.AccountBaseModel.UserId] = languageCode;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (languageCode == FdConstants.FdDefaultLanguage || languageCode == "en_US")
                    {
                        isRunning = false;
                        return;
                    }
                    await BrowserWindow.GoToCustomUrl("https://www.facebook.com/settings/?tab=language", 6);
                    await LoadSource();
                    await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.includes(\"English\"))[0]", timetoWait: 7);
                    await ClickOrScrollToViewAndClick("[...document.querySelectorAll('input[type=\"text\"]')][0]", timetoWait: 2);
                    await BrowserWindow.EnterCharsAsync("English", delayAtLast: 5);
                    await ClickOrScrollToViewAndClick("[...document.querySelectorAll('div[role=\"radio\"]')][0]", timetoWait: 2);
                    await Task.Run(() => BrowserWindow.Refresh());
                    await LoadSource();
                    await BrowserWindow.GoToCustomUrl(FdConstants.FbHomeUrl, 8);
                    await LoadSource();


                }
                catch (ArgumentException)
                {

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500).Wait(cancellationToken);

        }

        public List<FacebookUser> GetMutualFriends(DominatorAccountModel accountModel, string userId)
        {
            bool isRunning = true;

            string pageSource = string.Empty;

            DateTime currentDate = DateTime.Now;

            Application.Current.Dispatcher.Invoke(() =>
            {
                CustomBrowserWindow = new BrowserWindow(accountModel, targetUrl: FdConstants.MutualFriendUrl(userId), customUse: true)
                {
                    Visibility = Visibility.Hidden
                };
#if DEBUG
                CustomBrowserWindow.Visibility = Visibility.Visible;
#endif
                CustomBrowserWindow.BrowserSetCookie();
                CustomBrowserWindow.Show();
            });

            do
            {
                isRunning = true;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        pageSource = await CustomBrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "", valueType: ValueTypes.OuterHtml);

                        await RefereshPage(pageSource);
                    }
                    catch (Exception ex) { ex.DebugLog(); }
                    isRunning = false;
                });
                while (isRunning)
                    Task.Delay(500, cancellationToken).Wait();
            }
            while ((string.IsNullOrEmpty(pageSource)) && (DateTime.Now - currentDate).TotalSeconds < 60);

            isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    pageSource = await CustomBrowserWindow.GetPageSourceAsync();
                    isRunning = false;
                    CustomBrowserWindow.Close();
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Thread.Sleep(300);

            return new MutualFriendsResponseHandler(new ResponseParameter() { Response = pageSource }).ObjFdScraperResponseParameters.ListUser;
        }

        public IResponseHandler ScrollWindowGetRepliesForComment(DominatorAccountModel account, FdPostCommentDetails commentDetails,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "")
        {
            var itemList = new List<string>();
            var tempjsonresponselist = new List<string>();
            bool isRunning = true;
            string pageSource = string.Empty;

            bool hasMoreRsults = true;

            if (string.IsNullOrEmpty(className))
                className = entity == FbEntityType.NewFeedPost ? FdConstants.NewsFeedPostElement : FdConstants.PostElement4Class;
            if (!BrowserWindow.CurrentUrl().Contains(commentDetails.CommentId))
                LoadPageSource(account, $"{FdConstants.FbHomeUrl}/{commentDetails.CommentId}", true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                int failedCount = 0;
                int previousCount = 0;
                int currentCount = 0;
                BrowserWindow.ClearResources();
                BrowserWindow.SetResourceLoadInstance();
                try
                {
                    await ClickOrScrollToViewAndClick($"{FdConstants.CommentFilterLocScript}[0]", timetoWait: 3);
                    var indexSelectedFilter = 0;
                    var commentFilterList = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role,
                            "menuitem");
                    commentFilterList.Reverse();
                    if (commentFilterList.Any(x => x.Contains("All comments")))
                        indexSelectedFilter = commentFilterList.FindIndex(x => x.Contains("All comments"));
                    else if (commentFilterList.Any(x => x.Contains("Newest")))
                        indexSelectedFilter = commentFilterList.FindIndex(x => x.Contains("Newest"));
                    if (indexSelectedFilter > -1)
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: indexSelectedFilter, delayAfter: 10);
                    if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentPreviousReplyScript}.length"))?.Result?.ToString(), out int previousrepliescount) && previousrepliescount > 0)
                        await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentPreviousReplyScript}[0].click()", delayInSec: 5);
                    try
                    {
                        while (lastPageNo < noOfPageToScroll && failedCount < 2)
                        {
                            await BrowserWindow.ExecuteScriptAsync($"{FdConstants.PostCommentReplyScript}[{FdConstants.PostCommentReplyScript}.length-1].scrollIntoViewIfNeeded()",delayInSec:4);
                            if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "textContent", "view more replies")}.length"))?.Result?.ToString(), out int viewMorecount) && viewMorecount > 0)
                                await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "textContent", "view more replies")}[0]", timetoWait: 5);
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"data\":{\"node\":{\"__typename\":\"Feedback\"", true, "\"replies_connection\":{\"edges\"");
                            if (tempjsonresponselist.Count == 0)
                                tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"Feedback\"", true, "\"replies_connection\":{\"edges\"");
                            tempjsonresponselist.RemoveAll(x => itemList.Any(y => y.Contains(x)));
                            currentCount += tempjsonresponselist.Count;
                            itemList.AddRange(tempjsonresponselist);
                            lastPageNo++;
                            if (currentCount == previousCount && itemList.Count == currentCount)
                            {
                                failedCount++;
                                if (failedCount < 1)
                                    await Task.Delay(2000, cancellationToken);
                            }
                            else
                            {
                                failedCount = 0;
                                previousCount = currentCount;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
            return new CommentRepliesResponseHandler(itemList, new ResponseParameter() { Response = pageSource }, commentDetails, hasMoreRsults, false);
        }




        public bool GoToNextPagination(DominatorAccountModel accountModel)
        {
            BrowserWindow.MouseClick(5, 5, delayAfter: 2);
            return true;
        }

        public async Task RefereshPage(string pageSource)
        {
            if (pageSource.Contains("<body></body>") || string.IsNullOrEmpty(pageSource))
            {
                CustomBrowserWindow.Refresh();
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
        public bool CopyPasteText(string message, AttributeType attributeType = AttributeType.ClassName, string attributeValue = "", string activityType = "POST", int index = 0)
        {
            return CopyPasteTextAsync(message, attributeType, attributeValue, activityType, index).Result;
        }

        public async Task<bool> CopyPasteTextAsync(string message, AttributeType attributeType = AttributeType.ClassName, string attributeValue = "",
               string activityType = "POST", int parentIndex = 0)
        {
            try
            {
                int retryCount = 0;

                string pageElement = string.Empty;

                await _copyPasteLock.WaitAsync();

                List<string> splitMessage = Regex.Split(message?.Replace("\r\n", "\n"), "\n").Where(x => !string.IsNullOrEmpty(x))?.ToList();

                while (pageElement == null || splitMessage.Any(x => !pageElement.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Contains(x)))
                {
                    var chkpageElement = pageElement?.Replace("\r\n", string.Empty)?.Replace("\n", string.Empty) ?? string.Empty;
                    if (!string.IsNullOrEmpty(chkpageElement) && splitMessage.Any(x => !chkpageElement.Contains(x)))
                        break;

                    if (retryCount > 0)
                        await BrowserWindow.CopyPasteContentAsync(winKeyCode: 90, delayAtLast: 10);

                    if (retryCount >= 2)
                    {
                        await BrowserWindow.CopyPasteContentAsync(winKeyCode: 86, delayAtLast: 5);
                        _copyPasteLock.Release();
                        return false;
                    }

                    try
                    {
                        await BrowserWindow.CopyPasteContentAsync(message, 86, delayAtLast: 10);

                        if (activityType == "POST")
                            await ExpandPostWindowAsyc(attributeValue);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (activityType == "POST")
                    {
                        pageElement = await BrowserWindow.GetElementValueAsync(ActType.GetValue, attributeType,
                                attributeValue, ValueTypes.InnerText, clickIndex: parentIndex);
                        if (string.IsNullOrEmpty(pageElement))
                        {
                            var length = Convert.ToInt32(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.Role, "textbox"));
                            pageElement = await BrowserWindow.GetElementValueAsync(ActType.CustomActByQueryType, AttributeType.Role, "textbox", clickIndex: length - 1, value: "innerText");
                        }
                        if (message.Contains("#"))
                            pageElement = System.Web.HttpUtility.HtmlDecode(pageElement).Replace("\\\"", "\"");
                    }
                    else if (activityType == "ReplyToComment")
                    {
                        List<string> listReplyInnerText = await BrowserWindow.GetListInnerHtmlChildElement(ActType.GetValue, AttributeType.ClassName,
                                FdConstants.PostElement4Class, AttributeType.ClassName, "xdj266r xexx8yu x4uap5 x18d9i69 xkhd6sd", valueType: ValueTypes.InnerText, parentIndex: parentIndex);
                        pageElement = listReplyInnerText.FirstOrDefault(x => x.Contains(message));
                        pageElement = string.IsNullOrEmpty(pageElement) ? listReplyInnerText.FirstOrDefault() : pageElement;

                        pageElement = !string.IsNullOrEmpty(pageElement) && pageElement == "\n" ? listReplyInnerText[listReplyInnerText.Count - 1] : pageElement;

                        if (string.IsNullOrEmpty(pageElement))
                            pageElement = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "x78zum5 x13a6bvl x1a02dak"
                                , ValueTypes.InnerText);
                        if (string.IsNullOrEmpty(pageElement))
                        {
                            var length = Convert.ToInt32(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.Role, "textbox"));
                            pageElement = await BrowserWindow.GetElementValueAsync(ActType.CustomActByQueryType, AttributeType.Role, "textbox", clickIndex: length - 1, value: "innerText");
                        }
                    }
                    else if (activityType == "Message")
                    {
                        pageElement = string.IsNullOrEmpty(pageElement) ? await BrowserWindow.GetChildElementValueAsync(ActType.GetValue, AttributeType.ClassName, FdConstants.MessageTextElement
                            , AttributeType.ClassName, FdConstants.PostingTextLocClass, ValueTypes.InnerText, 0, parentIndex, 0) : pageElement;
                        if (string.IsNullOrEmpty(pageElement))
                            pageElement = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, FdConstants.MessageText3Element, ValueTypes.InnerText);
                        if (string.IsNullOrEmpty(pageElement))
                            pageElement = await BrowserWindow.GetElementValueAsync(ActType.CustomActByQueryType, AttributeType.AriaLabel, "Message", clickIndex: 1, value: "innerText");
                        if (string.IsNullOrEmpty(pageElement))
                        {
                            var length = Convert.ToInt32(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.Role, "textbox"));
                            pageElement = await BrowserWindow.GetElementValueAsync(ActType.CustomActByQueryType, AttributeType.Role, "textbox", clickIndex: length - 1, value: "innerText");
                        }
                    }
                    else if (activityType == "STORY")
                    {
                        pageElement = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.StartTypingButtonForImageScript}[0].innerText"))?.Result?.ToString().Trim();
                        if (string.IsNullOrEmpty(pageElement))
                            pageElement = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.StartTypingButtonForTextScript}[0].InnerHtml"))?.Result?.ToString().Trim();
                    }
                    else if (activityType == "FB Sell")
                    {
                        pageElement = await BrowserWindow.GetElementValueAsync(ActType.GetValue, attributeType,
                                attributeValue, ValueTypes.OuterHtml, clickIndex: parentIndex);

                        if (!string.IsNullOrEmpty(pageElement) && pageElement.Contains("Price"))
                        {
                            pageElement = pageElement.Replace(",", "");
                        }
                    }
                    else if (activityType == "Chat")
                        pageElement = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, FdConstants.TextBoxElement, ValueTypes.InnerText);
                    else if (activityType == "Comment")
                        pageElement = (await BrowserWindow.ExecuteScriptAsync($"{FdConstants.CommentTextBoxScript}.innerText"))?.Result?.ToString().Trim();
                    else
                        pageElement = !string.IsNullOrEmpty(pageElement) ? pageElement : await BrowserWindow.GetElementValueAsync(ActType.CustomActType, AttributeType.ClassName,
                            FdConstants.PostElementClass, value: $"getElementsByClassName('{FdConstants.TextBoxElement}')[0]", valueType: ValueTypes.InnerText, clickIndex: parentIndex);

                    retryCount++;
                }

                if (retryCount < 2)
                {
                    _copyPasteLock.Release();
                    return true;
                }

            }
            catch (Exception ex)
            {
                _copyPasteLock.Release();
                ex.DebugLog();
            }
            _copyPasteLock.Release();

            return false;
        }


        public void ExpandPostWindow(string className)
        {
            try
            {
                BrowserWindow.BrowserAct(ActType.ScrollIntoView, className, attributeType: AttributeType.ClassName,
                    attributeValue: className, delayAfter: 1);

                BrowserWindow.MouseScroll(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                    , 0, 150, delayAfter: 2, scrollCount: 1);

                KeyValuePair<int, int> createPostLocation = BrowserWindow.GetXAndYAsync(AttributeType.ClassName, className).Result;

                if (createPostLocation.Key != 0 && createPostLocation.Value != 0)
                    BrowserWindow.MouseClick(createPostLocation.Key + 170, createPostLocation.Value + 40, delayAfter: 1);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task ExpandPostWindowAsyc(string className)
        {
            try
            {
                BrowserWindow.BrowserAct(ActType.ScrollIntoView, className, attributeType: AttributeType.ClassName,
                    attributeValue: className, delayAfter: 1);

                await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2
                    , 0, 50, delayAfter: 2, scrollCount: 1);

                KeyValuePair<int, int> createPostLocation = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, className);

                if (createPostLocation.Key != 0 || createPostLocation.Value != 0)
                    await BrowserWindow.MouseClickAsync(createPostLocation.Key + 170, createPostLocation.Value + 80, delayAfter: 1);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

       
        

        public int GetfriendsCount(DominatorAccountModel account)
        {
            bool isRunning = true;
            string countData = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var pageSource = BrowserWindow.GetPageSource();
                    Task.Delay(2000, cancellationToken).Wait();
                    var countDataLst = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "b6ax4al1 lq84ybu9 hf30pyar om3e55n1 tr46kb4q", ValueTypes.InnerText);
                    if (countDataLst.Count() == 0)
                        countDataLst = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "x1lliihq x6ikm8r x10wlt62 x1n2onr6 x1j85h84", ValueTypes.InnerText);
                    var index = countDataLst.IndexOf(countDataLst.FirstOrDefault(x => x.ToLower().Contains("friends")));
                    countData = await BrowserWindow.GetElementValueAsync(ActType.GetValue,
                    AttributeType.ClassName, FdConstants.FriendsCount, valueType: ValueTypes.InnerText);
                    if (string.IsNullOrEmpty(countData) || !countData.Contains("friends"))
                        countData = await BrowserWindow.GetElementValueAsync(ActType.GetValue,
                    AttributeType.ClassName, FdConstants.FriendsCount2, valueType: ValueTypes.InnerText);
                    if (string.IsNullOrEmpty(countData) || !countData.Contains("friends"))
                        countData = countDataLst[index];
                    //     countData = BrowserWindow.EvaluateScript($"document.getElementsByClassName('j83agx80 cbu4d94t ew0dbk1b irj2b8pg')[0].getElementsByClassName('a8c37x1j ni8dbmo4 stjgntxs l9j0dhe7 ojkyduve')[0].innerText").Result?.ToString();

                    countData = FdFunctions.GetIntegerOnlyString(countData);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000).Wait();

            return FdFunctions.IsIntegerOnly(countData) ? int.Parse(countData) : 0;
        }
        public int GetGroupsCount(DominatorAccountModel account)
        {
            bool isRunning = true;
            string countData = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl(FdConstants.GroupJoinedUrl, 5);
                    var pageSource = BrowserWindow.GetPageSource();
                    Task.Delay(2000, cancellationToken).Wait();
                    var countDataLst = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, FdConstants.GroupsCount1, ValueTypes.InnerText);
                    if (countDataLst.Count() == 0)
                        countDataLst = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, FdConstants.GroupsCount2, ValueTypes.InnerText);
                    var index = countDataLst.IndexOf(countDataLst.FirstOrDefault(x => x.Contains("All groups you've joined")));
                    if (index >= 0)
                        countData = countDataLst[index];
                    if (string.IsNullOrEmpty(countData) || !countData.Contains("All groups you've joined"))
                    {
                        var length = await BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.ClassName, FdConstants.GroupsCount3);
                        var ElementLength = !string.IsNullOrEmpty(length) ? Convert.ToInt32(length) : 0;
                        if (ElementLength > 0)
                            countData = await BrowserWindow.GetElementValueAsync(ActType.GetValue,
                            AttributeType.ClassName, FdConstants.GroupsCount3, valueType: ValueTypes.InnerText, clickIndex: ElementLength - 1);
                    }
                    countData = FdFunctions.GetIntegerOnlyString(countData);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000).Wait();

            return FdFunctions.IsIntegerOnly(countData) ? int.Parse(countData) : 0;
        }

        public bool DeletePost(DominatorAccountModel account, string postId, PostDeletionModel postDeletionModel)
        {
            bool isRunning = true;

            string postUrl = !postId.Contains(FdConstants.FbHomeUrl) ? $"{FdConstants.FbHomeUrl}{postId}" : postId;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl(postUrl, delayAfter: 10);
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Actions for this post", delayAfter: 5);
                    List<string> butns = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", ValueTypes.InnerHtml);

                    butns.Reverse();

                    int deleteButnIndex = butns.IndexOf(butns.FirstOrDefault(x => x.Contains("Delete post")));

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: deleteButnIndex, delayAfter: 5);

                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Delete", delayAfter: 5);

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000).Wait();

            return false;
        }

        public bool DisablePostComment(DominatorAccountModel account, FacebookPostDetails postDetails, PostOptions objPostOption)
        {
            bool isRunning = true;

            string postUrl = postDetails.PostUrl;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (BrowserWindow.CurrentUrl() != postUrl)
                        await BrowserWindow.GoToCustomUrl(postUrl, delayAfter: 6);
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Actions for this post", delayAfter: 5);
                    List<string> butns = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "menuitem", ValueTypes.InnerHtml);

                    butns.Reverse();

                    int disableCommentButnIndex = 0;

                    if (objPostOption == PostOptions.OwnWall)
                    {
                        disableCommentButnIndex = butns.IndexOf(butns.FirstOrDefault(x => x.Contains("Who can comment on your post")));

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: disableCommentButnIndex, delayAfter: 4);

                        List<string> radioButns = await BrowserWindow.GetListInnerHtml(ActType.ActByQuery, AttributeType.Role, "radio");

                        radioButns.Reverse();

                        int profileIndex = radioButns.IndexOf(radioButns.FirstOrDefault(x => x.Contains("Profiles and Pages you mention")));

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "radio", index: profileIndex, delayAfter: 4);

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Done", delayAfter: 4);
                    }
                    else
                    {
                        disableCommentButnIndex = butns.IndexOf(butns.FirstOrDefault(x => x.Contains("Turn off commenting")));

                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "menuitem", index: disableCommentButnIndex, delayAfter: 5);
                    }



                }
                catch (Exception)
                {
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000).Wait();

            return false;
        }

        public void SwitchToOwnPage(DominatorAccountModel account, string url, FanpageDetails fanpageDetails = null)
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var currentUrl = BrowserWindow.CurrentUrl();
                    if (!string.IsNullOrEmpty(fanpageDetails?.FanPageName))
                    {
                        int lengthv = 0;
                        if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{FdConstants.PageSwitchButtonScript}.length")).Result?.ToString(), out lengthv) && lengthv > 0)
                        {
                            await ClickOrScrollToViewAndClick($"{FdConstants.PageSwitchButtonScript}[0]", timetoWait: 5, isLoadSource: true);
                            await ClickOrScrollToViewAndClick($"{FdConstants.PageButtonScriptBYName(fanpageDetails?.FanPageName)}[0]", timetoWait: 10, isLoadSource: true);
                        }
                        if (BrowserWindow.CurrentUrl() != currentUrl)
                            await BrowserWindow.GoToCustomUrl(currentUrl, 6);
                    }
                    else if (!string.IsNullOrEmpty(url))
                    {
                        await BrowserWindow.GoToCustomUrl(url, 10);
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Switch", delayAfter: 7);
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Switch", index: 1, delayAfter: 10);
                        if (BrowserWindow.CurrentUrl() != currentUrl)
                            await BrowserWindow.GoToCustomUrl(currentUrl, 6);
                    }
                    else
                    {
                        await BrowserWindow.ExecuteScriptAsync($"{FdConstants.SwitchProfileButton}[0].click()", delayInSec: 8);
                        await BrowserWindow.ExecuteScriptAsync($"{FdConstants.UserSwitchButtonScript(fanpageDetails?.FanPageName)}[0].click()", delayInSec: 10);
                        if (BrowserWindow.CurrentUrl() != currentUrl)
                            await BrowserWindow.GoToCustomUrl(currentUrl, 6);
                    }
                    await LoadSource(5);
                    isActorChangedtoFanPage = true;
                }
                catch (Exception)
                {
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();
        }

        public void OpenNewWindowAndGoToUrl(DominatorAccountModel account, string url)
        {
            LoadPageSource(account, url);
        }

        public async Task<IResponseHandler> UpdateProfilePic(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            bool isRunning = true;

            var pageSource = string.Empty;

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    await BrowserWindow.BrowserActAsync(
                        ActType.ActByQuery,
                        AttributeType.AriaLabel,
                        "Update profile picture", delayAfter: 5);
                    await Task.Delay(1000, cancellationToken);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (!string.IsNullOrEmpty(editProfileModel.ProfilePicPath))
                    {
                        BrowserWindow.ChooseFileFromDialog(editProfileModel.ProfilePicPath);
                        await Task.Delay(1000, cancellationToken);
                        var xy = await BrowserWindow.GetXAndYAsync(
                            customScriptX: "document.querySelectorAll('[aria-label=\"Upload Photo\"]')[0].getBoundingClientRect().x",
                            customScriptY: "document.querySelectorAll('[aria-label=\"Upload Photo\"]')[0].getBoundingClientRect().y");
                        await BrowserWindow.MouseClickAsync(xy.Key + 5, xy.Value + 5, delayAfter: 5);
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    await BrowserWindow.BrowserActAsync(
                        ActType.ActByQuery,
                        AttributeType.AriaLabel,
                        "Save", delayAfter: 10);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, cancellationToken).Wait();

            return new UpdateProfilePicResponseHandler(new ResponseParameter() { Response = pageSource }, editProfileModel);
        }

        public void CheckGotReactions(DominatorAccountModel accountModel, ref JobProcessResult jobProcessResult)
        {
            var dialogPresent = BrowserWindow.GetElementValueAsync(ActType.CustomActByQueryType, AttributeType.Role, "dialog", clickIndex: 1, value: "InnerHtml").Result;
            if (!string.IsNullOrEmpty(dialogPresent))
                jobProcessResult.HasNoResult = false;
            else
                jobProcessResult.HasNoResult = true;
        }

        public bool TurnOffCommentsOrNotificationsForPost(DominatorAccountModel accountModel, string postUrl, CancellationTokenSource campaignCancellationToken, bool turnOffComment = false, bool turnOffNotification = false)
        {
            bool isRunning = true;
            bool IsTurnedOff = false;
            LoadPageSource(accountModel, postUrl);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "ariaLabel", "actions for this post")}[0]", timetoWait: 6);
                    if (turnOffComment)
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "menuitem", "textContent", "turn off commenting")}[0]", timetoWait: 5);
                    if (turnOffNotification)
                    {
                        if (turnOffComment)
                            await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "ariaLabel", "actions for this post")}[0]");
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "menuitem", "textContent", "turn off notifications for this post")}[0]", timetoWait: 5);
                        await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagAttributeValueButtonScript, "div", "role", "button", "ariaLabel", "actions for this post")}[0]", timetoWait: 6);
                        var pageSource = await BrowserWindow.GetPageSourceAsync();
                        IsTurnedOff = !pageSource.Contains("Turn off notifications for this post");
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            return IsTurnedOff;
        }



        public UpdateUserNameAndFullNameResponseHandler UpdateUserNameAndName(DominatorAccountModel accountModel, EditProfileModel editProfileModel, bool UpdateUserName = false, bool UpdateFullName = false, string ProfileId = "")
        {
            var IsRunning = true;
            var pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (UpdateFullName)
                    {
                        await ClickOrScrollToViewAndClick($"[...document.querySelectorAll('div>a[role=\"link\"],div>a[aria-label=\"Name\"]')].filter(x=>x.href?.includes(\"/name/\")||x.textContent===\"Name\")[0]", destinationUrl: $"https://accountscenter.facebook.com/profiles/{ProfileId}/name/", timetoWait: 7, isLoadSource: true);
                        await Task.Delay(1000, cancellationToken);
                        var namesArray = Regex.Split(editProfileModel.Fullname.Trim(), " ");
                        var firstName = namesArray.FirstOrDefault();
                        var lastName = namesArray.Count() > 1 ? namesArray?.LastOrDefault() : "";
                        var middleName = namesArray?.Count() > 2 ? namesArray[1] : "";
                        if (!string.IsNullOrEmpty(firstName))
                        {
                            await ClickOrScrollToViewAndClick(
                                "[...[document.querySelectorAll('input[type=\"text\"]')[0]],...[...document.querySelectorAll('label')].filter(label => label.textContent?.includes(\"First name\"))][0]");
                            BrowserWindow.SelectAllText();
                            await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                            await BrowserWindow.EnterCharsAsync(" " + firstName, typingDelay: 0.2, delayAtLast: 2);
                        }
                        if (!string.IsNullOrEmpty(middleName))
                        {
                            await ClickOrScrollToViewAndClick(
                                "[...[document.querySelectorAll('input[type=\"text\"]')[1]],...[...document.querySelectorAll('label')].filter(label => label.textContent?.includes(\"Middle name\"))][0]");
                            BrowserWindow.SelectAllText();
                            await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                            await BrowserWindow.EnterCharsAsync(" " + middleName, typingDelay: 0.2, delayAtLast: 2);
                        }
                        if (!string.IsNullOrEmpty(lastName))
                        {
                            await ClickOrScrollToViewAndClick(
                                "[...[document.querySelectorAll('input[type=\"text\"]')[2]],...[...document.querySelectorAll('label')].filter(label => label.textContent?.includes(\"Surname\"))][0]");
                            BrowserWindow.SelectAllText();
                            await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                            await BrowserWindow.EnterCharsAsync(" " + lastName, typingDelay: 0.2, delayAtLast: 2);
                        }
                        await ClickOrScrollToViewAndClick("[document.querySelectorAll('div[role=\"button\"]')[document.querySelectorAll('div[role=\"button\"]').length-1],...[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent?.includes(\"Review change\"))][0]", timetoWait: 10, isLoadSource: true);
                        await ClickOrScrollToViewAndClick("[document.querySelectorAll('div[role=\"button\"]')[document.querySelectorAll('div[role=\"button\"]').length-1],...[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent?.includes(\"Done\"))][0]", timetoWait: 7, isLoadSource: true);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                    }
                    if (UpdateUserName)
                    {
                        await ClickOrScrollToViewAndClick($"[...document.querySelectorAll('div>a[role=\"link\"],div>a[aria-label=\"Username\"]')].filter(x=>x.href?.includes(\"/username/\")||x.textContent===\"Username\")[0]", destinationUrl: $"https://accountscenter.facebook.com/profiles/{ProfileId}/username/", timetoWait: 7, isLoadSource: true);

                        await Task.Delay(1000, cancellationToken);
                        await ClickOrScrollToViewAndClick(
                            "[...[document.querySelectorAll('input[type=\"text\"]')[0]],...[...document.querySelectorAll('label')].filter(label => label.textContent?.includes(\"Username\"))][0]");

                        BrowserWindow.SelectAllText();
                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                        await BrowserWindow.EnterCharsAsync(" " + editProfileModel.Username?.Replace(" ", ""), typingDelay: 0.2, delayAtLast: 6);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        if (!pageSource.Contains("You're currently unable to choose a username") && !pageSource.Contains("Username is not available"))
                        {
                            await ClickOrScrollToViewAndClick("[document.querySelectorAll('div[role=\"button\"]')[document.querySelectorAll('div[role=\"button\"]').length-1],...[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent?.includes(\"Done\"))][0]", timetoWait: 7, isLoadSource: true);
                            await BrowserWindow.GoToCustomUrl($"https://www.facebook.com/{editProfileModel.Username?.Trim()?.Replace(" ", "")}", delayAfter: 5);
                            pageSource = await BrowserWindow.GetPageSourceAsync();
                        }
                    }

                }
                finally
                {
                    IsRunning = false;
                }
            });
            while (IsRunning) Task.Delay(2000).Wait();
            return new UpdateUserNameAndFullNameResponseHandler(new ResponseParameter { Response = pageSource }, editProfileModel, UpdateUserName, UpdateFullName);
        }

        public AdvancedProfileUpdateResponseHandler UpdateAdvancedProfileDetails(DominatorAccountModel accountModel, EditProfileModel editProfileModel, bool UpdateWebsite = false, bool UpdateBio = false, bool UpdateEmail = false, bool UpdateContact = false, bool UpdateGender = false, string ProfileId = "")
        {
            var IsRunning = true;
            var pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (UpdateWebsite)
                    {
                        var WebSiteClass = "x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x1ypdohk xt0psk2 xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz x1qq9wsj xt0b8zv";
                        var Nodes = HtmlParseUtility.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(pageSource, string.Empty, true, WebSiteClass, false);
                        if (Nodes is null || !Nodes.Any(x => x.Contains("Add a website")))
                            await BrowserWindow.ExecuteScriptAsync($"document.querySelector('div[aria-label=\"Add Website\"]').click();", delayInSec: 3);
                        else
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, WebSiteClass, index: 0, delayAfter: 5);
                        var XandY = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Website address\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Website address\"]')[0].getBoundingClientRect().y");
                        await BrowserWindow.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 4);
                        BrowserWindow.SelectAllText();
                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                        await BrowserWindow.EnterCharsAsync(" " + editProfileModel.ExternalUrl, delayAtLast: 2);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Save\"]').click();", delayInSec: 4);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        UpdateWebsite = pageSource.Contains(editProfileModel.ExternalUrl);
                    }
                    if (UpdateBio)
                    {

                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Edit profile\"]').click();", delayInSec: 5);
                        var addbiolength = Convert.ToInt32(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Add Bio"));
                        var editbiolength = Convert.ToInt32(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Edit Bio"));
                        if (addbiolength > 0)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.AriaLabel, "Add Bio", index: 0, delayAfter: 4);
                            var xyAddBio = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Add Bio\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Add Bio\"]')[0].getBoundingClientRect().y");
                            if (xyAddBio.Key != 0 && xyAddBio.Value != 0 && (xyAddBio.Key < 100 || xyAddBio.Value < 100))
                            {
                                await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2, 0, 180, delayAfter: 4, scrollCount: 1);
                                xyAddBio = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Add Bio\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Add Bio\"]')[0].getBoundingClientRect().y");
                            }
                            await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Add Bio\"]').click();", delayInSec: 5);
                        }
                        if (editbiolength > 0)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.AriaLabel, "Edit Bio", index: 0, delayAfter: 4);
                            var xyEditBio = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Edit Bio\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Edit Bio\"]')[0].getBoundingClientRect().y");
                            if (xyEditBio.Key != 0 && xyEditBio.Value != 0 && (xyEditBio.Key < 100 || xyEditBio.Value < 100))
                            {
                                await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2, 0, 180, delayAfter: 4, scrollCount: 1);
                                xyEditBio = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Edit Bio\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Edit Bio\"]')[0].getBoundingClientRect().y");
                            }
                            await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Edit Bio\"]').click();", delayInSec: 5);
                        }
                        var BioClass = "xzsf02u x1qlqyl8 xk50ysn xh8yej3 xha3pab xyc4ar7 x10lcxz4 xzt8jt4 xiighnt xviufn9 x1b3pals x10bruuh x2b8uid x1y1aw1k xn6708d xwib8y2 x1ye3gou xtt52l0";
                        var Location = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, BioClass);
                        if (Location.Key == 0 && Location.Value == 0)
                            Location = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Enter bio text\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Enter bio text\"]')[0].getBoundingClientRect().y");

                        await BrowserWindow.MouseClickAsync(Location.Key + 5, Location.Value + 5, delayAfter: 3);
                        BrowserWindow.SelectAllText();
                        await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                        await BrowserWindow.EnterCharsAsync(" " + editProfileModel.Bio, typingDelay: 0.1, delayAtLast: 3);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Save\"]').click();", delayInSec: 2);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        UpdateBio = pageSource.Contains(editProfileModel.Bio);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('a[aria-label=\"Edit Your About Info\"]').click();", delayInSec: 2);
                    }
                    if (UpdateGender)
                    {
                        var TabClass = "x87ps6o x1lku1pv x1a2a7pz xhk9q7s x1otrzb0 x1i1ezom x1o6z2jb x1lliihq x12nagc xz9dl7a x1iji9kk xsag5q8 x1sln4lm x1n2onr6";
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var Nodes = HtmlParseUtility.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(pageSource, string.Empty, true, TabClass);
                        var ClickIndex = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.Contains("Contact and basic info"))) : 3;
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, TabClass, index: ClickIndex, delayAfter: 3);
                        if (Nodes == null || Nodes.Count == 0)
                        {
                            var url = editProfileModel.Username.Contains("about_contact_and_basic_info") ? $"https://www.facebook.com/{editProfileModel.Username?.Trim()}"
                                : $"https://www.facebook.com/{editProfileModel.Username?.Trim()?.Replace(" ", "")}/about_contact_and_basic_info";
                            await BrowserWindow.GoToCustomUrl(url, delayAfter: 4);
                        }
                        await BrowserWindow.BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.AriaLabel, "Add your gender", index: 0, delayAfter: 4);
                        var xyEditBio = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[aria-label=\"Add your gender\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[aria-label=\"Add your gender\"]')[0].getBoundingClientRect().y");
                        if (xyEditBio.Key != 0 && xyEditBio.Value != 0 && (xyEditBio.Key < 100 || xyEditBio.Value < 100))
                            await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2, ScreenResolution.Value / 2, 0, 180, delayAfter: 4, scrollCount: 1);

                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Add your gender\"]').click();", delayInSec: 3);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"combobox\"]').click();", delayInSec: 3);
                        var GenderClass = "x9f619 x1ypdohk x78zum5 x1q0g3np x2lah0s xnqzcj9 x1gh759c xdj266r xat24cr x1344otq x1de53dj xz9dl7a xsag5q8 x1n2onr6 x16tdsg8 x1ja2u2z";
                        var Index = editProfileModel.IsFemaleChecked ? 0 : editProfileModel.IsMaleChecked ? 1 : 2;
                        var Gender = editProfileModel.IsFemaleChecked ? "Female" : editProfileModel.IsMaleChecked ? "Male" : "Nobinary";
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"combobox\"]').click();", delayInSec: 3);
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, GenderClass, index: Index);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Save\"]').click();", delayInSec: 2);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        UpdateGender = pageSource.Contains(Gender);
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                finally
                {
                    IsRunning = false;
                }
            }); while (IsRunning) Task.Delay(2000).Wait();
            return new AdvancedProfileUpdateResponseHandler(new ResponseParameter { Response = pageSource }, UpdateWebsite, UpdateBio, UpdateContact, UpdateEmail, UpdateGender);
        }
       
        public void SearchByfeed(DominatorAccountModel account, SearchKeywordType entity, string keyWord)
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await clickSeeMoreButton();
                    BrowserWindow.ClearResources();
                    await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Feeds")}[0]", timetoWait: 5, isLoadSource: true);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();

        }

        public async Task<FdGetAllMessagedUserResponseHandler> GetPendingUserChat(DominatorAccountModel dominatorAccount)
        {
            var isRunning = true;
            var PendingUserList = new List<string>();
            var messageDetails = new List<FdMessageDetails>();
            var listChatDetails = new List<ChatDetails>();
            var messageSenderDetailsList = new List<SenderDetails>();
            var hasMore = true;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl("https://www.facebook.com/messages/requests", delayAfter: 6);
                    var LastScrollCount = 0;
                    var ScrollCount = 0;
                    var ThreadID = string.Empty;
                ScrollAgain:
                    var ListUsers = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(await BrowserWindow.GetPageSourceAsync(), "a", "href", "/messages/requests/t/");
                    var _pageText = await BrowserWindow.PageText();
                    _pageText = Utilities.GetBetween(_pageText + "/", "You may know", "/");
                    if (ListUsers != null && ListUsers.Count != LastScrollCount)
                    {
                        foreach (var item in ListUsers)
                        {
                            try
                            {
                                ThreadID = Regex.Match(item.GetAttributeValue("href", ""), "[0-9]+")?.Value;
                                if (!string.IsNullOrEmpty(ThreadID) && !PendingUserList.Any(x => x.Contains(ThreadID)))
                                    PendingUserList.Add($"https://www.facebook.com/messages/requests/t/{ThreadID}/");
                            }
                            catch { }
                        }
                    }
                    while (ScrollCount++ <= 5)
                    {
                        if (LastScrollCount == ListUsers.Count)
                        {
                            hasMore = false;
                            break;
                        }
                        LastScrollCount = ListUsers.Count;
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('a[href=\"/messages/requests/t/{ThreadID}/\"]')[0].scrollIntoViewIfNeeded();", delayInSec: 3);
                        goto ScrollAgain;
                    }
                    PendingUserList = PendingUserList.Distinct().ToList();
                    PendingUserList.Reverse();
                    if (PendingUserList.Count > 0)
                    {
                        foreach (var item in PendingUserList)
                        {
                            try
                            {
                                BrowserWindow.ClearResources();
                                var ThreadId = Utilities.GetBetween(item, "/t/", "/");
                                await BrowserWindow.ExecuteScriptAsync($"document.querySelector('a[href=\"{item.Replace("https://www.facebook.com", "")}\"]').click();", delayInSec: 8);
                                var NameString = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"presentation\"]').innerText", delayInSec: 3);
                                var message = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[class=\"x78zum5\"]>div>div[role=\"presentation\"]>div[role=\"presentation\"]').innerText", delayInSec: 3);
                                var FamilyName = Utilities.GetBetween("##" + NameString?.Result?.ToString(), "##", "\n");
                                if (!_pageText.Contains(FamilyName)) break;
                                messageDetails.Add(new FdMessageDetails
                                {
                                    MessageSenderId = ThreadId,
                                    MessageSenderName = FamilyName,
                                    ProfileUrl = $"https://www.facebook.com/profile.php?id={ThreadId}",
                                    Message = message?.Result?.ToString()
                                });
                            }
                            catch { }
                        }
                    }
                    isRunning = false;
                }
                catch
                {
                    isRunning = false;
                }
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
            return new FdGetAllMessagedUserResponseHandler(new ResponseParameter() { Response = "" }, true, hasMore)
            {
                ObjFdScraperResponseParameters = new FdScraperResponseParameters()
                {
                    MessageDetailsList = messageDetails,
                    MessageSenderDetailsList = messageSenderDetailsList,
                    ListSenderDetails = listChatDetails
                }
            };
        }
        public async Task<bool> ClickOrScrollToViewAndClick(string script, string destinationUrl = "", int timetoWait = 3, bool isLoadSource = false, int addX = 25, int addY = 11)
        {
            var isDone = false;
            try
            {
                var elementLocation = await BrowserWindow.GetXAndYAsync(customScriptX: $"{script}.getBoundingClientRect().x", customScriptY: $"{script}.getBoundingClientRect().y");

                if (elementLocation.Key != 0 && elementLocation.Value != 0 && (elementLocation.Value < 80 || elementLocation.Value > ScreenResolution.Value - 200))
                {
                    await BrowserWindow.ExecuteScriptAsync($"{script}.scrollIntoViewIfNeeded()", delayInSec: 4);
                    elementLocation = await BrowserWindow.GetXAndYAsync(customScriptX: $"{script}.getBoundingClientRect().x", customScriptY: $"{script}.getBoundingClientRect().y");

                }
                if (elementLocation.Value > 80 && elementLocation.Value < ScreenResolution.Value - 200)
                {
                    await BrowserWindow.MouseClickAsync(elementLocation.Key + addX, elementLocation.Value + addY, delayAfter: timetoWait);
                    isDone = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(destinationUrl))
                        isDone = (await BrowserWindow.ExecuteScriptAsync($"{script}.click()", timetoWait)).Success;
                    else
                    {
                        await BrowserWindow.GoToCustomUrl(destinationUrl, timetoWait);
                        isDone = true;
                    }

                }
                if (isLoadSource && ((elementLocation.Key != 0 && elementLocation.Value != 0) || (!string.IsNullOrEmpty(destinationUrl))))
                    await LoadSource();
            }
            catch (Exception e) { e.DebugLog(); }
            return isDone;
        }
        public async Task clickSeeMoreButton()
        {
            try
            {
                if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>div", "button", "textContent", "See more")}.length"))?.Result?.ToString(), out int seemore) && seemore > 0)
                    await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>div", "button", "textContent", "See more")}[0].click()", delayInSec: 5);
            }
            catch (Exception e) { e.DebugLog(); }
        }

        public FdSeachGroupResponseHandler GetGroupDetails(DominatorAccountModel account, GroupDetails facebookGroup, bool isNewWindow = true, bool isCloseBrowser = false)
        {
            throw new NotImplementedException();
        }

        public void SearchByBirthDayEvent(DominatorAccountModel account, string url)
        {
            LoadPageSource(account, url);
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    BrowserWindow.ClearResources();
                    BrowserWindow.SetResourceLoadInstance();
                    await ClickOrScrollToViewAndClick($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "href", "/events/birthdays/")}[0]", timetoWait: 5, isLoadSource: true);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(1000, cancellationToken).Wait();
        }
    }
}

