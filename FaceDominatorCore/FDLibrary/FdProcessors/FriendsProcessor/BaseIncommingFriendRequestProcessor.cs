using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.FriendsProcessor
{
    public class BaseIncommingFriendRequestProcessor : BaseFbProcessor
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        public BaseIncommingFriendRequestProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        protected void FilterAndStartFinalProcessForCancelRequest(JobProcessResult jobProcessResult, string queyType, string className, string paginationClassName)
        {

            IResponseHandler incomingFriendListResponseHandler = null;

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                if (AccountModel.IsRunProcessThroughBrowser)
                    Browsermanager.SearchByFriendRequests(AccountModel, FbEntityType.IncommingFriendRequests);

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {

                    incomingFriendListResponseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.IncommingFriendRequests, 50, 0, className, paginationClassName)
                        : ObjFdRequestLibrary.GetIncomingFriendRequests(AccountModel, incomingFriendListResponseHandler);

                    if (incomingFriendListResponseHandler.Status)
                    {
                        List<FacebookUser> lstFriendIds = incomingFriendListResponseHandler.ObjFdScraperResponseParameters.ListUser;

                        GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, lstFriendIds.Count, "LangKeyAcceptRequest".FromResourceDictionary(), "",
                            _ActivityType);

                        if (lstFriendIds.Count == 0)
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        AppplyPostFilterAndStartFinalProcessForCancel(ref jobProcessResult, lstFriendIds, queyType);
                        jobProcessResult.maxId = incomingFriendListResponseHandler.PageletData;

                        jobProcessResult.HasNoResult = !incomingFriendListResponseHandler.HasMoreResults;
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Requested Cancelled !");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

        }

        private bool IsCancelIncommingFilterApplied()
        {
            try
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                    return true;

                if (_ActivityType == ActivityType.Unfriend ||
                    _ActivityType == ActivityType.IncommingFriendRequest)
                {
                    if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsFilterByGender
                        || JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsLocationFilterChecked)
                        return true;

                    if ((JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendFilterChecked
                       && JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsNoOfMutualFriend &&
                       JobProcess.ModuleSetting.GenderAndLocationCancelFilter.TotalNoOfMutualFriend >= 0) ||
                       (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendFilterChecked &&
                       JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendFilterChecked
                       && JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsFriendOfFriend &&
                       JobProcess.ModuleSetting.GenderAndLocationCancelFilter.ListFriends.Count > 0) ||
                       (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendFilterChecked &&
                      JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendsCountFilterSelected
                            && JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsNoOfMutualFriendSmallerThan &&
                       JobProcess.ModuleSetting.GenderAndLocationCancelFilter.TotalNoOfMutualFriendSmallerThan > 0))
                        return true;

                    else
                        return false;
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }


        private bool ApplyCancelIncommingFilter(FacebookUser user)
        {

            if (_ActivityType == ActivityType.Unfriend)
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(user.InteractionDate, out interactionDate)
                        && _ActivityType == ActivityType.Unfriend)
                    {
                        var hours = (DateTime.Now - interactionDate).Hours;
                        var days = (DateTime.Now - interactionDate).Days;

                        if (days < JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore)
                            return true;

                        else if (days == JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours < JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                            return true;

                    }
                }
            }
            if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsFilterByGender &&
               (_ActivityType == ActivityType.Unfriend || _ActivityType == ActivityType.IncommingFriendRequest))
            {
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                   && user.Gender != Gender.Male.ToString())
                    return true;

                else if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                    && user.Gender == Gender.Female.ToString())
                    return true;

            }

            if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsLocationFilterChecked &&
                (_ActivityType == ActivityType.Unfriend || _ActivityType == ActivityType.IncommingFriendRequest))
            {
                JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.RemoveAll(x => string.IsNullOrEmpty(x));

                if (string.IsNullOrEmpty(user.Currentcity) && string.IsNullOrEmpty(user.Hometown))
                    return true;

                if (!JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Any(x =>
                    System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{user.Currentcity.Replace(",", " ").Split(' ').FirstOrDefault()}\b"
                    , System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                    System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{user.Hometown.Replace(",", " ").Split(' ').FirstOrDefault()}\b"
                    , System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                    return true;

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.FirstOrDefault
                (z => user.Currentcity.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))
                || user.Hometown.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))) == null)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType.IncommingFriendRequest, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                    return true;
                }

            }

            if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendFilterChecked &&
                _ActivityType == ActivityType.IncommingFriendRequest)
            {

                List<FacebookUser> mutualFriendList = AccountModel.IsRunProcessThroughBrowser
                    ? Browsermanager.GetMutualFriends(AccountModel, user.UserId)
                    : ObjFdRequestLibrary.GetAllMutualFriends(AccountModel, user.UserId);

                if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendsCountFilterSelected && JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsNoOfMutualFriend)
                    if (mutualFriendList.Count <= JobProcess.ModuleSetting.GenderAndLocationCancelFilter.TotalNoOfMutualFriend)
                        return true;

                if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsMutualFriendsCountFilterSelected && JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsNoOfMutualFriendSmallerThan)
                    if (mutualFriendList.Count >= JobProcess.ModuleSetting.GenderAndLocationCancelFilter.TotalNoOfMutualFriendSmallerThan)
                        return true;

                if (JobProcess.ModuleSetting.GenderAndLocationCancelFilter.IsFriendOfFriend)
                {
                    if (mutualFriendList.FirstOrDefault
                        (x => JobProcess.ModuleSetting.GenderAndLocationCancelFilter.ListFriends.Any(y => x.UserId == y)) == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void AppplyPostFilterAndStartFinalProcessForCancel(ref JobProcessResult jobProcessResult, List<FacebookUser> lstFriendIds, string queryType)
        {

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            foreach (var post in lstFriendIds)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    bool isIgnore = false;

                    if (CheckDuplicateDataForUsers(post))
                        continue;

                    IResponseHandler _userInfoResponseHandler = null;

                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{post.UserId}"]
                              .Resolve<IFdBrowserManager>();
                        _userInfoResponseHandler = userSpecificWindow.GetFullUserDetails(AccountModel, post);

                        userSpecificWindow.CloseBrowser(AccountModel);
                    }
                    else
                        _userInfoResponseHandler =
                        ObjFdRequestLibrary.GetDetailedInfoUserMobile(post, AccountModel, false, true);

                    if (IsCancelIncommingFilterApplied())
                        isIgnore = ApplyCancelIncommingFilter(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!isIgnore)
                    {

                        FilterAndStartFinalProcessForEachFriend(out jobProcessResult,
                            post, queryType);

                        if (JobProcess.IsStopped())
                            break;

                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with profileUrl {_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.ProfileUrl} dosent match with filter condition");
                        Thread.Sleep(10000);
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Requested Cancelled !");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private void FilterAndStartFinalProcessForEachFriend(out JobProcessResult jobProcessResult, FacebookUser user, string queryType)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                QueryInfo query = new QueryInfo { QueryType = queryType };

                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                {
                    ResultUser = user,
                    QueryInfo = query
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }

        }

        private void AppplyPostFilterAndStartFinalProcessForAccept(ref JobProcessResult jobProcessResult, List<FacebookUser> listIncomingFriendId, string queryType)
        {

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            int nullAccount = 0;

            foreach (var post in listIncomingFriendId)
            {
                try
                {

                    bool isIgnore = false;

                    if (CheckDuplicateDataForUsers(post))
                        continue;

                    IResponseHandler _userInfoResponseHandler = null;

                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{post.UserId}"]
                              .Resolve<IFdBrowserManager>();
                        _userInfoResponseHandler = userSpecificWindow.GetFullUserDetails(AccountModel, post);

                        userSpecificWindow.CloseBrowser(AccountModel);
                    }
                    else
                        _userInfoResponseHandler =
                        ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper(post, AccountModel, false, true);

                    if (string.IsNullOrEmpty(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.UserId))
                    {
                        nullAccount++;
                        if (nullAccount >= 10)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType,
                                "LangKeyActivityBlockedByFacebook".FromResourceDictionary());
                            JobProcess.Stop();
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType,
                                "LangKeyUnableToScrapUserDetails".FromResourceDictionary());
                            continue;
                        }
                    }

                    if (IsUnfriendFriendFilterApplied())
                        isIgnore = ApplyUnfriendFilter(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (!isIgnore)
                    {
                        FilterAndStartFinalProcessForEachFriend(out jobProcessResult,
                            _userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser, queryType);

                        if (JobProcess.IsStopped())
                            break;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            string.Format("LangKeyUserDosentMatchWithFilter".FromResourceDictionary(), $"{_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.ProfileUrl}"));
                        Thread.Sleep(5000);
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Requested Cancelled !");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private bool IsUnfriendFriendFilterApplied()
        {
            try
            {

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (_ActivityType == ActivityType.Unfriend)
                {
                    if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                        return true;
                }


                if (_ActivityType == ActivityType.Unfriend || _ActivityType == ActivityType.IncommingFriendRequest
                    || _ActivityType == ActivityType.ProfileScraper || _ActivityType == ActivityType.SendFriendRequest
                    || _ActivityType == ActivityType.WithdrawSentRequest || _ActivityType == ActivityType.BroadcastMessages)
                {
                    if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFilterByGender
                        || JobProcess.ModuleSetting.GenderAndLocationFilter.IsLocationFilterChecked)
                        return true;
                    if ((JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked
                       && JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriend &&
                       JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriend >= 0) ||
                       (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked
                       && JobProcess.ModuleSetting.GenderAndLocationFilter.IsFriendOfFriend &&
                       JobProcess.ModuleSetting.GenderAndLocationFilter.ListFriends.Count > 0) ||
                       (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked
                       && JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriendSmallerThan &&
                       JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriendSmallerThan > 0))
                        return true;

                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        private bool ApplyUnfriendFilter(FacebookUser user)
        {

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_ActivityType == ActivityType.Unfriend)
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(user.InteractionDate, out interactionDate)
                        && _ActivityType == ActivityType.Unfriend)
                    {
                        var hours = (DateTime.Now - interactionDate).Hours;
                        var days = (DateTime.Now - interactionDate).Days;

                        if (days < JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore)
                        {
                            return true;
                        }
                        else if (days == JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours < JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                        {
                            return true;
                        }
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                }
            }


            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFilterByGender)
            {
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser &&
                    JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser &&
                    (user.Gender != Gender.Male.ToString() || user.Gender != Gender.Female.ToString()))
                {
                    return false;
                }
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && user.Gender != Gender.Male.ToString())
                {
                    return true;
                }
                else if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                    && user.Gender != Gender.Female.ToString())
                {
                    return true;
                }

            }

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsLocationFilterChecked)
            {
                JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.RemoveAll(x => string.IsNullOrEmpty(x));

                if (string.IsNullOrEmpty(user.Currentcity) && string.IsNullOrEmpty(user.Hometown))
                    return true;

                if (!JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Any(x =>
                        System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{user.Currentcity.Replace(",", " ").Split(' ').FirstOrDefault()}\b"
                        , System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                        System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{user.Currentcity.Replace(",", " ").Split(' ').LastOrDefault().Trim()}\b"
                        , System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                        System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{user.Hometown.Replace(",", " ").Split(' ').FirstOrDefault()}\b"
                        , System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                        System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{user.Hometown.Replace(",", " ").Split(' ').LastOrDefault().Trim()}\b"
                        , System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                    return true;

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.FirstOrDefault
                (z => !user.Currentcity.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))
                || !user.Hometown.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))) == null)
                    return true;

            }

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked &&
                _ActivityType == ActivityType.IncommingFriendRequest)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var mutualFriendList = ObjFdRequestLibrary.GetAllMutualFriends(AccountModel, user.UserId);

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendsCountFilterSelected && JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriend)
                {
                    if (mutualFriendList.Count <= JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriend)
                    {
                        return true;
                    }
                }
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendsCountFilterSelected && JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriendSmallerThan)
                {
                    if (mutualFriendList.Count >= JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriendSmallerThan)
                    {
                        return true;
                    }
                }
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFriendOfFriend)
                {
                    if (mutualFriendList.FirstOrDefault
                        (x => JobProcess.ModuleSetting.GenderAndLocationFilter.ListFriends.Any(y => x.UserId == y)) == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void FilterAndStartFinalProcessForAcceptRequest(JobProcessResult jobProcessResult, string queyType, string className, string paginationClassName)
        {

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            IResponseHandler objIncomingFriendListResponseHandler = null;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByFriendRequests(AccountModel, FbEntityType.IncommingFriendRequests);

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    objIncomingFriendListResponseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.IncommingFriendRequests, 5, 0, className, paginationClassName)
                        : ObjFdRequestLibrary.GetIncomingFriendRequests(AccountModel, objIncomingFriendListResponseHandler);

                    if (objIncomingFriendListResponseHandler.Status)
                    {
                        List<FacebookUser> lstFriendIds = objIncomingFriendListResponseHandler.ObjFdScraperResponseParameters.ListUser;
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, lstFriendIds.Count, "Accept Request", "",
                            _ActivityType);

                        if (lstFriendIds.Count == 0)
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }

                        AppplyPostFilterAndStartFinalProcessForAccept(ref jobProcessResult, lstFriendIds, queyType);
                        jobProcessResult.maxId = objIncomingFriendListResponseHandler.PageletData;
                        if (!objIncomingFriendListResponseHandler.HasMoreResults)
                        {
                            jobProcessResult.HasNoResult = true;
                        }
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Requested Cancelled !");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }


        public bool CheckDuplicateDataForUsers(FacebookUser facebookuser)
        {

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_ActivityType == ActivityType.SendFriendRequest)
                return bool.TryParse(facebookuser.IsAlreadyFriend, out bool isAlreadyFriend) && isAlreadyFriend ? true : _dbAccountService.DoesInteractedUserExist(facebookuser.UserId, _ActivityType);

            return ((_ActivityType == ActivityType.BroadcastMessages || _ActivityType == ActivityType.ProfileScraper)
                    && !JobProcess.ModuleSetting.IsAccountGrowthActive)
                ? _campaignService.DoesInteractedUserExist(facebookuser.UserId, _ActivityType)
                : _dbAccountService.DoesInteractedUserExist(facebookuser.UserId, _ActivityType);
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }
    }
}
