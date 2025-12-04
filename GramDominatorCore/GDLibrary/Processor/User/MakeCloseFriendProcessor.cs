using DominatorHouseCore;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class MakeCloseFriendProcessor : BaseInstagramUserProcessor
    {
        private readonly CloseFriendModel closeFriend;
        private Dictionary<string,List<string>> CloseFriendCollections=new Dictionary<string, List<string>>();
        public MakeCloseFriendProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService,
            IGdBrowserManager gdBrowserManager) : base(jobProcess, dbAccountService, campaignService, processScopeModel,
                delayService, gdBrowserManager)
        {
            closeFriend = processScopeModel.GetActivitySettingsAs<CloseFriendModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if(closeFriend != null)
                {
                    if (closeFriend.IsCheckAllFollowers)
                    {
                        var closeFriendsCollections = new List<string>();
                        InstaFunction.GdBrowserManager.CloseBrowser(false);
                        if (!CloseFriendCollections.Any(x => x.Key.Contains(DominatorAccountModel.AccountId))
                            && !CloseFriendCollections.Any(y=>y.Value.Count > 0))
                        {
                            var closeFriendsList = InstaFunction.GdBrowserManager.GetCloseFriendList(DominatorAccountModel).Result;
                            closeFriendsCollections = closeFriendsList.CloseFriendsList;
                            CloseFriendCollections.Add(DominatorAccountModel.AccountId,closeFriendsCollections);
                        }
                        else
                        {
                            CloseFriendCollections.TryGetValue(DominatorAccountModel.AccountId, out closeFriendsCollections);
                        }
                        var followings = InstaFunction.GdBrowserManager.GetUserFollowerChrome(DominatorAccountModel, DominatorAccountModel.UserName, Token,true,jobProcessResult.maxId, closeFriendsCollections);
                        jobProcessResult.maxId = followings?.MaxId;
                        var filtered = FilterInteractedClosedFriend(followings.UsersList.Select(x => x.Username).ToList(),followings.SkippedCount);
                        followings.UsersList = followings.UsersList.Where(x=>filtered.Any(y=>y==x.Username)).ToList();
                        foreach(var user in followings.UsersList)
                        {
                            StartSingleUserToMakeFriend(user, ref jobProcessResult);
                            if (jobProcessResult.IsProcessSuceessfull)
                            {
                                try
                                {
                                    CloseFriendCollections.TryGetValue(DominatorAccountModel.AccountId,out closeFriendsCollections);
                                    closeFriendsCollections.Add(user.Username);
                                    CloseFriendCollections[DominatorAccountModel.AccountId] = closeFriendsCollections;
                                }
                                catch { }
                            }
                        }
                        jobProcessResult.HasNoResult = !followings.HasMoreresult;
                        JobProcess.DominatorAccountModel.IsNeedToSchedule = followings.HasMoreresult;
                    }
                    else if (closeFriend.IsCheckedCustomFollowerList)
                    {
                        var followerList = closeFriend.CustomFollowerList?.Replace("\r\n","\n").Split('\n').ToList();
                        followerList = FilterInteractedClosedFriend(followerList);
                        foreach(var follower in followerList)
                        {
                            //var instagramUser = InstaFunction.GdBrowserManager.GetUserInfo(DominatorAccountModel,follower, Token);
                            var instagramUser = InstaFunction.SearchUsername(DominatorAccountModel, follower, Token);
                            StartSingleUserToMakeFriend(instagramUser,ref jobProcessResult);
                        }
                    }

                }
            }
            catch (OperationCanceledException)
            {
                throw;
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
                if (closeFriend != null && closeFriend.IsCheckedCustomFollowerList)
                {
                    jobProcessResult.IsProcessCompleted = true;
                    JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                }
            }
        }

        private void StartSingleUserToMakeFriend(InstagramUser user,ref JobProcessResult jobProcessResult)
        {
            FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, user);
        }

        private List<string> FilterInteractedClosedFriend(List<string> followerList,int SkippedCount=0)
        {
            try
            {
                var interected = CampaignService.GetCloseFriend(DominatorAccountModel.AccountBaseModel.UserName);
                if(interected == null || interected.Count == 0)
                    return followerList;
                var filteredfollower = new List<string>();
                foreach (var follower in followerList)
                {
                    var username = follower.Split('/')?.ToList()?.LastOrDefault(x=>x!=string.Empty);
                    if (!interected.Any(x => x.UserName == username))
                        filteredfollower.Add(username);
                    else
                        SkippedCount++;
                }
                followerList = filteredfollower;
            }
            catch { }
            finally
            {
                if (SkippedCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"Successfully Skipped {SkippedCount} Closed Friends.");
            }
            return followerList;
        }
    }
}
