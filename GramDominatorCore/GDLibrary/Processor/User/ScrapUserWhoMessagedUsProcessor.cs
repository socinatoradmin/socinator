using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class ScrapUserWhoMessagedUsProcessor : BaseInstagramUserProcessor
    {
        public ScrapUserWhoMessagedUsProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel,delayService,gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            string username = DominatorAccountModel.UserName;
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;
                var browser = GramStatic.IsBrowser;
                var usernameInfoIgResponseHandler = 
                    browser ?
                    GdBrowserManager.GetUserInfo(DominatorAccountModel, username, Token)
                    : InstaFunction.SearchUsername(DominatorAccountModel, username, Token);
                if (!CheckingLoginRequiredResponse(usernameInfoIgResponseHandler.ToString(), "", queryInfo))
                    return;

                do
                {
                    var lstSender = new List<InstagramUser>();
                    V2InboxResponse v2InboxResponse = null;
                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        v2InboxResponse = InstaFunction.Getv2Inbox(DominatorAccountModel, false, jobProcessResult.maxId).Result; 
                    }
                    else
                    {
                        v2InboxResponse = GdBrowserManager.GetInbox(DominatorAccountModel, false, Token);
                    }
                    if (!CheckingLoginRequiredResponse(v2InboxResponse.ToString(), "", queryInfo))
                        return;
                    if (v2InboxResponse.Success)
                    {
                        string accountUserId = usernameInfoIgResponseHandler.Pk;
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "please wait we are getting user who sent a message to you");

                        foreach (var senderId in v2InboxResponse.LstSenderDetails)
                        {
                            Token.ThrowIfCancellationRequested();
                            string cursorId = string.Empty;
                            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                            {
                                do
                                {
                                    VisualThreadResponse getChatInfo = InstaFunction.GetVisualThread(DominatorAccountModel, senderId.ThreadId, cursorId: cursorId);
                                    if (getChatInfo == null)
                                    {
                                        DelayService.ThreadSleep(TimeSpan.FromSeconds(2));// Thread.Sleep(TimeSpan.FromSeconds(2));
                                        getChatInfo = InstaFunction.GetVisualThread(DominatorAccountModel, senderId.ThreadId);
                                    }
                                    DelayService.ThreadSleep(TimeSpan.FromSeconds(1));// Thread.Sleep(TimeSpan.FromSeconds(1));
                                    foreach (ChatDetails visualThreadResponse in getChatInfo.LstChatDetails)
                                    {
                                        Token.ThrowIfCancellationRequested();
                                        if (visualThreadResponse.ClientContext == null || accountUserId != visualThreadResponse.SenderId)
                                        {
                                            usernameInfoIgResponseHandler = InstaFunction.SearchUsername(DominatorAccountModel, senderId.SenderName, Token);
                                            InstagramUser objMessanger = MessangerData(usernameInfoIgResponseHandler);
                                            lstSender.Add(objMessanger);
                                            getChatInfo.oldestCursor = "";
                                            break;
                                        }
                                    }

                                    cursorId = getChatInfo.oldestCursor;
                                } while (!string.IsNullOrEmpty(cursorId)); 
                            }
                            else
                            {
                                var userinfo = 
                                    browser ?
                                    GdBrowserManager.GetUserInfo(DominatorAccountModel, senderId.SenderName, Token)
                                    : InstaFunction.SearchUsername(DominatorAccountModel, senderId.SenderName, Token);
                                senderId.SenderId = userinfo.Pk;
                                InstagramUser objMessanger = MessangerData(userinfo);
                                bool isPresent = lstSender.Any(x => x.Username.Equals(objMessanger.Username));
                                if (!isPresent)
                                    lstSender.Add(objMessanger);
                                else
                                    continue;
                            }
                        }
                        var usersList = FilterWhitelistBlacklistUsers(lstSender);
                        if (ActivityType == ActivityType.UserScraper && ModuleSetting.IsTaggedPostUser)
                            GetTaggedUser(queryInfo, jobProcessResult, usersList ?? lstSender);
                        else
                            FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? lstSender);
                    }
                    jobProcessResult.maxId = v2InboxResponse.CursorId;
                } while (!string.IsNullOrEmpty(jobProcessResult.maxId));

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception)
            {
                // ex.DebugLog();
            }
        }
    }
}
