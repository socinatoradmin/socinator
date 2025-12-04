using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
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
    public class UserSearchUrlProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IGlobalInteractionDetails _ldGlobalInteractionDetails;
        private readonly IProcessScopeModel _processScopeModel;
        private BrowserWindow _browserWindow;
        private BrowserAutomationExtension automationExtension;


        public UserSearchUrlProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _ldGlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
            _processScopeModel = processScopeModel;
            _delayService = delayService;
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var keyword = queryInfo.QueryValue;
            var start = IsBrowser ? 1 : 0;
            try
            {
                var userSearchUrlParams = new UserSearchUrlParams();
                var isNotFirst = false;
                var count = 0;
                var paginationCount = 0;
                var browserPageCount = 0;
                if (LdJobProcess.ModuleSetting.IsViewProfileUsingEmbeddedBrowser)
                {
                    ViewSearchUrl(queryInfo, jobProcessResult, userSearchUrlParams);
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                var constructedActionUrl = ConstructedActionUrl(queryInfo, keyword, userSearchUrlParams,start);
                var totalResultsInSearch = 0;

                if (LdJobProcess.ModuleSetting.IsSavePagination)
                {
                    int.TryParse(GetPaginationId(queryInfo), out start);
                    if (IsBrowser && start == 0)
                        start = 1;
                }

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    // We are not saving pagination first time we hit the url we will 
                    // save next time when we paginate it because pagination id gives us next page users
                    // here we save last page id/count
                    if (LdJobProcess.ModuleSetting.IsSavePagination)
                        AddOrUpdatePaginationId(queryInfo, start.ToString(), ref isNotFirst);

                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var actionUrl = IsBrowser
                        ? browserPageCount == 1 ? constructedActionUrl : $"{constructedActionUrl}&page={browserPageCount}"
                        //: LdDataHelper.GetInstance.ActionUrl(constructedActionUrl, start.ToString());
                        : start == 0 ?constructedActionUrl : ConstructedActionUrl(queryInfo, keyword, userSearchUrlParams, start);

                    #region ResponseHandler

                    SearchLinkedinUsersResponseHandler searchLinkedinUsersResponseHandler;
                    if (IsSalesNavigatorSearchUrl(queryInfo))
                    {
                        //Check The Query Value whether it is sales Url Or Not.
                        if(queryInfo.QueryValue!=null && queryInfo.QueryType== "Sales Navigator SearchUrl" && (!queryInfo.QueryValue.Contains("savedSearchId") ||!queryInfo.QueryValue.Contains("sessionId")))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"Please Provide Valid Sales Navigator Saved Search Url.It Should Look Like Similar To {{ \" {"https://www.linkedin.com/sales/search/people?savedSearchId=50542701&sessionId=%2BywiTvMyRxCuKbx%2Be%2BKgEw%3D%3D"} \"}}");
                            return;
                        }
                        // this is because here we remove some url part for getting exact response
                        if (!IsBrowser && queryInfo.QueryValue.Contains("titleTimeScope=") &&
                            actionUrl.Contains("&decoration"))
                            actionUrl = "https://www.linkedin.com/" +
                                        Utils.GetBetween(actionUrl, "https://www.linkedin.com/", "&decoration");
                        
                        //set web request headers
                        LdFunctions.SetWenRequestparamtersForsalesUrl(queryInfo.QueryValue,true);
                        searchLinkedinUsersResponseHandler =
                            LdFunctions.SalesNavigatorLinkedinUsersFromSearchUrl(actionUrl,
                                userSearchUrlParams.SessionId);

                        #region this is because we create Api when we not getting responseat at first time

                        if (!searchLinkedinUsersResponseHandler.Success &&
                            queryInfo.QueryValue.Contains("titleTimeScope="))
                        {
                            LdFunctions.SetWenRequestparamtersForsalesUrl(queryInfo.QueryValue);
                            var lastPartOfUrl =
                                $"&decoration{Utils.GetBetween($"{constructedActionUrl}\\", "&decoration", "\\")}";
                            var getTrackingParam = Utils.GetBetween(constructedActionUrl, "trackingParam:", ")");
                            var searchId = Regex.Replace(
                                Utils.GetBetween(constructedActionUrl, "recentSearchParam", ")"), "[^0-9]", "");
                            constructedActionUrl =
                                $"https://www.linkedin.com/sales-api/salesApiPeopleSearch?q=recentSearches&recentSearchId={searchId}&start=0&count=25&trackingParam={getTrackingParam}){lastPartOfUrl}"
                                    .Replace("count=25", "count=40");
                            actionUrl = LdDataHelper.GetInstance.ActionUrl(constructedActionUrl, start.ToString());
                            searchLinkedinUsersResponseHandler =
                                LdFunctions.SalesNavigatorLinkedinUsersFromSearchUrl(actionUrl,
                                    userSearchUrlParams.SessionId);
                        }

                        #endregion

                        //therefore again creating api url

                        #region sometimes encoding of companyName required in GetNewSalesNavApiPeoplesType

                        if (!IsBrowser && start <= 1 && !searchLinkedinUsersResponseHandler.Success)
                        {
                            _apiAssist.IsEncodeCompanyName = true;
                            constructedActionUrl = GetConstructedApiSalesNavigatorSearch(queryInfo.QueryValue);
                            actionUrl = LdDataHelper.GetInstance.ActionUrl(constructedActionUrl, start.ToString());
                            searchLinkedinUsersResponseHandler =
                                LdFunctions.SalesNavigatorLinkedinUsersFromSearchUrl(actionUrl,
                                    userSearchUrlParams.SessionId);
                        }

                        #endregion

                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        _delayService.ThreadSleep(5000);
                    }
                    else
                    {
                        searchLinkedinUsersResponseHandler =
                            LdFunctions.SearchForLinkedinUsers(actionUrl,
                                userSearchUrlParams.DFlagship3SearchSrpPeople);


                        //Someone's Connections User's Scrapping Here..
                        if (!IsBrowser && queryInfo.QueryValue.Contains("connectionOf="))
                        {
                            var decodedUrl = WebUtility.UrlDecode(queryInfo.QueryValue);
                            var profileId = Utils.GetBetween(decodedUrl, "[\"", "\"]");
                            var queryParameter = $"(key:connectionOf,value:List({profileId}))";
                            queryParameter += decodedUrl.Contains("currentCompany") ?$",(key:currentCompany,value:List({Utils.GetBetween(decodedUrl, "currentCompany=[\"", "\"]")?.Replace("\"","")}))": "";
                            queryParameter += decodedUrl.Contains("geoUrn") ? $",(key:geoUrn,value:List({Utils.GetBetween(decodedUrl, "geoUrn=[\"", "\"]")?.Replace("\"", "")}))" : "";
                            queryParameter += decodedUrl.Contains("network") ? $",(key:network,value:List({Utils.GetBetween(decodedUrl, "network=[\"", "\"]")?.Replace("\"", "")}))" : "";
                            queryParameter += ",(key:resultType,value:List(PEOPLE))";
                            var UsersAPI = LdConstants.GetSomeonesConnectionUsersAPI(queryParameter, paginationCount);
                            searchLinkedinUsersResponseHandler = LdFunctions.SearchForLinkedinUsers(UsersAPI,
                                userSearchUrlParams.DFlagship3SearchSrpPeople);
                        }
                        // for non english language we are not getting reponse from api
                        if (!IsBrowser && start == 0 && !searchLinkedinUsersResponseHandler.Success)
                        {
                            IsBrowser = true;
                            ++start;
                            constructedActionUrl = ConstructedActionUrl(queryInfo, keyword, userSearchUrlParams,start);
                            actionUrl = IsBrowser
                                ? start == 1 ? constructedActionUrl : $"{constructedActionUrl}&page={start}"
                                : LdDataHelper.GetInstance.ActionUrl(constructedActionUrl, start.ToString());

                            searchLinkedinUsersResponseHandler = LdFunctions.SearchForLinkedinUsers(actionUrl,
                                userSearchUrlParams.DFlagship3SearchSrpPeople);
                        }
                    }
                    if (start <= 1 || count <= 0)
                    {
                        totalResultsInSearch = ShowTotalResultsInSearch(searchLinkedinUsersResponseHandler);
                        count++;
                    }
                    if (searchLinkedinUsersResponseHandler?.InValidLinkedInUserCount > 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Successfully Skipped {searchLinkedinUsersResponseHandler.InValidLinkedInUserCount} LinkedIn Users To Which We Can't Perform {ActivityType}");
                        if (searchLinkedinUsersResponseHandler.UsersList.Count == 0)
                        {
                            jobProcessResult.HasNoResult = true;
                            return;
                        }
                    }
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (searchLinkedinUsersResponseHandler.Success)
                    {
                        var resultsInCurrentPage = 0;
                        if (searchLinkedinUsersResponseHandler.UsersList.Count > 0)
                        {
                            paginationCount+=resultsInCurrentPage = searchLinkedinUsersResponseHandler.UsersList.Count;
                            FilterUsers(searchLinkedinUsersResponseHandler, userSearchUrlParams);

                            if (searchLinkedinUsersResponseHandler.UsersList.Count > 0)
                                ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult,
                                    searchLinkedinUsersResponseHandler.UsersList);
                            else if (totalResultsInSearch > resultsInCurrentPage)
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "Sorry! No Unique User Found In This Page. Navigating To Next Page...");
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

                            if (searchLinkedinUsersResponseHandler.HasMoreResults && IsBrowser)
                            {
                                jobProcessResult.HasNoResult = true;
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "we have reached end of the search page");
                                break;
                            }

                            if ((start >= 1000 || IsBrowser && start >= 100) &&
                                searchLinkedinUsersResponseHandler.HasMoreResults)
                            {
                                jobProcessResult.HasNoResult = true;
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "we have reached 100 pages from this account");
                                break;
                            }

                            start = IsBrowser ? browserPageCount++ : paginationCount;
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
                        jobProcessResult.HasNoResult = true;
                    }

                    #endregion
                }

                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }

        private int ShowTotalResultsInSearch(SearchLinkedinUsersResponseHandler searchLinkedinUsersResponseHandler)
        {
            var totalResultsInSearch = 0;

            if (!string.IsNullOrEmpty(searchLinkedinUsersResponseHandler.TotalResultsInSearch))
            {
                int.TryParse(searchLinkedinUsersResponseHandler.TotalResultsInSearch,out totalResultsInSearch);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "total results in search = " + (totalResultsInSearch > 0 ? totalResultsInSearch.ToString() : searchLinkedinUsersResponseHandler.TotalResultsInSearch));
            }

            return totalResultsInSearch;
        }


        private void ViewSearchUrl(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            UserSearchUrlParams userSearchUrlParams)
        {
            try
            {
                if (_browserWindow == null)
                {
                    automationExtension = new BrowserAutomationExtension(_browserWindow);
                    _browserWindow = automationExtension.ViewProfileBrowserInitializing(DominatorAccountModel,
                        queryInfo.QueryValue, out _);
                }

                var pageCount = 1;
                var totalResultsInSearch = 0;
                var resultsInCurrentPage = 0;

                do
                {
                    if (pageCount > 1)
                        automationExtension.ScrollAndSaveCurrentPage();

                    var searchLinkedinUsersResponseHandler =
                        new SearchLinkedinUsersResponseHandler(
                            new ResponseParameter {Response = _browserWindow.GetPageSource()},
                            ActivityType == ActivityType.SalesNavigatorUserScraper, "");

                    if (pageCount == 1)
                        totalResultsInSearch = ShowTotalResultsInSearch(searchLinkedinUsersResponseHandler);

                    FilterUsers(searchLinkedinUsersResponseHandler, userSearchUrlParams);
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

        private void FilterUsers(SearchLinkedinUsersResponseHandler searchLinkedinUsersResponseHandler,
            UserSearchUrlParams userSearchUrlParams)
        {
            #region  Filter Already Interacted

            GlobalFilter(searchLinkedinUsersResponseHandler);
            var SearchedUsers = searchLinkedinUsersResponseHandler.UsersList.Count;
            // removing already interacted users
            RemoveOrSkipAlreadyInteractedLinkedInUsers(searchLinkedinUsersResponseHandler.UsersList);
            var AfterSkipInteractedUser = searchLinkedinUsersResponseHandler.UsersList.Count;
            #endregion
            var TotalSkippedUsers = SearchedUsers - AfterSkipInteractedUser;
            if(TotalSkippedUsers > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Successfully Skipped {TotalSkippedUsers} Already Interacted Users From This Page.");
            if (userSearchUrlParams.IsCheckedFilterProfileImageCheckbox)
            {
                searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x =>
                    x.ProfilePicUrl == "N/A" || string.IsNullOrEmpty(x.ProfilePicUrl));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "successfully filtered users having no profile picture");
            }

            // this filter part not work for sales Navigator since we didn't get its PublicIdentifier
            if (userSearchUrlParams.IsChkSkipBlackListedUser &&
                (userSearchUrlParams.IsChkPrivateBlackList || userSearchUrlParams.IsChkGroupBlackList))
                FilterBlacklistedUsers(searchLinkedinUsersResponseHandler.UsersList,
                    userSearchUrlParams.IsChkPrivateBlackList,
                    userSearchUrlParams.IsChkGroupBlackList);
            var FilteredBlackListedUsers =AfterSkipInteractedUser-searchLinkedinUsersResponseHandler.UsersList.Count;
            if(FilteredBlackListedUsers > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Successfully Skipped {FilteredBlackListedUsers} BlackListed Users.");
        }

        private void GlobalFilter(SearchLinkedinUsersResponseHandler searchLinkedinUsersResponseHandler)
        {
            if (ActivityType == ActivityType.ConnectionRequest)
                try
                {
                    // it will only work for normal account only not for sales profile url 
                    // since we are using profile url it will work for sales if it is saved in sales profileUrl
                    var value = _ldGlobalInteractionDetails[SocialNetworks.LinkedIn, ActivityType];
                    if (ActivityType == ActivityType.ConnectionRequest && value != null)
                        searchLinkedinUsersResponseHandler.UsersList.RemoveAll(x =>
                            value.InteractedData.Any(y => y.Key == x.ProfileUrl));
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        private string ConstructedActionUrl(QueryInfo queryInfo, string keyword,
            UserSearchUrlParams userSearchUrlParams,int PaginationCount=0)
        {
            #region ConnectionRequest and UserScraper

            // filter if not a first connection
            // getting constructed action url
            string constructedActionUrl;
            if (IsSalesNavigatorSearchUrl(queryInfo))
            {
                var queryValue = queryInfo.QueryValue;
                var sessionId = Utils.GetBetween(queryValue+"**", "sessionId=", "**");
                var searchId = Utils.GetBetween(queryValue, "savedSearchId=", "&sessionId=");
                // api is present in pageSource get it from page source else we creating it manually
                constructedActionUrl = IsBrowser
                    ? queryInfo.QueryValue :
                    LdConstants.GetSalesUsersSearchAPI(searchId, sessionId, PaginationCount);
                //: GetSalesNavUserApi(LdFunctions, queryInfo.QueryValue, out sessionId);
                userSearchUrlParams.SessionId = sessionId;
            }


            else
            {                
                var dFlagship3SearchSrpPeople = "";
                constructedActionUrl=IsBrowser ? queryInfo.QueryValue : GetSearchUrlPeopleApi(LdFunctions, queryInfo.QueryValue, out dFlagship3SearchSrpPeople,PaginationCount);
                if (string.IsNullOrEmpty(constructedActionUrl))
                {
                    userSearchUrlParams.DFlagship3SearchSrpPeople = Get_d_flagship3_search_srp_people(keyword);
                    constructedActionUrl = GetConstructedApiLinkedinUserSearch(queryInfo.QueryValue);
                }
                userSearchUrlParams.DFlagship3SearchSrpPeople = dFlagship3SearchSrpPeople;
            }

            // getting PrivateBlackList and GroupBlackList from models
            switch (ActivityType)
            {
                case ActivityType.ConnectionRequest:
                {
                    var connectionRequestModel = _processScopeModel.GetActivitySettingsAs<ConnectionRequestModel>();
                    userSearchUrlParams.IsChkSkipBlackListedUser = connectionRequestModel.IsChkSkipBlackListedUser;
                    userSearchUrlParams.IsCheckedFilterProfileImageCheckbox = connectionRequestModel.LDUserFilterModel
                        .IsCheckedFilterProfileImageCheckbox;
                    userSearchUrlParams.IsChkPrivateBlackList = connectionRequestModel.IsChkPrivateBlackList;
                    userSearchUrlParams.IsChkGroupBlackList = connectionRequestModel.IsChkGroupBlackList;
                    break;
                }

                case ActivityType.UserScraper:
                {
                    var userScraperModel = _processScopeModel.GetActivitySettingsAs<UserScraperModel>();
                    userSearchUrlParams.IsChkSkipBlackListedUser = userScraperModel.IsChkSkipBlackListedUser;
                    userSearchUrlParams.IsCheckedFilterProfileImageCheckbox =
                        userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                    userSearchUrlParams.IsChkPrivateBlackList = userScraperModel.IsChkPrivateBlackList;
                    userSearchUrlParams.IsChkGroupBlackList = userScraperModel.IsChkGroupBlackList;
                    break;
                }

                //sales navigator user scraper
                case ActivityType.SalesNavigatorUserScraper:
                {
                    var userScraperModel = _processScopeModel
                        .GetActivitySettingsAs<LDModel.SalesNavigatorScraper.UserScraperModel>();
                    userSearchUrlParams.IsChkSkipBlackListedUser = userScraperModel.IsChkSkipBlackListedUser;
                    userSearchUrlParams.IsCheckedFilterProfileImageCheckbox =
                        userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                    userSearchUrlParams.IsChkPrivateBlackList = userScraperModel.IsChkPrivateBlackList;
                    userSearchUrlParams.IsChkGroupBlackList = userScraperModel.IsChkGroupBlackList;
                }
                    break;
            }

            #endregion

            return constructedActionUrl;
        }

        public bool IsSalesNavigatorSearchUrl(QueryInfo queryInfo)
        {
            return ActivityType.Equals(ActivityType.SalesNavigatorUserScraper) ||
                   queryInfo.QueryType.Equals("Sales Navigator SearchUrl");
        }

        private class UserSearchUrlParams
        {
            public string SessionId { get; set; } = string.Empty;
            public bool IsChkPrivateBlackList { get; set; }
            public bool IsChkGroupBlackList { get; set; }
            public bool IsChkSkipBlackListedUser { get; set; }
            public bool IsCheckedFilterProfileImageCheckbox { get; set; }
            public string DFlagship3SearchSrpPeople { get; set; }
        }
    }
}