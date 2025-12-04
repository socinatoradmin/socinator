using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDLibrary.MessengerProcesses;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DominatorHouseCore.Enums.DHEnum;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public abstract class BaseLinkedinUserProcessor : BaseLinkedinProcessor
    {
        private readonly IDelayService _delayService;
        private readonly LdDataHelper _ldDataHelper;

        protected BaseLinkedinUserProcessor(ILdJobProcess ldJobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel ProcessScopeModel)
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, ProcessScopeModel)
        {
            _ldDataHelper = LdDataHelper.GetInstance;
            _delayService = delayService;
        }

        public string Get_d_flagship3_search_srp_people(string keyword)
        {
            try
            {
                string searchUrl;
                if (keyword.Contains("https:") || keyword.Contains("http:"))
                    searchUrl = keyword;
                else
                    searchUrl = "https://www.linkedin.com/search/results/people/?keywords=" + keyword +
                                "&origin=SWITCH_SEARCH_VERTICAL";

                var searchResponse = LdFunctions.GetRequestUpdatedUserAgent(searchUrl);
                searchResponse = HttpUtility.HtmlDecode(searchResponse);
                var dFlagship3SearchSrpPeople =
                    Utils.GetBetween(searchResponse, "d_flagship3_search_srp_people;", "\n");
                return dFlagship3SearchSrpPeople;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public string GetConstructedApiLinkedinUserSearch(string desktopUrl)
        {
            string api;
            try
            {
                #region Variable Initialization

                var keyword = string.Empty;
                var company = string.Empty;
                var currentCompany = string.Empty;
                var firstName = string.Empty;
                var lastName = string.Empty;
                var school = string.Empty;
                var title = string.Empty;

                #endregion

                desktopUrl = Uri.UnescapeDataString(desktopUrl);

                // selectedCriteria
                var selectedCriteria = Utils.GetBetween(desktopUrl, "people/?", "&origin");
                if (string.IsNullOrEmpty(selectedCriteria))
                    selectedCriteria = Utils.GetBetween(desktopUrl, "people/v2/?", "&origin");

                if (!string.IsNullOrEmpty(selectedCriteria))
                {
                    selectedCriteria = selectedCriteria.Replace("=[\"", "->");
                    selectedCriteria = selectedCriteria.Replace("\",\"", "|");
                    selectedCriteria = selectedCriteria.Replace("\"]", "");
                    selectedCriteria = selectedCriteria.Replace(">", "%3E").Replace(":", "%3A").Replace("|", "%7C")
                        .Replace("&", ",");
                }

                #region keyword

                if (desktopUrl.Contains("people/?keywords"))
                    keyword = Utils.GetBetween(desktopUrl, "people/?keywords=", "&");

                if (desktopUrl.Contains("results/all/?keywords"))
                    keyword = Utils.GetBetween(desktopUrl, "keywords=", "&");

                else if (desktopUrl.Contains("people/v2/?keywords"))
                    keyword = Utils.GetBetween(desktopUrl, "people/v2/?keywords=", "&");
                else if (desktopUrl.Contains("&keywords="))
                    keyword = Utils.GetBetween(desktopUrl, "&keywords=", "&");

                if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("people/?keywords") &&
                    selectedCriteria.Contains("keywords"))
                    keyword = Utils.GetBetween(desktopUrl, "people/?keywords=", "&");
                else if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("&keywords=") &&
                         !selectedCriteria.Contains("keywords"))
                    keyword = Utils.GetBetween(desktopUrl, "&keywords=", "&");

                #endregion

                #region firstName

                if (desktopUrl.Contains("people/?firstName"))
                    firstName = Utils.GetBetween(desktopUrl, "people/?firstName=", "&");
                else if (desktopUrl.Contains("people/v2/?firstName"))
                    firstName = Utils.GetBetween(desktopUrl, "people/v2/?firstName", "&");
                else if (desktopUrl.Contains("&firstName="))
                    firstName = Utils.GetBetween(desktopUrl, "&firstName=", "&");

                if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("people/?firstName") &&
                    selectedCriteria.Contains("firstName"))
                    firstName = Utils.GetBetween(desktopUrl, "people/?firstName=", "&");
                else if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("&firstName=") &&
                         !selectedCriteria.Contains("firstName"))
                    firstName = Utils.GetBetween(desktopUrl, "&firstName=", "&");

                if (!string.IsNullOrEmpty(firstName))
                    selectedCriteria += ",firstName-%3E" + firstName;

                #endregion

                #region lastName

                if (desktopUrl.Contains("people/?lastName"))
                    lastName = Utils.GetBetween(desktopUrl, "people/?lastName=", "&");

                if (desktopUrl.Contains("people/v2/?lastName"))
                    lastName = Utils.GetBetween(desktopUrl, "people/v2/?lastName=", "&");
                else if (desktopUrl.Contains("&lastName="))
                    lastName = Utils.GetBetween(desktopUrl, "&lastName=", "&");

                if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("people/?lastName") &&
                    selectedCriteria.Contains("lastName"))
                    lastName = Utils.GetBetween(desktopUrl, "people/?lastName=", "&");

                else if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("&lastName=") &&
                         !selectedCriteria.Contains("lastName"))
                    lastName = Utils.GetBetween(desktopUrl, "&lastName=", "&");

                if (!string.IsNullOrEmpty(lastName))
                    selectedCriteria += ",lastName-%3E" + lastName;

                #endregion

                #region title

                if (desktopUrl.Contains("people/?title"))
                    title = Utils.GetBetween(desktopUrl, "people/?title=", "&");
                else if (desktopUrl.Contains("people/v2/?title"))
                    title = Utils.GetBetween(desktopUrl, "people/v2/?title=", "&");
                else if (desktopUrl.Contains("&title="))
                    title = Utils.GetBetween(desktopUrl, "&title=", "&");

                if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("people/?title") &&
                    selectedCriteria.Contains("title"))
                    title = Utils.GetBetween(desktopUrl, "people/?title=", "&");
                else if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("&title=") &&
                         !selectedCriteria.Contains("title"))
                    title = Utils.GetBetween(desktopUrl, "&title=", "&");

                if (!string.IsNullOrEmpty(title))
                {
                    title = Uri.EscapeDataString(title);
                    selectedCriteria += ",title-%3E" + title;
                }

                #endregion

                #region company

                if (desktopUrl.Contains("people/?company"))
                    company = Utils.GetBetween(desktopUrl, "people/?company=", "&");
                else if (desktopUrl.Contains("people/v2/?company"))
                    company = Utils.GetBetween(desktopUrl, "people/v2/?company=", "&");
                else if (desktopUrl.Contains("&company="))
                    company = Utils.GetBetween(desktopUrl, "&company=", "&");

                if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("people/?company") &&
                    selectedCriteria.Contains("company"))
                    company = Utils.GetBetween(desktopUrl, "people/?company=", "&");
                else if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("&company=") &&
                         !selectedCriteria.Contains("company"))
                    company = Utils.GetBetween(desktopUrl, "&company=", "&");

                if (!string.IsNullOrEmpty(company))
                    selectedCriteria += ",company-%3E" + company;

                #endregion

                #region Current company

                if (desktopUrl.Contains("people/?facetCurrentCompany="))
                    currentCompany = Utils.GetBetween(desktopUrl, "people/?facetCurrentCompany=[\"", "\"]");
                else if (desktopUrl.Contains("&facetCurrentCompany="))
                    currentCompany = Utils.GetBetween(desktopUrl, "&facetCurrentCompany=", "&");

                if (!string.IsNullOrEmpty(currentCompany))
                    selectedCriteria += ",currentCompany-%3E" + currentCompany;

                #endregion

                #region school

                if (desktopUrl.Contains("people/?school"))
                    school = Utils.GetBetween(desktopUrl, "people/?school=", "&");
                else if (desktopUrl.Contains("people/v2/?school"))
                    school = Utils.GetBetween(desktopUrl, "people/v2/?school=", "&");
                else if (desktopUrl.Contains("&school="))
                    school = Utils.GetBetween(desktopUrl, "&school=", "&");

                if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("people/?school") &&
                    selectedCriteria.Contains("school"))
                    school = Utils.GetBetween(desktopUrl, "people/?school=", "&");
                else if (!string.IsNullOrEmpty(selectedCriteria) && desktopUrl.Contains("&school=") &&
                         !selectedCriteria.Contains("school"))
                    school = Utils.GetBetween(desktopUrl, "&school=", "&");

                if (!string.IsNullOrEmpty(school))
                    selectedCriteria += ",school-%3E" + school;

                #endregion

                #region ListItems

                var listItems = selectedCriteria;

                if (!string.IsNullOrEmpty(selectedCriteria) && selectedCriteria.Contains("keywords="))
                {
                    if (selectedCriteria.Contains(",keywords="))
                        listItems = selectedCriteria.Replace(",keywords=" + keyword, "");
                    else if (selectedCriteria.Contains("keywords="))
                        listItems = selectedCriteria.Replace("keywords=" + keyword, "");
                }

                listItems = listItems?.TrimStart(',').TrimEnd(',');

                #endregion

                #region Concatinating ListItems and Keyword along with generateNc to contruct API

                listItems = listItems?.Replace("facetGeoRegion", "geoRegion").Replace("facetNetwork", "network")
                    .Replace("facetProfileLanguage", "profileLanguage").Replace("facetCurrentCompany", "currentCompany")
                    .Replace("facetPastCompany", "pastCompany").Replace("facetSchool", "school")
                    .Replace("facetIndustry", "industry");
                if (string.IsNullOrEmpty(listItems))
                    api = LdConstants.SearchTypeApiConstantV2 +
                          "origin=FACETED_SEARCH&queryContext=List(spellCorrectionEnabled-%3Etrue,kcardTypes-%3EPROFILE)&q=all&filters=List(resultType-%3EPEOPLE)";
                else
                    api = LdConstants.SearchTypeApiConstantV2 +
                          "origin=FACETED_SEARCH&queryContext=List(spellCorrectionEnabled-%3Etrue,kcardTypes-%3EPROFILE)&q=all&filters=List(" +
                          listItems + ",resultType-%3EPEOPLE)";

                if (!string.IsNullOrEmpty(keyword))
                {
                    if (keyword.Contains("”") || keyword.Contains("“"))
                        keyword = keyword.Replace("”", "\"").Replace("“", "\"");
                    keyword = Uri.EscapeDataString(keyword);
                    api += "&keywords=" + keyword;
                }

                #endregion
            }
            catch (Exception ex)
            {
                api = null;
                ex.DebugLog();
            }

            return api;
        }

        public string GetConstructedApiToGetLinkedinUserByKeyword(string keyword, ActivityType activityType,int paginationCount=0)
        {
            try
            {

                var api = "";
                if (string.IsNullOrEmpty(keyword))
                    return api;

                if (keyword.Contains("”") || keyword.Contains("“"))
                    keyword = keyword.Replace("”", "\"").Replace("“", "\"");

                keyword = Uri.EscapeDataString(keyword);
                if (activityType == ActivityType.ConnectionRequest)
                    // it is for 2nd and 3rd degree 
                    api = IsBrowser
                        ? $"https://www.linkedin.com/search/results/people/?facetNetwork=%5B%22S%22%2C%22O%22%5D&keywords={keyword}&origin=FACETED_SEARCH"
                        : //$"{LdConstants.SearchTypeApiConstantV2}origin=FACETED_SEARCH&queryContext=List(spellCorrectionEnabled-%3Etrue,kcardTypes-%3EPROFILE)&q=all&filters=List(network-%3ES%7CO,resultType-%3EPEOPLE)&keywords={keyword}";
                        LdConstants.GetUserSearchAPI(keyword, paginationCount);
                else if (activityType == ActivityType.UserScraper)
                    // it is for all
                    api = IsBrowser
                        ? $"https://www.linkedin.com/search/results/people/?keywords={keyword}&origin=SWITCH_SEARCH_VERTICAL"
                        : //$"{LdConstants.SearchTypeApiConstantV2}origin=FACETED_SEARCH&queryContext=List(spellCorrectionEnabled-%3Etrue,kcardTypes-%3EPROFILE)&q=all&filters=List(resultType-%3EPEOPLE)&keywords={keyword}";
                        LdConstants.GetUserSearchAPI(keyword, paginationCount);

                else if (activityType == ActivityType.SalesNavigatorUserScraper)

                    api = IsBrowser
                          ? $"https://www.linkedin.com/sales/search/people?keywords={keyword}"
                          : GetSalesApiForKeyword(keyword,paginationCount);
                return api;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        private string GetSalesApiForKeyword(string keyword,int PaginationCount=0)
        {
            return $"https://www.linkedin.com/sales-api/salesApiLeadSearch?q=searchQuery&query=(spellCorrectionEnabled:true,recentSearchParam:(doLogHistory:true),keywords:{keyword})&start={PaginationCount}&count=25&decorationId=com.linkedin.sales.deco.desktop.searchv2.LeadSearchResult-14";
            //api = $"https://www.linkedin.com/sales-api/salesApiLeadSearch?q=searchQuery&query=(recentSearchParam:(doLogHistory:true),keywords:{keyword})&decorationId=com.linkedin.sales.deco.desktop.searchv2.LeadSearchResult-7";
        }

        public List<LinkedinUser> GetListUsers(List<Connections> lstConnections, string selectedOption)
        {
            try
            {
                var lstUsersBySoftware = new List<LinkedinUser>();
                if (lstConnections == null || lstConnections.Count <= 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"Sorry! no more connections found {selectedOption.ToString()}");
                    return lstUsersBySoftware;
                }

                if (ActivityType == ActivityType.RemoveConnections)
                    foreach (var currentElement in lstConnections)
                    {
                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var linkedinUser = new LinkedinUser();
                        ClassMapper.MapModelClass(currentElement, ref linkedinUser);
                        lstUsersBySoftware.Add(linkedinUser);
                    }

                if (ActivityType != ActivityType.ExportConnection && ActivityType != ActivityType.BroadcastMessages &&
                    ActivityType != ActivityType.ProfileEndorsement)
                    return lstUsersBySoftware;

                lstConnections.ForEach(z =>
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    lstUsersBySoftware.Add(new LinkedinUser
                    {
                        FullName = z.FullName,
                        ProfileId = z.ProfileId,
                        ProfileUrl = z.ProfileUrl,
                        HasAnonymousProfilePicture = z.HasAnonymousProfilePicture,
                        ProfilePicUrl = z.ProfilePicUrl,
                        ConnectedTimeStamp = z.ConnectedTimeStamp,
                        Occupation = z.Occupation,
                        CompanyName = z.CompanyName,
                        SelectedSource =
                            selectedOption == Application.Current.FindResource("LangKeyBySoftware")?.ToString()
                                ? Application.Current.FindResource("LangKeyBySoftware")?.ToString()
                                : Application.Current.FindResource("LangKeyOutsideSoftware")?.ToString(),
                        DetailedUserInfo = z.DetailedUserInfo,
                        PublicIdentifier = z.PublicIdentifier,
                    });
                });

                return lstUsersBySoftware;
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
        }

        public List<LinkedinUser> CustomUserList(List<string> lstCustomUser)
        {
            try
            {
                var lstCustomUserWithDetails = new List<LinkedinUser>();
                var lstCustomUserWithDetailsForRemoving = new List<LinkedinUser>();
                var lstCustomUserWithDetailsForWithdrawing = new List<LinkedinUser>();
                var connections = DbAccountService.GetConnections().ToList();
                switch (ActivityType)
                {
                    case ActivityType.RemoveConnections:

                        #region MyRegion

                        try
                        {
                            var lstRemovedConnections = DbAccountService.GetRemovedConnections().ToList();
                            var lstCustomUserForRemoving = lstCustomUser;
                            lstCustomUserForRemoving.RemoveAll(customConnectionUrl =>
                                lstRemovedConnections.Any(x => x.ProfileUrl == customConnectionUrl.Trim('/')));

                            #region LstCustomUserForRemoving(Contains connections for removing)

                            foreach (var profileUrl in lstCustomUserForRemoving)
                            {
                                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                #region Get Final LstCustomUser

                                Connections objConnections = null;
                                try
                                {
                                    var trimmedProfileUrl = profileUrl.Trim('/');
                                    objConnections = connections.FirstOrDefault(x => x.ProfileUrl == trimmedProfileUrl);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                if (objConnections == null)
                                    continue;
                                var objLinkedinUser = Utility.ClassMapper.Instance.MappedConnectionToLinkedInUser(objConnections);
                                if (!lstCustomUserWithDetailsForRemoving.Contains(objLinkedinUser))
                                    lstCustomUserWithDetailsForRemoving.Add(objLinkedinUser);

                                #endregion
                            }

                            #endregion

                            if (lstCustomUserWithDetailsForRemoving.Count > 0)
                                lstCustomUserWithDetails.AddRange(lstCustomUserWithDetailsForRemoving);
                        }
                        catch (Exception ex)
                        {
                            ex.ErrorLog(ex.Message);
                        }

                        #endregion

                        break;
                    case ActivityType.WithdrawConnectionRequest:
                    {
                        var lstCustomUserForWithdrawing = lstCustomUser.ToList();
                        var lstWithdrawnConnections = DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
                        lstCustomUserForWithdrawing.RemoveAll(customUserProfileUrl =>
                            lstWithdrawnConnections.Any(x => x.UserProfileUrl == customUserProfileUrl.Trim('/')));

                        #region LstCustomUser(Conatins Users that has been withdrawn request from )

                        foreach (var customUrl in lstCustomUserForWithdrawing)
                        {
                            LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            #region Get Final LstCustomUserWithDetails
                            InvitationsSent objInvitationsSent = null;
                            try
                            {
                                #region Get Pending Sent Invitation For Withdrawing
                                objInvitationsSent = DbAccountService.GetSingleInvitationSent(customUrl?.Trim('/'));

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            if (objInvitationsSent == null)

                                break;
                            var publicIdentifier = Utils.GetBetween(objInvitationsSent.ProfileUrl + "**", "in/", "**");
                            var objLinkedinUser = new LinkedinUser(publicIdentifier)
                            {
                                ProfileUrl = objInvitationsSent.ProfileUrl,
                                FullName = objInvitationsSent.FullName,
                                ProfileId = objInvitationsSent.ProfileId,
                                RequestedTimeStamp = objInvitationsSent.RequestedTimeStamp,
                                TrackingId = objInvitationsSent.TrackingId,
                                InvitationId = objInvitationsSent.InvitationId,
                                Occupation = objInvitationsSent.Occupation,
                                CompanyName = objInvitationsSent.CompanyName
                            };
                            if (!lstCustomUserWithDetailsForWithdrawing.Contains(objLinkedinUser))
                                lstCustomUserWithDetailsForWithdrawing.Add(objLinkedinUser);

                            #endregion
                        }

                        #endregion

                        if (lstCustomUserWithDetailsForWithdrawing.Count > 0)
                            lstCustomUserWithDetails.AddRange(lstCustomUserWithDetailsForWithdrawing);

                        break;
                    }

                    case ActivityType.ExportConnection:
                    case ActivityType.ProfileEndorsement:
                    case ActivityType.BroadcastMessages:
                    {
                        #region MyRegion

                        var selectedSource = string.Empty;
                        try
                        {
                            selectedSource = Application.Current.FindResource("LangKeyCustomUsersList")?.ToString();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        #endregion

                        var lstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();

                        if (ActivityType == ActivityType.BroadcastMessages)
                        {
                            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                            var linkedinConfig =
                                genericFileManager.GetModel<LinkedInModel>(
                                    ConstantVariable.GetOtherLinkedInSettingsFile());

                            if (linkedinConfig.IsFilterDuplicateMessageByCheckingConversationsHistory)
                                lstCustomUser.RemoveAll(customConnectionUrl =>
                                    lstInteractedUsers.Any(x => x.UserProfileUrl == customConnectionUrl.Trim('/')));
                        }
                        else
                        {
                            lstCustomUser.RemoveAll(customConnectionUrl =>
                                lstInteractedUsers.Any(x => x.UserProfileUrl == customConnectionUrl.Trim('/')));
                        }


                        #region LstCustomUser(Contains connections)

                        foreach (var profileUrl in lstCustomUser)
                        {
                            LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            #region Get Final LstCustomUser

                            Connections objConnections = null;

                            try
                            {
                                var trimmedProfileUrl = profileUrl.Trim('/');
                                objConnections = connections.FirstOrDefault(x => x.ProfileUrl == trimmedProfileUrl);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            try
                            {
                                var trimmedProfileUrl = profileUrl.Trim('/');
                                objConnections = connections.FirstOrDefault(x => x.ProfileUrl == trimmedProfileUrl);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            if (objConnections != null)
                            {
                                var publicIdentifier = Utils.GetBetween(objConnections.ProfileUrl + "**", "in/", "**");
                                var objLinkedinUser = new LinkedinUser(publicIdentifier)
                                {
                                    ProfileId = objConnections.ProfileId,
                                    ProfileUrl = objConnections.ProfileUrl,
                                    FullName = objConnections.FullName,
                                    HasAnonymousProfilePicture = objConnections.HasAnonymousProfilePicture,
                                    ProfilePicUrl = objConnections.ProfilePicUrl,
                                    ConnectedTimeStamp = objConnections.ConnectedTimeStamp,
                                    SelectedSource = selectedSource,
                                    Occupation = objConnections.Occupation
                                };

                                if (!lstCustomUserWithDetails.Contains(objLinkedinUser))
                                    lstCustomUserWithDetails.Add(objLinkedinUser);
                            }
                            else
                            {
                                var objLinkedinUser = GetUserInformation(profileUrl, false);
                                objLinkedinUser.SelectedSource = selectedSource;
                                if (!lstCustomUserWithDetails.Contains(objLinkedinUser))
                                    lstCustomUserWithDetails.Add(objLinkedinUser);
                            }

                            #endregion
                        }

                        #endregion

                        break;
                    }
                }

                return lstCustomUserWithDetails;
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
        }

        public void FurtherProcessLinkedinUsersFromUserList(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinUser> lstLinkedinUser)
        {
            try
            {
                var finalListLinkedinUser = new List<LinkedinUser>();
                var count = 0;
                var perJobCount = LdJobProcess.ModuleSetting.JobConfiguration.ActivitiesPerJob.EndValue + 10;

                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "Searching for users (Probably take longer time by checking conversation history).");
                foreach (var linkedinUser in lstLinkedinUser)
                {
                    ++count;
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (FilterMessage(linkedinUser))
                        continue;
                    finalListLinkedinUser.Add(linkedinUser);
                    // if we get perJobCount then break loop go for further process
                    if (finalListLinkedinUser.Count > perJobCount)
                        break;
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Filtered Users {count}  by checking conversation history.");

                ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, finalListLinkedinUser);
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                ex.DebugLog();
            }
        }


        public void ProcessLinkedinUsersFromUserList(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinUser> lstLinkedinUser)
        {
            try
            {
                foreach (var linkedinUser in lstLinkedinUser)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    SendToPerformActivity(ref jobProcessResult, linkedinUser, queryInfo);
                    
                }

                // dispose browser if work is done
                // view profile also uses browser 
                if (ActivityType.Equals(ActivityType.ExportConnection) || LdJobProcess.ModuleSetting.IsViewProfileUsingEmbeddedBrowser)
                    LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                ex.DebugLog();
            }
        }

        private bool FilterMessage(LinkedinUser linkedinUser)
        {
            var isFiltered = false;
            if (!LdJobProcess.ActivityType.Equals(ActivityType.BroadcastMessages))
                return isFiltered;
            try
            {
                // check from db
                var userList = DbAccountService.GetInteractedUsers(ActivityType.BroadcastMessages.ToString()).ToList();
                userList.AddRange(DbAccountService.GetInteractedUsers($"{LdJobProcess.ActivityType.ToString()}_Scrap"));

                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var BroadcastMessagesModel =
                    JsonConvert.DeserializeObject<BroadcastMessagesModel>(
                        templatesFileManager.Get()
                            .FirstOrDefault(x => x.Id == LdJobProcess.TemplateId).ActivitySettings);

                var objFilterMessage = new FilterMessage(BroadcastMessagesModel, _delayService);
                objFilterMessage.IsShowFilterMessage = false;
                var imageSource = "";
                var message = objFilterMessage.GetMessage(DominatorAccountModel, linkedinUser, ActivityTypeString,
                    LdFunctions, ref imageSource);
                isFiltered = userList.Any(x => x.ProfileId == linkedinUser.ProfileId && x.DetailedUserInfo == message);
                if (isFiltered)
                    return isFiltered;

                isFiltered = objFilterMessage.FilterMessageFromConversationHistory(LdFunctions, linkedinUser,
                    DominatorAccountModel, ActivityType, message);

                if (isFiltered)
                {
                    
                    var interactedUsers = new InteractedUsers
                    {
                        ActivityType = $"{ActivityType.BroadcastMessages}_Scrap",
                        ProfileId = linkedinUser.ProfileId,
                        UserProfileUrl = linkedinUser.ProfileUrl,
                        DetailedUserInfo = message
                    };
                    DbAccountService.Add(interactedUsers);
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isFiltered;
        }

        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, LinkedinUser linkedinUser,
            QueryInfo queryInfo)
        {
            try
            {
                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultUser = linkedinUser,
                    QueryInfo = queryInfo
                });
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddCustomUrlToList(string profileUrl, List<InteractedUsers> interactedUser)
        {
            interactedUser.Add(new InteractedUsers
            {
                UserProfileUrl = !string.IsNullOrEmpty(profileUrl) && profileUrl.Contains("/sales/lead/")? profileUrl:
                    $"https://www.linkedin.com/in/{_ldDataHelper.GetPublicInstanceFromProfileUrl(profileUrl)}"
            });
        }

        public void AddCustomUrlToList(string profileUrl, List<LinkedinUser> interactedUser)
        {
            var publicIdentifier = _ldDataHelper.GetPublicInstanceFromProfileUrl(profileUrl);
            interactedUser.Add(new LinkedinUser
            {
                ProfileUrl = $"https://www.linkedin.com/in/{publicIdentifier}",
                PublicIdentifier=publicIdentifier
            });
        }


        public bool RemoveOrSkipAlreadyInteractedUsers(List<InteractedUsers> interactedUsers)
        {
            try
            {
                var SkippedUserCount = 0;
                if (string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId) ||
                    ActivityType == ActivityType.ConnectionRequest)
                {
                    var listInteractedUsersFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString)
                        .Select(x => new KeyValuePair<string, string>(x?.ProfileId, x?.UserProfileUrl)).ToList();
                    SkippedUserCount = interactedUsers.RemoveAll(x => listInteractedUsersFromAccountDb.Any(y =>
                        !string.IsNullOrEmpty(y.Key) && y.Key == x.ProfileId ||
                        !string.IsNullOrEmpty(y.Value) && y.Value.Contains(x.UserProfileUrl)));
                }
                else
                {
                    var listInteractedUsersFromCampaignDb = DbCampaignService.GetInteractedUsers(ActivityTypeString)
                        .Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserProfileUrl)).ToList();
                    SkippedUserCount = interactedUsers.RemoveAll(x => listInteractedUsersFromCampaignDb.Any(y =>
                        !string.IsNullOrEmpty(y.Key) && y.Key == x.ProfileId ||
                        !string.IsNullOrEmpty(y.Value) && y.Value.Contains(x.UserProfileUrl)));
                }
                if(SkippedUserCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn, DominatorAccountModel.UserName,
                        UserType.BlackListedUser, $"Skipped Interacted {SkippedUserCount} Users.");
                if (interactedUsers.Count <= 0)
                    return true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }

        public bool RemoveOrSkipAlreadyInteractedLinkedInUsers(List<LinkedinUser> interactedLinkedInUsers)
        {
            try
            {
                //since we can send connection request only a user therefore here also taking connections request
                if (string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId) ||
                    ActivityType.Equals(ActivityType.ConnectionRequest) ||
                    ActivityType.Equals(ActivityType.SendMessageToNewConnection))
                {
                    var listInteractedUsersFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString)
                        .Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserProfileUrl)).ToList();
                    interactedLinkedInUsers.RemoveAll(x =>
                        listInteractedUsersFromAccountDb.Any(y =>
                            !string.IsNullOrEmpty(y.Key) && y.Key == x.ProfileId ||
                            !string.IsNullOrEmpty(y.Value) && y.Value.Contains(x.ProfileUrl)));
                }
                else
                {
                    var listInteractedUsersFromCampaignDb = DbCampaignService.GetInteractedUsers(ActivityTypeString)
                        .Select(x => new KeyValuePair<string, string>(x.ProfileId, x.UserProfileUrl)).ToList();

                    interactedLinkedInUsers.RemoveAll(x =>
                        listInteractedUsersFromCampaignDb.Any(y =>
                            !string.IsNullOrEmpty(y.Key) && y.Key == x.ProfileId ||
                            !string.IsNullOrEmpty(y.Value) && y.Value.Contains(x.ProfileUrl)));
                }


                if (interactedLinkedInUsers.Count <= 0)
                    return true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }
        /// <summary>
        /// This method is use to remove users who already send message from all messager module through software.
        /// </summary>
        /// <param name="interactedUSers"></param>
        /// <returns></returns>
        public bool RemoveOrSkipAlreadySendMessageToUsers(List<LinkedinUser> interactedUSers)
        {
            try
            {
                var lstAccountDb = DbAccountService.Get<InteractedUsers>(x=>
                x.ActivityType== "AutoReplyToNewMessage" ||
                x.ActivityType== "SendMessageToNewConnection" ||
                x.ActivityType== "SendGreetingsToConnections"
                );                                
                interactedUSers.RemoveAll(x => lstAccountDb.Any(y =>
                         y.PublicIdentifer == x.PublicIdentifier ||
                         y.ProfileId == x.ProfileId||
                         y.UserProfileUrl == x.ProfileUrl
                       ));
                if (interactedUSers.Count <= 0)
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        public void MapConnectionToLinkedInUsers(List<LinkedinUser> userList, List<Connections> connections = null)
        {
            if (connections == null)
                connections = DbAccountService.GetConnections().ToList();

            var tempUserList = new List<LinkedinUser>();
            ClassMapper.MapListOfModelClass(connections, ref tempUserList);
            userList.AddRange(tempUserList);
        }
    }
}