using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public abstract class BaseTumblrUserProcessor : BaseTumblrProcessor
    {
        protected BaseTumblrUserProcessor(IProcessScopeModel processScopeModel, ITumblrJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) :
            base(processScopeModel, jobProcess, dbAccountService, campaignService, tumblrFunct, globalService)
        {
        }


        public void FilterData(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, TumblrUser user)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_dbAccountService.GetInteractedUsers(base.ActivityType).Any(x => x.InteractedUsername.Contains(user.Username)))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType.ToString(), $"Successfully Skipped Already Interacted User => {user.Username}");
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }
                SendToPerformActivity(ref jobProcessResult, user, queryInfo);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FilterAndStartFinalProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<TumblrUser> lstTumblrUsers)
        {
            var FilteredResult = FilterUser(lstTumblrUsers);
            lstTumblrUsers = FilteredResult.Item1;
            if (FilteredResult.Item2 > 0)
                GlobusLogHelper.log.Info(Log.FilterApplied, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ((BaseTumblrProcessor)this).ActivityType, lstTumblrUsers.Count);
            foreach (var user in lstTumblrUsers)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!CheckUserUniqueNess(jobProcessResult, user.Username, base.ActivityType)) continue;
                FilterData(queryInfo, ref jobProcessResult, user);
            }
        }

        protected (List<TumblrUser>, int) FilterUser(List<TumblrUser> list)
        {
            var FilteredCount = 0;
            try
            {
                var SkippedCount = 0;
                switch (ActivityType)
                {
                    case ActivityType.Follow:
                        {
                            if (followerModel.UserFilterModel.IsFilterMustNotContainInvalidWords)
                            {
                                var ListWords = followerModel.UserFilterModel.LstInvalidWords;
                                FilteredCount = list.RemoveAll(x => ListWords.Any(y => x.FullName.Contains(y) || x.Username.Contains(y)));
                            }
                            //if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            //    FilteredCount = list.RemoveAll(x => x.IsFollowed || !x.CanFollow||x.IsYou);
                            break;
                        }
                    case ActivityType.BroadcastMessages:
                        {
                            if (broadcastMessagesModel.IsCheckSkipUsersWhoWereAlreadySentAMessageFromTheSoftware)
                            {
                                var interractedUser = _dbAccountService.GetInteractedUsers(base.ActivityType);
                                var interactedusernameForSelectedAccount = interractedUser.Where(x =>
                                    x.UserName.Equals(JobProcess.DominatorAccountModel.AccountBaseModel.UserId)).ToList();
                                FilteredCount = list.RemoveAll(z => interactedusernameForSelectedAccount.Any(t => z.Username.Contains(t.InteractedUsername)));
                            }
                            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                                FilteredCount = list.RemoveAll(x => !x.CanMessage || x.IsYou);
                            break;
                        }
                    case ActivityType.Unfollow:
                        if (unFollowerModel.IsWhoDoNotFollowBackChecked || unFollowerModel.IsWhoFollowBackChecked)
                        {
                            var lstFollowerUsernames = new List<string>();
                            var searchUsersForFollowingResponse = TumblrFunct.GetAcccountFollowers(JobProcess.DominatorAccountModel);
                            lstFollowerUsernames = searchUsersForFollowingResponse.LstTumblrUser
                             .Select(eachFollower => eachFollower.Username).ToList();
                            while (searchUsersForFollowingResponse.Success && !string.IsNullOrEmpty(searchUsersForFollowingResponse.NextPageUrl))
                            {
                                searchUsersForFollowingResponse = TumblrFunct.GetAcccountFollowers(JobProcess.DominatorAccountModel);
                                lstFollowerUsernames.AddRange(searchUsersForFollowingResponse?.LstTumblrUser
                                .Select(eachFollower => eachFollower.Username).ToList());
                            }
                            if (unFollowerModel.IsWhoDoNotFollowBackChecked)
                                SkippedCount = list.RemoveAll(x => lstFollowerUsernames.Any(y => y == x.Username));
                            if (unFollowerModel.IsWhoFollowBackChecked)
                                SkippedCount += list.RemoveAll(x => lstFollowerUsernames.Any(y => y != x.Username));
                        }
                        break;

                }
                if (followerModel.IsSkipBlacklistsUser || broadcastMessagesModel.IsSkipBlacklistsUser || unFollowerModel.IsChkSkipWhiteListedUser)
                {
                    var SkippedBlackorWhiteListedPostOwnerNames = FilterBlacklistedOrWhiteListedUsers(list.Select(x => x.Username).ToList());
                    SkippedCount = list.RemoveAll(x => SkippedBlackorWhiteListedPostOwnerNames.Count == 0 || !SkippedBlackorWhiteListedPostOwnerNames.Any(y => y == x.Username));
                }
                if (SkippedCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"SuccessFully Skipped  {SkippedCount} BlackListed Users");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return (list, FilteredCount);
        }
        public void SendToPerformActivity([NotNull] ref JobProcessResult jobProcessResult, TumblrUser tumblrUser,
            QueryInfo queryInfo)
        {
            if (jobProcessResult == null) throw new ArgumentNullException(nameof(jobProcessResult));
            jobProcessResult = JobProcess.FinalProcess(new TumblrScrapeResult
            {
                ResultUser = tumblrUser,
                TumblrFormKey = tumblrUser.TumblrsFormKey,
                QueryInfo = queryInfo
            });
        }
    }
}