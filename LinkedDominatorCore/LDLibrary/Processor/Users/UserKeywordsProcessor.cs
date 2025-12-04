using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class UserKeywordsProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;
        private BrowserWindow _browserWindow;
        private BrowserAutomationExtension automationExtension;

        public UserKeywordsProcessor(ILdJobProcess jobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
            _delayService = delayService;
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var keyword = queryInfo.QueryValue;
            var start = IsBrowser ? 1 : 0;
            try
            {
                var constructedActionUrl = string.Empty;
                var isCheckedFilterProfileImageCheckbox = false;
                var isChkPrivateBlackList = false;
                var isChkSkipBlackListedUser = false;
                var isChkGroupBlackList = false;
                var isViewProfile = false;
                var isNotFirst = false;
                var dFlagship3SearchSrpPeople = string.Empty;


                switch (ActivityType)
                {
                    case ActivityType.ConnectionRequest:
                    case ActivityType.UserScraper:
                    {
                        #region MyRegion

                        if (!IsBrowser)
                            dFlagship3SearchSrpPeople = Get_d_flagship3_search_srp_people(keyword);
                        if (ActivityType == ActivityType.ConnectionRequest)
                        {
                            var connectionRequestModel =_processScopeModel.GetActivitySettingsAs<ConnectionRequestModel>();
                            isCheckedFilterProfileImageCheckbox = connectionRequestModel.LDUserFilterModel
                                .IsCheckedFilterProfileImageCheckbox;
                            isChkPrivateBlackList = connectionRequestModel.IsChkPrivateBlackList;
                            isChkSkipBlackListedUser = connectionRequestModel.IsChkSkipBlackListedUser;
                            isChkGroupBlackList = connectionRequestModel.IsChkGroupBlackList;
                        }
                        else if (ActivityType == ActivityType.UserScraper)
                        {
                            var userScraperModel =_processScopeModel.GetActivitySettingsAs<UserScraperModel>();
                            isCheckedFilterProfileImageCheckbox =
                                userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                            isChkPrivateBlackList = userScraperModel.IsChkPrivateBlackList;
                            isChkSkipBlackListedUser = userScraperModel.IsChkSkipBlackListedUser;
                            isViewProfile = userScraperModel.IsViewProfileUsingEmbeddedBrowser;
                            isChkGroupBlackList = userScraperModel.IsChkGroupBlackList;
                        }

                        #endregion

                        break;
                    }

                    case ActivityType.SalesNavigatorUserScraper:
                    {
                        var userScraperModel = _processScopeModel.GetActivitySettingsAs<LDModel.SalesNavigatorScraper.UserScraperModel>();
                        isCheckedFilterProfileImageCheckbox =
                            userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                        isChkSkipBlackListedUser = userScraperModel.IsChkSkipBlackListedUser;
                        isChkPrivateBlackList = userScraperModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = userScraperModel.IsChkGroupBlackList;
                        isViewProfile = userScraperModel.IsViewProfileUsingEmbeddedBrowser;
                        break;
                    }
                }

                constructedActionUrl = GetConstructedApiToGetLinkedinUserByKeyword(keyword, ActivityType,start);

                if (isViewProfile)
                {
                    ViewSearchUrl(queryInfo, isCheckedFilterProfileImageCheckbox, isChkSkipBlackListedUser,
                        isChkPrivateBlackList, isChkGroupBlackList);
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                if (LdJobProcess.ModuleSetting.IsSavePagination)
                {
                    int.TryParse(GetPaginationId(queryInfo), out start);
                    if (IsBrowser && start == 0)
                        start = 1;
                }

                var totalResultsInSearch = 0;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    if (LdJobProcess.ModuleSetting.IsSavePagination)
                        AddOrUpdatePaginationId(queryInfo, start.ToString(), ref isNotFirst);

                    var actionUrl = IsBrowser
                        ? start == 1 ? constructedActionUrl : $"{constructedActionUrl}&page={start}"
                        :GetConstructedApiToGetLinkedinUserByKeyword(keyword, ActivityType, start);

                    #region SearchForLinkedinUsers
                    if (ActivityType == ActivityType.SalesNavigatorUserScraper)
                        LdFunctions.SetWenRequestparamtersForsalesUrl("", true);
                    var searchLinkedinUsersResponseHandler = ActivityType == ActivityType.SalesNavigatorUserScraper
                        ? LdFunctions.SalesNavigatorLinkedinUsersByKeyword(actionUrl)
                        : LdFunctions.SearchForLinkedinUsers(actionUrl, dFlagship3SearchSrpPeople);

                    if (start <= 1)
                        totalResultsInSearch = ShowTotalResultsInSearch(searchLinkedinUsersResponseHandler);
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (searchLinkedinUsersResponseHandler.Success)
                    {
                        var resultsInCurrentPage = 0;
                        if (searchLinkedinUsersResponseHandler.UsersList.Count > 0)
                        {
                            resultsInCurrentPage = searchLinkedinUsersResponseHandler.UsersList.Count;
                            FilterUsers(searchLinkedinUsersResponseHandler, isCheckedFilterProfileImageCheckbox,
                                isChkSkipBlackListedUser, isChkPrivateBlackList, isChkGroupBlackList);

                            if (searchLinkedinUsersResponseHandler.UsersList.Count > 0)
                                ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult,
                                    searchLinkedinUsersResponseHandler.UsersList);
                            else if (totalResultsInSearch > resultsInCurrentPage)
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "sorry no unique results found navigating to next page...");
                            else if(totalResultsInSearch == resultsInCurrentPage)
                                GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"We Have Reached End Of The Page And No Unique Results Found To Perform {ActivityType} Operation.");
                        }

                        #region Pagination Logic

                        if (totalResultsInSearch > resultsInCurrentPage)
                        {
                            if (start >= totalResultsInSearch && (jobProcessResult.HasNoResult = true))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "we have reached end of the search pages from this account");
                                break;
                            }

                            if ((start >= 1000 || IsBrowser && start >= 100) && (jobProcessResult.HasNoResult = true))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "we have reached 100 pages from this account");
                                break;
                            }
                            start += IsBrowser ? 1 : 40;
                            _delayService.ThreadSleep(5000);
                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                        }

                        #endregion
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "no more results found");
                        jobProcessResult.HasNoResult = true;
                    }

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }

        private void ViewSearchUrl(QueryInfo queryInfo, bool isCheckedFilterProfileImageCheckbox,
            bool isChkSkipBlackListedUser, bool isChkPrivateBlackList, bool isChkGroupBlackList)
        {
            JobProcessResult jobProcessResult = null;

            try
            {
                if (_browserWindow == null)
                {
                    var url = ActivityType == ActivityType.SalesNavigatorUserScraper
                        ? $"https://www.linkedin.com/sales/search/people?doFetchHeroCard=true&keywords={Uri.EscapeDataString(queryInfo.QueryValue)}"
                        : $"https://www.linkedin.com/search/results/people/?keywords={Uri.EscapeDataString(queryInfo.QueryValue)}&origin=SWITCH_SEARCH_VERTICAL";
                    automationExtension = new BrowserAutomationExtension(_browserWindow);
                    _browserWindow =
                        automationExtension.ViewProfileBrowserInitializing(DominatorAccountModel, url, out _);
                    
                }

                var pageCount = 1;
                var totalResultsInSearch = 0;
                var resultsInCurrentPage = 0;

                do
                {
                    if (pageCount > 1)
                        automationExtension.ScrollAndSaveCurrentPage();
                    var pageSource = _browserWindow.GetPageSource();
                    var searchLinkedinUsersResponseHandler =
                        new SearchLinkedinUsersResponseHandler(new ResponseParameter {Response = pageSource},
                            ActivityType == ActivityType.SalesNavigatorUserScraper);

                    if (pageCount == 1)
                        totalResultsInSearch = ShowTotalResultsInSearch(searchLinkedinUsersResponseHandler);

                    FilterUsers(searchLinkedinUsersResponseHandler, isCheckedFilterProfileImageCheckbox,
                        isChkSkipBlackListedUser, isChkPrivateBlackList, isChkGroupBlackList);
                    if (searchLinkedinUsersResponseHandler.UsersList.Count > 0)
                        ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult,
                            searchLinkedinUsersResponseHandler.UsersList);
                    else if (totalResultsInSearch > resultsInCurrentPage)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "sorry no unique results found navigating to next page...");
                    ++pageCount;
                } while (pageCount < 40 && totalResultsInSearch / 25 > pageCount);

                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }


        private int ShowTotalResultsInSearch(SearchLinkedinUsersResponseHandler searchLinkedinUsersResponseHandler)
        {
            var totalResultsInSearch = 0;

            if (!string.IsNullOrEmpty(searchLinkedinUsersResponseHandler.TotalResultsInSearch))
            {
                int.TryParse(searchLinkedinUsersResponseHandler.TotalResultsInSearch?.Replace(",", ""), out totalResultsInSearch);
                if(totalResultsInSearch > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "total results in search = " + searchLinkedinUsersResponseHandler.TotalResultsInSearch + "");
                }
            }

            return totalResultsInSearch;
        }

        private void FilterUsers(
            SearchLinkedinUsersResponseHandler searchLinkedinUsersResponseHandler,
            bool isCheckedFilterProfileImageCheckbox,
            bool isChkSkipBlackListedUser, bool isChkPrivateBlackList, bool isChkGroupBlackList)
        {
            List<InteractedUsers>
                listInteractedUsersFromCampaignDb = null;
            var listInteractedUsersFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
            if (!string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                listInteractedUsersFromCampaignDb = DbCampaignService.GetInteractedUsers(ActivityTypeString).ToList();

            var ldGlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();

            #region  Filter Already Interacted

            try
            {
                var value = ldGlobalInteractionDetails[SocialNetworks.LinkedIn, ActivityType];
                if (ActivityType == ActivityType.ConnectionRequest && value != null)
                    searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x =>
                        value.InteractedData.Any(y => y.Key == x.ProfileUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (ActivityType.Equals(ActivityType.ConnectionRequest))
                searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x =>
                    listInteractedUsersFromAccountDb.Any(y => y.UserProfileUrl == x.ProfileUrl ||
                                                              !string.IsNullOrEmpty(y.ProfileId) &&
                                                              y.ProfileId == x.ProfileId));

            if (listInteractedUsersFromCampaignDb != null && listInteractedUsersFromCampaignDb.Count > 0)
                searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x =>
                    listInteractedUsersFromCampaignDb.Any(y => y.UserProfileUrl == x.ProfileUrl ||
                                                               !string.IsNullOrEmpty(y.ProfileId) &&
                                                               y.ProfileId == x.ProfileId));

            #endregion

            if (isCheckedFilterProfileImageCheckbox)
            {
                int removed=(searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x => string.IsNullOrEmpty(x.ProfilePicUrl))>0|| searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x => x.ProfilePicUrl == "N/A")>0)?1:0;
                if(removed > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,"successfully filtered users having no profile picture");
            }

            if (isChkSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList))
                FilterBlacklistedUsers(searchLinkedinUsersResponseHandler.UsersList, isChkPrivateBlackList,
                    isChkGroupBlackList);
        }
    }
}