using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.MessageResponseHandler
{
    public class FdGetAllMessagedUserResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string EntityId { get; set; }


        public bool HasMoreResults { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public string CursorId { get; set; }
        public FdGetAllMessagedUserResponseHandler(IResponseParameter responseParameter, bool isStatus, bool hasMoreResult) : base(responseParameter)
        { Status = isStatus; HasMoreResults = hasMoreResult; }
        public FdGetAllMessagedUserResponseHandler(IResponseParameter responseParameter
            , DominatorAccountModel account, MessageType messageType, bool hasMoreResult = false) : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.MessageDetailsList = new List<FdMessageDetails>();
            ObjFdScraperResponseParameters.ListChatDetails = new List<ChatDetails>();
            ObjFdScraperResponseParameters.MessageSenderDetailsList = new List<SenderDetails>();
            if (messageType == MessageType.Pending)
            {
                GetMessageDetails(responseParameter, account, messageType);
                ObjFdScraperResponseParameters.MessageDetailsList.RemoveAll(x => x.UnreadCount == 0);
                Status = ObjFdScraperResponseParameters.MessageDetailsList.Count > 0;
            }
            else if (messageType == MessageType.Inbox)
            {
                GetMessageDetails(responseParameter, account, messageType);
                Status = ObjFdScraperResponseParameters.MessageDetailsList.Count > 0;
            }
            else
            {
                GetMessageDetails(responseParameter, account, messageType);
                Status = ObjFdScraperResponseParameters.MessageSenderDetailsList.Count > 0
                    || ObjFdScraperResponseParameters.MessageDetailsList.Count > 0;
            }
            HasMoreResults = hasMoreResult;
        }
        public FdGetAllMessagedUserResponseHandler(IResponseParameter responseParameter, List<string> listMessageData
        , string userId, string userFullName, MessageType messageType = MessageType.Pending, string accountId = "",
        bool hasMoreResults = true) : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            if (messageType == MessageType.Pending)
            {
                ObjFdScraperResponseParameters.MessageDetailsList = new List<FdMessageDetails>();
                ObjFdScraperResponseParameters.ListChatDetails = new List<ChatDetails>();
                GetMessageList(listMessageData, userId, userFullName);
                ObjFdScraperResponseParameters.MessageDetailsList.RemoveAll(x => x.UnreadCount == 0);
                Status = ObjFdScraperResponseParameters.MessageDetailsList.Count > 0;
            }
            else if (messageType == MessageType.Inbox)
            {
                ObjFdScraperResponseParameters.MessageDetailsList = new List<FdMessageDetails>();
                ObjFdScraperResponseParameters.ListChatDetails = new List<ChatDetails>();
                GetMessageList(listMessageData, userId, userFullName);
                Status = ObjFdScraperResponseParameters.MessageDetailsList.Count > 0;
            }
            else
            {
                ObjFdScraperResponseParameters.MessageSenderDetailsList = new List<SenderDetails>();
                ObjFdScraperResponseParameters.MessageDetailsList = new List<FdMessageDetails>();
                GetSendersListNewUI(listMessageData, userId, accountId);
                //GetSendersList(listMessageData, userId, accountId);
                Status = ObjFdScraperResponseParameters.MessageSenderDetailsList.Count > 0
                    || ObjFdScraperResponseParameters.MessageDetailsList.Count > 0;
            }

            HasMoreResults = hasMoreResults;
        }
        private void GetMessageDetails(IResponseParameter response, DominatorAccountModel account, MessageType messageType, List<string> PendingUsers = null)
        {

            try
            {
                var jsonHandler = new JsonHandler(Utilities.ValidateJsonString(response.Response));
                #region Get CursorId
                var cursorToken = jsonHandler.GetElementValue("step", 1, 1, 3);
                if (cursorToken.Contains("executeFirstBlockForSyncTransaction"))
                    CursorId = jsonHandler.GetElementValue("step", 1, 1, 3, 5);
                #endregion

                #region Get Messages.
                var messageToken = jsonHandler.GetJToken("step", 1, 2, 2);

                foreach (JToken token in messageToken)
                {
                    if (token.ToString().Contains("deleteThenInsertThread"))
                    {
                        try
                        {
                            string message = jsonHandler.GetJTokenValue(token, 1, 4);
                            string senderImage = jsonHandler.GetJTokenValue(token, 1, 6);
                            string username = jsonHandler.GetJTokenValue(token, 1, 38);
                            username = Utilities.GetBetween("/" + username, "/", " u00b7");
                            string threadId = jsonHandler.GetJTokenValue(token, 1, 9, 1);
                            if (string.IsNullOrEmpty(username))
                            {
                                var userInfoToken = messageToken.FirstOrDefault(x => x.ToString().Contains("verifyContactRowExists") && (x.ToString().Contains(senderImage) || x.ToString().Contains(threadId)));
                                if (userInfoToken != null && userInfoToken.Count() != 0)
                                    username = jsonHandler.GetJTokenValue(userInfoToken, 1, 5);
                            }
                            string timestamp = jsonHandler.GetJTokenValue(token, 1, 2, 1);

                            int unreadCount = messageType != MessageType.Pending && (message.StartsWith("You:") || message.StartsWith("You sent ") || message.StartsWith("You can now call each other and see information")
                                || message.ToLower().StartsWith("say hi to your new facebook friend") || message.ToLower().StartsWith("you are now connected on messenger") || message.ToLower().StartsWith("say hi to your new facebook friend")
                                || message.ToLower().Contains("of friendship on facebook")) ? 0 : 1;
                            if (threadId == account.AccountBaseModel.UserId || username == "Facebook user" || ObjFdScraperResponseParameters.MessageDetailsList.Any(x => x.MessageSenderId == threadId))
                                continue;
                            if (messageType != MessageType.Pending && messageType != MessageType.Inbox)
                                ObjFdScraperResponseParameters.MessageSenderDetailsList.Add(new SenderDetails()
                                {
                                    SenderName = username,
                                    LastMesseges = message,
                                    ThreadId = threadId,
                                    SenderImage = senderImage,
                                    LastMessegedate = timestamp
                                });
                            else
                                ObjFdScraperResponseParameters.ListChatDetails.Add(new ChatDetails()
                                {
                                    Sender = username,
                                    Messeges = message,
                                    SenderId = threadId,
                                    ListMediaUrls = new System.Collections.ObjectModel.ObservableCollection<string>() { senderImage },
                                    Time = timestamp
                                });

                            ObjFdScraperResponseParameters.MessageDetailsList.Add(new FdMessageDetails()
                            {
                                MessageSenderName = username,
                                MessageSenderId = threadId,
                                MessageReceiverId = account.AccountBaseModel.UserId,
                                MessageReceiverName = account.AccountBaseModel.UserFullName,
                                Message = message,
                                MesageType = messageType,
                                UnreadCount = unreadCount,
                                ProfileUrl = FdConstants.FbHomeUrl + threadId
                            });
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else
                        continue;
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        private void GetSendersListNewUI(List<string> messageResponseList, string userId,
            string accountId)
        {
            messageResponseList.Reverse();

            foreach (var messageDetail in messageResponseList)
            {
                try
                {

                    var messageDATA = FdFunctions.GetPrtialDecodedResponse(messageDetail);
                    SenderDetails senderDetails = new SenderDetails();
                    FdMessageDetails objmessageDetails = new FdMessageDetails();
                    senderDetails.AccountId = accountId;
                    senderDetails.SenderId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(messageDATA, "href=\"(.*?)\""));

                    using (FdHtmlParseUtility utility = new FdHtmlParseUtility())
                    {
                        senderDetails.LastMessageOwnerId = string.Empty;
                        objmessageDetails.MessageSenderId = senderDetails.SenderId;

                        if (string.IsNullOrEmpty(senderDetails.SenderId) || senderDetails.SenderId == "0"
                            || senderDetails.SenderId == userId)
                            continue;

                        senderDetails.LastMesseges = utility.GetListInnerTextFromPartialTagNameContains(messageDATA, "span", "class", "g0qnabr5 ojkyduve").LastOrDefault();

                        objmessageDetails.Message = senderDetails.LastMesseges;

                        senderDetails.SenderName = utility.GetInnerTextFromPartialTagNameContains(messageDATA, "span", "class", "l9j0dhe7 ltmttdrg g0qnabr5");

                        objmessageDetails.MessageSenderName = senderDetails.SenderName;

                        var imageDATA = utility.GetInnerHtmlFromPartialTagName(messageDATA, "div", "class", "q9uorilb l9j0dhe7 pzggbiyp du4w35lb");
                        senderDetails.SenderImage = !string.IsNullOrEmpty(imageDATA) ? FdRegexUtility.FirstMatchExtractor(imageDATA, "href=\"(.*?)\"") : string.Empty;

                        var dateTimeString = utility.GetListInnerTextFromPartialTagNameContains(messageDATA, "span", "class", "g0qnabr5").LastOrDefault();
                        senderDetails.LastMessegeDateTime = FdFunctions.GetDateTimeFromSymbols(dateTimeString);
                        objmessageDetails.InteractionDate = senderDetails.LastMessegeDateTime;
                    }

                    senderDetails.ThreadId = senderDetails.SenderId;
                    objmessageDetails.ProfileUrl = $"{FdConstants.FbHomeUrl}{senderDetails.SenderId}";


                    if (ObjFdScraperResponseParameters.MessageSenderDetailsList.FirstOrDefault(x => x.ThreadId == senderDetails.ThreadId) == null)
                        ObjFdScraperResponseParameters.MessageSenderDetailsList.Add(senderDetails);

                    if (ObjFdScraperResponseParameters.MessageDetailsList.FirstOrDefault(x => x.MessageSenderId == objmessageDetails.MessageSenderId) == null)
                        ObjFdScraperResponseParameters.MessageDetailsList.Add(objmessageDetails);


                }
                catch (Exception)
                {

                }
            }
        }

        private void GetMessageList(List<string> listMessageData, string userId, string UserFullName)
        {
            foreach (var messageData in listMessageData)
            {
                try
                {
                    FdMessageDetails objMessageDetail = new FdMessageDetails();
                    var dateTimeString = string.Empty;
                    objMessageDetail.MessageSenderId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(messageData, "href=\"(.*?)\""));
                    using (FdHtmlParseUtility objUtility = new FdHtmlParseUtility())
                    {
                        try
                        {
                            objMessageDetail.MessageSenderName = objUtility.GetInnerTextFromPartialTagNameContains(messageData, "span", "class", "x1lliihq x6ikm8r x10wlt62 x1n2onr6 xlyipyv xuxw1ft");
                            objMessageDetail.Message = objUtility.GetListInnerTextFromPartialTagNameContains(messageData, "span", "class", "x1lliihq x6ikm8r x10wlt62 x1n2onr6 xlyipyv xuxw1ft x1j85h84").FirstOrDefault();
                            dateTimeString = objUtility.GetListInnerTextFromPartialTagNameContains(messageData, "span", "class", "xuxw1ft").LastOrDefault();
                        }
                        catch (Exception)
                        { }
                    }

                    if (objMessageDetail.MessageSenderId == userId
                        || objMessageDetail.Message.ToLower().StartsWith("say hi to your new facebook friend")
                        || objMessageDetail.Message.ToLower().StartsWith("you are now connected on messenger")
                        || objMessageDetail.Message.ToLower().Contains("of friendship on facebook") || objMessageDetail.MessageSenderName == "Facebook user")
                        continue;

                    objMessageDetail.UnreadCount = messageData.ToLower().Contains("mark as read") || (messageData.ToLower().Contains("role=\"button\"") && !messageData.ToLower().Contains("aria-label=\"seen by")) ? 1 : 0;
                    objMessageDetail.MessageReceiverId = userId;
                    objMessageDetail.MessageReceiverName = UserFullName;
                    objMessageDetail.OtherUserFbid = objMessageDetail.MessageSenderId;
                    objMessageDetail.ProfileUrl = $"{FdConstants.FbHomeUrl}{objMessageDetail.MessageSenderId}";
                    objMessageDetail.InteractionDate = FdFunctions.GetDateTimeFromSymbols(dateTimeString);

                    if (string.IsNullOrEmpty(objMessageDetail.MessageSenderId))
                        continue;
                    if (!ObjFdScraperResponseParameters.MessageDetailsList.Any
                            (x => x.MessageReceiverId == objMessageDetail.MessageSenderId))
                        ObjFdScraperResponseParameters.MessageDetailsList.Add(objMessageDetail);

                    var details = new ChatDetails()
                    {
                        SenderId = objMessageDetail.MessageSenderId,
                        Sender = objMessageDetail.MessageSenderName,
                        Messeges = objMessageDetail.Message,
                        Time = objMessageDetail.InteractionDate.ToString()
                    };

                    ObjFdScraperResponseParameters.ListChatDetails.Add(details);


                }
                catch (Exception)
                {

                }

            }
        }
    }
}
