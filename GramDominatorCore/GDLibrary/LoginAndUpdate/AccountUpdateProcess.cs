using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using GramDominatorCore.Request;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public interface IAccountUpdateProcess
    {
        Task UpdateAccountAsync(DominatorAccountModel objDominatorAccountModel, CancellationToken token);

        Task UpdateAccountFollowerFollowingPostAsync(DominatorAccountModel dominatorAccountModel,
           CancellationToken token);

        void UpdateFollowers(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection = null,
           IEnumerable<Friendships> lstFollowers = null);

        void UpdateFollowings(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection = null,
            IEnumerable<Friendships> lstFollowings = null);

        void UpdateFeeds(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection);

        void UpdateAccountInbox(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection);
    }
    public class AccountUpdateProcess : IAccountUpdateProcess
    {
        private IGdHttpHelper HttpHelper { get; set; }

        private IInstaFunction InstaFunction { get; set; }

        private IGdLogInProcess LogInProcess { get; set; }

        private readonly IAccountScopeFactory _accountScopeFactory;
        public AccountUpdateProcess(IGdHttpHelper httpHelper, IInstaFunction instaFunction, IAccountScopeFactory accountScopeFactory, IGdLogInProcess logInProcess)
        {
            HttpHelper = httpHelper;
            InstaFunction = instaFunction;
            _accountScopeFactory = accountScopeFactory;
            LogInProcess = logInProcess;
        }

        public Task UpdateAccountAsync(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {
                return UpdateAccountFollowerFollowingPostAsync(objDominatorAccountModel, token);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public async Task UpdateAccountFollowerFollowingPostAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.UpdatingDetails;

                InstaFunction = LogInProcess.AssignBrowserFunction(dominatorAccountModel);
                GlobusLogHelper.log.Info(Log.UpdatingDetails, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Account Details");

                HttpHelper = ((LogInProcess)(LogInProcess)).GetHttpHelper();
                if (!dominatorAccountModel.IsUserLoggedIn || dominatorAccountModel.Cookies == null || HttpHelper.GetRequestParameter().Cookies == null && !dominatorAccountModel.IsRunProcessThroughBrowser)
                    if (!await LogInProcess.CheckLoginAsync(dominatorAccountModel, token))
                        return;

                var dboperationAccount = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                var lstAllFriendShips = dboperationAccount.Get<Friendships>();

                var lstFollowers = lstAllFriendShips.Where(x => x.Followers == 1).ToList();
                var lstFollowings = lstAllFriendShips.Where(x => x.Followings == 1).ToList();

                var userInfo = InstaFunction.SearchUsername(dominatorAccountModel, dominatorAccountModel.AccountBaseModel.ProfileId, CancellationToken.None);
                //if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    ((InstaFunct)InstaFunction).GetGdHttpHelper().GetRequestParameter().Cookies = dominatorAccountModel.Cookies;
                //    userInfo = InstaFunction.SearchUsername(dominatorAccountModel, dominatorAccountModel.AccountBaseModel.UserName, CancellationToken.None);
                //}
                //else
                //{
                //    if (InstaFunction.GdBrowserManager == null || InstaFunction.GdBrowserManager.BrowserWindow == null|| (InstaFunction.GdBrowserManager.BrowserWindow!=null && InstaFunction.GdBrowserManager.BrowserWindow.IsDisposed))
                //    {
                //        await LogInProcess.CheckLoginAsync(dominatorAccountModel, token);
                //    }
                //    userInfo = InstaFunction.GdBrowserManager.GetUserInfo(dominatorAccountModel,dominatorAccountModel.UserName, token);
                //}
                if (userInfo != null)
                {
                    dominatorAccountModel.DisplayColumnValue1 = userInfo.FollowerCount;
                    dominatorAccountModel.DisplayColumnValue2 = userInfo.FollowingCount;
                    dominatorAccountModel.DisplayColumnValue3 = userInfo.MediaCount;
                    GDAccountUpdateFactory.Instance.AddToDailyGrowth(dominatorAccountModel.AccountBaseModel.AccountId,
                        userInfo.FollowerCount, userInfo.FollowingCount,
                        userInfo.MediaCount);
                    dominatorAccountModel.Token.ThrowIfCancellationRequested();
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                        .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                        .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                        .SaveToBinFile();
                    // Update Follower details
                    if (userInfo.FollowerCount >= 0)
                        UpdateFollowers(dominatorAccountModel, dboperationAccount, lstFollowers);

                    // Update Followings details
                    if (userInfo.FollowingCount >= 0)
                        UpdateFollowings(dominatorAccountModel, dboperationAccount, lstFollowings);

                    // Update Feed details
                    if (userInfo.MediaCount >= 0)
                        UpdateFeeds(dominatorAccountModel, dboperationAccount);

                    GlobusLogHelper.log.Info(Log.DetailsUpdated, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Account Details");

            //        UpdateAccountInbox(dominatorAccountModel,dboperationAccount);

                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    InstaFunction.GdBrowserManager.CloseBrowser();
            }
        }

        public void UpdateFollowers(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection = null, IEnumerable<Friendships> lstFollowers = null)
        {
            GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                objDominatorAccountModel.AccountBaseModel.UserName, "Followers");

            try
            {
                var accountModel = new AccountModel(objDominatorAccountModel);
                if (databaseConnection == null)
                    databaseConnection = new DbOperations(objDominatorAccountModel.AccountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                if (lstFollowers == null)
                    lstFollowers = databaseConnection.Get<Friendships>().Where(x => x.Followers == 1);

                #region Update Followers Process

                string maxId = string.Empty;
                bool isFollowerUpdated = false;
                var lstUserFollowers = new List<InstagramUser>();
                var browser = GramStatic.IsBrowser;
                if (!browser)
                {
                    var userInfo = InstaFunction.SearchUsername(objDominatorAccountModel, objDominatorAccountModel.AccountBaseModel.ProfileId, CancellationToken.None);
                    while (!isFollowerUpdated)
                    {
                        FollowerAndFollowingIgResponseHandler followerResponse =
                            InstaFunction.GetUserFollowers(objDominatorAccountModel,userInfo?.Pk, CancellationToken.None, maxId, objDominatorAccountModel.AccountBaseModel.ProfileId, IsWeb: true).Result;
                        lstUserFollowers.AddRange(followerResponse.UsersList);
                        Thread.Sleep(2000);
                        if (lstUserFollowers.Count >= userInfo.FollowerCount)
                            break;
                        if (!string.IsNullOrEmpty(followerResponse.MaxId))
                            maxId = followerResponse.MaxId;
                        else
                            isFollowerUpdated = true;
                    }
                }
                else
                {
                    FollowerAndFollowingIgResponseHandler followerResponse =
                            InstaFunction.GdBrowserManager.GetUserFollowers(objDominatorAccountModel, objDominatorAccountModel.AccountBaseModel.UserName, CancellationToken.None);
                    lstUserFollowers.AddRange(followerResponse.UsersList);
                    Thread.Sleep(2000);
                    isFollowerUpdated = true;
                }
                objDominatorAccountModel.DisplayColumnValue1 = lstUserFollowers.Count;
                List<InstagramUser> lstNonAvailableFollower =
                    lstUserFollowers.Where(x => lstFollowers.All(y => x.Username != y.Username)).Distinct().ToList();

                List<Friendships> lstFriends = new List<Friendships>();
                lstNonAvailableFollower.ForEach(x =>
                {
                    try
                    {
                        Friendships friendship = new Friendships()
                        {
                            Username = x.Username,

                            IsPrivate = x.IsPrivate,
                            IsVerified = x.IsVerified,
                            UserId = x.Pk,
                            FullName = x.FullName,
                            HasAnonymousProfilePicture = x.HasAnonymousProfilePicture,
                            ProfilePicUrl = x.ProfilePicUrl,
                            Followers = 1,
                            Time = DateTimeUtilities.GetEpochTime()
                        };
                        lstFriends.Add(friendship);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                try
                {
                    lstFriends.Select(x => x.UserId).Distinct();
                    // var lstFriend = lstFriends.Distinct();
                    databaseConnection.AddRange(lstFriends);
                }
                catch (Exception ex)
                {
                    ex.DebugLog("error: while saving followers into database" + ex.StackTrace);
                }

                // Get all old followers those are currently not following
                List<Friendships> lstOldFollowers = lstFollowers.Where(x =>
                    lstUserFollowers.All(y => x.Username != y.Username)).ToList();

                // Remove lstOldFollowers data from database table
                lstOldFollowers.ForEach(x => { databaseConnection.Remove(x); });

                //lstOldFollowers.RemoveAll(x=>databaseConnection.)
                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Followers");

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }


        public void UpdateFollowings(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection = null, IEnumerable<Friendships> lstFollowings = null)
        {
            GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                objDominatorAccountModel.AccountBaseModel.UserName, "Followings");
            try
            {
                if (databaseConnection == null)
                    databaseConnection = new DbOperations(objDominatorAccountModel.AccountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                if (lstFollowings == null)
                    lstFollowings = databaseConnection.Get<Friendships>().Where(x => x.Followings == 1);

                var lstInteractedUserse = databaseConnection.Get<InteractedUsers>();
                AccountModel accountModel = new AccountModel(objDominatorAccountModel);
                #region Update Followings Process

                string maxId = string.Empty;
                bool isFollowingsUpdated = false;
                var lstUserFollowings = new List<InstagramUser>();
                var browser = GramStatic.IsBrowser;
                if (!browser)
                {
                    var userInfo = InstaFunction.SearchUsername(objDominatorAccountModel, objDominatorAccountModel.AccountBaseModel.ProfileId, CancellationToken.None);

                    while (!isFollowingsUpdated)
                    {
                        var followingsResponse =
                            InstaFunction.GetUserFollowings(objDominatorAccountModel, accountModel,userInfo?.Pk, CancellationToken.None,string.Empty, maxId, objDominatorAccountModel.AccountBaseModel.ProfileId, IsWeb: true).Result;
                        lstUserFollowings.AddRange(followingsResponse.UsersList);
                        Thread.Sleep(2000);
                        if (lstUserFollowings.Count >= userInfo.FollowingCount)
                            break;
                        if (!string.IsNullOrEmpty(followingsResponse.MaxId))
                            maxId = followingsResponse.MaxId;
                        else
                            isFollowingsUpdated = true;
                    }
                }
                else
                {
                    var followingsResponse =
                        InstaFunction.GdBrowserManager.GetUserFollowings(objDominatorAccountModel, objDominatorAccountModel.AccountBaseModel.UserName, CancellationToken.None);
                    lstUserFollowings.AddRange(followingsResponse.UsersList);
                    Thread.Sleep(2000);
                    isFollowingsUpdated = true;
                }
                List<InstagramUser> lstNonAvailableFollowings =
                    lstUserFollowings.Where(x => lstFollowings.All(y => x.Username != y.Username)).Distinct().ToList();

                List<Friendships> lstFollowingFriendShip = new List<Friendships>();
                lstNonAvailableFollowings.ForEach(x =>
                {
                    try
                    {
                        Friendships friendship = new Friendships()
                        {
                            Username = x.Username,
                            IsPrivate = x.IsPrivate,
                            IsVerified = x.IsVerified,
                            UserId = x.Pk,
                            FullName = x.FullName,
                            HasAnonymousProfilePicture = (x.HasAnonymousProfilePicture),
                            ProfilePicUrl = x.ProfilePicUrl,
                            Followings = 1,
                            FollowType = DominatorHouseCore.DatabaseHandler.GdTables.FollowType.Following,
                            Time = DateTimeUtilities.GetEpochTime()
                        };

                        lstFollowingFriendShip.Add(friendship);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
                try
                {
                    lstFollowingFriendShip.Select(x => x.UserId).Distinct();
                    databaseConnection.AddRange(lstFollowingFriendShip);
                }
                catch (Exception ex)
                {
                    ex.DebugLog("error:while saving Following into database" + ex.StackTrace);
                }
                // Get all old followings those are currently not following
                List<Friendships> lstOldFollowings =
                    lstFollowings.Where(x => lstUserFollowings.All(y => x.Username != y.Username)).ToList();

                // Remove lstOldFollowings data from database table
                lstOldFollowings.ForEach(x => { databaseConnection.Remove(x); });

                List<InteractedUsers> lstinInteractedUserses =
                    lstInteractedUserse.Where(x => lstUserFollowings.All(y => x.InteractedUsername != y.Username)).ToList();

                lstinInteractedUserses.ForEach(x => { databaseConnection.Remove(x); });

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Followings");
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }


        public void UpdateFeeds(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection)
        {
            GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                objDominatorAccountModel.AccountBaseModel.UserName, "Feeds");
            AccountModel accountModel = new AccountModel(objDominatorAccountModel);
            try
            {
                if (databaseConnection == null)
                    databaseConnection = new DbOperations(objDominatorAccountModel.AccountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                List<FeedInfoes> lstAllFeedsPresentsInDb = databaseConnection.Get<FeedInfoes>();
                #region Update Feeds Process
                string maxId = string.Empty;
                bool isFeedUpdated = false;
                var lstUserPost = new List<InstagramPost>();
                var browser = GramStatic.IsBrowser;
                if (!browser)
                {
                    while (!isFeedUpdated)
                    {
                        UserFeedIgResponseHandler userFeedresponse =
                            InstaFunction.GetUserFeed(objDominatorAccountModel, accountModel, objDominatorAccountModel.AccountBaseModel.ProfileId, CancellationToken.None, maxId);
                        if (userFeedresponse == null)
                        {
                            Thread.Sleep(5 * 1000);
                            userFeedresponse =
                                InstaFunction.GetUserFeed(objDominatorAccountModel, accountModel, objDominatorAccountModel.AccountBaseModel.ProfileId, CancellationToken.None, maxId);
                        }
                        lstUserPost.AddRange(userFeedresponse.Items);
                        Thread.Sleep(2000);

                        if (!string.IsNullOrEmpty(userFeedresponse.MaxId))
                            maxId = userFeedresponse.MaxId;
                        else
                            isFeedUpdated = true;
                    }
                }
                else
                {
                    var userFeedresponse =
                            InstaFunction.GdBrowserManager.GetUserFeed(objDominatorAccountModel, objDominatorAccountModel.AccountBaseModel.UserName, CancellationToken.None);
                    lstUserPost.AddRange(userFeedresponse.Items);
                    Thread.Sleep(2000);
                }


                // Get non-available followers of current account in DB
                List<InstagramPost> lstNonAvailableFeeds =
                    lstUserPost.Where(x => lstAllFeedsPresentsInDb.All(y => x.Code != y.MediaCode)).ToList();
                List<FeedInfoes> lstFeedsFeedInfoes = new List<FeedInfoes>();

                lstNonAvailableFeeds.ForEach(post =>
                {
                    try
                    {
                        FeedInfoes feedInfoes = new FeedInfoes()
                        {
                            Caption = post.Caption,
                            CommentCount = post.CommentCount,
                            CommentsDisabled = post.CommentsDisabled,
                            TakenAt = post.TakenAt,
                            MediaId = post.Pk,
                            MediaCode = post.Code,
                            MediaType = post.MediaType,
                            PostedBy = "OutSide Software"
                        };

                        lstFeedsFeedInfoes.Add(feedInfoes);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                try
                {
                    databaseConnection.AddRange(lstFeedsFeedInfoes);
                }
                catch (Exception ex)
                {
                    ex.DebugLog("error:while saving feeds into database" + ex.StackTrace);
                }
                // Get all old feeds those are currently not present on users feeds
                List<FeedInfoes> lstOldFeeds =
                    lstAllFeedsPresentsInDb.Where(x => lstUserPost.All(y => x.MediaCode != y.Code)).ToList();

                // Remove lstOldFollowers data from database table
                lstOldFeeds.ForEach(x => { databaseConnection.Remove(x); });

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Feeds");

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }

        public void UpdateAccountInbox(DominatorAccountModel objDominatorAccountModel, DbOperations databaseConnection)
        {
            GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                objDominatorAccountModel.AccountBaseModel.UserName, "Account MessageBox");
            try
            {
                if (databaseConnection == null)
                    databaseConnection = new DbOperations(objDominatorAccountModel.AccountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                List<UserConversation> lstAllUsersConversationPresentsInDb = databaseConnection.Get<UserConversation>();
                #region Update Feeds Process
                string maxId = string.Empty;
                bool isFeedUpdated = false;
                List<SenderDetails> lstUserConversation = new List<SenderDetails>();

                while (!isFeedUpdated)
                {
                    V2InboxResponse v2InboxResponse = InstaFunction.Getv2Inbox(objDominatorAccountModel, false, maxId).Result;
                    if (v2InboxResponse == null)
                    {
                        Thread.Sleep(5 * 1000);
                        v2InboxResponse = InstaFunction.Getv2Inbox(objDominatorAccountModel, false, maxId).Result;
                    }
                    lstUserConversation.AddRange(v2InboxResponse.LstSenderDetails);
                    Thread.Sleep(2000);

                    if (!string.IsNullOrEmpty(v2InboxResponse.CursorId))
                        maxId = v2InboxResponse.CursorId;
                    else
                        isFeedUpdated = true;
                }


                // Get non-available followers of current account in DB
                List<SenderDetails> lstNonAvailableUsers =
                    lstUserConversation.Where(x => lstAllUsersConversationPresentsInDb.All(y => x.SenderId != y.SenderId)).ToList();
                List<UserConversation> lstFeedsFeedInfoes = new List<UserConversation>();

                lstNonAvailableUsers.ForEach(user =>
                {
                    try
                    {
                        UserConversation userInbox = new UserConversation()
                        {
                            SenderName = user.SenderId,
                            SenderId = user.SenderName,
                            ThreadId = user.ThreadId,
                            Date = user.LastMessegedate,
                            ConversationType = "OutSide Software"
                        };

                        lstFeedsFeedInfoes.Add(userInbox);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                try
                {
                    databaseConnection.AddRange(lstFeedsFeedInfoes);
                }
                catch (Exception ex)
                {
                    ex.DebugLog("error:while saving feeds into database" + ex.StackTrace);
                }
                // Get all old feeds those are currently not present on users feeds
                List<UserConversation> lstOldFeeds =
                    lstAllUsersConversationPresentsInDb.Where(x => lstUserConversation.All(y => x.SenderId != y.SenderId)).ToList();

                // Remove lstOldFollowers data from database table
                lstOldFeeds.ForEach(x => { databaseConnection.Remove(x); });

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Account Message Inbox");

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }
    }
}
