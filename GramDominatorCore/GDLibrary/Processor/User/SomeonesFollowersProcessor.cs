using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class SomeonesFollowersProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        
          
        public SomeonesFollowersProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel,  IDelayService delayService, IGdBrowserManager gdBrowserManager) : 
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService,gdBrowserManager)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                QueryType = queryInfo.QueryType;
                List<InstagramUser> instaUserList = new List<InstagramUser>();
                if (queryInfo.QueryValue.ToLower() == "[own]")
                {
                    StartProcessForOwnFollowers(queryInfo);
                    return;
                }

                var userInfo = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token, true);
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    userInfo = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token, true); 
                //}
                //else
                //{
                //    userInfo = GdBrowserManager.GetUserInfo(DominatorAccountModel, queryInfo.QueryValue, Token);
                //}
               
                if (!CheckingLoginRequiredResponse(userInfo.ToString(), "", queryInfo))
                    return;
                if (userInfo.Success)
                {
                    if (userInfo.IsPrivate && (userInfo?.instaUserDetails != null && !userInfo.instaUserDetails.IsFollowing))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                            $"Account {queryInfo.QueryValue} Is Private");
                        jobProcessResult.IsProcessCompleted = true;
                        return;
                    }
                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        Token.ThrowIfCancellationRequested();

                        var browser = GramStatic.IsBrowser;
                        var followerUsers = 
                            browser ?
                            GdBrowserManager.GetUserFollowers(DominatorAccountModel, userInfo.Username, Token)
                            : InstaFunction.GetUserFollowers(DominatorAccountModel, userInfo.Pk, Token, jobProcessResult.maxId, userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followerUsers.ToString(), "", queryInfo))
                            return;
                        if (followerUsers.Success && followerUsers.UsersList.Count > 0)
                        {
                            var usersList = FilterWhitelistBlacklistUsers(followerUsers.UsersList);
                            GetInteractedUserAccrossAllFor(usersList ?? followerUsers.UsersList);
                            GetInteractedCampaignUser(usersList ?? followerUsers.UsersList);
                            CheckUserInDatabase(usersList ?? followerUsers.UsersList);
                            if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                                GetTaggedUser(queryInfo, jobProcessResult, usersList ?? followerUsers.UsersList);
                            else
                            {
                                if (ActivityType == ActivityType.Follow)
                                {
                                    var userList = usersList ?? followerUsers.UsersList;
                                    int count = 0;
                                    for (int i = 0; i < userList.Count; i++)
                                    {
                                        Token.ThrowIfCancellationRequested();
                                        count++;
                                        instaUserList.Add(userList[i]);

                                        if (count == 30 || userList.Count - 1 == i)
                                        {
                                            count = 0;
                                            instaUserList = GetUserInfoDetails(instaUserList);
                                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, instaUserList);
                                            instaUserList = new List<InstagramUser>();
                                        }
                                    }
                                }
                                else
                                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? followerUsers.UsersList);
                            }
                            jobProcessResult.maxId = followerUsers.MaxId;
                            if (string.IsNullOrEmpty(jobProcessResult.maxId))
                                jobProcessResult.HasNoResult = true;
                            Token.ThrowIfCancellationRequested();
                        }
                        else
                            jobProcessResult.maxId = null;
                        
                        CheckNoMoreDataForWithQuery(ref jobProcessResult);
                    }
                }
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
            catch (Exception )
            {
                //ex.DebugLog();
            }
        }
    }
}
