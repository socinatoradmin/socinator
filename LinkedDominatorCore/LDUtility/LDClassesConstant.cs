using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedDominatorCore.LDUtility
{
    public class LDClassesConstant
    {
        public static class GrowConnection
        {
            #region Connection Request.
            #endregion
            #region Accept connection request.
            #endregion
            #region Remove connection.
            #endregion
            #region WithDraw connection request.
            #endregion
            #region Export Connection.
            #endregion
            #region Follow Page.
            #endregion
            #region Send Page Invitation.
            public static string AlreadySendPageInvitationClass { get; set; } = "invitee-picker-connections-result-item__status t-14 t-black--light t-bold mt2";
            public static string InviteButtonClass { get; set; } = "artdeco-button artdeco-button--2 artdeco-button--primary ember-view org-top-card";
            public static string MoreOptionClass { get; set; } = "artdeco-dropdown artdeco-dropdown--placement-bottom artdeco-dropdown--justification-left ember-view";
            #endregion
            #region Send Group invitation.
            public static string SelectUserClass => "flex-1 inline-block align-self-center pl2 mr5";
            public static string SearchUserClass => "artdeco-typeahead__input ";
            public static string SuccessCaptionMessageClass => "artdeco-toast-item__message";
            #endregion
            #region Block User.
            public static string UserFullNameClass => "text-heading-xlarge inline t-24 v-align-middle break-words";
            #endregion
        }
        public static class Messenger
        {
            #region Broadcast Message.
            public static string MessageButtonClass { get; set; } = "artdeco-button artdeco-button--2 artdeco-button--primary ember-view";
            public static string SendMessageButtonClass { get; set; } = "msg-form__send-button artdeco-button artdeco-button--1";
            public static string CloseConversationWindow { get; set; } = "close_conversation_window";
            public static string ConnectButtonClass { get; set; } = "pv-s-profile-actions pv-s-profile-actions--connect ml2 artdeco-button artdeco-button--2 artdeco-button--primary ember-view";
            #endregion
            #region Auto Reply to new message.
            public static string NewMessageNotificationClass { get; set; } = "artdeco-notification-badge ember-view msg-conversation-card__unread-count artdeco-notification-badge--new text-align-center";
            public static string ChatUserListClass { get; set; } = "ember-view  scaffold-layout__list-item msg-conversation-listitem msg-conversations-container__convo-item msg-conversations-container__pillar ";//msg-conversation-listitem__link msg-conversations-container__convo-item-link
            public static string AcceptMessageRequestClass { get; set; } = "msg-pending-message-request-footer-presenter__accept-button";
            #endregion
            #region Send Message To new connections.
            #endregion
            #region Send Greetings to connections.
            #endregion
            #region Delete conversation.
            #endregion
        }
        public static class Engage
        {
            #region Like.
            public static string PostLikeClass { get; set; } = "social-actions-button react-button__trigger";
            #endregion
            #region Comment.
            public static string CommentTextAreaClass { get; set; } = "editor-content ql-container";
            public static string SubmitCommentClass { get; set; } = "comments-comment-box__submit-button";
            #endregion
            #region Share.
            public static string PostShareClass { get; set; } = "social-reshare-button flex-wrap";
            public static string RepostPostClass { get; set; } = "artdeco-dropdown__item artdeco-dropdown__item--is-dropdown ember-view social-reshare-button__sharing-as-is-dropdown-item";
            #endregion
        }
        public static class Scrapper
        {
            #region UserScrapper.
            #endregion
            #region Attachment Message Scrapper.
            public static string CloseConversationWindowClass1 { get; set; } = "msg-overlay-bubble-header__control js-msg-close artdeco-button artdeco-button--circle artdeco-button--inverse artdeco-button--1 artdeco-button--tertiary ember-view";
            public static string CloseConversationWindowClass2 { get; set; } = "msg-overlay-bubble-header__control artdeco-button artdeco-button--circle artdeco-button--muted artdeco-button--1 artdeco-button--tertiary ember-view";
            #endregion
        }
        public static class SocioPublisherPost
        {
            public static string ClassForPickImageOrVideoFromDialog { get; set; } = "share-promoted-detour-button__compact share-promoted-detour-button__card-promoted-";
            public static string ClassForPickImageOrVideoFromDialog1 { get; set; } = "share-promoted-detour-button__icon-container";
            public static string ClassForPickDocumentFromDialog { get; set; } = "share-promoted-detour-button__compact share-promoted-detour-button__card-neutral-";
            public static string ClassForPickDocumentFromDialog1 { get; set; } = "cloud-filepicker-visually-hidden";
            public static string ClassForMediaUploadPanel1 { get; set; } = "artdeco-button artdeco-button--muted artdeco-button--4 artdeco-button--tertiary share-box-feed-entry__trigger--v2";
            public static string ClassForMediaUploadPanel2 { get; set; } = "artdeco-button artdeco-button--muted artdeco-button--4 artdeco-button--tertiary ember-view share-box-feed-entry__trigger";
            public static string ClassForMediaUploadPanelOnGroup { get; set; } = "artdeco-button artdeco-button--muted artdeco-button--4 artdeco-button--tertiary ember-view share-box-feed-entry__trigger";
            public static string ClassForTitleOfDocument { get; set; } = "ember-text-field ember-view document-title-form__title-input";
            public static string ClassForVideoUploadProcessingLabel { get; set; } = "share-status-container__processing-text t-14 t-black--light t-normal";

        }
        public static class ScriptConstant
        {
            public static string ScriptWithQuerySelectorToGetXandY { get; set; } = "document.querySelector(\'{0}[{1}=\"{2}\"]\').getBoundingClientRect().{3}";
            public static string ScriptWithQuerySelectorAllToGetXandY { get; set; } = "document.querySelectorAll(\'{0}[{1}=\"{2}\"]\')[{3}].getBoundingClientRect().{4}";
            public static string ScriptWithQuerySelectorAllToFilter { get; set; } = "[...document.querySelectorAll(\'{0}[{1}=\"{2}\"]\')].filter(x=>x.textContent.trim()===\"{3}\")[{4}].{5}";
            public static string ScriptWithQuerySelectorAllToFilterNormal { get; set; } = "[...document.querySelectorAll(\'{0}\')].filter(x=>x.textContent.trim()===\"{1}\")[{2}].{3}";
            public static string ScriptWithQuerySelectorToClick { get; set; } = "document.querySelector(\'{0}[{1}=\"{2}\"]\').click();";
            public static string ScriptWithQuerySelectorAllToClick { get; set; } = "document.querySelectorAll(\'{0}[{1}=\"{2}\"]\')[{3}].click();";
            public static string ScriptWithQuerySelector { get; set; } = "document.querySelector(\'{0}[{1}]\')";
            public static string ScriptWithQuerySelectorAll { get; set; } = "document.querySelectorAll(\'{0}[{1}]\')[{2}]";
            public static string ScriptGetElementByIdToClick { get; set; } = "document.getElementById(\'{0}\').click();";
            public static string ScriptGetElementsByClassNameToClick { get; set; } = "document.getElementsByClassName(\'{0}\')[{1}].click();";
            public static string ScriptGetElementsByClassNameToGetXY { get; set; } = "document.getElementsByClassName(\'{0}\')[{1}].getBoundingClientRect().{2}";
            public static string ScriptGetElementByIdToGetXY { get; set; } = "document.getElementById(\'{0}\').getBoundingClientRect().{1}";
            public static string ScrollWindowByXXPixel { get; set; } = "window.scrollBy({0},{1})";
            public static string ScrollWindowToXXPixel { get; set; } = "window.scrollTo({0},{1})";
            public static string ScrollWindow { get; set; } = "window.scroll({0},{1})";
        }
    }
}
