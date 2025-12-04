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
using System.Linq;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class SomeonesFollowingsProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        public SomeonesFollowingsProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) : 
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                List<InstagramUser> instaUserList = new List<InstagramUser>();
                QueryType = queryInfo.QueryType;
                if (queryInfo.QueryValue.ToLower() == "[own]")
                {
                    StartProcessForOwnFollowings(queryInfo);
                    return;
                }
                if(CheckQueryValueOnMessageList(BroadcastMessagesModel,queryInfo))return;
                var usernameInfo = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    usernameInfo = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token); 
                //}
                //else
                //{
                //    usernameInfo = GdBrowserManager.GetUserInfo(DominatorAccountModel,queryInfo.QueryValue, Token);
                //}

                if (!CheckingLoginRequiredResponse(usernameInfo.ToString(), "", queryInfo))
                    return;
                if (usernameInfo.Success)
                {
                    if (usernameInfo.IsPrivate && (usernameInfo?.instaUserDetails != null && !usernameInfo.instaUserDetails.IsFollowing))
                    {
                        jobProcessResult.IsProcessCompleted = true;
                        return;
                    }
                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        Token.ThrowIfCancellationRequested();
                        var followingUsers = 
                            GramStatic.IsBrowser ?
                            GdBrowserManager.GetUserFollowings(DominatorAccountModel, usernameInfo.Username, Token)
                            : InstaFunction.GetUserFollowings(DominatorAccountModel, AccountModel, usernameInfo.Pk, Token, string.Empty, jobProcessResult.maxId, usernameInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followingUsers.ToString(), "", queryInfo))
                            return;

                        if (followingUsers.Success && followingUsers.UsersList.Count > 0)
                        {
                            var usersList = FilterWhitelistBlacklistUsers(followingUsers.UsersList);
                            GetInteractedUserAccrossAllFor(usersList ?? followingUsers.UsersList);
                            GetInteractedCampaignUser(usersList ?? followingUsers.UsersList);
                            CheckUserInDatabase(usersList ?? followingUsers.UsersList);
                            if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                                GetTaggedUser(queryInfo, jobProcessResult, usersList ?? followingUsers.UsersList);
                            else
                            {
                                if (ActivityType == ActivityType.Follow)
                                {
                                    var userList = usersList ?? followingUsers.UsersList;
                                    int count = 0;
                                    for (int i = 0; i < userList.Count; i++)
                                    {
                                        count++;
                                        instaUserList.Add(userList[i]);

                                        if (count ==30|| userList.Count - 1 == i)
                                        {
                                            Token.ThrowIfCancellationRequested();
                                            count = 0;
                                            instaUserList = GetUserInfoDetails(instaUserList);
                                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, instaUserList);
                                            instaUserList = new List<InstagramUser>();
                                        }
                                    }
                                }
                                else
                                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? followingUsers.UsersList);
                            }                               
                            jobProcessResult.maxId = followingUsers.MaxId;
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
            catch (Exception)
            {
                //  ex.DebugLog();
            }
        }
    }
}
