using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace GramDominatorCore.GDFactories
{
    public class GdLiveChatFactory : ChatFactory
    {
        public IAccountScopeFactory _accountScopeFactory;

        public GdLiveChatFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
        }

        // private IGdHttpHelper httpHelper;
        public override void UpdateFriendList(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            try
            {
                V2InboxResponse inboxResponse = null;
                //List<SenderDetails> LstSenderDetails = new List<SenderDetails>();
                //if (liveChatModel.DominatorAccountModel == null)
                //{
                //    var instance = InstanceProvider.GetInstance<IAccountsFileManager>();
                //    var allAccounts = instance.GetAll().First();
                //    liveChatModel.DominatorAccountModel = allAccounts;
                //}

                var logInProcess =
                _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IGdLogInProcess>();

                // var loginProcess = new LogInProcess(httpHelper);
                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                    logInProcess.CheckLoginAsync(liveChatModel.DominatorAccountModel, liveChatModel.DominatorAccountModel.Token).Wait(cancellation);


                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                {
                    liveChatModel.LstSender = new ObservableCollection<SenderDetails>();
                    liveChatModel.LstChat = new ObservableCollection<ChatDetails>();
                    return;
                    //  return liveChatModel;
                }
                var instaFunct =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IInstaFunction>();
                //var isnbtaFunct = new InstaFunct(liveChatModel.DominatorAccountModel);
                string cursorId = string.Empty;
                while (true)
                {
                    inboxResponse = instaFunct.Getv2Inbox(liveChatModel.DominatorAccountModel, cursorId: cursorId).Result;
                    //LstSenderDetails.AddRange(inboxResponse.LstSenderDetails);
                    if (string.IsNullOrEmpty(inboxResponse.CursorId))
                        break;
                    else
                        cursorId = inboxResponse.CursorId;

                    inboxResponse.LstSenderDetails.ForEach(x =>
                         {
                             x.AccountId = liveChatModel.DominatorAccountModel.AccountId;
                             SaveFriendDetails(liveChatModel, x, cancellation);
                         });

                    cancellation.ThrowIfCancellationRequested();
                }


                //liveChatModel.LstSender =new ObservableCollection<SenderDetails>(inboxResponse.LstSenderDetails);


                // liveChatModel.SenderDetailsCursorId = inboxResponse.CursorId;
                // return liveChatModel;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public override void UpdateCurrentChat(LiveChatModel liveChatModel, CancellationToken cancellation)
        {
            //List<SenderDetails> lstSenderDetails = new List<SenderDetails>();
            //List<ChatDetails> LstChatDetails = new List<ChatDetails>();
            VisualThreadResponse visualThreadResponse = null;
            try
            {
                //LogInProcess loginProcess = new LogInProcess();
                //loginProcess.CheckLogin(liveChatModel.DominatorAccountModel);

                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                    return;

                var instaFunct =
                    _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IInstaFunction>();

                // InstaFunct isnbtaFunct = new InstaFunct(liveChatModel.DominatorAccountModel);
                string cursorId = string.Empty;
                while (true)
                {
                    visualThreadResponse = instaFunct.GetVisualThread(liveChatModel.DominatorAccountModel, liveChatModel.SenderDetails.ThreadId, cursorId: cursorId);
                    
                    //visualThreadResponse.LstChatDetails = visualThreadResponse.LstChatDetails.OrderBy(x => x.Time).ToList();
                    
                    foreach (var chatDetails in visualThreadResponse.LstChatDetails)
                    {
                        if (chatDetails.MessegeType == ChatMessageType.LiveViewerInvite || chatDetails.MessegeType == ChatMessageType.Mediashare || chatDetails.MessegeType == ChatMessageType.Null
                            || chatDetails.MessegeType == ChatMessageType.RavenMedia || chatDetails.MessegeType == ChatMessageType.ReelShare || chatDetails.MessegeType == ChatMessageType.StoryShare
                            || chatDetails.MessegeType == ChatMessageType.TextAndMedia)
                            continue;
                        if (chatDetails.SenderId == liveChatModel.DominatorAccountModel.AccountBaseModel.UserId)
                            chatDetails.Type = ChatMessage.Sent.ToString();
                        else
                            chatDetails.Type = ChatMessage.Received.ToString();

                        chatDetails.SenderId = liveChatModel.SenderDetails.SenderId;
                        SaveChatDetails(liveChatModel, chatDetails, cancellation);
                    }

                    
                    if (string.IsNullOrEmpty(visualThreadResponse.oldestCursor))
                        break;
                    cursorId = visualThreadResponse.oldestCursor;

                }

            }
            catch (Exception)
            {
            }
        }

        public override async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string textMessage, List<string> lstImages, ChatMessageType chatMessageType, CancellationToken cancellation)
        {
            try
            {
                AccountModel accountModel = new AccountModel(liveChatModel.DominatorAccountModel);
                SendMessageIgResponseHandler sendMessageIgResponseHandler;
                if (!liveChatModel.DominatorAccountModel.IsUserLoggedIn)
                {
                    return false;
                }
                var instaFunct =
                _accountScopeFactory[liveChatModel.DominatorAccountModel.AccountId].Resolve<IInstaFunction>();
                if (textMessage.Contains("http:") || textMessage.Contains("http:") || textMessage.Contains("www.") || textMessage.Contains("bit.ly"))
                {
                    var lstLinks = new List<string>();
                    var linkUrlPattern = @"\b(?:https?://|www\.|bit.ly)\S+\b";
                    var linkParser = new Regex(linkUrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    foreach (Match matchedLink in linkParser.Matches(textMessage))
                    {
                        lstLinks.Add(matchedLink.Value);
                    }

                    sendMessageIgResponseHandler = lstLinks.Count > 0 ? instaFunct.SendMessageWithLink(liveChatModel.DominatorAccountModel, cancellation, liveChatModel.SenderDetails.SenderId, textMessage, lstLinks,"") : instaFunct.SendMessage(liveChatModel.DominatorAccountModel, accountModel, liveChatModel.SenderDetails.SenderId, textMessage,"", cancellation).Result;
                }
                else
                {
                    sendMessageIgResponseHandler =await instaFunct.SendMessage(liveChatModel.DominatorAccountModel, accountModel, liveChatModel.SenderDetails.SenderId, textMessage, "", cancellation);
                }
                if (sendMessageIgResponseHandler.Success)
                {
                    liveChatModel.LstChat.Add(new ChatDetails()
                    {
                        Messeges = textMessage,
                        SenderId = liveChatModel.DominatorAccountModel.UserName,
                        Sender = liveChatModel.SenderDetails.SenderName,
                        Time = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        Type = ChatMessage.Sent.ToString()
                    });

                    return true;
                }

                #region commented
                //liveChatModel.LstChat = new ObservableCollection<ChatDetails>(visualThreadResponse.lstChatDetails.Reverse()); ;
                //liveChatModel.LstChat.ForEach(x =>
                //{
                //    if (x.SenderId == liveChatModel.SenderDetails.SenderId)
                //    {
                //        x.Type = "Received";
                //    }
                //    else
                //    {
                //        x.Type = "Sent";
                //    }
                //});

                ////  liveChatModel = visualThreadResponse.cursorId;
                //liveChatModel.SenderDetails.more_available_max = visualThreadResponse.more_available_max;
                //liveChatModel.SenderDetails.more_available_min = visualThreadResponse.more_available_min;
                //liveChatModel.SenderDetails.next_max_id = visualThreadResponse.next_max_id;
                //liveChatModel.SenderDetails.next_min_id = visualThreadResponse.next_min_id; 
                #endregion

                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
