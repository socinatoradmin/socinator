using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
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
using System.Text.RegularExpressions;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public abstract class BaseInstagramUserProcessor : BaseInstagramProcessor
    {

        protected BaseInstagramUserProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }
        public List<InteractedUsers> LstInteractedUsers { get; set; } = new List<InteractedUsers>();
        private List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers> LstCampaignIntractedUsers { get; set; } = new List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
        public JobProcessResult FilterAndStartFinalProcess(QueryInfo queryInfo, JobProcessResult jobProcessResult, List<InstagramUser> lstUserNames)
        {
            List<InstagramUser> lstUserName = CheckUserInDatabase(lstUserNames);
            if(lstUserName.Count == 0)
            {
                jobProcessResult.HasNoResult = true;
                jobProcessResult.IsProcessCompleted= true;
                return jobProcessResult;
            }
            if (ActivityType == ActivityType.Follow)
            {
                FilterAllUserResults(lstUserName, queryInfo);
                if (lstUserName.Count != 0 && ModuleSetting.FollowOnlyBusinessAccounts)
                {
                    FilterOnlyBusinessAccounts(lstUserName);
                }
            }
            if (ActivityType == ActivityType.StoryViewer && !DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                GetStoryUsers(lstUserName);
                if (lstUserName.Count == 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"No Story users found from this query {queryInfo.QueryValue}");

            }
            foreach (var User in lstUserName)
            {
                var eachUser = User;
                if (eachUser.Pk == DominatorAccountModel.AccountBaseModel.UserId)
                    continue;
                try
                {
                    Token.ThrowIfCancellationRequested();
                    if (!Checkduplicatemessage(eachUser, queryInfo, jobProcessResult))
                        continue;
                    if (ActivityType == ActivityType.Follow && IsAlreadyFollowingUsers(eachUser))
                        continue;
                    if (SkippedAsBlackListWhitelisted(User))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Skipped User {User?.Username} As BlackListed User");
                        continue;
                    }
                    if (!FilterUserResults(eachUser, queryInfo))
                        SendToPerformActivity(ref jobProcessResult, eachUser, queryInfo);
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{eachUser.Username} Filter not matched");
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
                    //ex.DebugLog();
                }
                finally
                {
                    jobProcessResult.IsProcessCompleted = true;
                }
            }

            return jobProcessResult;
        }

        public bool FilterUserResults(InstagramUser instaUser, QueryInfo queryInfo)
        {
            if (instaUser == null)
                return true;
            switch (ActivityType)
            {
                case ActivityType.Unfollow:
                case ActivityType.Like:
                case ActivityType.Comment:
                    break;
                case ActivityType.Follow:
                case ActivityType.BroadcastMessages:
                case ActivityType.AutoReplyToNewMessage:
                case ActivityType.SendMessageToFollower:
                case ActivityType.UserScraper:

                    // Get already action performed users for current account

                    if (ModuleSetting.IsSkipUserWhoReceivedMessage)
                        if (IsAlreadyRecievedMessaged(instaUser))
                            return true;
                    if (ModuleSetting.shouldNotBeAlreadyFollowingThisAccount)
                        if (IsAlreadyFollowingUsers(instaUser))
                            return true;
                    break;
            }
            if (ActivityType != ActivityType.Follow)
                return FilterUserApply(instaUser, queryInfo);
            else
                return FilterUserApplyForFollow(instaUser, queryInfo);

        }

        public List<InstagramUser> FilterAllUserResults(List<InstagramUser> instaUser, QueryInfo queryInfo)
        {
            if (instaUser == null || instaUser.Count == 0)
                return instaUser;
            return FilterAllUserApply(instaUser, queryInfo);
        }

        public List<InstagramPost> FilterAllPostUserResults(List<InstagramPost> instaPostUser, QueryInfo queryInfo)
        {
            if (instaPostUser == null || instaPostUser.Count == 0)
                return instaPostUser;
            return FilterAllPostUserApply(instaPostUser, queryInfo);
        }

        internal bool IsAlreadyRecievedMessaged(InstagramUser instaUser)
        {
            if (!string.IsNullOrEmpty(CampaignId))
            {
                LstCampaignIntractedUsers = CampaignService.GetAllInteractedUsers().Where(x => x.Username == DominatorAccountModel.UserName && x.ActivityType == ActivityType.ToString()).ToList();
                if (LstCampaignIntractedUsers.Any(user => user.InteractedUsername == instaUser.Username))
                    return true;
            }
            return false;
        }

        protected void FilterAndStartFinalProcessForOneUser(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, InstagramUser instagramUser, InstagramPost instagramPost = null)
        {

            Token.ThrowIfCancellationRequested();
            if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.AutoReplyToNewMessage ||
                ActivityType == ActivityType.SendMessageToFollower)
            {
                if (CheckingUniqueUserAccountWise(true, instagramUser))
                    return;
            }
            if (!Checkduplicatemessage(instagramUser, queryInfo, jobProcessResult))
                return;
            if (ActivityType == ActivityType.Follow && (instagramUser.IsFollowing || IsAlreadyFollowingUsers(instagramUser)))
            {
                if (queryInfo.QueryType == "Custom Users List")
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                    $"This {DominatorAccountModel.UserName} is already following this {instagramUser.Username} user");
                    return;
                }
                else
                    return;
            }
            if (ModuleSetting.FollowOnlyBusinessAccounts && ActivityType == ActivityType.Follow)
            {
                if (!instagramUser.IsBusiness)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"This {instagramUser.Username} is not a business account");
                    return;
                }

            }
            if (ActivityType == ActivityType.StoryViewer && !GetStoryUsers(ref instagramUser))
            {
                var message = string.IsNullOrEmpty(instagramUser?.Username)?queryInfo.QueryValue:instagramUser?.Username;
                GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"No Story Found For {message}");
                return;
            }

            if (!FilterUserResults(instagramUser, queryInfo))
            {
                // Accept pending messaged user request for "AutoReplyToNewMessages" activity
                if (ActivityType == ActivityType.AutoReplyToNewMessage && !CheckAndAcceptPendingMessagedUserRequest(queryInfo, instagramUser))
                    return;
                if (queryInfo.QueryType == "Custom Users List"
                    && !string.IsNullOrEmpty(queryInfo.QueryValue)
                    && string.IsNullOrEmpty(instagramUser?.Username))
                    instagramUser.Username = queryInfo.QueryValue;
                SendToPerformActivity(ref jobProcessResult, instagramUser, queryInfo, instagramPost);
            }
            else 
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{instagramUser.Username} Filter not matched");
                if(queryInfo.QueryType != null && queryInfo.QueryType.Equals("Custom Users List"))
                    jobProcessResult.HasNoResult = true;
            }
                

        }

        protected bool CheckAndAcceptPendingMessagedUserRequest(QueryInfo queryInfo, InstagramUser instagramUser)
        {
            try
            {
                // Only applicable for "AutoReplyToNewMessage" module
                string[] senderDetailsEssentialInfo = Regex.Split(queryInfo.QueryTypeDisplayName, "<:>");
                bool isPendingUser = Convert.ToBoolean(senderDetailsEssentialInfo.First());

                if (isPendingUser)
                {
                    string threadId = senderDetailsEssentialInfo.Last();

                    if (!InstaFunction.AcceptMessageRequest(threadId).Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                            $"Unable to allow {instagramUser.Username} to message");
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        protected List<InstagramUser> GetAllUser(List<Friendships> friends)
        {
            List<InstagramUser> listInstagramUser = new List<InstagramUser>();

            foreach (Friendships friend in friends)
            {
                Token.ThrowIfCancellationRequested();

                InstagramUser instagramUser = new InstagramUser
                {
                    Username = friend.Username,
                    UserId = friend.UserId,
                    FullName = friend.FullName,
                    IsPrivate = friend.IsPrivate,
                    IsVerified = friend.IsVerified,
                    IsBusiness = friend.IsBusiness,
                    ProfilePicUrl = friend.ProfilePicUrl,
                    HasAnonymousProfilePicture = friend.HasAnonymousProfilePicture,
                    Pk = friend.UserId,
                    IsFollowing = friend.FollowType == DominatorHouseCore.DatabaseHandler.GdTables.FollowType.Following
                };
                listInstagramUser.Add(instagramUser);
            }
            return listInstagramUser;
        }
        protected void StartProcessForOwnFollowings(QueryInfo queryInfo)
        {
            try
            {
                if (queryInfo.QueryValue == null)
                    throw new NullReferenceException();

                QueryType = queryInfo.QueryType;

                if (JobProcess.IsStop())
                    return;

                JobProcessResult jobProcessResult = new JobProcessResult();

                var allFollowers = DbAccountService.GetFollowers().Where(x => x.Followers == 1).ToList();
                allFollowers.Shuffle();

                foreach (var follower in allFollowers)
                {
                    if (JobProcess.IsStop())
                        return;

                    DelayService.ThreadSleep(TimeSpan.FromSeconds(1));// Thread.Sleep(1000);
                    var followerInfo = InstaFunction.SearchUsername(DominatorAccountModel, follower.Username, Token);

                    #region Process for "SomeonesFollowers" query parameter

                    if (QueryType == "SomeonesFollowers")
                        FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, followerInfo);

                    #endregion
                    Token.ThrowIfCancellationRequested();
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
                // ignored
            }
        }

        protected void StartProcessForOwnFollowers(QueryInfo queryInfo)
        {
            try
            {
                if (queryInfo.QueryValue == null)
                    throw new NullReferenceException();

                QueryType = queryInfo.QueryType;

                if (JobProcess.IsStop())
                    return;

                JobProcessResult jobProcessResult = new JobProcessResult();
                var allFollowers = DbAccountService.GetFollowers().Where(x => x.Followers == 1).ToList();
                allFollowers.Shuffle();

                foreach (var follower in allFollowers)
                {
                    if (JobProcess.IsStop())
                        return;

                    DelayService.ThreadSleep(TimeSpan.FromSeconds(1));// Thread.Sleep(1000);
                    var followerInfo = InstaFunction.SearchUsername(DominatorAccountModel, follower.Username, Token);

                    if (QueryType == "SomeonesFollowers")
                        FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, followerInfo);

                    Token.ThrowIfCancellationRequested();
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

        public void SendToPerformActivity([NotNull] ref JobProcessResult jobProcessResult, InstagramUser instagramUser, QueryInfo queryInfo, InstagramPost instagramPost = null)
        {
            if (jobProcessResult == null) throw new ArgumentNullException(nameof(jobProcessResult));
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultUser = instagramUser,
                QueryInfo = queryInfo,
                ResultPost = instagramPost,
            });
        }

        public InstagramUser MessangerData(UsernameInfoIgResponseHandler usernameInfoIgResponseHandler)
        {
            InstagramUser objInstagramUser = new InstagramUser();
            objInstagramUser.UserId = usernameInfoIgResponseHandler.Pk;
            objInstagramUser.Pk = usernameInfoIgResponseHandler.Pk;
            objInstagramUser.Username = usernameInfoIgResponseHandler.Username;
            objInstagramUser.ProfilePicUrl = usernameInfoIgResponseHandler.ProfilePicUrl;
            objInstagramUser.IsPrivate = usernameInfoIgResponseHandler.IsPrivate;
            objInstagramUser.IsVerified = usernameInfoIgResponseHandler.IsVerified;
            objInstagramUser.ProfilePicUrl = usernameInfoIgResponseHandler.ProfilePicUrl;
            objInstagramUser.FullName = usernameInfoIgResponseHandler.FullName;
            return objInstagramUser;
        }

        public bool CheckingUniqueUserAccountWise(bool isRefreshData, InstagramUser instaUser)
        {
            if (isRefreshData)
            {
                try
                {
                    //LstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                    //if (LstInteractedUsers.Any(user => user.InteractedUsername == instaUser.Username))
                    //    return true;
                    return ActivityType == ActivityType.Follow && instaUser != null && instaUser.IsFollowing;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return false;
        }

        public List<InstagramUser> CheckUserInDatabase(List<InstagramUser> lstUserNames)
        {
            var Removed = 0;
            if (ActivityType == ActivityType.UserScraper && !ModuleSetting.IsScrpeUniqueUserForThisCampaign)
            {
                LstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                Removed += lstUserNames.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.Username));
            }
            if (ActivityType == ActivityType.BroadcastMessages || ActivityType == ActivityType.SendMessageToFollower || ActivityType == ActivityType.AutoReplyToNewMessage && ModuleSetting.IsSkipUserWhoReceivedMessage)
            {
                LstInteractedUsers = DbAccountService.GetInteractedUsersMessageData().ToList();
                Removed += lstUserNames.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.Username));
            }
            if (ActivityType == ActivityType.Follow)
            {
                LstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                Removed += lstUserNames.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.Username));
            }
            if (ActivityType == ActivityType.Follow)
            {
                List<Friendships> lstFrindList = DbAccountService.GetFriendships(DominatorHouseCore.DatabaseHandler.GdTables.FollowType.Following).Where(x => x.Followings == 1).ToList();
                Removed += lstUserNames.RemoveAll(x => lstFrindList.Any(y => y.Username == x.Username));
            }
            if(Removed > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                   DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                   $"Successfully Skipped {Removed} Interacted Users.");
            return lstUserNames;
        }

        public void GetTaggedUser(QueryInfo queryInfo, JobProcessResult jobProcessResult, List<InstagramUser> usersList)
        {
            try
            {
                if (ModuleSetting.IsTaggedPostUser)
                {
                    foreach (var user in usersList)
                    {
                        Token.ThrowIfCancellationRequested();
                        do
                        {
                            var userFeed =
                                GramStatic.IsBrowser ?
                                GdBrowserManager.GetUserFeed(DominatorAccountModel, user?.Username, Token)
                                :InstaFunction.GetUserFeed(DominatorAccountModel, AccountModel, user?.Username, Token, jobProcessResult.maxId);
                            if (userFeed != null)
                            {
                                userFeed.Items.RemoveAll(x => x.UserTags.Count == 0);
                                if (userFeed.Items.Count == 0)
                                {
                                    DelayService.ThreadSleep(TimeSpan.FromSeconds(2)); // Thread.Sleep(TimeSpan.FromSeconds(2));
                                }
                            }

                            if (userFeed != null)
                            {
                                foreach (var itemUser in userFeed.Items)
                                {
                                    Token.ThrowIfCancellationRequested();

                                    FilterAndStartFinalProcess(queryInfo, jobProcessResult, itemUser.UserTags);
                                }

                                if (!string.IsNullOrEmpty(userFeed.MaxId))
                                    jobProcessResult.maxId = userFeed.MaxId;
                            }
                        } while (jobProcessResult.maxId != null);
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

        public bool Checkduplicatemessage(InstagramUser instagramUser, QueryInfo queryInfo, JobProcessResult jobProcessResult)
        {
            try
            {
                if (ModuleSetting.IsSkipUserWhoReceivedMessage && (ActivityType == ActivityType.BroadcastMessages || ActivityType == ActivityType.SendMessageToFollower || ActivityType == ActivityType.AutoReplyToNewMessage))
                {

                    List<UserConversation> lstConversationUserList = DbAccountService.GetConversationUser().ToList();
                    if (lstConversationUserList.Any(x => x.SenderName == instagramUser.Username))
                    {
                        var userThread = lstConversationUserList.Where(x => x.SenderName == instagramUser.Username).ToList();
                        string threadId = userThread[0].ThreadId;
                        if (string.IsNullOrEmpty(threadId))
                            return false;
                        VisualThreadResponse getChatInfo = InstaFunction.GetVisualThread(DominatorAccountModel, threadId);
                        foreach (ChatDetails visualThreadResponse in getChatInfo.LstChatDetails)
                        {
                            if (visualThreadResponse.ClientContext != null)
                                return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        public SenderDetails SetDataIntoSendDetailslst(InstagramUser instagramUser)
        {
            SenderDetails senderDetails = new SenderDetails
            {
                SenderId = instagramUser.Pk,
                SenderName = instagramUser.Username,

            };
            return senderDetails;
        }

        public List<InstagramUser> GetStoryUsers(List<InstagramUser> lstUserName)
        {
            List<string> storyChecking = new List<string>();
            try
            {
                List<UsersStory> lstUsers = new List<UsersStory>();

                for (int i = 0; i < lstUserName.Count; i++)
                {
                    storyChecking.Add(lstUserName[i].Pk);

                    if (storyChecking.Count == 19 || i == lstUserName.Count - 1)
                    {
                        PostStoryResponse postStoryResponse = null;
                        if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            postStoryResponse = InstaFunction.GetStoriesUsers(DominatorAccountModel, AccountModel, storyChecking);
                            lstUsers.AddRange(postStoryResponse.LstUsers);
                            storyChecking = new List<string>();
                        }
                    }
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var postStoryResponse = InstaFunction.GdBrowserManager.GetStoriesUser(DominatorAccountModel, lstUserName[i], Token);
                        lstUsers.AddRange(postStoryResponse.LstUsers);
                    }
                }
                
                lstUserName.RemoveAll(x => lstUsers.All(y => x.Pk != y.UserId));
                for (int users = 0; users < lstUserName.Count; users++)
                {
                    lstUserName[users].UserStories = lstUsers[users].LstMedia;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return lstUserName;
        }

        public bool GetStoryUsers(ref InstagramUser instaUser)
        {
            List<string> storyChecking = new List<string>();
            try
            {
                storyChecking.Add(instaUser.Pk);
                PostStoryResponse checking = null;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    checking = InstaFunction.GetStoriesUsers(DominatorAccountModel, AccountModel, storyChecking);
                }
                else
                    checking = InstaFunction.GdBrowserManager.GetStoriesUser(DominatorAccountModel, instaUser, Token);
                
                if (checking.LstUsers.Count == 0)
                    return false;
                else
                    instaUser.UserStories = checking.LstUsers[0].LstMedia;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }
    }
}
