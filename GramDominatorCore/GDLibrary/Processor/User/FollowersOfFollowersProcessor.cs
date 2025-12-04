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

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class FollowersOfFollowersProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        public FollowersOfFollowersProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
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
                if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                JobProcessResult result = jobProcessResult;
                string maxId = null;
                bool hasnoResult = false;

                var userInfo = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                if (!CheckingLoginRequiredResponse(userInfo.ToString(), "", queryInfo))
                    return;
                if (userInfo.Success)
                {
                    if (userInfo.IsPrivate && (userInfo?.instaUserDetails != null && !userInfo.instaUserDetails.IsFollowing))
                        return;

                    #region Getting Followers operation
                    while (!jobProcessResult.IsProcessCompleted && !hasnoResult)
                    {
                        Token.ThrowIfCancellationRequested();

                        FollowerAndFollowingIgResponseHandler followerUsers =
                                InstaFunction.GetUserFollowers(DominatorAccountModel, userInfo.Pk, Token, maxid: maxId, userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followerUsers.ToString(), "", queryInfo))
                            return;
                        if (followerUsers.Success)
                        {
                           
                            foreach (InstagramUser instaUser in followerUsers.UsersList)
                            {
                                Token.ThrowIfCancellationRequested();
                                jobProcessResult.HasNoResult = false;
                                // Inner loop Opertaion mentioning Followers of Followers
                                #region Inner loop mentioning Followers of Followers Opertaion

                                if (instaUser.IsPrivate && !instaUser.IsFollowing)
                                    continue;

                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                {
                                    Token.ThrowIfCancellationRequested();

                                    FollowerAndFollowingIgResponseHandler followerOfFollowersUsers;
                                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                                        followerOfFollowersUsers =InstaFunction.GetUserFollowersBrowser(DominatorAccountModel, instaUser.Username, Token, QueryType, jobProcessResult.maxId);
                                    else
                                        followerOfFollowersUsers =InstaFunction.GetUserFollowers(DominatorAccountModel, instaUser.Pk, Token, maxid: jobProcessResult.maxId, instaUser.Username, IsWeb: true).Result;

                                    // Get Followers of Followers Users from intermediate users
                                    if (!CheckingLoginRequiredResponse(followerOfFollowersUsers.ToString(), "", queryInfo))
                                        continue;
                                    if (followerOfFollowersUsers.Success)
                                    {
                                        #region Process for "FollowersOfFollowers" query parameter
                                        var usersList = FilterWhitelistBlacklistUsers(followerOfFollowersUsers.UsersList);                                                                                
                                        GetInteractedUserAccrossAllFor(usersList ?? followerOfFollowersUsers.UsersList);
                                        GetInteractedCampaignUser(usersList ?? followerOfFollowersUsers.UsersList);
                                        CheckUserInDatabase(usersList ?? followerOfFollowersUsers.UsersList);
                                        if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                                            GetTaggedUser(queryInfo, jobProcessResult, usersList ?? followerOfFollowersUsers.UsersList);
                                        else
                                        {
                                            if (ActivityType == ActivityType.Follow)
                                            {
                                                var userList = usersList ?? followerOfFollowersUsers.UsersList;
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
                                                jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? followerOfFollowersUsers.UsersList);
                                        }                                            
                                        #endregion
                                        jobProcessResult.maxId = followerOfFollowersUsers.MaxId;
                                        if(string.IsNullOrEmpty(jobProcessResult.maxId))
                                            jobProcessResult.HasNoResult = true;
                                        
                                    }
                                    else
                                        jobProcessResult.maxId = null;

                                  
                                }
                                #endregion
                            }

                            result.maxId = followerUsers.MaxId;
                        }
                        else
                        {
                            maxId = null;
                            result.maxId = maxId;
                        }
                        CheckNoMoreDataForWithQuery(ref jobProcessResult);
                        result.IsProcessCompleted = jobProcessResult.IsProcessCompleted;
                        CheckNoMoreDataForWithQuery(ref result);
                        hasnoResult = jobProcessResult.HasNoResult;

                    }
                    #endregion
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
                // ex.DebugLog();
            }
        }
    }
}
