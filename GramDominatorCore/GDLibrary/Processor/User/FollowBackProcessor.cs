using ThreadUtils;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Linq;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class FollowBackProcessor : BaseInstagramUserProcessor
    {
        public FollowBackProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel,  IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService,gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();

                if (ModuleSetting.IsFollowBack)
                {
                    var allFollowers = DbAccountService.GetFollowers().Where(x => x.Followers == 1);
                    foreach (var follower in allFollowers)
                    {

                        if (DbAccountService.GetInteractedUsers(ActivityType).Any(x => x.InteractedUsername == follower.Username && x.FollowedBack == 1))
                            continue;
                        //var followerInfo = new UserFriendshipResponse(new ResponseParameter { Response = "DOCTYPE html" });
                        //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                        //{
                        //    followerInfo = InstaFunction.UserFriendship(DominatorAccountModel, AccountModel, follower.UserId);
                        //    if (!CheckingLoginRequiredResponse(followerInfo.ToString(), "", queryInfo))
                        //        return; 
                        //}
                        //else
                        //{
                        //    followerInfo = GdBrowserManager.UserFriendship(DominatorAccountModel, AccountModel, follower.UserId,Token);
                        //}
                        var followerResponse = InstaFunction.SearchUsername(DominatorAccountModel, follower?.Username, Token);
                        if (followerResponse.Success && followerResponse?.instaUserDetails!=null && !followerResponse.instaUserDetails.IsFollowing)
                        {
                            InstagramUser instagramUser = new InstagramUser
                            {
                                Username = follower.Username,
                                UserId = follower.UserId,
                                FullName = follower.FullName,
                                IsPrivate = follower.IsPrivate,
                                IsVerified = follower.IsVerified,
                                IsBusiness = follower.IsBusiness,
                                ProfilePicUrl = follower.ProfilePicUrl,
                                HasAnonymousProfilePicture = follower.HasAnonymousProfilePicture
                            };

                            FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, instagramUser);

                            Token.ThrowIfCancellationRequested();
                        }
                    }
                }
                else
                {
                    var userInfo = InstaFunction.SearchUserInfoById(DominatorAccountModel,AccountModel, DominatorAccountModel.AccountBaseModel.UserId, Token).Result;
                    if (userInfo.IsPrivate)
                    {
                        var pendingUser = InstaFunction.PendingRequest(DominatorAccountModel);
                        var usersList = FilterWhitelistBlacklistUsers(pendingUser.UserList) ?? pendingUser.UserList;
                        LstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                        usersList.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.Username));
                        foreach (var user in pendingUser.UserList)
                        {
                            FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, user);

                            Token.ThrowIfCancellationRequested();
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                   JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                   $"This Account {DominatorAccountModel.AccountBaseModel.UserId} is public,please make sure it should be private Account");
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
                // ex.DebugLog();
            }
            finally
            {
                jobProcessResult.IsProcessCompleted=true;
            }
        }
    }
}
