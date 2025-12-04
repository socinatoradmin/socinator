using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDLibrary.MessengerProcesses;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.LDUtility;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.Processor
{
    public abstract class BaseLinkedinProcessor
    {
        protected BaseLinkedinProcessor(ILdJobProcess ldJobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel ProcessScopeModel)
        {
            ClassMapper = InstanceProvider.GetInstance<IClassMapper>();
            DetailsFetcher = InstanceProvider.GetInstance<IDetailsFetcher>();
            ActivityTypeString = (ActivityType = ldJobProcess.ActivityType).ToString();
            LdJobProcess = ldJobProcess;
            _moduleSetting = ldJobProcess.ModuleSetting;
            _campaignDetails = ProcessScopeModel.CampaignDetails;
            processScopeModel =(ProcessScopeModel)ProcessScopeModel;
            DbAccountService = LdJobProcess?.DbAccountService;
            if (campaignService != null)
                DbCampaignService = campaignService;
            LdFunctions = ldFunctionFactory.LdFunctions;
            manageBlacklistWhitelist = new ManageBlacklistWhitelist(DbAccountService, delayService);
            _delayService = delayService;
            IsBrowser = ldFunctionFactory.LdFunctions.IsBrowser;
            InitializeFilters();
        }


        public void Start(QueryInfo queryInfo)
        {
            try
            {
                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var jobProcessResult = new JobProcessResult();

                LdFunctions.SetJobCancellationTokenInBrowser(DominatorAccountModel,
                    LdJobProcess.JobCancellationTokenSource.Token);
                var Message = (string.IsNullOrEmpty(queryInfo?.QueryType) || string.IsNullOrEmpty(queryInfo?.QueryValue)) ? LdJobProcess.ActivityType.ToString() : $"{queryInfo?.QueryType} {queryInfo?.QueryValue}";
                if (queryInfo?.QueryType == null || queryInfo.QueryType != null &&
                    !queryInfo.QueryType.Equals(LdConstants.GroupProcessor))
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        LdJobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        LdJobProcess.DominatorAccountModel.UserName, LdJobProcess.ActivityType,
                        $"Searching for {Message}");
                Process(queryInfo, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
            }
        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        public string GetConstructedApiSalesNavigatorSearch(string desktopUrl)
        {
            try
            {
                if (desktopUrl.Contains("sales/search/people"))
                {
                    desktopUrl = _apiAssist.GetNewSalesNavApiPeoplesType(desktopUrl);
                }

                else if (desktopUrl.Contains("sales/search/companies") || desktopUrl.Contains("sales/search/company"))
                {
                    desktopUrl = _apiAssist.GetNewSalesNavApiCompanyType(desktopUrl);
                }

                else if (desktopUrl.Contains("sales/search"))
                {
                    var firstPart = Regex.Split(desktopUrl, "sales/search")[0];
                    var newFirstPart = firstPart + "sales/search/results";
                    desktopUrl = desktopUrl.Replace(firstPart + "sales/search", newFirstPart);

                    if (desktopUrl.Contains("&count="))
                    {
                        var countPart = Utils.GetBetween(desktopUrl, "&count=", "&");
                        desktopUrl = desktopUrl.Replace("&count=" + countPart, "");
                    }

                    if (desktopUrl.Contains("&start="))
                    {
                        var startPart = Utils.GetBetween(desktopUrl, "&start=", "&");
                        desktopUrl = desktopUrl.Replace("&start=" + startPart, "");
                    }

                    if (desktopUrl.Contains(" "))
                        desktopUrl = desktopUrl.Replace(" ", "%20");
                }

                return desktopUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public LinkedinUser GetUserInformation(string profileInput, bool isCheckedFilterProfileImageCheckbox)
        {
            var objLinkedinUser = new LinkedinUser();
            try
            {
                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                #region Initializations Required For This Method

                var objGetDetailedUserInfo = new GetDetailedUserInfo(_delayService);
                var profilePageSource = string.Empty;

                #endregion

                #region ProfileUrl

                var profileUrl = profileInput.Contains("<:>") ? Regex.Split(profileInput, "<:>")[0] : profileInput;

                #endregion

                if (!profileInput.Contains("<:>"))
                    profilePageSource = objGetDetailedUserInfo.GetProfilePageSource(profileUrl, LdFunctions);


                if (DetailsFetcher.UserInformation(profileInput, isCheckedFilterProfileImageCheckbox, profileUrl,
                    profilePageSource, objLinkedinUser, LdFunctions, DominatorAccountModel, ActivityType))
                    return null;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }

            return objLinkedinUser;
        }

        /// <summary>
        ///     here we filter all the user of all modules except salesNavigator, as we are not getting publicIdentifier
        ///     and we get 40 results to filter, if we  hit all and get publicIdentifier of all profiles here
        ///     account might get blocked therefore we skip it here and do filter part on final processes.
        /// </summary>
        /// <param name="lstLinkedinUser"></param>
        /// <param name="isChkPrivateBlackList"></param>
        /// <param name="isChkGroupBlackList"></param>
        public void FilterBlacklistedUsers(List<LinkedinUser> lstLinkedinUser, bool isChkPrivateBlackList,
            bool isChkGroupBlackList)
        {
            try
            {
                #region Filter Blacklisted Users

                if (isChkPrivateBlackList)
                {
                    var lstPrivateBlackListedUsers = manageBlacklistWhitelist.GetPrivateBlackListedUsers();
                    lstPrivateBlackListedUsers.ForEach(privateBlackListedUser =>
                    {
                        lstLinkedinUser.RemoveAll(y =>
                            y.ProfileUrl.Contains(privateBlackListedUser.UserName) ||
                            y.PublicIdentifier == privateBlackListedUser.UserName);
                    });
                }

                if (!isChkGroupBlackList)
                    return;

                // in send message to new followers public identifier is null
                foreach (var linkedinUser in lstLinkedinUser)
                    if (string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                        linkedinUser.PublicIdentifier =
                            _ldDataHelper.GetPublicInstanceFromProfileUrl(linkedinUser.ProfileUrl);

                
                var lstGroupBlackListedUsers = manageBlacklistWhitelist.GetGroupBlackListedUsers();

                lstGroupBlackListedUsers.ForEach(groupBlackListedUsers =>
                {
                    lstLinkedinUser.RemoveAll(y => y.PublicIdentifier == groupBlackListedUsers.UserName);
                });
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FilterBlacklistedUsers(List<Connections> lstMyConnections, bool isChkPrivateBlackList,
            bool isChkGroupBlackList)
        {
            try
            {
                #region Filter Blacklisted Users

                if (isChkPrivateBlackList)
                {
                    var lstPrivateBlackListedUsers = manageBlacklistWhitelist.GetPrivateBlackListedUsers();
                    lstPrivateBlackListedUsers.ForEach(privateBlackListedUser =>
                    {
                        lstMyConnections.RemoveAll(y => y.ProfileId == privateBlackListedUser.UserId
                        ||y.PublicIdentifier == privateBlackListedUser.UserName);
                    });
                }

                if (!isChkGroupBlackList)
                    return;
                var lstGroupBlackListedUsers = manageBlacklistWhitelist.GetGroupBlackListedUsers();

                lstGroupBlackListedUsers.ForEach(groupBlackListedUsers =>
                {
                    lstMyConnections.RemoveAll(y => y.ProfileId == groupBlackListedUsers.UserId
                    ||y.PublicIdentifier == groupBlackListedUsers.UserName);
                });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UseWhitelistedUsers(List<LinkedinUser> lstLinkedinUser, bool isChkUsePrivateWhiteList,
            bool isChkUseGroupWhiteList)
        {
            try
            {
                #region Filter Blacklisted Users

                if (isChkUsePrivateWhiteList)
                {
                    var lstPrivateWhitelist = manageBlacklistWhitelist.GetPrivateWhitelistedUsers(DbAccountService);
                    lstPrivateWhitelist.ForEach(privateWhitelistedUser =>
                    {
                        lstLinkedinUser.RemoveAll(y => y.PublicIdentifier == privateWhitelistedUser.UserName);
                    });
                }

                if (isChkUseGroupWhiteList)
                {
                    try
                    {
                        var lstGroupWhiteListUser = manageBlacklistWhitelist.GetGroupWhitelistedUsers();
                        lstGroupWhiteListUser.ForEach(groupBlackListedUsers =>
                        {
                            if ((groupBlackListedUsers.UserName.Substring(groupBlackListedUsers.UserName.Length - 1).Equals("/")))
                                groupBlackListedUsers.UserName = groupBlackListedUsers.UserName.Remove(groupBlackListedUsers.UserName.Length - 1);
                            lstGroupWhiteListUser.ForEach(groupWhitelistedUser =>
                            {
                                lstLinkedinUser.RemoveAll(y => y.PublicIdentifier == groupWhitelistedUser.UserName ||
                                                          y.ProfileUrl.Contains(groupWhitelistedUser.UserName));
                            });
                        });
                    }
                    catch (Exception)
                    {
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CloseBrowserWhenNoMoreResults()
        {
            LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel, ActivityType != ActivityType.ExportConnection);
        }

        /// <summary>
        ///     taking ActivityType:QueryType:QueryValue combination as a key
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="paginationId"></param>
        /// <param name="isToAddPagination"></param>
        /// <param name="currentUser"></param>
        protected void AddOrUpdatePaginationId(QueryInfo queryInfo, string paginationId, ref bool isToAddPagination,
            string currentUser = "")
        {
            try
            {
                if (string.IsNullOrEmpty(paginationId) || !isToAddPagination)
                {
                    isToAddPagination = true;
                    return;
                }

                if (!LdJobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var queryCombination =
                        $"{LdJobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{LdJobProcess.DominatorAccountModel.UserName}:{queryInfo.Id}";
                    SocinatorAccountBuilder.Instance(LdJobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdatePaginationId(queryCombination, paginationId).SaveToBinFile();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected string GetPaginationId(QueryInfo queryInfo, string currentUser = "")
        {
            var paginationId = string.Empty;
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var dominatorAccountModel =
                    accountsFileManager.GetAccountById(LdJobProcess.DominatorAccountModel.AccountId);
                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var queryCombination =
                        $"{LdJobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{LdJobProcess.DominatorAccountModel.UserName}:{queryInfo.Id}";

                    dominatorAccountModel.PaginationId.TryGetValue(queryCombination, out paginationId);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return paginationId;
        }

        private void InitializeFilters()
        {
            try
            {
                var userFilter = LdJobProcess.ModuleSetting.LDUserFilterModel;
                var postFilter = LdJobProcess.ModuleSetting.LDPostFilterModel;
                IsUserFilterActive =userFilter.IscheckedFilterMinimumCharacterInBio ||userFilter.IsCheckedHasInvalidWordsCheckBox ||userFilter.IsCheckedHasValidWordsCheckBox ||userFilter.IsCheckedMinimumConnectionsCheckbox ||userFilter.IsCheckedExperienceCheckbox ||userFilter.IsCheckedEducationCheckbox;
                IsPostFilterActive = postFilter.FilterAcceptedPostCaptionList || postFilter.FilterComments || postFilter.FilterLikes || postFilter.FilterPostAge || postFilter.FilterRestrictedPostCaptionList || postFilter.IgnoreAdPost || postFilter.IgnoreCommentedPosts || postFilter.IgnoreLikedPost || postFilter.IgnoreOwnPosts || postFilter.IgnoreTopPost;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region intialize

        protected readonly ILdJobProcess LdJobProcess;
        protected IDbAccountService DbAccountService { get; set; }
        protected IDbCampaignService DbCampaignService { get; set; }
        protected readonly ILdFunctions LdFunctions;
        protected ActivityType ActivityType { get; set; }
        protected string ActivityTypeString { get; set; }
        protected DominatorAccountModel DominatorAccountModel => LdJobProcess.DominatorAccountModel;
        protected bool IsUserFilterActive;
        protected bool IsPostFilterActive;
        protected readonly IClassMapper ClassMapper;

        protected readonly IDetailsFetcher DetailsFetcher;
        protected readonly ManageBlacklistWhitelist manageBlacklistWhitelist;
        private readonly LdDataHelper _ldDataHelper = LdDataHelper.GetInstance;
        protected readonly ApiAssist _apiAssist = new ApiAssist();
        protected readonly ModuleSetting _moduleSetting;
        protected readonly CampaignDetails _campaignDetails;

        protected readonly ProcessScopeModel processScopeModel;
        protected bool IsBrowser;
        private readonly IDelayService _delayService;
        private BrowserWindow _browserWindow { get; set; }

        #endregion


        #region  updated code
        public string GetSalesNavCompanyApi(ILdFunctions ldFunctions, string desktopUrl)
        {
            var apiUrl = string.Empty;
            try
            {
                var responseParameter = ldFunctions.GetInnerHttpHelper().GetRequest(desktopUrl);
                var pageText = responseParameter.Response;
                var data = Utils.GetBetween(pageText, " {\"request\":\"/sales-api/salesApiCompanySearch?q", "\",\"");
                var decodedUrl = Regex.Unescape(Regex.Replace(data, "\\\\([^u])", "\\\\$1"))
                    .Replace("&start=0&count=25", "");
                if (!string.IsNullOrEmpty(decodedUrl))
                    apiUrl = "https://www.linkedin.com/sales-api/salesApiCompanySearch?q" + decodedUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            if (string.IsNullOrEmpty(apiUrl))
            {
                var searchId = Utils.GetBetween(desktopUrl, "savedSearchId=", "&sessionId=");
                var sessionId = Utils.GetBetween(desktopUrl, "&sessionId=", "&page=");
                if (string.IsNullOrEmpty(sessionId))
                    sessionId = Utils.GetBetween(desktopUrl + "**", "sessionId=", "**");
                apiUrl = LdConstants.GetSalesCompanyScrapperAPI(searchId, sessionId);
            }
            return apiUrl;
        }

        public string GetSearchUrlPeopleApi(ILdFunctions ldFunctions, string desktopUrl,
            out string dFlagship3SearchSrpPeople,int PaginationCount = 0)
        {
            var apiUrl = dFlagship3SearchSrpPeople = string.Empty;
            var alternateDecodedUrl = string.Empty;
            var reqParams = ldFunctions.GetInnerHttpHelper().GetRequestParameter();
            try
            {
                var responseParameter = ldFunctions.GetInnerHttpHelper().GetRequest(desktopUrl);
                var pageText = responseParameter.Response;
                var data = Utils.GetBetween(pageText, "{\"request\":\"/voyager/api/search/blended", "\",\"");
                if (string.IsNullOrEmpty(data) && pageText.Contains("dash/clusters?"))
                    data = Utils.GetBetween(pageText, "{\"request\":\"/voyager/api/search/dash/clusters", "\",\"");
                var decodedUrl = Regex.Unescape(Regex.Replace(data, "\\\\([^u])", "\\\\$1")).Replace("&count=10", "")
                      .Replace("count=10", "")
                      .Replace("&start=0", "").Replace("&start=10", "");
                if (string.IsNullOrEmpty(data) && string.IsNullOrEmpty(decodedUrl))
                {
                    var decodedSearchUrl = WebUtility.UrlDecode(desktopUrl);
                    var getBetweenResult = Utils.GetBetween(pageText, "{\"request\":\"/voyager/api/graphql", "\",\"");
                    var query = GetQueryValue(decodedSearchUrl, desktopUrl, getBetweenResult);
                    var geoUrn = Utils.GetBetween(decodedSearchUrl, "geoUrn=[\"", "\"]")?.Replace("\"", "");
                    geoUrn = string.IsNullOrEmpty(geoUrn) ? "" : geoUrn.Contains("List") ? $"(key:geoUrn,value:{geoUrn})," : $"(key:geoUrn,value:List({geoUrn})),";
                    var currentCompany = Utils.GetBetween(decodedSearchUrl, "currentCompany=[\"", "\"]")?.Replace("\"", "");
                    currentCompany = string.IsNullOrEmpty(currentCompany) ? "" : currentCompany.Contains("List") ? $"(key:currentCompany,value:{currentCompany})," : $"(key:currentCompany,value:List({currentCompany})),";
                    var company = Utils.GetBetween(decodedSearchUrl, "company=", "&");
                    company = string.IsNullOrEmpty(company) ? "" : company.Contains("List") ? $"(key:company,value:{company})," : $"(key:company,value:List({company})),";
                    var pastCompany = Utils.GetBetween(decodedSearchUrl, "pastCompany=[\"", "\"]")?.Replace("\"","");
                    pastCompany = string.IsNullOrEmpty(pastCompany) ? "" : pastCompany.Contains("List") ? $"(key:pastCompany,value:{pastCompany})," : $"(key:pastCompany,value:List({pastCompany})),";
                    var connectionOf = Utils.GetBetween(decodedSearchUrl, "connectionOf=\"", "\"");
                    connectionOf = string.IsNullOrEmpty(connectionOf) ? "" : connectionOf.Contains("List") ? $"(key:connectionOf,value:{connectionOf})," : $"(key:connectionOf,value:List({connectionOf})),";
                    var contactInterest = Utils.GetBetween(decodedSearchUrl, "contactInterest=[\"", "\"]")?.Replace("\"","");
                    contactInterest = string.IsNullOrEmpty(contactInterest) ? "" : contactInterest.Contains("List") ? $"(key:contactInterest,value:{contactInterest})," : $"(key:contactInterest,value:List({contactInterest})),";
                    var firstName = Utils.GetBetween(decodedSearchUrl, "firstName=", "&");
                    firstName = string.IsNullOrEmpty(firstName) ? "" : firstName.Contains("List") ? $"(key:firstName,value:{firstName})," : $"(key:firstName,value:List({firstName})),";
                    var lastName = Utils.GetBetween(decodedSearchUrl, "lastName=", "&");
                    lastName = string.IsNullOrEmpty(lastName) ? "" : lastName.Contains("List") ? $"(key:lastName,value:{lastName})," : $"(key:lastName,value:List({lastName})),";
                    var followerOf = Utils.GetBetween(decodedSearchUrl, "followerOf=\"", "\"");
                    followerOf = string.IsNullOrEmpty(followerOf) ? "" : followerOf.Contains("List") ? $"(key:followerOf,value:{followerOf})," : $"(key:followerOf,value:List({followerOf})),";
                    var network = Utils.GetBetween(decodedSearchUrl, "network=[\"", "\"]")?.Replace("\"", "");
                    network = string.IsNullOrEmpty(network) ? "" : network.Contains("List") ? $"(key:network,value:{network})," : $"(key:network,value:List({network})),";
                    var resultType = Utils.GetBetween(decodedSearchUrl, "/results/", "/?")?.ToUpper()?.Replace("\"", "");
                    resultType = string.IsNullOrEmpty(resultType) ? "" : resultType.Contains("List") ? $"(key:resultType,value:{resultType})," : $"(key:resultType,value:List({resultType})),";
                    var industry = Utils.GetBetween(decodedSearchUrl, "industry=[\"", "\"]")?.Replace("\"", "");
                    industry = string.IsNullOrEmpty(industry) ? "" : industry.Contains("List") ? $"(key:industry,value:{industry})," : $"(key:industry,value:List({industry})),";
                    var language = Utils.GetBetween(decodedSearchUrl, "profileLanguage=[\"", "\"]")?.Replace("\"", "");
                    language = string.IsNullOrEmpty(language) ? string.Empty : language.Contains("List") ? $"(key:profileLanguage,value:{language})," : $"(key:profileLanguage,value:List({language})),";
                    var schoolFilter = Utils.GetBetween(decodedSearchUrl, "schoolFilter=[\"", "\"]")?.Replace("\"", "");
                    schoolFilter = string.IsNullOrEmpty(schoolFilter) ? string.Empty : schoolFilter.Contains("List") ? $"(key:schoolFilter,value:{schoolFilter})," : $"(key:schoolFilter,value:List({schoolFilter})),";
                    var serviceCategory = Utils.GetBetween(decodedSearchUrl, "serviceCategory=[\"", "\"]")?.Replace("\"", "");
                    serviceCategory = string.IsNullOrEmpty(serviceCategory) ? string.Empty : serviceCategory.Contains("List") ? $"(key:serviceCategory,value:{serviceCategory})," : $"(key:serviceCategory,value:List({serviceCategory})),";
                    var schoolFreeText = Utils.GetBetween(decodedSearchUrl, "schoolFreetext=\"", "\"");
                    schoolFreeText = string.IsNullOrEmpty(schoolFreeText) ? string.Empty : schoolFreeText.Contains("List") ? $"(key:schoolFreetext,value:{schoolFreeText})," : $"(key:schoolFreetext,value:List({schoolFreeText})),";
                    var titleFreeText = Utils.GetBetween(decodedSearchUrl+"*", "titleFreeText=", "*");
                    titleFreeText = string.IsNullOrEmpty(titleFreeText) ? string.Empty : titleFreeText.Contains("List") ? $"(key:title,value:{titleFreeText})," : $"(key:title,value:List({titleFreeText})),";
                    var origin = Utils.GetBetween(desktopUrl, "origin=", "&profileLanguage");
                    origin = string.IsNullOrEmpty(origin) ? Utils.GetBetween(desktopUrl, "origin=", "&sid"):origin;
                    origin = string.IsNullOrEmpty(origin) ? Utils.GetBetween(desktopUrl, "origin=", "&"):origin;
                    origin = string.IsNullOrEmpty(origin) ? "FACETED_SEARCH" : origin;
                    var queryParameter = $"{company}{connectionOf}{contactInterest}{currentCompany}{firstName}{followerOf}{geoUrn}{industry}{lastName}{network}{pastCompany}{language}{resultType}{schoolFilter}{schoolFreeText}{serviceCategory}{titleFreeText}"?.TrimEnd(',');
                    apiUrl = $"https://www.linkedin.com/voyager/api/graphql?variables=(start:{PaginationCount},origin:{origin},query:({query}flagshipSearchIntent:SEARCH_SRP,queryParameters:List({queryParameter}),includeFiltersInResponse:false))&queryId=voyagerSearchDashClusters.2e313ab8de30ca45e1c025cd0cfc6199";
                }
                
                pageText = HttpUtility.HtmlDecode(pageText);
                dFlagship3SearchSrpPeople = Utils.GetBetween(pageText, "d_flagship3_search_srp_people;", "\n");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                try
                {
                    reqParams.UserAgent = LdConstants.UserAgent;
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }
            }

            return apiUrl;
        }

        private string GetQueryValue(string decodedSearchUrl, string desktopUrl, string getBetweenResult)
        {
            var QueryValue=string.Empty;
            try
            {
                QueryValue = Utils.GetBetween(getBetweenResult, "(keywords:", ",flagshipSearchIntent:");
                if(string.IsNullOrEmpty(QueryValue))
                    QueryValue = Utils.GetBetween(desktopUrl, "keywords=", "&");
                //if (string.IsNullOrEmpty(QueryValue))
                //    QueryValue = Utils.GetBetween(decodedSearchUrl+"&", "titleFreeText=", "&");
            }
            catch { }

            return string.IsNullOrEmpty(QueryValue) ? string.Empty : $"keywords:{QueryValue},";
        }

        public string GetSearchUrlJobsApi(ILdFunctions ldFunctions, string desktopUrl,
            out string dFlagship3SearchSrpPeople)
        {
            var apiUrl = dFlagship3SearchSrpPeople = string.Empty;
            var reqParams = ldFunctions.GetInnerHttpHelper().GetRequestParameter();
            try
            {
                reqParams.UserAgent = null;
                var responseParameter = ldFunctions.GetInnerHttpHelper().GetRequest(desktopUrl);

                var pageText = responseParameter.Response;
                var data = Utils.GetBetween(pageText, "{\"request\":\"/voyager/api/search/hits", "\",\"");
                var decodedUrl = Regex.Unescape(Regex.Replace(data, "\\\\([^u])", "\\\\$1"))
                    .Replace("&count=25", "count=25")
                    .Replace("&count=0", "count=0").Replace("count=0", "count=25")
                    .Replace("&start=0", "");
                if (!string.IsNullOrEmpty(decodedUrl))
                    apiUrl = "https://www.linkedin.com/voyager/api/search/hits" + decodedUrl;
                pageText = HttpUtility.HtmlDecode(pageText);
                dFlagship3SearchSrpPeople = Utils.GetBetween(pageText, "d_flagship3_job_details;", "\"");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                try
                {
                    reqParams.UserAgent = LdConstants.UserAgent;
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }
            }

            return apiUrl;
        }

        protected void SkipUserAlreadyRecievedMessageFromSoftware(MapperModel modelClass)
        {
            // only for broadcast message
            try
            {
                if (modelClass.IsSkipUserAlreadyRecievedMessageFromSoftware)
                {
                    var SkippedUsersOrGroup = 0;
                    // here removing user from FullName and profileId
                    if (modelClass.IsGroup && !string.IsNullOrEmpty(modelClass.GroupUrlInput))
                    {
                        var groupUsers = DbAccountService.GetInteractedUsers(ActivityType.ToString())
                            .Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserFullName)).ToList();
                        SkippedUsersOrGroup = modelClass.ListUsersFromSelectedSource.RemoveAll(x =>
                            groupUsers.Any(y => !string.IsNullOrEmpty(y.Key) && y.Key == x.ProfileId|| !string.IsNullOrEmpty(y.Value) && y.Value == x.FullName));
                    }
                    else
                    // here removing user from profile url and profileId
                    {
                        var users = DbAccountService.GetInteractedUsers(ActivityType.ToString())
                            .Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserProfileUrl)).ToList();
                        SkippedUsersOrGroup = modelClass.ListUsersFromSelectedSource.RemoveAll(x =>
                            users.Any(y => !string.IsNullOrEmpty(y.Key) && y.Key == x.ProfileId || !string.IsNullOrEmpty(y.Value) && y.Value == x.ProfileUrl));

                    }
                    if (SkippedUsersOrGroup > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Skipped {SkippedUsersOrGroup} Users Sent Message From This Software");
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
        protected void SkipUserAlreadyReceivedMessageFromOutSideSoftware(MapperModel modelClass)
        {
            if (modelClass.IsSkipUserAlreadyRecievedMessageFromOutSideSoftware)
            {
                try
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                   DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Trying To Skip User Message Sent From Out Side Of Software.It May Take Bit Time");
                    var SkippedUserCount = 0;
                    var finalMessage = modelClass.LstDisplayManageMessagesModel.GetRandomItem().MessagesText;
                    var Users = modelClass.ListUsersFromSelectedSource.ToList();
                    var jsonHandler = JsonJArrayHandler.GetInstance;
                    Users.ForEach(objLinkedinUser =>
                    {
                      var conversation = new FilterMessage(_delayService);
                       var isMessageFiltered = LdFunctions.IsBrowser ? conversation.CheckConversationFromBrowser(LdFunctions,
                        objLinkedinUser, finalMessage,DominatorAccountModel, out _) : conversation.CheckConversation(LdFunctions,
                        objLinkedinUser, finalMessage,DominatorAccountModel, out _);
                        //here we are count number of sent message only  
                        //at time of sending connection we can send personal note and its taking as message
                        if (isMessageFiltered)
                            CheckForBySoftwareAndRemoveUser(modelClass, objLinkedinUser,ref SkippedUserCount);
                    });
                    if (SkippedUserCount > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                   DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successfully Skipped {SkippedUserCount} User Already Sent Message From Out Side Of Software.");
                }
                catch(Exception ex) {ex.DebugLog(ex.GetBaseException().Message);}
            }
        }

        private void CheckForBySoftwareAndRemoveUser(MapperModel modelClass, LinkedinUser objLinkedinUser, ref int SkippedUserCount)
        {
            if(modelClass.IsGroup && !string.IsNullOrEmpty(modelClass.GroupUrlInput))
            {
                var groupUsers = DbAccountService.GetInteractedUsers(ActivityType.ToString()).Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserFullName)).ToList();
                if (groupUsers!=null && groupUsers.Count >0 && !groupUsers.Any(user => user.Key == objLinkedinUser.ProfileId || user.Value == objLinkedinUser.ProfileUrl))
                {
                    modelClass.ListUsersFromSelectedSource.RemoveAt(modelClass.ListUsersFromSelectedSource.IndexOf(objLinkedinUser));
                    SkippedUserCount++;
                }
            }
            else
            {
                var users = DbAccountService.GetInteractedUsers(ActivityType.ToString()).Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserProfileUrl)).ToList();
                if (users!=null && users.Count > 0 && !users.Any(user => user.Key == objLinkedinUser.ProfileId || user.Value == objLinkedinUser.ProfileUrl))
                {
                    modelClass.ListUsersFromSelectedSource.RemoveAt(modelClass.ListUsersFromSelectedSource.IndexOf(objLinkedinUser));
                    SkippedUserCount++;
                }
            }
        }
        #endregion
    }
}