using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FaceDominatorCore.FDLibrary
{
    public abstract class BaseFbProcessor : IQueryProcessor
    {
        #region Properties

        protected DominatorAccountModel AccountModel;

        private readonly ModuleConfiguration _moduleConfiguration;


        protected readonly FdJobProcess JobProcess;

        //protected readonly IFdJobProcess FdJobProcess;

        protected readonly IDbAccountServiceScoped _dbAccountService;

        protected readonly IDbCampaignService _campaignService;

        protected readonly IFdRequestLibrary ObjFdRequestLibrary;

        protected IFdHttpHelper HttpHelper;

        protected IProcessScopeModel ProcessScopeModel;

        /*
        private static ConcurrentDictionary<string, object> _lockObjects = new ConcurrentDictionary<string, object>();
        */

        protected ActivityType _ActivityType => JobProcess.ActivityType;


        protected SoftwareSettingsModel _SoftwareSettingsModel { get; set; }

        protected IFdBrowserManager Browsermanager { get; set; }

        protected IFdPostModel _activitySettings { get; set; }

        protected int _maxCommentsPerPost { get; set; }



        #endregion


        protected BaseFbProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
        {
            _campaignService = campaignService;
            _dbAccountService = dbAccountService;
            ProcessScopeModel = processScopeModel;
            JobProcess = (FdJobProcess)jobProcess;
            AccountModel = jobProcess.AccountModel;
            ObjFdRequestLibrary = objRequestLibrary;
            Browsermanager = browserManager;
            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            _moduleConfiguration = jobActivityConfigurationManager[AccountModel.AccountId, _ActivityType];
        }

        public void SetActivity(IProcessScopeModel processScopeModel)
        {

            if (_ActivityType == ActivityType.PostLiker)
                _activitySettings = processScopeModel.GetActivitySettingsAs<PostLikerModel>();

            else if (_ActivityType == ActivityType.PostCommentor)
            {
                _activitySettings = processScopeModel.GetActivitySettingsAs<PostCommentorModel>();
                _maxCommentsPerPost = _activitySettings.MaximumCommentPerPost.GetRandom();
            }

        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        public void Start(QueryInfo queryInfo)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var jobProcessResult = new JobProcessResult();

                if (queryInfo.QueryType != null)
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                           string.Format("LangKeySearchingFor".FromResourceDictionary(), $"{queryInfo.QueryType}", $"{queryInfo.QueryValue}"));

                if (AccountModel.IsRunProcessThroughBrowser)
                {

                    Browsermanager.ChangeLanguage(AccountModel, FdConstants.FdDefaultLanguage);
                    Browsermanager.AssignCancelationToken(JobProcess.JobCancellationTokenSource.Token);
                    //some work have done by http so cookies assigned
                    ObjFdRequestLibrary.SetCoockies(AccountModel);
                }
                else
                {
                    ObjFdRequestLibrary.GetLangugae(AccountModel);
                    ObjFdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.FdDefaultLanguage);
                }

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    Process(queryInfo, ref jobProcessResult);
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //protected bool AlreadyInteractedUser(FacebookUser objFacebookUser)
        //{

        //    if (ActivityType != ActivityType.ProfileScraper)
        //    {
        //        if (_dbAccountService.DoesInteractedUserExist(objFacebookUser.UserId, ActivityType))
        //            return true;
        //    }
        //    else
        //    {
        //        var modulesetting = AccountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(
        //        x => x.ActivityType == ActivityType);

        //        if (modulesetting != null)
        //        {
        //            if(modulesetting.IsTemplateMadeByCampaignMode)
        //            {
        //                if (_campaignService.DoesInteractedUserExist(objFacebookUser.UserId, ActivityType))
        //                    return true;
        //            }
        //            else
        //            {
        //                if (_dbAccountService.DoesInteractedUserExist(objFacebookUser.UserId, ActivityType))
        //                    return true;
        //            }
        //        }

        //    }
        //    if (objFacebookUser.IsAlreadyFriend && ActivityType==ActivityType.SendFriendRequest)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        protected bool AlreadyInteractedUser(FacebookUser objFacebookUser)
        {
            if (_moduleConfiguration.IsTemplateMadeByCampaignMode && (_ActivityType == ActivityType.BroadcastMessages ||
                                                                      _ActivityType == ActivityType.ProfileScraper))
            {
                if (_campaignService.DoesInteractedUserExistForAccount(objFacebookUser.UserId, _ActivityType,
                    AccountModel.AccountBaseModel.UserName))
                    return true;
            }
            else if (_dbAccountService.DoesInteractedUserExist(objFacebookUser.UserId, _ActivityType))
                return true;

            //Group Member Scraper ke liye
            if (!string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl) && _ActivityType == ActivityType.GroupScraper)
                return _dbAccountService.DoesInteractedUserExistCustom(objFacebookUser.ScrapedProfileUrl, _ActivityType);

            if (_dbAccountService.DoesInteractedUserExist(objFacebookUser.UserId, _ActivityType, true))
                return true;

            return false;
        }

        protected bool AlreadyInteractedUserCustom(FacebookUser objFacebookUser)
            => _dbAccountService.DoesInteractedUserExistCustom(objFacebookUser.UserId, _ActivityType)
                || _dbAccountService.DoesInteractedUserExistCustom(objFacebookUser.ScrapedProfileUrl, _ActivityType);

        protected bool AlreadyInteractedComments(FdPostCommentDetails objFdPostCommentDetails)
        {

            if (_ActivityType != ActivityType.CommentScraper)
            {
                return _dbAccountService.DoesInteractedCommentsExist(objFdPostCommentDetails.CommentId, _ActivityType);
            }
            else
            {
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[AccountModel.AccountId, _ActivityType];

                if (modulesetting != null)
                {
                    return modulesetting.IsTemplateMadeByCampaignMode
                        ? _campaignService.DoesInteractedCommentsExist(objFdPostCommentDetails.CommentUrl, _ActivityType)
                        : _dbAccountService.DoesInteractedCommentsExist(objFdPostCommentDetails.CommentId, _ActivityType);
                }

            }

            return false;
        }

        /*
        protected bool AlreadyInteractedCampaignUser(FacebookUser objFacebookUser)
        {
            if (_campaignService.DoesInteractedUserExist(objFacebookUser.UserId, ActivityType))
                return true;
            if (objFacebookUser.IsAlreadyFriend && ActivityType == ActivityType.SendFriendRequest)
            {
                return true;
            }
            return false;
        }
    */

        protected bool AlreadyInteractedPages(FanpageDetails objFanpageDetails, DominatorAccountModel accountModel)
            => (_ActivityType == ActivityType.PlaceScraper && _campaignService.DoesInteractedPagesExist(objFanpageDetails.FanPageID, accountModel.AccountBaseModel.UserName, _ActivityType))
                || (_ActivityType != ActivityType.PlaceScraper && _dbAccountService.DoesInteractedPagesExist(objFanpageDetails.FanPageID, _ActivityType));

        protected bool AlreadyInteractedPagesCustom(FanpageDetails objFanpageDetails, DominatorAccountModel accountModel)
            => (_ActivityType == ActivityType.PlaceScraper && _campaignService.DoesInteractedPagesExist(objFanpageDetails.FanPageUrl, accountModel.UserName, _ActivityType))
                || (_ActivityType != ActivityType.PlaceScraper && _dbAccountService.DoesInteractedPagesExistCustom(objFanpageDetails.FanPageUrl, _ActivityType));

        protected bool AlreadyInteractedGroups(GroupDetails objGroupDetails)
            => (_dbAccountService.DoesInteractedGroupsExist(objGroupDetails.GroupUrl, _ActivityType))
                || (objGroupDetails.GroupJoinStatus == "Member" && _ActivityType == ActivityType.GroupJoiner);

        protected bool AlreadyInteractedPosts(FacebookPostDetails objFacebookPostDetails)
        {
            if (_ActivityType == ActivityType.PostCommentor && _activitySettings.IschkAllowMultipleComment
                && _dbAccountService.DoesInteractedPostsExist(objFacebookPostDetails.Id, objFacebookPostDetails.PostUrl,
                _ActivityType, false))
                return true;
            else if (_ActivityType == ActivityType.PostCommentor && !_activitySettings.IschkAllowMultipleComment &&
                _dbAccountService.DoesInteractedPostsExist(objFacebookPostDetails.Id, objFacebookPostDetails.PostUrl,
                _ActivityType))
                return true;
            else if (_ActivityType == ActivityType.PostScraper &&
                _campaignService.DoesInteractedPostsExist(objFacebookPostDetails.Id, objFacebookPostDetails.PostUrl, _ActivityType))
                return true;
            else if (_ActivityType == ActivityType.PostLiker &&
                     _dbAccountService.DoesInteractedPostsExist(objFacebookPostDetails.Id,
                         objFacebookPostDetails.PostUrl, _ActivityType))
                return true;
            return false;
        }


        protected bool CheckUserUniqueNess(JobProcessResult jobProcessResult, FacebookUser objFacebookUser
            , ActivityType activityType)
        {
            if (_moduleConfiguration.IsTemplateMadeByCampaignMode && JobProcess.ModuleSetting.IschkUniqueRequest)
            {
                try
                {
                    var fdCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                    fdCampaignInteractionDetails.AddInteractedData(SocialNetworks.Facebook, JobProcess.CampaignId,
                        objFacebookUser.UserId);
                }
                catch (Exception)
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    return false;
                }
            }
            return true;
        }

        public List<FacebookUser> CheckBlacklistUser(List<FacebookUser> lstFacebookUser)
        {
            if (_ActivityType != ActivityType.ProfileScraper)
            {
                if (JobProcess.BlackListWhitelistHandler == null)
                    JobProcess.BlackListWhitelistHandler = new BlackListWhitelistHandler
                        (JobProcess.ModuleSetting, AccountModel, _ActivityType);

                if (JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers &&
                    (JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList ||
                     JobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUsePrivateWhiteList))

                    lstFacebookUser = JobProcess.BlackListWhitelistHandler.SkipWhiteListUsers(lstFacebookUser);

                if (JobProcess.ModuleSetting.SkipBlacklist.IsSkipBlackListUsers &&
                    (JobProcess.ModuleSetting.SkipBlacklist.IsSkipGroupBlackListUsers ||
                     JobProcess.ModuleSetting.SkipBlacklist.IsSkipPrivateBlackListUser))
                    lstFacebookUser = JobProcess.BlackListWhitelistHandler.SkipBlackListUsers(lstFacebookUser);

            }

            return lstFacebookUser;
        }

        protected bool AlreadyInteractedEvents(string eventId) =>
            _dbAccountService.DoesInteractedEventsExist(eventId, _ActivityType);

        public static readonly ConcurrentDictionary<string, object> LockObjects = new ConcurrentDictionary<string, object>();

        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postUrl, [NotNull] CampaignDetails campaignDetails)
        {
            if (campaignDetails == null) throw new ArgumentNullException(nameof(campaignDetails));
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            campaignDetails = campaignFileManager.GetCampaignById(JobProcess.CampaignId);

            if (campaignDetails != null)
            {
                try
                {
                    JobProcess.AddedToDb = false;
                    #region Action From Random Percentage Of Accounts
                    if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock1" + postUrl, new object());
                        lock (lockObject)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Facebook,
                                   ConstantVariable.GetCampaignDb);
                            try
                            {
                                decimal count = campaignDetails.SelectedAccountList.Count;
                                var randomMaxAccountToPerform = (int)Math.Round(count * JobProcess.ModuleSetting.PerformActionFromRandomPercentage.GetRandom() / 100);

                                var numberOfAccountsAlreadyPerformedAction = _campaignService.GetInteractedPosts(_ActivityType).Where(x => x.PostUrl == postUrl).ToList();

                                if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction.Count)
                                    return false;

                                AddPendingActivityValueToDb(queryInfo, postUrl, dbOperation);
                                JobProcess.AddedToDb = true;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }

                    }
                    #endregion

                    #region Delay Between action On SamePost
                    if (JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock2" + postUrl, new object());
                        lock (lockObject)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Instagram,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                List<int> recentlyPerformedActions;
                                recentlyPerformedActions = _campaignService.GetInteractedPosts(_ActivityType).
                                    Where(x => x.PostUrl == postUrl && (x.Status == "Success" || x.Status == "Working")).
                                    OrderByDescending(x => x.InteractionTimeStamp).Select(x => x.InteractionTimeStamp)
                                    .Take(1).ToList();

                                if (recentlyPerformedActions.Count > 0)
                                {
                                    var recentlyPerformedTime = recentlyPerformedActions[0];
                                    var delay = JobProcess.ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                                    var time = DateTimeUtilities.GetEpochTime();
                                    var time2 = recentlyPerformedTime + delay;
                                    if (time < time2)
                                    {
                                        Thread.Sleep((time2 - time) * 1000);// Thread.Sleep((time2 - time) * 1000);
                                    }
                                }
                                if (!JobProcess.AddedToDb)
                                    AddWorkingActivityValueToDb(queryInfo, postUrl, dbOperation);
                                else
                                {
                                    var interactedPost = dbOperation.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                                            x => x.Permalink == postUrl && x.ActivityType == _ActivityType &&
                                                 x.Username == AccountModel.AccountBaseModel.UserName && (x.Status == "Pending" || x.Status == "Working"));
                                    interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                                    interactedPost.Status = "Working";
                                    dbOperation.Update(interactedPost);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }

                    }
                    #endregion
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException(@"Cancellation Requested !");
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        protected void AddPendingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            dbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
            {
                ActivityType = _ActivityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                Username = AccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                Permalink = postPermalink,
                Status = "Pending"
            });
        }

        protected void AddWorkingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            dbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
            {
                ActivityType = _ActivityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                Username = AccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                Permalink = postPermalink,
                Status = "Working"
            });
        }

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, FacebookPostDetails post)
        {
            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[AccountModel.AccountId, _ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (JobProcess.ModuleSetting.IschkUniquePostForCampaign)
                {
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Facebook, $"{JobProcess.CampaignId}.post", post.Id);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
                }
                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                {
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Facebook, JobProcess.CampaignId, post.OwnerId);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
                }

            }

            if (JobProcess.ModuleSetting.IschkUniqueUserForAccount)
            {
                try
                {
                    if (_ActivityType == ActivityType.PostLiker || _ActivityType == ActivityType.PostLiker)
                    {
                        if ((_dbAccountService.GetInteractedPosts(_ActivityType).Where(x => x.OwnerId == post.OwnerId)).Any())
                            return false;
                    }
                    else
                    {
                        if ((_dbAccountService.GetInteractedPosts(_ActivityType).Where(x => x.OwnerId == post.OwnerId)).Any())
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        public void AddLocationFilterValues()
        {
            if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsLocationFilterChecked
                || JobProcess.ModuleSetting.GenderAndLocationFilter.IsLocationFilterChecked)
            {
                foreach (var locationPair in JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrlPair)
                    JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Add(locationPair.Value);
                foreach (var locationPair in JobProcess.ModuleSetting.GenderAndLocationCancelFilter.ListLocationUrlPair)
                    JobProcess.ModuleSetting.GenderAndLocationCancelFilter.ListLocationUrl.Add(locationPair.Value);
                JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl
                = JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Distinct().ToList();
                JobProcess.ModuleSetting.GenderAndLocationCancelFilter.ListLocationUrl
                    = JobProcess.ModuleSetting.GenderAndLocationCancelFilter.ListLocationUrl.Distinct().ToList();
            }
        }
    }
}
