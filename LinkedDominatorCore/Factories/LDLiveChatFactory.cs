using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;
using Unity;

namespace LinkedDominatorCore.Factories
{
    public class LDLiveChatFactory : ChatFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        public LDLiveChatFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
        }

        public string SessionId { get; set; } = string.Empty;


        public override void UpdateFriendList(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            try
            {
                SenderDetails lastSenderDetails = null;

                if (liveChatModel.DominatorAccountModel == null)
                {
                    var instance = InstanceProvider.GetInstance<IAccountsFileManager>();
                    var allAccounts = instance.GetAll().First();
                    liveChatModel.DominatorAccountModel = allAccounts;
                }
                var logInProcess = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<ILdLogInProcess>();
                logInProcess
                    .CheckLoginAsync(liveChatModel.DominatorAccountModel, liveChatModel.DominatorAccountModel.Token)
                    .Wait(cancellation);


                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                {
                    liveChatModel.LstSender = new ObservableCollection<SenderDetails>();
                    liveChatModel.LstChat = new ObservableCollection<ChatDetails>();
                }

                var isAllMessagesRead = false;
                LiveChatMessageUserdetailResponseHandler objTrackNewMessagesResponseHandler = null;
                cancellation.ThrowIfCancellationRequested();
                var linkedFunction = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId]
                    .Resolve<ILdFunctions>();
                while (!isAllMessagesRead)
                    try
                    {
                        cancellation.ThrowIfCancellationRequested();

                        objTrackNewMessagesResponseHandler = linkedFunction.LivechatUserMessage(
                            liveChatModel.DominatorAccountModel,
                            objTrackNewMessagesResponseHandler == null
                                ? 0
                                : objTrackNewMessagesResponseHandler.LastConnectedTimeStamp, linkedFunction);

                        if (lastSenderDetails != null && ObjectComparer.Compare(
                                objTrackNewMessagesResponseHandler.ChatListNewMessage.LastOrDefault(),
                                lastSenderDetails))
                        {
                            isAllMessagesRead = true;
                            continue;
                        }

                        lastSenderDetails = objTrackNewMessagesResponseHandler.ChatListNewMessage.LastOrDefault()
                            .DeepCloneObject();


                        var senderDetails = objTrackNewMessagesResponseHandler.ChatListNewMessage;


                        senderDetails.ForEach(x =>
                        {
                            try
                            {
                                x.AccountId = liveChatModel.DominatorAccountModel.AccountId;
                                cancellation.ThrowIfCancellationRequested();
                                SaveFriendDetails(liveChatModel, x, cancellation);
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
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
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

                var ldFunction =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<ILdFunctions>();

                liveChatModel.DominatorAccountModel = liveChatModel.DominatorAccountModel.Clone();


                var isAllMessagesRead = false;

                LiveChatUsermessagesResponseHandler objLiveChatUsermessagesResponseHandler = null;

                while (!isAllMessagesRead)
                {
                    try
                    {
                        cancellation.ThrowIfCancellationRequested();
                        objLiveChatUsermessagesResponseHandler = ldFunction.UserLiveChatMessage
                            (liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails, ldFunction);

                        if (objLiveChatUsermessagesResponseHandler != null)
                        {
                            var senderDetails = objLiveChatUsermessagesResponseHandler.ListNewMessage;

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
                                var chatDetails = new ChatDetails
                                {
                                    Type = liveChatModel.SenderDetails.LastMessageOwnerId ==
                                           liveChatModel.SenderDetails.SenderId
                                        ? ChatMessage.Received.ToString()
                                        : ChatMessage.Sent.ToString(),
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
                        liveChatModel.SenderDetails.LastMesseges=liveChatModel.LstChat.LastOrDefault().Messeges;
                        if (objLiveChatUsermessagesResponseHandler == null ||
                            !objLiveChatUsermessagesResponseHandler.HasMore) isAllMessagesRead = true;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    Thread.Sleep(1000);
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

        public override async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string message,
            List<string> lstImages,
            ChatMessageType messageType, CancellationToken cancellation)
        {
            try
            {
                var linkedInFunction =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<ILdFunctions>();
                var Image = "";

                cancellation.ThrowIfCancellationRequested();

                liveChatModel.DominatorAccountModel.SessionId = SessionId;

                if (messageType == ChatMessageType.Text)
                {
                    var status =
                        linkedInFunction.NormalMessageProcess(lstImages, liveChatModel.SenderDetails, "", message);
                    if (status.Contains("{\"value\":{\"createdAt\":"))
                    {
                        liveChatModel.LstChat.Add(new ChatDetails
                        {
                            Messeges = message,
                            SenderId = liveChatModel.DominatorAccountModel.UserName,
                            Sender = liveChatModel.SenderDetails.SenderName,
                            Time = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            Type = ChatMessage.Sent.ToString()
                        });
                        liveChatModel.SenderDetails.LastMesseges = message;
                        return true;
                    }

                    return false;
                }

                if (messageType == ChatMessageType.TextAndMedia || messageType == ChatMessageType.Mediashare ||
                    messageType == ChatMessageType.Media)
                {
                    var status =
                        linkedInFunction.NormalMessageProcess(lstImages, liveChatModel.SenderDetails, "", message);

                    if (status.Contains("{\"value\":{\"createdAt\":"))
                    {
                        var lstImagePath = new List<string>();

                        lstImages.ForEach(x =>
                        {
                            var filePath = x.Split('\\').Last();
                            File.Copy(x, $@"{Utils.GetLivechatAttachmentFilePath()}\{filePath}");
                            lstImagePath.Add($@"{Utils.GetLivechatAttachmentFilePath()}\{filePath}");
                        });

                        var createdTime = Utils.GetBetween(status, "\"createdAt\":", ",");
                        liveChatModel.LstChat.Add(new ChatDetails
                        {
                            Messeges = message,
                            MessegesId = $"{createdTime}_{liveChatModel.SenderDetails.SenderId}",
                            MessegeType = lstImagePath.Count > 0 && !string.IsNullOrEmpty(message)
                                ? ChatMessageType.TextAndMedia
                                : string.IsNullOrEmpty(message)
                                    ? ChatMessageType.Media
                                    : ChatMessageType.Text,
                            SenderId = liveChatModel.DominatorAccountModel.UserName,
                            Sender = liveChatModel.SenderDetails.SenderName,
                            ListMediaUrls = new ObservableCollection<string>(lstImagePath),
                            Time = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            Type = ChatMessage.Sent.ToString()
                        });
                        return true;
                    }

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
    }
}