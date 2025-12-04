using AutoMapper;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDLibrary;
using Unity;

namespace TwtDominatorCore.TDFactories
{
    public class TDLiveChatFactory : ChatFactory
    {
        private ITdHttpHelper httpHelper;
        private readonly IAccountScopeFactory _accountScopeFactory;
        public string SessionId { get; set; } = string.Empty;

        public TDLiveChatFactory(IAccountScopeFactory accountScopeFactory, IAccountsFileManager accountsFileManager)
        {
            _accountScopeFactory = accountScopeFactory;
        }
        public override void UpdateFriendList(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            try
            {
                if (liveChatModel.DominatorAccountModel == null)
                {
                    var instance = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
                    var allAccounts = instance.GetAll().First();
                    liveChatModel.DominatorAccountModel = allAccounts;
                }
                var logInProcess = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();
                //var loginProcess = new LogInProcess(httpHelper);
              logInProcess.CheckLoginAsync(liveChatModel.DominatorAccountModel, liveChatModel.DominatorAccountModel.Token).Wait(cancellation);
                


                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                {
                    liveChatModel.LstSender = new ObservableCollection<SenderDetails>();
                    liveChatModel.LstChat = new ObservableCollection<ChatDetails>();

                }
                bool isAllMessagesRead = false;
                TrackNewMessagesResponseHandler objTrackNewMessagesResponseHandler = null;
                cancellation.ThrowIfCancellationRequested();
                var TwitterFunct = _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<ITwitterFunctions>(); //ServiceLocator.Current.GetInstance<ITwitterFunctions>();
                var singleMessageUserdetail = new SenderDetails();
                while (!isAllMessagesRead)
                {

                    try
                    {

                        cancellation.ThrowIfCancellationRequested();

                        objTrackNewMessagesResponseHandler = TwitterFunct.getNewMessages(liveChatModel.DominatorAccountModel, objTrackNewMessagesResponseHandler == null ? null : objTrackNewMessagesResponseHandler.MinPosition, true);


                        if (objTrackNewMessagesResponseHandler != null && objTrackNewMessagesResponseHandler.HasMore)
                        {
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
                        else
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

                }

                //  var inboxResponse = TwitterFunct.getNewMessages(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetailsCursorId);

                //liveChatModel.LstSender = new ObservableCollection<SenderDetails>(inboxResponse.ListNewMessage);
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    inboxResponse.LstSenderDetails.ForEach(x =>
                //    {
                //        x.AccountId = liveChatModel.DominatorAccountModel.AccountId;
                //        SaveFriendDetails(liveChatModel, x, cancellation);
                //    });
                // });


                // liveChatModel.SenderDetailsCursorId = inboxResponse.CursorId;

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

                var tdFunction =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<ITwitterFunctions>();
                //var fdRequestLibrary = ServiceLocator.Current.GetInstance<IFdRequestLibrary>();
                //FdRequestLibrary fdRequestLibrary = new FdRequestLibrary();

                liveChatModel.DominatorAccountModel = liveChatModel.DominatorAccountModel.Clone();

               // liveChatModel.DominatorAccountModel.SessionId = SessionId;

                bool isAllMessagesRead = false;

                LiveChatUsermessagesResponseHandler objLiveChatUsermessagesResponseHandler = null;

                while (!isAllMessagesRead)
                {

                    try
                    {
                        cancellation.ThrowIfCancellationRequested();
                       // var _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                       // var senderuserDetails = _genericFileManager.GetModuleDetails<ChatDetails>(
                       //FileDirPath.GetChatDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork)).Where(x => x.SenderId == liveChatModel.SenderDetails.SenderId) ;

                       // if (senderuserDetails.ToList().Count >= 2)
                       // {
                       //     var userId = senderuserDetails.FirstOrDefault().SenderId.Intersect(senderuserDetails.LastOrDefault().SenderId);
                       // }

                        objLiveChatUsermessagesResponseHandler = tdFunction.UserLiveChatMessage
                                        (liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails);

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
                        if (objLiveChatUsermessagesResponseHandler == null || !objLiveChatUsermessagesResponseHandler.HasMore)
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

        public override async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string message, ChatMessageType messageType, CancellationToken cancellation)
        {
            try
            {
                var twitterFunction =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<ITwitterFunctions>();
                // var fdRequestLibrary = ServiceLocator.Current.GetInstance<IFdRequestLibrary>();
                //FdRequestLibrary fdRequestLibrary = new FdRequestLibrary();

                cancellation.ThrowIfCancellationRequested();

                liveChatModel.DominatorAccountModel.SessionId = SessionId;

                if (messageType == ChatMessageType.Text)
                {

                    var status = twitterFunction.SendDirectMessage(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails.SenderId, message, liveChatModel.DominatorAccountModel.UserName);

                    return status.Success;
                }

                else
                    return false;
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

    //public class ClassMapper
    //{
    //    public IMapper ClassMap<TSource, TDestination>() 
    //    {
    //        var config = new MapperConfiguration(cfg =>
    //        {
    //            cfg.CreateMap<TSource, TDestination>();
    //        });
    //        return config.CreateMapper();
    //    }
    //}



//public override void UpdateCurrentChat(LiveChatModel liveChatModel, CancellationToken cancellation)
//{
//    List<SenderDetails> lstSenderDetails = new List<SenderDetails>();

//    try
//    {

//        if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
//        {
//            return;
//        }
//        var TwitterFunct = ServiceLocator.Current.GetInstance<ITwitterFunctions>();
//          VisualThreadResponse visualThreadResponse = TwitterFunct.GetVisualThread(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails.ThreadId);
//       liveChatModel.LstChat = new ObservableCollection<ChatDetails>(visualThreadResponse.LstChatDetails.Reverse());

//        Application.Current.Dispatcher.Invoke(() =>
//        {
//            liveChatModel.LstChat.ForEach(x =>
//            {
//                x.SenderId = liveChatModel.SenderDetails.SenderId;
//                SaveChatDetails(liveChatModel, x, cancellation);
//            });
//        });

//        liveChatModel.SenderDetails.MoreAvailableMax = visualThreadResponse.MoreAvailableMax;
//        liveChatModel.SenderDetails.MoreAvailableMin = visualThreadResponse.MoreAvailableMin;
//        liveChatModel.SenderDetails.NextMaxId = visualThreadResponse.NextMaxId;
//        liveChatModel.SenderDetails.NextMinId = visualThreadResponse.NextMinId;
//    }
//    catch (Exception)
//    {
//    }
//}

//public override async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string textMessage, ChatMessageType chatMessageType, CancellationToken cancellation)
// {
//     try
//     {
//         SendMessageIgResponseHandler sendMessageIgResponseHandler;
//         if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
//         {
//             return false;
//         }
//         var twitterfunc = ServiceLocator.Current.GetInstance<ITwitterFunctions>();
//         if (textMessage.Contains("http:") || textMessage.Contains("http:") || textMessage.Contains("www.") || textMessage.Contains("bit.ly"))
//         {
//             var lstLinks = new List<string>();
//             var linkUrlPattern = @"\b(?:https?://|www\.|bit.ly)\S+\b";
//             var linkParser = new Regex(linkUrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

//             foreach (Match matchedLink in linkParser.Matches(textMessage))
//             {
//                 lstLinks.Add(matchedLink.Value);
//             }

//             // in SendMessagewithLink Method:-Parameter accountModel need to pass in future implementation
//             sendMessageIgResponseHandler = lstLinks.Count > 0 ? twitterfunc.SendDirectMessage(liveChatModel.DominatorAccountModel, liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails.SenderId, textMessage, lstLinks) : instaFunct.SendMessage(liveChatModel.DominatorAccountModel, cancellation, liveChatModel.SenderDetails.SenderId, textMessage);
//         }
//         else
//         {
//             sendMessageIgResponseHandler = twitterfunc.SendDirectMessage(liveChatModel.DominatorAccountModel, cancellation, liveChatModel.SenderDetails.SenderId, textMessage);
//         }
//          if (sendMessageIgResponseHandler.Success)
//         {
//             liveChatModel.LstChat.Add(new ChatDetails()
//             {
//                 Messeges = textMessage,
//                 SenderId = liveChatModel.DominatorAccountModel.UserName,
//                 Sender = liveChatModel.SenderDetails.SenderName,
//                 Time = DateTime.Now.ToString(CultureInfo.InvariantCulture),
//                 Type = ChatMessage.Sent.ToString()
//             });

//             return true;
//         }

//         return false;

//     }
//     catch (Exception)
//     {
//         return false;
//     }
// }



