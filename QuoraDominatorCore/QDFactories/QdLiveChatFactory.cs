using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using Unity;

namespace QuoraDominatorCore.QDFactories
{
    public class QdLiveChatFactory : ChatFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly QuoraFunct quoraFuncts;
        private IQuoraBrowserManager _browser { get; }


        public QdLiveChatFactory(IAccountScopeFactory accountScopeFactory,
            IAccountsFileManager accountsFile)
        {
            _accountScopeFactory = accountScopeFactory;
            _accountsFileManager = accountsFile;
            quoraFuncts = InstanceProvider.GetInstance<QuoraFunct>();
            _browser = InstanceProvider.GetInstance<QdBrowserManager>();
        }

        public override void UpdateFriendList(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                if (liveChatModel.DominatorAccountModel == null)
                {
                    var allAccounts = _accountsFileManager.GetAll().First();
                    liveChatModel.DominatorAccountModel = allAccounts;
                }

                var loginProcess = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQdLogInProcess>();

                var httpHelper = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQdHttpHelper>();

                cancellation.ThrowIfCancellationRequested();
                loginProcess.LoginUsingGlobusHttpQuora(liveChatModel.DominatorAccountModel, cancellation);
                cancellation.ThrowIfCancellationRequested();

                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        liveChatModel.LstChat = new ObservableCollection<ChatDetails>();
                    });
                    return;
                }

                var quoraFunct = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQuoraFunctions>();

                var req = httpHelper.GetRequestParameter();
                req.Cookies = liveChatModel.DominatorAccountModel.Cookies;
                httpHelper.SetRequestParameter(req);

                quoraFuncts._httpHelper.GetRequestParameter();

                cancellation.ThrowIfCancellationRequested();
                var response = httpHelper.GetRequest("https://www.quora.com/messages");
                cancellation.ThrowIfCancellationRequested();
                var basePostData = new BasePostData(response.Response);
                var reqParams = httpHelper.GetRequestParameter();

                reqParams.Headers["Host"] = "www.quora.com";
                reqParams.KeepAlive = true;
                reqParams.Headers["sec-ch-ua"] = "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"96\", \"Google Chrome\";v=\"96\"";
                reqParams.Headers["sec-ch-ua-mobile"] = "?0";
                reqParams.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                reqParams.Headers["Content-Type"] = "application/json";
                reqParams.Headers["Quora-Revision"] = basePostData.Revision;
                reqParams.Headers["Quora-Broadcast-Id"] = basePostData.Broadcast_Id;
                reqParams.Headers["Quora-Formkey"] = basePostData.FormKey;
                reqParams.Headers["Quora-Canary-Revision"] = "false";
                reqParams.Headers["Quora-Window-Id"] = basePostData.WindowId;
                reqParams.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                reqParams.Accept = "*/*";
                reqParams.Headers["Origin"] = "https://www.quora.com";
                reqParams.Headers["Sec-Fetch-Site"] = "same-origin";
                reqParams.Headers["Sec-Fetch-Mode"] = "cors";
                reqParams.Headers["Sec-Fetch-Dest"] = "empty";

                reqParams.Headers["Accept-Language"] = "en-US,en;q=0.9";
                reqParams.Cookies = liveChatModel.DominatorAccountModel.Cookies;
                var postData = string.Empty;
                var threadId = string.Empty;
                ScrapeMessageResponseHandler ChatDetailsResponse = null;
            ApplyPagination:
                ChatDetailsResponse = quoraFunct.ReadMessage(liveChatModel.DominatorAccountModel, basePostData, ChatDetailsResponse is null ? "-1" : ChatDetailsResponse.PaginationCount.ToString(), false, true);
                var IsRunning = true;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await SetFriendDetail(liveChatModel, ChatDetailsResponse, cancellation);
                    IsRunning = false;
                });
                while(IsRunning);Thread.Sleep(2000);
                while (ChatDetailsResponse != null && ChatDetailsResponse.HasMoreResult)
                    goto ApplyPagination;
                #region Old Code To Get Chat Details.
                //var msgUrl = "https://www.quora.com/graphql/gql_para_POST?q=MessageInboxThreadQuery ";
                //ScrapeMessageResponseHandler scrapeMessageResponseHandler;


                //foreach (var eachId in basePostData.ThreadIds)
                //{
                //    threadId = Utilities.GetBetween(eachId, "thread/", "\\");
                //    reqParams.Referer = ": https://www.quora.com" + eachId;

                //    postData = "{\"queryName\":\"MessageInboxThreadQuery\",\"extensions\":{\"hash\":\"f51de9c25086dbfd6f56f8c05c18bd2fd8b099444a98348d7581537e74d2248f\"},\"variables\":{\"threadId\":" + threadId + "}}";
                //    var msgResponse = httpHelper.PostRequest(msgUrl, postData);

                //    scrapeMessageResponseHandler = new ScrapeMessageResponseHandler(msgResponse);

                //    SetFriendDetail(liveChatModel, scrapeMessageResponseHandler, cancellation);

                //}
                #endregion
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task SetFriendDetail(LiveChatModel liveChatModel,
            ScrapeMessageResponseHandler scrapeMessageResponseHandler, CancellationToken cancellation)
        {
            if (scrapeMessageResponseHandler.LstMessageDetails != null)
                await Application.Current.Dispatcher.Invoke(async () =>
                {
                    foreach (var messageDetail in scrapeMessageResponseHandler.LstMessageDetails)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        try
                        {
                            var friendDetail = new SenderDetails
                            {
                                SenderImage = messageDetail.UserProfilePic,
                                LastMesseges = messageDetail.LastMessage,
                                SenderName = messageDetail.UserFullName,
                                ThreadId = messageDetail.MessageId,
                                LastMessegedate = messageDetail.MessageDateTime,
                                AccountId = liveChatModel.DominatorAccountModel.AccountId,
                                SenderId = messageDetail.MessageId
                            };
                            SaveFriendDetails(liveChatModel, friendDetail, cancellation);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                });
        }

        public override async void UpdateCurrentChat(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                if (liveChatModel == null) return;
                var loginProcess = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQdLogInProcess>();

                cancellation.ThrowIfCancellationRequested();
                await loginProcess.CheckLoginAsync(liveChatModel.DominatorAccountModel, cancellation);

                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn) return;

                var quoraFunct = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQuoraFunctions>();
                var httpHelper = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQdHttpHelper>();
                var response = httpHelper.GetRequest("https://www.quora.com/messages/");
                var basePostData = new BasePostData(response.Response);
                var currentFriend =
                    liveChatModel.LstSender?.FirstOrDefault(x =>
                        x.SenderName == liveChatModel.SenderDetails.SenderName);
                var messageId = currentFriend?.ThreadId;
                var readResponseHandler = quoraFunct.ReadMessage(liveChatModel.DominatorAccountModel, basePostData, messageId,true,false);
                quoraFunct.MarkMessageAsRead(messageId);
                cancellation.ThrowIfCancellationRequested();
                if (readResponseHandler.LstChatDetails.Count != 0)
                {
                    UpdateCurrentChat(liveChatModel, readResponseHandler, cancellation);
                }
                liveChatModel.SenderDetails.LastMesseges = readResponseHandler.LstChatDetails.LastOrDefault()?.Messeges;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateCurrentChat(LiveChatModel liveChatModel,
            ScrapeMessageResponseHandler scrapeMessageResponseHandler, CancellationToken cancellation)
        {
            if (scrapeMessageResponseHandler.LstChatDetails != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var eachChatDetail in scrapeMessageResponseHandler.LstChatDetails)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        try
                        {
                            var chatDetail = new ChatDetails
                            {
                                Time = eachChatDetail.Time,
                                Messeges = eachChatDetail.Messeges,
                                IsRecieved = eachChatDetail.IsRecieved,
                                MessegesId = eachChatDetail.MessegesId,
                                Sender = eachChatDetail.Sender,
                                SenderId = eachChatDetail.SenderId,
                                MessegeType = eachChatDetail.MessegeType
                            };
                            if (chatDetail.SenderId == liveChatModel.DominatorAccountModel.AccountBaseModel.UserId)
                                chatDetail.Type = ChatMessage.Sent.ToString();
                            else
                                chatDetail.Type = ChatMessage.Received.ToString();

                            chatDetail.SenderId = liveChatModel.SenderDetails.SenderId;
                            SaveChatDetails(liveChatModel, chatDetail, cancellation);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                });
        }

        public override async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string message,
            List<string> list, ChatMessageType messageType, CancellationToken cancellation)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn) return false;
                var loginProcess = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQdLogInProcess>();
                await loginProcess.CheckLoginAsync(liveChatModel.DominatorAccountModel, cancellation);
                var httpHelper = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQdHttpHelper>();
                var quoraFunct = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<IQuoraFunctions>();
                var currentFriend = liveChatModel.LstSender?.FirstOrDefault(x => x.SenderName == liveChatModel.SenderDetails.SenderName);
                var threadId = currentFriend?.ThreadId;
                var response = httpHelper.GetRequest("https://www.quora.com/messages/");
                var basePostData = new BasePostData(response.Response);
                var readResponseHandler = quoraFunct.ReadMessage(liveChatModel.DominatorAccountModel, basePostData, threadId,true,false);
                cancellation.ThrowIfCancellationRequested();
                try
                {

                    cancellation.ThrowIfCancellationRequested();
                    var sendn = liveChatModel.SenderDetails;
                    var Referrer = $"https://www.quora.com/messages/thread/{threadId}";
                    var respHomepage = httpHelper.GetRequest("https://www.quora.com/");
                    var userName = Utilities.GetBetween(respHomepage.Response, "\\\"profileUrl\\\":\\\"", "\\\"");
                    var detail = readResponseHandler.LstMessageDetails.Where(x => x.UserFullName == currentFriend.SenderName).ToList();
                    Referrer = detail != null && detail.Count > 0 ? detail.FirstOrDefault()?.UserProfileUrl : Referrer;
                    response = httpHelper.GetRequest(Referrer);
                    basePostData = new BasePostData(response.Response);
                    var sendMsgResponse = quoraFunct.SendMessage(liveChatModel.DominatorAccountModel,
                                basePostData,liveChatModel.DominatorAccountModel.AccountBaseModel.UserId, message, readResponseHandler.LstMessageDetails.Count().ToString(),Referrer,threadId);

                    cancellation.ThrowIfCancellationRequested();
                    if (sendMsgResponse.Success)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            liveChatModel.LstChat.Add(new ChatDetails
                            {
                                Messeges = message,
                                SenderId = liveChatModel.DominatorAccountModel.UserName,
                                Sender = liveChatModel.SenderDetails.SenderName,
                                Time = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                Type = ChatMessage.Sent.ToString()
                            });
                            liveChatModel.TextMessage = string.Empty;
                            liveChatModel.SenderDetails.LastMesseges = message;
                        });

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return false;
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