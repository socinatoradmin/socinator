using DominatorHouseCore;
using DominatorHouseCore.Enums;
//using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThreadUtils;
using Unity;

namespace FaceDominatorCore.FDFactories
{
    public class FdChatFactory : ChatFactory
    {
        static FdChatFactory _instance;
        public string SessionId { get; set; } = string.Empty;
        //public static FdChatFactory Instance 
        //    => _instance ?? (_instance = new FdChatFactory());
        public IAccountScopeFactory _accountScopeFactory;
        public IAccountsFileManager _accountsFileManager;
        private readonly IDelayService _delayService;
        private readonly IFdBrowserManager _browserManager;

        public FdChatFactory(IAccountScopeFactory accountScopeFactory,
            IDelayService delayService, IFdBrowserManager browserManager)
        {
            _accountScopeFactory = accountScopeFactory;
            _delayService = delayService;
            _browserManager = browserManager;
        }

        public override void UpdateFriendList(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            // var fdRequestLibrary = InstanceProvider.GetInstance<IFdRequestLibrary>();
            // FdRequestLibrary fdRequestLibrary = new FdRequestLibrary();

            var fdRequestLibrary =
                _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IFdRequestLibrary>();
            try
            {
                var isLoggedIn = false;

                cancellation.ThrowIfCancellationRequested();
                if (!liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                    isLoggedIn = fdRequestLibrary.Login(liveChatModel.DominatorAccountModel);
                else
                    isLoggedIn = _browserManager.BrowserLogin(liveChatModel.DominatorAccountModel, cancellation, loginType: LoginType.BrowserLogin);


                SessionId = liveChatModel.DominatorAccountModel.SessionId;

                if (string.IsNullOrEmpty(SessionId))
                    SessionId = fdRequestLibrary.SessionId;

                if (!isLoggedIn)
                    return;

                cancellation.ThrowIfCancellationRequested();

                //if (liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    _browserManager.OpenMessengerWindow(liveChatModel.DominatorAccountModel);
                //}

                #region Getting messages for display in view [Useful]
                //while (!isAllMessagesRead)
                //{

                //    try
                //    {
                //        cancellation.ThrowIfCancellationRequested();

                //        if (liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                //        {
                //            //Opening Messanger window and getting chat data
                //            objMessageSenderResponseHandler = _browserManager.ScrollWindowAndGetUnRepliedMessages(liveChatModel.DominatorAccountModel, MessageType.Inbox, 5, 0);
                //        }
                //        else
                //            objMessageSenderResponseHandler = fdRequestLibrary.GetRecentFriendMessageDetails(liveChatModel.DominatorAccountModel, objMessageSenderResponseHandler, cancellation);


                //        if (objMessageSenderResponseHandler != null)
                //        {
                //            var senderDetails = objMessageSenderResponseHandler.ObjFdScraperResponseParameters.MessageSenderDetailsList;

                //            senderDetails.ForEach(x =>
                //            {
                //                try
                //                {
                //                    cancellation.ThrowIfCancellationRequested();
                //                    SaveFriendDetails(liveChatModel, x, cancellation);
                //                }
                //                catch (OperationCanceledException)
                //                {
                //                    throw new OperationCanceledException();
                //                }
                //                catch (Exception ex)
                //                {
                //                    ex.DebugLog();
                //                }
                //            });
                //        }

                //        if (!objMessageSenderResponseHandler.HasMoreResults)
                //        {
                //            isAllMessagesRead = true;
                //        }
                //    }
                //    catch (OperationCanceledException)
                //    {
                //        throw new OperationCanceledException();
                //    }
                //    catch (Exception ex)
                //    {
                //        ex.DebugLog();
                //    }

                //} 
                #endregion


                _browserManager.SearchByGraphSearchUrl(liveChatModel.DominatorAccountModel, FbEntityType.Friends, $"{FdConstants.FbHomeUrl}messages/");

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


        public override void UpdateCurrentChat(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();

                var fdRequestLibrary =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IFdRequestLibrary>();

                var isLoggedIn = false;

                cancellation.ThrowIfCancellationRequested();

                //if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn || string.IsNullOrEmpty(SessionId))
                //{
                //    isLoggedIn = fdRequestLibrary.Login(liveChatModel.DominatorAccountModel);
                //}
                //else if (liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                //    isLoggedIn = _browserManager.BrowserLogin(liveChatModel.DominatorAccountModel, cancellation, LoginType.CheckLogin);
                //else

                isLoggedIn = liveChatModel.DominatorAccountModel.IsUserLoggedIn;

                bool isChatWindow = false;
                var pageSource = _browserManager.BrowserWindow.GetPageSource();
                isChatWindow = pageSource.Contains("data-testid=\"mwthreadlist-item\"");

                if (liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    if (!isChatWindow)
                        _browserManager.SearchByGraphSearchUrl(liveChatModel.DominatorAccountModel, FbEntityType.AddedFriends,
                        $"{FdConstants.FbHomeUrl}messages/t/{liveChatModel.SenderDetails.SenderId}");

                }

                Thread.Sleep(2000);

                liveChatModel.DominatorAccountModel = liveChatModel.DominatorAccountModel.Clone();

                SessionId = liveChatModel.DominatorAccountModel.SessionId;

                if (string.IsNullOrEmpty(SessionId))
                    SessionId = fdRequestLibrary.SessionId;

                bool isAllMessagesRead = false;

                IResponseHandler objMessageSenderResponseHandler = null;

                while (!isChatWindow && !isAllMessagesRead && isLoggedIn)
                {

                    try
                    {
                        cancellation.ThrowIfCancellationRequested();

                        if (!liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                            objMessageSenderResponseHandler = fdRequestLibrary.GetRecentMessageDetails
                                            (liveChatModel.DominatorAccountModel, objMessageSenderResponseHandler, liveChatModel, cancellation);
                        else
                            objMessageSenderResponseHandler = _browserManager.ScrollWindowAndGetMessages(liveChatModel.DominatorAccountModel, 20,
                               liveChatModel.SenderDetails.SenderId, 0);

                        if (objMessageSenderResponseHandler != null)
                        {
                            var senderDetails = objMessageSenderResponseHandler.ObjFdScraperResponseParameters.ListChatDetails;

                            senderDetails.ForEach(x =>
                            {
                                try
                                {
                                    cancellation.ThrowIfCancellationRequested();
                                    SaveChatDetails(liveChatModel, x, cancellation);
                                }
                                catch (OperationCanceledException)
                                {
                                    throw new OperationCanceledException();
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            });
                        }
                        else if (liveChatModel.LstChat.Count == 0)
                        {
                            try
                            {
                                cancellation.ThrowIfCancellationRequested();
                                ChatDetails chatDetails = new ChatDetails()
                                {
                                    Type = liveChatModel.SenderDetails.LastMessageOwnerId == liveChatModel.SenderDetails.SenderId ? ChatMessage.Received.ToString() : ChatMessage.Sent.ToString(),
                                    SenderId = liveChatModel.SenderDetails.SenderId,
                                    Sender = liveChatModel.SenderDetails.SenderName,
                                    Messeges = liveChatModel.SenderDetails.LastMesseges,
                                    Time = liveChatModel.SenderDetails.LastMessegedate,
                                    MessegesId = liveChatModel.SenderDetails.SenderId
                                };
                                SaveChatDetails(liveChatModel, chatDetails, cancellation);

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
                        if (objMessageSenderResponseHandler == null || !objMessageSenderResponseHandler.HasMoreResults)
                        {
                            isAllMessagesRead = true;
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

                    _delayService.ThreadSleep(1000);
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


        public override async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string message, List<string> lstImages, ChatMessageType messageType, CancellationToken cancellation)
        {
            try
            {
                var fdRequestLibrary =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IFdRequestLibrary>();

                cancellation.ThrowIfCancellationRequested();

                if (liveChatModel.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var pageSource = await _browserManager.BrowserWindow.GetPageSourceAsync();
                    _browserManager.SelectUserAndSendMessage(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails, message);
                    return false;
                }
                else
                {
                    if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn || string.IsNullOrEmpty(SessionId))
                    {
                        await fdRequestLibrary.LoginAsync(liveChatModel.DominatorAccountModel, cancellation);
                    }
                    liveChatModel.DominatorAccountModel.SessionId = SessionId;

                    if (messageType == ChatMessageType.Text)
                    {
                        var responseHandler = await fdRequestLibrary.SendTextMessageAsync(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails, message, cancellation);

                        return await SaveMessage(liveChatModel, responseHandler, cancellation);
                    }
                    else if (messageType == ChatMessageType.TextAndMedia)
                    {
                        var responseHandler = await fdRequestLibrary.SendImageWithTextAsync(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails, lstImages);

                        await SaveMessage(liveChatModel, responseHandler, cancellation);

                        if (responseHandler != null && responseHandler.Status)
                        {
                            responseHandler = await fdRequestLibrary.SendTextMessageAsync(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails, message, cancellation);
                        }

                        return await SaveMessage(liveChatModel, responseHandler, cancellation);
                    }
                    else if (messageType == ChatMessageType.Media)
                    {
                        var responseHandler = await fdRequestLibrary.SendImageWithTextAsync(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails, lstImages);

                        return await SaveMessage(liveChatModel, responseHandler, cancellation);
                    }
                    else
                        return false;
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
            return false;
        }


        public async Task<bool> SaveMessage(LiveChatModel liveChatModel, IResponseHandler responseHandler
            , CancellationToken cancellation)
        {
            if (responseHandler != null)
                SaveChatDetails(liveChatModel, responseHandler.ObjFdScraperResponseParameters.ListSenderDetails
                    .FirstOrDefault(), cancellation);

            return responseHandler == null ? false : responseHandler.Status;
        }


        public override void CloseBrowser(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            _browserManager.CloseBrowser(liveChatModel.DominatorAccountModel);
        }
    }
}
